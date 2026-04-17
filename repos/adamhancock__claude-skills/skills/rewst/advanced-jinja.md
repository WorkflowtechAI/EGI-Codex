# Advanced Jinja Techniques

Expert-level Jinja patterns, complex transformations, recursive operations, and edge case handling for sophisticated Rewst automations.

---

## Complex Data Transformations

### Nested Object Flattening

```jinja
{# Flatten deeply nested objects for processing #}
{% macro flatten_dict(d, parent_key='', sep='.') %}
    {% set items = [] %}
    {% for key, value in d.items() %}
        {% set new_key = parent_key ~ sep ~ key if parent_key else key %}
        {% if value is mapping %}
            {% set nested = flatten_dict(value, new_key, sep) %}
            {% for k, v in nested.items() %}
                {% set _ = items.append((k, v)) %}
            {% endfor %}
        {% elif value is iterable and value is not string %}
            {% for i, item in value | enumerate %}
                {% if item is mapping %}
                    {% set nested = flatten_dict(item, new_key ~ '[' ~ i ~ ']', sep) %}
                    {% for k, v in nested.items() %}
                        {% set _ = items.append((k, v)) %}
                    {% endfor %}
                {% else %}
                    {% set _ = items.append((new_key ~ '[' ~ i ~ ']', item)) %}
                {% endif %}
            {% endfor %}
        {% else %}
            {% set _ = items.append((new_key, value)) %}
        {% endif %}
    {% endfor %}
    {{ dict(items) }}
{% endmacro %}

{# Usage #}
{% set nested_data = {
    "user": {
        "name": "John",
        "address": {
            "street": "123 Main",
            "city": "Springfield"
        },
        "roles": ["admin", "user"]
    }
} %}
{% set flat = flatten_dict(nested_data) %}
{# Result: {"user.name": "John", "user.address.street": "123 Main", "user.address.city": "Springfield", "user.roles[0]": "admin", "user.roles[1]": "user"} #}
```

### Unflatten Objects

```jinja
{# Rebuild nested structure from flattened data #}
{% macro unflatten_dict(flat_dict, sep='.') %}
    {% set result = {} %}
    {% for key, value in flat_dict.items() %}
        {% set keys = key.split(sep) %}
        {% set current = result %}
        {% for i, k in keys[:-1] | enumerate %}
            {% if k not in current %}
                {% set _ = current.update({k: {}}) %}
            {% endif %}
            {% set current = current[k] %}
        {% endfor %}
        {% set _ = current.update({keys[-1]: value}) %}
    {% endfor %}
    {{ result }}
{% endmacro %}
```

### Deep Merge Dictionaries

```jinja
{# Recursively merge dictionaries with conflict resolution #}
{% macro deep_merge(base, override, strategy='override') %}
    {% set result = base | d({}) | tojson | from_json %}
    {% for key, value in (override | d({})).items() %}
        {% if key in result and result[key] is mapping and value is mapping %}
            {# Recursively merge nested dicts #}
            {% set _ = result.update({key: deep_merge(result[key], value, strategy)}) %}
        {% elif key in result and result[key] is iterable and result[key] is not string and value is iterable and value is not string %}
            {# Handle list merging based on strategy #}
            {% if strategy == 'append' %}
                {% set _ = result.update({key: result[key] + value}) %}
            {% elif strategy == 'unique' %}
                {% set merged = result[key] + value %}
                {% set _ = result.update({key: merged | unique | list}) %}
            {% else %}
                {% set _ = result.update({key: value}) %}
            {% endif %}
        {% else %}
            {% set _ = result.update({key: value}) %}
        {% endif %}
    {% endfor %}
    {{ result }}
{% endmacro %}

{# Usage #}
{% set base_config = {
    "settings": {"timeout": 30, "retries": 3},
    "features": ["auth", "logging"]
} %}
{% set overrides = {
    "settings": {"timeout": 60, "new_option": true},
    "features": ["caching"]
} %}
{% set merged = deep_merge(base_config, overrides, 'append') %}
{# Result: {"settings": {"timeout": 60, "retries": 3, "new_option": true}, "features": ["auth", "logging", "caching"]} #}
```

### Object Diffing

```jinja
{# Compare two objects and identify differences #}
{% macro diff_objects(obj1, obj2, path='') %}
    {% set diffs = [] %}

    {# Get all keys from both objects #}
    {% set all_keys = (obj1.keys() | list + obj2.keys() | list) | unique | list %}

    {% for key in all_keys %}
        {% set current_path = path ~ '.' ~ key if path else key %}
        {% set val1 = obj1.get(key) %}
        {% set val2 = obj2.get(key) %}

        {% if val1 is none and val2 is not none %}
            {% set _ = diffs.append({"path": current_path, "type": "added", "new_value": val2}) %}
        {% elif val2 is none and val1 is not none %}
            {% set _ = diffs.append({"path": current_path, "type": "removed", "old_value": val1}) %}
        {% elif val1 is mapping and val2 is mapping %}
            {% set nested_diffs = diff_objects(val1, val2, current_path) %}
            {% for d in nested_diffs %}
                {% set _ = diffs.append(d) %}
            {% endfor %}
        {% elif val1 != val2 %}
            {% set _ = diffs.append({"path": current_path, "type": "changed", "old_value": val1, "new_value": val2}) %}
        {% endif %}
    {% endfor %}

    {{ diffs }}
{% endmacro %}

{# Usage: Compare before/after states #}
{% set before = {"name": "John", "email": "john@old.com", "role": "user"} %}
{% set after = {"name": "John", "email": "john@new.com", "department": "IT"} %}
{% set changes = diff_objects(before, after) %}
{# Result: [{"path": "email", "type": "changed", ...}, {"path": "role", "type": "removed", ...}, {"path": "department", "type": "added", ...}] #}
```

---

## Advanced List Operations

### Group By Multiple Keys

```jinja
{# Group items by multiple attributes #}
{% macro group_by_multiple(items, keys) %}
    {% set groups = {} %}
    {% for item in items %}
        {# Build composite key #}
        {% set key_parts = [] %}
        {% for k in keys %}
            {% set _ = key_parts.append(item.get(k, 'None') | string) %}
        {% endfor %}
        {% set composite_key = key_parts | join('|') %}

        {% if composite_key not in groups %}
            {% set _ = groups.update({composite_key: {"keys": dict(zip(keys, key_parts)), "items": []}}) %}
        {% endif %}
        {% set _ = groups[composite_key].items.append(item) %}
    {% endfor %}
    {{ groups.values() | list }}
{% endmacro %}

{# Usage #}
{% set tickets = [
    {"priority": "High", "status": "Open", "title": "Issue 1"},
    {"priority": "High", "status": "Open", "title": "Issue 2"},
    {"priority": "Low", "status": "Closed", "title": "Issue 3"}
] %}
{% set grouped = group_by_multiple(tickets, ["priority", "status"]) %}
{# Result: [{"keys": {"priority": "High", "status": "Open"}, "items": [...]}, ...] #}
```

### Chunk Lists with Overlap

```jinja
{# Split list into overlapping chunks (useful for pagination with context) #}
{% macro chunk_with_overlap(items, size, overlap=0) %}
    {% set chunks = [] %}
    {% set step = size - overlap %}
    {% for i in range(0, items | length, step) %}
        {% set chunk = items[i:i + size] %}
        {% if chunk | length > 0 %}
            {% set _ = chunks.append(chunk) %}
        {% endif %}
    {% endfor %}
    {{ chunks }}
{% endmacro %}

{# Usage: Process items with context from previous batch #}
{% set data = range(1, 20) | list %}
{% set batches = chunk_with_overlap(data, 5, 1) %}
{# Result: [[1,2,3,4,5], [5,6,7,8,9], [9,10,11,12,13], [13,14,15,16,17], [17,18,19]] #}
```

### Interleave Lists

```jinja
{# Interleave multiple lists together #}
{% macro interleave(*lists) %}
    {% set result = [] %}
    {% set max_len = lists | map('length') | max | d(0) %}
    {% for i in range(max_len) %}
        {% for lst in lists %}
            {% if i < lst | length %}
                {% set _ = result.append(lst[i]) %}
            {% endif %}
        {% endfor %}
    {% endfor %}
    {{ result }}
{% endmacro %}

{# Usage: Round-robin assignment #}
{% set team_a = ["Alice", "Bob"] %}
{% set team_b = ["Charlie", "Dave", "Eve"] %}
{% set rotation = interleave(team_a, team_b) %}
{# Result: ["Alice", "Charlie", "Bob", "Dave", "Eve"] #}
```

### Partition by Predicate

```jinja
{# Split list into two based on condition #}
{% macro partition(items, predicate_attr, predicate_value=true) %}
    {% set matching = [] %}
    {% set non_matching = [] %}
    {% for item in items %}
        {% if item.get(predicate_attr) == predicate_value %}
            {% set _ = matching.append(item) %}
        {% else %}
            {% set _ = non_matching.append(item) %}
        {% endif %}
    {% endfor %}
    {{ {"matching": matching, "non_matching": non_matching} }}
{% endmacro %}

{# Usage #}
{% set users = [
    {"name": "Alice", "active": true},
    {"name": "Bob", "active": false},
    {"name": "Charlie", "active": true}
] %}
{% set partitioned = partition(users, "active", true) %}
{# Result: {"matching": [Alice, Charlie], "non_matching": [Bob]} #}
```

### Running Totals / Cumulative Sum

```jinja
{# Calculate running totals #}
{% macro running_total(items, value_attr) %}
    {% set results = [] %}
    {% set total = {"value": 0} %}
    {% for item in items %}
        {% set new_total = total.value + (item.get(value_attr) | d(0)) %}
        {% set _ = total.update({"value": new_total}) %}
        {% set _ = results.append(item | combine({"running_total": new_total})) %}
    {% endfor %}
    {{ results }}
{% endmacro %}

{# Usage: Track cumulative costs #}
{% set transactions = [
    {"date": "2024-01-01", "amount": 100},
    {"date": "2024-01-02", "amount": 50},
    {"date": "2024-01-03", "amount": -30}
] %}
{% set with_totals = running_total(transactions, "amount") %}
{# Result: [{..., "running_total": 100}, {..., "running_total": 150}, {..., "running_total": 120}] #}
```

---

## String Manipulation

### Template String Rendering

```jinja
{# Render dynamic templates within data #}
{% macro render_template_string(template_str, context) %}
    {% set result = template_str %}
    {% for key, value in context.items() %}
        {% set placeholder = '{{' ~ key ~ '}}' %}
        {% set result = result | replace(placeholder, value | string) %}
    {% endfor %}
    {{ result }}
{% endmacro %}

{# Usage #}
{% set template = "Hello {{name}}, your ticket #{{ticket_id}} is {{status}}" %}
{% set data = {"name": "John", "ticket_id": "12345", "status": "resolved"} %}
{{ render_template_string(template, data) }}
{# Result: "Hello John, your ticket #12345 is resolved" #}
```

### Smart Truncation

```jinja
{# Truncate at word boundaries with ellipsis #}
{% macro smart_truncate(text, max_length, suffix='...') %}
    {% if text | length <= max_length %}
        {{ text }}
    {% else %}
        {% set truncated = text[:max_length - suffix | length] %}
        {% set last_space = truncated.rfind(' ') %}
        {% if last_space > max_length * 0.7 %}
            {{ truncated[:last_space] ~ suffix }}
        {% else %}
            {{ truncated ~ suffix }}
        {% endif %}
    {% endif %}
{% endmacro %}

{# Usage #}
{{ smart_truncate("This is a very long description that needs to be truncated at word boundaries", 50) }}
{# Result: "This is a very long description that needs..." #}
```

### Extract Email Domain

```jinja
{# Extract domain from email with validation #}
{% macro extract_domain(email) %}
    {% if '@' in (email | d('')) %}
        {{ email.split('@')[1] | lower }}
    {% else %}
        {{ none }}
    {% endif %}
{% endmacro %}

{# Group users by email domain #}
{% set users = CTX.users %}
{% set by_domain = {} %}
{% for user in users %}
    {% set domain = extract_domain(user.email) %}
    {% if domain %}
        {% if domain not in by_domain %}
            {% set _ = by_domain.update({domain: []}) %}
        {% endif %}
        {% set _ = by_domain[domain].append(user) %}
    {% endif %}
{% endfor %}

{{ by_domain }}
```

### Parse Key-Value Strings

```jinja
{# Parse "key1=value1; key2=value2" format #}
{% macro parse_kv_string(text, pair_sep=';', kv_sep='=') %}
    {% set result = {} %}
    {% for pair in text.split(pair_sep) %}
        {% set trimmed = pair | trim %}
        {% if kv_sep in trimmed %}
            {% set parts = trimmed.split(kv_sep, 1) %}
            {% set _ = result.update({parts[0] | trim: parts[1] | trim}) %}
        {% endif %}
    {% endfor %}
    {{ result }}
{% endmacro %}

{# Usage: Parse cookie strings or similar formats #}
{% set cookies = "session_id=abc123; user_id=456; expires=2024-12-31" %}
{% set parsed = parse_kv_string(cookies) %}
{# Result: {"session_id": "abc123", "user_id": "456", "expires": "2024-12-31"} #}
```

### Generate Slug

```jinja
{# Generate URL-safe slug from text #}
{% macro slugify(text, separator='-') %}
    {% set slug = text | lower | trim %}
    {# Replace spaces and special chars #}
    {% set slug = slug | regex_replace('[^a-z0-9]+', separator) %}
    {# Remove leading/trailing separators #}
    {% set slug = slug | trim(separator) %}
    {# Remove consecutive separators #}
    {% set slug = slug | regex_replace(separator ~ '+', separator) %}
    {{ slug }}
{% endmacro %}

{# Usage #}
{{ slugify("Hello World! This is a Test...") }}
{# Result: "hello-world-this-is-a-test" #}
```

---

## Date/Time Advanced Patterns

### Business Days Calculation

```jinja
{# Calculate business days between dates (excluding weekends) #}
{% macro business_days_between(start_date, end_date) %}
    {% set start = start_date | as_datetime if start_date is string else start_date %}
    {% set end = end_date | as_datetime if end_date is string else end_date %}

    {% set total_days = (end - start).days %}
    {% set business_days = 0 %}

    {% for i in range(total_days) %}
        {% set current = start | datedelta(days=i) %}
        {% set weekday = current.weekday() %}
        {% if weekday < 5 %}
            {% set business_days = business_days + 1 %}
        {% endif %}
    {% endfor %}

    {{ business_days }}
{% endmacro %}

{# Usage #}
{% set sla_days = business_days_between(CTX.ticket_created, CTX.ticket_resolved) %}
```

### Add Business Days

```jinja
{# Add N business days to a date #}
{% macro add_business_days(start_date, days_to_add) %}
    {% set current = start_date | as_datetime if start_date is string else start_date %}
    {% set days_added = 0 %}

    {% for _ in range(days_to_add * 2) %}
        {% if days_added < days_to_add %}
            {% set current = current | datedelta(days=1) %}
            {% if current.weekday() < 5 %}
                {% set days_added = days_added + 1 %}
            {% endif %}
        {% endif %}
    {% endfor %}

    {{ current | format_datetime("%Y-%m-%d") }}
{% endmacro %}

{# Usage: Calculate SLA due date #}
{% set sla_due = add_business_days(CTX.created_date, 5) %}
```

### Time Zone Aware Scheduling

```jinja
{# Check if current time falls within business hours for a timezone #}
{% macro is_business_hours(timezone, start_hour=9, end_hour=17) %}
    {% set local_time = CTX.current_time | as_datetime | as_timezone(timezone) %}
    {% set hour = local_time.hour %}
    {% set weekday = local_time.weekday() %}

    {% set is_weekday = weekday < 5 %}
    {% set is_work_hours = hour >= start_hour and hour < end_hour %}

    {{ is_weekday and is_work_hours }}
{% endmacro %}

{# Usage: Route tickets based on business hours #}
{% set client_tz = CTX.client.timezone | d("US/Eastern") %}
{% if is_business_hours(client_tz) %}
    {% set routing = "normal" %}
{% else %}
    {% set routing = "after_hours" %}
{% endif %}
```

### Relative Time Formatting

```jinja
{# Format datetime as human-readable relative time #}
{% macro time_ago(dt) %}
    {% set now = CTX.current_time | as_datetime %}
    {% set then = dt | as_datetime if dt is string else dt %}
    {% set diff = now - then %}
    {% set seconds = diff.total_seconds() | int %}

    {% if seconds < 60 %}
        {{ "just now" }}
    {% elif seconds < 3600 %}
        {% set minutes = (seconds / 60) | int %}
        {{ minutes ~ " minute" ~ ("s" if minutes != 1 else "") ~ " ago" }}
    {% elif seconds < 86400 %}
        {% set hours = (seconds / 3600) | int %}
        {{ hours ~ " hour" ~ ("s" if hours != 1 else "") ~ " ago" }}
    {% elif seconds < 604800 %}
        {% set days = (seconds / 86400) | int %}
        {{ days ~ " day" ~ ("s" if days != 1 else "") ~ " ago" }}
    {% elif seconds < 2592000 %}
        {% set weeks = (seconds / 604800) | int %}
        {{ weeks ~ " week" ~ ("s" if weeks != 1 else "") ~ " ago" }}
    {% else %}
        {% set months = (seconds / 2592000) | int %}
        {{ months ~ " month" ~ ("s" if months != 1 else "") ~ " ago" }}
    {% endif %}
{% endmacro %}

{# Usage #}
{% set last_login = time_ago(CTX.user.lastSignInDateTime) %}
{# Result: "3 days ago" #}
```

---

## Error Handling Patterns

### Safe Attribute Access Chain

```jinja
{# Safely navigate deep attribute chains #}
{% macro safe_get(obj, path, default=none) %}
    {% set current = obj %}
    {% set parts = path.split('.') %}
    {% for part in parts %}
        {% if current is mapping and part in current %}
            {% set current = current[part] %}
        {% elif current is iterable and current is not string and part | int | string == part %}
            {% set idx = part | int %}
            {% if idx < current | length %}
                {% set current = current[idx] %}
            {% else %}
                {{ default }}
            {% endif %}
        {% else %}
            {{ default }}
        {% endif %}
    {% endfor %}
    {{ current if current is not none else default }}
{% endmacro %}

{# Usage #}
{% set email = safe_get(CTX.response, 'data.user.contact.email', 'no-email@example.com') %}
```

### Try-Catch Pattern

```jinja
{# Simulate try-catch with validation #}
{% macro try_operation(value, operation, default=none) %}
    {% set result = none %}
    {% set error = none %}

    {% if operation == 'as_int' %}
        {% if value | string | regex_match('^-?\\d+$') %}
            {% set result = value | int %}
        {% else %}
            {% set error = "Cannot convert to integer: " ~ value %}
        {% endif %}
    {% elif operation == 'as_float' %}
        {% if value | string | regex_match('^-?\\d+\\.?\\d*$') %}
            {% set result = value | float %}
        {% else %}
            {% set error = "Cannot convert to float: " ~ value %}
        {% endif %}
    {% elif operation == 'as_datetime' %}
        {% if value and value | string | length > 0 %}
            {% set result = value | as_datetime %}
        {% else %}
            {% set error = "Invalid datetime: " ~ value %}
        {% endif %}
    {% elif operation == 'as_json' %}
        {% if value is string %}
            {% set result = value | from_json %}
        {% else %}
            {% set error = "Value is not a JSON string" %}
        {% endif %}
    {% endif %}

    {{ {"success": error is none, "result": result if result is not none else default, "error": error} }}
{% endmacro %}

{# Usage #}
{% set parsed = try_operation(CTX.user_input, 'as_int', 0) %}
{% if parsed.success %}
    {% set quantity = parsed.result %}
{% else %}
    {% set error_message = parsed.error %}
{% endif %}
```

### Validation Framework

```jinja
{# Comprehensive input validation #}
{% macro validate(value, rules) %}
    {% set errors = [] %}
    {% set is_valid = true %}

    {% for rule in rules %}
        {% set rule_type = rule.type %}
        {% set failed = false %}

        {% if rule_type == 'required' %}
            {% if value is none or value | string | trim == '' %}
                {% set failed = true %}
            {% endif %}
        {% elif rule_type == 'min_length' %}
            {% if value | string | length < rule.value %}
                {% set failed = true %}
            {% endif %}
        {% elif rule_type == 'max_length' %}
            {% if value | string | length > rule.value %}
                {% set failed = true %}
            {% endif %}
        {% elif rule_type == 'pattern' %}
            {% if not value | string | regex_match(rule.value) %}
                {% set failed = true %}
            {% endif %}
        {% elif rule_type == 'email' %}
            {% if not value | string | regex_match('^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$') %}
                {% set failed = true %}
            {% endif %}
        {% elif rule_type == 'in_list' %}
            {% if value not in rule.value %}
                {% set failed = true %}
            {% endif %}
        {% elif rule_type == 'min_value' %}
            {% if (value | float) < rule.value %}
                {% set failed = true %}
            {% endif %}
        {% elif rule_type == 'max_value' %}
            {% if (value | float) > rule.value %}
                {% set failed = true %}
            {% endif %}
        {% endif %}

        {% if failed %}
            {% set is_valid = false %}
            {% set _ = errors.append(rule.message | d("Validation failed for " ~ rule_type)) %}
        {% endif %}
    {% endfor %}

    {{ {"is_valid": is_valid, "errors": errors, "value": value} }}
{% endmacro %}

{# Usage #}
{% set email_validation = validate(CTX.user_email, [
    {"type": "required", "message": "Email is required"},
    {"type": "email", "message": "Invalid email format"},
    {"type": "max_length", "value": 255, "message": "Email too long"}
]) %}

{% if not email_validation.is_valid %}
    {% set validation_errors = email_validation.errors %}
{% endif %}
```

---

## Performance Patterns

### Memoization Pattern

```jinja
{# Cache expensive computations (per-execution) #}
{% set _cache = {} %}

{% macro memoized_lookup(key, compute_func) %}
    {% if key not in _cache %}
        {% set result = compute_func() %}
        {% set _ = _cache.update({key: result}) %}
    {% endif %}
    {{ _cache[key] }}
{% endmacro %}

{# Usage: Cache API lookups within a workflow #}
{% macro get_user_details(user_id) %}
    {# Expensive API call #}
    {{ TASKS.get_user.result.result.data }}
{% endmacro %}

{% set user1 = memoized_lookup("user_123", get_user_details) %}
{% set user1_again = memoized_lookup("user_123", get_user_details) %}  {# Returns cached #}
```

### Lazy Evaluation

```jinja
{# Only compute expensive values when needed #}
{% macro lazy_value(condition, compute_if_true, compute_if_false=none) %}
    {% if condition %}
        {{ compute_if_true }}
    {% elif compute_if_false %}
        {{ compute_if_false }}
    {% endif %}
{% endmacro %}

{# Usage: Avoid unnecessary computation #}
{% set result = lazy_value(
    CTX.need_full_report,
    CTX.expensive_full_report,
    CTX.simple_summary
) %}
```

### Batch Processing Generator

```jinja
{# Generate batch configurations for With Items processing #}
{% macro generate_batches(items, batch_size, include_metadata=true) %}
    {% set batches = [] %}
    {% set total_batches = ((items | length) / batch_size) | round(0, 'ceil') | int %}

    {% for i in range(0, items | length, batch_size) %}
        {% set batch_items = items[i:i + batch_size] %}
        {% set batch_num = (i / batch_size) | int + 1 %}

        {% if include_metadata %}
            {% set batch = {
                "batch_number": batch_num,
                "total_batches": total_batches,
                "start_index": i,
                "end_index": i + (batch_items | length),
                "item_count": batch_items | length,
                "is_first": batch_num == 1,
                "is_last": batch_num == total_batches,
                "items": batch_items
            } %}
        {% else %}
            {% set batch = batch_items %}
        {% endif %}

        {% set _ = batches.append(batch) %}
    {% endfor %}

    {{ batches }}
{% endmacro %}

{# Usage: Process users in batches of 50 #}
{% set user_batches = generate_batches(CTX.all_users, 50) %}
{# Use with With Items to process each batch #}
```

---

## API Response Handling

### Pagination Handler

```jinja
{# Build pagination info from API response #}
{% macro parse_pagination(response, items_path='value', next_link_path='@odata.nextLink') %}
    {% set items = safe_get(response, items_path, []) %}
    {% set next_link = safe_get(response, next_link_path) %}
    {% set total_count = safe_get(response, '@odata.count') %}

    {{
        {
            "items": items,
            "count": items | length,
            "total": total_count,
            "has_more": next_link is not none,
            "next_link": next_link,
            "page_token": next_link.split('$skiptoken=')[1] if next_link and '$skiptoken=' in next_link else none
        }
    }}
{% endmacro %}

{# Usage #}
{% set page_info = parse_pagination(TASKS.list_users.result.result.data) %}
{% if page_info.has_more %}
    {# Continue pagination #}
{% endif %}
```

### Error Response Parser

```jinja
{# Parse and normalize API error responses #}
{% macro parse_api_error(response) %}
    {% set error_info = {
        "has_error": false,
        "code": none,
        "message": "Unknown error",
        "details": [],
        "retry_after": none
    } %}

    {# Check common error patterns #}
    {% if response.error is defined %}
        {# Microsoft Graph / OData style #}
        {% set _ = error_info.update({
            "has_error": true,
            "code": response.error.code | d("UnknownError"),
            "message": response.error.message | d("An error occurred"),
            "details": response.error.details | d([])
        }) %}
    {% elif response.errors is defined %}
        {# GraphQL style #}
        {% set _ = error_info.update({
            "has_error": true,
            "code": response.errors[0].extensions.code | d("GraphQLError") if response.errors else "UnknownError",
            "message": response.errors | map(attribute="message") | join("; ") if response.errors else "An error occurred",
            "details": response.errors | d([])
        }) %}
    {% elif response.status_code is defined and response.status_code >= 400 %}
        {# HTTP error #}
        {% set _ = error_info.update({
            "has_error": true,
            "code": "HTTP_" ~ response.status_code,
            "message": response.reason | d("HTTP Error " ~ response.status_code)
        }) %}
    {% elif response.fault is defined %}
        {# SOAP style #}
        {% set _ = error_info.update({
            "has_error": true,
            "code": response.fault.faultcode | d("SOAPFault"),
            "message": response.fault.faultstring | d("SOAP Fault")
        }) %}
    {% endif %}

    {# Check for retry-after header #}
    {% if response.headers is defined and response.headers.get('Retry-After') %}
        {% set _ = error_info.update({"retry_after": response.headers.get('Retry-After') | int}) %}
    {% endif %}

    {{ error_info }}
{% endmacro %}

{# Usage #}
{% set error = parse_api_error(TASKS.api_call.result.result.data) %}
{% if error.has_error %}
    {% if error.retry_after %}
        {# Schedule retry #}
    {% else %}
        {# Handle error #}
    {% endif %}
{% endif %}
```

### Response Normalizer

```jinja
{# Normalize responses from different APIs to common format #}
{% macro normalize_user_response(response, source) %}
    {% set user = {} %}

    {% if source == "azure_ad" %}
        {% set _ = user.update({
            "id": response.id,
            "email": response.mail | d(response.userPrincipalName),
            "display_name": response.displayName,
            "first_name": response.givenName,
            "last_name": response.surname,
            "job_title": response.jobTitle,
            "department": response.department,
            "phone": response.mobilePhone | d(response.businessPhones[0] if response.businessPhones else none),
            "created_at": response.createdDateTime,
            "enabled": response.accountEnabled
        }) %}
    {% elif source == "google_workspace" %}
        {% set _ = user.update({
            "id": response.id,
            "email": response.primaryEmail,
            "display_name": response.name.fullName,
            "first_name": response.name.givenName,
            "last_name": response.name.familyName,
            "job_title": response.organizations[0].title if response.organizations else none,
            "department": response.organizations[0].department if response.organizations else none,
            "phone": response.phones[0].value if response.phones else none,
            "created_at": response.creationTime,
            "enabled": not response.suspended
        }) %}
    {% elif source == "okta" %}
        {% set _ = user.update({
            "id": response.id,
            "email": response.profile.email,
            "display_name": response.profile.displayName | d(response.profile.firstName ~ ' ' ~ response.profile.lastName),
            "first_name": response.profile.firstName,
            "last_name": response.profile.lastName,
            "job_title": response.profile.title,
            "department": response.profile.department,
            "phone": response.profile.mobilePhone | d(response.profile.primaryPhone),
            "created_at": response.created,
            "enabled": response.status == "ACTIVE"
        }) %}
    {% endif %}

    {{ user }}
{% endmacro %}

{# Usage #}
{% set normalized_user = normalize_user_response(CTX.raw_user, CTX.identity_provider) %}
```

---

## Macro Library Pattern

### Reusable Macro Collection

```jinja
{# Define a library of reusable macros at the start of your workflow #}

{# ===== String Utilities ===== #}
{% macro str_coalesce(*values) %}
    {% for v in values %}
        {% if v is not none and v | string | trim != '' %}
            {{ v }}
            {% break %}
        {% endif %}
    {% endfor %}
{% endmacro %}

{% macro str_title_case(text) %}
    {{ text | title | replace("'S", "'s") | replace("'T", "'t") }}
{% endmacro %}

{% macro str_initials(name) %}
    {{ name.split() | map('first') | join('') | upper }}
{% endmacro %}

{# ===== Number Utilities ===== #}
{% macro num_format_currency(value, currency='USD') %}
    {% set symbols = {"USD": "$", "EUR": "€", "GBP": "£", "CAD": "CA$"} %}
    {{ symbols[currency] | d('$') }}{{ "{:,.2f}".format(value | float) }}
{% endmacro %}

{% macro num_format_bytes(bytes, precision=2) %}
    {% set units = ['B', 'KB', 'MB', 'GB', 'TB', 'PB'] %}
    {% set value = bytes | float %}
    {% set unit_index = 0 %}
    {% for _ in range(units | length - 1) %}
        {% if value >= 1024 %}
            {% set value = value / 1024 %}
            {% set unit_index = unit_index + 1 %}
        {% endif %}
    {% endfor %}
    {{ "{:.{}f}".format(value, precision) }} {{ units[unit_index] }}
{% endmacro %}

{# ===== List Utilities ===== #}
{% macro list_pluck(items, key) %}
    {{ items | map(attribute=key) | list }}
{% endmacro %}

{% macro list_unique_by(items, key) %}
    {% set seen = {} %}
    {% set result = [] %}
    {% for item in items %}
        {% set k = item.get(key) %}
        {% if k not in seen %}
            {% set _ = seen.update({k: true}) %}
            {% set _ = result.append(item) %}
        {% endif %}
    {% endfor %}
    {{ result }}
{% endmacro %}

{% macro list_sort_by_multiple(items, keys) %}
    {# Sort by multiple keys in order #}
    {% set sorted_items = items %}
    {% for key in keys | reverse %}
        {% set sorted_items = sorted_items | sort(attribute=key) | list %}
    {% endfor %}
    {{ sorted_items }}
{% endmacro %}

{# ===== Date Utilities ===== #}
{% macro date_is_today(dt) %}
    {% set check_date = dt | as_datetime if dt is string else dt %}
    {% set today = CTX.current_time | as_datetime %}
    {{ check_date.date() == today.date() }}
{% endmacro %}

{% macro date_is_weekend(dt) %}
    {% set check_date = dt | as_datetime if dt is string else dt %}
    {{ check_date.weekday() >= 5 }}
{% endmacro %}

{% macro date_format_friendly(dt) %}
    {% set d = dt | as_datetime if dt is string else dt %}
    {{ d | format_datetime("%B %d, %Y at %I:%M %p") }}
{% endmacro %}
```

---

## Best Practices

### Code Organization

1. **Define macros at workflow start** for reusability
2. **Use descriptive names** for variables and macros
3. **Comment complex logic** for maintainability
4. **Keep expressions single-purpose** - split complex operations

### Performance

1. **Avoid repeated calculations** - cache results in variables
2. **Use `| list` sparingly** - only when you need to reuse the result
3. **Filter early** - reduce data sets before complex operations
4. **Use `selectattr` over list comprehensions** when possible

### Safety

1. **Always use `| d()` defaults** for potentially undefined values
2. **Validate inputs** before processing
3. **Handle empty lists/dicts** gracefully
4. **Test edge cases** - empty inputs, null values, malformed data

### Debugging

1. **Use `| tojson`** to inspect complex objects
2. **Break complex expressions** into steps with intermediate variables
3. **Add diagnostic output** during development
4. **Test macros in isolation** before integrating
