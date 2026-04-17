# Rewst Crates & Bundles

Pre-built automation packages and configuration guides.

---

## What Are Crates?

**Crates** are pre-built, installable automation packages containing:
- Workflows
- Forms
- Triggers
- Organization variables
- Templates

They're designed to solve common MSP use cases with minimal customization.

---

## Installing Crates

### From the Marketplace

1. Navigate to **Crates** in Rewst
2. Browse or search the Crate Marketplace
3. Click **Install** on desired crate
4. Configure required variables
5. Enable for organizations

### Configuration Steps

1. **Review Requirements** - Check integration prerequisites
2. **Set Org Variables** - Configure required settings
3. **Map Integrations** - Connect to your PSA/RMM/etc.
4. **Test in Sandbox** - Verify with test org first
5. **Enable for Production** - Roll out to client orgs

---

## Essential Crate Categories

### Identity & Access Management

| Crate | Description |
|-------|-------------|
| User Onboarding | Complete new user provisioning |
| User Offboarding | Disable, backup, archive users |
| Password Reset Self-Service | End-user password changes |
| MFA Reset | Reset multi-factor authentication |
| Group Management | Add/remove users from groups |

### Microsoft 365

| Crate | Description |
|-------|-------------|
| M365 License Management | Assign/revoke licenses |
| Shared Mailbox Creation | Create and configure shared mailboxes |
| Distribution List Management | Create/modify DLs |
| Mailbox Delegation | Set up send-as/send-on-behalf |
| Out-of-Office Setup | Configure auto-replies |

### Ticket Automation

| Crate | Description |
|-------|-------------|
| Ticket Triage | Auto-categorize and route |
| SLA Monitoring | Track and alert on SLAs |
| Customer Feedback | Post-resolution surveys |
| Escalation Automation | Time-based escalations |
| Status Sync | Sync ticket status across systems |

### Security & Compliance

| Crate | Description |
|-------|-------------|
| Compromised User Response | Lock, reset, investigate |
| Security Alert Triage | Process security alerts |
| Phishing Response | Handle reported phishing |
| Conditional Access Audit | Review CA policies |
| License Compliance | Track license usage |

### Reporting & Documentation

| Crate | Description |
|-------|-------------|
| Time Saved Reports | Track automation ROI |
| User Audit Reports | Login history, changes |
| License Reports | Usage and allocation |
| Documentation Sync | Keep docs current |

---

## Common Crate Configuration

### User Lifecycle Crates

**Required Org Variables:**

| Variable | Description | Example |
|----------|-------------|---------|
| `psa_default_board_id` | Ticket board for tasks | `123` |
| `m365_usage_location` | Country for licensing | `US` |
| `default_user_licenses` | SKUs to assign | `["ENTERPRISEPACK"]` |
| `username_format` | Format for usernames | `flast` |
| `email_domain` | Primary email domain | `company.com` |
| `default_groups` | Groups for new users | `["All Users", "VPN"]` |

**Integration Mappings:**

```jinja
{# Map org to M365 tenant #}
{{ ORG.VARIABLES.m365_tenant_id }}

{# Map org to PSA company #}
{{ ORG.VARIABLES.psa_company_id }}

{# Map org to RMM site #}
{{ ORG.VARIABLES.rmm_site_id }}
```

### Ticket Automation Crates

**Required Org Variables:**

| Variable | Description | Example |
|----------|-------------|---------|
| `psa_default_board_id` | Default ticket board | `10` |
| `psa_ticket_status_new` | New ticket status | `New` |
| `psa_ticket_status_closed` | Closed status | `Closed` |
| `psa_triage_queue_id` | Triage queue/resource | `5` |
| `sla_priority_map` | Priority to SLA mapping | `{"1": 1, "2": 4, "3": 8}` |

---

## Integration Bundles

### Microsoft Cloud Integration Bundle

Provides unified access to Microsoft services:

| Service | Capabilities |
|---------|-------------|
| Azure AD / Entra ID | Users, groups, licenses |
| Exchange Online | Mailboxes, distribution lists |
| SharePoint | Sites, permissions |
| Teams | Teams, channels |
| Intune | Devices, policies |

**Setup Requirements:**
1. Azure AD App Registration
2. API permissions configured
3. Admin consent granted
4. Service principal created

### ConnectWise Integration Bundle

| Service | Capabilities |
|---------|-------------|
| Manage | Tickets, companies, contacts |
| Automate | Computers, scripts, patching |
| Control | Remote sessions |
| Sell | Quotes, products |

### Datto Integration Bundle

| Service | Capabilities |
|---------|-------------|
| Autotask PSA | Tickets, accounts, resources |
| RMM | Devices, scripts, alerts |
| Backup | Jobs, restores, reports |

---

## Crate Customization

### When to Customize

- **Always customize:** Variable names, email templates, ticket content
- **Sometimes customize:** Workflow logic, additional steps
- **Rarely customize:** Core action sequences

### Safe Customization Points

```
1. Email templates (branding, wording)
2. Ticket templates (content, formatting)
3. Condition thresholds (SLA times, approval limits)
4. Notification recipients
5. Additional logging/documentation
```

### Extending vs. Modifying

**Preferred: Use Completion Handlers**

```
Original Crate Workflow → [Completes] → Your Extension Workflow
```

This preserves the original for updates while adding your logic.

**When to Modify Directly:**
- Bug fixes
- Critical business logic changes
- Integration-specific requirements

---

## Agent Smith Crates

### Device Provisioning Crate

Deploys Agent Smith to endpoints.

**Requirements:**
1. Azure IoT Hub configured
2. Microsoft Cloud Integration Bundle
3. RMM integration

**Process:**
```
1. Create IoT Hub device identity
2. Generate connection string
3. Deploy agent via RMM
4. Verify connectivity
```

### Service Provisioning Crate

Configures Agent Smith for workflows.

**Capabilities:**
- Run PowerShell on endpoints
- Collect system information
- Execute remediation scripts
- Report back to workflows

---

## Crate Versioning

### Update Considerations

| Before Updating | Action |
|----------------|--------|
| Review changelog | Understand changes |
| Check customizations | Note what you've modified |
| Test in sandbox | Verify with test org |
| Backup current version | Export if possible |
| Plan rollback | Know how to revert |

### Version Conflicts

If you've customized a crate:

1. **Option A:** Export your changes, update, re-apply
2. **Option B:** Keep current version, review new for ideas
3. **Option C:** Use completion handlers to avoid conflicts

---

## Creating Custom Crates

### When to Create

- Common pattern used across clients
- Reusable automation package
- Shareable with community

### Structure

```
Custom Crate
├── Workflows (1+)
├── Forms (optional)
├── Triggers (optional)
├── Org Variables (documented)
└── README/Documentation
```

### Best Practices

1. **Document thoroughly** - Required variables, integrations
2. **Use org variables** - Not hardcoded values
3. **Include error handling** - Graceful failures
4. **Test across scenarios** - Different org configs
5. **Version semantically** - Major.minor.patch

---

## ROI Tracking

### Time Saved Configuration

Each workflow can track time saved:

| Field | Description |
|-------|-------------|
| Time Saved per Run | Minutes saved per execution |
| Category | Type of automation |

### Reporting Query

```yaml
# GraphQL for time saved
operation: "timeSavedGroupByWorkflow"
fields: "workflowId, workflowName, secondsSaved, totalExecutions"
```

### Calculating ROI

```jinja
{# Monthly time saved in hours #}
{% set hours_saved = CTX.total_seconds_saved / 3600 %}

{# At $100/hour tech rate #}
{% set monthly_value = hours_saved * 100 %}

Monthly Value: ${{ monthly_value | round(2) }}
```

---

## Troubleshooting Crates

### Common Issues

| Issue | Cause | Solution |
|-------|-------|----------|
| Crate won't install | Missing integration | Enable required integration first |
| Variables undefined | Not configured | Set all required org variables |
| Wrong org context | Multi-tenant issue | Check Run as Org settings |
| Trigger not firing | Not enabled | Enable trigger for organization |
| API errors | Auth expired | Reauthorize integration |

### Debug Steps

1. Check org has required integrations
2. Verify all org variables are set
3. Test with single org first
4. Review workflow execution logs
5. Check integration authorization status
