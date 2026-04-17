# Rewst Decision Guide

Expert decision trees and selection criteria for common scenarios.

---

## "Which Filter Should I Use?"

### For Lists

```
Need to...
├── Filter items by attribute → selectattr()
│   └── "Only enabled users" → | selectattr("enabled") | list
├── Remove items by attribute → rejectattr()
│   └── "All except admins" → | rejectattr("role", "eq", "admin") | list
├── Extract single attribute → map()
│   └── "Get all emails" → | map(attribute="email") | list
├── Get first/last item → first / last
├── Sort items → sort()
│   └── "By name" → | sort(attribute="name")
├── Remove duplicates → unique()
├── Flatten nested lists → flatten
├── Combine two lists → zip()
└── Count items → length
```

### For Strings

```
Need to...
├── Case conversion → lower / upper / title / capitalize
├── Remove whitespace → trim
├── Replace text → replace or regex_replace
├── Split to list → split("delimiter")
├── Find pattern → regex_search / regex_match
├── Extract value → regex_substring
├── Truncate → truncate(length)
└── Concatenate → use ~ operator
```

### For Dates

```
Need to...
├── Parse date string → as_datetime (NOT strptime!)
├── Parse custom format → load_datetime("%format")
├── Convert timezone → as_timezone("US/Eastern")
├── Add/subtract time → datedelta(days=7)
├── Format for display → format_datetime("%Y-%m-%d")
├── Get current time → now()
├── Compare dates → parse both with as_datetime first
└── Epoch to datetime → convert_from_epoch
```

### For Dictionaries

```
Need to...
├── Merge two dicts → combine()
├── Get keys → .keys() | list
├── Get values → .values() | list
├── Iterate pairs → | items
├── Safe access → | d({}) before accessing
└── Create from list → {item.key: item.val for item in list}
```

---

## "CTX or TASKS?"

```
Where is the data?
│
├── Just created by a task? → TASKS.action_name.result.result.data
│   └── Not yet processed, raw API response
│
├── Processed and stored? → CTX.variable_name
│   └── Stored via data alias on transition
│
├── Passed into workflow? → CTX.input_name
│   └── From trigger, form, or parent workflow
│
├── Organization setting? → ORG.VARIABLES.variable_name
│   └── Configured per organization
│
└── From With Items loop? → item()
    └── Current iteration item
```

---

## "Which Trigger Type?"

```
What initiates it?
│
├── User request → Form Submission
│   └── Need approval/input from human
│
├── Scheduled → Cron Job
│   └── "0 8 * * 1" = Mondays at 8 AM
│
├── External event → Webhook
│   └── PSA, monitoring, external system
│
├── Ticket event → PSA Ticket Trigger
│   └── Created, updated, status changed
│
├── After another workflow → Completion Handler
│   └── Post-processing, notifications
│
├── Testing/Option Generator → Always Pass
│   └── No conditions needed
│
└── Regular interval → Time Interval
    └── Every X minutes/hours
```

---

## "How Should I Loop?"

```
Iteration need?
│
├── Process each item in list → With Items
│   ├── Set With Items field: {{ CTX.my_list }}
│   ├── Access current: {{ item() }}
│   └── Concurrency: max 10
│
├── Transform list in Jinja → List Comprehension
│   └── {{ [x.name for x in CTX.items if x.active] }}
│
├── Accumulate values → Namespace
│   └── {% set ns = namespace(total=0) %}
│
├── Multiple lists together → zip()
│   └── {{ for a, b in list1|zip(list2) }}
│
└── Track index → enumerate()
    └── {{ for idx, item in CTX.items|enumerate }}
```

---

## "How Should I Handle Errors?"

```
Error type?
│
├── Variable might not exist
│   └── Use | d("default") or | d({}) or | d([])
│
├── API might fail
│   ├── Add On Failure transition
│   ├── Check status_code
│   └── Build retry logic
│
├── Complex expression might error
│   └── Use {% try %}...{% catch %}...{% endtry %}
│
├── Need graceful degradation
│   └── Set fallback values in On Failure transition
│
└── Critical failure needs alerting
    ├── On Failure → Create ticket
    └── On Failure → Send notification
```

---

## "Follow All or Follow First?"

```
Transition behavior needed?
│
├── Multiple paths can run simultaneously
│   └── Follow All (blue arrows)
│   └── Example: Send email AND create ticket
│
└── Only one path should run (decision)
    └── Follow First (orange arrows)
    └── Example: IF approved → process ELSE → reject

Note: Follow First evaluates LEFT to RIGHT
Put most specific conditions on the left
```

---

## "Subworkflow or Data Alias?"

```
What are you doing?
│
├── Transforming data in place
│   └── Data Alias on transition
│   └── {{ [u.email for u in TASKS.list.result.result.data.value] }}
│
├── Reusable logic across workflows
│   └── Subworkflow
│
├── Complex multi-step processing
│   └── Subworkflow (easier to debug)
│
├── Running in different org context
│   └── Subworkflow with Run as Org
│
└── Option generation
    └── Subworkflow (Option Generator type)
```

---

## "API Field Selection"

### Microsoft Graph

```
Need to...
├── Reduce payload size → $select=field1,field2
├── Filter server-side → $filter=field eq 'value'
├── Include related data → $expand=relationship
├── Limit results → $top=100
├── Page through all → follow @odata.nextLink
└── Get only changes → delta queries + @odata.deltaLink
```

### ConnectWise Manage

```
Need to...
├── Filter results → conditions=field="value"
├── Limit results → pageSize=100
├── Select fields → columns=field1,field2
├── Page through → page=1, page=2, etc.
└── Sort results → orderBy=field asc/desc
```

---

## "Form Field Type Selection"

```
User needs to...
│
├── Pick one from many options
│   ├── < 5 options → Radio Buttons (all visible)
│   └── ≥ 5 options → Dropdown
│
├── Pick multiple options → Multi-Select
│
├── Yes/No decision → Checkbox
│
├── Enter short text → Text Input
│
├── Enter long text → Multi-Line Input
│
├── Enter a number → Number Input
│
├── Pick a date → Date Picker
│
├── Upload a file → File Upload
│
└── See information only → Text/Markdown
```

---

## "When to Use Options Filter vs. Option Generator"

```
Need to...
│
├── Filter existing options dynamically
│   └── Options Filter on the field
│   └── Based on form values or org variables
│
├── Generate options from API data
│   └── Option Generator workflow
│   └── Outputs { label, value } pairs
│
├── Combine both
│   └── Option Generator for base data
│   └── Options Filter for additional filtering
│
└── Static options
    └── Define directly in form field
```

---

## "Integration Authorization Issues"

```
Error type?
│
├── 401 Unauthorized
│   ├── Token expired → Reauthorize integration
│   ├── API key invalid → Check/regenerate key
│   └── Wrong credentials → Verify settings
│
├── 403 Forbidden
│   ├── Missing permissions → Add API scopes
│   ├── Need admin consent → Grant in Azure/provider
│   └── IP restrictions → Whitelist Rewst IPs
│
└── Integration not found
    └── Enable integration for this organization
```

---

## "Optimizing Slow Workflows"

```
Bottleneck is...
│
├── Too many API calls
│   ├── Batch where possible
│   ├── Use $select to reduce payload
│   ├── Filter at API level
│   └── Cache frequently used data
│
├── With Items too slow
│   ├── Reduce concurrency if rate limited
│   ├── Increase concurrency if not (max 10)
│   └── Batch items into groups
│
├── Large data processing
│   ├── Paginate through results
│   ├── Process in chunks
│   └── Filter before processing
│
├── Many sequential tasks
│   ├── Parallelize independent tasks
│   └── Combine where logical
│
└── External service slow
    ├── Increase timeout
    └── Add async pattern (webhook callback)
```

---

## "Multi-Tenant Patterns"

```
Running for multiple clients?
│
├── Same action for all orgs
│   ├── List Organizations (GraphQL)
│   ├── With Items: org list
│   └── Run as Org: {{ item().id }}
│
├── Different behavior per org
│   ├── Use ORG.VARIABLES for config
│   └── Conditional logic based on vars
│
├── Aggregate results from all orgs
│   ├── Collect in With Items
│   └── Combine after loop
│
└── Parent org processing for children
    └── Always set Run as Org explicitly
```

---

## "When to Escalate"

```
Issue persists after...
│
├── Checking all variables/paths
├── Testing in Live Editor
├── Reviewing execution details
├── Verifying integration auth
├── Checking API directly
│
└── Then escalate with:
    ├── Organization name
    ├── Workflow name
    ├── Execution link
    ├── Error details/screenshots
    └── What you've already tried
```
