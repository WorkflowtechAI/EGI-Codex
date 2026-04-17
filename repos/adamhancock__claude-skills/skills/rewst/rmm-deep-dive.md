# Rewst RMM Integration Deep Dive

Complete reference for RMM integrations: Datto RMM, NinjaRMM, ConnectWise Automate, N-able.

---

## Datto RMM

### Device Queries

```jinja
{# Get device by hostname #}
GET /api/v2/device?hostname={{ CTX.hostname }}

{# Get devices by site #}
GET /api/v2/site/{{ CTX.site_uid }}/devices

{# Response structure #}
{{ TASKS.get_devices.result.result.data.devices }}
```

### Script Execution

```jinja
{# Job creation payload #}
{
    "jobName": "{{ CTX.job_name | d('Rewst Automation') }}",
    "jobPriority": "{{ CTX.priority | d('Normal') }}",
    "jobType": "quickJob",
    "devices": [
        {% for device in CTX.device_ids %}
        {"uid": "{{ device }}"}{{ "," if not loop.last }}
        {% endfor %}
    ],
    "scriptComponents": [
        {
            "componentUid": "{{ ORG.VARIABLES.datto_script_uid }}",
            "scriptParameters": {
                {% for key, value in CTX.script_params.items() %}
                "{{ key }}": "{{ value }}"{{ "," if not loop.last }}
                {% endfor %}
            }
        }
    ]
}
```

### Job Status Checking

```jinja
{# Check job status - poll pattern #}
{% set job_status = TASKS.get_job.result.result.data.status %}

{% if job_status == "completed" %}
    {# All devices finished #}
{% elif job_status == "running" %}
    {# Still in progress #}
{% elif job_status == "failed" %}
    {# Job failed #}
{% endif %}

{# Per-device results #}
{% for result in TASKS.get_job.result.result.data.deviceResults %}
    Device: {{ result.device.hostname }}
    Status: {{ result.status }}
    Output: {{ result.stdout }}
    Error: {{ result.stderr }}
{% endfor %}
```

### Common Datto RMM Scripts

```powershell
# Get installed software
$software = Get-ItemProperty HKLM:\Software\Microsoft\Windows\CurrentVersion\Uninstall\* |
    Select-Object DisplayName, DisplayVersion, Publisher |
    Where-Object { $_.DisplayName } |
    ConvertTo-Json -Compress

# Return to Rewst
@{
    "status" = "success"
    "software" = $software
} | ConvertTo-Json

# Get local admin users
$admins = Get-LocalGroupMember -Group "Administrators" |
    Select-Object Name, ObjectClass, PrincipalSource |
    ConvertTo-Json -Compress

@{
    "status" = "success"
    "administrators" = $admins
} | ConvertTo-Json

# Check disk space
$disks = Get-WmiObject Win32_LogicalDisk -Filter "DriveType=3" |
    Select-Object DeviceID,
        @{N='SizeGB';E={[math]::Round($_.Size/1GB,2)}},
        @{N='FreeGB';E={[math]::Round($_.FreeSpace/1GB,2)}},
        @{N='PercentFree';E={[math]::Round(($_.FreeSpace/$_.Size)*100,2)}} |
    ConvertTo-Json -Compress

@{
    "status" = "success"
    "disks" = $disks
} | ConvertTo-Json
```

### Agent Smith Integration

```jinja
{# Agent Smith uses Azure IoT Hub for communication #}
{# Requires: Azure integration + Agent Smith crates #}

{# Send command to Agent Smith #}
{
    "deviceId": "{{ CTX.device_id }}",
    "command": "{{ CTX.command }}",
    "parameters": {{ CTX.params | to_json_string }},
    "callbackUrl": "{{ CTX.webhook_url }}"
}

{# Handle Agent Smith callback #}
{# In webhook trigger workflow #}
{% set result = CTX.body %}
{% if result.status == "success" %}
    {{ result.output }}
{% else %}
    Error: {{ result.error }}
{% endif %}
```

---

## NinjaRMM (NinjaOne)

### Device Queries

```jinja
{# Get all devices #}
GET /v2/devices

{# Get device by ID #}
GET /v2/device/{{ CTX.device_id }}

{# Get devices by organization #}
GET /v2/organization/{{ CTX.org_id }}/devices

{# Search devices #}
GET /v2/devices?df=class eq WINDOWS_WORKSTATION
```

### NinjaRMM Filter Syntax

```jinja
{# Common device filters #}
?df=class eq WINDOWS_SERVER
?df=class eq WINDOWS_WORKSTATION
?df=class eq MAC
?df=status eq ONLINE
?df=status eq OFFLINE

{# Combined filters #}
?df=class eq WINDOWS_SERVER and status eq ONLINE

{# Organization filter #}
?of={{ CTX.organization_id }}

{# Pagination #}
?pageSize=100&after={{ CTX.cursor }}
```

### Script Execution

```jinja
{# Create scripted job #}
POST /v2/organization/{{ CTX.org_id }}/devices/script

{
    "deviceIds": {{ CTX.device_ids | to_json_string }},
    "scriptId": {{ ORG.VARIABLES.ninja_script_id }},
    "parameters": {
        "param1": "{{ CTX.param1 }}",
        "param2": "{{ CTX.param2 }}"
    },
    "runAs": "SYSTEM"
}

{# Response contains job UID #}
{% set job_uid = TASKS.run_script.result.result.data.uid %}
```

### Custom Fields

```jinja
{# Get custom field values #}
GET /v2/device/{{ CTX.device_id }}/custom-fields

{# Update custom field #}
PATCH /v2/device/{{ CTX.device_id }}/custom-fields

{
    "fields": {
        "{{ ORG.VARIABLES.ninja_field_name }}": "{{ CTX.field_value }}"
    }
}
```

### Activities and Alerts

```jinja
{# Get device activities #}
GET /v2/device/{{ CTX.device_id }}/activities
    ?activityType=CONDITION
    &status=TRIGGERED
    &pageSize=50

{# Get alerts for organization #}
GET /v2/organization/{{ CTX.org_id }}/alerts

{# Acknowledge/resolve alert #}
POST /v2/alert/{{ CTX.alert_uid }}/acknowledge
POST /v2/alert/{{ CTX.alert_uid }}/reset
```

---

## ConnectWise Automate

### Computer Queries

```jinja
{# Get computers by client #}
GET /v1/Clients/{{ CTX.client_id }}/Computers

{# Get computer details #}
GET /v1/Computers/{{ CTX.computer_id }}

{# Search computers #}
GET /v1/Computers?condition=ComputerName like '%{{ CTX.search }}%'
```

### Script Execution

```jinja
{# Run script on computer #}
POST /v1/Computers/{{ CTX.computer_id }}/RunScript

{
    "ScriptId": {{ ORG.VARIABLES.automate_script_id }},
    "ScriptParams": "{{ CTX.params }}",
    "Priority": {{ CTX.priority | d(5) }},
    "Timeout": {{ CTX.timeout | d(300) }}
}

{# Check script results #}
{# Script results stored in Automate - may need to poll or use DataViews #}
```

### Extra Data Fields (EDFs)

```jinja
{# Get computer EDFs #}
GET /v1/Computers/{{ CTX.computer_id }}/ExtraDataFields

{# Update EDF #}
PUT /v1/Computers/{{ CTX.computer_id }}/ExtraDataFields

{
    "Id": {{ CTX.edf_id }},
    "Value": "{{ CTX.edf_value }}"
}
```

### Automate Conditions

```jinja
{# Query parameters use conditions #}
?condition=Status = 'Online'
?condition=ClientId = {{ CTX.client_id }} AND OS like '%Server%'
?condition=LastContact > '{{ now() | datedelta(hours=-24) | format_datetime('%Y-%m-%dT%H:%M:%S') }}'

{# Combining conditions #}
?condition=(ClientId = 123 OR ClientId = 456) AND Status = 'Online'
```

---

## N-able (N-central / RMM)

### Device Queries

```jinja
{# N-central API - Get devices #}
POST /dms2/services2/ServerEI2

{# SOAP-based API - use HTTP Request action #}
{% set soap_body %}
<soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">
    <soap:Body>
        <deviceList xmlns="http://ei2.nobj.nable.com/">
            <username>{{ ORG.VARIABLES.ncentral_username }}</username>
            <password>{{ ORG.VARIABLES.ncentral_password }}</password>
            <customerId>{{ CTX.customer_id }}</customerId>
        </deviceList>
    </soap:Body>
</soap:Envelope>
{% endset %}
```

### N-able RMM (formerly SolarWinds)

```jinja
{# REST API pattern #}
GET /api/v2/devices
Authorization: Bearer {{ ORG.VARIABLES.nable_api_key }}

{# Response pagination #}
{% set devices = TASKS.list_devices.result.result.data.data %}
{% set next_cursor = TASKS.list_devices.result.result.data.meta.nextCursor %}
```

---

## Cross-RMM Abstraction Pattern

### Unified Device Interface

```jinja
{# Normalize device data across RMMs #}
{% set rmm_type = ORG.VARIABLES.default_rmm %}

{% if rmm_type == "datto_rmm" %}
    {% set device = {
        "id": CTX.device.uid,
        "hostname": CTX.device.hostname,
        "os": CTX.device.operatingSystem,
        "status": "online" if CTX.device.online else "offline",
        "last_seen": CTX.device.lastSeenAt,
        "site_id": CTX.device.siteUid
    } %}
{% elif rmm_type == "ninja_rmm" %}
    {% set device = {
        "id": CTX.device.id,
        "hostname": CTX.device.systemName,
        "os": CTX.device.os.name,
        "status": CTX.device.nodeApprovalMode | lower,
        "last_seen": CTX.device.lastContact,
        "site_id": CTX.device.organizationId
    } %}
{% elif rmm_type == "cw_automate" %}
    {% set device = {
        "id": CTX.device.Id,
        "hostname": CTX.device.ComputerName,
        "os": CTX.device.OperatingSystemName,
        "status": CTX.device.Status | lower,
        "last_seen": CTX.device.LastContact,
        "site_id": CTX.device.ClientId
    } %}
{% endif %}
```

### Script Execution Abstraction

```jinja
{# Unified script execution #}
{% set rmm_type = ORG.VARIABLES.default_rmm %}

{% if rmm_type == "datto_rmm" %}
    {# Use Datto RMM action #}
{% elif rmm_type == "ninja_rmm" %}
    {# Use NinjaRMM action #}
{% elif rmm_type == "cw_automate" %}
    {# Use Automate action #}
{% endif %}

{# Store script mapping in org variables #}
{
    "install_software": {
        "datto_rmm": "component-uid-123",
        "ninja_rmm": "456",
        "cw_automate": "789"
    }
}
```

---

## PowerShell Templates for RMM

### Safe Script Wrapper

```powershell
# Standard wrapper for Rewst RMM scripts
param(
    [string]$InputJson = '{}'
)

# Parse input
try {
    $params = $InputJson | ConvertFrom-Json
} catch {
    $params = @{}
}

# Result container
$result = @{
    "status" = "success"
    "timestamp" = (Get-Date -Format "yyyy-MM-ddTHH:mm:ssZ")
    "data" = @{}
    "errors" = @()
}

try {
    # === YOUR LOGIC HERE ===

    $result.data = @{
        "output" = "your data"
    }
}
catch {
    $result.status = "error"
    $result.errors += @{
        "message" = $_.Exception.Message
        "line" = $_.InvocationInfo.ScriptLineNumber
    }
}

# Output JSON for Rewst
$result | ConvertTo-Json -Depth 10 -Compress
```

### Common RMM Tasks

```powershell
# Install software silently
param([string]$InstallerUrl, [string]$Arguments = "/S")

$installer = "$env:TEMP\installer.exe"
Invoke-WebRequest -Uri $InstallerUrl -OutFile $installer
$process = Start-Process -FilePath $installer -ArgumentList $Arguments -Wait -PassThru

@{
    "status" = if($process.ExitCode -eq 0) {"success"} else {"error"}
    "exitCode" = $process.ExitCode
} | ConvertTo-Json

# Check Windows Update status
$session = New-Object -ComObject Microsoft.Update.Session
$searcher = $session.CreateUpdateSearcher()
$pending = $searcher.Search("IsInstalled=0").Updates

@{
    "status" = "success"
    "pendingUpdates" = $pending.Count
    "updates" = ($pending | ForEach-Object {
        @{
            "title" = $_.Title
            "severity" = $_.MsrcSeverity
            "kb" = $_.KBArticleIDs -join ","
        }
    })
} | ConvertTo-Json -Depth 3

# Get BitLocker status
$volumes = Get-BitLockerVolume | Select-Object MountPoint, VolumeStatus, EncryptionPercentage, ProtectionStatus

@{
    "status" = "success"
    "bitlocker" = $volumes
} | ConvertTo-Json -Depth 2

# Clear browser cache (all users)
$users = Get-ChildItem C:\Users -Directory
foreach ($user in $users) {
    $paths = @(
        "$($user.FullName)\AppData\Local\Google\Chrome\User Data\Default\Cache",
        "$($user.FullName)\AppData\Local\Microsoft\Edge\User Data\Default\Cache"
    )
    foreach ($path in $paths) {
        if (Test-Path $path) {
            Remove-Item -Path "$path\*" -Recurse -Force -ErrorAction SilentlyContinue
        }
    }
}

@{"status" = "success"; "message" = "Cache cleared"} | ConvertTo-Json
```

---

## RMM Webhook Patterns

### Alert-Triggered Automation

```jinja
{# Webhook receives RMM alert #}
{% set alert = CTX.body %}

{# Datto RMM alert structure #}
{% set device_id = alert.device.uid %}
{% set alert_type = alert.alertType %}
{% set severity = alert.priority %}

{# NinjaRMM alert structure #}
{% set device_id = alert.deviceId %}
{% set alert_type = alert.type %}
{% set severity = alert.severity %}

{# Route based on alert type #}
{% if "disk" in alert_type | lower %}
    {# Trigger disk cleanup workflow #}
{% elif "cpu" in alert_type | lower or "memory" in alert_type | lower %}
    {# Trigger performance investigation #}
{% elif "offline" in alert_type | lower %}
    {# Trigger connectivity check #}
{% endif %}
```

### Automatic Remediation Pattern

```
Trigger: RMM Alert Webhook

1. Parse Alert
   └── Extract device, type, severity

2. Check Remediation Rules
   ├── Disk Full → Run cleanup script
   ├── High CPU → Restart service
   ├── Offline → Wait + recheck
   └── Other → Create ticket

3. Execute Remediation
   └── Run appropriate RMM script

4. Verify Fix
   ├── Success → Auto-close alert + log
   └── Failed → Escalate to ticket

5. Document
   └── Update PSA ticket / documentation
```

---

## RMM Performance Considerations

### Batch Operations

```jinja
{# Don't loop through 1000 devices individually #}
{# BAD #}
{% for device in CTX.all_devices %}
    {# Individual API call per device #}
{% endfor %}

{# GOOD - Batch into groups #}
{% set batch_size = 50 %}
{% for batch in CTX.all_devices | batch(batch_size) %}
    {# Process 50 devices at once #}
{% endfor %}
```

### Script Result Caching

```jinja
{# Cache frequently needed device info #}
{# Store in org variable with timestamp #}
{
    "device_inventory": {
        "last_updated": "{{ now() | format_datetime('%Y-%m-%dT%H:%M:%SZ') }}",
        "devices": {{ CTX.devices | to_json_string }}
    }
}

{# Check cache before fresh query #}
{% set cache = ORG.VARIABLES.device_inventory | d({}) %}
{% set cache_age = (now() - (cache.last_updated | as_datetime)).total_seconds() %}
{% if cache_age > 3600 %}  {# Older than 1 hour #}
    {# Refresh cache #}
{% else %}
    {# Use cached data #}
    {% set devices = cache.devices %}
{% endif %}
```

### Rate Limiting by RMM

| RMM | Rate Limit | Recommendation |
|-----|------------|----------------|
| Datto RMM | ~100/min | Concurrency 5 |
| NinjaRMM | ~60/min | Concurrency 3 |
| CW Automate | Varies | Concurrency 5 |
| N-able | ~30/min | Concurrency 2 |

```jinja
{# With Items concurrency based on RMM #}
{% set concurrency_map = {
    "datto_rmm": 5,
    "ninja_rmm": 3,
    "cw_automate": 5,
    "nable": 2
} %}

{# Set With Items concurrency #}
{{ concurrency_map[ORG.VARIABLES.default_rmm] | d(3) }}
```
