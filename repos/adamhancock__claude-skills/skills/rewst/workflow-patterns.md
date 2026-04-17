# Rewst Workflow Patterns

Complete reference for workflow design, triggers, actions, and advanced patterns.

---

## Trigger Types

| Trigger | Use Case | Notes |
|---------|----------|-------|
| **Cron Job** | Scheduled tasks | `0 3 * * 1` = Mondays 3AM UTC |
| **Form Submission** | User requests | Links to a Rewst form |
| **Webhook** | External systems calling in | URL includes org ID |
| **Always Pass** | Testing, option generators | Runs immediately on test |
| **PSA Ticket Saved** | Ticket automation | Filters by board/type/status |

### Cron Expression Examples

```
0 3 * * 1      = Every Monday at 3:00 AM
0 */6 * * *    = Every 6 hours
0 9 * * 1-5    = Weekdays at 9:00 AM
0 0 1 * *      = First of every month at midnight
```

---

## Task Transitions

Transitions are the **small rectangles beneath each task**. They are separate from the task itself — clicking the task opens the task config; clicking the rectangle opens the transition config.

### Transition Conditions

| Condition | When it triggers |
|-----------|-----------------|
| **Success** | Task completed without error |
| **Failure** | Task encountered an error |
| **Always** | Regardless of task outcome |
| **Custom** | Jinja expression evaluated as boolean |

### Custom Transition Conditions

```jinja
{# Success AND a condition #}
{{ SUCCEEDED and CTX.user_count > 0 }}

{# Failure path #}
{{ FAILED }}

{# Check specific value from task result #}
{{ SUCCEEDED and TASKS.get_user.result.result.data.accountEnabled == true }}

{# Check CTX variable #}
{{ SUCCEEDED and CTX.approval_result == "approve" }}
```

### Transition Modes (Advanced Tab)

- **Follow All** (blue arrows) — Default. All matching transitions are followed. Use for parallel branching.
- **Follow First** (orange arrows) — Evaluates left to right, stops at first match. Use for if/else routing.

> **Tip:** Transitions are evaluated **left to right**. Order matters with Follow First.

### Data Aliases on Transitions

Add data aliases on the transition (not the task) to extract and store data:

```jinja
{# Key: enabled_users #}
{{ [u for u in TASKS.list_users.result.result.data.value if u.accountEnabled] }}

{# Key: new_user_id #}
{{ TASKS.create_user.result.result.data.id }}

{# Key: ticket_number #}
{{ TASKS.create_ticket.result.result.data.ticketNumber }}
```

---

## Advanced Task Options

These are all found in the **Advanced** tab of each task.

### With Items (foreach)

Equivalent to a `foreach` loop. Pass a list to iterate over.

```jinja
{# With Items field — the list to iterate #}
{{ CTX.all_users }}

{# Access current item in action fields #}
{{ item() }}
{{ item().firstName }}
{{ item().email | lower }}
```

**Best practices:**
- Max concurrency: **10** (to guarantee performance)
- Make the task name unique within the workflow
- Use a subworkflow for complex per-item logic
- Limit for testing: `{{ CTX.all_users[:CTX.max_items | int] }}`

**After With Items completes**, results are accessible on the TASKS object as a list.

### Run as Org

Executes the task/subworkflow in the context of a **different organization**.

- Only works **parent → child** (not child → parent)
- Use case: MSP running actions against a client org
- Common pattern: List all child orgs → With Items → Run as Org subworkflow

### Task Timeout

Default: **10 minutes**. Adjustable in the Advanced tab (in seconds).

Increase for long-running operations (large data exports, complex scripts).

### Task Transition Criteria Sensitivity

Controls how many parent tasks must complete before this task runs. `0` means all parent tasks must complete. Use when a task has multiple incoming connections.

### Shallow Clone

Clones a single resource (workflow/form) but **re-uses all dependencies** (subworkflows, forms, templates). Useful for creating variants without duplicating everything. Rewst auto-detects and shallow clones when cloning into your own org.

---

## Subworkflows

Subworkflows are standard workflows called from within another workflow.

### Passing Data In: Input Configuration

Define input variables on the subworkflow's **Input Configuration**. In the parent, set those values in the action's input fields.

### Passing Data Out: Output Configuration

Define output variables on the subworkflow's **Output Configuration**.

```jinja
{# In the subworkflow's output config, map: #}
{# Key: created_user_id, Value: #}
{{ CTX.new_user.id }}
```

### Accessing Subworkflow Output in Parent

```jinja
{# After subworkflow action with Publish Result As: onboard_result #}
{{ CTX.onboard_result.created_user_id }}
```

> **Critical:** Variables do NOT automatically pass between workflows. Always define input/output config explicitly.

---

## Core Actions Reference

### Noop (No-operation)
- **Purpose:** Placeholder, logic gate, convergence point for multiple transitions
- **Output:** None

### Debug
- **Purpose:** Log and inspect values without affecting workflow
- **Params:** `text` (any string/Jinja), `template` (template reference)
- **Output:** Returns the same inputs — visible in task result

```jinja
{# text field #}
User count: {{ CTX.users | length }}, First: {{ CTX.users | first | d({}) | tojson }}
```

### Mock
- **Purpose:** Simulate an action response for testing
- **Output:** Wraps your mock data in a `data` object
- **Note:** Jinja expressions in mock values are returned as strings, not evaluated

### HTTP Request
- **Purpose:** Ad-hoc API call to any URL
- **Methods:** GET, POST, PUT, PATCH, DELETE, HEAD, OPTIONS, etc.
- **Key params:** URL, method, headers, params, body/JSON, timeout, require_success_status
- **Output:** Response with status_code, data, headers, cookies

### Generate Password V2
- **Purpose:** Cryptographically secure password
- **Params:** length, min numeric count, min capital count, optional punctuation
- **Output:** `password` key in result

```jinja
{# Access generated password #}
{{ TASKS.generate_password.result.result.data.password }}
```

### Confirmation Email
- **Purpose:** Pause workflow, send email with clickable buttons for approval
- **Params:** to, subject, title, message, buttons (with label, style, value), render_markdown
- **Output:** `inquiry_result` = value of button clicked
- **States:** Workflow enters `Awaiting-User-Input` until button clicked or timeout

```jinja
{# Route on confirmation result in transition #}
{{ SUCCEEDED and TASKS.confirm.result.result.data.inquiry_result == "approve" }}
```

### Create Pending Task
- **Purpose:** Pause for manual human approval within Rewst UI
- **Params:** message, buttons (array of {label, style, value})
- **Output:** Value of button clicked

### Send Mail
- **Purpose:** Send email via Rewst platform
- **Params:** sender prefix, to, subject, title, message, render_markdown, Custom HTML
- **Note:** Emails sent from rewst.io domain; images must be externally referenced

### Create Webhook + Await Webhook Request
- **Purpose:** Create a temporary webhook URL, then pause workflow until it's called
- **Create output:** webhook ID and URL
- **Await params:** webhook ID
- **Await output:** HTTP method, query params, headers, body, timestamp

### Delay Workflow For Period
- **Purpose:** Pause for a set duration
- **Params:** days, hours, minutes, seconds

### Delay Workflow Until Date/Time
- **Purpose:** Pause until a specific datetime
- **Params:** target datetime

### UUID
- **Purpose:** Generate a UUID
- **Params:** type (uuid1 or uuid4, default uuid4)
- **Output:** the generated UUID string

### Parse HTML / Parse XML
- **Purpose:** Extract elements from HTML/XML using CSS selectors or XPath
- **Useful for:** Processing API responses, web scraping, data extraction from documents

---

## Common Workflow Patterns

### If/Else Routing

```
[Task A]
   ├─ (orange/Follow First) Success + condition → [Task B: Happy Path]
   └─ (orange/Follow First) Always            → [Task C: Default Path]
```

Set transition mode to **Follow First** on Task A.

### Parallel Execution

```
         [Task A]
    ┌────────┴────────┐
[Task B]          [Task C]
    └────────┬────────┘
         [Task D]
```

Use **Follow All** (default) on Task A. Task D has sensitivity set to require both B and C.

### Approval Workflow

```
[Collect Request] → [Confirmation Email] → [Check inquiry_result]
    ├── "approve" → [Process Request]
    └── "deny"   → [Send Rejection]
```

### Multi-Org Operation

```
[List Child Orgs] → [With Items: CTX.orgs]
                        └── [Subworkflow: Run as Org = item().id]
                                └── [Per-org actions]
```

### Paginated Data Collection

```
[Initial Request] → [Check @odata.nextLink]
    ├── Has more → [Fetch next page] → [Append + loop]
    └── Done     → [Process all results]
```

### Human-in-the-Loop

```
[Trigger] → [Prepare Summary] → [Confirmation Email or Pending Task]
    ├── Approved → [Execute Changes]
    └── Denied   → [Log + Notify]
```

---

## Option Generator Pattern

For populating dynamic form dropdowns:

1. **Create workflow** → Workflow Type: **Option Generator**
2. **Add Output Configuration** with key: `options` (must be exactly this name)
3. **Build workflow** that produces `CTX.options`
4. **Connect to form field** in form builder

```jinja
{# Standard options format #}
{{
    [
        {"label": user.displayName, "value": user.id}
        for user in CTX.users
        if user.accountEnabled
    ] | sort(attribute="label")
}}
```

---

## Workflow Type Reference

| Type | Purpose | Key Requirement |
|------|---------|----------------|
| Standard | General automation | n/a |
| Option Generator | Dynamic form dropdowns | Output variable named `options` |
| Sub-workflow | Called from other workflows | Input/Output config for data passing |
