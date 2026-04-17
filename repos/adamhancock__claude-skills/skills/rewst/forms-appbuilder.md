# Rewst Forms & App Builder

Complete reference for forms, dynamic fields, and App Builder components.

---

## Form Builder Overview

Access: **Automations > Assets > Forms**

Forms collect user input that triggers workflows. Each form field becomes a workflow input variable accessible via `{{ CTX.field_name }}`.

---

## Form Field Types

| Field Type | Use Case | Notes |
|-----------|----------|-------|
| **Text/Markdown** | Display text to users | Can render Jinja and org variables; not an input |
| **Drop-down** | Single selection | Supports dynamic options from integration or option generator |
| **Multi-Select** | Multiple selections | Returns list; access with `{{ CTX.field[0].value }}` |
| **Checkbox** | Boolean toggle | Returns true/false |
| **Radio Buttons** | Single choice from visible options | Rarely used with dynamic options |
| **Text Input** | Single-line text | Supports regex validation and default values |
| **Number Input** | Numeric input | Supports min/max/default |
| **Multi-Line Input** | Large text areas | No validation |
| **Date** | Date/time picker | Time zone based on browser locale; converted to UTC in workflow |
| **File Upload** | Upload files | Supports .CSV and .JSON only (convert XLSX to CSV first) |

---

## Dynamic Options

Two ways to populate dropdown/multi-select options dynamically:

### Option 1: Integration Reference

Pulls directly from a predefined integration action (e.g., Microsoft Graph list users).

- Simple to configure — no workflow needed
- **Only works at the top/parent organization level** — cannot pull child org data
- No filtering or transformation available

### Option 2: Workflow Generated (Option Generator)

Uses an option generator workflow for full control over data and filtering.

- Supports Jinja for complex data manipulation
- Works for child organizations (pass org context via Run as Org)
- Supports default pre-selected items

**Required format — the workflow must output a list named `options`:**

```jinja
{{
    [
        {"label": user.displayName, "value": user.id}
        for user in CTX.users
        if user.accountEnabled
    ] | sort(attribute="label")
}}
```

**With default pre-selection:**

```jinja
{{
    [
        {
            "label": user.displayName,
            "value": user.id,
            "is_default": user.id == CTX.current_user_id
        }
        for user in CTX.users
    ]
}}
```

In form builder, set:
- **Value Field**: `id` (or whatever key holds the value to pass to workflow)
- **Label Field**: `label` (or whatever key holds display text)
- **Default Selected Field**: `is_default` (boolean attribute on each option)

### Option 3: Options Filter

Filter existing options **without modifying the option generator workflow**. Applied directly in the form builder on the field. Useful for restricting choices per organization using org variables.

---

## Drop-down Advanced Settings

| Setting | Purpose |
|---------|---------|
| **Auto-Populate** | Pre-populate if only one option returned |
| **Allow Custom Input** | Let user type a custom value not in the list |
| **Always Skip Cache** | Force re-fetch instead of using 8-hour cache |
| **Always Override Option** | Re-run option generator when a dependent field changes |

---

## Multi-Select Access Patterns

```jinja
{# Get first selected value #}
{{ CTX.field_name[0].value | d }}

{# Check if any selected #}
{{ CTX.field_name | length > 0 }}

{# Get all selected values as list #}
{{ CTX.field_name | map(attribute="value") | list }}

{# Check if specific value selected #}
{{ "some_value" in (CTX.field_name | map(attribute="value") | list) }}
```

---

## Form URLs

### Get form URL for testing

From the workflow:
1. Click the trigger on the workflow
2. Click **View Form URLs**
3. Select the desired organization

### Dynamic form links

Instead of static org-specific URLs, use **Copy URL** when sharing to get:

```
https://app.rewst.io/form/<form_trigger_guid>
```

This auto-redirects users to the correct org-specific form based on who is logged in. Always use this for sharing forms across multiple organizations.

---

## Date Fields

- Date format (DD/MM/YYYY vs MM/DD/YYYY) is controlled by **browser locale**, not Rewst
- Form dates are converted to **UTC** in the workflow
- If submitting a form for a customer in a different timezone, account for the offset

---

## Organization Variables in Forms

Use org variables in form fields for per-client customization:

```jinja
{# In a Text/Markdown field — show org-specific data #}
Welcome to {{ ORG.VARIABLES.company_name }}!

{# In a workflow input field default value #}
{{ ORG.VARIABLES.default_location }}
```

### Restricting dropdown options per org

1. Add org variable: `form_default_email_domain` = `["domain.com"]`
2. In form field settings, set `schema.enumSourceWorkflow.input.force_default` = `true`
3. Set `schema.enumSourceWorkflow.input.choose_variable` = `email_domain`

> **Important:** Set the org variable on EVERY organization that uses the form, not just one.

---

## Common Form Variables Reference

After form submission, all field values are available as `{{ CTX.field_name }}`:

```jinja
{# Text input #}
{{ CTX.first_name }}
{{ CTX.email_address }}

{# Dropdown (single value) #}
{{ CTX.selected_user }}

{# Multi-select (list of objects) #}
{{ CTX.selected_groups[0].value }}
{{ CTX.selected_groups | map(attribute="value") | list }}

{# Checkbox #}
{{ CTX.enable_mfa }}  {# true or false #}

{# Date field — arrives as string, parse with as_datetime #}
{{ CTX.start_date | as_datetime | format_datetime("%Y-%m-%d") }}

{# File upload — arrives as parsed CSV/JSON content #}
{{ CTX.uploaded_file }}
```

---

## Best Practices

- **Naming convention**: Use snake_case for field names (they become CTX variable names)
- **Dynamic forms**: Use option generators for any data that changes per org
- **Caching**: Option generators cache for 8 hours by default — use "Always Skip Cache" if data changes frequently
- **Child orgs**: Integration reference fields only work at parent org — use option generators for MSP child org data
- **Testing**: Get the form URL via the workflow trigger → View Form URLs
- **Large lists**: If option generator returns > 500 items, consider filtering with an options filter or accepting slower load times

---

## App Builder Overview

App Builder creates custom web portals/dashboards that connect to Rewst workflows.

Access: **App Builder** in the main navigation

### Key concepts

| Concept | Description |
|---------|-------------|
| **Pages** | Individual screens in your app |
| **Components** | UI building blocks dragged onto pages |
| **Authentication** | Controls who can access the app |
| **Themes** | Brand colors, fonts, styling |
| **Domains** | Custom domain configuration |

### Pre-built apps

| App | Purpose |
|-----|---------|
| **End User Portal** | Customer-facing self-service portal |
| **Forms Portal** | Centralized forms management |
| **MSP Reporting Portal** | Reporting dashboard |

### Connecting App Builder to workflows

Components in App Builder can trigger workflows and display results. The workflow connection works the same as form submission — data flows through triggers and CTX variables.
