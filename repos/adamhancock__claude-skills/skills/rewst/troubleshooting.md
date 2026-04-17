# Rewst Troubleshooting Guide

Complete reference for debugging workflows, interpreting errors, and resolving issues.

---

## Four-Step Troubleshooting Process

When a workflow fails, follow this sequence:

### Step 1: Where did it run?
- Check **Automations > Results** (org-level: all workflows for one org)
- Or open the workflow and check its execution history (workflow-level: one workflow across all orgs)
- Both paths lead to the **Workflow Execution Summary**

### Step 2: What data came in?
- Open the **Inputs** section in the Execution Summary
- Confirm trigger values, form data, or parent workflow data were passed correctly
- Look for missing fields, wrong casing, or null values

### Step 3: What was created in context?
- Open the **Context** section — lists all CTX variables in chronological order
- Look for expected variables — are they present? Correct type? Correct value?
- Common issues: typos, casing mismatches, null where a value was expected

### Step 4: What failed and why?
- Click into the failed task
- Check the **request** (what Rewst sent) and the **response** (what came back)
- Look at Jinja, transition criteria, integration auth, API details

---

## Common Task Failures

| Failure | Meaning | Resolution |
|---------|---------|------------|
| `Jinja error` | Bad syntax, missing data, or invalid filter | Use Live Editor to test the expression with real context |
| `Integration not authorized` | 401/403 — not connected or lacks permissions | Recheck integration settings; reauthorize in Rewst |
| `Request rejected by API` | 404/405/4xx/5xx — bad endpoint, method, or ID | Check endpoint path, HTTP method, resource IDs against integration docs |
| `200 OK, but no result` | Request accepted but returned no useful data | Check query parameters; API may not return data for this operation |
| `Transition criteria not met` | Task skipped — condition not met or logic error | Review transition conditions; check upstream tasks for silent failures |

---

## Rewst Debugging Tools

### Live Editor
**Use when:** Troubleshooting a Jinja error, checking if a CTX variable exists, previewing loops/filters before adding to workflow.

Tests Jinja expressions against **real context data** from an execution without re-running. Safe — no side effects.

### Test Button (Workflow Editor)
**Use when:** Building or editing a workflow, want fast feedback while adjusting tasks.

Runs a true execution from inside the workflow builder.

### Re-run Button
**Use when:** You've fixed something and want to confirm end-to-end, or validating consistent inputs.

Re-runs with the same original inputs.

> **Warning:** Before re-running, temporarily comment out tasks that make changes, use a test org, or add conditional logic to skip sensitive steps.

---

## Variable Path Debugging

The most common source of errors. Always confirm which access method you're using:

```
TASKS.action_name.result.result.data    ← Raw access (no Publish Result As)
CTX.var_name.data.value                 ← After Publish Result As: var_name
CTX.alias_name                          ← After data alias on transition
RESULT.result                           ← In current transition (current task output)
```

### Common Path Mistakes

| Mistake | Correct |
|---------|---------|
| `CTX.published.result.result.data` | `CTX.published.data.value` |
| `{{ CTX.action_name }}` (using action name as CTX key) | Set a data alias on transition, or use `TASKS.action_name.result.result.data` |
| `CTX.list_item.property` inside With Items | `item().property` |
| `TASKS.action.data` | `TASKS.action.result.result.data` |

### Debug Expressions

```jinja
{# Check what's in a published result #}
{{ CTX.my_result | tojson }}

{# Check TASKS directly #}
{{ TASKS.my_action.result | tojson }}

{# Safe access to detect missing value #}
{{ CTX.my_var | d("MISSING") }}
```

---

## Jinja Error Checklist

- [ ] Using Rewst-specific filters? (`as_datetime` not `strptime`, `as_timezone` not `astimezone`)
- [ ] Using `| d` for potentially undefined variables?
- [ ] Lists end with `| list` after `selectattr`/`map`?
- [ ] Using `item()` inside With Items (not `CTX.item`)?
- [ ] Correct quotes (double `"` inside `{{ }}`)?
- [ ] Using `namespace` for loop variable mutation?

### Common Jinja Test Patterns

```jinja
{# Test filter chain #}
{{ CTX.users | selectattr("accountEnabled", "eq", true) | list | length }}

{# Test comprehension #}
{{ [u.displayName for u in CTX.users if u.accountEnabled] }}

{# Test datetime #}
{{ "2025-01-15" | as_datetime | format_datetime("%B %d, %Y") }}
```

---

## Workflow Not Triggering

- [ ] Trigger enabled? (toggle in trigger config)
- [ ] Correct organizations selected in trigger?
- [ ] For webhook: URL includes org ID?
- [ ] For cron: correct expression? (test at crontab.guru)
- [ ] For PSA ticket: correct board/type/status conditions?

---

## With Items Issues

| Symptom | Fix |
|---------|-----|
| `item` is undefined | Use `item()` — it's a function call, not a variable |
| Only first item processed | Check concurrency; verify list isn't sliced |
| Timeout | Reduce concurrency to ≤ 10; optimize per-item action |
| Task name conflict | Make With Items task name unique within the workflow |

---

## Integration Auth Failures

1. **401 Unauthorized** — Token expired or invalid → Reauthorize in Rewst
2. **403 Forbidden** — Missing permissions → Add required scopes in the external platform, then reauthorize
3. **After changing permissions in external tool** — Always reauthorize the Rewst integration to apply updates

---

## Transition Criteria Failures

Common causes:
- Custom condition references a CTX variable that wasn't set yet
- **Follow First** mode — earlier transition matched first, skipping others
- Task sensitivity setting requires more parent tasks to complete than are done
- Silent upstream failure that caused a variable to be empty

```jinja
{# Custom conditions — use SUCCEEDED to gate on task success #}
{{ SUCCEEDED and CTX.users | length > 0 }}

{# Check specific result value #}
{{ SUCCEEDED and TASKS.my_task.result.result.data.status == "active" }}
```

---

## Subworkflow Data Issues

- CTX variables do NOT automatically pass into or out of subworkflows
- Must explicitly define: **Input Configuration** (data in) and **Output Configuration** (data out)
- Access subworkflow output in parent: `{{ CTX.published_subworkflow_name.output_key }}`

---

## Option Generator Issues

- Output variable MUST be named exactly `options`
- Must return a list with `label` and `value` keys
- If form field shows no options: check workflow executed, check `CTX.options` in context, verify label/value field mapping in form editor

---

## Error Message Reference

| Error Pattern | Cause | Fix |
|--------------|-------|-----|
| `UndefinedError: ... has no attribute` | Accessing attribute on None or wrong path | Add `\| d({})` before attribute access |
| `TypeError: ...` | Wrong data type for operation | Check type with `\| tojson` in Live Editor |
| `400 Bad Request` | Invalid request payload | Check required fields, data types in payload |
| `401 Unauthorized` | Auth token issue | Reauthorize integration |
| `403 Forbidden` | Permission denied | Check scopes, reauthorize |
| `404 Not Found` | Resource doesn't exist or wrong ID | Verify IDs and endpoint |
| `429 Too Many Requests` | Rate limited | Reduce concurrency, add delay |
| `500 Internal Server Error` | Server-side issue | Retry; check external system status |

---

## Escalating to Rewst Support

Include in your support ticket:
1. Org name and workflow name
2. Link to the Workflow Execution Summary
3. Screenshots of the task result (request + response)
4. What you've already tried

Support: https://docs.rewst.help/support-and-community/roc-support/
Training: https://learn.rewst.io/troubleshooting-in-rewst
