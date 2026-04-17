# Compliance & Audit Automation

Comprehensive patterns for compliance monitoring, audit automation, regulatory frameworks, and evidence collection workflows.

---

## Framework Overview

| Framework | Focus | Key Requirements |
|-----------|-------|-----------------|
| SOC 2 | Security, availability, confidentiality | Access controls, monitoring, incident response |
| HIPAA | Healthcare data | PHI protection, access logs, encryption |
| PCI-DSS | Payment card data | Network segmentation, vulnerability scanning |
| NIST CSF | Cybersecurity | Identify, Protect, Detect, Respond, Recover |
| CIS Controls | Security best practices | Prioritized security controls |
| GDPR | EU data privacy | Consent, data rights, breach notification |
| CMMC | Defense contractors | Maturity levels, practice implementation |

---

## Compliance Check Patterns

### Unified Compliance Status

```jinja
{# Generate unified compliance status across frameworks #}
{% set org = CTX.organization %}
{% set checks = CTX.compliance_checks | d([]) %}

{# Group checks by framework #}
{% set by_framework = {} %}
{% for check in checks %}
    {% set framework = check.framework %}
    {% if framework not in by_framework %}
        {% set _ = by_framework.update({framework: {"passed": 0, "failed": 0, "warning": 0, "checks": []}}) %}
    {% endif %}
    {% set _ = by_framework[framework].checks.append(check) %}
    {% if check.status == "passed" %}
        {% set _ = by_framework.update({framework: by_framework[framework] | combine({"passed": by_framework[framework].passed + 1})}) %}
    {% elif check.status == "failed" %}
        {% set _ = by_framework.update({framework: by_framework[framework] | combine({"failed": by_framework[framework].failed + 1})}) %}
    {% else %}
        {% set _ = by_framework.update({framework: by_framework[framework] | combine({"warning": by_framework[framework].warning + 1})}) %}
    {% endif %}
{% endfor %}

{# Calculate compliance scores #}
{% set framework_scores = {} %}
{% for framework, data in by_framework.items() %}
    {% set total = data.passed + data.failed + data.warning %}
    {% set score = ((data.passed + (data.warning * 0.5)) / total * 100) | round(1) if total > 0 else 0 %}
    {% set _ = framework_scores.update({
        framework: {
            "score": score,
            "status": "compliant" if score >= 95 else "needs_attention" if score >= 75 else "non_compliant",
            "passed": data.passed,
            "failed": data.failed,
            "warning": data.warning,
            "total": total
        }
    }) %}
{% endfor %}

{# Overall compliance score #}
{% set all_passed = checks | selectattr("status", "eq", "passed") | list | length %}
{% set all_failed = checks | selectattr("status", "eq", "failed") | list | length %}
{% set all_warning = checks | selectattr("status", "eq", "warning") | list | length %}
{% set total_checks = checks | length %}
{% set overall_score = ((all_passed + (all_warning * 0.5)) / total_checks * 100) | round(1) if total_checks > 0 else 0 %}

{
    "organization": "{{ org.name }}",
    "assessment_date": "{{ CTX.current_time | format_datetime('%Y-%m-%d') }}",
    "overall_score": {{ overall_score }},
    "overall_status": "{{ 'compliant' if overall_score >= 95 else 'needs_attention' if overall_score >= 75 else 'non_compliant' }}",
    "summary": {
        "total_checks": {{ total_checks }},
        "passed": {{ all_passed }},
        "failed": {{ all_failed }},
        "warning": {{ all_warning }}
    },
    "frameworks": {{ framework_scores | tojson }},
    "critical_findings": {{ checks | selectattr("status", "eq", "failed") | selectattr("severity", "eq", "critical") | list | tojson }}
}
```

### SOC 2 Control Checks

```jinja
{# SOC 2 Trust Services Criteria checks #}
{% set soc2_controls = {
    "CC1": {
        "name": "Control Environment",
        "controls": [
            {"id": "CC1.1", "name": "COSO Principle 1", "check": "organizational_structure_defined"},
            {"id": "CC1.2", "name": "COSO Principle 2", "check": "board_oversight_exists"},
            {"id": "CC1.3", "name": "COSO Principle 3", "check": "management_structure_defined"},
            {"id": "CC1.4", "name": "COSO Principle 4", "check": "competence_commitment_exists"},
            {"id": "CC1.5", "name": "COSO Principle 5", "check": "accountability_established"}
        ]
    },
    "CC2": {
        "name": "Communication and Information",
        "controls": [
            {"id": "CC2.1", "name": "COSO Principle 13", "check": "quality_information_used"},
            {"id": "CC2.2", "name": "COSO Principle 14", "check": "internal_communication_exists"},
            {"id": "CC2.3", "name": "COSO Principle 15", "check": "external_communication_exists"}
        ]
    },
    "CC3": {
        "name": "Risk Assessment",
        "controls": [
            {"id": "CC3.1", "name": "COSO Principle 6", "check": "objectives_specified"},
            {"id": "CC3.2", "name": "COSO Principle 7", "check": "risks_identified"},
            {"id": "CC3.3", "name": "COSO Principle 8", "check": "fraud_risk_assessed"},
            {"id": "CC3.4", "name": "COSO Principle 9", "check": "change_assessed"}
        ]
    },
    "CC5": {
        "name": "Control Activities",
        "controls": [
            {"id": "CC5.1", "name": "COSO Principle 10", "check": "control_activities_selected"},
            {"id": "CC5.2", "name": "COSO Principle 11", "check": "technology_controls_exist"},
            {"id": "CC5.3", "name": "COSO Principle 12", "check": "policies_deployed"}
        ]
    },
    "CC6": {
        "name": "Logical and Physical Access",
        "controls": [
            {"id": "CC6.1", "name": "Logical Access Security", "check": "logical_access_controls"},
            {"id": "CC6.2", "name": "Access Registration", "check": "access_provisioning_process"},
            {"id": "CC6.3", "name": "Access Removal", "check": "access_deprovisioning_process"},
            {"id": "CC6.4", "name": "Access Review", "check": "access_review_periodic"},
            {"id": "CC6.5", "name": "Physical Access", "check": "physical_access_restricted"},
            {"id": "CC6.6", "name": "Transmission Security", "check": "data_transmission_protected"},
            {"id": "CC6.7", "name": "Data Disposal", "check": "data_disposal_process"},
            {"id": "CC6.8", "name": "Media Protection", "check": "media_protection_controls"}
        ]
    },
    "CC7": {
        "name": "System Operations",
        "controls": [
            {"id": "CC7.1", "name": "Vulnerability Management", "check": "vulnerability_scanning"},
            {"id": "CC7.2", "name": "Security Monitoring", "check": "security_monitoring_active"},
            {"id": "CC7.3", "name": "Event Evaluation", "check": "security_events_evaluated"},
            {"id": "CC7.4", "name": "Incident Response", "check": "incident_response_process"},
            {"id": "CC7.5", "name": "Incident Recovery", "check": "incident_recovery_process"}
        ]
    },
    "CC8": {
        "name": "Change Management",
        "controls": [
            {"id": "CC8.1", "name": "Change Management", "check": "change_management_process"}
        ]
    },
    "CC9": {
        "name": "Risk Mitigation",
        "controls": [
            {"id": "CC9.1", "name": "Risk Mitigation", "check": "risk_mitigation_activities"},
            {"id": "CC9.2", "name": "Vendor Management", "check": "vendor_risk_management"}
        ]
    }
} %}

{# Available automated checks #}
{% set automated_checks = {
    "logical_access_controls": CTX.mfa_enabled and CTX.rbac_implemented,
    "access_provisioning_process": CTX.provisioning_workflow_exists,
    "access_deprovisioning_process": CTX.offboarding_workflow_exists,
    "access_review_periodic": CTX.last_access_review_days <= 90,
    "data_transmission_protected": CTX.tls_enforced,
    "vulnerability_scanning": CTX.vulnerability_scan_days <= 30,
    "security_monitoring_active": CTX.siem_configured,
    "security_events_evaluated": CTX.alert_review_process,
    "incident_response_process": CTX.incident_response_plan_exists,
    "change_management_process": CTX.change_management_documented
} %}

{# Generate check results #}
{% set results = [] %}
{% for category_id, category in soc2_controls.items() %}
    {% for control in category.controls %}
        {% set check_name = control.check %}
        {% set is_automated = check_name in automated_checks %}
        {% set status = "passed" if automated_checks.get(check_name, false) else "manual_review" if not is_automated else "failed" %}
        {% set _ = results.append({
            "framework": "SOC2",
            "category": category_id,
            "category_name": category.name,
            "control_id": control.id,
            "control_name": control.name,
            "check": check_name,
            "is_automated": is_automated,
            "status": status,
            "severity": "high" if category_id in ["CC6", "CC7"] else "medium"
        }) %}
    {% endfor %}
{% endfor %}

{{ results }}
```

### HIPAA Control Checks

```jinja
{# HIPAA Security Rule checks #}
{% set hipaa_controls = {
    "administrative": {
        "name": "Administrative Safeguards",
        "controls": [
            {"id": "164.308(a)(1)", "name": "Security Management Process", "checks": ["risk_analysis", "risk_management", "sanction_policy", "information_system_review"]},
            {"id": "164.308(a)(2)", "name": "Assigned Security Responsibility", "checks": ["security_officer_assigned"]},
            {"id": "164.308(a)(3)", "name": "Workforce Security", "checks": ["authorization_procedures", "workforce_clearance", "termination_procedures"]},
            {"id": "164.308(a)(4)", "name": "Information Access Management", "checks": ["access_authorization", "access_establishment"]},
            {"id": "164.308(a)(5)", "name": "Security Awareness Training", "checks": ["security_reminders", "malware_protection_training", "login_monitoring_training", "password_management_training"]},
            {"id": "164.308(a)(6)", "name": "Security Incident Procedures", "checks": ["incident_response_reporting"]},
            {"id": "164.308(a)(7)", "name": "Contingency Plan", "checks": ["data_backup_plan", "disaster_recovery_plan", "emergency_mode_plan", "testing_procedures", "applications_criticality"]},
            {"id": "164.308(a)(8)", "name": "Evaluation", "checks": ["periodic_evaluation"]}
        ]
    },
    "physical": {
        "name": "Physical Safeguards",
        "controls": [
            {"id": "164.310(a)", "name": "Facility Access Controls", "checks": ["contingency_operations", "facility_security_plan", "access_control_validation", "maintenance_records"]},
            {"id": "164.310(b)", "name": "Workstation Use", "checks": ["workstation_use_policy"]},
            {"id": "164.310(c)", "name": "Workstation Security", "checks": ["workstation_security_controls"]},
            {"id": "164.310(d)", "name": "Device and Media Controls", "checks": ["media_disposal", "media_reuse", "accountability", "data_backup_storage"]}
        ]
    },
    "technical": {
        "name": "Technical Safeguards",
        "controls": [
            {"id": "164.312(a)", "name": "Access Control", "checks": ["unique_user_identification", "emergency_access_procedure", "automatic_logoff", "encryption_decryption"]},
            {"id": "164.312(b)", "name": "Audit Controls", "checks": ["audit_controls_implemented"]},
            {"id": "164.312(c)", "name": "Integrity", "checks": ["mechanism_authenticate_ephi"]},
            {"id": "164.312(d)", "name": "Person Authentication", "checks": ["person_entity_authentication"]},
            {"id": "164.312(e)", "name": "Transmission Security", "checks": ["integrity_controls", "encryption_transmission"]}
        ]
    }
} %}

{# Automated technical checks #}
{% set technical_checks = {
    "unique_user_identification": CTX.all_users_have_unique_ids,
    "automatic_logoff": CTX.session_timeout_configured,
    "encryption_decryption": CTX.encryption_at_rest_enabled,
    "audit_controls_implemented": CTX.audit_logging_enabled,
    "mechanism_authenticate_ephi": CTX.data_integrity_controls,
    "person_entity_authentication": CTX.mfa_enabled,
    "encryption_transmission": CTX.tls_enforced
} %}

{# Generate HIPAA compliance status #}
{% set hipaa_results = [] %}
{% for category_id, category in hipaa_controls.items() %}
    {% for control in category.controls %}
        {% for check in control.checks %}
            {% set is_automated = check in technical_checks %}
            {% set status = "passed" if technical_checks.get(check, false) else "manual_review" if not is_automated else "failed" %}
            {% set _ = hipaa_results.append({
                "framework": "HIPAA",
                "category": category_id,
                "category_name": category.name,
                "control_id": control.id,
                "control_name": control.name,
                "check": check,
                "is_automated": is_automated,
                "status": status,
                "severity": "critical" if category_id == "technical" else "high"
            }) %}
        {% endfor %}
    {% endfor %}
{% endfor %}

{{ hipaa_results }}
```

### CIS Controls Assessment

```jinja
{# CIS Controls v8 automated assessment #}
{% set cis_controls = [
    {
        "id": "CIS1",
        "name": "Inventory and Control of Enterprise Assets",
        "checks": [
            {"id": "1.1", "name": "Establish Asset Inventory", "automated_check": "asset_inventory_exists"},
            {"id": "1.2", "name": "Address Unauthorized Assets", "automated_check": "unauthorized_asset_detection"}
        ]
    },
    {
        "id": "CIS2",
        "name": "Inventory and Control of Software Assets",
        "checks": [
            {"id": "2.1", "name": "Establish Software Inventory", "automated_check": "software_inventory_exists"},
            {"id": "2.2", "name": "Ensure Authorized Software", "automated_check": "software_whitelist_enforced"},
            {"id": "2.3", "name": "Address Unauthorized Software", "automated_check": "unauthorized_software_detection"}
        ]
    },
    {
        "id": "CIS3",
        "name": "Data Protection",
        "checks": [
            {"id": "3.1", "name": "Establish Data Management Process", "automated_check": "data_classification_exists"},
            {"id": "3.4", "name": "Enforce Data Retention", "automated_check": "retention_policies_enforced"},
            {"id": "3.6", "name": "Encrypt Data on End-User Devices", "automated_check": "endpoint_encryption_enabled"},
            {"id": "3.9", "name": "Encrypt Data in Transit", "automated_check": "tls_enforced"},
            {"id": "3.11", "name": "Encrypt Data at Rest", "automated_check": "encryption_at_rest_enabled"}
        ]
    },
    {
        "id": "CIS4",
        "name": "Secure Configuration of Enterprise Assets",
        "checks": [
            {"id": "4.1", "name": "Establish Secure Configuration Process", "automated_check": "baseline_configs_exist"},
            {"id": "4.7", "name": "Manage Default Accounts", "automated_check": "default_accounts_secured"}
        ]
    },
    {
        "id": "CIS5",
        "name": "Account Management",
        "checks": [
            {"id": "5.1", "name": "Establish Account Inventory", "automated_check": "account_inventory_exists"},
            {"id": "5.2", "name": "Use Unique Passwords", "automated_check": "password_policy_enforced"},
            {"id": "5.3", "name": "Disable Dormant Accounts", "automated_check": "dormant_account_detection"},
            {"id": "5.4", "name": "Restrict Administrator Privileges", "automated_check": "admin_least_privilege"},
            {"id": "5.6", "name": "Centralized Account Management", "automated_check": "centralized_identity"}
        ]
    },
    {
        "id": "CIS6",
        "name": "Access Control Management",
        "checks": [
            {"id": "6.1", "name": "Establish Access Granting Process", "automated_check": "access_request_process"},
            {"id": "6.2", "name": "Establish Access Revoking Process", "automated_check": "access_revocation_process"},
            {"id": "6.3", "name": "Require MFA for Remote Access", "automated_check": "mfa_remote_access"},
            {"id": "6.4", "name": "Require MFA for Administrative Access", "automated_check": "mfa_admin_access"},
            {"id": "6.5", "name": "Require MFA for All Access", "automated_check": "mfa_all_access"}
        ]
    },
    {
        "id": "CIS7",
        "name": "Continuous Vulnerability Management",
        "checks": [
            {"id": "7.1", "name": "Establish Vulnerability Management Process", "automated_check": "vuln_management_process"},
            {"id": "7.2", "name": "Establish Remediation Process", "automated_check": "vuln_remediation_process"},
            {"id": "7.4", "name": "Perform Automated Vulnerability Scans", "automated_check": "automated_vuln_scanning"},
            {"id": "7.5", "name": "Perform Authenticated Vulnerability Scans", "automated_check": "authenticated_scanning"}
        ]
    },
    {
        "id": "CIS8",
        "name": "Audit Log Management",
        "checks": [
            {"id": "8.1", "name": "Establish Audit Log Management Process", "automated_check": "log_management_process"},
            {"id": "8.2", "name": "Collect Audit Logs", "automated_check": "centralized_logging"},
            {"id": "8.5", "name": "Collect DNS Logs", "automated_check": "dns_logging_enabled"},
            {"id": "8.9", "name": "Centralize Audit Logs", "automated_check": "siem_configured"},
            {"id": "8.11", "name": "Conduct Audit Log Reviews", "automated_check": "log_review_process"}
        ]
    }
] %}

{# Check statuses from data collection #}
{% set check_data = CTX.compliance_data | d({}) %}

{% set cis_results = [] %}
{% for control in cis_controls %}
    {% for check in control.checks %}
        {% set check_key = check.automated_check %}
        {% set check_result = check_data.get(check_key, none) %}
        {% set status = "passed" if check_result == true else "failed" if check_result == false else "not_assessed" %}
        {% set _ = cis_results.append({
            "framework": "CIS_v8",
            "control_id": control.id,
            "control_name": control.name,
            "check_id": check.id,
            "check_name": check.name,
            "status": status,
            "severity": "high" if control.id in ["CIS3", "CIS5", "CIS6"] else "medium"
        }) %}
    {% endfor %}
{% endfor %}

{{ cis_results }}
```

---

## Evidence Collection Patterns

### Automated Evidence Gathering

```jinja
{# Collect evidence for compliance audits #}
{% set evidence_requirements = CTX.evidence_requirements %}
{% set date_range = CTX.audit_date_range %}

{% set evidence_sources = {
    "user_access_list": {
        "source": "azure_ad",
        "query": "list_users_with_roles",
        "description": "List of all users with their assigned roles and permissions"
    },
    "mfa_status": {
        "source": "azure_ad",
        "query": "mfa_registration_status",
        "description": "MFA enrollment status for all users"
    },
    "access_reviews": {
        "source": "azure_ad",
        "query": "access_review_history",
        "description": "History of access review completions"
    },
    "login_attempts": {
        "source": "azure_ad",
        "query": "sign_in_logs",
        "description": "Sign-in logs including failed attempts"
    },
    "privileged_access": {
        "source": "azure_ad",
        "query": "admin_role_assignments",
        "description": "List of users with administrative privileges"
    },
    "terminated_users": {
        "source": "hr_system",
        "query": "terminated_employees",
        "description": "List of terminated employees during audit period"
    },
    "offboarding_tickets": {
        "source": "psa",
        "query": "offboarding_tickets",
        "description": "Tickets related to user offboarding"
    },
    "security_incidents": {
        "source": "psa",
        "query": "security_tickets",
        "description": "Security incident tickets during audit period"
    },
    "vulnerability_scans": {
        "source": "vulnerability_scanner",
        "query": "scan_reports",
        "description": "Vulnerability scan results"
    },
    "patch_compliance": {
        "source": "rmm",
        "query": "patch_status",
        "description": "Patch compliance status for all endpoints"
    },
    "backup_verification": {
        "source": "backup_system",
        "query": "backup_test_results",
        "description": "Backup verification test results"
    },
    "encryption_status": {
        "source": "rmm",
        "query": "bitlocker_status",
        "description": "Endpoint encryption status"
    },
    "firewall_rules": {
        "source": "firewall",
        "query": "rule_export",
        "description": "Current firewall rule configuration"
    },
    "security_training": {
        "source": "training_platform",
        "query": "completion_report",
        "description": "Security awareness training completion"
    }
} %}

{# Build evidence collection tasks #}
{% set collection_tasks = [] %}
{% for evidence_id in evidence_requirements %}
    {% if evidence_id in evidence_sources %}
        {% set source = evidence_sources[evidence_id] %}
        {% set _ = collection_tasks.append({
            "evidence_id": evidence_id,
            "description": source.description,
            "source": source.source,
            "query": source.query,
            "date_range": date_range,
            "status": "pending"
        }) %}
    {% endif %}
{% endfor %}

{{ collection_tasks }}
```

### Evidence Documentation Template

```jinja
{# Generate evidence documentation for audit #}
{% set evidence = CTX.collected_evidence %}
{% set audit = CTX.audit_info %}

{# Evidence Document Structure #}
{% set document = {
    "metadata": {
        "document_title": "Compliance Evidence Package",
        "audit_name": audit.name,
        "audit_period": audit.start_date ~ " to " ~ audit.end_date,
        "prepared_by": audit.preparer,
        "prepared_date": CTX.current_time | format_datetime("%Y-%m-%d"),
        "organization": audit.organization_name,
        "framework": audit.framework
    },
    "executive_summary": {
        "total_controls": evidence.controls | length,
        "controls_with_evidence": evidence.controls | selectattr("evidence_count", "gt", 0) | list | length,
        "evidence_items": evidence.items | length,
        "compliance_score": evidence.compliance_score
    },
    "evidence_inventory": []
} %}

{# Build evidence inventory #}
{% for item in evidence.items %}
    {% set _ = document.evidence_inventory.append({
        "evidence_id": item.id,
        "control_reference": item.control_id,
        "description": item.description,
        "source_system": item.source,
        "collection_date": item.collected_at,
        "file_reference": item.file_path | d("N/A"),
        "hash": item.sha256_hash | d("N/A"),
        "reviewer": item.reviewed_by | d("Pending Review"),
        "review_date": item.reviewed_at | d("Pending"),
        "notes": item.notes | d("")
    }) %}
{% endfor %}

{{ document }}
```

### Access Review Evidence

```jinja
{# Generate access review evidence for audit #}
{% set users = CTX.all_users %}
{% set review_period = CTX.review_period %}

{% set access_review = {
    "review_date": CTX.current_time | format_datetime("%Y-%m-%d"),
    "review_period": review_period,
    "reviewer": CTX.reviewer_name,
    "total_accounts": users | length,
    "active_accounts": users | selectattr("accountEnabled", "eq", true) | list | length,
    "disabled_accounts": users | selectattr("accountEnabled", "eq", false) | list | length,
    "admin_accounts": users | selectattr("is_admin", "eq", true) | list | length,
    "service_accounts": users | selectattr("is_service_account", "eq", true) | list | length,
    "stale_accounts": [],
    "privileged_users": [],
    "findings": []
} %}

{# Identify stale accounts #}
{% set stale_threshold_days = 90 %}
{% set stale_date = CTX.current_time | as_datetime | datedelta(days=-stale_threshold_days) %}

{% for user in users %}
    {% if user.lastSignInDateTime %}
        {% set last_login = user.lastSignInDateTime | as_datetime %}
        {% if last_login < stale_date and user.accountEnabled %}
            {% set _ = access_review.stale_accounts.append({
                "user_principal_name": user.userPrincipalName,
                "display_name": user.displayName,
                "last_login": user.lastSignInDateTime,
                "days_since_login": ((CTX.current_time | as_datetime) - last_login).days,
                "department": user.department | d("Unknown"),
                "recommendation": "Review and disable if no longer needed"
            }) %}
            {% set _ = access_review.findings.append({
                "type": "stale_account",
                "severity": "medium",
                "user": user.userPrincipalName,
                "description": "Account inactive for " ~ ((CTX.current_time | as_datetime) - last_login).days ~ " days"
            }) %}
        {% endif %}
    {% endif %}

    {# Document privileged users #}
    {% if user.is_admin %}
        {% set _ = access_review.privileged_users.append({
            "user_principal_name": user.userPrincipalName,
            "display_name": user.displayName,
            "admin_roles": user.admin_roles | join(", "),
            "mfa_enabled": user.mfa_enabled | d(false),
            "last_login": user.lastSignInDateTime | d("Never"),
            "justification": user.admin_justification | d("Requires documentation")
        }) %}

        {# Flag admins without MFA #}
        {% if not user.mfa_enabled | d(false) %}
            {% set _ = access_review.findings.append({
                "type": "admin_without_mfa",
                "severity": "critical",
                "user": user.userPrincipalName,
                "description": "Administrative account without MFA enabled"
            }) %}
        {% endif %}
    {% endif %}
{% endfor %}

{{ access_review }}
```

---

## Audit Workflow Patterns

### Pre-Audit Preparation Workflow

```jinja
{# Pre-audit preparation checklist and status #}
{% set audit = CTX.upcoming_audit %}
{% set days_until_audit = CTX.days_until_audit %}

{% set preparation_tasks = [
    {
        "id": "scope_confirmation",
        "name": "Confirm Audit Scope",
        "deadline_days_before": 30,
        "owner": "Compliance Manager",
        "status": CTX.scope_confirmed | d(false),
        "description": "Confirm systems, processes, and controls in scope"
    },
    {
        "id": "evidence_collection",
        "name": "Collect Required Evidence",
        "deadline_days_before": 21,
        "owner": "IT Team",
        "status": CTX.evidence_collected | d(false),
        "description": "Gather all required documentation and evidence"
    },
    {
        "id": "control_testing",
        "name": "Internal Control Testing",
        "deadline_days_before": 14,
        "owner": "Compliance Manager",
        "status": CTX.controls_tested | d(false),
        "description": "Perform internal testing of all controls"
    },
    {
        "id": "gap_remediation",
        "name": "Remediate Identified Gaps",
        "deadline_days_before": 7,
        "owner": "IT Team",
        "status": CTX.gaps_remediated | d(false),
        "description": "Address any control deficiencies identified"
    },
    {
        "id": "documentation_review",
        "name": "Review All Documentation",
        "deadline_days_before": 5,
        "owner": "Compliance Manager",
        "status": CTX.docs_reviewed | d(false),
        "description": "Final review of all policies and procedures"
    },
    {
        "id": "team_briefing",
        "name": "Brief Interview Participants",
        "deadline_days_before": 3,
        "owner": "Compliance Manager",
        "status": CTX.team_briefed | d(false),
        "description": "Prepare team members for auditor interviews"
    },
    {
        "id": "logistics",
        "name": "Audit Logistics",
        "deadline_days_before": 2,
        "owner": "Office Manager",
        "status": CTX.logistics_ready | d(false),
        "description": "Conference room, network access, parking"
    }
] %}

{# Calculate task status #}
{% set task_status = [] %}
{% for task in preparation_tasks %}
    {% set deadline = days_until_audit - task.deadline_days_before %}
    {% set is_overdue = deadline < 0 and not task.status %}
    {% set is_due_soon = deadline <= 3 and deadline >= 0 and not task.status %}
    {% set _ = task_status.append({
        "id": task.id,
        "name": task.name,
        "owner": task.owner,
        "description": task.description,
        "deadline_days_before_audit": task.deadline_days_before,
        "days_remaining": deadline,
        "status": "complete" if task.status else "overdue" if is_overdue else "due_soon" if is_due_soon else "pending",
        "completed": task.status
    }) %}
{% endfor %}

{# Summary #}
{% set completed = task_status | selectattr("completed", "eq", true) | list | length %}
{% set overdue = task_status | selectattr("status", "eq", "overdue") | list | length %}

{
    "audit_name": "{{ audit.name }}",
    "audit_date": "{{ audit.date }}",
    "days_until_audit": {{ days_until_audit }},
    "preparation_progress": {{ (completed / (task_status | length) * 100) | round(0) }},
    "tasks_completed": {{ completed }},
    "tasks_total": {{ task_status | length }},
    "tasks_overdue": {{ overdue }},
    "readiness_status": "{{ 'ready' if completed == task_status | length else 'at_risk' if overdue > 0 else 'on_track' }}",
    "tasks": {{ task_status | tojson }}
}
```

### Finding Tracking

```jinja
{# Track and manage audit findings #}
{% set findings = CTX.audit_findings | d([]) %}

{% set severity_weights = {
    "critical": 10,
    "high": 5,
    "medium": 2,
    "low": 1
} %}

{# Categorize findings #}
{% set by_status = {
    "open": findings | selectattr("status", "eq", "open") | list,
    "in_progress": findings | selectattr("status", "eq", "in_progress") | list,
    "remediated": findings | selectattr("status", "eq", "remediated") | list,
    "accepted": findings | selectattr("status", "eq", "risk_accepted") | list
} %}

{# Calculate risk score #}
{% set risk_score = 0 %}
{% for finding in by_status.open + by_status.in_progress %}
    {% set weight = severity_weights[finding.severity | lower] | d(1) %}
    {% set risk_score = risk_score + weight %}
{% endfor %}

{# Overdue findings #}
{% set overdue = [] %}
{% for finding in by_status.open + by_status.in_progress %}
    {% if finding.due_date %}
        {% set due = finding.due_date | as_datetime %}
        {% if due < CTX.current_time | as_datetime %}
            {% set _ = overdue.append(finding) %}
        {% endif %}
    {% endif %}
{% endfor %}

{
    "summary": {
        "total_findings": {{ findings | length }},
        "open": {{ by_status.open | length }},
        "in_progress": {{ by_status.in_progress | length }},
        "remediated": {{ by_status.remediated | length }},
        "risk_accepted": {{ by_status.accepted | length }},
        "overdue": {{ overdue | length }},
        "risk_score": {{ risk_score }},
        "risk_level": "{{ 'critical' if risk_score >= 30 else 'high' if risk_score >= 15 else 'medium' if risk_score >= 5 else 'low' }}"
    },
    "by_severity": {
        "critical": {{ findings | selectattr("severity", "eq", "critical") | list | length }},
        "high": {{ findings | selectattr("severity", "eq", "high") | list | length }},
        "medium": {{ findings | selectattr("severity", "eq", "medium") | list | length }},
        "low": {{ findings | selectattr("severity", "eq", "low") | list | length }}
    },
    "open_findings": {{ by_status.open | tojson }},
    "overdue_findings": {{ overdue | tojson }},
    "requiring_attention": {{ (by_status.open + by_status.in_progress) | selectattr("severity", "in", ["critical", "high"]) | list | tojson }}
}
```

---

## Policy Compliance Monitoring

### Password Policy Compliance

```jinja
{# Check password policy compliance #}
{% set users = CTX.all_users %}
{% set policy = CTX.password_policy %}

{% set policy_requirements = {
    "min_length": policy.min_length | d(12),
    "require_uppercase": policy.require_uppercase | d(true),
    "require_lowercase": policy.require_lowercase | d(true),
    "require_numbers": policy.require_numbers | d(true),
    "require_symbols": policy.require_symbols | d(true),
    "max_age_days": policy.max_age_days | d(90),
    "password_history": policy.history_count | d(12),
    "mfa_required": policy.mfa_required | d(true)
} %}

{% set compliance_results = [] %}
{% set non_compliant_users = [] %}

{% for user in users %}
    {% if user.accountEnabled %}
        {% set issues = [] %}

        {# Check password age #}
        {% if user.passwordLastChanged %}
            {% set password_age = ((CTX.current_time | as_datetime) - (user.passwordLastChanged | as_datetime)).days %}
            {% if password_age > policy_requirements.max_age_days %}
                {% set _ = issues.append("Password expired (" ~ password_age ~ " days old)") %}
            {% endif %}
        {% else %}
            {% set _ = issues.append("Password never changed") %}
        {% endif %}

        {# Check MFA #}
        {% if policy_requirements.mfa_required and not user.mfaEnabled | d(false) %}
            {% set _ = issues.append("MFA not enabled") %}
        {% endif %}

        {% if issues | length > 0 %}
            {% set _ = non_compliant_users.append({
                "user": user.userPrincipalName,
                "display_name": user.displayName,
                "issues": issues,
                "is_admin": user.is_admin | d(false)
            }) %}
        {% endif %}

        {% set _ = compliance_results.append({
            "user": user.userPrincipalName,
            "compliant": issues | length == 0,
            "issues": issues
        }) %}
    {% endif %}
{% endfor %}

{# Calculate compliance rate #}
{% set total_users = compliance_results | length %}
{% set compliant_users = compliance_results | selectattr("compliant", "eq", true) | list | length %}
{% set compliance_rate = (compliant_users / total_users * 100) | round(1) if total_users > 0 else 100 %}

{
    "policy_requirements": {{ policy_requirements | tojson }},
    "summary": {
        "total_users": {{ total_users }},
        "compliant": {{ compliant_users }},
        "non_compliant": {{ total_users - compliant_users }},
        "compliance_rate": {{ compliance_rate }}
    },
    "status": "{{ 'compliant' if compliance_rate >= 95 else 'needs_attention' if compliance_rate >= 80 else 'non_compliant' }}",
    "non_compliant_users": {{ non_compliant_users | tojson }},
    "admin_non_compliant": {{ non_compliant_users | selectattr("is_admin", "eq", true) | list | tojson }}
}
```

### Endpoint Compliance

```jinja
{# Check endpoint security compliance #}
{% set endpoints = CTX.all_endpoints %}
{% set requirements = CTX.endpoint_requirements %}

{% set compliance_checks = {
    "encryption_enabled": {
        "name": "Disk Encryption",
        "check_field": "bitlocker_status",
        "expected": "encrypted"
    },
    "antivirus_active": {
        "name": "Antivirus Active",
        "check_field": "av_status",
        "expected": "active"
    },
    "av_definitions_current": {
        "name": "AV Definitions Current",
        "check_field": "av_definitions_age_days",
        "max_value": 3
    },
    "os_supported": {
        "name": "Supported OS Version",
        "check_field": "os_supported",
        "expected": true
    },
    "patches_current": {
        "name": "Patches Current",
        "check_field": "missing_critical_patches",
        "max_value": 0
    },
    "firewall_enabled": {
        "name": "Firewall Enabled",
        "check_field": "firewall_status",
        "expected": "enabled"
    }
} %}

{% set endpoint_results = [] %}
{% for endpoint in endpoints %}
    {% set checks_passed = 0 %}
    {% set checks_failed = [] %}
    {% set total_checks = compliance_checks | length %}

    {% for check_id, check in compliance_checks.items() %}
        {% set value = endpoint.get(check.check_field) %}
        {% set passed = false %}

        {% if check.expected is defined %}
            {% set passed = value == check.expected %}
        {% elif check.max_value is defined %}
            {% set passed = value is not none and value <= check.max_value %}
        {% endif %}

        {% if passed %}
            {% set checks_passed = checks_passed + 1 %}
        {% else %}
            {% set _ = checks_failed.append({
                "check": check.name,
                "expected": check.expected | d("≤" ~ check.max_value),
                "actual": value
            }) %}
        {% endif %}
    {% endfor %}

    {% set compliance_score = (checks_passed / total_checks * 100) | round(0) if total_checks > 0 else 0 %}
    {% set _ = endpoint_results.append({
        "device_name": endpoint.hostname,
        "device_id": endpoint.id,
        "os": endpoint.operating_system,
        "user": endpoint.primary_user | d("Unknown"),
        "compliance_score": compliance_score,
        "checks_passed": checks_passed,
        "checks_failed": checks_failed | length,
        "failed_checks": checks_failed,
        "status": "compliant" if compliance_score == 100 else "partial" if compliance_score >= 75 else "non_compliant"
    }) %}
{% endfor %}

{# Summary statistics #}
{% set fully_compliant = endpoint_results | selectattr("status", "eq", "compliant") | list | length %}
{% set partial = endpoint_results | selectattr("status", "eq", "partial") | list | length %}
{% set non_compliant = endpoint_results | selectattr("status", "eq", "non_compliant") | list | length %}

{
    "summary": {
        "total_endpoints": {{ endpoint_results | length }},
        "fully_compliant": {{ fully_compliant }},
        "partially_compliant": {{ partial }},
        "non_compliant": {{ non_compliant }},
        "overall_compliance_rate": {{ (fully_compliant / (endpoint_results | length) * 100) | round(1) if endpoint_results | length > 0 else 100 }}
    },
    "non_compliant_endpoints": {{ endpoint_results | selectattr("status", "ne", "compliant") | sort(attribute="compliance_score") | list | tojson }},
    "common_failures": {{
        endpoint_results
        | map(attribute="failed_checks")
        | sum(start=[])
        | map(attribute="check")
        | list
        | unique
        | list
        | tojson
    }}
}
```

---

## Compliance Reporting

### Executive Compliance Dashboard

```jinja
{# Executive compliance summary #}
{% set org = CTX.organization %}
{% set frameworks = CTX.compliance_frameworks %}
{% set period = CTX.reporting_period %}

{% set dashboard = {
    "organization": org.name,
    "report_date": CTX.current_time | format_datetime("%Y-%m-%d"),
    "reporting_period": period,
    "overall_status": "compliant",
    "risk_score": 0,
    "frameworks": [],
    "key_metrics": {},
    "trending": {},
    "action_items": []
} %}

{% set total_risk = 0 %}
{% for framework in frameworks %}
    {% set framework_data = {
        "name": framework.name,
        "score": framework.compliance_score,
        "status": framework.status,
        "controls_total": framework.total_controls,
        "controls_compliant": framework.compliant_controls,
        "last_assessment": framework.last_assessment_date,
        "next_audit": framework.next_audit_date | d("Not scheduled")
    } %}
    {% set _ = dashboard.frameworks.append(framework_data) %}

    {# Accumulate risk #}
    {% set risk_factor = (100 - framework.compliance_score) * framework.risk_weight | d(1) %}
    {% set total_risk = total_risk + risk_factor %}

    {# Add action items for failing controls #}
    {% for finding in framework.open_findings | d([]) %}
        {% if finding.severity in ["critical", "high"] %}
            {% set _ = dashboard.action_items.append({
                "framework": framework.name,
                "finding": finding.title,
                "severity": finding.severity,
                "due_date": finding.due_date,
                "owner": finding.owner
            }) %}
        {% endif %}
    {% endfor %}
{% endfor %}

{# Calculate overall risk score (0-100, lower is better) #}
{% set avg_risk = (total_risk / (frameworks | length)) | round(0) if frameworks | length > 0 else 0 %}
{% set _ = dashboard.update({"risk_score": avg_risk}) %}

{# Determine overall status #}
{% set min_score = frameworks | map(attribute="compliance_score") | min | d(100) %}
{% if min_score < 75 %}
    {% set _ = dashboard.update({"overall_status": "non_compliant"}) %}
{% elif min_score < 95 %}
    {% set _ = dashboard.update({"overall_status": "needs_attention"}) %}
{% endif %}

{# Key metrics #}
{% set _ = dashboard.key_metrics.update({
    "mfa_adoption": CTX.mfa_adoption_rate | d(0),
    "patch_compliance": CTX.patch_compliance_rate | d(0),
    "encryption_coverage": CTX.encryption_coverage | d(0),
    "security_training": CTX.training_completion_rate | d(0),
    "access_reviews_current": CTX.access_reviews_current | d(false),
    "backup_verification": CTX.backup_verified | d(false)
}) %}

{# Trending data (vs previous period) #}
{% set prev = CTX.previous_period_data | d({}) %}
{% set _ = dashboard.trending.update({
    "compliance_trend": "improving" if min_score > prev.get("min_score", 0) else "declining" if min_score < prev.get("min_score", 100) else "stable",
    "score_change": min_score - prev.get("min_score", min_score),
    "findings_trend": "improving" if (dashboard.action_items | length) < prev.get("findings_count", 999) else "worsening",
    "findings_change": prev.get("findings_count", 0) - (dashboard.action_items | length)
}) %}

{{ dashboard }}
```

### Compliance Report for Clients

```jinja
{# Client-facing compliance report #}
{% set client = CTX.client %}
{% set compliance = CTX.compliance_data %}
{% set period = CTX.reporting_period %}

# {{ client.name }} Compliance Report

**Report Period:** {{ period.start }} to {{ period.end }}
**Generated:** {{ CTX.current_time | format_datetime("%B %d, %Y") }}
**Prepared By:** {{ ORG.VARIABLES.company_name }}

## Executive Summary

Your organization's overall compliance posture is **{{ compliance.overall_status | upper }}** with a compliance score of **{{ compliance.overall_score }}%**.

{% if compliance.overall_score >= 95 %}
Your security controls are operating effectively. Continue maintaining current practices and monitoring for new requirements.
{% elif compliance.overall_score >= 80 %}
Your security posture is good with some areas requiring attention. We recommend addressing the identified gaps to improve your compliance position.
{% else %}
Significant compliance gaps have been identified that require immediate attention. Please review the findings below and work with your IT team to implement the recommended remediations.
{% endif %}

## Framework Compliance Status

| Framework | Score | Status | Last Assessment |
|-----------|-------|--------|-----------------|
{% for framework in compliance.frameworks %}
| {{ framework.name }} | {{ framework.score }}% | {{ framework.status }} | {{ framework.last_assessment }} |
{% endfor %}

## Security Metrics

| Metric | Current | Target | Status |
|--------|---------|--------|--------|
| MFA Adoption | {{ compliance.metrics.mfa_adoption }}% | 100% | {{ "✅" if compliance.metrics.mfa_adoption >= 95 else "⚠️" if compliance.metrics.mfa_adoption >= 80 else "❌" }} |
| Patch Compliance | {{ compliance.metrics.patch_compliance }}% | 95% | {{ "✅" if compliance.metrics.patch_compliance >= 95 else "⚠️" if compliance.metrics.patch_compliance >= 80 else "❌" }} |
| Endpoint Encryption | {{ compliance.metrics.encryption_coverage }}% | 100% | {{ "✅" if compliance.metrics.encryption_coverage >= 95 else "⚠️" if compliance.metrics.encryption_coverage >= 80 else "❌" }} |
| Security Training | {{ compliance.metrics.training_completion }}% | 100% | {{ "✅" if compliance.metrics.training_completion >= 90 else "⚠️" if compliance.metrics.training_completion >= 75 else "❌" }} |

{% if compliance.findings | length > 0 %}
## Open Findings

The following items require attention:

{% for finding in compliance.findings | sort(attribute="severity") %}
### {{ finding.title }}

**Severity:** {{ finding.severity }}
**Framework:** {{ finding.framework }}
**Due Date:** {{ finding.due_date }}

{{ finding.description }}

**Recommended Action:** {{ finding.recommendation }}

---
{% endfor %}
{% endif %}

## Next Steps

1. Review and address any open findings listed above
2. Schedule remediation activities for critical items
3. Prepare for upcoming compliance assessments
4. Contact us with any questions or concerns

---

*This report is provided for informational purposes. Please contact your account manager for detailed remediation guidance.*
```

---

## Best Practices

### Compliance Automation

1. **Automate evidence collection** to reduce manual effort
2. **Schedule regular assessments** not just before audits
3. **Track findings in ticketing system** for accountability
4. **Maintain audit trail** for all compliance activities
5. **Test backup and recovery** procedures regularly

### Framework Implementation

1. **Start with gap analysis** before implementing controls
2. **Prioritize high-risk areas** first
3. **Document all policies** and procedures
4. **Train staff** on compliance requirements
5. **Review and update** controls periodically

### Audit Preparation

1. **Begin preparation 60+ days** before audit
2. **Conduct internal assessments** first
3. **Brief all participants** on their roles
4. **Organize evidence** by control
5. **Have subject matter experts** available

### Continuous Compliance

1. **Monitor compliance continuously** not just periodically
2. **Integrate compliance checks** into daily operations
3. **Automate remediation** where possible
4. **Track metrics** and trends over time
5. **Report regularly** to leadership
