---
name: transform
description: Transform an existing OpenAPI spec to be Rewst Custom Integration v2 compatible
argument-hint: <path-to-spec.json> [output-path.json]
---

# Transform OpenAPI Spec for Rewst

You are transforming an existing OpenAPI specification to be compatible with Rewst Custom Integration v2.

## Your Task

Transform the spec at: `$ARGUMENTS`

Parse the arguments:
- First argument: source spec path (required)
- Second argument: output path (optional, defaults to `{source}-rewst.json`)

## Transformation Process

### Phase 1: Analysis

First, analyze the source spec:
1. Read and validate the JSON
2. Check current OpenAPI version
3. Identify authentication method and token patterns
4. Detect pagination style (offset/limit, cursor, page-based)
5. Find session/token endpoints (auth, refresh)
6. Note rate limiting documentation
7. Count operations and estimate output size
8. Report findings before proceeding

Report on API patterns found:
```
### API Patterns Detected
- **Auth**: [API Key / OAuth / Session-based / Other]
- **Token endpoints**: [/auth/token, /auth/refresh, etc.]
- **Pagination**: [offset/limit / cursor / page-based / none]
- **Rate limits**: [documented / not found]
```

### Phase 2: Version Conversion

If needed:
- **Swagger 2.0** → Convert to OpenAPI 3.0.3
  - Move `definitions` to `components/schemas`
  - Convert `securityDefinitions` to `components/securitySchemes`
  - Update response structure
  - Update parameter structure

- **OpenAPI 3.1.x** → Downgrade to OpenAPI 3.0.3
  - Remove webhooks
  - Adjust JSON Schema differences
  - Update version field

### Phase 3: Authentication Conversion

Convert to Bearer token auth:

1. Add/update security scheme:
```json
"components": {
  "securitySchemes": {
    "BearerAuth": {
      "type": "http",
      "scheme": "bearer",
      "description": "Bearer token authentication"
    }
  }
}
```

2. Set global security:
```json
"security": [{ "BearerAuth": [] }]
```

3. Remove operation-level security that uses other schemes

4. **Preserve auth/session endpoints** - Keep any endpoints for:
   - Token creation (POST /auth, /oauth/token, /session)
   - Token refresh (POST /auth/refresh, /oauth/token with grant_type=refresh_token)
   - Mark these with `"security": []` so they can be called without a token

5. **Document token acquisition** in info.description:
```json
"info": {
  "description": "## Authentication\n\n[How to obtain Bearer token from original auth method]"
}
```

### Phase 4: Structure Fixes

Fix all structural issues:

#### Fix 0: Missing operationId/summary (CRITICAL)

Operations without BOTH `operationId` AND `summary` are silently skipped by Rewst. Generate missing values:

```json
// If missing operationId, generate from method + path:
// GET /users/{id} → "getUsersById"
// POST /orders → "postOrders"

// If missing summary, generate from operationId or path:
// operationId: "getUsersById" → summary: "Get Users By Id"
// path: /users/{id} + method: GET → summary: "Get User"
```

Ensure every operation ends up with both fields.

1. **Responses** - Ensure content property:
```json
"responses": {
  "200": {
    "description": "Success",
    "content": {
      "application/json": {
        "schema": { ... }
      }
    }
  }
}
```

2. **RequestBody** - Ensure content property:
```json
"requestBody": {
  "required": true,
  "content": {
    "application/json": {
      "schema": { ... }
    }
  }
}
```

3. **Default values** - Fix type mismatches:
   - `"default": "0"` → `"default": 0` for integers
   - `"default": "true"` → `"default": true` for booleans

### Phase 5: Tags

1. Collect all tags used in operations
2. Create global tags array with all used tags
3. Add descriptions to tags where possible

### Phase 6: Size Optimization

If spec exceeds 450KB:

1. **Ask user** which operations are most important
2. **Suggest** operations to remove (least common use cases)
3. **Remove** orphaned schemas after path removal
4. **Trim** verbose descriptions
5. **Remove** examples if needed

**Preserve critical patterns:**
- Keep auth/session/refresh endpoints
- Keep pagination parameters on list endpoints
- Keep rate limit documentation in descriptions

### Phase 7: Validation

Before writing output:
1. Validate JSON syntax
2. Check all $ref references resolve
3. Verify size is under 500KB
4. Run through validation checklist

## Output

Create a dedicated folder for the integration:

```
{api-name}/
├── {api-name}-rewst.json    # The transformed spec
├── {api-name}-icon.svg      # Logo/icon if available (copy from source or fetch)
└── README.md                # Transformation notes and setup instructions
```

Use lowercase with hyphens for the folder name (e.g., `acme-api/`).

### Deliverables

1. Create the integration folder
2. Write transformed spec to the folder
3. **Find and save the logo/icon** (SVG preferred):
   - Check common paths: `/favicon.svg`, `/logo.svg`, `/assets/logo.svg`
   - If not found, **do a web search** for `"{API name}" logo SVG` or brand assets
   - Save as `{api-name}-icon.svg`
4. Write README.md with:
   - Summary of transformations applied
   - How to obtain Bearer token
   - List of available operations
   - Any manual steps needed

### Output Summary

Include a clear summary showing the plugin's value:

```
## Transformation Complete

### What This Plugin Fixed
- **X operations saved**: Generated missing operationId/summary (would have been silently skipped!)
- Converted authentication from [original] to Bearer token
- Fixed N response content structures
- Fixed N requestBody content structures
- Added N missing global tags

### Auto-Generated Fields
| Path | Method | Generated operationId | Generated summary |
|------|--------|----------------------|-------------------|
| /users | GET | listUsers | List Users |
| ... | ... | ... | ... |

### Result
- Operations: X (all will appear in Rewst)
- Size: X KB (under 500KB limit)
- Ready for upload!
```

## Reference

See the knowledge files:
- @knowledge/rewst-requirements.md - Full requirements
- @knowledge/common-issues.md - Common fixes
- @knowledge/examples/minimal-spec.json - Reference template
