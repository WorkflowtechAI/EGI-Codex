# Rewst Jinja Reference

Complete reference for Jinja filters and patterns in Rewst.

> **Note:** Rewst uses Jinja2 with custom extensions. Some standard Jinja2 filters behave differently, and Rewst adds its own. Always use `as_datetime` (not `strptime`) and `as_timezone` (not `astimezone`).
>
> **Rewst Quirk:** Mutable objects (lists, dicts, namespaces) display their **final state** immediately when first rendered, not their state at render time. This differs from standard Jinja2.

---

## Complete Filter Reference (A–Z)

All filters available in Rewst, sourced from official docs:

| Filter | Description | Example |
|--------|-------------|---------|
| `abs` | Absolute value | `{{ -5\|abs }}` → `5` |
| `all` | True if all items are truthy | `{{ [True, True]\|all }}` → `True` |
| `any` | True if any item is truthy | `{{ [0, 1, 0]\|any }}` → `True` |
| `as_timezone` | Convert datetime to timezone (use IANA names) | `{{ dt\|as_timezone("US/Eastern") }}` |
| `attr` | Get attribute of object | `{{ user\|attr("name") }}` |
| `base64` | Encode string in base64 | `{{ "hello"\|base64 }}` |
| `basename` | Get filename from path | `{{ "/path/file.txt"\|basename }}` → `file.txt` |
| `batch` | Split list into batches | `{{ list\|batch(5) }}` |
| `capitalize` | First letter uppercase, rest lower | `{{ "john"\|capitalize }}` → `John` |
| `center` | Center string in given width | `{{ "hi"\|center(20) }}` |
| `combine` | Merge two dicts | `{{ dict1\|combine(dict2) }}` |
| `convert_from_epoch` | Convert epoch timestamp to datetime | `{{ 1700000000\|convert_from_epoch }}` |
| `count` | Count items (alias for length) | `{{ list\|count }}` |
| `csv` | Format value as CSV | `{{ list\|csv }}` |
| `d` / `default` | Default value if undefined/empty | `{{ var\|d("fallback") }}` |
| `datedelta` | Add/subtract time from datetime | `{{ dt\|datedelta(days=7, hours=-2) }}` |
| `dict` | Create dictionary | `{{ dict(key=value) }}` |
| `dictsort` | Sort dict by key or value | `{{ mydict\|dictsort }}` |
| `dirname` | Get directory from path | `{{ "/path/file.txt"\|dirname }}` → `/path` |
| `enumerate` | Add index to iterable | `{{ list\|enumerate }}` |
| `escape` | HTML-escape string | `{{ "<b>"\|escape }}` → `&lt;b&gt;` |
| `filesizeformat` | Human-readable file size | `{{ 1048576\|filesizeformat }}` → `1 MB` |
| `first` | First item of list | `{{ list\|first }}` |
| `flatten` | Flatten nested list | `{{ [[1,2],[3]]\|flatten }}` → `[1,2,3]` |
| `float` | Convert to float | `{{ "3.14"\|float }}` |
| `forceescape` | Force HTML escaping | `{{ str\|forceescape }}` |
| `format` | Python-style string format | `{{ "%s is %d"\|format("age", 30) }}` |
| `format_datetime` | Format datetime as string | `{{ dt\|format_datetime("%Y-%m-%d") }}` |
| `from_json_string` | Parse JSON string to object | `{{ '{"key":"val"}'\|from_json_string }}` |
| `from_yaml_string` | Parse YAML string to object | `{{ yaml_str\|from_yaml_string }}` |
| `groupby` | Group list by attribute | `{{ users\|groupby("department") }}` |
| `hex` | Convert integer to hex | `{{ 255\|hex }}` → `ff` |
| `hmac` | Generate HMAC hash | `{{ msg\|hmac(key, "sha256") }}` |
| `indent` | Indent string | `{{ text\|indent(4) }}` |
| `int` | Convert to integer | `{{ "42"\|int }}` |
| `is_json` | Check if string is valid JSON | `{{ str\|is_json }}` |
| `is_type` | Check variable type | `{{ var\|is_type("list") }}` |
| `items` | Get dict items as list | `{{ mydict\|items }}` |
| `join` | Join list to string | `{{ list\|join(", ") }}` |
| `json` / `tojson` | Serialize to JSON string | `{{ obj\|tojson }}` |
| `json_dump` | Pretty-print JSON | `{{ obj\|json_dump }}` |
| `json_escape` | Escape for JSON | `{{ str\|json_escape }}` |
| `json_parse` | Parse JSON string | `{{ str\|json_parse }}` |
| `json_stringify` | Alias for tojson | `{{ obj\|json_stringify }}` |
| `jsonpath_query` | Query using JSONPath | `{{ data\|jsonpath_query("$.users[*].name") }}` |
| `last` | Last item of list | `{{ list\|last }}` |
| `length` | Count items | `{{ list\|length }}` |
| `list` | Convert to list (materialize iterable) | `{{ gen\|list }}` |
| `load_datetime` | Load datetime from string | `{{ "2025-01-15"\|load_datetime }}` |
| `lower` | Lowercase string | `{{ "HELLO"\|lower }}` → `hello` |
| `map` | Apply operation to each item | `{{ users\|map(attribute="email")\|list }}` |
| `max` | Maximum value | `{{ [1, 5, 3]\|max }}` → `5` |
| `min` | Minimum value | `{{ [1, 5, 3]\|min }}` → `1` |
| `parse_csv` | Parse CSV string | `{{ csv_str\|parse_csv }}` |
| `parse_datetime` | Parse datetime string | `{{ "2025-01-15"\|parse_datetime("%Y-%m-%d") }}` |
| `pprint` | Pretty-print for debugging | `{{ obj\|pprint }}` |
| `random` | Random item from list | `{{ list\|random }}` |
| `reduce` | Reduce list with function | `{{ [1,2,3]\|reduce("lambda a,b: a+b") }}` |
| `regex_findall` | Find all regex matches | `{{ str\|regex_findall("[0-9]+") }}` |
| `regex_match` | Check if string matches regex | `{{ str\|regex_match("^[A-Z]") }}` |
| `regex_replace` | Replace using regex | `{{ str\|regex_replace("[^a-z]", "") }}` |
| `regex_search` | Search for regex pattern | `{{ str\|regex_search("[0-9]+") }}` |
| `regex_substring` | Extract regex group | `{{ str\|regex_substring("(\d+)") }}` |
| `reject` | Exclude items (by test) | `{{ list\|reject("equalto", 0)\|list }}` |
| `rejectattr` | Exclude items by attribute | `{{ users\|rejectattr("active")\|list }}` |
| `replace` | Replace substring | `{{ str\|replace("old", "new") }}` |
| `reverse` | Reverse list or string | `{{ list\|reverse\|list }}` |
| `round` | Round number | `{{ 3.7\|round }}` → `4.0` |
| `safe` | Mark string as safe HTML | `{{ html\|safe }}` |
| `select` | Filter items by test | `{{ list\|select("odd")\|list }}` |
| `selectattr` | Filter by attribute | `{{ users\|selectattr("active", "eq", true)\|list }}` |
| `set` | Convert to set (deduplicate) | `{{ list\|set }}` |
| `slice` | Slice list into chunks | `{{ list\|slice(3) }}` |
| `sort` | Sort list | `{{ list\|sort }}` / `{{ users\|sort(attribute="name") }}` |
| `string` | Convert to string | `{{ 42\|string }}` |
| `striptags` | Remove HTML tags | `{{ html\|striptags }}` |
| `sum` | Sum numeric list | `{{ [1, 2, 3]\|sum }}` → `6` |
| `time_delta` | Time difference between datetimes | `{{ dt1\|time_delta(dt2) }}` |
| `title` | Title Case | `{{ "hello world"\|title }}` → `Hello World` |
| `to_ascii` | Convert to ASCII | `{{ "café"\|to_ascii }}` → `cafe` |
| `to_human_time_from_seconds` | Convert seconds to readable time | `{{ 3661\|to_human_time_from_seconds }}` → `1h 1m 1s` |
| `to_json_string` | Serialize to JSON string | `{{ obj\|to_json_string }}` |
| `to_yaml_string` | Serialize to YAML string | `{{ obj\|to_yaml_string }}` |
| `trim` | Remove leading/trailing whitespace | `{{ "  hello  "\|trim }}` |
| `truncate` | Truncate string | `{{ text\|truncate(20) }}` |
| `tuple` | Convert to tuple | `{{ list\|tuple }}` |
| `unidecode` | Transliterate unicode to ASCII | `{{ "Ångström"\|unidecode }}` → `Angstrom` |
| `unique` | Remove duplicates | `{{ list\|unique\|list }}` |
| `upper` | Uppercase string | `{{ "hello"\|upper }}` → `HELLO` |
| `urldecode` | URL-decode string | `{{ "hello%20world"\|urldecode }}` → `hello world` |
| `urlencode` | URL-encode string | `{{ "hello world"\|urlencode }}` → `hello+world` |
| `urlize` | Convert URLs to clickable links | `{{ text\|urlize }}` |
| `use_none` | Return None if falsy (instead of empty string) | `{{ var\|use_none }}` |
| `version_bump_major` | Bump major version | `{{ "1.2.3"\|version_bump_major }}` → `2.0.0` |
| `version_bump_minor` | Bump minor version | `{{ "1.2.3"\|version_bump_minor }}` → `1.3.0` |
| `version_bump_patch` | Bump patch version | `{{ "1.2.3"\|version_bump_patch }}` → `1.2.4` |
| `version_compare` | Compare two versions | `{{ "1.2"\|version_compare("1.3") }}` |
| `version_equal` | Check version equality | `{{ "1.2"\|version_equal("1.2") }}` |
| `version_less_than` | Check if version is less than | `{{ "1.2"\|version_less_than("1.3") }}` |
| `version_more_than` | Check if version is greater than | `{{ "1.3"\|version_more_than("1.2") }}` |
| `version_strip_patch` | Remove patch from version | `{{ "1.2.3"\|version_strip_patch }}` → `1.2` |
| `wordcount` | Count words | `{{ text\|wordcount }}` |
| `wordwrap` | Wrap text at word boundaries | `{{ text\|wordwrap(80) }}` |
| `wrap_text` | Wrap text with prefix/suffix | `{{ text\|wrap_text(prefix="- ") }}` |
| `xmlattr` | Create XML attributes | `{{ dict\|xmlattr }}` |
| `yaml_dump` | Serialize to YAML | `{{ obj\|yaml_dump }}` |
| `yaml_parse` | Parse YAML string | `{{ yaml_str\|yaml_parse }}` |
| `zip` | Combine two lists element-by-element | `{{ list1\|zip(list2)\|list }}` |

---

## Date/Time Filters (DIFFERENT from standard Jinja2!)

These are **Rewst-specific** — do not use standard Python equivalents:

| Task | Use This | NOT This |
|------|----------|----------|
| Parse string to datetime | `\| as_datetime` or `\| parse_datetime` | ~~strptime~~ |
| Convert timezone | `\| as_timezone("US/Eastern")` | ~~astimezone~~ |
| Add/subtract time | `\| datedelta(days=7)` | — |
| Format to string | `\| format_datetime("%Y-%m-%d")` | — |
| Get current time | `{{ now() }}` | — |

```jinja
{# Parse a date string #}
{{ "2025-01-15" | as_datetime }}

{# Convert timezone (IANA timezone names) #}
{{ CTX.event_time | as_timezone("America/New_York") }}

{# Add 30 days #}
{{ now() | datedelta(days=30) }}

{# Subtract 1 hour #}
{{ CTX.start_time | datedelta(hours=-1) }}

{# Format for display #}
{{ now() | format_datetime("%B %d, %Y at %H:%M") }}

{# Format for API (ISO 8601) #}
{{ now() | format_datetime("%Y-%m-%dT%H:%M:%SZ") }}

{# Compare dates #}
{{ CTX.last_login | as_datetime < now() | datedelta(days=-90) }}
```

---

## String Manipulation

```jinja
{# Username generation #}
{% set username = (CTX.first_name[0] ~ CTX.last_name) | lower | regex_replace("[^a-z]", "") %}

{# Email normalization #}
{{ CTX.email | lower | trim }}

{# Strip special chars #}
{{ CTX.input | regex_replace("[^a-zA-Z0-9 ]", "") }}

{# Extract digits only #}
{{ CTX.phone | regex_replace("[^0-9]", "") }}

{# Title case name #}
{{ CTX.full_name | title }}

{# Check pattern #}
{{ CTX.email | regex_match("^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$") }}

{# Find all matches #}
{{ CTX.text | regex_findall("[A-Z]{2,}") }}

{# Replace with regex #}
{{ CTX.text | regex_replace("(password|secret)", "***") }}
```

---

## List Operations

```jinja
{# Filter by attribute with operator #}
{{ CTX.users | selectattr("accountEnabled", "eq", true) | list }}
{{ CTX.users | selectattr("age", "gt", 18) | list }}
{{ CTX.users | rejectattr("accountEnabled") | list }}

{# Map to single attribute #}
{{ CTX.users | map(attribute="email") | list }}

{# Map with filter applied #}
{{ CTX.users | map(attribute="email") | map("lower") | list }}

{# Unique values #}
{{ CTX.emails | unique | list }}

{# Unique by attribute #}
{{ CTX.users | unique(attribute="department") | list }}

{# Sort #}
{{ CTX.users | sort(attribute="displayName") | list }}
{{ CTX.numbers | sort(reverse=true) | list }}

{# Group by attribute #}
{% for dept, members in CTX.users | groupby("department") %}
  {{ dept }}: {{ members | length }} users
{% endfor %}

{# Flatten nested list #}
{{ [[1, 2], [3, 4]] | flatten }}

{# Batch into chunks #}
{% for batch in CTX.users | batch(20) %}
  {# Process up to 20 at a time #}
{% endfor %}

{# Sum a field #}
{{ CTX.items | map(attribute="quantity") | sum }}

{# Max/Min #}
{{ CTX.items | map(attribute="price") | max }}

{# Zip two lists #}
{{ [dict(a, **b) for a, b in CTX.list1 | zip(CTX.list2)] }}
```

---

## Dictionary Operations

```jinja
{# Combine dicts (second overwrites first) #}
{{ {"a": 1} | combine({"b": 2}) }}

{# Iterate dict items #}
{% for key, value in CTX.my_dict | items %}
  {{ key }}: {{ value }}
{% endfor %}

{# Sort dict by key #}
{{ CTX.my_dict | dictsort }}

{# Build dict in comprehension #}
{{ {user.id: user.email for user in CTX.users} }}

{# Safe nested access #}
{{ (CTX.response | d({})).data | d([]) }}
```

---

## JSON Operations

```jinja
{# Serialize to JSON #}
{{ CTX.obj | to_json_string }}
{{ CTX.obj | tojson }}

{# Parse JSON string #}
{{ CTX.json_str | from_json_string }}

{# Check if valid JSON #}
{{ CTX.str | is_json }}

{# JSONPath query #}
{{ CTX.data | jsonpath_query("$.users[*].email") }}

{# Pretty print for debugging #}
{{ CTX.obj | json_dump }}
```

---

## Encoding

```jinja
{# Base64 encode/decode #}
{{ "hello world" | base64 }}
{{ CTX.encoded | base64(decode=true) }}

{# URL encode/decode #}
{{ "hello world" | urlencode }}
{{ CTX.encoded_url | urldecode }}

{# HTML escape #}
{{ CTX.user_input | escape }}

{# Strip HTML tags #}
{{ CTX.html_content | striptags }}
```

---

## Namespace (Loop Variable Mutation)

Standard Jinja2 doesn't allow modifying variables inside loops. Use `namespace`:

```jinja
{% set ns = namespace(found=false, count=0, result=none) %}

{% for item in CTX.items %}
    {% if item.status == "active" %}
        {% set ns.count = ns.count + 1 %}
        {% if not ns.found %}
            {% set ns.found = true %}
            {% set ns.result = item %}
        {% endif %}
    {% endif %}
{% endfor %}

Found: {{ ns.found }}, Count: {{ ns.count }}, First: {{ ns.result | tojson }}
```

---

## Macros (Reusable Jinja Functions)

```jinja
{# Define a macro #}
{% macro format_user(user) %}
{{ user.displayName }} <{{ user.mail | d("no-email") }}>
{% endmacro %}

{# Call it #}
{% for user in CTX.users %}
{{ format_user(user) }}
{% endfor %}

{# Macro with default arguments #}
{% macro ticket_note(title, body, priority="Normal") %}
[{{ priority }}] {{ title }}
{{ body }}
{% endmacro %}
```

---

## Try/Catch Pattern

Rewst Jinja doesn't have native try/catch, but use these patterns:

```jinja
{# Safe default for missing/null #}
{{ CTX.user.email | d("unknown@example.com") }}

{# Safe nested access #}
{{ CTX.result.data.value | d([]) | first | d({}) }}

{# Conditional with default #}
{% if CTX.response and CTX.response.data %}
    {{ CTX.response.data.id }}
{% else %}
    not-found
{% endif %}

{# use_none for explicit null instead of empty string #}
{{ CTX.optional_value | use_none }}
```

---

## Transform Actions (GUI Alternative to Jinja)

Transform actions are drag-and-drop workflow actions that perform common data operations **without writing Jinja**. Available in the workflow builder under **Transform** actions:

| Category | Transform Actions |
|----------|------------------|
| **List** | Append with items results, Diff lists, Flatten list, Get list length, Map to attribute, Merge lists, Remove duplicates, Return element, Select attribute, Set list field value, Sort, Transform list objects |
| **String** | Get string length, Split text, Trim variable, URL encode/decode, Wrap text |
| **Date/Time** | Add/subtract from DateTime, Compare dates, Convert DateTime to timezone, Convert from epoch, Extract part of date, Format DateTime, Get business days between dates, Get DateTime, Get days between dates |
| **Data** | All, Any, Average, Base64 encode/decode, Convert list to object, Defang/Refang, Is JSON, Parse CSV, Parse text to JSON, Range, Set variable, YAML parse |
| **Version** | (use Jinja version_* filters) |

> **Tip:** For simple transforms, actions are faster to configure. For complex chains or list comprehensions, Jinja in a Noop's data alias gives more control.

---

## Common Patterns Quick Reference

```jinja
{# Username from first/last #}
{{ (CTX.first[0] ~ CTX.last) | lower | regex_replace("[^a-z]", "") }}

{# Safe dict build #}
{{ {"key": CTX.val | d("default")} | tojson }}

{# Date 30 days from now #}
{{ now() | datedelta(days=30) | format_datetime("%Y-%m-%dT%H:%M:%SZ") }}

{# Filter, map, unique pipeline #}
{{ CTX.users | selectattr("active") | map(attribute="email") | map("lower") | unique | list }}

{# Count by status #}
{% set active = CTX.users | selectattr("active", "eq", true) | list | length %}
{% set inactive = CTX.users | rejectattr("active", "eq", true) | list | length %}

{# Build options list for option generator #}
{{ [{"label": u.displayName, "value": u.id} for u in CTX.users | sort(attribute="displayName")] }}

{# Combine with zip #}
{{ [dict(a, **b) for a, b in CTX.list1 | zip(CTX.list2)] }}

{# Epoch to readable #}
{{ CTX.timestamp | convert_from_epoch | as_timezone("US/Eastern") | format_datetime("%Y-%m-%d %H:%M") }}
```
