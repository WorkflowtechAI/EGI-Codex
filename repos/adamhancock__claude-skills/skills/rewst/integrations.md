# Rewst Integration Patterns

Complete reference for integration-specific patterns and API handling.

---

## Microsoft 365 / Graph API

### Standard Response Pattern

```jinja
{# API responses with value array #}
{{ TASKS.list_users.result.result.data.value }}
```

### Pagination

```jinja
{# Check for more pages #}
{% if TASKS.list_users.result.result.data["@odata.nextLink"] %}
    {# More pages available #}
{% endif %}

{# Delta queries #}
{{ TASKS.delta_query.result.result.data["@odata.deltaLink"] }}
```

### Common Queries

```jinja
{# Request specific properties #}
$select=id,displayName,mail,userPrincipalName,accountEnabled,assignedLicenses

{# Expand relationships #}
$expand=manager($select=id,displayName,mail)

{# Filter results #}
$filter=accountEnabled eq true
```

### License Operations

```jinja
{# Check if user has license #}
{% set target_sku = "ENTERPRISEPACK" %}
{{ true if CTX.user.assignedLicenses | selectattr("skuId", "eq", target_sku) | list else false }}

{# Assign multiple licenses #}
{
    "addLicenses": [
        {% for sku in CTX.licenses_to_add %}
        {"skuId": "{{ sku.id }}"}{{ "," if not loop.last }}
        {% endfor %}
    ],
    "removeLicenses": []
}
```

### Group Membership

```jinja
{# Check group membership #}
{{ CTX.group_id in (CTX.user_groups | map(attribute="id") | list) }}
```

### Error Handling

```jinja
{% if TASKS.m365_action.result.result.status_code == 404 %}
    User not found
{% elif TASKS.m365_action.result.result.status_code == 403 %}
    Insufficient permissions
{% elif TASKS.m365_action.result.result.status_code == 429 %}
    Rate limited - retry needed
{% endif %}
```

---

## ConnectWise Manage

### Response Pattern

```jinja
{# Direct array response #}
{{ TASKS.list_tickets.result.result.data }}

{# Nested company info #}
{{ ticket.company.identifier }}
{{ ticket.company.name }}
```

### Ticket Creation

```jinja
{
    "company": {"id": {{ CTX.company_id }}},
    "board": {"id": {{ ORG.VARIABLES.psa_default_board_id }}},
    "status": {"name": "{{ ORG.VARIABLES.psa_ticket_status_new }}"},
    "type": {"name": "{{ CTX.ticket_type }}"},
    "summary": "{{ CTX.summary | escape }}",
    "initialDescription": "{{ CTX.description | escape }}",
    "contact": {"id": {{ CTX.contact_id }}},
    "priority": {"id": {{ CTX.priority_id | d(3) }}}
}
```

---

## Datto PSA (Autotask)

### Response Pattern

```jinja
{# Items array in response #}
{{ TASKS.list_tickets.result.result.data.items }}

{# Pagination info #}
{{ TASKS.list_tickets.result.result.data.pageDetails }}
```

---

## Datto RMM

### Script Execution

```jinja
{
    "variables": [
        {"name": "UserName", "value": "{{ CTX.username }}"},
        {"name": "Action", "value": "{{ CTX.action }}"}
    ],
    "device_id": "{{ CTX.device_id }}",
    "script_id": "{{ ORG.VARIABLES.rmm_script_id }}"
}
```

---

## HaloPSA

### Response Pattern

```jinja
{# Direct array or wrapped #}
{{ TASKS.list_tickets.result.result.data }}
{{ TASKS.list_tickets.result.result.data.tickets }}
```

### Action Creation

```jinja
{
    "ticket_id": {{ CTX.ticket_id }},
    "outcome": "{{ CTX.action_outcome }}",
    "note": "{{ CTX.action_note | escape }}",
    "who": "Rewst Automation",
    "hiddenfromuser": {{ CTX.internal_only | lower }}
}
```

---

## IT Glue

### Flexible Asset

```jinja
{
    "data": {
        "type": "flexible-assets",
        "attributes": {
            "organization-id": {{ CTX.itglue_org_id }},
            "flexible-asset-type-id": {{ ORG.VARIABLES.itglue_asset_type_id }},
            "traits": {
                "user-name": "{{ CTX.user.name }}",
                "email": "{{ CTX.user.email }}",
                "last-updated": "{{ now() | format_datetime('%Y-%m-%d') }}"
            }
        }
    }
}
```

---

## Agent Smith

Azure IoT Hub-based agent for PowerShell on endpoints.

### Requirements

1. Microsoft Cloud Integration Bundle with Azure
2. Agent Smith: Device Provisioning Crate
3. Agent Smith: Service Provisioning Crate

### Callback Pattern

```powershell
# Execute script logic
$results = Get-Process | Select-Object Name, Id, CPU

# Return to Rewst
$postData = $PS_Results | ConvertTo-Json
Invoke-RestMethod -Method 'Post' -Uri $post_url -Body $postData
```

---

## Cross-Integration Mapping

### Company/Organization Mapping

```jinja
{# Store mappings in org variable #}
{
    "psa_company_id": "123",
    "rmm_site_id": "456",
    "m365_tenant_id": "abc-123",
    "itglue_org_id": "789"
}

{# Access #}
{{ ORG.VARIABLES.integration_mappings.psa_company_id }}
```

### Status/Priority Mapping

```jinja
{% set priority_map = {
    "1": "critical",
    "2": "high",
    "3": "medium",
    "4": "low"
} %}
{{ priority_map[CTX.source_priority | string] | d("medium") }}
```

---

## Custom HTTP Integrations

### Building Custom Requests

```yaml
URL: "https://api.example.com/v1/{{ CTX.endpoint }}"
Method: POST
Headers:
  Authorization: "Bearer {{ ORG.VARIABLES.api_key }}"
  Content-Type: "application/json"
Body: |
  {"key": "{{ CTX.value | escape }}"}
Timeout: 30
Require Success Status: true
```

### OAuth Token Management

```jinja
{% set token_expiry = ORG.VARIABLES.api_token_expiry | as_datetime %}
{% if token_expiry < now() %}
    {# Trigger token refresh #}
{% endif %}
```

---

## GraphQL API (Rewst Internal)

### Parameters

| Parameter | Description |
|-----------|-------------|
| Operation Type | `query` or `mutation` |
| Graph Operation | Operation name |
| Variable Values | Variables to pass |
| Response Fields | Fields to return |
| Raw Query | Complete GraphQL string |

### Common Queries

```yaml
# List Organizations
operation_type: "query"
operation: "organizations"
variable_values:
  limit: 50
  order: [["name"]]
fields: "id, name, domain, isEnabled"

# List Workflow Executions
operation_type: "query"
operation: "workflowExecutions"
variable_values:
  where:
    orgId: "{{ CTX.org_id }}"
  limit: 100
fields: "id, status, createdAt, workflow { name }"

# Time Saved Statistics
operation_type: "query"
operation: "timeSavedGroupByWorkflow"
variable_values:
  orgId: "{{ CTX.org_id }}"
fields: "workflowId, workflowName, secondsSaved, totalExecutions"
```

### Common Mutations

```yaml
# Create Organization Variable
operation_type: "mutation"
operation: "createOrgVariable"
variable_values:
  orgVariable:
    name: "api_endpoint"
    value: "https://api.example.com"
    category: "general"
    orgId: "{{ CTX.org_id }}"
fields: "id, name, value"
```

### Key GraphQL Entities

| Entity | Description |
|--------|-------------|
| `Organization` | Core org data |
| `Workflow` | Workflow definitions |
| `WorkflowExecution` | Execution history |
| `Trigger` | Event triggers |
| `Form` | Dynamic forms |
| `OrgVariable` | Org variables |

---

## PowerShell Template Patterns

### RMM Execution Template

```powershell
# Variables from Rewst
$UserName = "{{ CTX.username }}"
$Action = "{{ CTX.action }}"

try {
    # Your logic here
    $result = Do-Something

    @{
        "status" = "success"
        "result" = $result
    } | ConvertTo-Json
}
catch {
    @{
        "status" = "error"
        "message" = $_.Exception.Message
    } | ConvertTo-Json
}
```

### Common PowerShell Tasks

```powershell
# Get local admins
Get-LocalGroupMember -Group "Administrators" | ConvertTo-Json

# Check service status
Get-Service -Name "ServiceName" | Select Name, Status | ConvertTo-Json

# Registry check
$value = Get-ItemProperty -Path "HKLM:\Software\Key" -Name "Name"
@{"exists" = ($null -ne $value); "value" = $value.Name} | ConvertTo-Json
```

---

## API Error Handling

| Status | Common Causes | Actions |
|--------|---------------|---------|
| 400 | Invalid request body | Check payload structure |
| 401 | Expired/invalid token | Reauthorize integration |
| 403 | Insufficient permissions | Check API scopes |
| 404 | Resource not found | Verify resource ID |
| 409 | Conflict/duplicate | Check existing resource |
| 429 | Rate limited | Add delay, reduce concurrency |
| 500 | Server error | Retry with backoff |
| 502/503 | Service unavailable | Wait and retry |

---

## Retry Pattern

```
1. Make request
   ├── Success (2xx) → Continue
   └── Failure → Check conditions
       ├── 429/5xx → Delay + Retry
       └── 4xx → Fail (don't retry)

2. Track retry count
   ├── Count < Max → Retry with backoff
   └── Count >= Max → Fail workflow
```

---

## ConnectWise Manage Deep Dive

### Ticket Creation Full Example

```jinja
{
    "company": {"id": {{ CTX.company_id }}},
    "contact": {"id": {{ CTX.contact_id | d("null") }}},
    "board": {"id": {{ ORG.VARIABLES.psa_default_board_id }}},
    "status": {"name": "{{ ORG.VARIABLES.psa_status_new | d("New") }}"},
    "type": {"name": "{{ CTX.ticket_type | d("Service") }}"},
    "subType": {"name": "{{ CTX.sub_type | d("") }}"},
    "item": {"name": "{{ CTX.item | d("") }}"},
    "priority": {"id": {{ CTX.priority_id | d(4) }}},
    "severity": "{{ CTX.severity | d("Medium") }}",
    "impact": "{{ CTX.impact | d("Medium") }}",
    "summary": "{{ CTX.summary | truncate(100) | escape }}",
    "initialDescription": "{{ CTX.description | escape }}",
    "source": {"name": "{{ CTX.source | d("Rewst") }}"},
    "resources": "{{ CTX.resource_list | d("") }}",
    "budgetHours": {{ CTX.budget_hours | d(0) }},
    "automaticEmailContactFlag": {{ CTX.email_contact | d(true) | lower }},
    "automaticEmailResourceFlag": {{ CTX.email_resource | d(false) | lower }},
    "automaticEmailCcFlag": {{ CTX.email_cc | d(false) | lower }}
}
```

### Ticket Note (Time Entry)

```jinja
{
    "text": "{{ CTX.note_text | escape }}",
    "detailDescriptionFlag": true,
    "internalAnalysisFlag": {{ CTX.internal | d(false) | lower }},
    "resolutionFlag": false,
    "member": {"identifier": "{{ ORG.VARIABLES.psa_automation_member | d("API") }}"},
    "timeStart": "{{ CTX.start_time | d(now() | format_datetime('%Y-%m-%dT%H:%M:%SZ')) }}",
    "timeEnd": "{{ CTX.end_time | d(now() | format_datetime('%Y-%m-%dT%H:%M:%SZ')) }}",
    "actualHours": {{ CTX.hours | d(0) }},
    "chargeToType": "ServiceTicket",
    "chargeToId": {{ CTX.ticket_id }}
}
```

### Common CW Manage Conditions

```jinja
{# Get tickets by status #}
conditions=status/name="New" or status/name="In Progress"

{# Get tickets by board #}
conditions=board/id={{ ORG.VARIABLES.board_id }}

{# Get tickets modified today #}
conditions=lastUpdated > [{{ now() | format_datetime("%Y-%m-%d") }}T00:00:00Z]

{# Combine conditions #}
conditions=status/name="New" and board/id=10 and company/id=123
```

### CW Manage Pagination

```jinja
{# Parameters #}
page: {{ CTX.current_page | d(1) }}
pageSize: 100
orderBy: id asc

{# Check for more pages - response includes pagination headers #}
{% set total = TASKS.list_tickets.result.result.headers.get("Link", "") %}
```

---

## Autotask/Datto PSA Deep Dive

### Ticket Creation

```jinja
{
    "CompanyID": {{ CTX.company_id }},
    "CompanyLocationID": {{ CTX.location_id | d("null") }},
    "ContactID": {{ CTX.contact_id | d("null") }},
    "Title": "{{ CTX.title | truncate(255) | escape }}",
    "Description": "{{ CTX.description | escape }}",
    "DueDateTime": "{{ CTX.due_date | d(now() | datedelta(days=3) | format_datetime('%Y-%m-%dT%H:%M:%SZ')) }}",
    "Priority": {{ CTX.priority | d(3) }},
    "Status": {{ ORG.VARIABLES.at_status_new | d(1) }},
    "QueueID": {{ ORG.VARIABLES.at_default_queue }},
    "IssueType": {{ CTX.issue_type | d("null") }},
    "SubIssueType": {{ CTX.sub_issue_type | d("null") }},
    "TicketType": {{ CTX.ticket_type | d(2) }},
    "Source": {{ ORG.VARIABLES.at_source_rewst | d(8) }}
}
```

### Autotask Query Syntax

```jinja
{# Filter syntax uses JSON body #}
{
    "filter": [
        {
            "field": "Status",
            "op": "noteq",
            "value": 5
        },
        {
            "field": "QueueID",
            "op": "eq",
            "value": {{ ORG.VARIABLES.at_queue_id }}
        }
    ]
}

{# Operators: eq, noteq, gt, gte, lt, lte, contains, beginsWith, endsWith, in #}
```

### Autotask UDFs (User Defined Fields)

```jinja
{# Include UDFs in ticket creation #}
{
    "Title": "{{ CTX.title }}",
    "userDefinedFields": [
        {
            "name": "Automation Source",
            "value": "Rewst"
        },
        {
            "name": "Original Ticket ID",
            "value": "{{ CTX.source_ticket_id }}"
        }
    ]
}
```

---

## HaloPSA Deep Dive

### Ticket Creation

```jinja
{
    "summary": "{{ CTX.summary | escape }}",
    "details": "{{ CTX.details | escape }}",
    "client_id": {{ CTX.client_id }},
    "site_id": {{ CTX.site_id | d("null") }},
    "user_id": {{ CTX.user_id | d("null") }},
    "tickettype_id": {{ CTX.ticket_type_id | d(1) }},
    "category_1": {{ CTX.category_1 | d("null") }},
    "category_2": {{ CTX.category_2 | d("null") }},
    "category_3": {{ CTX.category_3 | d("null") }},
    "priority_id": {{ CTX.priority_id | d(4) }},
    "status_id": {{ ORG.VARIABLES.halo_status_new | d(1) }},
    "team": "{{ CTX.team | d("") }}",
    "agent_id": {{ CTX.agent_id | d("null") }},
    "sla_id": {{ CTX.sla_id | d("null") }},
    "dateoccurred": "{{ now() | format_datetime('%Y-%m-%dT%H:%M:%SZ') }}",
    "reportedby": "{{ CTX.reported_by | d("Rewst Automation") }}"
}
```

### HaloPSA Actions (Notes)

```jinja
{
    "ticket_id": {{ CTX.ticket_id }},
    "outcome": "{{ CTX.outcome | d("note") }}",
    "note": "{{ CTX.note_text | escape }}",
    "hiddenfromuser": {{ CTX.hidden | d(false) | lower }},
    "emailfromaddress": "{{ CTX.from_email | d("") }}",
    "who": "{{ CTX.performed_by | d("Rewst Automation") }}",
    "timetaken": {{ CTX.minutes | d(0) }},
    "sendemail": {{ CTX.send_email | d(false) | lower }}
}
```

### HaloPSA Query Parameters

```jinja
{# Common query parameters #}
?count=100
&page_no=1
&order=id
&orderdesc=false
&search={{ CTX.search_term | urlencode }}
&client_id={{ CTX.client_id }}
&status_id={{ CTX.status_id }}
&agent_id={{ CTX.agent_id }}
```

---

## Multi-PSA Abstraction Pattern

### Unified Ticket Interface

```jinja
{# Org variable determines PSA type #}
{% set psa_type = ORG.VARIABLES.default_psa %}

{% if psa_type == "cw_manage" %}
    {# ConnectWise format #}
    {% set ticket_payload = {
        "company": {"id": CTX.company_id},
        "summary": CTX.summary,
        "initialDescription": CTX.description
    } %}
{% elif psa_type == "datto_psa" %}
    {# Autotask format #}
    {% set ticket_payload = {
        "CompanyID": CTX.company_id,
        "Title": CTX.summary,
        "Description": CTX.description
    } %}
{% elif psa_type == "halo_psa" %}
    {# HaloPSA format #}
    {% set ticket_payload = {
        "client_id": CTX.client_id,
        "summary": CTX.summary,
        "details": CTX.description
    } %}
{% endif %}

{{ ticket_payload | to_json_string }}
```

### PSA Response Normalization

```jinja
{# Normalize ticket ID from response #}
{% if ORG.VARIABLES.default_psa == "cw_manage" %}
    {% set ticket_id = CTX.response.id %}
{% elif ORG.VARIABLES.default_psa == "datto_psa" %}
    {% set ticket_id = CTX.response.item.id %}
{% elif ORG.VARIABLES.default_psa == "halo_psa" %}
    {% set ticket_id = CTX.response.id %}
{% endif %}
```

---

## Documentation Integration Patterns

### IT Glue Flexible Asset

```jinja
{
    "data": {
        "type": "flexible-assets",
        "attributes": {
            "organization-id": {{ CTX.itglue_org_id }},
            "flexible-asset-type-id": {{ ORG.VARIABLES.itglue_user_asset_type }},
            "traits": {
                "name": "{{ CTX.user.displayName }}",
                "email": "{{ CTX.user.mail }}",
                "username": "{{ CTX.user.userPrincipalName }}",
                "job-title": "{{ CTX.user.jobTitle | d("") }}",
                "department": "{{ CTX.user.department | d("") }}",
                "manager": "{{ CTX.manager.displayName | d("") }}",
                "licenses": "{{ CTX.licenses | map(attribute='skuPartNumber') | join(', ') }}",
                "groups": "{{ CTX.groups | map(attribute='displayName') | join(', ') }}",
                "last-updated": "{{ now() | format_datetime('%Y-%m-%d %H:%M') }}",
                "status": "{{ 'Active' if CTX.user.accountEnabled else 'Disabled' }}"
            }
        }
    }
}
```

### Hudu Asset

```jinja
{
    "asset": {
        "asset_layout_id": {{ ORG.VARIABLES.hudu_user_layout_id }},
        "company_id": {{ CTX.hudu_company_id }},
        "name": "{{ CTX.user.displayName }}",
        "primary_serial": "{{ CTX.user.userPrincipalName }}",
        "fields": [
            {"label": "Email", "value": "{{ CTX.user.mail }}"},
            {"label": "Department", "value": "{{ CTX.user.department | d("N/A") }}"},
            {"label": "Job Title", "value": "{{ CTX.user.jobTitle | d("N/A") }}"},
            {"label": "Status", "value": "{{ 'Active' if CTX.user.accountEnabled else 'Disabled' }}"},
            {"label": "Last Sync", "value": "{{ now() | format_datetime('%Y-%m-%d %H:%M:%S') }}"}
        ]
    }
}
```
