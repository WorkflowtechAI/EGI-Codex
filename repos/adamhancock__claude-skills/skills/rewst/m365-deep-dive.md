# Microsoft 365 Deep Dive

Comprehensive M365/Graph API patterns for Rewst automation.

---

## License SKU Reference

### Common Business SKUs

| SKU Part Number | Display Name | GUID |
|-----------------|--------------|------|
| `ENTERPRISEPACK` | Office 365 E3 | `6fd2c87f-b296-42f0-b197-1e91e994b900` |
| `ENTERPRISEPREMIUM` | Office 365 E5 | `c7df2760-2c81-4ef7-b578-5b5392b571df` |
| `SPE_E3` | Microsoft 365 E3 | `05e9a617-0261-4cee-bb44-138d3ef5d965` |
| `SPE_E5` | Microsoft 365 E5 | `06ebc4ee-1bb5-47dd-8120-11324bc54e06` |
| `BUSINESS_BASIC` | Microsoft 365 Business Basic | `dab7782a-93b1-4074-8bb1-0e61318bea0b` |
| `BUSINESS_STANDARD` | Microsoft 365 Business Standard | `f245ecc8-75af-4f8e-b61f-27d8114de5f3` |
| `BUSINESS_PREMIUM` | Microsoft 365 Business Premium | `cbdc14ab-d96c-4c30-b9f4-6ada7cdc1d46` |
| `EXCHANGESTANDARD` | Exchange Online Plan 1 | `4b9405b0-7788-4568-add1-99614e613b69` |
| `EXCHANGEENTERPRISE` | Exchange Online Plan 2 | `19ec0d23-8335-4cbd-94ac-6050e30712fa` |
| `POWER_BI_PRO` | Power BI Pro | `f8a1db68-be16-40ed-86d5-cb42ce701560` |
| `PROJECTPREMIUM` | Project Plan 5 | `09015f9f-377f-4538-bbb5-f75ceb09358a` |
| `VISIOCLIENT` | Visio Plan 2 | `c5928f49-12ba-48f7-ada3-0d743a3601d5` |
| `EMS` | Enterprise Mobility + Security E3 | `efccb6f7-5641-4e0e-bd10-b4976e1bf68e` |
| `EMSPREMIUM` | Enterprise Mobility + Security E5 | `b05e124f-c7cc-45a0-a6aa-8cf78c946968` |
| `AAD_PREMIUM` | Azure AD Premium P1 | `078d2b04-f1bd-4111-bbd4-b4b1b354cef4` |
| `AAD_PREMIUM_P2` | Azure AD Premium P2 | `84a661c4-e949-4bd2-a560-ed7766fcaf2b` |
| `ATP_ENTERPRISE` | Defender for Office 365 P1 | `4ef96642-f096-40de-a3e9-d83fb2f90211` |
| `THREAT_INTELLIGENCE` | Defender for Office 365 P2 | `8e0c0a52-6a6c-4d40-8370-dd62790dcd70` |

### Checking License Assignment

```jinja
{# Get user's assigned license GUIDs #}
{% set user_skus = CTX.user.assignedLicenses | map(attribute="skuId") | list %}

{# Check for specific license #}
{{ "06ebc4ee-1bb5-47dd-8120-11324bc54e06" in user_skus }}

{# Check by SKU part number (need SKU list) #}
{% set has_e5 = CTX.tenant_skus | selectattr("skuPartNumber", "eq", "SPE_E5") | selectattr("skuId", "in", user_skus) | list | length > 0 %}
```

### License Assignment Body

```jinja
{
    "addLicenses": [
        {
            "skuId": "{{ CTX.sku_to_add }}",
            "disabledPlans": {{ CTX.disabled_services | d([]) | to_json_string }}
        }
    ],
    "removeLicenses": {{ CTX.skus_to_remove | d([]) | to_json_string }}
}
```

---

## Common Graph API Endpoints

### Users

| Operation | Method | Endpoint |
|-----------|--------|----------|
| List users | GET | `/users` |
| Get user | GET | `/users/{id}` |
| Create user | POST | `/users` |
| Update user | PATCH | `/users/{id}` |
| Delete user | DELETE | `/users/{id}` |
| Get manager | GET | `/users/{id}/manager` |
| Set manager | PUT | `/users/{id}/manager/$ref` |
| Get direct reports | GET | `/users/{id}/directReports` |
| Get photo | GET | `/users/{id}/photo/$value` |
| Revoke sign-in sessions | POST | `/users/{id}/revokeSignInSessions` |

### Groups

| Operation | Method | Endpoint |
|-----------|--------|----------|
| List groups | GET | `/groups` |
| Get group | GET | `/groups/{id}` |
| Create group | POST | `/groups` |
| Add member | POST | `/groups/{id}/members/$ref` |
| Remove member | DELETE | `/groups/{id}/members/{userId}/$ref` |
| List members | GET | `/groups/{id}/members` |
| Check membership | POST | `/users/{id}/checkMemberGroups` |

### Mailboxes

| Operation | Method | Endpoint |
|-----------|--------|----------|
| Get mailbox settings | GET | `/users/{id}/mailboxSettings` |
| Update mailbox settings | PATCH | `/users/{id}/mailboxSettings` |
| Get auto-reply | GET | `/users/{id}/mailboxSettings/automaticRepliesSetting` |
| Set auto-reply | PATCH | `/users/{id}/mailboxSettings` |
| Send mail | POST | `/users/{id}/sendMail` |
| List messages | GET | `/users/{id}/messages` |
| Create mail folder | POST | `/users/{id}/mailFolders` |

### Calendar

| Operation | Method | Endpoint |
|-----------|--------|----------|
| List calendars | GET | `/users/{id}/calendars` |
| List events | GET | `/users/{id}/events` |
| Create event | POST | `/users/{id}/events` |
| Get free/busy | POST | `/users/{id}/calendar/getSchedule` |

### Teams

| Operation | Method | Endpoint |
|-----------|--------|----------|
| List teams | GET | `/groups?$filter=resourceProvisioningOptions/Any(x:x eq 'Team')` |
| Get team | GET | `/teams/{id}` |
| Create team | POST | `/teams` |
| List channels | GET | `/teams/{id}/channels` |
| Add member to team | POST | `/teams/{id}/members` |

---

## User Operations

### Create User

```jinja
{
    "accountEnabled": true,
    "displayName": "{{ CTX.display_name }}",
    "mailNickname": "{{ CTX.mail_nickname }}",
    "userPrincipalName": "{{ CTX.upn }}",
    "passwordProfile": {
        "forceChangePasswordNextSignIn": true,
        "password": "{{ CTX.temp_password }}"
    },
    "usageLocation": "{{ ORG.VARIABLES.m365_usage_location | d('US') }}",
    "givenName": "{{ CTX.first_name }}",
    "surname": "{{ CTX.last_name }}",
    "jobTitle": "{{ CTX.job_title | d('') }}",
    "department": "{{ CTX.department | d('') }}",
    "officeLocation": "{{ CTX.office | d('') }}",
    "mobilePhone": "{{ CTX.mobile | d('') }}"
}
```

### Disable User

```jinja
{
    "accountEnabled": false
}
```

### Reset Password

```jinja
{
    "passwordProfile": {
        "forceChangePasswordNextSignIn": true,
        "password": "{{ CTX.new_password }}"
    }
}
```

### Set Manager

```jinja
{# PUT to /users/{id}/manager/$ref #}
{
    "@odata.id": "https://graph.microsoft.com/v1.0/users/{{ CTX.manager_id }}"
}
```

---

## Group Operations

### Create Security Group

```jinja
{
    "displayName": "{{ CTX.group_name }}",
    "mailEnabled": false,
    "mailNickname": "{{ CTX.group_name | lower | replace(' ', '-') }}",
    "securityEnabled": true,
    "description": "{{ CTX.description | d('') }}"
}
```

### Create Microsoft 365 Group

```jinja
{
    "displayName": "{{ CTX.group_name }}",
    "mailEnabled": true,
    "mailNickname": "{{ CTX.group_name | lower | replace(' ', '-') }}",
    "securityEnabled": false,
    "groupTypes": ["Unified"],
    "description": "{{ CTX.description | d('') }}"
}
```

### Add Member to Group

```jinja
{# POST to /groups/{groupId}/members/$ref #}
{
    "@odata.id": "https://graph.microsoft.com/v1.0/directoryObjects/{{ CTX.user_id }}"
}
```

### Bulk Add Members

```jinja
{# PATCH to /groups/{id} - up to 20 at a time #}
{
    "members@odata.bind": [
        {% for user_id in CTX.user_ids[:20] %}
        "https://graph.microsoft.com/v1.0/directoryObjects/{{ user_id }}"{{ "," if not loop.last }}
        {% endfor %}
    ]
}
```

---

## Mailbox Operations

### Set Out of Office

```jinja
{
    "automaticRepliesSetting": {
        "status": "scheduled",
        "scheduledStartDateTime": {
            "dateTime": "{{ CTX.start_date }}",
            "timeZone": "{{ CTX.timezone | d('UTC') }}"
        },
        "scheduledEndDateTime": {
            "dateTime": "{{ CTX.end_date }}",
            "timeZone": "{{ CTX.timezone | d('UTC') }}"
        },
        "internalReplyMessage": "{{ CTX.internal_message | escape }}",
        "externalReplyMessage": "{{ CTX.external_message | escape }}",
        "externalAudience": "{{ CTX.external_audience | d('contactsOnly') }}"
    }
}
```

### Convert to Shared Mailbox

```powershell
# Via Exchange Online PowerShell (Agent Smith or Azure Automation)
Set-Mailbox -Identity "{{ CTX.user_upn }}" -Type Shared
```

### Grant Mailbox Access

```jinja
{# Via Exchange Admin - Add-MailboxPermission equivalent #}
{# Use Exchange Online PowerShell action #}
Add-MailboxPermission -Identity "{{ CTX.mailbox }}" -User "{{ CTX.delegate }}" -AccessRights FullAccess -InheritanceType All
Add-RecipientPermission -Identity "{{ CTX.mailbox }}" -Trustee "{{ CTX.delegate }}" -AccessRights SendAs
```

### Create Shared Mailbox

```jinja
{# Step 1: Create user without license #}
{
    "accountEnabled": false,
    "displayName": "{{ CTX.display_name }}",
    "mailNickname": "{{ CTX.mail_nickname }}",
    "userPrincipalName": "{{ CTX.upn }}",
    "passwordProfile": {
        "forceChangePasswordNextSignIn": false,
        "password": "{{ CTX.random_password }}"
    }
}

{# Step 2: Convert to shared via PowerShell #}
```

---

## Pagination Handling

### Standard Pagination

```jinja
{# Check for next page #}
{% if TASKS.list_users.result.result.data["@odata.nextLink"] %}
    {# Extract skiptoken or use full URL #}
    {% set next_link = TASKS.list_users.result.result.data["@odata.nextLink"] %}
{% endif %}
```

### Pagination Loop Pattern

```
1. Initial Request
2. Check @odata.nextLink
   ├── Exists → Append results, request next page
   └── None → Exit loop with all results
```

### Delta Queries

```jinja
{# Initial delta query #}
/users/delta?$select=displayName,mail

{# Store delta link for subsequent calls #}
{% set delta_link = TASKS.delta.result.result.data["@odata.deltaLink"] %}

{# Next sync uses delta link to get only changes #}
```

---

## Query Parameters

### $select - Specify Fields

```
/users?$select=id,displayName,mail,userPrincipalName
```

### $filter - Filter Results

```jinja
{# Enabled users only #}
/users?$filter=accountEnabled eq true

{# By department #}
/users?$filter=department eq 'Engineering'

{# Multiple conditions #}
/users?$filter=accountEnabled eq true and department eq 'IT'

{# Contains #}
/users?$filter=startswith(displayName,'John')

{# In list #}
/users?$filter=userPrincipalName in ('user1@domain.com','user2@domain.com')
```

### $expand - Include Related Data

```jinja
{# Include manager #}
/users?$expand=manager($select=displayName,mail)

{# Include group memberships #}
/users/{id}?$expand=memberOf($select=displayName,id)
```

### $top - Limit Results

```
/users?$top=100
```

### $count - Include Total Count

```
/users?$count=true
```

### $orderby - Sort Results

```
/users?$orderby=displayName asc
```

### Combining Parameters

```jinja
/users?$select=id,displayName,mail&$filter=accountEnabled eq true&$expand=manager($select=displayName)&$top=100&$orderby=displayName
```

---

## Common Patterns

### Get All Licensed Users

```jinja
{{
    [
        user for user in CTX.all_users
        if user.assignedLicenses | length > 0
    ]
}}
```

### Users by License Type

```jinja
{# Users with E3 #}
{% set e3_sku = "6fd2c87f-b296-42f0-b197-1e91e994b900" %}
{{
    [
        user for user in CTX.users
        if e3_sku in (user.assignedLicenses | map(attribute="skuId") | list)
    ]
}}
```

### Get User's Groups

```jinja
{# Filter to security groups #}
{{
    [
        g for g in CTX.user_groups
        if g.securityEnabled == true
    ]
}}
```

### Check Admin Role

```jinja
{# Global Admin role ID #}
{% set global_admin_role = "62e90394-69f5-4237-9190-012177145e10" %}
{{ global_admin_role in (CTX.user_roles | map(attribute="roleTemplateId") | list) }}
```

---

## Error Handling

### Common Graph Errors

| Error Code | Description | Resolution |
|------------|-------------|------------|
| `Request_ResourceNotFound` | User/resource doesn't exist | Verify ID |
| `Authorization_RequestDenied` | Insufficient permissions | Check app permissions |
| `Request_BadRequest` | Invalid request format | Check payload |
| `Directory_QuotaExceeded` | Tenant quota exceeded | Contact Microsoft |
| `ServiceNotAvailable` | M365 service issue | Retry with backoff |

### Error Response Pattern

```jinja
{% if TASKS.graph_call.result.result.status_code != 200 %}
    {% set error = TASKS.graph_call.result.result.data.error | d({}) %}
    Error: {{ error.code | d("Unknown") }} - {{ error.message | d("No message") }}
{% endif %}
```

---

## Security Considerations

### Least Privilege Permissions

Only request permissions you need:

| Operation | Minimum Permission |
|-----------|-------------------|
| Read users | `User.Read.All` |
| Manage users | `User.ReadWrite.All` |
| Read groups | `Group.Read.All` |
| Manage groups | `Group.ReadWrite.All` |
| Assign licenses | `User.ReadWrite.All` + `Directory.ReadWrite.All` |
| Send mail | `Mail.Send` |
| Manage mailboxes | `MailboxSettings.ReadWrite` |

### Audit Logging

Track all changes:

```jinja
{
    "action": "{{ CTX.action }}",
    "target_user": "{{ CTX.user_upn }}",
    "performed_by": "Rewst Automation",
    "timestamp": "{{ now() | format_datetime('%Y-%m-%dT%H:%M:%SZ') }}",
    "org": "{{ ORG.ATTRIBUTES.name }}",
    "details": {{ CTX.change_details | to_json_string }}
}
```
