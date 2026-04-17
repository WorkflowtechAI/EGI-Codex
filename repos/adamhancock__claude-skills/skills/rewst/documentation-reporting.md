# Rewst Documentation & Reporting Patterns

Patterns for IT documentation tools integration, automated reporting, and documentation generation workflows.

---

## IT Documentation Tool Integration

### IT Glue Integration

```jinja
{# IT Glue API Base #}
{% set itg_base = "https://api.itglue.com" %}
{% set itg_headers = {
    "x-api-key": ORG.VARIABLES.itglue_api_key,
    "Content-Type": "application/vnd.api+json"
} %}

{# Get Organization by Name #}
{# GET /organizations?filter[name]={name} #}
{% set org_filter = "?filter[name]=" ~ CTX.company_name | urlencode %}

{# Get Organization Details #}
{# GET /organizations/{id} #}

{# Create/Update Flexible Asset #}
{# POST /flexible_assets #}
{
    "data": {
        "type": "flexible-assets",
        "attributes": {
            "organization-id": {{ CTX.org_id }},
            "flexible-asset-type-id": {{ CTX.asset_type_id }},
            "traits": {
                "{{ CTX.trait_name }}": "{{ CTX.trait_value }}"
                {% for key, value in CTX.additional_traits.items() %},
                "{{ key }}": "{{ value }}"
                {% endfor %}
            }
        }
    }
}

{# Create Configuration #}
{# POST /configurations #}
{
    "data": {
        "type": "configurations",
        "attributes": {
            "organization-id": {{ CTX.org_id }},
            "configuration-type-id": {{ CTX.config_type_id }},
            "name": "{{ CTX.config_name }}",
            "hostname": "{{ CTX.hostname | d('') }}",
            "primary-ip": "{{ CTX.ip_address | d('') }}",
            "mac-address": "{{ CTX.mac_address | d('') }}",
            "serial-number": "{{ CTX.serial_number | d('') }}",
            "asset-tag": "{{ CTX.asset_tag | d('') }}",
            "operating-system-name": "{{ CTX.os_name | d('') }}",
            "notes": "{{ CTX.notes | d('') | escape }}"
        }
    }
}
```

### Hudu Integration

```jinja
{# Hudu API Base #}
{% set hudu_base = ORG.VARIABLES.hudu_base_url ~ "/api/v1" %}
{% set hudu_headers = {
    "x-api-key": ORG.VARIABLES.hudu_api_key,
    "Content-Type": "application/json"
} %}

{# Get Company by Name #}
{# GET /companies?name={name} #}

{# Create Asset #}
{# POST /companies/{company_id}/assets #}
{
    "asset": {
        "name": "{{ CTX.asset_name }}",
        "asset_layout_id": {{ CTX.layout_id }},
        "primary_serial": "{{ CTX.serial_number | d('') }}",
        "primary_model": "{{ CTX.model | d('') }}",
        "primary_manufacturer": "{{ CTX.manufacturer | d('') }}",
        "custom_fields": [
            {% for field in CTX.custom_fields %}
            {
                "id": {{ field.id }},
                "value": "{{ field.value }}"
            }{{ "," if not loop.last }}
            {% endfor %}
        ]
    }
}

{# Create/Update Magic Dash Widget #}
{# POST /magic_dash #}
{
    "title": "{{ CTX.widget_title }}",
    "company_name": "{{ CTX.company_name }}",
    "content_link": "{{ CTX.link_url | d('') }}",
    "icon": "{{ CTX.icon | d('fa-info-circle') }}",
    "shade": "{{ CTX.shade | d('') }}",
    "message": "{{ CTX.message | escape }}"
}

{# Create Password #}
{# POST /companies/{company_id}/asset_passwords #}
{
    "asset_password": {
        "name": "{{ CTX.credential_name }}",
        "company_id": {{ CTX.company_id }},
        "password_folder_id": {{ CTX.folder_id | d("null") }},
        "username": "{{ CTX.username }}",
        "password": "{{ CTX.password }}",
        "url": "{{ CTX.url | d('') }}",
        "description": "{{ CTX.description | d('') | escape }}"
    }
}
```

### Confluence Integration

```jinja
{# Atlassian Confluence API #}
{% set confluence_base = ORG.VARIABLES.confluence_url ~ "/wiki/rest/api" %}

{# Create Page #}
{# POST /content #}
{
    "type": "page",
    "title": "{{ CTX.page_title }}",
    "space": {
        "key": "{{ CTX.space_key }}"
    },
    "ancestors": [
        {% if CTX.parent_page_id %}
        {"id": "{{ CTX.parent_page_id }}"}
        {% endif %}
    ],
    "body": {
        "storage": {
            "value": "{{ CTX.page_content | escape }}",
            "representation": "storage"
        }
    }
}

{# Update Page #}
{# PUT /content/{pageId} #}
{
    "id": "{{ CTX.page_id }}",
    "type": "page",
    "title": "{{ CTX.page_title }}",
    "version": {
        "number": {{ CTX.current_version + 1 }}
    },
    "body": {
        "storage": {
            "value": "{{ CTX.page_content | escape }}",
            "representation": "storage"
        }
    }
}

{# Confluence Storage Format (HTML-like) #}
{% set confluence_body %}
<h2>Overview</h2>
<p>{{ CTX.overview }}</p>

<h2>Details</h2>
<table>
    <tr><th>Property</th><th>Value</th></tr>
    {% for key, value in CTX.details.items() %}
    <tr><td>{{ key }}</td><td>{{ value }}</td></tr>
    {% endfor %}
</table>

<ac:structured-macro ac:name="info">
    <ac:rich-text-body>
        <p>Last updated: {{ now() | format_datetime("%Y-%m-%d %H:%M") }}</p>
    </ac:rich-text-body>
</ac:structured-macro>
{% endset %}
```

---

## Documentation Templates

### Server Documentation

```jinja
{# Generate server documentation #}
{% set server = CTX.server_data %}

{% set server_doc %}
# Server: {{ server.hostname }}

## General Information
| Property | Value |
|----------|-------|
| Hostname | {{ server.hostname }} |
| FQDN | {{ server.fqdn | d("N/A") }} |
| IP Address | {{ server.ip_address }} |
| Operating System | {{ server.os_name }} {{ server.os_version }} |
| Last Updated | {{ now() | format_datetime("%Y-%m-%d %H:%M") }} |

## Hardware Specifications
| Component | Details |
|-----------|---------|
| Manufacturer | {{ server.manufacturer | d("N/A") }} |
| Model | {{ server.model | d("N/A") }} |
| Serial Number | {{ server.serial_number | d("N/A") }} |
| CPU | {{ server.cpu_model }} ({{ server.cpu_cores }} cores) |
| RAM | {{ server.ram_gb }} GB |
| Total Storage | {{ server.storage_total_gb }} GB |

## Network Configuration
| Interface | IP Address | Subnet | Gateway |
|-----------|------------|--------|---------|
{% for nic in server.network_interfaces %}
| {{ nic.name }} | {{ nic.ip }} | {{ nic.subnet }} | {{ nic.gateway | d("N/A") }} |
{% endfor %}

## Installed Roles/Services
{% for role in server.roles %}
- {{ role }}
{% endfor %}

## Backup Information
| Property | Value |
|----------|-------|
| Backup Solution | {{ server.backup_solution | d("Not configured") }} |
| Last Backup | {{ server.last_backup | d("Never") }} |
| Backup Status | {{ server.backup_status | d("Unknown") }} |

## Notes
{{ server.notes | d("No additional notes.") }}

---
*Documentation generated automatically by Rewst.*
{% endset %}
```

### User Account Documentation

```jinja
{# Generate user account documentation #}
{% set user = CTX.user_data %}

{% set user_doc %}
# User Account: {{ user.display_name }}

## Account Information
| Property | Value |
|----------|-------|
| Display Name | {{ user.display_name }} |
| Username | {{ user.username }} |
| Email | {{ user.email }} |
| Employee ID | {{ user.employee_id | d("N/A") }} |
| Department | {{ user.department | d("N/A") }} |
| Job Title | {{ user.job_title | d("N/A") }} |
| Manager | {{ user.manager_name | d("N/A") }} |
| Start Date | {{ user.start_date | d("N/A") }} |

## Account Status
| System | Status | Last Login |
|--------|--------|------------|
| Microsoft 365 | {{ user.m365_status | d("N/A") }} | {{ user.m365_last_login | d("N/A") }} |
| Active Directory | {{ user.ad_status | d("N/A") }} | {{ user.ad_last_login | d("N/A") }} |
{% if user.okta_status %}
| Okta | {{ user.okta_status }} | {{ user.okta_last_login | d("N/A") }} |
{% endif %}

## Assigned Licenses
{% for license in user.licenses | d([]) %}
- {{ license.name }} ({{ license.sku }})
{% endfor %}
{% if not user.licenses %}
No licenses assigned.
{% endif %}

## Group Memberships
{% for group in user.groups | d([]) %}
- {{ group.name }}{% if group.description %} - {{ group.description }}{% endif %}
{% endfor %}
{% if not user.groups %}
No group memberships.
{% endif %}

## Devices
{% for device in user.devices | d([]) %}
- **{{ device.name }}** ({{ device.type }}) - Last seen: {{ device.last_seen | d("Unknown") }}
{% endfor %}
{% if not user.devices %}
No registered devices.
{% endif %}

## Security
| Property | Value |
|----------|-------|
| MFA Enabled | {{ "Yes" if user.mfa_enabled else "No" }} |
| MFA Methods | {{ user.mfa_methods | join(", ") if user.mfa_methods else "None" }} |
| Password Last Changed | {{ user.password_last_changed | d("Unknown") }} |
| Account Locked | {{ "Yes" if user.locked else "No" }} |

---
*Last updated: {{ now() | format_datetime("%Y-%m-%d %H:%M") }}*
{% endset %}
```

### Network Documentation

```jinja
{# Generate network documentation #}
{% set network = CTX.network_data %}

{% set network_doc %}
# Network Documentation: {{ network.site_name }}

## Site Information
| Property | Value |
|----------|-------|
| Site Name | {{ network.site_name }} |
| Address | {{ network.address | d("N/A") }} |
| Primary Contact | {{ network.primary_contact | d("N/A") }} |
| ISP | {{ network.isp | d("N/A") }} |
| WAN IP | {{ network.wan_ip | d("N/A") }} |

## Network Subnets
| Network | VLAN | Description | DHCP Range |
|---------|------|-------------|------------|
{% for subnet in network.subnets %}
| {{ subnet.network }}/{{ subnet.cidr }} | {{ subnet.vlan_id | d("N/A") }} | {{ subnet.description }} | {{ subnet.dhcp_range | d("N/A") }} |
{% endfor %}

## Key Network Devices
### Firewall/Router
| Property | Value |
|----------|-------|
| Model | {{ network.firewall.model | d("N/A") }} |
| IP Address | {{ network.firewall.ip | d("N/A") }} |
| Firmware | {{ network.firewall.firmware | d("N/A") }} |

### Switches
| Name | Model | IP Address | Ports |
|------|-------|------------|-------|
{% for switch in network.switches %}
| {{ switch.name }} | {{ switch.model }} | {{ switch.ip }} | {{ switch.port_count }} |
{% endfor %}

### Wireless Access Points
| Name | Model | IP Address | SSID(s) |
|------|-------|------------|---------|
{% for ap in network.access_points %}
| {{ ap.name }} | {{ ap.model }} | {{ ap.ip }} | {{ ap.ssids | join(", ") }} |
{% endfor %}

## DNS Configuration
| Type | Server | Purpose |
|------|--------|---------|
{% for dns in network.dns_servers %}
| {{ dns.type }} | {{ dns.ip }} | {{ dns.purpose | d("General") }} |
{% endfor %}

## VPN Configuration
{% if network.vpn %}
| Property | Value |
|----------|-------|
| Type | {{ network.vpn.type }} |
| Endpoint | {{ network.vpn.endpoint }} |
| Auth Method | {{ network.vpn.auth_method }} |
{% else %}
No VPN configured.
{% endif %}

---
*Documentation generated: {{ now() | format_datetime("%Y-%m-%d %H:%M") }}*
{% endset %}
```

---

## Automated Report Generation

### Executive Summary Report

```jinja
{# Monthly executive summary report #}
{% set report_month = CTX.report_month %}
{% set client = CTX.client_data %}

{% set exec_report %}
# {{ client.name }} - IT Monthly Summary
## {{ report_month }}

### Service Overview

| Metric | This Month | Last Month | Change |
|--------|------------|------------|--------|
| Total Tickets | {{ client.tickets_this_month }} | {{ client.tickets_last_month }} | {{ ((client.tickets_this_month - client.tickets_last_month) / client.tickets_last_month * 100) | round(1) if client.tickets_last_month > 0 else 0 }}% |
| Avg Resolution Time | {{ client.avg_resolution_hours | round(1) }}h | {{ client.prev_avg_resolution_hours | round(1) }}h | {{ (client.avg_resolution_hours - client.prev_avg_resolution_hours) | round(1) }}h |
| SLA Compliance | {{ client.sla_compliance }}% | {{ client.prev_sla_compliance }}% | {{ (client.sla_compliance - client.prev_sla_compliance) | round(1) }}% |
| User Satisfaction | {{ client.satisfaction_score }}/5 | {{ client.prev_satisfaction_score }}/5 | {{ (client.satisfaction_score - client.prev_satisfaction_score) | round(1) }} |

### Ticket Breakdown

| Category | Count | % of Total |
|----------|-------|------------|
{% for category in client.ticket_categories %}
| {{ category.name }} | {{ category.count }} | {{ ((category.count / client.tickets_this_month) * 100) | round(1) }}% |
{% endfor %}

### Security Status

| Item | Status |
|------|--------|
| Endpoint Protection | {{ "✅ All Protected" if client.all_endpoints_protected else "⚠️ " ~ client.unprotected_endpoints ~ " Unprotected" }} |
| Patch Compliance | {{ client.patch_compliance }}% |
| MFA Adoption | {{ client.mfa_adoption }}% |
| Security Incidents | {{ client.security_incidents }} |

### Backup Status

| Metric | Value |
|--------|-------|
| Protected Devices | {{ client.protected_devices }}/{{ client.total_devices }} |
| Backup Success Rate | {{ client.backup_success_rate }}% |
| Last Successful Test | {{ client.last_dr_test | d("Never") }} |

### Recommendations

{% for rec in client.recommendations %}
{{ loop.index }}. **{{ rec.title }}** - {{ rec.description }}
   - Priority: {{ rec.priority }}
   - Estimated Impact: {{ rec.impact }}

{% endfor %}

### Upcoming
{% for item in client.upcoming_items %}
- {{ item.date }}: {{ item.description }}
{% endfor %}

---
*Report generated: {{ now() | format_datetime("%Y-%m-%d %H:%M") }}*
*Prepared by: {{ ORG.VARIABLES.company_name }}*
{% endset %}
```

### Technical Health Report

```jinja
{# Technical health check report #}
{% set health = CTX.health_data %}

{% set health_report %}
# Technical Health Report: {{ health.client_name }}
## Generated: {{ now() | format_datetime("%Y-%m-%d %H:%M") }}

---

## Overall Health Score: {{ health.overall_score }}/100

{% if health.overall_score >= 90 %}
🟢 **Excellent** - Systems are performing optimally
{% elif health.overall_score >= 70 %}
🟡 **Good** - Minor issues detected
{% elif health.overall_score >= 50 %}
🟠 **Fair** - Several issues require attention
{% else %}
🔴 **Poor** - Critical issues need immediate attention
{% endif %}

---

## Server Health

| Server | CPU | Memory | Disk | Status |
|--------|-----|--------|------|--------|
{% for server in health.servers %}
| {{ server.name }} | {{ server.cpu_usage }}% | {{ server.memory_usage }}% | {{ server.disk_usage }}% | {{ "🟢" if server.healthy else "🔴" }} |
{% endfor %}

### Servers Requiring Attention
{% for server in health.servers | selectattr("healthy", "eq", false) | list %}
- **{{ server.name }}**: {{ server.issues | join(", ") }}
{% endfor %}
{% if health.servers | selectattr("healthy", "eq", false) | list | length == 0 %}
All servers healthy.
{% endif %}

---

## Workstation Health

| Metric | Value | Status |
|--------|-------|--------|
| Total Devices | {{ health.workstations.total }} | |
| Online (Last 24h) | {{ health.workstations.online_24h }} | {{ "🟢" if health.workstations.online_percent >= 90 else "🟠" }} |
| Patch Compliant | {{ health.workstations.patched }} ({{ health.workstations.patch_percent }}%) | {{ "🟢" if health.workstations.patch_percent >= 95 else "🟠" if health.workstations.patch_percent >= 80 else "🔴" }} |
| AV Current | {{ health.workstations.av_current }} ({{ health.workstations.av_percent }}%) | {{ "🟢" if health.workstations.av_percent >= 98 else "🔴" }} |

---

## Microsoft 365 Health

| Service | Status |
|---------|--------|
{% for service in health.m365_services %}
| {{ service.name }} | {{ "🟢 Operational" if service.status == "Healthy" else "🟠 " ~ service.status }} |
{% endfor %}

### License Utilization
| License | Assigned | Available | Utilization |
|---------|----------|-----------|-------------|
{% for license in health.m365_licenses %}
| {{ license.name }} | {{ license.assigned }} | {{ license.total - license.assigned }} | {{ ((license.assigned / license.total) * 100) | round(0) }}% |
{% endfor %}

---

## Security Posture

### Identity Security
| Control | Status | Details |
|---------|--------|---------|
| MFA Enabled (All Users) | {{ "✅" if health.security.mfa_all_users else "❌" }} | {{ health.security.mfa_coverage }}% coverage |
| Admin MFA | {{ "✅" if health.security.admin_mfa else "❌" }} | |
| Conditional Access | {{ "✅" if health.security.ca_enabled else "❌" }} | {{ health.security.ca_policy_count }} policies |
| Legacy Auth Blocked | {{ "✅" if health.security.legacy_blocked else "❌" }} | |

### Endpoint Security
| Control | Status | Details |
|---------|--------|---------|
| Antivirus Deployed | {{ "✅" if health.security.av_deployed else "❌" }} | {{ health.security.av_coverage }}% coverage |
| EDR Deployed | {{ "✅" if health.security.edr_deployed else "❌" }} | {{ health.security.edr_coverage }}% coverage |
| Disk Encryption | {{ "✅" if health.security.encryption_enabled else "❌" }} | {{ health.security.encryption_coverage }}% coverage |

---

## Backup Summary

| Category | Protected | Unprotected | Success Rate |
|----------|-----------|-------------|--------------|
| Servers | {{ health.backup.servers_protected }} | {{ health.backup.servers_unprotected }} | {{ health.backup.server_success_rate }}% |
| Workstations | {{ health.backup.workstations_protected }} | {{ health.backup.workstations_unprotected }} | {{ health.backup.workstation_success_rate }}% |
| M365 Data | {{ health.backup.m365_protected }} | {{ health.backup.m365_unprotected }} | {{ health.backup.m365_success_rate }}% |

---

## Action Items

{% for action in health.action_items | sort(attribute="priority") %}
### {{ loop.index }}. {{ action.title }}
- **Priority**: {{ action.priority }}
- **Category**: {{ action.category }}
- **Description**: {{ action.description }}
- **Recommendation**: {{ action.recommendation }}

{% endfor %}

---
*This report was automatically generated. For questions, contact {{ ORG.VARIABLES.support_email }}.*
{% endset %}
```

### Ticket Summary Report

```jinja
{# Weekly/Monthly ticket summary #}
{% set tickets = CTX.tickets %}
{% set period = CTX.period | d("This Week") %}

{# Calculate metrics #}
{% set total = tickets | length %}
{% set resolved = tickets | selectattr("status", "in", ["Closed", "Resolved"]) | list | length %}
{% set open = total - resolved %}

{# By priority #}
{% set critical = tickets | selectattr("priority", "eq", "Critical") | list | length %}
{% set high = tickets | selectattr("priority", "eq", "High") | list | length %}
{% set medium = tickets | selectattr("priority", "eq", "Medium") | list | length %}
{% set low = tickets | selectattr("priority", "eq", "Low") | list | length %}

{# Average resolution time #}
{% set resolved_tickets = tickets | selectattr("resolved_date") | list %}
{% set total_resolution_hours = 0 %}
{% for ticket in resolved_tickets %}
    {% set created = ticket.created_date | as_datetime %}
    {% set resolved = ticket.resolved_date | as_datetime %}
    {% set hours = ((resolved - created).total_seconds() / 3600) %}
    {% set total_resolution_hours = total_resolution_hours + hours %}
{% endfor %}
{% set avg_resolution = (total_resolution_hours / (resolved_tickets | length)) | round(1) if resolved_tickets else 0 %}

{% set ticket_report %}
# Ticket Summary Report
## Period: {{ period }}
## Generated: {{ now() | format_datetime("%Y-%m-%d %H:%M") }}

---

### Overview

| Metric | Value |
|--------|-------|
| Total Tickets | {{ total }} |
| Resolved | {{ resolved }} ({{ ((resolved/total)*100) | round(0) if total > 0 else 0 }}%) |
| Open | {{ open }} |
| Avg Resolution Time | {{ avg_resolution }}h |

### By Priority

| Priority | Count | Resolved | Open |
|----------|-------|----------|------|
| Critical | {{ critical }} | {{ tickets | selectattr("priority", "eq", "Critical") | selectattr("status", "in", ["Closed", "Resolved"]) | list | length }} | {{ tickets | selectattr("priority", "eq", "Critical") | rejectattr("status", "in", ["Closed", "Resolved"]) | list | length }} |
| High | {{ high }} | {{ tickets | selectattr("priority", "eq", "High") | selectattr("status", "in", ["Closed", "Resolved"]) | list | length }} | {{ tickets | selectattr("priority", "eq", "High") | rejectattr("status", "in", ["Closed", "Resolved"]) | list | length }} |
| Medium | {{ medium }} | {{ tickets | selectattr("priority", "eq", "Medium") | selectattr("status", "in", ["Closed", "Resolved"]) | list | length }} | {{ tickets | selectattr("priority", "eq", "Medium") | rejectattr("status", "in", ["Closed", "Resolved"]) | list | length }} |
| Low | {{ low }} | {{ tickets | selectattr("priority", "eq", "Low") | selectattr("status", "in", ["Closed", "Resolved"]) | list | length }} | {{ tickets | selectattr("priority", "eq", "Low") | rejectattr("status", "in", ["Closed", "Resolved"]) | list | length }} |

### Open Tickets Requiring Attention

{% for ticket in tickets | rejectattr("status", "in", ["Closed", "Resolved"]) | sort(attribute="created_date") | list %}
{% set age_hours = ((now() - (ticket.created_date | as_datetime)).total_seconds() / 3600) | round(1) %}
| {{ ticket.id }} | {{ ticket.subject | truncate(40) }} | {{ ticket.priority }} | {{ age_hours }}h old |
{% endfor %}

---

### By Category

| Category | Count | % |
|----------|-------|---|
{% for category in CTX.ticket_categories %}
| {{ category.name }} | {{ category.count }} | {{ ((category.count / total) * 100) | round(1) if total > 0 else 0 }}% |
{% endfor %}

---
{% endset %}
```

---

## Documentation Sync Patterns

### Auto-Sync Device Documentation

```jinja
{# Sync device info from RMM to documentation tool #}

1. Fetch Device from RMM
   └── Get device details (hostname, IP, specs, etc.)

2. Find/Create Documentation Record
   ├── Search documentation tool by hostname/serial
   │   ├── Found → Update existing record
   │   └── Not found → Create new record
   └── Map fields to documentation schema

3. Update Documentation
   ├── Core details (hostname, IP, OS)
   ├── Hardware specs
   ├── Software inventory
   ├── Backup status
   └── Last updated timestamp

4. Link Related Items
   ├── Link to company/organization
   ├── Link to user (if assigned)
   └── Link to related configurations
```

### User Account Documentation Sync

```jinja
{# Sync user accounts to documentation #}
{% set user = CTX.m365_user %}
{% set doc_tool = ORG.VARIABLES.documentation_tool %}  {# itglue, hudu #}

{# Build user documentation record #}
{% set user_record = {
    "name": user.displayName,
    "username": user.userPrincipalName | split("@") | first,
    "email": user.mail | d(user.userPrincipalName),
    "department": user.department | d(""),
    "job_title": user.jobTitle | d(""),
    "phone": user.mobilePhone | d(""),
    "office_location": user.officeLocation | d(""),
    "account_enabled": user.accountEnabled,
    "mfa_enabled": user.mfa_enabled | d(false),
    "licenses": user.licenses | map(attribute="skuPartNumber") | list | join(", "),
    "groups": user.groups | map(attribute="displayName") | list | join(", "),
    "last_sign_in": user.signInActivity.lastSignInDateTime | d("Never"),
    "created_date": user.createdDateTime,
    "last_sync": now() | format_datetime("%Y-%m-%dT%H:%M:%SZ")
} %}

{# Format for IT Glue #}
{% if doc_tool == "itglue" %}
    {% set itg_traits = {
        "display-name": user_record.name,
        "username": user_record.username,
        "email-address": user_record.email,
        "department": user_record.department,
        "job-title": user_record.job_title,
        "phone-number": user_record.phone,
        "account-status": "Active" if user_record.account_enabled else "Disabled",
        "mfa-status": "Enabled" if user_record.mfa_enabled else "Not Enabled",
        "licenses-assigned": user_record.licenses,
        "group-memberships": user_record.groups,
        "last-sign-in": user_record.last_sign_in
    } %}
{% endif %}

{# Format for Hudu #}
{% if doc_tool == "hudu" %}
    {% set hudu_fields = [
        {"label": "Username", "value": user_record.username},
        {"label": "Email", "value": user_record.email},
        {"label": "Department", "value": user_record.department},
        {"label": "Job Title", "value": user_record.job_title},
        {"label": "Phone", "value": user_record.phone},
        {"label": "Account Status", "value": "Active" if user_record.account_enabled else "Disabled"},
        {"label": "MFA Status", "value": "Enabled" if user_record.mfa_enabled else "Not Enabled"},
        {"label": "Licenses", "value": user_record.licenses},
        {"label": "Groups", "value": user_record.groups}
    ] %}
{% endif %}
```

---

## Email Report Distribution

### Format and Send Report

```jinja
{# Generate HTML email report #}
{% set email_html %}
<!DOCTYPE html>
<html>
<head>
    <style>
        body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; }
        .header { background: #2c3e50; color: white; padding: 20px; text-align: center; }
        .content { padding: 20px; }
        table { border-collapse: collapse; width: 100%; margin: 15px 0; }
        th, td { border: 1px solid #ddd; padding: 10px; text-align: left; }
        th { background: #f5f5f5; }
        .metric { display: inline-block; margin: 10px; padding: 15px; background: #f9f9f9; border-radius: 5px; text-align: center; }
        .metric-value { font-size: 24px; font-weight: bold; color: #2c3e50; }
        .metric-label { font-size: 12px; color: #666; }
        .status-good { color: #27ae60; }
        .status-warning { color: #f39c12; }
        .status-critical { color: #e74c3c; }
        .footer { background: #f5f5f5; padding: 15px; text-align: center; font-size: 12px; color: #666; }
    </style>
</head>
<body>
    <div class="header">
        <h1>{{ CTX.report_title }}</h1>
        <p>{{ CTX.report_period }}</p>
    </div>
    <div class="content">
        <div style="text-align: center;">
            {% for metric in CTX.key_metrics %}
            <div class="metric">
                <div class="metric-value {{ metric.status_class }}">{{ metric.value }}</div>
                <div class="metric-label">{{ metric.label }}</div>
            </div>
            {% endfor %}
        </div>

        <h2>Summary</h2>
        {{ CTX.summary_html }}

        {% if CTX.action_items %}
        <h2>Action Items</h2>
        <ul>
            {% for item in CTX.action_items %}
            <li><strong>{{ item.priority }}:</strong> {{ item.description }}</li>
            {% endfor %}
        </ul>
        {% endif %}
    </div>
    <div class="footer">
        <p>This report was automatically generated by {{ ORG.VARIABLES.company_name }}.</p>
        <p>Questions? Contact {{ ORG.VARIABLES.support_email }}</p>
    </div>
</body>
</html>
{% endset %}
```

### Schedule Report Distribution

```jinja
{# Report scheduling configuration #}
{% set report_schedules = [
    {
        "report_type": "executive_summary",
        "frequency": "monthly",
        "day_of_month": 1,
        "recipients": ["ceo@client.com", "cfo@client.com"],
        "format": "pdf"
    },
    {
        "report_type": "technical_health",
        "frequency": "weekly",
        "day_of_week": "Monday",
        "recipients": ["it_manager@client.com"],
        "format": "html"
    },
    {
        "report_type": "ticket_summary",
        "frequency": "daily",
        "time": "08:00",
        "recipients": ["helpdesk@msp.com"],
        "format": "html"
    }
] %}

{# Cron expressions for triggers #}
{% set cron_map = {
    "daily": "0 8 * * *",
    "weekly": "0 8 * * 1",
    "monthly": "0 8 1 * *"
} %}
```

---

## Best Practices

```markdown
## Documentation & Reporting Best Practices

### Documentation
□ Auto-sync device info from RMM to documentation tool
□ Include last-updated timestamps on all records
□ Link related assets (users ↔ devices ↔ configurations)
□ Maintain consistent naming conventions
□ Archive outdated records instead of deleting

### Reporting
□ Schedule recurring reports automatically
□ Include actionable insights, not just data
□ Use visual indicators (colors, icons) for status
□ Provide comparison to previous periods
□ Include clear recommendations

### Data Quality
□ Validate data before documentation sync
□ Handle missing/null values gracefully
□ Deduplicate records on sync
□ Log sync operations for troubleshooting
□ Implement error handling for API failures

### Distribution
□ Match report format to audience (exec vs technical)
□ Use HTML for email, PDF for formal distribution
□ Include report period clearly
□ Provide contact info for questions
□ Archive reports for historical reference
```

