# Rewst Security Patterns

Security automation, credential handling, and compliance patterns.

---

## Credential Security

### Never Hardcode Credentials

```jinja
{# WRONG - credentials in workflow #}
{% set api_key = "sk-abc123..." %}

{# RIGHT - use org variables #}
{{ ORG.VARIABLES.api_key }}

{# RIGHT - use integration credentials #}
{# Configured in integration settings, not visible in workflow #}
```

### Sensitive Data in Logs

```jinja
{# WRONG - logs password #}
Debug: User password is {{ CTX.password }}

{# RIGHT - mask sensitive values #}
Debug: Password set for user {{ CTX.username }}

{# RIGHT - use masked indicator #}
Debug: API Key: {{ CTX.api_key[:4] }}****{{ CTX.api_key[-4:] if CTX.api_key | length > 8 else "****" }}
```

### Secure Webhook Patterns

```jinja
{# Validate webhook signature (HMAC) #}
{% set expected = CTX.request_body | hmac("sha256", ORG.VARIABLES.webhook_secret) %}
{% if CTX.headers.get("X-Signature") != expected %}
    {# Reject - invalid signature #}
{% endif %}

{# Require secret key header #}
{% if CTX.headers.get("X-API-Key") != ORG.VARIABLES.webhook_api_key %}
    {# Reject - unauthorized #}
{% endif %}
```

---

## Identity Security Automations

### Compromised User Response

```
Trigger: Security Alert Webhook

1. Parse Alert
   └── Extract user, threat indicators

2. Immediate Actions [Parallel]
   ├── Disable Sign-in
   ├── Revoke All Sessions
   │   └── /users/{id}/revokeSignInSessions
   ├── Reset Password (random)
   └── Block from Conditional Access

3. Evidence Collection
   ├── Sign-in Logs (7 days)
   ├── Audit Logs
   ├── Mailbox Rules
   └── Inbox Forwarding

4. Containment
   ├── Remove MFA Methods
   ├── Disconnect OAuth Apps
   └── Enable Litigation Hold

5. Documentation
   ├── Create Security Ticket
   ├── Document Timeline
   └── Attach Evidence

6. Notification
   ├── Security Team
   ├── User's Manager
   └── Compliance (if PII involved)
```

### Risky Sign-in Response

```jinja
{# Triggered by M365 risky sign-in alert #}

{% set risk_level = CTX.alert.riskLevel %}
{% set user_id = CTX.alert.userId %}

{% if risk_level == "high" %}
    {# Force password reset and MFA re-registration #}
    {# Block until verified #}
{% elif risk_level == "medium" %}
    {# Require MFA for this sign-in #}
    {# Send notification to user #}
{% else %}
    {# Log and monitor #}
{% endif %}
```

### MFA Reset Workflow

```
Trigger: Form + Verification

1. Verify Requester Identity
   ├── Manager Approval
   └── OR Secret Question
   └── OR Callback Verification

2. Remove Existing MFA Methods
   └── /users/{id}/authentication/methods

3. Generate Temporary Access Pass
   └── POST /users/{id}/authentication/temporaryAccessPassMethods

4. Send Secure Delivery
   └── Separate channel (not email)

5. Audit Trail
   ├── Log all actions
   └── Create ticket
```

---

## Conditional Access Automation

### CA Policy Audit

```jinja
{# List all CA policies and their settings #}
GET /identity/conditionalAccess/policies

{# Check for risky configurations #}
{% set risky_policies = [] %}
{% for policy in CTX.policies %}
    {% if policy.conditions.users.includeUsers == ["All"] and
          policy.grantControls.builtInControls | length == 0 %}
        {% set _ = risky_policies.append(policy) %}
    {% endif %}
{% endfor %}
```

### Named Location Management

```jinja
{# Add IP to trusted locations #}
{
    "displayName": "{{ CTX.location_name }}",
    "@odata.type": "#microsoft.graph.ipNamedLocation",
    "isTrusted": true,
    "ipRanges": [
        {
            "@odata.type": "#microsoft.graph.iPv4CidrRange",
            "cidrAddress": "{{ CTX.ip_range }}"
        }
    ]
}
```

---

## Privileged Access Management

### Admin Role Assignment Audit

```jinja
{# Get all users with admin roles #}
{% set admin_roles = [
    "62e90394-69f5-4237-9190-012177145e10",  {# Global Admin #}
    "fe930be7-5e62-47db-91af-98c3a49a38b1",  {# User Admin #}
    "29232cdf-9323-42fd-ade2-1d097af3e4de",  {# Exchange Admin #}
    "f28a1f50-f6e7-4571-818b-6a12f2af6b6c"   {# SharePoint Admin #}
] %}

{# Query each role's members #}
{# GET /directoryRoles/{id}/members #}
```

### JIT Access Pattern

```
Trigger: Approval Form

1. Verify Request
   └── Check requester, business justification

2. Multi-Level Approval
   ├── Manager Approval
   └── Security Team Approval

3. Grant Temporary Access
   ├── Add to PIM-eligible role
   ├── Set expiration (4-8 hours)
   └── Enable audit logging

4. Monitor Session
   └── Alert on suspicious activity

5. Auto-Revoke at Expiration

6. Document Access
   └── Full audit trail
```

---

## Data Protection

### Sensitivity Label Automation

```jinja
{# Check if document has required label #}
{% set required_labels = ["Confidential", "Highly Confidential"] %}
{% if CTX.document.sensitivityLabel not in required_labels %}
    {# Flag for review #}
{% endif %}
```

### DLP Policy Alert Response

```
Trigger: DLP Alert Webhook

1. Parse Alert Details
   └── User, content type, rule matched

2. Assess Severity
   ├── High (PII/PHI) → Immediate response
   └── Medium/Low → Queue for review

3. Containment [If High]
   ├── Quarantine content
   ├── Block external sharing
   └── Revoke links

4. Investigation
   ├── Get file details
   ├── Check sharing history
   └── Identify data subjects

5. Notification
   ├── Data owner
   ├── Compliance team
   └── User (educational)

6. Remediation Tracking
```

---

## Compliance Automation

### License Compliance Check

```jinja
{# Compare assigned vs purchased licenses #}
{% set purchased = CTX.subscribed_skus %}
{% set assigned = CTX.license_assignments %}

{% for sku in purchased %}
    {% set assigned_count = assigned | selectattr("skuId", "eq", sku.skuId) | list | length %}
    {% set available = sku.prepaidUnits.enabled %}

    {% if assigned_count > available %}
        {# Over-allocated: {{ assigned_count - available }} #}
    {% elif assigned_count < available * 0.8 %}
        {# Under-utilized: {{ available - assigned_count }} unused #}
    {% endif %}
{% endfor %}
```

### Inactive Account Audit

```jinja
{# Find accounts with no sign-in in 90 days #}
{% set cutoff = now() | datedelta(days=-90) %}
{% set inactive = [] %}

{% for user in CTX.users %}
    {% set last_signin = user.signInActivity.lastSignInDateTime | d("") %}
    {% if not last_signin or last_signin | as_datetime < cutoff %}
        {% set _ = inactive.append(user) %}
    {% endif %}
{% endfor %}
```

### External User Review

```jinja
{# List all guest users and their access #}
{% set guests = CTX.users | selectattr("userType", "eq", "Guest") | list %}

{# Get each guest's group memberships #}
{# Flag guests with access to sensitive groups #}
{% set sensitive_groups = ORG.VARIABLES.sensitive_group_ids | d([]) %}

{% for guest in guests %}
    {% set guest_groups = guest.memberOf | map(attribute="id") | list %}
    {% if guest_groups | select("in", sensitive_groups) | list %}
        {# Flag for review #}
    {% endif %}
{% endfor %}
```

---

## Secure Password Handling

### Password Generation

```jinja
{# Use Rewst's Generate Password action #}
{# Parameters: #}
{# - length: 16-24 characters #}
{# - include_uppercase: true #}
{# - include_lowercase: true #}
{# - include_numbers: true #}
{# - include_special: true #}
{# - exclude_ambiguous: true (removes 0, O, l, 1, etc.) #}
```

### Secure Password Delivery

```
Options (in order of preference):
1. Self-service password set (user creates own)
2. Temporary Access Pass (M365)
3. Secure portal link (time-limited)
4. SMS to verified number
5. Manager delivery (last resort)

NEVER:
- Email password directly
- Include in ticket
- Leave in plain text logs
```

---

## Security Monitoring

### Failed Sign-in Monitoring

```jinja
{# Alert on multiple failed sign-ins #}
{% set threshold = 5 %}
{% set timeframe_minutes = 15 %}

{% set failed_attempts = CTX.signin_logs | selectattr("status.errorCode", "ne", 0) | list %}

{% if failed_attempts | length >= threshold %}
    {# Trigger alert #}
{% endif %}
```

### Mailbox Rule Monitoring

```jinja
{# Detect suspicious inbox rules #}
{% set suspicious_keywords = ["forward", "delete", "move"] %}
{% set external_domains = ["gmail.com", "yahoo.com", "outlook.com"] %}

{% for rule in CTX.inbox_rules %}
    {% if rule.forwardTo %}
        {% for recipient in rule.forwardTo %}
            {% if recipient.emailAddress.address.split("@")[1] in external_domains %}
                {# Flag: External forwarding rule #}
            {% endif %}
        {% endfor %}
    {% endif %}

    {% if rule.deleteMessage and rule.enabled %}
        {# Flag: Auto-delete rule #}
    {% endif %}
{% endfor %}
```

### OAuth App Consent Monitoring

```jinja
{# Review recently consented apps #}
{% set high_risk_permissions = [
    "Mail.ReadWrite",
    "Mail.Send",
    "Files.ReadWrite.All",
    "Directory.ReadWrite.All"
] %}

{% for app in CTX.oauth_apps %}
    {% set app_permissions = app.oauth2PermissionGrants | map(attribute="scope") | join(" ").split(" ") %}
    {% if app_permissions | select("in", high_risk_permissions) | list %}
        {# Flag for review #}
    {% endif %}
{% endfor %}
```

---

## Audit & Reporting

### Security Audit Trail

```jinja
{# Standard audit log format #}
{
    "timestamp": "{{ now() | format_datetime('%Y-%m-%dT%H:%M:%SZ') }}",
    "action": "{{ CTX.action }}",
    "actor": {
        "type": "automation",
        "name": "Rewst",
        "workflow": "{{ CTX.__workflow_name__ | d('Unknown') }}"
    },
    "target": {
        "type": "{{ CTX.target_type }}",
        "id": "{{ CTX.target_id }}",
        "name": "{{ CTX.target_name }}"
    },
    "result": "{{ CTX.result }}",
    "details": {{ CTX.details | to_json_string }},
    "organization": "{{ ORG.ATTRIBUTES.id }}"
}
```

### Compliance Report Template

```markdown
# Security Compliance Report
Period: {{ CTX.start_date }} - {{ CTX.end_date }}
Organization: {{ ORG.ATTRIBUTES.name }}

## Executive Summary
- Total Security Events: {{ CTX.events | length }}
- Critical Incidents: {{ CTX.events | selectattr("severity", "eq", "critical") | list | length }}
- Policy Violations: {{ CTX.violations | length }}
- Remediation Rate: {{ CTX.remediation_rate }}%

## Identity Security
- MFA Coverage: {{ CTX.mfa_percentage }}%
- Privileged Accounts: {{ CTX.admin_count }}
- Stale Accounts: {{ CTX.stale_accounts | length }}
- Guest Users: {{ CTX.guest_count }}

## Access Controls
- Conditional Access Policies: {{ CTX.ca_policies | length }}
- High-Risk Sign-ins Blocked: {{ CTX.blocked_signins }}
- Password Resets: {{ CTX.password_resets }}

## Data Protection
- DLP Alerts: {{ CTX.dlp_alerts | length }}
- External Sharing Events: {{ CTX.external_shares | length }}
- Sensitivity Labels Applied: {{ CTX.labeled_docs }}

## Recommendations
{% for rec in CTX.recommendations %}
- {{ rec.priority }}: {{ rec.description }}
{% endfor %}
```
