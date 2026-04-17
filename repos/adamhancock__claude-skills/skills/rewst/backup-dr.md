# Rewst Backup & Disaster Recovery Patterns

Automation patterns for backup monitoring, disaster recovery workflows, and business continuity with Datto BCDR, Veeam, Acronis, and cloud backup solutions.

---

## Backup Monitoring Overview

### Multi-Vendor Status Normalization

```jinja
{# Normalize backup status from any vendor #}
{% set vendor = CTX.backup_vendor | lower %}
{% set raw_status = CTX.raw_backup_status %}

{% set status_mappings = {
    "datto": {
        "success": ["Success", "Successful"],
        "warning": ["Warning", "SuccessWithWarnings", "Partial"],
        "failed": ["Failed", "Error", "Critical"],
        "running": ["Running", "InProgress"],
        "missed": ["Missed", "Skipped", "NoBackup"]
    },
    "veeam": {
        "success": ["Success"],
        "warning": ["Warning"],
        "failed": ["Failed", "Error"],
        "running": ["Running", "InProgress", "Pending"],
        "missed": ["None", "Disabled"]
    },
    "acronis": {
        "success": ["ok", "completed"],
        "warning": ["warning", "completed_with_warnings"],
        "failed": ["error", "failed", "critical"],
        "running": ["running", "in_progress"],
        "missed": ["not_started", "canceled"]
    }
} %}

{% set mapping = status_mappings[vendor] | d(status_mappings["datto"]) %}

{# Find normalized status #}
{% set ns = namespace(normalized="unknown") %}
{% for status_type, values in mapping.items() %}
    {% if raw_status in values %}
        {% set ns.normalized = status_type %}
    {% endif %}
{% endfor %}

{% set backup_status = ns.normalized %}
```

### Backup Health Scoring

```jinja
{# Calculate backup health score for a device/client #}
{% set backup_jobs = CTX.backup_jobs %}  {# List of recent backup results #}
{% set lookback_days = CTX.lookback_days | d(7) %}
{% set cutoff = now() | datedelta(days=-lookback_days) %}

{# Filter to recent jobs #}
{% set recent_jobs = [] %}
{% for job in backup_jobs %}
    {% set job_time = job.timestamp | as_datetime %}
    {% if job_time >= cutoff %}
        {% set _ = recent_jobs.append(job) %}
    {% endif %}
{% endfor %}

{# Count by status #}
{% set total = recent_jobs | length %}
{% set success_count = recent_jobs | selectattr("status", "eq", "success") | list | length %}
{% set warning_count = recent_jobs | selectattr("status", "eq", "warning") | list | length %}
{% set failed_count = recent_jobs | selectattr("status", "eq", "failed") | list | length %}

{# Calculate health score (0-100) #}
{% if total > 0 %}
    {% set success_score = (success_count / total) * 100 %}
    {% set warning_penalty = (warning_count / total) * 10 %}
    {% set failure_penalty = (failed_count / total) * 50 %}
    {% set health_score = [0, success_score - warning_penalty - failure_penalty] | max | round(0) %}
{% else %}
    {% set health_score = 0 %}  {# No backups = unhealthy #}
{% endif %}

{# Health rating #}
{% if health_score >= 95 %}
    {% set health_rating = "Excellent" %}
{% elif health_score >= 80 %}
    {% set health_rating = "Good" %}
{% elif health_score >= 60 %}
    {% set health_rating = "Fair" %}
{% elif health_score >= 40 %}
    {% set health_rating = "Poor" %}
{% else %}
    {% set health_rating = "Critical" %}
{% endif %}
```

---

## Datto BCDR Integration

### Device Status Retrieval

```jinja
{# Datto API - Get all BCDR devices #}
{# GET /v1/bcdr/device #}

{# Response handling #}
{% set devices = TASKS.get_devices.result.result.data.items | d([]) %}

{# Normalize device data #}
{% set normalized_devices = [] %}
{% for device in devices %}
    {% set _ = normalized_devices.append({
        "id": device.serialNumber,
        "name": device.name,
        "model": device.model,
        "client_name": device.clientCompanyName,
        "registration_date": device.registrationDate,
        "service_plan": device.servicePlan,
        "local_storage_used_gb": (device.localStorageUsed / 1073741824) | round(2),
        "local_storage_available_gb": (device.localStorageAvailable / 1073741824) | round(2),
        "offsite_storage_used_gb": (device.offsiteStorageUsed / 1073741824) | round(2),
        "last_checkin": device.lastSeenDate,
        "active_tickets": device.activeTickets | d(0),
        "alerts": device.alertSummary | d({})
    }) %}
{% endfor %}
```

### Agent Backup Status

```jinja
{# Datto API - Get agents for a device #}
{# GET /v1/bcdr/device/{serialNumber}/asset/agent #}

{% set agents = TASKS.get_agents.result.result.data.items | d([]) %}

{% set agent_status = [] %}
{% for agent in agents %}
    {# Get latest backup info #}
    {% set last_backup = agent.lastSnapshot | d({}) %}
    {% set last_offsite = agent.lastOffsiteSnapshot | d({}) %}

    {% set status = {
        "agent_id": agent.agentUid,
        "name": agent.name,
        "os": agent.os,
        "ip_address": agent.localIp | d("Unknown"),
        "protected_volumes": agent.protectedVolumesCount | d(0),
        "local_snapshots": agent.localSnapshots | d(0),
        "offsite_snapshots": agent.offsiteSnapshots | d(0),
        "last_local_backup": last_backup.timestamp | d(none),
        "last_local_status": last_backup.status | d("Unknown"),
        "last_offsite_backup": last_offsite.timestamp | d(none),
        "last_offsite_status": last_offsite.status | d("Unknown"),
        "backup_size_gb": (agent.protectedSize / 1073741824) | round(2) if agent.protectedSize else 0,
        "paused": agent.isPaused | d(false),
        "archived": agent.isArchived | d(false)
    } %}

    {# Calculate hours since last backup #}
    {% if status.last_local_backup %}
        {% set last_time = status.last_local_backup | as_datetime %}
        {% set hours_since = ((now() - last_time).total_seconds() / 3600) | round(1) %}
        {% set status = status | combine({"hours_since_backup": hours_since}) %}
    {% else %}
        {% set status = status | combine({"hours_since_backup": -1}) %}
    {% endif %}

    {% set _ = agent_status.append(status) %}
{% endfor %}
```

### Screenshot Verification

```jinja
{# Datto API - Get screenshot verification results #}
{# GET /v1/bcdr/device/{serialNumber}/asset/agent/{agentUid}/screenshot #}

{% set screenshots = TASKS.get_screenshots.result.result.data.items | d([]) %}

{# Get most recent screenshot #}
{% set latest = screenshots | sort(attribute="timestamp", reverse=true) | first | d({}) %}

{% set verification = {
    "timestamp": latest.timestamp | d(none),
    "status": latest.status | d("Unknown"),  {# Success, Failed, Skipped #}
    "screenshot_url": latest.screenshotUrl | d(none),
    "screenshot_text": latest.screenshotText | d(""),  {# OCR results #}
    "boot_time_seconds": latest.bootTimeSeconds | d(none),
    "verification_type": latest.verificationType | d("local")  {# local, cloud #}
} %}

{# Determine if verification is healthy #}
{% set hours_since_verification = -1 %}
{% if verification.timestamp %}
    {% set ver_time = verification.timestamp | as_datetime %}
    {% set hours_since_verification = ((now() - ver_time).total_seconds() / 3600) | round(1) %}
{% endif %}

{% set verification_healthy = verification.status == "Success" and hours_since_verification <= 24 %}
```

### Local & Cloud Restore Operations

```jinja
{# Datto API - Initiate local restore #}
{# POST /v1/bcdr/device/{serialNumber}/asset/agent/{agentUid}/restore #}
{
    "snapshotTimestamp": "{{ CTX.snapshot_timestamp }}",
    "restoreType": "{{ CTX.restore_type | d('localVirt') }}",  {# localVirt, export, bmr #}
    "targetDevice": "{{ CTX.target_device | d('') }}",
    "options": {
        "networkMode": "{{ CTX.network_mode | d('isolated') }}",  {# isolated, bridged #}
        "assignedIp": "{{ CTX.assigned_ip | d('') }}",
        "cpuCount": {{ CTX.cpu_count | d(2) }},
        "memoryMb": {{ CTX.memory_mb | d(4096) }}
    }
}

{# Datto API - Initiate cloud restore (Datto Cloud) #}
{
    "snapshotTimestamp": "{{ CTX.snapshot_timestamp }}",
    "restoreType": "cloudVirt",
    "duration": {{ CTX.duration_hours | d(24) }},
    "options": {
        "instanceSize": "{{ CTX.instance_size | d('medium') }}"  {# small, medium, large #}
    }
}
```

---

## Veeam Integration

### Job Status Retrieval

```jinja
{# Veeam Enterprise Manager API #}
{# GET /api/jobs #}

{% set jobs = TASKS.get_jobs.result.result.data.Refs | d([]) %}

{# For each job, get detailed status #}
{# GET /api/jobs/{jobUid}/sessions?count=1 #}

{% set job_status = [] %}
{% for job in jobs %}
    {% set session = job.latest_session | d({}) %}

    {% set status = {
        "job_id": job.UID,
        "job_name": job.Name,
        "job_type": job.JobType,  {# Backup, Replication, Copy, etc. #}
        "last_run": session.EndTimeUTC | d(none),
        "last_status": session.Result | d("Unknown"),  {# Success, Warning, Failed #}
        "next_run": job.NextRun | d(none),
        "is_enabled": job.IsEnabled,
        "vm_count": job.VmCount | d(0),
        "backup_size_bytes": session.BackupStats.BackupSize | d(0),
        "data_size_bytes": session.BackupStats.DataSize | d(0),
        "dedupe_ratio": session.BackupStats.DedupRatio | d(1),
        "compress_ratio": session.BackupStats.CompressRatio | d(1)
    } %}

    {% set _ = job_status.append(status) %}
{% endfor %}

{# Summary statistics #}
{% set total_jobs = job_status | length %}
{% set failed_jobs = job_status | selectattr("last_status", "eq", "Failed") | list %}
{% set warning_jobs = job_status | selectattr("last_status", "eq", "Warning") | list %}
{% set success_jobs = job_status | selectattr("last_status", "eq", "Success") | list %}
```

### Repository Capacity

```jinja
{# Veeam Enterprise Manager - Get repositories #}
{# GET /api/repositories #}

{% set repos = TASKS.get_repos.result.result.data.Refs | d([]) %}

{% set repo_status = [] %}
{% for repo in repos %}
    {% set capacity_gb = (repo.Capacity / 1073741824) | round(2) %}
    {% set free_gb = (repo.FreeSpace / 1073741824) | round(2) %}
    {% set used_gb = capacity_gb - free_gb %}
    {% set used_percent = ((used_gb / capacity_gb) * 100) | round(1) if capacity_gb > 0 else 0 %}

    {% set status = {
        "id": repo.UID,
        "name": repo.Name,
        "type": repo.Type,  {# WinLocal, LinuxLocal, Cloud, etc. #}
        "capacity_gb": capacity_gb,
        "free_gb": free_gb,
        "used_gb": used_gb,
        "used_percent": used_percent,
        "is_out_of_date": repo.IsOutOfDate,
        "is_unavailable": repo.IsUnavailable
    } %}

    {# Determine health #}
    {% if used_percent >= 95 %}
        {% set status = status | combine({"health": "critical"}) %}
    {% elif used_percent >= 85 %}
        {% set status = status | combine({"health": "warning"}) %}
    {% else %}
        {% set status = status | combine({"health": "healthy"}) %}
    {% endif %}

    {% set _ = repo_status.append(status) %}
{% endfor %}
```

### Restore Operations

```jinja
{# Veeam - Start VM restore #}
{# POST /api/vmRestorePoints/{restorePointId}?action=restore #}

{# VM-level restore options #}
{
    "VmName": "{{ CTX.restore_vm_name | d(CTX.original_vm_name ~ '_restored') }}",
    "Host": "{{ CTX.target_host_uid }}",
    "Datastore": "{{ CTX.target_datastore_uid }}",
    "PowerOnAfterRestore": {{ CTX.power_on | d(false) | lower }},
    "QuickRollback": {{ CTX.quick_rollback | d(false) | lower }},
    "RestoreNetworkSettings": {{ CTX.restore_network | d(false) | lower }}
}

{# File-level restore (FLR) #}
{# POST /api/vmRestorePoints/{restorePointId}?action=startFLR #}

{# Instant VM recovery #}
{# POST /api/vmRestorePoints/{restorePointId}?action=startInstantRecovery #}
{
    "VmName": "{{ CTX.vm_name }}",
    "Host": "{{ CTX.target_host_uid }}",
    "PowerOn": {{ CTX.power_on | d(true) | lower }},
    "Network": {
        "Mode": "{{ CTX.network_mode | d('NoConnected') }}"  {# Original, NoConnected, Custom #}
    }
}
```

---

## Acronis Integration

### Tenant & Machine Status

```jinja
{# Acronis Cyber Protect Cloud API #}
{# GET /api/2/tenants/{tenant_id}/usage #}

{% set usage = TASKS.get_usage.result.result.data %}

{# Machine protection status #}
{# GET /api/2/agents #}

{% set agents = TASKS.get_agents.result.result.data.items | d([]) %}

{% set machine_status = [] %}
{% for agent in agents %}
    {% set status = {
        "id": agent.id,
        "name": agent.name,
        "hostname": agent.hostname,
        "os": agent.platform.os,
        "version": agent.version,
        "online": agent.online,
        "registered": agent.registered,
        "last_seen": agent.last_seen,
        "tenant_id": agent.tenant_id,
        "protection_status": agent.protection_status | d("unknown")
    } %}
    {% set _ = machine_status.append(status) %}
{% endfor %}
```

### Backup Plan Status

```jinja
{# Acronis - Get backup activities #}
{# GET /api/2/activities #}

{% set activities = TASKS.get_activities.result.result.data.items | d([]) %}

{# Filter to backup activities #}
{% set backup_activities = activities | selectattr("type", "eq", "backup") | list %}

{# Group by resource (machine) #}
{% set by_machine = {} %}
{% for activity in backup_activities %}
    {% set machine_id = activity.resource_id %}
    {% set existing = by_machine[machine_id] | d([]) %}
    {% set _ = existing.append(activity) %}
    {% set by_machine = by_machine | combine({machine_id: existing}) %}
{% endfor %}

{# Get latest status per machine #}
{% set latest_status = {} %}
{% for machine_id, activities in by_machine.items() %}
    {% set sorted_activities = activities | sort(attribute="start_time", reverse=true) %}
    {% set latest = sorted_activities | first %}
    {% set latest_status = latest_status | combine({machine_id: {
        "last_backup": latest.start_time,
        "status": latest.state,  {# ok, warning, error, running #}
        "bytes_saved": latest.bytes_saved | d(0),
        "duration_seconds": latest.duration | d(0)
    }}) %}
{% endfor %}
```

### Recovery Operations

```jinja
{# Acronis - Start recovery #}
{# POST /api/2/recoveries #}
{
    "resource_id": "{{ CTX.resource_id }}",
    "backup_id": "{{ CTX.backup_id }}",
    "archive_id": "{{ CTX.archive_id }}",
    "recovery_point_id": "{{ CTX.recovery_point_id }}",
    "type": "{{ CTX.recovery_type }}",  {# entire_machine, disks_volumes, files_folders #}
    "destination": {
        "type": "{{ CTX.destination_type }}",  {# original, new_location, cloud #}
        {% if CTX.destination_type == "new_location" %}
        "path": "{{ CTX.destination_path }}",
        {% endif %}
    },
    "options": {
        "overwrite": {{ CTX.overwrite | d(false) | lower }},
        "preserve_permissions": {{ CTX.preserve_permissions | d(true) | lower }}
    }
}
```

---

## Cloud Backup Integration

### Microsoft 365 Backup Status

```jinja
{# Pattern for third-party M365 backup tools (Veeam, Datto SaaS, etc.) #}

{# Check Exchange Online backup status #}
{% set mailbox_backups = CTX.exchange_backups %}
{% set total_mailboxes = mailbox_backups | length %}
{% set protected_mailboxes = mailbox_backups | selectattr("last_backup") | list | length %}
{% set unprotected_mailboxes = total_mailboxes - protected_mailboxes %}

{# Check SharePoint/OneDrive backup status #}
{% set site_backups = CTX.sharepoint_backups %}
{% set onedrive_backups = CTX.onedrive_backups %}

{# Calculate protection coverage #}
{% set exchange_coverage = ((protected_mailboxes / total_mailboxes) * 100) | round(1) if total_mailboxes > 0 else 0 %}

{# Identify gaps #}
{% set stale_backups = [] %}
{% set stale_threshold_hours = 48 %}
{% for backup in mailbox_backups %}
    {% if backup.last_backup %}
        {% set last_time = backup.last_backup | as_datetime %}
        {% set hours_since = ((now() - last_time).total_seconds() / 3600) | round(1) %}
        {% if hours_since > stale_threshold_hours %}
            {% set _ = stale_backups.append(backup) %}
        {% endif %}
    {% else %}
        {% set _ = stale_backups.append(backup) %}
    {% endif %}
{% endfor %}
```

### Azure Backup Status

```jinja
{# Azure Backup - Recovery Services Vault status #}
{# GET /subscriptions/{sub}/resourceGroups/{rg}/providers/Microsoft.RecoveryServices/vaults/{vault}/backupJobs #}

{% set jobs = TASKS.get_backup_jobs.result.result.data.value | d([]) %}

{# Summarize by status #}
{% set by_status = {} %}
{% for job in jobs %}
    {% set status = job.properties.status %}
    {% set current = by_status[status] | d(0) %}
    {% set by_status = by_status | combine({status: current + 1}) %}
{% endfor %}

{# Azure Backup - Protected Items #}
{# GET /subscriptions/{sub}/resourceGroups/{rg}/providers/Microsoft.RecoveryServices/vaults/{vault}/backupProtectedItems #}

{% set protected_items = TASKS.get_protected_items.result.result.data.value | d([]) %}

{% set item_status = [] %}
{% for item in protected_items %}
    {% set props = item.properties %}
    {% set status = {
        "id": item.id,
        "name": item.name,
        "type": props.workloadType,  {# VM, AzureFileShare, SQLDatabase, etc. #}
        "policy_name": props.policyName,
        "protection_status": props.protectionStatus,
        "protection_state": props.protectionState,
        "health_status": props.healthStatus,
        "last_backup": props.lastBackupTime | d(none),
        "last_backup_status": props.lastBackupStatus | d("Unknown")
    } %}
    {% set _ = item_status.append(status) %}
{% endfor %}
```

---

## Backup Reporting

### Daily Backup Report

```jinja
{# Generate daily backup summary report #}
{% set all_results = CTX.all_backup_results %}
{% set report_date = now() | format_datetime("%Y-%m-%d") %}

{# Calculate totals #}
{% set total = all_results | length %}
{% set success = all_results | selectattr("status", "eq", "success") | list | length %}
{% set warning = all_results | selectattr("status", "eq", "warning") | list | length %}
{% set failed = all_results | selectattr("status", "eq", "failed") | list | length %}
{% set missed = all_results | selectattr("status", "eq", "missed") | list | length %}

{% set success_rate = ((success / total) * 100) | round(1) if total > 0 else 0 %}

{# Group by client #}
{% set by_client = {} %}
{% for result in all_results %}
    {% set client = result.client_name %}
    {% set existing = by_client[client] | d({"success": 0, "failed": 0, "warning": 0}) %}
    {% set existing = existing | combine({result.status: existing[result.status] + 1}) %}
    {% set by_client = by_client | combine({client: existing}) %}
{% endfor %}

{# Identify problem clients (any failures) #}
{% set problem_clients = [] %}
{% for client, stats in by_client.items() %}
    {% if stats.failed > 0 or stats.warning > 1 %}
        {% set _ = problem_clients.append({"client": client, "stats": stats}) %}
    {% endif %}
{% endfor %}

{# Generate report #}
{% set report %}
# Daily Backup Report - {{ report_date }}

## Summary
- **Total Jobs:** {{ total }}
- **Successful:** {{ success }} ({{ success_rate }}%)
- **Warnings:** {{ warning }}
- **Failed:** {{ failed }}
- **Missed:** {{ missed }}

## Critical Issues (Requires Attention)
{% if failed > 0 %}
{% for result in all_results | selectattr("status", "eq", "failed") | list %}
- {{ result.client_name }} / {{ result.job_name }} - {{ result.error_message | d("Unknown error") }}
{% endfor %}
{% else %}
No critical failures.
{% endif %}

## Warnings
{% if warning > 0 %}
{% for result in all_results | selectattr("status", "eq", "warning") | list %}
- {{ result.client_name }} / {{ result.job_name }} - {{ result.warning_message | d("Warning") }}
{% endfor %}
{% else %}
No warnings.
{% endif %}

## Client Summary
| Client | Success | Warning | Failed |
|--------|---------|---------|--------|
{% for client, stats in by_client.items() %}
| {{ client }} | {{ stats.success }} | {{ stats.warning }} | {{ stats.failed }} |
{% endfor %}
{% endset %}
```

### SLA Compliance Report

```jinja
{# Calculate SLA compliance for backup jobs #}
{% set jobs = CTX.backup_jobs %}
{% set sla_window_hours = CTX.sla_window_hours | d(24) %}

{% set compliance_results = [] %}
{% for job in jobs %}
    {% set job_sla = job.sla_hours | d(sla_window_hours) %}

    {# Check if backup occurred within SLA #}
    {% if job.last_backup %}
        {% set last_time = job.last_backup | as_datetime %}
        {% set hours_since = ((now() - last_time).total_seconds() / 3600) | round(2) %}
        {% set in_compliance = hours_since <= job_sla %}
    {% else %}
        {% set hours_since = -1 %}
        {% set in_compliance = false %}
    {% endif %}

    {% set result = {
        "job_name": job.name,
        "client": job.client_name,
        "sla_hours": job_sla,
        "hours_since_backup": hours_since,
        "in_compliance": in_compliance,
        "status": job.last_status | d("Unknown")
    } %}
    {% set _ = compliance_results.append(result) %}
{% endfor %}

{# Overall compliance rate #}
{% set total = compliance_results | length %}
{% set compliant = compliance_results | selectattr("in_compliance", "eq", true) | list | length %}
{% set compliance_rate = ((compliant / total) * 100) | round(1) if total > 0 else 0 %}

{# Non-compliant jobs for alerting #}
{% set non_compliant = compliance_results | selectattr("in_compliance", "eq", false) | list %}
```

---

## Disaster Recovery Workflows

### DR Test Automation

```jinja
{# Automated DR test workflow #}

1. Prepare DR Test
   ├── Select test target (device/VM)
   ├── Identify latest backup/snapshot
   ├── Verify offsite copy available
   └── Create DR test ticket

2. Execute Recovery
   ├── Initiate recovery to isolated environment
   ├── Wait for boot completion
   ├── Perform screenshot verification
   └── Run connectivity tests

3. Validate Recovery
   ├── Check services running
   ├── Test application functionality
   ├── Verify data integrity (spot checks)
   └── Document test results

4. Cleanup
   ├── Shutdown test VM
   ├── Delete temporary resources
   └── Generate test report

5. Report & Document
   ├── Update DR documentation
   ├── Close DR test ticket
   └── Schedule next test
```

### DR Failover Checklist

```jinja
{# DR failover checklist as Jinja template #}
{% set dr_checklist %}
## Disaster Recovery Failover Checklist

### Pre-Failover
- [ ] Confirm primary site is unavailable
- [ ] Verify DR site readiness
- [ ] Notify stakeholders of DR activation
- [ ] Review RTO/RPO requirements
- [ ] Document current timestamp: {{ now() | format_datetime("%Y-%m-%d %H:%M:%S %Z") }}

### Critical Systems (Priority 1)
{% for system in CTX.critical_systems %}
- [ ] {{ system.name }}
  - Backup timestamp: {{ system.last_backup }}
  - Recovery target: {{ system.dr_target }}
  - Dependencies: {{ system.dependencies | join(", ") }}
{% endfor %}

### Important Systems (Priority 2)
{% for system in CTX.important_systems %}
- [ ] {{ system.name }}
  - Backup timestamp: {{ system.last_backup }}
  - Recovery target: {{ system.dr_target }}
{% endfor %}

### Network & Access
- [ ] Update DNS records
- [ ] Configure VPN access to DR site
- [ ] Enable remote access for staff
- [ ] Test authentication services

### Post-Failover Validation
- [ ] All critical systems operational
- [ ] Users can access applications
- [ ] External services connected
- [ ] Monitoring enabled for DR environment

### Communication
- [ ] Internal team notified
- [ ] Customers notified (if applicable)
- [ ] Vendors notified (if applicable)
- [ ] Status page updated
{% endset %}
```

### Recovery Time Tracking

```jinja
{# Track recovery time for RTO compliance #}
{% set recovery_start = CTX.recovery_start_time | as_datetime %}
{% set current_time = now() %}

{# Calculate elapsed time #}
{% set elapsed_seconds = (current_time - recovery_start).total_seconds() %}
{% set elapsed_minutes = (elapsed_seconds / 60) | round(1) %}
{% set elapsed_hours = (elapsed_seconds / 3600) | round(2) %}

{# Check against RTO #}
{% set rto_hours = CTX.rto_hours | d(4) %}
{% set rto_seconds = rto_hours * 3600 %}
{% set time_remaining_seconds = rto_seconds - elapsed_seconds %}
{% set time_remaining_minutes = (time_remaining_seconds / 60) | round(0) %}

{% set rto_status = "on_track" if time_remaining_seconds > 0 else "breached" %}

{# Warning threshold (80% of RTO) #}
{% set warning_threshold = rto_seconds * 0.8 %}
{% if elapsed_seconds > warning_threshold and elapsed_seconds < rto_seconds %}
    {% set rto_status = "warning" %}
{% endif %}

{% set recovery_metrics = {
    "start_time": recovery_start | format_datetime("%Y-%m-%d %H:%M:%S"),
    "elapsed_minutes": elapsed_minutes,
    "elapsed_hours": elapsed_hours,
    "rto_hours": rto_hours,
    "time_remaining_minutes": time_remaining_minutes | int if time_remaining_minutes > 0 else 0,
    "rto_status": rto_status,
    "percentage_complete": CTX.recovery_progress | d(0)
} %}
```

---

## Automated Remediation

### Failed Backup Remediation

```jinja
{# Auto-remediation for common backup failures #}
{% set failure = CTX.backup_failure %}
{% set failure_type = failure.error_code | d("unknown") %}

{% set remediation_actions = {
    "VSS_FAILURE": {
        "action": "restart_vss_service",
        "script": "Restart-Service VSS -Force",
        "retry_backup": true
    },
    "DISK_FULL": {
        "action": "cleanup_old_backups",
        "script": "Remove old backup files to free space",
        "retry_backup": true
    },
    "AGENT_OFFLINE": {
        "action": "restart_backup_agent",
        "script": "Restart-Service DattoBackupAgent -Force",
        "retry_backup": true
    },
    "NETWORK_ERROR": {
        "action": "check_connectivity",
        "script": "Test network and retry",
        "retry_backup": true
    },
    "AUTH_FAILURE": {
        "action": "alert_and_escalate",
        "script": none,
        "retry_backup": false
    }
} %}

{% set action = remediation_actions[failure_type] | d({"action": "escalate", "retry_backup": false}) %}

{# Log remediation attempt #}
{% set remediation_log = {
    "timestamp": now() | format_datetime("%Y-%m-%dT%H:%M:%SZ"),
    "failure_type": failure_type,
    "device": failure.device_name,
    "client": failure.client_name,
    "action_taken": action.action,
    "will_retry": action.retry_backup
} %}
```

### Storage Capacity Alerts

```jinja
{# Monitor and alert on storage capacity #}
{% set repositories = CTX.backup_repositories %}
{% set alert_threshold = CTX.alert_threshold_percent | d(85) %}
{% set critical_threshold = CTX.critical_threshold_percent | d(95) %}

{% set alerts = [] %}
{% for repo in repositories %}
    {% set used_percent = repo.used_percent | d(0) %}

    {% if used_percent >= critical_threshold %}
        {% set _ = alerts.append({
            "severity": "critical",
            "repository": repo.name,
            "used_percent": used_percent,
            "free_gb": repo.free_gb,
            "message": "Repository critically full - immediate action required"
        }) %}
    {% elif used_percent >= alert_threshold %}
        {% set _ = alerts.append({
            "severity": "warning",
            "repository": repo.name,
            "used_percent": used_percent,
            "free_gb": repo.free_gb,
            "message": "Repository approaching capacity - plan expansion"
        }) %}
    {% endif %}
{% endfor %}

{# Generate storage forecast #}
{% set daily_growth_gb = CTX.avg_daily_growth_gb | d(10) %}
{% for repo in repositories %}
    {% set days_until_full = (repo.free_gb / daily_growth_gb) | round(0) if daily_growth_gb > 0 else 999 %}
    {% set forecast_date = now() | datedelta(days=days_until_full) | format_datetime("%Y-%m-%d") %}
    {% set repo = repo | combine({
        "days_until_full": days_until_full,
        "forecast_full_date": forecast_date
    }) %}
{% endfor %}
```

---

## Best Practices Checklist

```markdown
## Backup & DR Best Practices

### Monitoring
□ Check backup status daily (automated)
□ Verify screenshot/boot verification success
□ Monitor repository capacity trends
□ Track SLA compliance metrics
□ Alert on consecutive failures

### Testing
□ Perform monthly DR tests
□ Document recovery procedures
□ Validate RTO/RPO compliance
□ Test file-level restores regularly
□ Verify offsite/cloud copies

### Reporting
□ Daily backup summary to NOC
□ Weekly client backup reports
□ Monthly SLA compliance reports
□ Quarterly DR test documentation
□ Annual backup strategy review

### Remediation
□ Auto-remediate common failures
□ Escalate persistent failures
□ Track remediation success rates
□ Document manual interventions
□ Review and improve automation

### Documentation
□ Maintain DR runbooks
□ Document recovery procedures
□ Update contact lists
□ Record RTO/RPO requirements
□ Keep network diagrams current
```

