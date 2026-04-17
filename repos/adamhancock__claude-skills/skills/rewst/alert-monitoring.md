# Rewst Alert & Monitoring Automation

Patterns for handling alerts from RMM systems, processing monitoring data, and automating incident response.

---

## Alert Source Integration

### RMM Alert Webhooks

```jinja
{# Datto RMM Alert Webhook Structure #}
{% set datto_alert = CTX.webhook_payload %}
{
    "device_uid": "{{ datto_alert.device.uid }}",
    "device_hostname": "{{ datto_alert.device.hostname }}",
    "site_uid": "{{ datto_alert.site.uid }}",
    "site_name": "{{ datto_alert.site.name }}",
    "alert_uid": "{{ datto_alert.alert.alertUid }}",
    "alert_type": "{{ datto_alert.alert.alertType }}",
    "alert_category": "{{ datto_alert.alert.category }}",
    "alert_message": "{{ datto_alert.alert.message }}",
    "priority": "{{ datto_alert.alert.priority }}",
    "timestamp": "{{ datto_alert.alert.timestamp }}",
    "resolved": {{ datto_alert.alert.resolved | d(false) | lower }}
}

{# NinjaRMM Alert Webhook Structure #}
{% set ninja_alert = CTX.webhook_payload %}
{
    "device_id": "{{ ninja_alert.device.id }}",
    "device_name": "{{ ninja_alert.device.systemName }}",
    "organization_id": "{{ ninja_alert.organization.id }}",
    "organization_name": "{{ ninja_alert.organization.name }}",
    "alert_id": "{{ ninja_alert.alert.id }}",
    "alert_type": "{{ ninja_alert.alert.sourceType }}",
    "alert_message": "{{ ninja_alert.alert.message }}",
    "severity": "{{ ninja_alert.alert.severity }}",
    "created_at": "{{ ninja_alert.alert.createTime }}",
    "cleared": {{ ninja_alert.alert.cleared | d(false) | lower }}
}

{# ConnectWise Automate Alert Structure #}
{% set cwa_alert = CTX.webhook_payload %}
{
    "computer_id": "{{ cwa_alert.ComputerID }}",
    "computer_name": "{{ cwa_alert.ComputerName }}",
    "client_id": "{{ cwa_alert.ClientID }}",
    "client_name": "{{ cwa_alert.ClientName }}",
    "location_id": "{{ cwa_alert.LocationID }}",
    "alert_id": "{{ cwa_alert.AlertID }}",
    "alert_template": "{{ cwa_alert.AlertTemplate }}",
    "alert_message": "{{ cwa_alert.Message }}",
    "severity": "{{ cwa_alert.Severity }}",
    "timestamp": "{{ cwa_alert.TimeStamp }}"
}
```

### Alert Normalization

```jinja
{# Normalize alerts from different sources #}
{% set source = CTX.alert_source | lower %}
{% set raw = CTX.raw_alert %}

{% if source == "datto_rmm" %}
    {% set alert = {
        "id": raw.alert.alertUid,
        "device_id": raw.device.uid,
        "device_name": raw.device.hostname,
        "company_id": raw.site.uid,
        "company_name": raw.site.name,
        "type": raw.alert.alertType,
        "category": raw.alert.category,
        "message": raw.alert.message,
        "severity": raw.alert.priority | d("medium"),
        "timestamp": raw.alert.timestamp,
        "is_resolved": raw.alert.resolved | d(false),
        "source": "datto_rmm"
    } %}
{% elif source == "ninja_rmm" %}
    {% set severity_map = {"CRITICAL": "critical", "MAJOR": "high", "MODERATE": "medium", "MINOR": "low"} %}
    {% set alert = {
        "id": raw.alert.id,
        "device_id": raw.device.id | string,
        "device_name": raw.device.systemName,
        "company_id": raw.organization.id | string,
        "company_name": raw.organization.name,
        "type": raw.alert.sourceType,
        "category": raw.alert.sourceConfigUid | d("general"),
        "message": raw.alert.message,
        "severity": severity_map[raw.alert.severity] | d("medium"),
        "timestamp": raw.alert.createTime,
        "is_resolved": raw.alert.cleared | d(false),
        "source": "ninja_rmm"
    } %}
{% elif source == "cw_automate" %}
    {% set severity_map = {1: "critical", 2: "high", 3: "medium", 4: "low", 5: "info"} %}
    {% set alert = {
        "id": raw.AlertID | string,
        "device_id": raw.ComputerID | string,
        "device_name": raw.ComputerName,
        "company_id": raw.ClientID | string,
        "company_name": raw.ClientName,
        "type": raw.AlertTemplate,
        "category": raw.Category | d("general"),
        "message": raw.Message,
        "severity": severity_map[raw.Severity | int] | d("medium"),
        "timestamp": raw.TimeStamp,
        "is_resolved": raw.Status == "Cleared",
        "source": "cw_automate"
    } %}
{% elif source == "n_able" %}
    {% set alert = {
        "id": raw.notification_id | string,
        "device_id": raw.device_id | string,
        "device_name": raw.device_name,
        "company_id": raw.customer_id | string,
        "company_name": raw.customer_name,
        "type": raw.check_name,
        "category": raw.check_type | d("general"),
        "message": raw.message,
        "severity": raw.priority | lower | d("medium"),
        "timestamp": raw.timestamp,
        "is_resolved": raw.status == "resolved",
        "source": "n_able"
    } %}
{% endif %}
```

---

## Alert Classification & Filtering

### Severity Mapping

```jinja
{# Map alert types to severity levels #}
{% set severity_rules = {
    "critical": [
        "ransomware",
        "disk_full",
        "backup_failed_critical",
        "server_down",
        "dc_unreachable",
        "security_breach",
        "av_disabled"
    ],
    "high": [
        "service_stopped",
        "high_cpu_sustained",
        "high_memory_sustained",
        "backup_failed",
        "certificate_expiring_soon",
        "disk_space_critical",
        "failed_login_multiple"
    ],
    "medium": [
        "patch_missing",
        "av_outdated",
        "disk_space_warning",
        "service_restart",
        "certificate_expiring",
        "software_update_available"
    ],
    "low": [
        "disk_space_low",
        "uptime_threshold",
        "scheduled_task_warning",
        "info_event"
    ]
} %}

{# Determine severity from alert type #}
{% set alert_type_lower = CTX.alert.type | lower | replace(" ", "_") %}
{% set ns = namespace(severity="medium") %}

{% for level, types in severity_rules.items() %}
    {% for type_pattern in types %}
        {% if type_pattern in alert_type_lower %}
            {% set ns.severity = level %}
        {% endif %}
    {% endfor %}
{% endfor %}

{% set calculated_severity = ns.severity %}
```

### Alert Deduplication

```jinja
{# Check for duplicate/related alerts #}
{% set current_alert = CTX.new_alert %}
{% set recent_alerts = CTX.recent_alerts | d([]) %}  {# Last 24 hours #}
{% set dedup_window_minutes = 30 %}

{# Find matching alerts #}
{% set duplicates = [] %}
{% for alert in recent_alerts %}
    {% set same_device = alert.device_id == current_alert.device_id %}
    {% set same_type = alert.type == current_alert.type %}
    {% set same_category = alert.category == current_alert.category %}

    {% set alert_time = alert.timestamp | as_datetime %}
    {% set current_time = current_alert.timestamp | as_datetime %}
    {% set time_diff_minutes = ((current_time - alert_time).total_seconds() / 60) | abs %}
    {% set within_window = time_diff_minutes <= dedup_window_minutes %}

    {% if same_device and same_type and same_category and within_window %}
        {% set _ = duplicates.append(alert) %}
    {% endif %}
{% endfor %}

{% set is_duplicate = duplicates | length > 0 %}
{% set original_alert = duplicates | first if is_duplicate else none %}
```

### Alert Suppression Rules

```jinja
{# Define suppression rules #}
{% set suppression_rules = [
    {
        "name": "Maintenance Window",
        "condition": "maintenance_active",
        "action": "suppress_all"
    },
    {
        "name": "Known Issue",
        "condition": "known_issue_active",
        "types": ["specific_alert_type"],
        "action": "suppress_matching"
    },
    {
        "name": "Low Priority Overnight",
        "condition": "after_hours",
        "severity": ["low", "info"],
        "action": "delay_until_morning"
    },
    {
        "name": "Repeat Suppression",
        "condition": "already_alerted",
        "cooldown_minutes": 60,
        "action": "suppress_until_cooldown"
    }
] %}

{# Check maintenance window #}
{% set maint_start = ORG.VARIABLES.maintenance_start | d(none) %}
{% set maint_end = ORG.VARIABLES.maintenance_end | d(none) %}
{% set in_maintenance = false %}

{% if maint_start and maint_end %}
    {% set now = now() %}
    {% set start = maint_start | as_datetime %}
    {% set end = maint_end | as_datetime %}
    {% set in_maintenance = start <= now <= end %}
{% endif %}

{# Check business hours #}
{% set current_hour = now() | format_datetime("%H") | int %}
{% set is_business_hours = 8 <= current_hour < 18 %}
{% set is_weekend = now() | format_datetime("%w") | int in [0, 6] %}
{% set after_hours = not is_business_hours or is_weekend %}
```

---

## Automated Response Patterns

### Self-Healing Actions

```jinja
{# Define auto-remediation rules #}
{% set remediation_rules = {
    "service_stopped": {
        "action": "restart_service",
        "params": {"service_name": "{{ CTX.alert.service_name }}"},
        "max_attempts": 3,
        "cooldown_minutes": 15
    },
    "disk_space_low": {
        "action": "clear_temp_files",
        "params": {"paths": ["%TEMP%", "C:\\Windows\\Temp"]},
        "max_attempts": 1,
        "cooldown_minutes": 60
    },
    "high_memory": {
        "action": "identify_memory_hogs",
        "params": {"threshold_mb": 1024},
        "max_attempts": 1,
        "cooldown_minutes": 30
    },
    "av_outdated": {
        "action": "update_av_definitions",
        "params": {},
        "max_attempts": 2,
        "cooldown_minutes": 120
    }
} %}

{% set alert_type = CTX.alert.type | lower | replace(" ", "_") %}
{% set rule = remediation_rules[alert_type] | d(none) %}

{% if rule %}
    {# Check if within cooldown #}
    {% set last_attempt = CTX.last_remediation_attempt | d(none) %}
    {% set can_attempt = true %}

    {% if last_attempt %}
        {% set elapsed = ((now() - (last_attempt | as_datetime)).total_seconds() / 60) | int %}
        {% set can_attempt = elapsed >= rule.cooldown_minutes %}
    {% endif %}

    {% if can_attempt and CTX.attempt_count | d(0) < rule.max_attempts %}
        {# Execute remediation #}
    {% endif %}
{% endif %}
```

### Service Restart Automation

```jinja
{# RMM-agnostic service restart #}
{% set rmm_type = ORG.VARIABLES.default_rmm %}
{% set device_id = CTX.alert.device_id %}
{% set service_name = CTX.alert.service_name %}

{% if rmm_type == "datto_rmm" %}
    {# Use Datto RMM Component #}
    {
        "action": "run_component",
        "component_id": "{{ ORG.VARIABLES.datto_restart_service_component }}",
        "variables": {
            "ServiceName": "{{ service_name }}"
        }
    }
{% elif rmm_type == "ninja_rmm" %}
    {# Use NinjaRMM Script #}
    {
        "action": "run_script",
        "script_id": "{{ ORG.VARIABLES.ninja_restart_service_script }}",
        "parameters": {
            "serviceName": "{{ service_name }}"
        }
    }
{% elif rmm_type == "cw_automate" %}
    {# Use CW Automate Script #}
    {
        "action": "run_script",
        "script_id": {{ ORG.VARIABLES.cwa_restart_service_script_id }},
        "parameters": {
            "@ServiceName@": "{{ service_name }}"
        }
    }
{% endif %}
```

### Disk Cleanup Automation

```jinja
{# PowerShell disk cleanup script #}
{% set cleanup_script %}
# Automated Disk Cleanup
$ErrorActionPreference = "SilentlyContinue"
$results = @{
    "before_free_gb" = 0
    "after_free_gb" = 0
    "cleaned_mb" = 0
    "items_cleaned" = @()
}

# Get initial free space
$drive = Get-WmiObject Win32_LogicalDisk -Filter "DeviceID='C:'"
$results.before_free_gb = [math]::Round($drive.FreeSpace / 1GB, 2)

# Cleanup locations
$cleanupPaths = @(
    "$env:TEMP",
    "C:\Windows\Temp",
    "C:\Windows\Prefetch",
    "C:\Windows\SoftwareDistribution\Download"
)

foreach ($path in $cleanupPaths) {
    if (Test-Path $path) {
        $size = (Get-ChildItem $path -Recurse -Force | Measure-Object -Property Length -Sum).Sum / 1MB
        Remove-Item "$path\*" -Recurse -Force
        $results.items_cleaned += @{path=$path; size_mb=[math]::Round($size, 2)}
    }
}

# Clear recycle bin
Clear-RecycleBin -Force

# Get final free space
$drive = Get-WmiObject Win32_LogicalDisk -Filter "DeviceID='C:'"
$results.after_free_gb = [math]::Round($drive.FreeSpace / 1GB, 2)
$results.cleaned_mb = [math]::Round(($results.after_free_gb - $results.before_free_gb) * 1024, 2)

$results | ConvertTo-Json -Depth 3
{% endset %}
```

---

## Alert Escalation

### Escalation Matrix

```jinja
{# Define escalation paths by severity and time #}
{% set escalation_matrix = {
    "critical": {
        "initial_notify": ["oncall_tech", "service_manager"],
        "escalate_after_minutes": 15,
        "escalate_to": ["technical_director", "vp_operations"],
        "max_escalations": 3,
        "notification_methods": ["email", "sms", "phone"]
    },
    "high": {
        "initial_notify": ["oncall_tech"],
        "escalate_after_minutes": 30,
        "escalate_to": ["service_manager"],
        "max_escalations": 2,
        "notification_methods": ["email", "sms"]
    },
    "medium": {
        "initial_notify": ["dispatch_queue"],
        "escalate_after_minutes": 120,
        "escalate_to": ["oncall_tech"],
        "max_escalations": 1,
        "notification_methods": ["email"]
    },
    "low": {
        "initial_notify": ["dispatch_queue"],
        "escalate_after_minutes": 480,
        "escalate_to": [],
        "max_escalations": 0,
        "notification_methods": ["email"]
    }
} %}

{% set config = escalation_matrix[CTX.alert.severity] | d(escalation_matrix["medium"]) %}
```

### On-Call Schedule Integration

```jinja
{# Determine current on-call technician #}
{% set schedules = CTX.oncall_schedules %}  {# List from scheduling system #}
{% set now = now() %}
{% set current_day = now | format_datetime("%A") %}
{% set current_hour = now | format_datetime("%H") | int %}

{# Find active schedule #}
{% set ns = namespace(oncall_tech=none) %}
{% for schedule in schedules %}
    {% if not ns.oncall_tech %}
        {% set start = schedule.start_time | as_datetime %}
        {% set end = schedule.end_time | as_datetime %}
        {% if start <= now <= end %}
            {% set ns.oncall_tech = schedule.technician %}
        {% endif %}
    {% endif %}
{% endfor %}

{# Fallback to default #}
{% set oncall_technician = ns.oncall_tech | d(ORG.VARIABLES.default_oncall_tech) %}
```

### Notification Templates

```jinja
{# Alert notification email template #}
{% set email_template %}
Subject: [{{ CTX.alert.severity | upper }}] Alert: {{ CTX.alert.type }} - {{ CTX.alert.device_name }}

**ALERT NOTIFICATION**

Severity: {{ CTX.alert.severity | upper }}
Time: {{ CTX.alert.timestamp | as_datetime | format_datetime("%Y-%m-%d %H:%M:%S %Z") }}

**Device Information:**
- Device: {{ CTX.alert.device_name }}
- Company: {{ CTX.alert.company_name }}
- IP Address: {{ CTX.alert.device_ip | d("Unknown") }}

**Alert Details:**
- Type: {{ CTX.alert.type }}
- Category: {{ CTX.alert.category }}
- Message: {{ CTX.alert.message }}

{% if CTX.alert.metrics %}
**Metrics:**
{% for key, value in CTX.alert.metrics.items() %}
- {{ key }}: {{ value }}
{% endfor %}
{% endif %}

{% if CTX.remediation_attempted %}
**Auto-Remediation:**
Action: {{ CTX.remediation_action }}
Result: {{ CTX.remediation_result }}
{% endif %}

**Actions Required:**
{% if CTX.alert.severity == "critical" %}
IMMEDIATE ATTENTION REQUIRED - Acknowledge within 15 minutes
{% elif CTX.alert.severity == "high" %}
Review and respond within 30 minutes
{% else %}
Review at next available opportunity
{% endif %}

---
Alert ID: {{ CTX.alert.id }}
Source: {{ CTX.alert.source }}
{% endset %}

{# SMS notification (short format) #}
{% set sms_template %}
[{{ CTX.alert.severity | upper }}] {{ CTX.alert.type }} on {{ CTX.alert.device_name }} ({{ CTX.alert.company_name }}). {{ CTX.alert.message | truncate(80) }}
{% endset %}
```

---

## Monitoring Dashboards

### Alert Summary Data

```jinja
{# Generate alert summary for dashboard #}
{% set alerts = CTX.all_alerts %}
{% set time_range_hours = CTX.time_range_hours | d(24) %}
{% set cutoff = now() | datedelta(hours=-time_range_hours) %}

{# Filter to time range #}
{% set recent_alerts = [] %}
{% for alert in alerts %}
    {% set alert_time = alert.timestamp | as_datetime %}
    {% if alert_time >= cutoff %}
        {% set _ = recent_alerts.append(alert) %}
    {% endif %}
{% endfor %}

{# Count by severity #}
{% set by_severity = {
    "critical": recent_alerts | selectattr("severity", "eq", "critical") | list | length,
    "high": recent_alerts | selectattr("severity", "eq", "high") | list | length,
    "medium": recent_alerts | selectattr("severity", "eq", "medium") | list | length,
    "low": recent_alerts | selectattr("severity", "eq", "low") | list | length
} %}

{# Count by status #}
{% set open_alerts = recent_alerts | selectattr("is_resolved", "eq", false) | list %}
{% set resolved_alerts = recent_alerts | selectattr("is_resolved", "eq", true) | list %}

{# Top affected devices #}
{% set device_counts = {} %}
{% for alert in recent_alerts %}
    {% set device = alert.device_name %}
    {% set current = device_counts[device] | d(0) %}
    {% set device_counts = device_counts | combine({device: current + 1}) %}
{% endfor %}

{# Calculate MTTR (Mean Time To Resolution) #}
{% set total_resolution_seconds = 0 %}
{% set resolved_count = 0 %}
{% for alert in resolved_alerts %}
    {% set created = alert.timestamp | as_datetime %}
    {% set resolved = alert.resolved_at | as_datetime if alert.resolved_at else now() %}
    {% set resolution_time = (resolved - created).total_seconds() %}
    {% set total_resolution_seconds = total_resolution_seconds + resolution_time %}
    {% set resolved_count = resolved_count + 1 %}
{% endfor %}
{% set mttr_minutes = ((total_resolution_seconds / resolved_count) / 60) | round(1) if resolved_count > 0 else 0 %}

{# Summary object #}
{% set summary = {
    "time_range_hours": time_range_hours,
    "total_alerts": recent_alerts | length,
    "open_alerts": open_alerts | length,
    "resolved_alerts": resolved_alerts | length,
    "by_severity": by_severity,
    "mttr_minutes": mttr_minutes,
    "top_devices": device_counts
} %}
```

### Trend Analysis

```jinja
{# Analyze alert trends over time #}
{% set alerts = CTX.historical_alerts %}  {# Last 30 days #}

{# Group by day #}
{% set daily_counts = {} %}
{% for alert in alerts %}
    {% set day = alert.timestamp | as_datetime | format_datetime("%Y-%m-%d") %}
    {% set current = daily_counts[day] | d(0) %}
    {% set daily_counts = daily_counts | combine({day: current + 1}) %}
{% endfor %}

{# Calculate daily average #}
{% set total_days = daily_counts.keys() | list | length %}
{% set total_alerts = alerts | length %}
{% set daily_average = (total_alerts / total_days) | round(1) if total_days > 0 else 0 %}

{# Identify spike days (>2x average) #}
{% set spike_days = [] %}
{% for day, count in daily_counts.items() %}
    {% if count > (daily_average * 2) %}
        {% set _ = spike_days.append({"date": day, "count": count}) %}
    {% endif %}
{% endfor %}

{# Alert type trends #}
{% set type_counts = {} %}
{% for alert in alerts %}
    {% set alert_type = alert.type %}
    {% set current = type_counts[alert_type] | d(0) %}
    {% set type_counts = type_counts | combine({alert_type: current + 1}) %}
{% endfor %}

{# Top alert types #}
{% set top_types = [] %}
{% for type_name, count in type_counts.items() %}
    {% set _ = top_types.append({"type": type_name, "count": count}) %}
{% endfor %}
{% set top_types = top_types | sort(attribute="count", reverse=true) %}
```

---

## Integration Patterns

### Alert to Ticket Workflow

```
Trigger: Webhook (RMM Alert)

1. Parse Alert
   └── Normalize alert data from source

2. Classify Alert
   ├── Determine severity
   ├── Check suppression rules
   └── Check deduplication

3. Decision Gate (Follow First)
   ├── Suppressed → Log and Exit
   ├── Duplicate → Update Existing Ticket
   └── New Alert → Continue

4. Auto-Remediation Check
   ├── Remediation Available?
   │   ├── Yes → Execute Remediation
   │   │        └── Success? → Add Note to Alert
   │   └── No → Continue
   └── Continue regardless

5. Create/Update Ticket
   ├── Find existing open ticket for device/type
   │   ├── Found → Add note to existing
   │   └── Not found → Create new ticket
   └── Set priority from severity mapping

6. Notifications
   ├── Get escalation config
   ├── Determine recipients
   └── Send notifications

7. Log Alert
   └── Store in audit/reporting system
```

### Bulk Alert Processing

```jinja
{# Process multiple alerts efficiently #}
{% set alerts = CTX.pending_alerts %}
{% set batch_size = 50 %}

{# Group alerts by device for efficiency #}
{% set by_device = {} %}
{% for alert in alerts %}
    {% set device_id = alert.device_id %}
    {% set existing = by_device[device_id] | d([]) %}
    {% set _ = existing.append(alert) %}
    {% set by_device = by_device | combine({device_id: existing}) %}
{% endfor %}

{# Process device groups #}
With Items: {{ by_device.items() | list }}
Concurrency: 10

{# For each device, consolidate alerts into single action #}
{% set device_id = item()[0] %}
{% set device_alerts = item()[1] %}

{# Find highest severity #}
{% set severity_order = ["critical", "high", "medium", "low"] %}
{% set highest_severity = "low" %}
{% for alert in device_alerts %}
    {% if severity_order.index(alert.severity) < severity_order.index(highest_severity) %}
        {% set highest_severity = alert.severity %}
    {% endif %}
{% endfor %}

{# Create consolidated ticket if multiple alerts #}
{% if device_alerts | length > 1 %}
    {% set subject = "[MULTIPLE ALERTS] {{ device_alerts | length }} alerts on {{ device_alerts[0].device_name }}" %}
{% else %}
    {% set subject = device_alerts[0].message %}
{% endif %}
```

### Alert Resolution Handling

```jinja
{# Handle alert resolution from RMM #}
{% set resolution = CTX.resolution_webhook %}

1. Find Related Ticket
   └── Search for ticket with alert ID in notes

2. If Ticket Found
   ├── Add resolution note
   ├── Update status (if auto-close enabled)
   └── Notify assigned technician

3. Update Alert Record
   ├── Mark as resolved
   ├── Record resolution time
   └── Calculate MTTR

4. Cancel Pending Escalations
   └── Remove from escalation queue
```

---

## Health Check Automation

### Scheduled System Checks

```jinja
{# Daily health check workflow #}
Trigger: Cron (0 6 * * *)  {# 6 AM daily #}

1. Check All Integrations
   ├── M365 - List 1 user
   ├── PSA - Get ticket count
   ├── RMM - List devices
   └── Record results

2. Evaluate Health
   {% set health_status = {
       "m365": CTX.m365_result.status_code == 200,
       "psa": CTX.psa_result.status_code == 200,
       "rmm": CTX.rmm_result.status_code == 200
   } %}
   {% set all_healthy = health_status.values() | list | select("eq", true) | list | length == health_status | length %}

3. Report Results
   ├── All Healthy → Log success
   └── Any Failed → Create alert ticket, notify team

4. Update Dashboard
   └── Store health status for reporting
```

### Backup Monitoring

```jinja
{# Check backup status across all clients #}
{% set backup_results = CTX.all_backup_jobs %}

{# Classify results #}
{% set failed = backup_results | selectattr("status", "eq", "Failed") | list %}
{% set warning = backup_results | selectattr("status", "eq", "Warning") | list %}
{% set success = backup_results | selectattr("status", "eq", "Success") | list %}
{% set missed = backup_results | selectattr("status", "eq", "Missed") | list %}

{# Identify critical failures (>24h since last good backup) #}
{% set critical_failures = [] %}
{% for job in failed + missed %}
    {% set last_success = job.last_successful_backup | as_datetime if job.last_successful_backup else none %}
    {% if not last_success or (now() - last_success).total_seconds() > 86400 %}
        {% set _ = critical_failures.append(job) %}
    {% endif %}
{% endfor %}

{# Generate report #}
{% set backup_report = {
    "total_jobs": backup_results | length,
    "successful": success | length,
    "failed": failed | length,
    "warning": warning | length,
    "missed": missed | length,
    "critical_failures": critical_failures,
    "success_rate": ((success | length) / (backup_results | length) * 100) | round(1) if backup_results else 0
} %}
```

---

## Best Practices

```markdown
## Alert & Monitoring Best Practices

### Alert Design
□ Normalize alerts from all sources immediately
□ Implement deduplication to prevent alert storms
□ Use consistent severity levels across all sources
□ Include device and company context in all alerts

### Response
□ Define clear escalation paths by severity
□ Implement auto-remediation for common issues
□ Set appropriate cooldowns to prevent action loops
□ Log all automated actions for audit trail

### Notifications
□ Match notification urgency to severity
□ Avoid alert fatigue with smart suppression
□ Use appropriate channels (email/SMS/call)
□ Include actionable information in notifications

### Monitoring
□ Track MTTR and resolution rates
□ Identify trending alert types
□ Review suppressed alerts periodically
□ Maintain health check automation

### Integration
□ Link alerts to tickets consistently
□ Update tickets when alerts resolve
□ Maintain alert history for reporting
□ Test alert flows regularly
```

