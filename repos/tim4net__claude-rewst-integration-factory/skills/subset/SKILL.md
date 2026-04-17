---
name: subset
description: Reduce an OpenAPI spec to a smaller set of operations that fit Rewst size limits
argument-hint: <path-to-spec.json> [target-size-kb]
---

# Subset OpenAPI Spec for Rewst

You are helping reduce an OpenAPI specification to fit within Rewst Custom Integration v2's ~500KB size limit while keeping the most useful operations.

## Your Task

Subset the spec at: `$ARGUMENTS`

Parse arguments:
- First: source spec path (required)
- Second: target size in KB (optional, default 450KB for safety margin)

## Process

### Phase 1: Analysis

Read the spec and report:

```
## Current Spec Analysis

- **File size**: X KB (target: Y KB, need to reduce by Z KB)
- **Total operations**: X
- **Total schemas**: X

### Operations by Category
| Tag | GET | POST | PUT | PATCH | DELETE | Total |
|-----|-----|------|-----|-------|--------|-------|
| ... | ... | ...  | ... | ...   | ...    | ...   |

### Size Breakdown (estimated)
- Paths: ~X KB
- Schemas: ~X KB
- Other: ~X KB
```

### Phase 2: Understand Use Cases

Ask the user about their workflow needs:

1. **Primary use case**: What will they automate with this integration?
   - Monitoring/alerting?
   - Asset management?
   - Reporting?
   - User/access management?
   - Configuration management?

2. **Must-have operations**: Any specific endpoints they definitely need?

3. **Nice-to-have**: Operations that would be useful but not critical?

### Phase 3: Recommend Subset

Based on use cases, categorize operations:

```
## Recommended Subset

### Tier 1: Essential (always include)
These operations are fundamental to any integration:
- List/Get for primary resources
- Basic CRUD for commonly managed items

### Tier 2: Recommended for your use case
Based on [stated use case]:
- [Operations that support the workflow]

### Tier 3: Optional
Could be added if space permits:
- [Less common operations]

### Tier 4: Exclude
Rarely needed for automation:
- [Specialized/admin operations]

### Projected Result
- Operations: X (down from Y)
- Estimated size: ~X KB
```

### Phase 4: Create Subset

After user confirms:

1. **Copy base structure** (info, servers, security, components.securitySchemes)
2. **Include selected paths** with their operations
3. **Include only referenced schemas** (traverse $ref dependencies)
4. **Include all used tags** in global tags array
5. **Add ErrorResponse schema** if not present

### Phase 5: Validate & Output

1. Validate the subset spec
2. Check actual file size
3. Write to `{original}-subset.json` or user-specified path
4. Report final statistics

## Size Estimation Guidelines

Approximate sizes per element:
- Simple GET endpoint: ~1-2 KB
- POST/PUT with schema: ~2-4 KB
- Complex schema: ~1-3 KB
- Simple schema: ~0.3-0.5 KB

## Common Subset Patterns

### Monitoring/Alerting Focus (~60-80 ops)
- Devices: List, Get, Create, Update, Delete
- Device Groups: List, Get
- Alerts: List, Get, Acknowledge, Add Note
- SDTs: Full CRUD
- Collectors: List, Get

### Asset Management Focus (~40-60 ops)
- Devices: Full CRUD + Properties
- Device Groups: Full CRUD
- Collectors: List, Get
- Netscans: List, Get

### Reporting Focus (~30-40 ops)
- Reports: List, Get, Generate
- Dashboards: List, Get
- Devices: List, Get
- Alerts: List

### Full MSP (~150-180 ops)
- All of above
- Roles and permissions
- API tokens
- Audit logs
- Configuration management

## Output Format

Provide clear before/after comparison:

```
## Subset Complete

### Before
- Size: X KB
- Operations: X
- Schemas: X

### After
- Size: X KB (reduced by Y%)
- Operations: X (reduced by Y%)
- Schemas: X

### Output
Written to: path/to/output.json

### What's Included
[List of operation categories and counts]

### What's Excluded
[List of removed categories]
```
