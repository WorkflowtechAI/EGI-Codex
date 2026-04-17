# Rewst Ticket Automation Patterns

Comprehensive patterns for PSA ticket creation, updates, routing, and automation across ConnectWise Manage, Datto/Autotask PSA, and HaloPSA.

---

## Ticket Creation Patterns

### Universal Ticket Structure

```jinja
{# Build ticket payload that works across PSAs #}
{% set ticket_data = {
    "summary": CTX.ticket_subject | truncate(100, true, "..."),
    "description": CTX.ticket_body,
    "priority": CTX.priority | d("Medium"),
    "type": CTX.ticket_type | d("Service Request"),
    "source": "Rewst Automation",
    "contact_id": CTX.contact_id,
    "company_id": CTX.company_id
} %}

{# Add optional fields if present #}
{% if CTX.due_date %}
    {% set ticket_data = ticket_data | combine({"due_date": CTX.due_date}) %}
{% endif %}
{% if CTX.assignee_id %}
    {% set ticket_data = ticket_data | combine({"assignee_id": CTX.assignee_id}) %}
{% endif %}
```

### ConnectWise Manage Ticket Creation

```jinja
{# POST /service/tickets #}
{
    "summary": "{{ CTX.subject | escape }}",
    "board": {
        "id": {{ ORG.VARIABLES.psa_default_board_id }}
    },
    "company": {
        "id": {{ CTX.company_id }}
    },
    "contact": {
        "id": {{ CTX.contact_id | d("null") }}
    },
    "priority": {
        "id": {{ CTX.priority_id | d(ORG.VARIABLES.psa_default_priority_id | d(4)) }}
    },
    "type": {
        "id": {{ CTX.type_id | d(ORG.VARIABLES.psa_default_type_id) }}
    },
    "status": {
        "id": {{ CTX.status_id | d(ORG.VARIABLES.psa_new_status_id) }}
    },
    "initialDescription": "{{ CTX.description | escape }}",
    "source": {
        "id": {{ ORG.VARIABLES.psa_automation_source_id | d(1) }}
    }
    {% if CTX.service_location_id %},
    "serviceLocation": {
        "id": {{ CTX.service_location_id }}
    }
    {% endif %}
    {% if CTX.agreement_id %},
    "agreement": {
        "id": {{ CTX.agreement_id }}
    }
    {% endif %}
}
```

### Datto/Autotask PSA Ticket Creation

```jinja
{# POST /Tickets #}
{
    "title": "{{ CTX.subject | escape }}",
    "description": "{{ CTX.description | escape }}",
    "companyID": {{ CTX.company_id }},
    "contactID": {{ CTX.contact_id | d("null") }},
    "priority": {{ CTX.priority | d(3) }},
    "status": {{ CTX.status | d(1) }},
    "queueID": {{ ORG.VARIABLES.autotask_default_queue_id }},
    "issueType": {{ CTX.issue_type | d(ORG.VARIABLES.autotask_default_issue_type) }},
    "subIssueType": {{ CTX.sub_issue_type | d("null") }},
    "ticketType": {{ CTX.ticket_type | d(1) }},
    "source": {{ ORG.VARIABLES.autotask_automation_source | d(8) }}
    {% if CTX.due_datetime %},
    "dueDateTime": "{{ CTX.due_datetime }}"
    {% endif %}
    {% if CTX.contract_id %},
    "contractID": {{ CTX.contract_id }}
    {% endif %}
}
```

### HaloPSA Ticket Creation

```jinja
{# POST /Tickets #}
{
    "summary": "{{ CTX.subject | escape }}",
    "details": "{{ CTX.description | escape }}",
    "client_id": {{ CTX.client_id }},
    "user_id": {{ CTX.user_id | d("null") }},
    "tickettype_id": {{ CTX.ticket_type_id | d(ORG.VARIABLES.halo_default_ticket_type) }},
    "priority_id": {{ CTX.priority_id | d(3) }},
    "status_id": {{ CTX.status_id | d(1) }},
    "team_id": {{ ORG.VARIABLES.halo_default_team_id | d("null") }},
    "category_1": {{ CTX.category_1 | d("null") }},
    "category_2": {{ CTX.category_2 | d("null") }},
    "category_3": {{ CTX.category_3 | d("null") }},
    "category_4": {{ CTX.category_4 | d("null") }}
    {% if CTX.sla_id %},
    "sla_id": {{ CTX.sla_id }}
    {% endif %}
}
```

---

## Ticket Routing & Assignment

### Intelligent Ticket Routing

```jinja
{# Route based on ticket content and keywords #}
{% set subject_lower = CTX.subject | lower %}
{% set description_lower = CTX.description | lower %}
{% set combined_text = subject_lower ~ " " ~ description_lower %}

{# Define routing rules #}
{% set routing_rules = [
    {"keywords": ["password", "reset", "locked out", "mfa", "2fa"], "board": "Security", "team": "identity"},
    {"keywords": ["slow", "performance", "crash", "freeze", "blue screen"], "board": "Desktop Support", "team": "desktop"},
    {"keywords": ["email", "outlook", "mailbox", "spam", "phishing"], "board": "Email", "team": "email"},
    {"keywords": ["network", "internet", "wifi", "vpn", "firewall"], "board": "Network", "team": "network"},
    {"keywords": ["server", "backup", "storage", "vmware", "hyper-v"], "board": "Infrastructure", "team": "server"},
    {"keywords": ["onboard", "new user", "new hire", "provisioning"], "board": "Onboarding", "team": "identity"},
    {"keywords": ["offboard", "termination", "leaving", "disable"], "board": "Offboarding", "team": "identity"},
    {"keywords": ["printer", "printing", "scanner"], "board": "Desktop Support", "team": "desktop"},
    {"keywords": ["phone", "voip", "teams calling", "3cx"], "board": "Voice", "team": "voice"}
] %}

{# Find matching rule #}
{% set ns = namespace(matched_rule=none) %}
{% for rule in routing_rules %}
    {% if not ns.matched_rule %}
        {% for keyword in rule.keywords %}
            {% if keyword in combined_text and not ns.matched_rule %}
                {% set ns.matched_rule = rule %}
            {% endif %}
        {% endfor %}
    {% endif %}
{% endfor %}

{# Apply routing or use defaults #}
{% set target_board = ns.matched_rule.board if ns.matched_rule else "General Support" %}
{% set target_team = ns.matched_rule.team if ns.matched_rule else "helpdesk" %}
```

### Priority-Based Assignment

```jinja
{# Map priority to response times and escalation #}
{% set priority_config = {
    "Critical": {
        "sla_minutes": 15,
        "escalate_after_minutes": 30,
        "notify_manager": true,
        "assign_to": "senior_tech"
    },
    "High": {
        "sla_minutes": 60,
        "escalate_after_minutes": 120,
        "notify_manager": false,
        "assign_to": "available_tech"
    },
    "Medium": {
        "sla_minutes": 240,
        "escalate_after_minutes": 480,
        "notify_manager": false,
        "assign_to": "queue"
    },
    "Low": {
        "sla_minutes": 1440,
        "escalate_after_minutes": 2880,
        "notify_manager": false,
        "assign_to": "queue"
    }
} %}

{% set config = priority_config[CTX.priority] | d(priority_config["Medium"]) %}
```

### Round-Robin Assignment

```jinja
{# Distribute tickets evenly across team #}
{% set team_members = CTX.available_technicians | d([]) %}
{% set current_index = ORG.VARIABLES.ticket_assignment_index | d(0) | int %}

{% if team_members | length > 0 %}
    {% set assignee_index = current_index % (team_members | length) %}
    {% set assignee = team_members[assignee_index] %}

    {# Store next index for future assignments #}
    {% set next_index = (current_index + 1) % 1000 %}  {# Reset at 1000 to prevent overflow #}
{% endif %}
```

### Workload-Based Assignment

```jinja
{# Assign to technician with lowest open ticket count #}
{% set tech_workloads = CTX.technician_stats %}  {# List of {id, name, open_count} #}

{# Filter to available technicians only #}
{% set available = tech_workloads | selectattr("available", "eq", true) | list %}

{# Sort by open ticket count (ascending) #}
{% set sorted_techs = available | sort(attribute="open_count") %}

{# Assign to least busy technician #}
{% set assignee = sorted_techs | first if sorted_techs else none %}
```

---

## Ticket Updates & Notes

### Adding Ticket Notes

```jinja
{# ConnectWise Manage - POST /service/tickets/{id}/notes #}
{
    "text": "{{ CTX.note_text | escape }}",
    "detailDescriptionFlag": {{ CTX.detail_description | d(false) | lower }},
    "internalAnalysisFlag": {{ CTX.internal | d(true) | lower }},
    "resolutionFlag": {{ CTX.resolution | d(false) | lower }},
    "member": {
        "identifier": "{{ ORG.VARIABLES.psa_api_member | d('API') }}"
    }
    {% if CTX.notify_contact %},
    "customerUpdatedFlag": true
    {% endif %}
}

{# Datto/Autotask - POST /TicketNotes #}
{
    "ticketID": {{ CTX.ticket_id }},
    "title": "{{ CTX.note_title | d('Automation Update') | escape }}",
    "description": "{{ CTX.note_text | escape }}",
    "noteType": {{ CTX.note_type | d(1) }},  {# 1=Internal, 2=External #}
    "publish": {{ CTX.publish | d(1) }}  {# 1=All Users, 2=Internal Only #}
}

{# HaloPSA - POST /Actions #}
{
    "ticket_id": {{ CTX.ticket_id }},
    "note": "{{ CTX.note_text | escape }}",
    "outcome": "{{ CTX.outcome | d('Note Added') }}",
    "hiddenfromuser": {{ (not CTX.visible_to_user | d(false)) | lower }},
    "sendemail": {{ CTX.send_email | d(false) | lower }}
}
```

### Status Updates

```jinja
{# Generic status update with note #}
{% set status_map = {
    "new": {"cwm": 1, "autotask": 1, "halo": 1},
    "in_progress": {"cwm": 2, "autotask": 5, "halo": 2},
    "waiting_customer": {"cwm": 3, "autotask": 7, "halo": 3},
    "waiting_vendor": {"cwm": 4, "autotask": 8, "halo": 4},
    "scheduled": {"cwm": 5, "autotask": 9, "halo": 5},
    "resolved": {"cwm": 6, "autotask": 5, "halo": 6},
    "closed": {"cwm": 7, "autotask": 5, "halo": 7}
} %}

{% set psa_type = ORG.VARIABLES.default_psa %}
{% set status_id = status_map[CTX.new_status][psa_type] | d(1) %}
```

### Time Entry Creation

```jinja
{# ConnectWise Manage - POST /time/entries #}
{
    "chargeToId": {{ CTX.ticket_id }},
    "chargeToType": "ServiceTicket",
    "member": {
        "identifier": "{{ CTX.technician_id | d(ORG.VARIABLES.psa_api_member) }}"
    },
    "timeStart": "{{ CTX.start_time | format_datetime('%Y-%m-%dT%H:%M:%SZ') }}",
    "timeEnd": "{{ CTX.end_time | format_datetime('%Y-%m-%dT%H:%M:%SZ') }}",
    "actualHours": {{ CTX.hours | d(0.25) }},
    "billableOption": "{{ CTX.billable | d('Billable') }}",
    "notes": "{{ CTX.work_description | d('Automated time entry') | escape }}",
    "workType": {
        "id": {{ ORG.VARIABLES.psa_default_work_type_id | d(1) }}
    }
}

{# Datto/Autotask - POST /TimeEntries #}
{
    "ticketID": {{ CTX.ticket_id }},
    "resourceID": {{ CTX.resource_id | d(ORG.VARIABLES.autotask_api_resource_id) }},
    "dateWorked": "{{ CTX.work_date | format_datetime('%Y-%m-%d') }}",
    "startDateTime": "{{ CTX.start_time }}",
    "endDateTime": "{{ CTX.end_time }}",
    "hoursWorked": {{ CTX.hours | d(0.25) }},
    "summaryNotes": "{{ CTX.work_description | escape }}",
    "internalNotes": "{{ CTX.internal_notes | d('') | escape }}",
    "billingCodeID": {{ CTX.billing_code_id | d(ORG.VARIABLES.autotask_default_billing_code) }}
}
```

---

## Ticket Triggers & Automation

### PSA Ticket Trigger Handling

```jinja
{# Ticket triggers provide CTX.trigger_ticket with ticket data #}

{# Check if this is a new ticket or update #}
{% set is_new = CTX.trigger_ticket.status_id in [1, ORG.VARIABLES.psa_new_status_id] %}
{% set is_reopened = CTX.trigger_ticket.previous_status == "Closed" and CTX.trigger_ticket.status != "Closed" %}

{# Extract key fields #}
{% set ticket_id = CTX.trigger_ticket.id %}
{% set company_id = CTX.trigger_ticket.company.id | d(CTX.trigger_ticket.companyID) %}
{% set subject = CTX.trigger_ticket.summary | d(CTX.trigger_ticket.title) %}
{% set priority = CTX.trigger_ticket.priority.name | d(CTX.trigger_ticket.priority) %}
```

### Auto-Response Patterns

```jinja
{# Generate acknowledgment email #}
{% set response_template %}
Hello {{ CTX.contact_name | d("Valued Customer") }},

Thank you for contacting {{ ORG.VARIABLES.company_name }} support.

Your request has been received and assigned ticket number: {{ CTX.ticket_number }}

**Subject:** {{ CTX.subject }}
**Priority:** {{ CTX.priority }}
**Expected Response:** {{ CTX.sla_response_time }}

{% if CTX.priority in ["Critical", "High"] %}
A technician will be reaching out to you shortly.
{% else %}
A technician will review your request and respond within our standard SLA timeframe.
{% endif %}

You can reply to this email to add additional information to your ticket.

Best regards,
{{ ORG.VARIABLES.company_name }} Support Team
{% endset %}
```

### Escalation Automation

```jinja
{# Check if ticket needs escalation #}
{% set ticket_created = CTX.ticket.created_date | as_datetime %}
{% set now = now() %}
{% set age_minutes = ((now - ticket_created).total_seconds() / 60) | int %}

{% set sla_config = {
    "Critical": 15,
    "High": 60,
    "Medium": 240,
    "Low": 1440
} %}

{% set sla_minutes = sla_config[CTX.ticket.priority] | d(240) %}
{% set needs_escalation = age_minutes > sla_minutes and CTX.ticket.status not in ["Closed", "Resolved"] %}

{% if needs_escalation %}
    {# Build escalation notification #}
    {% set escalation_message %}
TICKET ESCALATION ALERT

Ticket #{{ CTX.ticket.number }} has exceeded SLA.

Summary: {{ CTX.ticket.subject }}
Priority: {{ CTX.ticket.priority }}
Age: {{ age_minutes }} minutes (SLA: {{ sla_minutes }} minutes)
Company: {{ CTX.ticket.company_name }}
Current Status: {{ CTX.ticket.status }}
Assigned To: {{ CTX.ticket.assignee | d("Unassigned") }}
    {% endset %}
{% endif %}
```

---

## Ticket Querying & Reporting

### Fetch Open Tickets

```jinja
{# ConnectWise Manage - GET /service/tickets #}
conditions=status/name not contains "Closed" and status/name not contains "Resolved"&orderBy=priority/sort asc&pageSize=1000

{# Datto/Autotask - GET /Tickets #}
{
    "filter": [
        {"op": "notIn", "field": "status", "value": [5, 13]}
    ],
    "sort": [
        {"field": "priority", "direction": "desc"}
    ],
    "maxRecords": 500
}

{# HaloPSA - GET /Tickets #}
?statusid_not=7,8&order=priority_id&orderdesc=true&count=500
```

### Ticket Metrics Calculation

```jinja
{# Calculate key metrics from ticket list #}
{% set tickets = CTX.all_tickets | d([]) %}

{# Count by status #}
{% set open_count = tickets | selectattr("status", "ne", "Closed") | list | length %}
{% set closed_count = tickets | selectattr("status", "eq", "Closed") | list | length %}

{# Count by priority #}
{% set critical_count = tickets | selectattr("priority", "eq", "Critical") | list | length %}
{% set high_count = tickets | selectattr("priority", "eq", "High") | list | length %}

{# Calculate average age for open tickets #}
{% set open_tickets = tickets | selectattr("status", "ne", "Closed") | list %}
{% set total_age_hours = 0 %}
{% for ticket in open_tickets %}
    {% set created = ticket.created_date | as_datetime %}
    {% set age_hours = ((now() - created).total_seconds() / 3600) | round(2) %}
    {% set total_age_hours = total_age_hours + age_hours %}
{% endfor %}
{% set avg_age_hours = (total_age_hours / (open_tickets | length)) | round(2) if open_tickets else 0 %}

{# SLA compliance #}
{% set breached = tickets | selectattr("sla_breached", "eq", true) | list | length %}
{% set sla_compliance = (((tickets | length) - breached) / (tickets | length) * 100) | round(1) if tickets else 100 %}
```

### Ticket Search Patterns

```jinja
{# Search tickets by various criteria #}

{# By date range #}
{% set start_date = CTX.start_date | format_datetime("%Y-%m-%dT00:00:00Z") %}
{% set end_date = CTX.end_date | format_datetime("%Y-%m-%dT23:59:59Z") %}

{# ConnectWise conditions #}
conditions=dateEntered >= [{{ start_date }}] and dateEntered <= [{{ end_date }}]

{# By company #}
conditions=company/id = {{ CTX.company_id }}

{# By technician #}
conditions=resources/member/identifier = "{{ CTX.tech_identifier }}"

{# Combined complex query #}
conditions=board/id = {{ CTX.board_id }} and status/id in (1,2,3) and priority/id <= 2
```

---

## Cross-PSA Abstraction

### PSA-Agnostic Ticket Interface

```jinja
{# Normalize ticket data from any PSA #}
{% set psa_type = ORG.VARIABLES.default_psa %}
{% set raw_ticket = CTX.raw_ticket %}

{% if psa_type == "cw_manage" %}
    {% set ticket = {
        "id": raw_ticket.id,
        "number": raw_ticket.id | string,
        "subject": raw_ticket.summary,
        "description": raw_ticket.initialDescription | d(raw_ticket.initialInternalAnalysis),
        "status": raw_ticket.status.name,
        "status_id": raw_ticket.status.id,
        "priority": raw_ticket.priority.name,
        "priority_id": raw_ticket.priority.id,
        "company_id": raw_ticket.company.id,
        "company_name": raw_ticket.company.name,
        "contact_id": raw_ticket.contact.id if raw_ticket.contact else none,
        "contact_name": raw_ticket.contact.name if raw_ticket.contact else none,
        "assignee": raw_ticket.resources[0].member.name if raw_ticket.resources else none,
        "board": raw_ticket.board.name,
        "board_id": raw_ticket.board.id,
        "created_date": raw_ticket.dateEntered,
        "updated_date": raw_ticket.lastUpdated
    } %}
{% elif psa_type == "datto_psa" or psa_type == "autotask" %}
    {% set ticket = {
        "id": raw_ticket.id,
        "number": raw_ticket.ticketNumber,
        "subject": raw_ticket.title,
        "description": raw_ticket.description,
        "status": raw_ticket.status,
        "status_id": raw_ticket.status,
        "priority": raw_ticket.priority,
        "priority_id": raw_ticket.priority,
        "company_id": raw_ticket.companyID,
        "company_name": raw_ticket.companyName | d("Unknown"),
        "contact_id": raw_ticket.contactID,
        "contact_name": raw_ticket.contactName | d("Unknown"),
        "assignee": raw_ticket.assignedResourceName | d(none),
        "board": raw_ticket.queueName | d("Default"),
        "board_id": raw_ticket.queueID,
        "created_date": raw_ticket.createDate,
        "updated_date": raw_ticket.lastActivityDate
    } %}
{% elif psa_type == "halo_psa" %}
    {% set ticket = {
        "id": raw_ticket.id,
        "number": raw_ticket.id | string,
        "subject": raw_ticket.summary,
        "description": raw_ticket.details,
        "status": raw_ticket.status_name,
        "status_id": raw_ticket.status_id,
        "priority": raw_ticket.priority_name,
        "priority_id": raw_ticket.priority_id,
        "company_id": raw_ticket.client_id,
        "company_name": raw_ticket.client_name,
        "contact_id": raw_ticket.user_id,
        "contact_name": raw_ticket.user_name,
        "assignee": raw_ticket.agent_name | d(none),
        "board": raw_ticket.team_name | d("Default"),
        "board_id": raw_ticket.team_id,
        "created_date": raw_ticket.dateoccurred,
        "updated_date": raw_ticket.lastactiondate
    } %}
{% endif %}
```

### Universal Ticket Operations Subworkflow

```
Input Variables:
- operation: "create" | "update" | "add_note" | "close"
- ticket_data: dict with ticket fields

Output Variables:
- ticket_id: Created/updated ticket ID
- success: boolean
- error_message: string if failed

Workflow:
1. Get PSA Type
   └── {{ ORG.VARIABLES.default_psa }}

2. Route by Operation (Follow First)
   ├── create → Create Ticket Task
   ├── update → Update Ticket Task
   ├── add_note → Add Note Task
   └── close → Close Ticket Task

3. Normalize Response
   └── Return standardized result
```

---

## Common Ticket Scenarios

### New Employee Ticket Creation

```jinja
{# Create onboarding ticket with all details #}
{% set ticket_body %}
## New Employee Onboarding Request

**Employee Details:**
- Name: {{ CTX.employee_name }}
- Start Date: {{ CTX.start_date | format_datetime("%B %d, %Y") }}
- Department: {{ CTX.department }}
- Manager: {{ CTX.manager_name }}
- Job Title: {{ CTX.job_title }}

**IT Requirements:**
- Computer Type: {{ CTX.computer_type | d("Standard Laptop") }}
- Software Needs: {{ CTX.software_needs | join(", ") if CTX.software_needs else "Standard Suite" }}
- Phone Extension: {{ "Yes" if CTX.needs_phone else "No" }}
- VPN Access: {{ "Yes" if CTX.needs_vpn else "No" }}

**Account Setup:**
- Email: {{ CTX.email_address }}
- Username: {{ CTX.username }}
- Security Groups: {{ CTX.groups | join(", ") if CTX.groups else "Standard User" }}
- M365 License: {{ CTX.license_type | d("Business Basic") }}

**Notes:**
{{ CTX.additional_notes | d("None") }}

---
*This ticket was automatically generated by Rewst automation.*
{% endset %}

{
    "summary": "Onboarding: {{ CTX.employee_name }} - Start {{ CTX.start_date | format_datetime('%m/%d') }}",
    "initialDescription": "{{ ticket_body | escape }}"
}
```

### Alert-to-Ticket Conversion

```jinja
{# Convert monitoring alert to structured ticket #}
{% set alert = CTX.alert %}

{% set severity_to_priority = {
    "critical": "Critical",
    "high": "High",
    "warning": "Medium",
    "low": "Low",
    "info": "Low"
} %}

{% set ticket_body %}
## Monitoring Alert

**Alert Details:**
- Alert Name: {{ alert.name }}
- Severity: {{ alert.severity | upper }}
- Device: {{ alert.device_name }}
- Source: {{ alert.source | d("Unknown") }}
- Triggered: {{ alert.timestamp | as_datetime | format_datetime("%Y-%m-%d %H:%M:%S") }}

**Alert Message:**
{{ alert.message }}

{% if alert.metrics %}
**Metrics at Time of Alert:**
{% for key, value in alert.metrics.items() %}
- {{ key }}: {{ value }}
{% endfor %}
{% endif %}

{% if alert.recommended_action %}
**Recommended Action:**
{{ alert.recommended_action }}
{% endif %}

---
*Alert ID: {{ alert.id }}*
*Auto-generated ticket - do not modify alert reference*
{% endset %}

{
    "summary": "[ALERT] {{ alert.severity | upper }}: {{ alert.name }} on {{ alert.device_name }}",
    "priority": "{{ severity_to_priority[alert.severity | lower] | d('Medium') }}",
    "initialDescription": "{{ ticket_body | escape }}"
}
```

### Bulk Ticket Operations

```jinja
{# Close multiple tickets with resolution #}
{% set tickets_to_close = CTX.ticket_ids %}
{% set resolution_note = CTX.resolution_note | d("Bulk closed by automation") %}

{# Use With Items to process in parallel #}
With Items: {{ tickets_to_close }}
Concurrency: 5

{# Each iteration closes one ticket #}
{
    "status": {
        "id": {{ ORG.VARIABLES.psa_closed_status_id }}
    },
    "closedFlag": true,
    "automaticEmailContactFlag": {{ CTX.notify_contacts | d(false) | lower }}
}
```

### Parent/Child Ticket Linking

```jinja
{# Create child tickets for multi-step tasks #}
{% set parent_id = CTX.parent_ticket_id %}
{% set child_tasks = [
    {"subject": "Backup user data", "assignee": "backup_team"},
    {"subject": "Disable Active Directory account", "assignee": "identity_team"},
    {"subject": "Remove M365 licenses", "assignee": "cloud_team"},
    {"subject": "Archive mailbox", "assignee": "email_team"},
    {"subject": "Collect equipment", "assignee": "onsite_team"}
] %}

{# Create each child ticket linked to parent #}
With Items: {{ child_tasks }}

{
    "summary": "{{ item().subject }}",
    "parentTicket": {
        "id": {{ parent_id }}
    },
    "board": {
        "id": {{ ORG.VARIABLES.psa_default_board_id }}
    }
}
```

---

## Ticket Templates

### Standard Request Template

```jinja
{% set templates = {
    "password_reset": {
        "subject": "Password Reset Request - {{ CTX.user_name }}",
        "priority": "Medium",
        "type": "Service Request",
        "board": "Identity",
        "body": "User {{ CTX.user_name }} has requested a password reset for their account.\n\nEmail: {{ CTX.user_email }}\nPhone: {{ CTX.user_phone | d('Not provided') }}"
    },
    "new_software": {
        "subject": "Software Request - {{ CTX.software_name }} for {{ CTX.user_name }}",
        "priority": "Low",
        "type": "Service Request",
        "board": "Desktop Support",
        "body": "Software installation request.\n\nUser: {{ CTX.user_name }}\nSoftware: {{ CTX.software_name }}\nBusiness Justification: {{ CTX.justification | d('Not provided') }}\nApproved By: {{ CTX.approver | d('Pending approval') }}"
    },
    "hardware_issue": {
        "subject": "Hardware Issue - {{ CTX.device_type }} - {{ CTX.user_name }}",
        "priority": "High",
        "type": "Incident",
        "board": "Desktop Support",
        "body": "Hardware issue reported.\n\nUser: {{ CTX.user_name }}\nDevice: {{ CTX.device_type }}\nSerial: {{ CTX.serial_number | d('Unknown') }}\nIssue: {{ CTX.issue_description }}"
    }
} %}

{% set template = templates[CTX.template_type] | d(templates["hardware_issue"]) %}
```

### Dynamic Email Templates

```jinja
{# Generate ticket update email #}
{% set email_template %}
Subject: [Ticket #{{ CTX.ticket_number }}] {{ CTX.update_type | title }} - {{ CTX.ticket_subject | truncate(50) }}

Dear {{ CTX.contact_name | d("Customer") }},

{% if CTX.update_type == "status_change" %}
The status of your support ticket has been updated:

Previous Status: {{ CTX.previous_status }}
New Status: {{ CTX.new_status }}

{% elif CTX.update_type == "note_added" %}
A new update has been added to your support ticket:

{{ CTX.note_content }}

{% elif CTX.update_type == "resolution" %}
Great news! Your support ticket has been resolved.

Resolution Summary:
{{ CTX.resolution_summary }}

If this issue persists or you need additional assistance, simply reply to this email.

{% elif CTX.update_type == "scheduled" %}
Your support request has been scheduled:

Scheduled Date: {{ CTX.scheduled_date | format_datetime("%B %d, %Y") }}
Scheduled Time: {{ CTX.scheduled_time }}
Technician: {{ CTX.technician_name }}

{% endif %}

Ticket Details:
- Ticket Number: {{ CTX.ticket_number }}
- Subject: {{ CTX.ticket_subject }}
- Priority: {{ CTX.ticket_priority }}
- Current Status: {{ CTX.current_status }}

Thank you for your patience.

Best regards,
{{ ORG.VARIABLES.company_name }} Support Team
{{ ORG.VARIABLES.support_phone | d("") }}
{% endset %}
```

---

## Error Handling for Tickets

### Ticket Creation Failure Recovery

```jinja
{# Handle ticket creation errors #}
{% set result = TASKS.create_ticket.result.result %}
{% set status = result.status_code %}

{% if status == 201 or status == 200 %}
    {# Success - ticket created #}
    {% set ticket_id = result.data.id %}
{% elif status == 400 %}
    {# Bad request - log details and create fallback ticket #}
    {% set error = result.data.message | d("Unknown validation error") %}
    {# Create simplified ticket without problematic fields #}
{% elif status == 401 or status == 403 %}
    {# Auth error - integration may need reconnection #}
    {# Notify admin and queue for retry #}
{% elif status == 404 %}
    {# Resource not found - company/contact may not exist #}
    {# Create ticket without contact link #}
{% elif status == 429 %}
    {# Rate limited - queue for retry with backoff #}
{% else %}
    {# Unknown error - log full response #}
{% endif %}
```

### Validation Before Ticket Creation

```jinja
{# Validate required fields before API call #}
{% set errors = [] %}

{% if not CTX.subject or CTX.subject | trim | length == 0 %}
    {% set _ = errors.append("Subject is required") %}
{% endif %}

{% if not CTX.company_id %}
    {% set _ = errors.append("Company ID is required") %}
{% endif %}

{% if CTX.subject | length > 100 %}
    {% set _ = errors.append("Subject exceeds 100 character limit") %}
{% endif %}

{% set is_valid = errors | length == 0 %}

{% if not is_valid %}
    {# Return validation errors #}
    {{ {"success": false, "errors": errors} | to_json_string }}
{% endif %}
```

---

## Best Practices Checklist

```markdown
## Ticket Automation Best Practices

### Creation
□ Always include descriptive subject with key identifiers
□ Set appropriate priority based on impact/urgency
□ Link to correct company and contact when available
□ Use templates for consistent formatting
□ Include source reference (automation, alert ID, etc.)

### Updates
□ Add meaningful notes with context
□ Track time entries for billable work
□ Update status to reflect actual state
□ Notify relevant parties of significant changes

### Routing
□ Implement keyword-based routing for efficiency
□ Use round-robin or workload-based assignment
□ Configure escalation rules for SLA compliance
□ Route to appropriate board/queue

### Integration
□ Normalize data across different PSA systems
□ Handle PSA-specific field requirements
□ Implement error recovery for API failures
□ Log ticket operations for audit trail

### Performance
□ Use batching for bulk operations
□ Implement appropriate concurrency limits
□ Cache frequently-used lookups (companies, contacts)
□ Monitor API rate limits
```

