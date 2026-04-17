# Rewst Testing & Quality Assurance

Comprehensive guide to testing workflows, debugging, and ensuring production readiness.

---

## Testing Philosophy

### Test Pyramid for Rewst

```
        /\
       /  \      Production Runs
      /    \     (Real integrations, real orgs)
     /------\
    /        \   Integration Tests
   /          \  (Test org, real APIs)
  /------------\
 /              \ Unit Tests
/                \(Mock actions, Jinja validation)
```

---

## Jinja Testing

### Live Editor Testing

```jinja
{# Test expressions in workflow Live Editor #}

{# 1. Set up test data #}
{% set test_users = [
    {"name": "Alice", "enabled": true, "dept": "IT"},
    {"name": "Bob", "enabled": false, "dept": "HR"},
    {"name": "Charlie", "enabled": true, "dept": "IT"}
] %}

{# 2. Test filter chain #}
{{ test_users | selectattr("enabled") | selectattr("dept", "eq", "IT") | list }}

{# Expected: [{"name": "Alice", ...}] #}
```

### Common Test Cases

```jinja
{# Test empty list handling #}
{% set empty_list = [] %}
{{ empty_list | first | d("no items") }}  {# Should: "no items" #}
{{ empty_list | length }}  {# Should: 0 #}

{# Test None handling #}
{% set maybe_none = none %}
{{ maybe_none | d("fallback") }}  {# Should: "fallback" #}
{{ maybe_none.property | d("safe") }}  {# Should: "safe" #}

{# Test string None vs actual None #}
{% set string_none = "None" %}
{{ string_none | d("fallback") }}  {# Should: "None" (not fallback!) #}

{# Test date parsing #}
{{ "2025-01-15T10:30:00Z" | as_datetime | format_datetime("%Y-%m-%d") }}
{# Should: "2025-01-15" #}

{# Test list comprehension edge cases #}
{% set users = [] %}
{{ [u.name for u in users] }}  {# Should: [] (empty list, not error) #}
```

---

## Mock Actions

### Using Mock Action for Testing

```jinja
{# Mock action returns whatever you configure #}

Mock Result:
{
    "status_code": 200,
    "data": {
        "value": [
            {"id": "1", "displayName": "Test User", "mail": "test@example.com"}
        ]
    }
}

{# Test workflow logic without real API calls #}
```

### Mock Different Scenarios

```jinja
{# Success scenario #}
{
    "status_code": 200,
    "data": {"id": "123", "status": "created"}
}

{# Error scenario #}
{
    "status_code": 404,
    "data": {"error": {"message": "User not found"}}
}

{# Empty results #}
{
    "status_code": 200,
    "data": {"value": []}
}

{# Rate limiting #}
{
    "status_code": 429,
    "data": {"error": "Rate limited", "retryAfter": 60}
}
```

---

## Test Organizations

### Developer Test Account Setup

```
1. Create dedicated test organization
2. Configure all integrations in test mode
3. Use test/sandbox APIs where available:
   - Microsoft: Developer tenant
   - ConnectWise: Sandbox instance
   - Datto: Test environment

4. Enable all triggers for testing
5. Document test credentials separately
```

### Test Data Management

```jinja
{# Prefix test entities #}
{% set test_prefix = "[TEST] " %}
{{ test_prefix }}{{ CTX.ticket_subject }}

{# Use test email domains #}
test-user@rewst-testing.com

{# Clean up test data after tests #}
{# Delete test tickets, users, etc. #}
```

---

## Workflow Testing Checklist

### Pre-Deploy Testing

```markdown
□ Happy Path
  - Normal inputs produce expected outputs
  - All transitions follow correct paths
  - Data aliases contain expected values

□ Edge Cases
  - Empty lists/inputs handled
  - None/null values handled
  - Missing optional fields handled

□ Error Handling
  - API failures trigger On Failure paths
  - Error messages are meaningful
  - Workflow doesn't hang on errors

□ With Items
  - Works with 0, 1, and many items
  - Handles item failures gracefully
  - Concurrency doesn't cause rate limiting

□ Subworkflows
  - Inputs passed correctly
  - Outputs returned correctly
  - Parent handles subworkflow failures

□ Multi-Org
  - Works for different org configurations
  - Run as Org executes in correct context
  - Org Variables accessed correctly
```

### Integration Testing

```markdown
□ API Connectivity
  - Integrations authenticated
  - API endpoints accessible
  - Permissions sufficient

□ Data Flow
  - Data transforms correctly between systems
  - IDs map properly across integrations
  - Timestamps convert to correct timezones

□ End-to-End
  - Trigger → Processing → Output works
  - Webhooks receive and parse correctly
  - Forms submit and populate CTX
```

---

## Debugging Techniques

### Debug Action

```jinja
{# Add Debug action to inspect values #}
{# Parameters: any values you want to log #}

Debug Parameters:
- user_data: {{ CTX.user | to_json_string }}
- task_result: {{ TASKS.get_user.result.result | to_json_string }}
- calculated_value: {{ CTX.processed_items | length }}
```

### Execution Details Analysis

```
1. Click workflow execution
2. Examine each task:
   - Input Parameters (what was sent)
   - Output (what was returned)
   - Transitions (which path taken)
   - Data Aliases (what was stored)

3. Look for:
   - Unexpected None values
   - Missing fields in responses
   - Wrong transition paths
   - Jinja evaluation errors
```

### Common Debug Patterns

```jinja
{# Check if value exists and what type #}
Debug:
- exists: {{ CTX.var is defined }}
- value: {{ CTX.var | d("UNDEFINED") }}
- type: {{ CTX.var.__class__.__name__ | d("N/A") }}
- length: {{ CTX.var | length | d("N/A") }}

{# Trace API response structure #}
Debug:
- status_code: {{ TASKS.api_call.result.result.status_code }}
- headers: {{ TASKS.api_call.result.result.headers | to_json_string }}
- data_keys: {{ TASKS.api_call.result.result.data.keys() | list if TASKS.api_call.result.result.data else "no data" }}
```

---

## Regression Testing

### Version Control Workflow Changes

```markdown
Before modifying production workflow:

1. Document current behavior
2. Export workflow JSON (backup)
3. Create test executions for current behavior
4. Make changes
5. Run same tests
6. Compare results
7. Only deploy if all tests pass
```

### Critical Path Testing

```markdown
For each workflow, identify critical paths:

User Provisioning:
1. Form → Workflow trigger
2. User creation in M365
3. License assignment
4. Group membership
5. PSA ticket creation
6. Notification sent

Test each path independently and together.
```

---

## Performance Testing

### Load Testing Patterns

```jinja
{# Test with increasing data volumes #}
{% set test_sizes = [10, 50, 100, 500, 1000] %}

{# For each size, measure: #}
{# - Execution time #}
{# - API calls made #}
{# - Memory usage (check for timeouts) #}
{# - Error rate #}
```

### Stress Testing

```markdown
Test limits:

1. Maximum With Items concurrency
   - Try 5, 10, 15, 20
   - Note when 429 errors start

2. Maximum list sizes
   - Process 100, 1000, 10000 items
   - Note when timeouts occur

3. Webhook throughput
   - Send rapid webhook calls
   - Note queue behavior
```

---

## Form Testing

### Form Validation Testing

```markdown
Test each field type:

□ Text Input
  - Empty submission
  - Maximum length
  - Special characters
  - Unicode/emoji

□ Dropdown
  - No selection
  - Each option value correct
  - Dynamic options load

□ Multi-Select
  - No selection
  - Single selection
  - All selections
  - Value format correct

□ Date
  - Past dates
  - Future dates
  - Invalid formats

□ File Upload
  - No file
  - Large file
  - Wrong file type
```

### Option Generator Testing

```jinja
{# Test option generator returns correct format #}
{
    "options": [
        {"label": "Display Text", "value": "actual_value"},
        ...
    ]
}

{# Test edge cases #}
- Returns empty array gracefully
- Handles API errors
- Returns within timeout
- Labels are user-friendly
- Values are correct identifiers
```

---

## Error Injection Testing

### Simulate Failures

```jinja
{# Manually trigger different error paths #}

{# 1. Invalid credentials - test auth error handling #}
{# 2. Wrong endpoint - test 404 handling #}
{# 3. Malformed payload - test 400 handling #}
{# 4. Rate limiting - add delays between calls #}
```

### Chaos Testing

```markdown
Test unexpected conditions:

1. Integration temporarily disabled
2. API returns unexpected schema
3. Partial data in response
4. Network timeout
5. Invalid JSON response
```

---

## Production Validation

### Staged Rollout

```markdown
1. Deploy to single test org first
2. Run with real but low-risk data
3. Monitor for 24-48 hours
4. Gradually enable more orgs
5. Full rollout after validation
```

### Monitoring Checklist

```markdown
After deployment, monitor for:

□ Execution success rate
□ Average execution time
□ Error patterns
□ API rate limiting
□ User feedback
□ Unexpected behavior
```

### Rollback Plan

```markdown
If issues detected:

1. Disable trigger immediately
2. Document the issue
3. Restore previous version (from backup)
4. Investigate root cause
5. Fix and retest
6. Redeploy with monitoring
```

---

## Test Documentation Template

```markdown
# Workflow: [Workflow Name]

## Test Cases

### TC-001: Happy Path
**Description:** Normal user provisioning
**Prerequisites:** Test org configured, M365 integration active
**Steps:**
1. Submit form with valid data
2. Workflow executes
3. User created in M365
4. Ticket created in PSA

**Expected Results:**
- User exists in M365 with correct attributes
- Ticket contains user details
- Notification sent

**Status:** [ ] Pass [ ] Fail

### TC-002: Missing Manager
**Description:** User with no manager selected
**Steps:**
1. Submit form without manager
2. Workflow executes

**Expected Results:**
- Manager field defaults to "None"
- Workflow completes without error

**Status:** [ ] Pass [ ] Fail

---

## Test Execution Log

| Date | Tester | TC | Result | Notes |
|------|--------|----|---------|----|
| 2025-01-15 | Tester | TC-001 | Pass | |
| 2025-01-15 | Tester | TC-002 | Fail | Error on line 45 |
```

---

## Automated Testing Patterns

### Self-Testing Workflows

```jinja
{# Create workflow that tests another workflow #}

1. Setup Test Data
   └── Create known input values

2. Execute Target Workflow
   └── Use subworkflow action

3. Validate Results
   └── Check CTX.published_results against expected

4. Report Results
   └── Create ticket/notification with test results
```

### Scheduled Health Checks

```
Trigger: Cron (daily 6 AM)

1. Test API Connectivity
   ├── M365 Graph - list 1 user
   ├── PSA - list 1 ticket
   └── RMM - list 1 device

2. Evaluate Results
   ├── All success → Log healthy
   └── Any failure → Alert team

3. Report
   └── Send daily health summary
```
