# Rewst API Patterns

Complete patterns for API calls, error handling, retries, and webhooks.

---

## HTTP Request Basics

### Action Configuration

| Parameter | Description | Example |
|-----------|-------------|---------|
| URL | Full endpoint URL | `https://api.example.com/v1/users` |
| Request Method | HTTP method | GET, POST, PUT, PATCH, DELETE |
| Body/JSON | Request payload | `{"name": "{{ CTX.name }}"}` |
| Headers | Custom headers | `{"Authorization": "Bearer {{ ORG.VARIABLES.token }}"}` |
| Timeout | Max wait seconds | 30 (default 5 is often too short) |
| Require Success Status | Fail on non-2xx | Usually enabled |

### URL Building

```jinja
{# Static URL with variable #}
https://graph.microsoft.com/v1.0/users/{{ CTX.user_id }}

{# Dynamic base URL from org variable #}
{{ ORG.VARIABLES.api_base_url }}/v1/{{ CTX.endpoint }}

{# With query parameters #}
https://api.example.com/users?limit={{ CTX.limit }}&offset={{ CTX.offset }}

{# URL encode parameters #}
https://api.example.com/search?q={{ CTX.search_term | urlencode }}
```

---

## Request Body Patterns

### JSON Body

```jinja
{
    "name": "{{ CTX.name | escape }}",
    "email": "{{ CTX.email | lower }}",
    "enabled": {{ CTX.enabled | d(true) | lower }},
    "tags": {{ CTX.tags | to_json_string }},
    "metadata": {
        "source": "rewst",
        "timestamp": "{{ now() | format_datetime('%Y-%m-%dT%H:%M:%SZ') }}"
    }
}
```

### Conditional Fields

```jinja
{
    "required_field": "{{ CTX.required }}",
    {% if CTX.optional_field %}
    "optional_field": "{{ CTX.optional_field }}",
    {% endif %}
    "always_last": "value"
}

{# Better approach - build dict then serialize #}
{% set payload = {"required": CTX.required} %}
{% if CTX.optional %}
    {% set payload = payload | combine({"optional": CTX.optional}) %}
{% endif %}
{{ payload | to_json_string }}
```

### Form Data (x-www-form-urlencoded)

```jinja
grant_type=client_credentials&client_id={{ CTX.client_id }}&client_secret={{ ORG.VARIABLES.secret | urlencode }}&scope={{ CTX.scope | urlencode }}
```

---

## Response Handling

### Standard Response Structure

```jinja
{# Access response parts #}
{{ TASKS.api_call.result.result.status_code }}
{{ TASKS.api_call.result.result.headers }}
{{ TASKS.api_call.result.result.data }}

{# Common data access #}
{{ TASKS.api_call.result.result.data.value }}           {# M365 list responses #}
{{ TASKS.api_call.result.result.data.items }}           {# Some APIs #}
{{ TASKS.api_call.result.result.data }}                 {# Direct object #}
```

### Status Code Handling

```jinja
{% set response = TASKS.api_call.result.result %}
{% set status = response.status_code %}

{% if status == 200 or status == 201 %}
    {# Success - process data #}
{% elif status == 204 %}
    {# Success - no content (common for DELETE) #}
{% elif status == 400 %}
    {# Bad request - check payload format #}
{% elif status == 401 %}
    {# Unauthorized - token expired/invalid #}
{% elif status == 403 %}
    {# Forbidden - insufficient permissions #}
{% elif status == 404 %}
    {# Not found - resource doesn't exist #}
{% elif status == 409 %}
    {# Conflict - resource already exists #}
{% elif status == 429 %}
    {# Rate limited - need to retry later #}
{% elif status >= 500 %}
    {# Server error - retry may help #}
{% endif %}
```

### Error Message Extraction

```jinja
{# Different APIs format errors differently #}

{# Microsoft Graph #}
{{ TASKS.api_call.result.result.data.error.message }}

{# Generic pattern #}
{{ TASKS.api_call.result.result.data.error | d(TASKS.api_call.result.result.data.message | d("Unknown error")) }}

{# ConnectWise #}
{{ TASKS.api_call.result.result.data.message }}

{# Detailed error with code #}
{% set err = TASKS.api_call.result.result.data.error | d({}) %}
Error {{ err.code | d("UNKNOWN") }}: {{ err.message | d("No message") }}
```

---

## Authentication Patterns

### Bearer Token

```jinja
{# Header #}
{
    "Authorization": "Bearer {{ ORG.VARIABLES.api_token }}"
}
```

### API Key

```jinja
{# In header #}
{
    "X-API-Key": "{{ ORG.VARIABLES.api_key }}"
}

{# In query string #}
{{ base_url }}?api_key={{ ORG.VARIABLES.api_key }}
```

### Basic Auth

```jinja
{# Base64 encoded username:password #}
{
    "Authorization": "Basic {{ (CTX.username ~ ':' ~ ORG.VARIABLES.password) | base64 }}"
}
```

### OAuth Token Refresh

```jinja
{# Check if token needs refresh #}
{% set expiry = ORG.VARIABLES.token_expiry | d("2000-01-01") | as_datetime %}
{% set buffer = 300 %}  {# 5 minute buffer #}
{% if (expiry - now()).total_seconds() < buffer %}
    {# Token expired or expiring - refresh needed #}
{% endif %}

{# Refresh token request #}
POST {{ ORG.VARIABLES.token_endpoint }}
Content-Type: application/x-www-form-urlencoded

grant_type=refresh_token
&refresh_token={{ ORG.VARIABLES.refresh_token }}
&client_id={{ ORG.VARIABLES.client_id }}
&client_secret={{ ORG.VARIABLES.client_secret }}
```

---

## Pagination Patterns

### Cursor-Based (Graph API style)

```jinja
{# Initial request #}
GET {{ endpoint }}?$top=999

{# Subsequent requests follow nextLink #}
{% set next_link = TASKS.list.result.result.data["@odata.nextLink"] %}
{% if next_link %}
    {# Call subworkflow with next_link as URL #}
{% endif %}

{# Accumulate results pattern #}
{% set all_results = CTX.accumulated | d([]) + TASKS.list.result.result.data.value %}
```

### Page Number Based

```jinja
{# Calculate total pages #}
{% set total = TASKS.list.result.result.data.total %}
{% set page_size = 100 %}
{% set pages = ((total / page_size) | round(0, "ceil")) | int %}

{# Fetch all pages with With Items #}
With Items: {{ range(1, pages + 1) | list }}

{# In task: #}
GET {{ endpoint }}?page={{ item() }}&pageSize={{ page_size }}
```

### Offset-Based

```jinja
{# Fetch in batches #}
{% set batch_size = 100 %}
{% set offset = CTX.current_offset | d(0) %}

GET {{ endpoint }}?limit={{ batch_size }}&offset={{ offset }}

{# Continue if more data #}
{% set results = TASKS.fetch.result.result.data %}
{% if results | length == batch_size %}
    {# More pages - continue with offset + batch_size #}
{% endif %}
```

---

## Retry Patterns

### Simple Retry with Delay

```
┌─────────────────────────────────────┐
│ Make API Call                       │
├──────────────┬──────────────────────┤
│              │                      │
│ Success      │ Failure              │
│ ↓            │ ↓                    │
│ Continue     │ Increment Counter    │
│              │ ↓                    │
│              │ Counter < Max?       │
│              │ ├─ Yes → Delay       │
│              │ │        → Retry     │
│              │ └─ No  → Fail        │
└──────────────┴──────────────────────┘
```

### Exponential Backoff

```jinja
{# Calculate delay based on attempt number #}
{% set attempt = CTX.retry_count | d(0) %}
{% set base_delay = 2 %}  {# seconds #}
{% set max_delay = 60 %}  {# cap at 60 seconds #}
{% set delay = [base_delay ** attempt, max_delay] | min %}

{# Use Delay action with calculated seconds #}
```

### Retry Only on Specific Errors

```jinja
{# Transition condition #}
{{
    FAILED and
    TASKS.api_call.result.result.status_code in [429, 500, 502, 503, 504] and
    CTX.retry_count | d(0) < 3
}}

{# Don't retry client errors (4xx except 429) #}
{{
    FAILED and
    TASKS.api_call.result.result.status_code >= 400 and
    TASKS.api_call.result.result.status_code < 500 and
    TASKS.api_call.result.result.status_code != 429
}}
{# → Go to error handling, not retry #}
```

### Rate Limit Handling

```jinja
{# Check for rate limit and extract retry-after #}
{% set status = TASKS.api_call.result.result.status_code %}
{% if status == 429 %}
    {% set retry_after = TASKS.api_call.result.result.headers.get("Retry-After", "60") | int %}
    {# Delay for retry_after seconds, then retry #}
{% endif %}
```

---

## Webhook Patterns

### Creating Callback Webhook

```jinja
{# 1. Create webhook for callback #}
{# Rewst Action: Create Webhook #}

{# 2. Use webhook URL in external request #}
{
    "callback_url": "{{ CTX.webhook_url }}",
    "data": {{ CTX.request_data | to_json_string }}
}

{# 3. Await webhook response #}
{# Rewst Action: Await Webhook #}
{# Continues when callback received #}
```

### Webhook Request Validation

```jinja
{# Validate webhook signature (HMAC) #}
{% set expected_sig = CTX.body | to_json_string | hmac("sha256", ORG.VARIABLES.webhook_secret) %}
{% set received_sig = CTX.headers.get("X-Signature", "") %}

{% if expected_sig != received_sig %}
    {# Invalid signature - reject request #}
{% endif %}

{# Validate timestamp (prevent replay attacks) #}
{% set timestamp = CTX.body.timestamp | as_datetime %}
{% set age = (now() - timestamp).total_seconds() %}
{% if age > 300 %}  {# 5 minutes #}
    {# Request too old - reject #}
{% endif %}
```

### Webhook Response Configuration

```jinja
{# Immediate response (don't wait for workflow) #}
Response Status: 200
Response Body: {"status": "received", "id": "{{ CTX.request_id }}"}

{# Wait for results (requires secret key) #}
Wait For Results: true
Response Body: {{ CTX.final_result | to_json_string }}
```

---

## Batch API Calls

### Microsoft Graph Batching

```jinja
{# Batch up to 20 requests #}
POST /$batch

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

{# Process responses #}
{% set responses = TASKS.batch.result.result.data.responses %}
{% for resp in responses | sort(attribute="id") %}
    {% if resp.status == 200 %}
        {# Success: resp.body #}
    {% else %}
        {# Error: resp.body.error.message #}
    {% endif %}
{% endfor %}
```

### Parallel Requests with With Items

```jinja
{# With Items for parallel execution #}
With Items: {{ CTX.endpoints }}
Concurrency: 10

{# Each iteration makes independent request #}
GET {{ item().url }}

{# Collect all results after loop #}
{{ TASKS.parallel_fetch.result.result | map(attribute="data") | list }}
```

---

## API-Specific Patterns

### Microsoft Graph

```jinja
{# Common headers #}
{
    "Authorization": "Bearer {{ CTX.access_token }}",
    "Content-Type": "application/json",
    "ConsistencyLevel": "eventual"  {# Required for $count, advanced filters #}
}

{# Handle throttling #}
{% if TASKS.graph.result.result.status_code == 429 %}
    {% set retry = TASKS.graph.result.result.headers.get("Retry-After", "30") %}
    {# Wait {{ retry }} seconds #}
{% endif %}
```

### ConnectWise Manage

```jinja
{# Headers #}
{
    "Authorization": "Basic {{ (ORG.VARIABLES.cw_company ~ '+' ~ ORG.VARIABLES.cw_public ~ ':' ~ ORG.VARIABLES.cw_private) | base64 }}",
    "clientId": "{{ ORG.VARIABLES.cw_client_id }}",
    "Content-Type": "application/json"
}

{# Pagination via Link header #}
{% set link = TASKS.cw.result.result.headers.get("Link", "") %}
{# Parse for next page URL #}
```

### REST API with API Key

```jinja
{# In header #}
{
    "X-API-Key": "{{ ORG.VARIABLES.api_key }}",
    "Content-Type": "application/json"
}

{# In URL (less secure, avoid if possible) #}
{{ endpoint }}?apiKey={{ ORG.VARIABLES.api_key | urlencode }}
```

---

## Error Recovery Patterns

### Graceful Degradation

```jinja
{# Try primary, fall back to secondary #}
{% if TASKS.primary_api.result.result.status_code == 200 %}
    {% set data = TASKS.primary_api.result.result.data %}
{% else %}
    {% set data = TASKS.fallback_api.result.result.data | d({"default": "values"}) %}
{% endif %}
```

### Partial Success Handling

```jinja
{# When batch has mixed results #}
{% set successes = [] %}
{% set failures = [] %}

{% for result in CTX.batch_results %}
    {% if result.status == "success" %}
        {% set _ = successes.append(result) %}
    {% else %}
        {% set _ = failures.append(result) %}
    {% endif %}
{% endfor %}

{# Continue with successes, report failures #}
```

### Circuit Breaker Pattern

```jinja
{# Track failures in org variable #}
{% set failure_count = ORG.VARIABLES.api_failures | d(0) %}
{% set last_failure = ORG.VARIABLES.api_last_failure | d("2000-01-01") | as_datetime %}
{% set circuit_open = failure_count >= 5 and (now() - last_failure).total_seconds() < 300 %}

{% if circuit_open %}
    {# Don't attempt - circuit is open #}
    {# Use cached data or fail fast #}
{% else %}
    {# Attempt API call #}
    {# On failure: increment counter, update timestamp #}
    {# On success: reset counter #}
{% endif %}
```

---

## Debugging API Calls

### Logging Request Details

```jinja
{# Debug action before API call #}
{
    "url": "{{ full_url }}",
    "method": "{{ method }}",
    "headers": {{ headers | to_json_string }},
    "body": {{ body | to_json_string }}
}
```

### Capturing Full Response

```jinja
{# Data alias on transition #}
{
    "status_code": {{ TASKS.api.result.result.status_code }},
    "headers": {{ TASKS.api.result.result.headers | to_json_string }},
    "data": {{ TASKS.api.result.result.data | to_json_string }},
    "success": {{ TASKS.api.result.result.status_code < 400 }}
}
```

### Common API Issues Checklist

```markdown
□ URL correct (no typos, proper encoding)
□ HTTP method correct (GET vs POST)
□ Headers include Content-Type
□ Auth header/token present and valid
□ Body is valid JSON (check escaping)
□ Timeout sufficient for operation
□ Handling pagination if needed
□ Checking status code before using data
```
