---
name: document
description: Generate human-readable documentation for a Rewst OpenAPI spec
argument-hint: <path-to-spec.json> [--format md|html]
---

# Document OpenAPI Spec

You are generating human-readable documentation for an OpenAPI specification, optimized for Rewst workflow builders.

## Your Task

Document the spec at: `$ARGUMENTS`

Parse arguments:
- Spec path (required)
- `--format`: Output format - `md` (default) or `html`

## Output Format

Generate documentation that helps Rewst users understand:
1. What operations are available
2. What parameters each operation needs
3. What responses to expect
4. How to use in Rewst workflows

### Markdown Structure

```markdown
# [API Name] - Rewst Integration Reference

> Generated from OpenAPI spec | [X operations] | [Y categories]

## Quick Setup

1. **Get your Bearer token**: [Instructions based on spec]
2. **Configure in Rewst**: Add custom integration with company variable
3. **Test connection**: Use [suggested first operation]

## Operations by Category

### [Tag Name]

#### [Operation Summary]
`[METHOD] [Path]`

[Description]

**Parameters:**
| Name | Location | Required | Type | Description |
|------|----------|----------|------|-------------|
| id | path | Yes | integer | Resource ID |

**Request Body:** (if applicable)
```json
{
  "field": "type - description"
}
```

**Response (200):**
```json
{
  "field": "type - description"
}
```

**Rewst Usage:**
- Workflow step type: [API Request]
- Common use: [Example workflow scenario]

---

## Schemas Reference

### [Schema Name]
[Description]

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| id | integer | - | Unique identifier |

---

## Error Handling

| Code | Meaning | Common Causes |
|------|---------|---------------|
| 400 | Bad Request | Invalid parameters |
| 401 | Unauthorized | Invalid/expired token |
| 404 | Not Found | Resource doesn't exist |
| 429 | Rate Limited | Too many requests |
| 500 | Server Error | API issue |

## Common Workflows

### [Workflow 1 Name]
1. Call [Operation A] to get...
2. Use result in [Operation B]...
3. Handle response...
```

## Documentation Sections

### 1. Overview
- API name and description
- Base URL pattern
- Authentication method
- Total operations count

### 2. Quick Start
- How to get credentials
- Rewst configuration steps
- Suggested first test

### 3. Operations Reference
For each operation:
- Method and path
- Summary and description
- Parameters table
- Request body structure (if POST/PUT/PATCH)
- Response structure
- Rewst-specific usage tips

### 4. Schemas Reference
For each schema:
- Name and description
- Properties table
- Required fields highlighted
- Related operations

### 5. Error Handling
- Standard error codes
- Error response structure
- Troubleshooting tips

### 6. Common Workflows
Suggest 3-5 common automation patterns:
- "Sync devices to documentation system"
- "Auto-acknowledge alerts based on criteria"
- "Generate weekly reports"

## Output

Write documentation to:
- `{spec-name}-docs.md` (Markdown)
- `{spec-name}-docs.html` (HTML with styling)

```
## Documentation Generated

- **Format**: Markdown
- **Output**: api-docs.md
- **Operations documented**: X
- **Schemas documented**: X

### Table of Contents
1. Overview
2. Quick Setup
3. Operations (X total)
   - Category A (X ops)
   - Category B (X ops)
4. Schemas (X total)
5. Error Handling
6. Common Workflows
```

## Tips for Good Documentation

1. **Use plain language** - Avoid jargon where possible
2. **Show examples** - Real parameter values, not just types
3. **Explain "why"** - When would you use this operation?
4. **Link related ops** - "After creating, use GET to verify"
5. **Rewst context** - How does this fit in a workflow?
