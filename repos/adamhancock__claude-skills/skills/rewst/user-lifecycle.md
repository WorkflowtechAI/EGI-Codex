# Rewst User Lifecycle Automation

Complete patterns for user provisioning, modifications, and offboarding.

---

## User Provisioning Overview

### Standard Onboarding Flow

```
Form Submission
     │
     ▼
┌─────────────────┐
│ Validate Input  │
│ Check conflicts │
└────────┬────────┘
         │
         ▼
┌─────────────────┐     ┌─────────────────┐
│ Create M365     │────►│ Assign Licenses │
│ User Account    │     └────────┬────────┘
└─────────────────┘              │
                                 ▼
                    ┌─────────────────────┐
                    │ Add to Groups       │
                    │ (Security + M365)   │
                    └──────────┬──────────┘
                               │
         ┌─────────────────────┼─────────────────────┐
         ▼                     ▼                     ▼
┌─────────────┐      ┌─────────────┐       ┌─────────────┐
│ Create PSA  │      │ Update Docs │       │ Configure   │
│ Contact     │      │ (IT Glue)   │       │ Email/DLs   │
└──────┬──────┘      └──────┬──────┘       └──────┬──────┘
       │                    │                     │
       └────────────────────┼─────────────────────┘
                            ▼
                  ┌─────────────────┐
                  │ Send Welcome    │
                  │ Notification    │
                  └─────────────────┘
```

---

## Username Generation

### Common Formats

```jinja
{# Format: first.last #}
{% set username = (CTX.first_name ~ "." ~ CTX.last_name) | lower | regex_replace("[^a-z.]", "") %}

{# Format: flast (first initial + last name) #}
{% set username = (CTX.first_name[0] ~ CTX.last_name) | lower | regex_replace("[^a-z]", "") %}

{# Format: firstl (first name + last initial) #}
{% set username = (CTX.first_name ~ CTX.last_name[0]) | lower | regex_replace("[^a-z]", "") %}

{# Format: firstmlast (first + middle initial + last) #}
{% set middle = CTX.middle_name[0] if CTX.middle_name else "" %}
{% set username = (CTX.first_name ~ middle ~ CTX.last_name) | lower | regex_replace("[^a-z]", "") %}
```

### Format from Org Variable

```jinja
{# Org variable: username_format = "flast" #}
{% set format = ORG.VARIABLES.username_format | d("first.last") %}

{% if format == "flast" %}
    {% set username = (CTX.first_name[0] ~ CTX.last_name) | lower %}
{% elif format == "firstl" %}
    {% set username = (CTX.first_name ~ CTX.last_name[0]) | lower %}
{% elif format == "first.last" %}
    {% set username = (CTX.first_name ~ "." ~ CTX.last_name) | lower %}
{% else %}
    {% set username = (CTX.first_name ~ CTX.last_name) | lower %}
{% endif %}

{{ username | regex_replace("[^a-z.]", "") }}
```

### Conflict Resolution

```jinja
{# Check if username exists, add number if needed #}
{% set base_username = CTX.generated_username %}
{% set existing_users = CTX.all_usernames | d([]) %}
{% set final_username = base_username %}

{% if base_username in existing_users %}
    {% set ns = namespace(counter=1, found=false) %}
    {% for i in range(1, 100) %}
        {% if not ns.found %}
            {% set test_name = base_username ~ i | string %}
            {% if test_name not in existing_users %}
                {% set ns.found = true %}
                {% set final_username = test_name %}
            {% endif %}
            {% set ns.counter = ns.counter + 1 %}
        {% endif %}
    {% endfor %}
{% endif %}

{{ final_username }}
```

---

## M365 User Creation

### User Creation Payload

```jinja
{
    "accountEnabled": true,
    "displayName": "{{ CTX.first_name }} {{ CTX.last_name }}",
    "givenName": "{{ CTX.first_name }}",
    "surname": "{{ CTX.last_name }}",
    "userPrincipalName": "{{ CTX.username }}@{{ ORG.VARIABLES.email_domain }}",
    "mailNickname": "{{ CTX.username }}",
    "mail": "{{ CTX.username }}@{{ ORG.VARIABLES.email_domain }}",
    "jobTitle": "{{ CTX.job_title | d("") }}",
    "department": "{{ CTX.department | d("") }}",
    "companyName": "{{ ORG.VARIABLES.company_name | d("") }}",
    "officeLocation": "{{ CTX.office | d("") }}",
    "usageLocation": "{{ ORG.VARIABLES.m365_usage_location | d("US") }}",
    "passwordProfile": {
        "password": "{{ CTX.temp_password }}",
        "forceChangePasswordNextSignIn": true,
        "forceChangePasswordNextSignInWithMfa": false
    },
    "passwordPolicies": "DisablePasswordExpiration"
}
```

### With Manager Assignment

```jinja
{# After user creation, set manager #}
{% if CTX.manager_id %}
PUT /users/{{ CTX.new_user_id }}/manager/$ref

{
    "@odata.id": "https://graph.microsoft.com/v1.0/users/{{ CTX.manager_id }}"
}
{% endif %}
```

### Extended User Properties

```jinja
{# Update additional properties after creation #}
PATCH /users/{{ CTX.user_id }}

{
    "employeeId": "{{ CTX.employee_id | d("") }}",
    "employeeHireDate": "{{ CTX.start_date }}T00:00:00Z",
    "employeeType": "{{ CTX.employee_type | d("Employee") }}",
    "streetAddress": "{{ CTX.address | d("") }}",
    "city": "{{ CTX.city | d("") }}",
    "state": "{{ CTX.state | d("") }}",
    "postalCode": "{{ CTX.zip | d("") }}",
    "mobilePhone": "{{ CTX.mobile | d("") }}",
    "businessPhones": {{ [CTX.business_phone] if CTX.business_phone else [] | to_json_string }}
}
```

---

## License Assignment

### Assign Single License

```jinja
POST /users/{{ CTX.user_id }}/assignLicense

{
    "addLicenses": [
        {
            "skuId": "{{ CTX.license_sku_id }}"
        }
    ],
    "removeLicenses": []
}
```

### Assign Multiple Licenses

```jinja
{
    "addLicenses": [
        {% for license in CTX.licenses_to_add %}
        {
            "skuId": "{{ license.sku_id }}"
            {% if license.disabled_plans %}
            ,"disabledPlans": {{ license.disabled_plans | to_json_string }}
            {% endif %}
        }{{ "," if not loop.last }}
        {% endfor %}
    ],
    "removeLicenses": []
}
```

### License by Role/Department

```jinja
{# Map roles to licenses #}
{% set license_map = {
    "Standard": ["c5928f49-12ba-48f7-ada3-0d743a3601d5"],  {# M365 BP Basic #}
    "Executive": [
        "c5928f49-12ba-48f7-ada3-0d743a3601d5",
        "efccb6f7-5641-4e0e-bd10-b4976e1bf68e"  {# + E5 EMS #}
    ],
    "Developer": [
        "c5928f49-12ba-48f7-ada3-0d743a3601d5",
        "b05e124f-c7cc-45a0-a6aa-8cf78c946968"  {# + Enterprise Dev #}
    ]
} %}

{% set licenses = license_map[CTX.user_role] | d(license_map["Standard"]) %}
```

### Check License Availability

```jinja
{# Before assigning, verify license is available #}
{% set sku = CTX.sku_info %}
{% set available = sku.prepaidUnits.enabled - sku.consumedUnits %}

{% if available < 1 %}
    {# No licenses available - alert or fail #}
{% endif %}
```

---

## Group Membership

### Add to Security Groups

```jinja
{# Add user to multiple groups #}
{% for group_id in CTX.group_ids %}
POST /groups/{{ group_id }}/members/$ref

{
    "@odata.id": "https://graph.microsoft.com/v1.0/directoryObjects/{{ CTX.user_id }}"
}
{% endfor %}
```

### Role-Based Group Assignment

```jinja
{# Map departments to groups #}
{% set dept_groups = {
    "IT": ["it-staff", "helpdesk-access"],
    "Sales": ["sales-team", "crm-users"],
    "Finance": ["finance-team", "accounting-access"],
    "HR": ["hr-team", "employee-records"]
} %}

{% set base_groups = ORG.VARIABLES.default_groups | d([]) %}
{% set role_groups = dept_groups[CTX.department] | d([]) %}
{% set all_groups = base_groups + role_groups %}
```

### Distribution List Membership

```jinja
{# Add to distribution groups #}
POST /groups/{{ CTX.dl_group_id }}/members/$ref

{
    "@odata.id": "https://graph.microsoft.com/v1.0/directoryObjects/{{ CTX.user_id }}"
}
```

---

## Email Configuration

### Mailbox Settings

```jinja
PATCH /users/{{ CTX.user_id }}/mailboxSettings

{
    "automaticRepliesSetting": {
        "status": "disabled"
    },
    "language": {
        "locale": "en-US"
    },
    "timeZone": "{{ ORG.VARIABLES.default_timezone | d("Eastern Standard Time") }}",
    "dateFormat": "M/d/yyyy",
    "timeFormat": "h:mm tt"
}
```

### Email Signature (via EWS or Graph)

```jinja
{# Set HTML signature #}
{% set signature_html %}
<div style="font-family: Arial, sans-serif;">
    <strong>{{ CTX.display_name }}</strong><br>
    {{ CTX.job_title }}<br>
    {{ ORG.VARIABLES.company_name }}<br>
    <a href="mailto:{{ CTX.email }}">{{ CTX.email }}</a><br>
    {{ CTX.phone | d("") }}
</div>
{% endset %}
```

### Add Email Alias

```jinja
{# Add alternate email address #}
PATCH /users/{{ CTX.user_id }}

{
    "proxyAddresses": {{ (CTX.current_proxies + ["smtp:" ~ CTX.alias]) | to_json_string }}
}
```

---

## PSA Contact Creation

### ConnectWise Contact

```jinja
{
    "firstName": "{{ CTX.first_name }}",
    "lastName": "{{ CTX.last_name }}",
    "company": {"id": {{ CTX.psa_company_id }}},
    "site": {"id": {{ CTX.psa_site_id | d("null") }}},
    "title": "{{ CTX.job_title | d("") }}",
    "department": {"id": {{ CTX.psa_department_id | d("null") }}},
    "defaultPhoneType": "Direct",
    "defaultPhoneNbr": "{{ CTX.phone | d("") }}",
    "communicationItems": [
        {
            "type": {"id": 1, "name": "Email"},
            "value": "{{ CTX.email }}",
            "defaultFlag": true
        }
        {% if CTX.mobile %}
        ,{
            "type": {"id": 3, "name": "Cell"},
            "value": "{{ CTX.mobile }}",
            "defaultFlag": false
        }
        {% endif %}
    ],
    "portalSecurityLevel": {{ CTX.portal_level | d(3) }},
    "disablePortalLoginFlag": {{ (not CTX.enable_portal) | d(true) | lower }}
}
```

### Autotask Contact

```jinja
{
    "FirstName": "{{ CTX.first_name }}",
    "LastName": "{{ CTX.last_name }}",
    "CompanyID": {{ CTX.company_id }},
    "Title": "{{ CTX.job_title | d("") }}",
    "EMailAddress": "{{ CTX.email }}",
    "Phone": "{{ CTX.phone | d("") }}",
    "MobilePhone": "{{ CTX.mobile | d("") }}",
    "Active": 1
}
```

### HaloPSA Contact

```jinja
{
    "name": "{{ CTX.first_name }} {{ CTX.last_name }}",
    "firstname": "{{ CTX.first_name }}",
    "surname": "{{ CTX.last_name }}",
    "client_id": {{ CTX.client_id }},
    "site_id": {{ CTX.site_id | d("null") }},
    "emailaddress": "{{ CTX.email }}",
    "phonenumber": "{{ CTX.phone | d("") }}",
    "mobilenumber": "{{ CTX.mobile | d("") }}",
    "jobtitle": "{{ CTX.job_title | d("") }}"
}
```

---

## User Modification Patterns

### Update User Properties

```jinja
{# Bulk update user attributes #}
PATCH /users/{{ CTX.user_id }}

{
    {% set updates = {} %}
    {% if CTX.new_title %}{% set updates = updates | combine({"jobTitle": CTX.new_title}) %}{% endif %}
    {% if CTX.new_department %}{% set updates = updates | combine({"department": CTX.new_department}) %}{% endif %}
    {% if CTX.new_manager %}{% set updates = updates | combine({"manager": CTX.new_manager}) %}{% endif %}
    {{ updates | to_json_string }}
}
```

### Department Transfer

```jinja
{# 1. Update department attribute #}
PATCH /users/{{ CTX.user_id }}
{"department": "{{ CTX.new_department }}"}

{# 2. Remove from old department groups #}
{% for group_id in CTX.old_dept_groups %}
DELETE /groups/{{ group_id }}/members/{{ CTX.user_id }}/$ref
{% endfor %}

{# 3. Add to new department groups #}
{% for group_id in CTX.new_dept_groups %}
POST /groups/{{ group_id }}/members/$ref
{"@odata.id": "https://graph.microsoft.com/v1.0/users/{{ CTX.user_id }}"}
{% endfor %}

{# 4. Update manager if changed #}
{% if CTX.new_manager_id %}
PUT /users/{{ CTX.user_id }}/manager/$ref
{"@odata.id": "https://graph.microsoft.com/v1.0/users/{{ CTX.new_manager_id }}"}
{% endif %}
```

### Name Change

```jinja
{# Update display name, UPN may change too #}
PATCH /users/{{ CTX.user_id }}

{
    "givenName": "{{ CTX.new_first_name }}",
    "surname": "{{ CTX.new_last_name }}",
    "displayName": "{{ CTX.new_first_name }} {{ CTX.new_last_name }}",
    "mailNickname": "{{ CTX.new_username }}"
}

{# If UPN changes, also update: #}
{# - Email aliases (keep old as alias) #}
{# - PSA contact #}
{# - Documentation #}
```

---

## User Offboarding

### Standard Offboarding Flow

```
Termination Request
        │
        ▼
┌─────────────────┐
│ Manager Approval│
│ (if required)   │
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ Block Sign-in   │◄── Immediate
│ Revoke Sessions │
└────────┬────────┘
         │
         ▼
┌─────────────────────────────────┐
│ Email Forward/Delegation        │
│ Out-of-Office                   │
│ Shared Mailbox Conversion       │
└────────────────┬────────────────┘
                 │
    ┌────────────┼────────────────┐
    ▼            ▼                ▼
┌────────┐  ┌────────────┐  ┌─────────────┐
│ Remove │  │ Transfer   │  │ Revoke      │
│ Groups │  │ OneDrive   │  │ Licenses    │
└───┬────┘  └─────┬──────┘  └──────┬──────┘
    │             │                │
    └─────────────┼────────────────┘
                  ▼
         ┌─────────────────┐
         │ Update PSA/Docs │
         │ Create Ticket   │
         └────────┬────────┘
                  │
                  ▼
         ┌─────────────────┐
         │ Schedule Delete │
         │ (30 days)       │
         └─────────────────┘
```

### Block Sign-in

```jinja
PATCH /users/{{ CTX.user_id }}

{
    "accountEnabled": false
}
```

### Revoke All Sessions

```jinja
POST /users/{{ CTX.user_id }}/revokeSignInSessions
```

### Set Out-of-Office

```jinja
PATCH /users/{{ CTX.user_id }}/mailboxSettings

{
    "automaticRepliesSetting": {
        "status": "alwaysEnabled",
        "externalReplyMessage": "{{ CTX.external_message | escape }}",
        "internalReplyMessage": "{{ CTX.internal_message | escape }}"
    }
}
```

### Forward Email to Manager

```jinja
{# Create inbox rule to forward #}
POST /users/{{ CTX.user_id }}/mailFolders/inbox/messageRules

{
    "displayName": "Forward to Manager - Offboarding",
    "sequence": 1,
    "isEnabled": true,
    "conditions": {},
    "actions": {
        "forwardTo": [
            {
                "emailAddress": {
                    "address": "{{ CTX.manager_email }}"
                }
            }
        ]
    }
}
```

### Remove All Group Memberships

```jinja
{# Get all groups first #}
GET /users/{{ CTX.user_id }}/memberOf?$select=id,displayName

{# Remove from each #}
{% for group in CTX.user_groups %}
DELETE /groups/{{ group.id }}/members/{{ CTX.user_id }}/$ref
{% endfor %}
```

### Revoke Licenses

```jinja
{# Get current licenses #}
{% set current_skus = CTX.user.assignedLicenses | map(attribute="skuId") | list %}

POST /users/{{ CTX.user_id }}/assignLicense

{
    "addLicenses": [],
    "removeLicenses": {{ current_skus | to_json_string }}
}
```

### Transfer OneDrive to Manager

```jinja
{# Grant manager access to OneDrive #}
{# Note: OneDrive URL format varies #}
{% set onedrive_url = "https://" ~ ORG.VARIABLES.tenant_name ~ "-my.sharepoint.com/personal/" ~ CTX.username | replace(".", "_") ~ "_" ~ ORG.VARIABLES.email_domain | replace(".", "_") %}

{# Add manager as site collection admin via SharePoint #}
```

### Convert to Shared Mailbox

```jinja
{# Via Exchange Online PowerShell or Graph #}
{# Requires Exchange admin permissions #}

{# After conversion, can remove license #}
```

### Hide from GAL

```jinja
{# Hide from address list #}
PATCH /users/{{ CTX.user_id }}

{
    "showInAddressList": false
}
```

---

## Bulk Operations

### Bulk User Import

```jinja
{# Process CSV data #}
{% for row in CTX.csv_data %}
{
    "displayName": "{{ row.FirstName }} {{ row.LastName }}",
    "userPrincipalName": "{{ row.Email }}",
    "mailNickname": "{{ row.Email.split('@')[0] }}",
    ...
}
{% endfor %}

{# Use With Items with concurrency 5 for creation #}
```

### Bulk License Assignment

```jinja
{# Users needing license #}
{% set users_need_license = [
    u for u in CTX.all_users
    if CTX.target_sku not in (u.assignedLicenses | map(attribute="skuId") | list)
] %}

{# Assign via With Items #}
With Items: {{ users_need_license }}
Concurrency: 10
```

### Bulk Group Membership

```jinja
{# Add multiple users to group at once #}
{# Graph supports batch of 20 per request #}

{% for batch in CTX.user_ids | batch(20) %}
{
    "members@odata.bind": [
        {% for user_id in batch %}
        "https://graph.microsoft.com/v1.0/directoryObjects/{{ user_id }}"{{ "," if not loop.last }}
        {% endfor %}
    ]
}
{% endfor %}
```

---

## Audit & Compliance

### User Creation Audit Log

```jinja
{
    "action": "user_created",
    "timestamp": "{{ now() | format_datetime('%Y-%m-%dT%H:%M:%SZ') }}",
    "performer": "{{ CTX.requester_email }}",
    "target_user": "{{ CTX.new_user_upn }}",
    "details": {
        "department": "{{ CTX.department }}",
        "manager": "{{ CTX.manager_upn }}",
        "licenses": {{ CTX.assigned_licenses | to_json_string }},
        "groups": {{ CTX.assigned_groups | to_json_string }}
    },
    "workflow_execution_id": "{{ CTX.__execution_id__ | d('') }}",
    "organization": "{{ ORG.ATTRIBUTES.name }}"
}
```

### Offboarding Compliance Checklist

```jinja
{
    "user": "{{ CTX.user_upn }}",
    "termination_date": "{{ CTX.term_date }}",
    "checklist": {
        "sign_in_blocked": {{ CTX.sign_in_blocked | d(false) | lower }},
        "sessions_revoked": {{ CTX.sessions_revoked | d(false) | lower }},
        "licenses_removed": {{ CTX.licenses_removed | d(false) | lower }},
        "groups_removed": {{ CTX.groups_removed | d(false) | lower }},
        "email_forwarded": {{ CTX.email_forwarded | d(false) | lower }},
        "onedrive_transferred": {{ CTX.onedrive_transferred | d(false) | lower }},
        "psa_updated": {{ CTX.psa_updated | d(false) | lower }},
        "docs_updated": {{ CTX.docs_updated | d(false) | lower }},
        "ticket_created": {{ CTX.ticket_created | d(false) | lower }}
    },
    "completed_at": "{{ now() | format_datetime('%Y-%m-%dT%H:%M:%SZ') }}",
    "completed_by": "Rewst Automation"
}
```
