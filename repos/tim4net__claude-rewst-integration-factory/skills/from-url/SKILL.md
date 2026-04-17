---
name: from-url
description: Build an OpenAPI spec by analyzing API documentation from a URL
argument-hint: <documentation-url>
---

# Build OpenAPI Spec from Documentation URL

You are creating a Rewst-compatible OpenAPI specification by analyzing API documentation from a website.

## Your Task

Analyze documentation at: `$ARGUMENTS`

If no URL provided, ask for the API documentation URL.

## Process

### Phase 1: Fetch and Analyze

1. Fetch the documentation page
2. Look for:
   - API base URL
   - Authentication method and token acquisition
   - Endpoint listings
   - Request/response examples
   - Rate limiting info
   - Pagination patterns
   - **Logo/Icon** - Look for an SVG logo to use as the integration icon

3. Report findings:

```
## Documentation Analysis

### API Information
- **Name**: [Extracted or inferred]
- **Base URL**: [Found URL pattern]
- **Auth type**: [Bearer/API Key/OAuth/etc.]

### Authentication Details
- **Token type**: [API Key as Bearer / OAuth / Session-based]
- **How to obtain**: [Settings page / OAuth app / Session endpoint]
- **Token expiration**: [Never / X hours / requires refresh]
- **Refresh mechanism**: [None / Refresh token / Re-authenticate]

### Pagination Style
- **Type**: [Offset/Limit / Cursor / Page-based / None found]
- **Parameters**: [offset+limit / cursor / page+per_page]
- **Response fields**: [total, has_more, next_cursor, etc.]

### Rate Limiting
- **Limits**: [X requests per minute/hour]
- **Headers**: [X-RateLimit-* / Retry-After]
- **Behavior**: [429 response / throttling]

### Discovered Endpoints
| Method | Path | Description |
|--------|------|-------------|
| GET | /resources | List all resources |
| POST | /resources | Create resource |
| ... | ... | ... |

### Documentation Quality
- [ ] Has endpoint listings
- [ ] Has request examples
- [ ] Has response examples
- [ ] Has schema definitions
- [ ] Has authentication docs
- [ ] Has pagination docs
- [ ] Has rate limit docs

### Challenges
- [Any issues found: incomplete docs, requires auth to view, etc.]
```

### Phase 2: Gather More Information

If documentation is incomplete, ask user:

1. **Missing base URL**: "What's the API base URL? (e.g., https://api.example.com/v1)"

2. **Missing auth info**: "How do users authenticate? Options:
   - API key used directly as Bearer token (where do they get it?)
   - OAuth app - client credentials flow (where do they create apps?)
   - Session endpoint - call /auth first to get token
   - Something else?"

3. **Token expiration**: "Do tokens expire? If so:
   - How long are they valid?
   - Is there a refresh endpoint or do users re-authenticate?"

4. **Pagination**: "How does pagination work on list endpoints?
   - Offset/limit (offset=0, limit=50)
   - Cursor-based (cursor=abc123)
   - Page numbers (page=1, per_page=25)
   - What fields indicate more pages? (total, has_more, next_cursor)"

5. **Rate limits**: "Are there rate limits?
   - How many requests per minute/hour?
   - What happens when exceeded? (429 response, Retry-After header)"

6. **Partial endpoints**: "I found X endpoints. Are there others you need? Common ones include: [suggestions]"

7. **Missing schemas**: "I couldn't find response schemas. Do you have example responses?"

### Phase 3: Find Logo/Icon

Find an SVG logo for the integration:

1. **Check the docs page** for logo images in the header/footer
2. **Try common paths** on the API's domain:
   - `https://domain.com/favicon.svg`
   - `https://domain.com/logo.svg`
   - `https://domain.com/assets/logo.svg`
   - `https://domain.com/images/logo.svg`

3. **If not found, do a web search** for:
   - `"{company name}" logo SVG`
   - `"{company name}" brand assets`
   - `"{company name}" press kit`

4. **Fetch and save** as `{api-name}-icon.svg` in the integration folder

Always attempt the web search if common paths don't yield an SVG - don't give up early.

### Phase 4: Explore Related Pages

Look for additional documentation:
- Link to "API Reference" or "Endpoints"
- Link to "Authentication"
- Link to "Schemas" or "Models"
- Swagger/OpenAPI spec link (might exist!)
- Postman collection link

If an existing spec is found:
```
Found existing OpenAPI spec at: [URL]
Would you like me to:
1. Use that spec and transform it for Rewst
2. Continue building from documentation
```

### Phase 4: Build Spec Structure

**CRITICAL**: Every operation MUST have both `operationId` AND `summary`. Operations missing both are silently skipped by Rewst. Always generate both fields, even if documentation doesn't provide them:
- `operationId`: Generate from method + path (e.g., `getUsersById`)
- `summary`: Generate from operationId or path description (e.g., "Get User By ID")

Create spec iteratively:

```json
{
  "openapi": "3.0.3",
  "info": {
    "title": "[API Name] - Rewst Integration",
    "description": "Built from documentation at [URL]",
    "version": "1.0.0"
  },
  "servers": [...],
  "security": [{ "BearerAuth": [] }],
  "tags": [...],
  "paths": {...},
  "components": {
    "securitySchemes": {...},
    "schemas": {...}
  }
}
```

### Phase 5: Infer Schemas

From examples in docs, infer schemas:

```
## Schema Inference

### From GET /users response example:
```json
{
  "id": 123,
  "name": "John",
  "email": "john@example.com"
}
```

### Inferred Schema:
```json
{
  "User": {
    "type": "object",
    "properties": {
      "id": { "type": "integer" },
      "name": { "type": "string" },
      "email": { "type": "string", "format": "email" }
    }
  }
}
```

Does this look correct? Any fields to add/modify?
```

### Phase 6: Validate and Output

1. Validate the built spec
2. Check Rewst compatibility
3. Create integration folder and write files:

```
{api-name}/
├── {api-name}-rewst.json    # The OpenAPI spec
├── {api-name}-icon.svg      # Logo/icon if found
└── README.md                # Setup instructions
```

Use lowercase with hyphens for the folder name (e.g., `buy-me-a-coffee/`).

## Handling Common Documentation Patterns

### REST API Docs (typical)
- Endpoints listed with methods
- Parameters documented
- Examples provided

### GraphQL Docs
- Not directly convertible
- Identify common queries
- Build REST-style operations for key queries

### SDK/Library Docs
- Look for underlying REST endpoints
- Map method calls to HTTP operations

### Incomplete Docs
- Start with what's available
- Mark TODOs for missing info
- User can fill in gaps

## Output

```
## Spec Generated from Documentation

### Source
- URL: [documentation URL]
- Scraped: [date]

### What This Plugin Did For You
- **X operations created** with proper operationId and summary (all will appear in Rewst)
- **X schemas inferred** from response examples
- **Authentication configured** for [Bearer/OAuth/etc.]
- **Pagination parameters added** using Rewst's auto-detection patterns
- **Rate limits documented** in API description

### Result
- File: api-name-rewst.json
- Operations: X
- Schemas: X (Y inferred)
- Size: X KB

### Operations Created
| operationId | summary | Method | Path |
|-------------|---------|--------|------|
| listUsers | List Users | GET | /users |
| getUserById | Get User By Id | GET | /users/{id} |
| createUser | Create User | POST | /users |
| ... | ... | ... | ... |

### Confidence Levels
| Section | Confidence | Notes |
|---------|------------|-------|
| Base URL | High | Explicitly documented |
| Auth | Medium | Inferred from examples |
| Endpoints | High | Listed in docs |
| Schemas | Low | Inferred from examples |

### Manual Review Needed
- [ ] Verify inferred schemas match actual API
- [ ] Add missing optional parameters
- [ ] Test with real credentials
- [ ] Add any undocumented endpoints you need
```

## Limitations

This skill works best when:
- Documentation is publicly accessible
- Endpoints are clearly listed
- Examples are provided

It may struggle with:
- Documentation behind login
- Auto-generated docs without examples
- Very large APIs (will suggest subset)
- Non-REST APIs (GraphQL, SOAP)
