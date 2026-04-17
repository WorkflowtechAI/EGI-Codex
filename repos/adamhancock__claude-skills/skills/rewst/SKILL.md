---
name: rewst
description: Use when helping with Rewst automation platform - building workflows, writing Jinja templates, designing automations, or referencing Rewst-specific syntax like CTX variables, TASKS results, data aliases, filters, or subworkflows
---

# Rewst Automation Platform

Rewst is an MSP automation platform using **Jinja templating** with Rewst-specific extensions. Workflows contain actions that call APIs, data flows through CTX (context variables) and TASKS (raw action outputs), and Jinja filters transform data.

## Additional Resources

**Core Reference:**
- [jinja-reference.md](jinja-reference.md) - Complete Jinja filter reference, comprehensions, macros
- [advanced-jinja.md](advanced-jinja.md) - Expert techniques, complex transformations, macro libraries
- [workflow-patterns.md](workflow-patterns.md) - Triggers, transitions, With Items, subworkflows, core actions
- [integrations.md](integrations.md) - PSA/RMM/M365 patterns, GraphQL API
- [api-patterns.md](api-patterns.md) - HTTP requests, auth, pagination, retries, webhooks
- [ticket-automation.md](ticket-automation.md) - PSA ticket creation, routing, updates, templates
- [alert-monitoring.md](alert-monitoring.md) - RMM alerts, monitoring, auto-remediation, escalation
- [troubleshooting.md](troubleshooting.md) - Debugging, error messages, checklists
- [examples.md](examples.md) - Real-world workflows, templates
- [forms-appbuilder.md](forms-appbuilder.md) - Forms, options filter, App Builder
- [client-communication.md](client-communication.md) - Email templates, Teams/Slack notifications, multi-channel patterns

**Advanced:**
- [m365-deep-dive.md](m365-deep-dive.md) - License SKUs, Graph API endpoints, pagination
- [rmm-deep-dive.md](rmm-deep-dive.md) - Datto RMM, NinjaRMM, CW Automate, N-able patterns
- [crates-bundles.md](crates-bundles.md) - Pre-built automations, configuration
- [consulting-playbook.md](consulting-playbook.md) - Discovery, pricing, ROI calculations
- [decision-guide.md](decision-guide.md) - Decision trees for common scenarios
- [security-patterns.md](security-patterns.md) - Security automation, compliance, incident response
- [performance-optimization.md](performance-optimization.md) - API optimization, caching, With Items tuning
- [testing-qa.md](testing-qa.md) - Testing patterns, debugging, production validation
- [user-lifecycle.md](user-lifecycle.md) - User provisioning, modifications, offboarding workflows
- [identity-providers.md](identity-providers.md) - Okta, Azure AD B2B, JumpCloud, Google Workspace patterns
- [backup-dr.md](backup-dr.md) - Datto BCDR, Veeam, Acronis, DR workflows, backup monitoring
- [documentation-reporting.md](documentation-reporting.md) - IT Glue, Hudu, report generation, documentation sync
- [billing-usage.md](billing-usage.md) - License management, billing reconciliation, cost optimization
- [network-firewall.md](network-firewall.md) - Fortinet, Meraki, SonicWall, UniFi automation patterns
- [compliance-audit.md](compliance-audit.md) - SOC2, HIPAA, CIS Controls, evidence collection, audit workflows

---

## Data Flow: CTX vs TASKS vs RESULT

**This is the most important concept in Rewst.**

There are three ways data is accessed, depending on whether you published the action result:

| Access Method | Syntax | When to Use |
|--------------|--------|-------------|
| **TASKS** (raw) | `{{ TASKS.action_name.result.result.data }}` | Access data inline without publishing |
| **CTX** (published) | `{{ CTX.var_name.data.value }}` | After using "Publish Result As" on the action |
| **CTX** (data alias) | `{{ CTX.alias_name }}` | After setting a data alias on a transition |
| **RESULT** | `{{ RESULT.result }}` | In a transition, references the current task's output |
| **Org Variable** | `{{ ORG.VARIABLES.name }}` | Organization-level config variables |
| **Input Variable** | `{{ CTX.input_name }}` | Values passed into workflow from trigger/form |

### TASKS Path — Raw Access

```jinja
{# Full path when accessing via TASKS #}
{{ TASKS.action_name.result.result.data }}

{# Example: M365 list users (value contains the array) #}
{{ TASKS.m365_list_users.result.result.data.value }}

{# Filter inline with TASKS #}
{{ [user for user in TASKS.m365_list_users.result.result.data.value if user.userPrincipalName == "user@domain.com"] }}
```

### CTX Path — After "Publish Result As"

When you set **Publish Result As: `var_name`** on an action, the result is stored at `CTX.var_name`. The path within it does **not** include `.result.result`:

```jinja
{# After Publish Result As: user_details #}
{{ CTX.user_details.data.value.displayName }}
{{ CTX.user_details.data.value.userPrincipalName }}
```

### Creating Data Aliases

Data aliases are created on **task transitions** (the small rectangle beneath a task), NOT on the task itself.

1. Click the transition rectangle beneath the task
2. Click **+** to add a data alias
3. Set a **Key** (the CTX variable name) and **Value** (Jinja expression)

```jinja
{# Key: enabled_users — Value: #}
{{ [u for u in TASKS.list_users.result.result.data.value if u.accountEnabled] }}
```

Then access as: `{{ CTX.enabled_users }}`

---

## Jinja Syntax Basics

### Brace Types

- `{{ }}` - Output values/expressions
- `{% %}` - Code blocks (if, for, set)
- `{# #}` - Comments (ignored in output)
- `{%- -%}` - Strip whitespace around block

### Data Types

| Type | Example |
|------|---------|
| Integer | `{{ 1 }}` |
| Float | `{{ 1.1 }}` |
| String | `{{ "hello" }}` |
| List | `{{ ["a", "b", 1] }}` |
| Dictionary | `{{ {"name": "rewsty"} }}` |
| Boolean | `{{ true }}` or `{{ false }}` |
| None | `{{ none }}` |

### Essential Filters

| Filter | Usage | Description |
|--------|-------|-------------|
| `d` / `default` | `{{ var \| d("fallback") }}` | Default if undefined/empty |
| `length` | `{{ list \| length }}` | Count items |
| `first` / `last` | `{{ list \| first }}` | Get first/last item |
| `lower` / `upper` | `{{ str \| lower }}` | Case conversion |
| `capitalize` | `{{ str \| capitalize }}` | First letter uppercase |
| `join` | `{{ list \| join(",") }}` | Join list to string |
| `selectattr` | `{{ list \| selectattr("active") \| list }}` | Filter by attribute |
| `rejectattr` | `{{ list \| rejectattr("active") \| list }}` | Exclude by attribute |
| `map` | `{{ list \| map(attribute="name") \| list }}` | Extract attribute |
| `truncate` | `{{ text \| truncate(20) }}` | Truncate string |
| `replace` | `{{ str \| replace("@", "at") }}` | Replace substring |
| `regex_replace` | `{{ str \| regex_replace("[^a-z]", "") }}` | Regex replace |

### Date/Time Filters (Rewst-specific — NOT standard Jinja2!)

| Filter | Usage | Description |
|--------|-------|-------------|
| `as_datetime` | `{{ "2025-01-15" \| as_datetime }}` | Parse to datetime (**NOT strptime!**) |
| `as_timezone` | `{{ dt \| as_timezone("US/Eastern") }}` | Convert timezone (**NOT astimezone!**) |
| `datedelta` | `{{ dt \| datedelta(days=7) }}` | Add/subtract time |
| `format_datetime` | `{{ dt \| format_datetime("%Y-%m-%d") }}` | Format output |

### List Comprehension

```jinja
{# "Give me a list of all X from Y, but only if Z" #}
{{ [user.id for user in CTX.my_user_list if user.enabled == true] }}

{# With transformation #}
{{ [user.email | lower for user in CTX.users if user.email] }}

{# Multi-line for clarity #}
{{
    [
        user.id
        for user in CTX.my_user_list
        if user.enabled == true
    ]
}}
```

---

## Workflow Fundamentals

### Triggers

| Trigger | Use Case |
|---------|----------|
| Cron Job | Scheduled tasks (`0 3 * * 1` = Mondays 3AM) |
| Form Submission | User requests via Rewst forms |
| Webhook | External integrations calling into Rewst |
| Always Pass | Testing, option generators |
| PSA Ticket Saved | Ticket automation |

### Task Transitions

Transitions are the small rectangles beneath each task. They control workflow flow.

| Condition | Description |
|-----------|-------------|
| Success | Task completed without error |
| Failure | Task encountered an error |
| Always | Regardless of task outcome |
| Custom | Jinja expression evaluated as boolean |

**Transition Modes** (set in Advanced tab):
- **Follow All** (blue) — Default. Follows ALL matching transitions
- **Follow First** (orange) — Evaluates left to right, stops at first match

### With Items (foreach)

Configured in the **Advanced** tab of a task. Equivalent to a `foreach` loop.

```jinja
{# With Items field — pass the list #}
{{ CTX.all_users }}

{# Access the current item inside the action's fields #}
{{ item() }}
{{ item().firstName }}
{{ item().email }}
```

> **Important:** When using With Items, use `item()` — NOT `CTX.all_users.property`.
> Recommended max concurrency: **10**.

### Subworkflows

- **Pass data IN:** Input Configuration on the subworkflow
- **Return data OUT:** Output Configuration on the subworkflow
- **Access in parent:** `{{ CTX.published_name.output_field }}`
- Variables do NOT automatically inherit — must be explicitly passed

### Run as Org

Set in the **Advanced** tab. Executes a task or subworkflow in the context of another organization. Useful for MSP parent→child org operations. Only works from parent→child, not child→parent.

---

## Common Patterns

### Safe Attribute Access

```jinja
{{ CTX.user.email | d("no-email") }}
{{ (CTX.response | d({})).data | d([]) }}
```

### Filter and Transform

```jinja
{{ CTX.users | selectattr("active") | map(attribute="email") | list }}
```

### Dictionary Switch

```jinja
{% set map = {"cwm": "cw_manage", "datto": "datto_psa"} %}
{{ map[CTX.psa_type] | d("unknown") }}
```

### Namespace for Loop Variables

```jinja
{# Use namespace to modify variables inside loops #}
{% set ns = namespace(found=false, result="") %}
{% for item in CTX.items %}
    {% if item.id == CTX.target_id %}
        {% set ns.found = true %}
        {% set ns.result = item.name %}
    {% endif %}
{% endfor %}
{{ ns.result }}
```

### Conditional Statements

```jinja
{% if CTX.user_type == 'admin' %}
    Admin access
{% elif CTX.user_type == 'manager' %}
    Manager access
{% else %}
    Standard access
{% endif %}
```

---

## Option Generator Workflows

Used to populate dynamic dropdowns in forms.

1. Set **Workflow Type** to **Option Generator** in workflow settings
2. Add **Output Configuration** with variable named exactly **`options`**
3. The workflow must produce `{{ CTX.options }}` containing a list with `label` and `value` keys

```jinja
{{
    [
        {"label": user.displayName, "value": user.id}
        for user in CTX.users
    ]
}}
```

The form field's **Label Field** shows what users see; **Value Field** is what the workflow receives.

---

## Core Actions (Built-in)

| Action | Purpose |
|--------|---------|
| `Noop` | No-operation — placeholder or logic gate |
| `Debug` | Logs text/template output; returns same as output |
| `Mock` | Simulates an action response for testing |
| `HTTP Request` | Ad-hoc API call to any URL |
| `Generate Password V2` | Cryptographically secure password generation |
| `Confirmation Email` | Pauses workflow, sends email with clickable buttons |
| `Create Pending Task` | Pauses for manual human approval in Rewst UI |
| `Send Mail` | Sends email via Rewst |
| `Create Webhook` | Creates a one-off webhook |
| `Await Webhook Request` | Pauses workflow until webhook is called |
| `Delay Workflow For Period` | Pauses for days/hours/minutes/seconds |
| `Delay Workflow Until Date/Time` | Pauses until specific datetime |
| `UUID` | Generates a UUID |
| `DNS Query` | Queries nameserver for DNS records |
| `Parse HTML` | Extracts elements from HTML (BeautifulSoup) |
| `Parse XML` | Extracts elements from XML (XPath) |

---

## Organization Variables

```jinja
{{ ORG.VARIABLES.psa_default_board_id }}
{{ ORG.VARIABLES.m365_usage_location | d("US") }}
```

### Common Variables

| Variable | Description |
|----------|-------------|
| `default_psa` | PSA type: cw_manage, datto_psa, halo_psa |
| `default_rmm` | RMM type: cw_automate, datto_rmm |
| `psa_default_board_id` | Default ticket board |
| `m365_usage_location` | Country code for M365 |
| `username_format` | Format: flast, firstl, firstmlast, first.last |
| `email_domain` | Primary email domain |

---

## Build Standards & Best Practices

From Rewst's official ROC team guidelines:

### Naming
- Workflow names: descriptive and clear (`List Disabled User Accounts` not `Steve's Workflow`)
- Variables: use `snake_case` consistently
- Org variables: prefix by integration (`psa_`, `m365_`, `rmm_`)

### Data Aliases
- **Separate complex data alias creation** into a dedicated `Set Variable` (Noop) task rather than on the API call task itself
  - Makes debugging easier: isolates API errors from Jinja errors
  - Allows testing with real data in the Live Editor
- Keep data aliases focused — extract only what you need

### Transitions
- Use `{{ SUCCEEDED and CTX.list_of_things | d }}` to gate on both task success AND data presence
- Order transitions left to right intentionally (evaluated left to right)
- **Follow All** for parallel branching, **Follow First** for if/else routing

### API Calls
- Limit response fields to only what you need (use `$select` in Graph API, etc.)
- Test without field filtering first to explore available data, then restrict

### With Items
- Use a subworkflow for complex per-item logic
- Max concurrency: **10**
- Make the task name unique in the workflow

---

## Common Mistakes

| Mistake | Fix |
|---------|-----|
| `CTX.action_name` for raw results | Use `TASKS.action_name.result.result.data` |
| `CTX.published.result.result.data` | Use `CTX.published.data.value` (no `.result.result`) |
| Using `strptime` for datetime | Use `as_datetime` |
| Using `astimezone` for timezone | Use `as_timezone` |
| Not handling undefined variables | Use `\| d` or `\| default('value')` |
| Creating data alias on the task | Create on **transition** instead |
| With Items: `CTX.list.property` | Use `{{ item().property }}` |
| `selectattr` result not a list | Add `\| list` to materialize |
| Option generator output key wrong | Must be named exactly `options` |
| Variables auto-inherit subworkflows | Must explicitly define input/output config |

---

## Quick Troubleshooting

### Where to Look

1. **Automations > Results** — execution history with inputs and context
2. **Inputs section** — what data entered the workflow
3. **Context section** — all CTX variables created, in order
4. **Failed task** — click to see request sent and response received
5. **Live Editor** — test Jinja expressions against real context data without re-running

### Common Failures

| Error | Likely Cause | Fix |
|-------|-------------|-----|
| Jinja error | Bad syntax, missing data, wrong filter | Use Live Editor to test expression |
| Integration not authorized | 401/403 | Recheck integration settings, reauthorize |
| 404/405 from API | Wrong endpoint or method | Check API docs, verify IDs |
| `200 OK` but no result | Query matched nothing | Check filter parameters |
| Transition criteria not met | Condition didn't evaluate as expected | Check upstream tasks for silent failures |
| Variable not found | Wrong path or alias not set | Check CTX vs TASKS path, alias on transition? |

### Variable Not Found Checklist
- CTX (aliases/published) vs TASKS (raw output)?
- Data alias on **transition**, not task?
- Correct path: `TASKS.action.result.result.data` or `CTX.published.data.value`?
- `Publish Result As` set on the action?

---

## Quick Reference

```jinja
{# Context variable (data alias) #}
{{ CTX.my_var }}

{# Published result (Publish Result As: user_details) #}
{{ CTX.user_details.data.value.displayName }}

{# Raw TASKS access #}
{{ TASKS.list_users.result.result.data.value }}

{# Org variable #}
{{ ORG.VARIABLES.psa_api_key }}

{# Default value #}
{{ CTX.var | d('fallback') }}

{# Parse datetime (Rewst-specific) #}
{{ "2025-01-15" | as_datetime }}

{# Timezone convert (Rewst-specific) #}
{{ dt | as_timezone("US/Eastern") }}

{# Date math #}
{{ dt | datedelta(days=7) }}

{# List comprehension with filter #}
{{ [u for u in CTX.users if u.enabled] }}

{# Current With Items item #}
{{ item().property }}

{# Combine dicts #}
{{ dict1 | combine(dict2) }}

{# Filter by attribute #}
{{ CTX.users | selectattr("active", "eq", true) | list }}

{# Extract attribute #}
{{ CTX.users | map(attribute="email") | list }}

{# Namespace for loop mutation #}
{% set ns = namespace(counter=0) %}
{% for item in CTX.items %}{% set ns.counter = ns.counter + 1 %}{% endfor %}
{{ ns.counter }}
```

---

## Documentation Reference

Full docs: https://docs.rewst.help/

| Topic | Path |
|-------|------|
| Jinja Filters | `/documentation/jinja/list-of-jinja-filters` |
| Jinja Essentials | `/documentation/jinja/jinja-essentials` |
| Workflows | `/documentation/automations/workflows/` |
| Data Aliases | `/documentation/automations/workflows/data-aliases` |
| Task Transitions | `/documentation/automations/workflows/task-transitions` |
| Input/Output Variables | `/documentation/automations/workflows/data-input-and-output-input-variables-and-context-variables` |
| Advanced Workflow Ops | `/documentation/automations/workflows/advanced-workflow-operations-menu` |
| Triggers | `/documentation/automations/intro-to-triggers/` |
| Core Actions | `/documentation/automations/actions-in-rewst/core-actions` |
| Forms | `/documentation/automations/forms/` |
| Option Generators | `/documentation/automations/workflows/option-generator-workflows` |
| Org Variables | `/documentation/configuration/organization-variables` |
| Troubleshooting | `/documentation/automations/workflows/troubleshoot-workflow-executions-and-task-results` |

Training: https://learn.rewst.io (Cluck University)
