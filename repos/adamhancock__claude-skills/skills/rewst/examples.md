# Rewst Workflow Examples

Real-world workflow patterns and complete examples.

---

## User Onboarding Complete Flow

```
Trigger: Form Submission (New User Request)

1. Validate Input Data
   └── Check required fields, validate email format

2. Check for Existing User
   ├── Found → Send conflict notification → End
   └── Not Found → Continue

3. Create User in Identity Provider
   └── Data Alias: created_user

4. [With Items: CTX.selected_licenses]
   └── Assign License
       └── item().sku_id

5. [With Items: CTX.selected_groups]
   └── Add to Group
       └── item().group_id

6. Generate Temporary Password
   └── Data Alias: temp_password

7. Create Documentation Entry
   └── IT Glue/Hudu

8. Create PSA Ticket
   └── User setup complete

9. Send Welcome Email
   └── Include credentials

10. Notify Requestor
    └── Confirmation with details

Output: {user_id, email, status: "completed"}
```

---

## Automated Offboarding Flow

```
Trigger: PSA Ticket (Termination Request)

1. Parse Ticket for User Info
   └── Extract username/email

2. Look Up User
   ├── Not Found → Update ticket → End
   └── Found → Continue

3. [Parallel Execution]
   ├── Disable User Account
   ├── Remove from All Groups
   ├── Revoke All Licenses
   └── Reset Password to Random

4. Convert Mailbox to Shared
   └── Assign to manager

5. Backup User Data
   └── Export to archive

6. Update Documentation

7. Update PSA Ticket
   └── Add completion notes

8. Send Manager Notification
```

---

## Ticket Triage Automation

```
Trigger: PSA Ticket Created (Webhook)

1. Extract Ticket Details

2. [Dictionary Switch] Route by Type
   ├── "Password Reset" → Password Reset Subworkflow
   ├── "New User" → User Onboarding Form Link
   ├── "Hardware" → Hardware Request Workflow
   └── Default → Assign to Triage Queue

3. Apply SLA Based on Priority

4. Assign to Appropriate Team

5. Send Acknowledgment

6. Update Ticket
```

---

## License Compliance Check

```
Trigger: Scheduled (Weekly)

1. [With Items: CTX.all_orgs]
   └── Run as Org: item().id
       ├── List All M365 Users
       ├── List Assigned Licenses
       └── Collect Results

2. Aggregate License Usage

3. Compare to Entitlements

4. [Conditional] Compliance Status
   ├── Over-licensed → Generate Alert
   ├── Under-licensed → Opportunity Report
   └── Compliant → Log Status

5. Generate Report

6. Send Report
```

---

## Backup Verification

```
Trigger: Scheduled (Daily)

1. List All Backup Jobs

2. [With Items: CTX.backup_jobs]
   └── Check Job Status

3. Filter Failed Jobs
   └── {{ [job for job in CTX.results if job.status == "failed"] }}

4. [Conditional] Any Failures?
   ├── Yes →
   │   ├── Create PSA Ticket per failure
   │   └── Send Alert Email
   └── No → Log Success

5. Update Backup Dashboard

6. Generate Weekly Report
```

---

## Password Reset Self-Service

```
Trigger: Form Submission

1. Verify User Identity
   └── Check against directory

2. Generate New Password
   └── Generate Password V2 action

3. Reset Password in Identity Provider

4. [Optional] Sync to Other Systems

5. Create Audit Trail
   └── PSA ticket or log

6. Send New Password to User
   └── Secure delivery method
```

---

## Multi-Org Report Generation

```
Trigger: Scheduled (Monthly)

1. List All Child Organizations

2. [With Items: CTX.organizations]
   └── Run as Org: item().id
       ├── Collect metrics
       ├── Get user counts
       └── Get ticket counts

3. Aggregate All Results

4. Generate Report
   └── Template with charts

5. Store Report

6. Email to Stakeholders
```

---

## Approval Workflow

```
Trigger: Form or Webhook

1. Gather Request Details

2. Determine Approver
   └── Based on type/amount

3. Send Confirmation Email
   └── Approve/Deny buttons

4. [Follow First] Check Response
   ├── "approve" →
   │   └── Process Request
   └── "deny" →
       └── Send Rejection Notice

5. Log Decision

6. Notify Requestor
```

---

## Device Inventory Collection

```
Trigger: Scheduled (Weekly)

1. List All Managed Devices

2. [With Items: CTX.devices]
   └── Run Script via RMM
       └── Collect system info

3. Process Results
   └── Parse JSON responses

4. Update Documentation
   └── Flexible assets

5. Compare to Previous
   └── Flag changes

6. Generate Change Report
```

---

## Common Jinja Patterns Used in Examples

### Filter Active Users

```jinja
{{ [u for u in CTX.users if u.accountEnabled] }}
```

### Create Options from Users

```jinja
{{
    [
        {"label": u.displayName, "value": u.id}
        for u in CTX.users
        if u.mail
    ] | sort(attribute="label")
}}
```

### Build Email Body

```jinja
User: {{ CTX.user.displayName }}
Email: {{ CTX.user.mail }}
Created: {{ now() | format_datetime("%Y-%m-%d %H:%M") }}

{% for group in CTX.assigned_groups %}
- {{ group.displayName }}
{% endfor %}
```

### Combine User and Group Data

```jinja
{{ [dict(user, **groups) for user, groups in CTX.users|zip(CTX.group_results)] }}
```

### Count by Status

```jinja
{% set open = CTX.tickets | selectattr("status", "eq", "open") | list | length %}
{% set closed = CTX.tickets | selectattr("status", "eq", "closed") | list | length %}
Open: {{ open }}, Closed: {{ closed }}
```

### Format for PSA Ticket

```jinja
## User Onboarding Complete

**User:** {{ CTX.user.displayName }}
**Email:** {{ CTX.user.mail }}
**Department:** {{ CTX.user.department | d("Not Set") }}

### Assigned Licenses
{% for lic in CTX.assigned_licenses %}
- {{ lic.skuPartNumber }}
{% endfor %}

### Assigned Groups
{% for grp in CTX.assigned_groups %}
- {{ grp.displayName }}
{% endfor %}

---
*Automated by Rewst*
```

---

## Template: PowerShell for RMM

```powershell
# Variables from Rewst
$Target = "{{ CTX.target }}"
$Action = "{{ CTX.action }}"

try {
    switch ($Action) {
        "inventory" {
            $result = Get-ComputerInfo | Select-Object *
        }
        "services" {
            $result = Get-Service | Where-Object Status -eq "Running"
        }
        "processes" {
            $result = Get-Process | Select-Object Name, Id, CPU
        }
    }

    @{
        status = "success"
        data = $result
    } | ConvertTo-Json -Depth 10
}
catch {
    @{
        status = "error"
        message = $_.Exception.Message
    } | ConvertTo-Json
}
```

---

## Template: HTML Email

```html
<!DOCTYPE html>
<html>
<head>
    <style>
        body { font-family: Arial, sans-serif; }
        .header { background: #0066cc; color: white; padding: 20px; }
        .content { padding: 20px; }
        .status-ok { color: green; }
        .status-error { color: red; }
        table { border-collapse: collapse; width: 100%; }
        th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }
        th { background: #f4f4f4; }
    </style>
</head>
<body>
    <div class="header">
        <h1>{{ CTX.report_title }}</h1>
    </div>
    <div class="content">
        <p>Generated: {{ now() | format_datetime("%B %d, %Y at %H:%M") }}</p>

        <h2>Summary</h2>
        <ul>
            <li>Total Items: {{ CTX.items | length }}</li>
            <li class="status-ok">Successful: {{ CTX.items | selectattr("status", "eq", "success") | list | length }}</li>
            <li class="status-error">Failed: {{ CTX.items | selectattr("status", "eq", "failed") | list | length }}</li>
        </ul>

        <h2>Details</h2>
        <table>
            <tr>
                <th>Name</th>
                <th>Status</th>
                <th>Message</th>
            </tr>
            {% for item in CTX.items %}
            <tr>
                <td>{{ item.name }}</td>
                <td class="status-{{ 'ok' if item.status == 'success' else 'error' }}">{{ item.status }}</td>
                <td>{{ item.message | d("N/A") }}</td>
            </tr>
            {% endfor %}
        </table>
    </div>
</body>
</html>
```

---

## Security Incident Response

```
Trigger: Webhook (Security Alert) or Manual Form

1. Parse Alert Details
   └── Extract user, threat type, severity

2. [Follow First] Severity Check
   ├── Critical/High →
   │   ├── Disable User Immediately
   │   ├── Revoke All Sessions
   │   └── Reset Password
   └── Medium/Low →
       └── Continue to investigation

3. Gather Evidence
   ├── Get Sign-in Logs (last 7 days)
   ├── Get Audit Logs
   └── Get User Risk Events

4. Create Incident Ticket
   └── High priority, attach evidence

5. [Conditional] Is Compromised?
   ├── Yes →
   │   ├── Block Sign-in
   │   ├── Remove from All Groups
   │   ├── Forward Email to Security
   │   └── Enable Litigation Hold
   └── No →
       └── Clear User Risk

6. Send Notifications
   ├── Security Team
   ├── User's Manager
   └── Compliance (if required)

7. Document in IT Glue/Hudu
```

---

## Shared Mailbox Management

```
Trigger: Form Submission

1. Validate Request
   └── Check permissions, mailbox doesn't exist

2. Create Shared Mailbox
   └── Set properties, auto-mapping

3. [With Items: CTX.full_access_users]
   └── Grant Full Access

4. [With Items: CTX.send_as_users]
   └── Grant Send As

5. [Optional] Set Auto-Reply

6. Update Documentation

7. Create Ticket with Details

8. Notify Requestor
```

---

## Stale User Cleanup

```
Trigger: Scheduled (Weekly)

1. List All Users

2. Filter Stale Users
   └── {{ [u for u in CTX.users if u.signInActivity.lastSignInDateTime | as_datetime < now() | datedelta(days=-90)] }}

3. Get Manager for Each

4. [With Items: CTX.stale_users]
   ├── Send Manager Notification
   └── Tag User for Review

5. Generate Report
   └── Users by last sign-in date

6. Create Summary Ticket

7. [After 14 days - Completion Handler]
   └── Disable users not cleared
```

---

## Advanced Jinja Recipes

### Build Username from Format

```jinja
{# ORG.VARIABLES.username_format: flast, firstl, first.last, firstmlast #}
{% set format = ORG.VARIABLES.username_format | d("flast") %}
{% set first = CTX.first_name | lower | trim %}
{% set last = CTX.last_name | lower | trim %}
{% set middle = CTX.middle_name | d("") | lower | trim %}

{% if format == "flast" %}
    {{ first[0] ~ last }}
{% elif format == "firstl" %}
    {{ first ~ last[0] }}
{% elif format == "first.last" %}
    {{ first ~ "." ~ last }}
{% elif format == "firstmlast" %}
    {{ first ~ (middle[0] if middle else "") ~ last }}
{% else %}
    {{ first ~ last }}
{% endif %}
```

### Deduplicate by Attribute

```jinja
{# Remove duplicate users by email #}
{% set seen = namespace(emails=[]) %}
{% set unique_users = [] %}
{% for user in CTX.users %}
    {% if user.email not in seen.emails %}
        {% set _ = unique_users.append(user) %}
        {% set seen.emails = seen.emails + [user.email] %}
    {% endif %}
{% endfor %}
{{ unique_users }}
```

### Group Items by Attribute

```jinja
{# Group users by department #}
{% set departments = {} %}
{% for user in CTX.users %}
    {% set dept = user.department | d("No Department") %}
    {% if dept not in departments %}
        {% set _ = departments.update({dept: []}) %}
    {% endif %}
    {% set _ = departments[dept].append(user) %}
{% endfor %}

{# Or use groupby filter #}
{% for dept, users in CTX.users | groupby("department") %}
{{ dept }}: {{ users | length }} users
{% endfor %}
```

### Merge Multiple API Results

```jinja
{# Combine user info with their licenses and groups #}
{{
    [
        {
            "user": user,
            "licenses": CTX.license_results[idx].result.result.data.value | d([]),
            "groups": CTX.group_results[idx].result.result.data.value | d([])
        }
        for idx, user in enumerate(CTX.users)
    ]
}}
```

### Find Differences Between Lists

```jinja
{# Users in list1 but not list2 #}
{% set list1_ids = CTX.list1 | map(attribute="id") | list %}
{% set list2_ids = CTX.list2 | map(attribute="id") | list %}

{# Added (in list1, not in list2) #}
{% set added = [u for u in CTX.list1 if u.id not in list2_ids] %}

{# Removed (in list2, not in list1) #}
{% set removed = [u for u in CTX.list2 if u.id not in list1_ids] %}
```

### Dynamic Field Mapping

```jinja
{# Map source fields to destination fields #}
{% set field_map = {
    "firstName": "first_name",
    "lastName": "last_name",
    "emailAddress": "email",
    "phoneNumber": "phone"
} %}

{{
    {
        field_map[key]: value
        for key, value in CTX.source_data.items()
        if key in field_map
    }
}}
```

### Calculate Business Days

```jinja
{# Count business days between two dates #}
{% set start = CTX.start_date | as_datetime %}
{% set end = CTX.end_date | as_datetime %}
{% set ns = namespace(business_days=0, current=start) %}

{% for _ in range(1000) %}  {# Max iterations #}
    {% if ns.current >= end %}
        {% break %}
    {% endif %}
    {% set weekday = ns.current | format_datetime("%w") | int %}
    {% if weekday not in [0, 6] %}  {# Not Sunday (0) or Saturday (6) #}
        {% set ns.business_days = ns.business_days + 1 %}
    {% endif %}
    {% set ns.current = ns.current | datedelta(days=1) %}
{% endfor %}

{{ ns.business_days }} business days
```

### Paginated API Collection

```jinja
{# Collect all pages of results #}
{# This pattern is used across multiple tasks with a loop #}

{# Task 1: Initial request #}
{# Store: CTX.all_results = TASKS.initial.result.result.data.value #}
{# Store: CTX.next_link = TASKS.initial.result.result.data["@odata.nextLink"] #}

{# Task 2: While loop condition #}
{{ CTX.next_link | d("") | length > 0 }}

{# Task 3: On success of page fetch #}
{# Append results: #}
{{ CTX.all_results + TASKS.fetch_page.result.result.data.value }}
```

### Conditional Ticket Assignment

```jinja
{# Assign based on multiple criteria #}
{% set category = CTX.ticket.category | lower %}
{% set priority = CTX.ticket.priority %}
{% set client_tier = ORG.VARIABLES.client_tier | d("standard") %}

{% if priority == 1 or client_tier == "premium" %}
    {% set assignee = ORG.VARIABLES.senior_tech_id %}
{% elif category in ["network", "infrastructure"] %}
    {% set assignee = ORG.VARIABLES.network_team_id %}
{% elif category in ["m365", "email", "azure"] %}
    {% set assignee = ORG.VARIABLES.cloud_team_id %}
{% else %}
    {% set assignee = ORG.VARIABLES.helpdesk_queue_id %}
{% endif %}

{{ assignee }}
```

---

## Template: Detailed Audit Report

```jinja
# User Audit Report
Generated: {{ now() | format_datetime("%Y-%m-%d %H:%M:%S UTC") }}
Organization: {{ ORG.ATTRIBUTES.name }}

## Summary
- Total Users Audited: {{ CTX.users | length }}
- Licensed Users: {{ CTX.users | selectattr("assignedLicenses") | list | length }}
- Disabled Users: {{ CTX.users | rejectattr("accountEnabled") | list | length }}
- External Users: {{ CTX.users | selectattr("userType", "eq", "Guest") | list | length }}

## Users by License Type
{% for sku_name, users in CTX.users_by_license | groupby("primaryLicense") %}
### {{ sku_name | d("No License") }}
| User | Email | Last Sign-In | Status |
|------|-------|--------------|--------|
{% for user in users %}
| {{ user.displayName }} | {{ user.mail | d("N/A") }} | {{ user.lastSignIn | d("Never") }} | {{ "Active" if user.accountEnabled else "Disabled" }} |
{% endfor %}
{% endfor %}

## Recent Changes (Last 30 Days)
{% for change in CTX.audit_logs %}
- {{ change.activityDateTime | format_datetime("%Y-%m-%d %H:%M") }}: {{ change.activityDisplayName }} by {{ change.initiatedBy.user.displayName | d("System") }}
{% endfor %}

## Recommendations
{% if CTX.stale_users | length > 0 %}
- [ ] Review {{ CTX.stale_users | length }} users with no sign-in > 90 days
{% endif %}
{% if CTX.over_licensed | length > 0 %}
- [ ] Review {{ CTX.over_licensed | length }} potentially over-licensed users
{% endif %}
{% if CTX.no_mfa | length > 0 %}
- [ ] Enable MFA for {{ CTX.no_mfa | length }} users without MFA
{% endif %}

---
*Report generated by Rewst Automation*
```

---

## Template: Webhook Response Body

```jinja
{# For workflows returning data via webhook #}
{
    "success": {{ "true" if CTX.workflow_success else "false" }},
    "message": "{{ CTX.status_message | d('Operation completed') | escape }}",
    "timestamp": "{{ now() | format_datetime('%Y-%m-%dT%H:%M:%SZ') }}",
    "data": {
        "operation": "{{ CTX.operation_type }}",
        "target": "{{ CTX.target_id }}",
        "results": {{ CTX.results | to_json_string }},
        "errors": {{ CTX.errors | d([]) | to_json_string }}
    },
    "metadata": {
        "workflow_id": "{{ CTX.__workflow_id__ | d('') }}",
        "execution_id": "{{ CTX.__execution_id__ | d('') }}",
        "organization": "{{ ORG.ATTRIBUTES.name }}"
    }
}
```
