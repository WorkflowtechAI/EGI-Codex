# Rewst Billing & Usage Automation

Patterns for license usage tracking, billing reconciliation, cost optimization, and automated invoicing workflows.

---

## Microsoft 365 License Management

### License Inventory

```jinja
{# Get all subscribed SKUs #}
{# GET https://graph.microsoft.com/v1.0/subscribedSkus #}

{% set skus = TASKS.get_skus.result.result.data.value | d([]) %}

{% set license_inventory = [] %}
{% for sku in skus %}
    {% set total = sku.prepaidUnits.enabled | d(0) %}
    {% set consumed = sku.consumedUnits | d(0) %}
    {% set available = total - consumed %}
    {% set utilization = ((consumed / total) * 100) | round(1) if total > 0 else 0 %}

    {% set license = {
        "sku_id": sku.skuId,
        "sku_part_number": sku.skuPartNumber,
        "display_name": sku.servicePlans[0].servicePlanName | d(sku.skuPartNumber) if sku.servicePlans else sku.skuPartNumber,
        "total": total,
        "consumed": consumed,
        "available": available,
        "utilization_percent": utilization,
        "suspended": sku.prepaidUnits.suspended | d(0),
        "warning": sku.prepaidUnits.warning | d(0),
        "applies_to": sku.appliesTo
    } %}
    {% set _ = license_inventory.append(license) %}
{% endfor %}

{# Identify licenses needing attention #}
{% set over_utilized = license_inventory | selectattr("available", "lt", 5) | list %}
{% set under_utilized = license_inventory | selectattr("utilization_percent", "lt", 50) | list %}
```

### User License Assignment Audit

```jinja
{# Get license assignments for all users #}
{# GET https://graph.microsoft.com/v1.0/users?$select=id,displayName,userPrincipalName,assignedLicenses,accountEnabled #}

{% set users = CTX.all_users %}

{# Build license assignment report #}
{% set assignment_report = [] %}
{% for user in users %}
    {% set user_licenses = [] %}
    {% for license in user.assignedLicenses | d([]) %}
        {% set sku_info = CTX.sku_lookup[license.skuId] | d({"name": license.skuId}) %}
        {% set _ = user_licenses.append({
            "sku_id": license.skuId,
            "sku_name": sku_info.name,
            "disabled_plans": license.disabledPlans | d([]) | length
        }) %}
    {% endfor %}

    {% set _ = assignment_report.append({
        "user_id": user.id,
        "upn": user.userPrincipalName,
        "display_name": user.displayName,
        "account_enabled": user.accountEnabled,
        "license_count": user_licenses | length,
        "licenses": user_licenses,
        "total_cost": user_licenses | sum(attribute="monthly_cost") | d(0)
    }) %}
{% endfor %}

{# Identify issues #}
{% set disabled_with_licenses = assignment_report | selectattr("account_enabled", "eq", false) | selectattr("license_count", "gt", 0) | list %}
{% set no_licenses = assignment_report | selectattr("account_enabled", "eq", true) | selectattr("license_count", "eq", 0) | list %}
```

### License Cost Analysis

```jinja
{# SKU pricing reference (approximate monthly per-user costs) #}
{% set sku_pricing = {
    "SPB": 12.50,           {# Microsoft 365 Business Basic #}
    "O365_BUSINESS_ESSENTIALS": 6.00,
    "O365_BUSINESS_PREMIUM": 22.00,
    "SMB_BUSINESS_PREMIUM": 22.00,
    "ENTERPRISEPACK": 36.00,       {# E3 #}
    "ENTERPRISEPREMIUM": 57.00,    {# E5 #}
    "SPE_E3": 36.00,
    "SPE_E5": 57.00,
    "EXCHANGESTANDARD": 4.00,
    "EXCHANGEENTERPRISE": 12.00,
    "POWER_BI_PRO": 10.00,
    "POWER_BI_PREMIUM_PER_USER": 20.00,
    "VISIOCLIENT": 15.00,
    "PROJECTPREMIUM": 55.00,
    "WIN10_PRO_ENT_SUB": 7.00,
    "FLOW_FREE": 0.00,
    "TEAMS_EXPLORATORY": 0.00,
    "STREAM": 0.00
} %}

{# Calculate total monthly cost #}
{% set total_monthly_cost = 0 %}
{% for sku in CTX.license_inventory %}
    {% set unit_cost = sku_pricing[sku.sku_part_number] | d(0) %}
    {% set sku_cost = unit_cost * sku.consumed %}
    {% set total_monthly_cost = total_monthly_cost + sku_cost %}
{% endfor %}

{# Calculate potential savings #}
{% set potential_savings = 0 %}
{% for user in CTX.disabled_with_licenses %}
    {% for license in user.licenses %}
        {% set unit_cost = sku_pricing[license.sku_name] | d(0) %}
        {% set potential_savings = potential_savings + unit_cost %}
    {% endfor %}
{% endfor %}
```

---

## PSA Billing Integration

### ConnectWise Manage Agreement Sync

```jinja
{# Get agreement additions (recurring billing items) #}
{# GET /finance/agreements/{agreementId}/additions #}

{% set additions = TASKS.get_additions.result.result.data | d([]) %}

{# Sync license counts to agreement #}
{% set license_additions = [] %}
{% for addition in additions %}
    {# Find matching product from catalog #}
    {% set product_type = addition.product.identifier | d("") %}

    {# Map M365 SKUs to agreement products #}
    {% set sku_product_map = {
        "SPE_E3": "M365-E3",
        "SPE_E5": "M365-E5",
        "O365_BUSINESS_PREMIUM": "M365-BP",
        "ENTERPRISEPACK": "O365-E3",
        "ENTERPRISEPREMIUM": "O365-E5"
    } %}

    {% if product_type in sku_product_map.values() %}
        {% set _ = license_additions.append(addition) %}
    {% endif %}
{% endfor %}

{# Update addition quantities #}
{# PATCH /finance/agreements/{agreementId}/additions/{additionId} #}
{
    "quantity": {{ CTX.new_quantity }},
    "effectiveDate": "{{ CTX.effective_date | format_datetime('%Y-%m-%d') }}"
}
```

### Autotask Contract Billing

```jinja
{# Autotask - Get contract services #}
{# GET /ContractServices?search={"filter":[{"op":"eq","field":"contractID","value":123}]} #}

{% set services = TASKS.get_services.result.result.data.items | d([]) %}

{# Update service quantity #}
{# PATCH /ContractServices #}
{
    "id": {{ CTX.service_id }},
    "quantity": {{ CTX.new_quantity }}
}

{# Create billing adjustment #}
{# POST /ContractCharges #}
{
    "contractID": {{ CTX.contract_id }},
    "chargeDescription": "{{ CTX.description | escape }}",
    "units": {{ CTX.units }},
    "unitCost": {{ CTX.unit_cost }},
    "chargeDate": "{{ now() | format_datetime('%Y-%m-%dT%H:%M:%SZ') }}",
    "chargeType": {{ CTX.charge_type | d(1) }}  {# 1=Billable #}
}
```

### HaloPSA Recurring Invoice Items

```jinja
{# HaloPSA - Get recurring invoices #}
{# GET /RecurringInvoice?client_id={clientId} #}

{% set recurring = TASKS.get_recurring.result.result.data | d([]) %}

{# Update recurring invoice item quantity #}
{# PUT /RecurringInvoice #}
{
    "id": {{ CTX.invoice_id }},
    "lines": [
        {
            "id": {{ CTX.line_id }},
            "count": {{ CTX.new_quantity }},
            "unit_price": {{ CTX.unit_price }}
        }
    ]
}
```

---

## Usage-Based Billing

### Device Count Billing

```jinja
{# Count billable devices from RMM #}
{% set devices = CTX.all_devices %}
{% set billing_date = now() | format_datetime("%Y-%m-01") %}  {# First of month #}

{# Filter to billable devices #}
{% set billable_devices = [] %}
{% for device in devices %}
    {# Exclude based on criteria #}
    {% set is_excluded = false %}

    {# Exclude offline devices (>30 days) #}
    {% if device.last_seen %}
        {% set last_seen = device.last_seen | as_datetime %}
        {% set days_offline = ((now() - last_seen).total_seconds() / 86400) | int %}
        {% if days_offline > 30 %}
            {% set is_excluded = true %}
        {% endif %}
    {% endif %}

    {# Exclude by device type #}
    {% set excluded_types = ["Printer", "Network Device", "Virtual Machine - Test"] %}
    {% if device.device_type in excluded_types %}
        {% set is_excluded = true %}
    {% endif %}

    {% if not is_excluded %}
        {% set _ = billable_devices.append({
            "id": device.id,
            "name": device.name,
            "type": device.device_type,
            "client": device.client_name,
            "client_id": device.client_id
        }) %}
    {% endif %}
{% endfor %}

{# Group by client #}
{% set by_client = {} %}
{% for device in billable_devices %}
    {% set client_id = device.client_id | string %}
    {% set existing = by_client[client_id] | d({"name": device.client, "devices": []}) %}
    {% set _ = existing.devices.append(device) %}
    {% set by_client = by_client | combine({client_id: existing}) %}
{% endfor %}

{# Generate billing summary #}
{% set billing_summary = [] %}
{% for client_id, data in by_client.items() %}
    {# Count by device type #}
    {% set servers = data.devices | selectattr("type", "in", ["Server", "Domain Controller"]) | list | length %}
    {% set workstations = data.devices | selectattr("type", "eq", "Workstation") | list | length %}

    {% set _ = billing_summary.append({
        "client_id": client_id,
        "client_name": data.name,
        "total_devices": data.devices | length,
        "servers": servers,
        "workstations": workstations,
        "server_cost": servers * ORG.VARIABLES.per_server_cost | d(75),
        "workstation_cost": workstations * ORG.VARIABLES.per_workstation_cost | d(25)
    }) %}
{% endfor %}
```

### Mailbox Billing

```jinja
{# Count billable mailboxes #}
{% set mailboxes = CTX.all_mailboxes %}

{# Classify mailboxes #}
{% set user_mailboxes = mailboxes | selectattr("recipientTypeDetails", "eq", "UserMailbox") | list %}
{% set shared_mailboxes = mailboxes | selectattr("recipientTypeDetails", "eq", "SharedMailbox") | list %}
{% set room_mailboxes = mailboxes | selectattr("recipientTypeDetails", "eq", "RoomMailbox") | list %}

{# Calculate sizes #}
{% set mailbox_usage = [] %}
{% for mailbox in user_mailboxes %}
    {% set size_gb = (mailbox.totalItemSize | d(0)) / 1073741824 | round(2) %}

    {# Determine tier based on size #}
    {% if size_gb <= 10 %}
        {% set tier = "small" %}
        {% set tier_cost = 5 %}
    {% elif size_gb <= 50 %}
        {% set tier = "medium" %}
        {% set tier_cost = 10 %}
    {% else %}
        {% set tier = "large" %}
        {% set tier_cost = 20 %}
    {% endif %}

    {% set _ = mailbox_usage.append({
        "email": mailbox.primarySmtpAddress,
        "display_name": mailbox.displayName,
        "size_gb": size_gb,
        "tier": tier,
        "cost": tier_cost
    }) %}
{% endfor %}

{# Summary #}
{% set mailbox_summary = {
    "total_mailboxes": user_mailboxes | length,
    "shared_mailboxes": shared_mailboxes | length,
    "room_mailboxes": room_mailboxes | length,
    "total_storage_gb": mailbox_usage | sum(attribute="size_gb") | round(2),
    "total_monthly_cost": mailbox_usage | sum(attribute="cost")
} %}
```

### Backup Storage Billing

```jinja
{# Calculate backup storage costs #}
{% set backup_clients = CTX.backup_clients %}

{% set storage_billing = [] %}
{% for client in backup_clients %}
    {% set local_tb = (client.local_storage_bytes / 1099511627776) | round(3) %}
    {% set cloud_tb = (client.cloud_storage_bytes / 1099511627776) | round(3) %}

    {# Tiered pricing #}
    {% set local_cost_per_tb = 50 %}
    {% set cloud_cost_per_tb = 100 %}

    {# First 1TB included, charge for overage #}
    {% set local_billable_tb = [local_tb - 1, 0] | max %}
    {% set cloud_billable_tb = [cloud_tb - 1, 0] | max %}

    {% set _ = storage_billing.append({
        "client_name": client.name,
        "client_id": client.id,
        "local_storage_tb": local_tb,
        "cloud_storage_tb": cloud_tb,
        "local_cost": (local_billable_tb * local_cost_per_tb) | round(2),
        "cloud_cost": (cloud_billable_tb * cloud_cost_per_tb) | round(2),
        "total_cost": ((local_billable_tb * local_cost_per_tb) + (cloud_billable_tb * cloud_cost_per_tb)) | round(2)
    }) %}
{% endfor %}
```

---

## Billing Reconciliation

### License vs Agreement Reconciliation

```jinja
{# Compare actual license usage to billed quantities #}
{% set actual_usage = CTX.license_counts %}  {# From M365 #}
{% set billed_quantities = CTX.agreement_additions %}  {# From PSA #}

{% set discrepancies = [] %}
{% for product_code, actual_count in actual_usage.items() %}
    {# Find matching billed item #}
    {% set billed = billed_quantities | selectattr("product_code", "eq", product_code) | first | d(none) %}

    {% if billed %}
        {% set billed_count = billed.quantity %}
        {% set difference = actual_count - billed_count %}

        {% if difference != 0 %}
            {% set _ = discrepancies.append({
                "product": product_code,
                "actual": actual_count,
                "billed": billed_count,
                "difference": difference,
                "action": "increase" if difference > 0 else "decrease",
                "monthly_impact": difference * billed.unit_price | d(0)
            }) %}
        {% endif %}
    {% else %}
        {# Actual usage but no billing item #}
        {% set _ = discrepancies.append({
            "product": product_code,
            "actual": actual_count,
            "billed": 0,
            "difference": actual_count,
            "action": "add_new_item",
            "monthly_impact": actual_count * CTX.default_prices[product_code] | d(0)
        }) %}
    {% endif %}
{% endfor %}

{# Check for billed items with no usage #}
{% for billed in billed_quantities %}
    {% if billed.product_code not in actual_usage %}
        {% set _ = discrepancies.append({
            "product": billed.product_code,
            "actual": 0,
            "billed": billed.quantity,
            "difference": -billed.quantity,
            "action": "remove_or_verify",
            "monthly_impact": -billed.quantity * billed.unit_price
        }) %}
    {% endif %}
{% endfor %}

{# Summary #}
{% set total_under_billed = discrepancies | selectattr("difference", "gt", 0) | sum(attribute="monthly_impact") %}
{% set total_over_billed = discrepancies | selectattr("difference", "lt", 0) | sum(attribute="monthly_impact") | abs %}
```

### Monthly Billing Report

```jinja
{# Generate monthly billing reconciliation report #}
{% set report_month = now() | format_datetime("%B %Y") %}

{% set billing_report %}
# Monthly Billing Reconciliation Report
## {{ CTX.client_name }}
## Period: {{ report_month }}

---

### License Inventory Summary

| License Type | Actual Count | Billed Count | Variance |
|--------------|--------------|--------------|----------|
{% for item in CTX.license_comparison %}
| {{ item.name }} | {{ item.actual }} | {{ item.billed }} | {{ item.difference | abs }}{% if item.difference > 0 %} ↑{% elif item.difference < 0 %} ↓{% endif %} |
{% endfor %}

### Billing Adjustments Required

{% if CTX.discrepancies | length > 0 %}
| Product | Change | Quantity | Monthly Impact |
|---------|--------|----------|----------------|
{% for d in CTX.discrepancies %}
| {{ d.product }} | {{ d.action | title }} | {{ d.difference | abs }} | ${{ d.monthly_impact | abs | round(2) }} |
{% endfor %}

**Total Monthly Adjustment:** ${{ CTX.net_adjustment | round(2) }}
{% else %}
No adjustments required - billing is accurate.
{% endif %}

### Device Counts

| Category | Count | Rate | Amount |
|----------|-------|------|--------|
| Servers | {{ CTX.server_count }} | ${{ ORG.VARIABLES.per_server_cost }} | ${{ CTX.server_count * ORG.VARIABLES.per_server_cost }} |
| Workstations | {{ CTX.workstation_count }} | ${{ ORG.VARIABLES.per_workstation_cost }} | ${{ CTX.workstation_count * ORG.VARIABLES.per_workstation_cost }} |
| **Subtotal** | | | **${{ CTX.device_total }}** |

### User Counts

| Category | Count | Rate | Amount |
|----------|-------|------|--------|
| Licensed Users | {{ CTX.licensed_users }} | Varies | ${{ CTX.license_total | round(2) }} |
| Shared Mailboxes | {{ CTX.shared_mailboxes }} | $0.00 | $0.00 |

### Summary

| Category | Amount |
|----------|--------|
| Device Management | ${{ CTX.device_total | round(2) }} |
| Microsoft 365 Licenses | ${{ CTX.license_total | round(2) }} |
| Backup Services | ${{ CTX.backup_total | round(2) }} |
| Security Services | ${{ CTX.security_total | round(2) }} |
| **Monthly Total** | **${{ CTX.grand_total | round(2) }}** |

---
*Report generated: {{ now() | format_datetime("%Y-%m-%d %H:%M") }}*
{% endset %}
```

---

## Cost Optimization

### Identify Unused Licenses

```jinja
{# Find licenses assigned to inactive users #}
{% set inactive_threshold_days = 90 %}
{% set inactive_with_licenses = [] %}

{% for user in CTX.all_users %}
    {% if user.assignedLicenses | length > 0 %}
        {% set last_sign_in = user.signInActivity.lastSignInDateTime | d(none) %}

        {% if last_sign_in %}
            {% set sign_in_date = last_sign_in | as_datetime %}
            {% set days_inactive = ((now() - sign_in_date).total_seconds() / 86400) | int %}

            {% if days_inactive > inactive_threshold_days %}
                {% set monthly_cost = 0 %}
                {% for license in user.assignedLicenses %}
                    {% set cost = CTX.sku_pricing[license.skuId] | d(0) %}
                    {% set monthly_cost = monthly_cost + cost %}
                {% endfor %}

                {% set _ = inactive_with_licenses.append({
                    "user": user.displayName,
                    "upn": user.userPrincipalName,
                    "last_sign_in": last_sign_in,
                    "days_inactive": days_inactive,
                    "license_count": user.assignedLicenses | length,
                    "monthly_cost": monthly_cost,
                    "recommendation": "Review and consider removing licenses"
                }) %}
            {% endif %}
        {% else %}
            {# Never signed in #}
            {% set monthly_cost = 0 %}
            {% for license in user.assignedLicenses %}
                {% set cost = CTX.sku_pricing[license.skuId] | d(0) %}
                {% set monthly_cost = monthly_cost + cost %}
            {% endfor %}

            {% set _ = inactive_with_licenses.append({
                "user": user.displayName,
                "upn": user.userPrincipalName,
                "last_sign_in": "Never",
                "days_inactive": -1,
                "license_count": user.assignedLicenses | length,
                "monthly_cost": monthly_cost,
                "recommendation": "User has never signed in - verify account needed"
            }) %}
        {% endif %}
    {% endif %}
{% endfor %}

{# Calculate potential savings #}
{% set potential_monthly_savings = inactive_with_licenses | sum(attribute="monthly_cost") %}
{% set potential_annual_savings = potential_monthly_savings * 12 %}
```

### License Downgrade Recommendations

```jinja
{# Identify users who could use cheaper licenses #}
{% set downgrade_candidates = [] %}

{% for user in CTX.users_with_e5 %}  {# E5 is most expensive #}
    {# Check feature usage from usage reports #}
    {% set usage = CTX.user_usage[user.id] | d({}) %}

    {% set uses_advanced_features = false %}

    {# E5-specific features #}
    {% set e5_features = [
        "audio_conferencing",
        "power_bi_pro",
        "phone_system",
        "advanced_ediscovery",
        "information_barriers"
    ] %}

    {% for feature in e5_features %}
        {% if usage[feature] | d(false) %}
            {% set uses_advanced_features = true %}
        {% endif %}
    {% endfor %}

    {% if not uses_advanced_features %}
        {% set _ = downgrade_candidates.append({
            "user": user.displayName,
            "upn": user.userPrincipalName,
            "current_license": "E5",
            "recommended_license": "E3",
            "monthly_savings": 57 - 36,  {# E5 - E3 cost #}
            "reason": "Not using E5-specific features"
        }) %}
    {% endif %}
{% endfor %}

{# Summary #}
{% set total_downgrade_savings = downgrade_candidates | sum(attribute="monthly_savings") %}
```

---

## Automated Billing Workflows

### Monthly License Sync Workflow

```
Trigger: Cron (1st of month, 2 AM)

1. Get Current License Counts
   ├── Fetch M365 subscribedSkus
   └── Count active licenses per SKU

2. For Each Client
   ├── Get PSA agreement/contract
   ├── Get current billed quantities
   └── Compare actual vs billed

3. Process Discrepancies
   ├── Auto-update minor changes (<= 5%)
   ├── Flag major changes for review
   └── Log all changes

4. Generate Report
   ├── Create reconciliation report
   ├── Email to billing team
   └── Update documentation

5. Create Tickets (if needed)
   └── For changes requiring manual review
```

### Usage Alert Workflow

```jinja
{# Alert when usage thresholds are exceeded #}
{% set alerts = [] %}

{# Check license utilization #}
{% for sku in CTX.license_inventory %}
    {% if sku.utilization_percent >= 90 %}
        {% set _ = alerts.append({
            "type": "license_utilization",
            "severity": "high" if sku.utilization_percent >= 95 else "medium",
            "message": sku.sku_part_number ~ " is at " ~ sku.utilization_percent ~ "% utilization (" ~ sku.available ~ " remaining)",
            "action": "Consider purchasing additional licenses"
        }) %}
    {% endif %}
{% endfor %}

{# Check storage usage #}
{% for repo in CTX.backup_repos %}
    {% if repo.used_percent >= 85 %}
        {% set _ = alerts.append({
            "type": "storage_utilization",
            "severity": "critical" if repo.used_percent >= 95 else "high",
            "message": repo.name ~ " backup storage at " ~ repo.used_percent ~ "%",
            "action": "Expand storage or archive old backups"
        }) %}
    {% endif %}
{% endfor %}

{# Check for billing discrepancies over threshold #}
{% set discrepancy_threshold = 500 %}  {# Monthly $ #}
{% if CTX.net_billing_discrepancy | abs > discrepancy_threshold %}
    {% set _ = alerts.append({
        "type": "billing_discrepancy",
        "severity": "high",
        "message": "Billing discrepancy of $" ~ CTX.net_billing_discrepancy | abs | round(2) ~ " detected",
        "action": "Review and reconcile billing"
    }) %}
{% endif %}
```

---

## Best Practices

```markdown
## Billing & Usage Best Practices

### License Management
□ Sync license counts monthly (at minimum)
□ Remove licenses from disabled accounts immediately
□ Review inactive user licenses quarterly
□ Track license assignments in documentation tool
□ Set alerts for high utilization (>85%)

### Cost Optimization
□ Review for downgrade candidates quarterly
□ Audit shared mailboxes (often free)
□ Consolidate redundant licenses
□ Use security groups for license assignment
□ Document license requirements by role

### Billing Reconciliation
□ Compare actual vs billed monthly
□ Auto-update small variances
□ Flag large changes for review
□ Maintain audit trail of changes
□ Generate client-facing reports

### Automation
□ Automate license count sync
□ Generate monthly billing reports
□ Alert on threshold breaches
□ Create tickets for discrepancies
□ Update PSA automatically when possible

### Compliance
□ Maintain license assignment history
□ Document justification for premium licenses
□ Track true-up requirements (EA agreements)
□ Audit third-party app licensing
□ Review vendor contracts annually
```

