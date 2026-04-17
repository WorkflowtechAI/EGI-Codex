# Rewst Performance Optimization

Advanced patterns for building fast, efficient, and scalable workflows.

---

## API Optimization

### Request Specific Fields Only

```jinja
{# BAD - fetches entire user object #}
GET /users

{# GOOD - only get what you need #}
GET /users?$select=id,displayName,mail,accountEnabled

{# Savings can be 10-50x smaller payloads #}
```

### Filter at API Level

```jinja
{# BAD - fetch all, filter in Jinja #}
{{ TASKS.list_users.result.result.data.value | selectattr("accountEnabled") | list }}

{# GOOD - filter at API #}
GET /users?$filter=accountEnabled eq true

{# Fetch 100 users vs 10,000 = huge time savings #}
```

### Batch API Calls

```jinja
{# Microsoft Graph Batch API #}
POST /$batch
Content-Type: application/json

{
    "requests": [
        {% for user_id in CTX.user_ids[:20] %}
        {
            "id": "{{ loop.index }}",
            "method": "GET",
            "url": "/users/{{ user_id }}?$select=id,displayName,mail"
        }{{ "," if not loop.last }}
        {% endfor %}
    ]
}

{# Process batch response #}
{% for response in TASKS.batch_request.result.result.data.responses %}
    {% if response.status == 200 %}
        {{ response.body }}
    {% endif %}
{% endfor %}
```

---

## Pagination Patterns

### Cursor-Based (Graph API)

```jinja
{# Initial request #}
GET /users?$top=999

{# Check for next page #}
{% set next_link = TASKS.list_users.result.result.data["@odata.nextLink"] %}

{# Pattern: Use subworkflow with loop #}
{
    "all_results": {{ CTX.accumulated_results | d([]) + current_page_results }},
    "next_link": "{{ next_link | d("") }}"
}

{# Continue until no next link #}
{% if next_link %}
    {# Call subworkflow again with next_link #}
{% endif %}
```

### Page-Based Pagination

```jinja
{# ConnectWise Manage style #}
page: {{ CTX.current_page | d(1) }}
pageSize: 1000

{# Check total from response headers or body #}
{% set total_count = TASKS.list_items.result.result.headers["X-Total-Count"] | int %}
{% set page_size = 1000 %}
{% set total_pages = (total_count / page_size) | round(0, "ceil") | int %}

{# Loop through pages with With Items #}
{{ range(1, total_pages + 1) | list }}
```

### Efficient Pagination Workflow

```
1. Get first page + total count
2. Calculate pages needed
3. With Items: Fetch all pages (concurrency 5)
4. Flatten results

{# Much faster than sequential page fetching #}
```

---

## With Items Optimization

### Optimal Concurrency

```jinja
{# Rule of thumb by target API #}
Microsoft Graph: 10 (very resilient)
ConnectWise: 5-10 (varies by instance)
Datto RMM: 5 (rate limiting)
NinjaRMM: 3 (strict limits)
Autotask: 5

{# Always start conservative, increase if no 429s #}
```

### Batch Items Before Loop

```jinja
{# BAD - 100 iterations of 1 item each #}
With Items: {{ CTX.items }}  {# 100 items #}
Concurrency: 10

{# GOOD - 10 iterations of 10 items each #}
With Items: {{ CTX.items | batch(10) | list }}
Concurrency: 5

{# Inside task, process batch #}
{% for item in item() %}
    {# Process each item in batch #}
{% endfor %}
```

### Pre-Filter Before Loop

```jinja
{# BAD - loop through all, skip inside #}
With Items: {{ CTX.all_users }}

{# Task transition: #}
{% if item().accountEnabled %}
    {# Do work #}
{% endif %}

{# GOOD - filter before loop #}
With Items: {{ CTX.all_users | selectattr("accountEnabled") | list }}

{# All iterations are useful #}
```

### Limit for Testing

```jinja
{# During development #}
With Items: {{ CTX.items[:5] }}  {# Only first 5 #}

{# Production - remove limit #}
With Items: {{ CTX.items }}
```

---

## Data Transformation Optimization

### Use Built-in Filters

```jinja
{# BAD - manual loop #}
{% set names = [] %}
{% for user in CTX.users %}
    {% set _ = names.append(user.name) %}
{% endfor %}

{# GOOD - map filter #}
{{ CTX.users | map(attribute="name") | list }}
```

### Chain Filters Efficiently

```jinja
{# Extract, filter, and sort in one expression #}
{{
    CTX.users
    | selectattr("department", "eq", "IT")
    | selectattr("accountEnabled")
    | map(attribute="displayName")
    | sort
    | list
}}
```

### Avoid Redundant Processing

```jinja
{# BAD - processes list twice #}
{% set active = CTX.users | selectattr("active") | list %}
{% set count = CTX.users | selectattr("active") | list | length %}

{# GOOD - process once, reuse #}
{% set active = CTX.users | selectattr("active") | list %}
{% set count = active | length %}
```

---

## Caching Strategies

### Org Variable Cache

```jinja
{# Store expensive query results #}
{
    "license_skus": {
        "data": {{ CTX.skus | to_json_string }},
        "cached_at": "{{ now() | format_datetime('%Y-%m-%dT%H:%M:%SZ') }}",
        "ttl_hours": 24
    }
}

{# Check cache validity #}
{% set cache = ORG.VARIABLES.license_skus | d({}) %}
{% set cached_at = cache.cached_at | d("2000-01-01") | as_datetime %}
{% set ttl = cache.ttl_hours | d(24) %}
{% set cache_valid = (now() - cached_at).total_seconds() < (ttl * 3600) %}

{% if cache_valid and cache.data %}
    {# Use cached data #}
    {% set skus = cache.data %}
{% else %}
    {# Fetch fresh and update cache #}
{% endif %}
```

### What to Cache

| Data Type | Cache Duration | Example |
|-----------|----------------|---------|
| License SKUs | 24 hours | Microsoft license GUIDs |
| Static lookups | 7 days | Priority/status mappings |
| User lists | 1-4 hours | Depends on change frequency |
| Device lists | 1 hour | RMM device inventory |
| Never cache | - | Tickets, live statuses |

### Cache Invalidation

```jinja
{# Force cache refresh #}
{% set force_refresh = CTX.force_refresh | d(false) %}

{# On user provisioning, invalidate user cache #}
{# Set org variable to expired timestamp #}
{
    "cached_at": "2000-01-01T00:00:00Z"
}
```

---

## Workflow Architecture

### Parallel Task Execution

```
                 ┌─── Get Users ───┐
                 │                 │
Start ───────────┼─── Get Groups ──┼───── Combine ───── Continue
                 │                 │
                 └─── Get Licenses─┘

{# Use Follow All transitions for parallel tasks #}
{# Join point has sensitivity = 0 (wait for all) #}
```

### Early Exit Pattern

```jinja
{# Check conditions before heavy processing #}
{% if not CTX.users %}
    {# Exit early - no users to process #}
{% endif %}

{% if CTX.skip_processing %}
    {# Skip based on input flag #}
{% endif %}
```

### Subworkflow Isolation

```
Main Workflow
├── Get Data (can fail)
└── Subworkflow: Process Each
    └── Failure doesn't crash parent

{# Subworkflow errors are contained #}
{# Parent can handle gracefully #}
```

---

## Memory Optimization

### Avoid Large Variable Storage

```jinja
{# BAD - storing entire API response #}
{# Data Alias: api_response #}
{{ TASKS.get_data.result }}

{# GOOD - extract only needed fields #}
{# Data Alias: user_emails #}
{{ TASKS.get_data.result.result.data.value | map(attribute="mail") | list }}
```

### Process and Discard

```jinja
{# For large datasets, process in chunks #}
{# Don't accumulate entire dataset in memory #}

{# Subworkflow processes chunk, returns summary only #}
{
    "processed_count": {{ processed | length }},
    "errors": {{ errors }},
    "summary": {{ summary }}
}
{# Not the entire processed dataset #}
```

### Limit List Sizes

```jinja
{# Cap returned results #}
{{ CTX.results[:1000] }}  {# Max 1000 items #}

{# Truncate long strings #}
{{ CTX.description | truncate(5000) }}
```

---

## Timeout Management

### Set Appropriate Timeouts

```jinja
{# HTTP Request defaults to 5 seconds - often too short #}

{# For RMM script execution #}
Timeout: 120  {# 2 minutes #}

{# For large API queries #}
Timeout: 60  {# 1 minute #}

{# For quick lookups #}
Timeout: 10  {# 10 seconds #}
```

### Async Patterns for Long Operations

```
1. Start Operation
   └── Returns job/task ID immediately

2. Webhook Creates Callback URL

3. Store job ID + callback URL

4. Exit Workflow (saves context)

5. External System Calls Webhook When Done

6. Resume Processing with Results
```

---

## Monitoring & Debugging

### Execution Time Tracking

```jinja
{# Add timestamps for performance debugging #}
{% set start_time = now() %}

{# ... operations ... #}

{% set elapsed = (now() - start_time).total_seconds() %}
{# Log: Operation took {{ elapsed }} seconds #}
```

### Identify Bottlenecks

```
Common bottlenecks:
1. Pagination loops (fetch all pages)
2. With Items on large lists
3. Sequential API calls that could parallel
4. Missing API filters
5. Over-fetching data
```

### Performance Testing

```jinja
{# Test with realistic data volumes #}
{% set test_sizes = [10, 100, 1000] %}

{# For each size, measure: #}
{# - Total execution time #}
{# - API call count #}
{# - Memory usage (rough estimate) #}
```

---

## Anti-Patterns to Avoid

### N+1 Query Problem

```jinja
{# BAD - N+1 queries #}
{% for user in CTX.users %}
    {# API call for each user's groups #}
    GET /users/{{ user.id }}/memberOf
{% endfor %}

{# GOOD - batch or use $expand #}
GET /users?$expand=memberOf&$select=id,displayName
```

### Synchronous Everything

```jinja
{# BAD - wait for each #}
Get User → Wait → Get Groups → Wait → Get Licenses → Wait

{# GOOD - parallel where possible #}
Get User ─┬─ Get Groups ─┬─ Combine
          └─ Get Licenses─┘
```

### Ignoring API Capabilities

```jinja
{# BAD - client-side aggregation #}
{{ CTX.orders | sum(attribute="total") }}

{# GOOD - use API aggregation if available #}
GET /orders?$apply=aggregate(total with sum as totalSum)
```

---

## Performance Checklist

```markdown
Before deploying workflow:

□ API calls use $select to limit fields
□ API calls use $filter where applicable
□ Pagination handles all pages efficiently
□ With Items concurrency is appropriate for target API
□ Large lists are batched before With Items
□ No N+1 query patterns
□ Independent tasks run in parallel
□ Frequently used data is cached
□ Timeouts are set appropriately
□ No unnecessary data stored in variables
□ Early exits for edge cases
□ Tested with production-scale data volume
```
