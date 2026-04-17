# Rewst Custom Integration v2 - OpenAPI Requirements

This document captures all requirements for OpenAPI specs to work with Rewst Custom Integration v2.

## Size Limits

- **Maximum file size**: ~500KB
- **Practical limit**: 450-480KB to leave headroom
- If your source spec is larger, you must subset the operations

## OpenAPI Version

- **Supported**: OpenAPI 3.0.x, 3.1.x, and Swagger 2.0
- Swagger 2.0 specs are auto-converted to OpenAPI 3.0 on upload
- OpenAPI 3.1.x is fully supported

## Required Fields

### Mandatory
- `openapi` - Version field (e.g., "3.0.3")
- `info.title` - Used as pack name, **must be unique within the organization**

### Recommended
- `info.description` - Truncated to 250 characters
- `info.version` - Defaults to "0.0.1" if missing
- `servers[0].url` - Used as default hostname configuration

## Authentication

Rewst CI v2 auto-detects authentication from `components.securitySchemes`:

### Supported Auth Types

**1. Bearer Token (Recommended)**
```json
"securitySchemes": {
  "BearerAuth": {
    "type": "http",
    "scheme": "bearer"
  }
}
```
→ Sets `authentication_method: api_key`, `api_key_header_name: Authorization`, `api_key_preamble: Bearer`

**2. API Key in Header**
```json
"securitySchemes": {
  "ApiKeyAuth": {
    "type": "apiKey",
    "in": "header",
    "name": "X-API-Key"
  }
}
```
→ Sets `authentication_method: api_key`, `api_key_header_name` from `name` field

**3. Basic Auth**
```json
"securitySchemes": {
  "BasicAuth": {
    "type": "http",
    "scheme": "basic"
  }
}
```
→ Sets `authentication_method: basic_auth`

**4. OAuth2 (Client Credentials or Authorization Code)**
```json
"securitySchemes": {
  "OAuth2": {
    "type": "oauth2",
    "flows": {
      "clientCredentials": {
        "tokenUrl": "https://example.com/oauth/token",
        "scopes": {}
      }
    }
  }
}
```
→ Auto-configures OAuth2 settings (clientCredentials takes precedence if both flows present)

### NOT Supported
- `openIdConnect` scheme type (skipped with warning)
- HMAC/signature-based auth (AWS Signature v4, LMv1, etc.)

## Server URL Configuration

Use server variables for multi-tenant APIs:

```json
"servers": [
  {
    "url": "https://{company}.example.com/api",
    "variables": {
      "company": {
        "default": "your-company",
        "description": "Your company subdomain"
      }
    }
  }
]
```

## Response Structure

**CRITICAL**: Every response MUST have a `content` property with proper structure.

### Response Selection Priority

Rewst extracts the output schema from responses in this order:
1. `200` response (preferred)
2. `2XX` response
3. First response in 200-299 range
4. `default` response (fallback)

### Correct Structure

```json
"responses": {
  "200": {
    "description": "Success",
    "content": {
      "application/json": {
        "schema": {
          "$ref": "#/components/schemas/ResponseModel"
        }
      }
    }
  }
}
```

### Common Mistake - Missing content

```json
"responses": {
  "200": {
    "description": "Success",
    "schema": { ... }  // WRONG - this is OpenAPI 2.0 style
  }
}
```

### Error Responses

Include standard error responses:

```json
"responses": {
  "400": {
    "description": "Bad Request",
    "content": {
      "application/json": {
        "schema": { "$ref": "#/components/schemas/ErrorResponse" }
      }
    }
  },
  "401": {
    "description": "Unauthorized",
    "content": {
      "application/json": {
        "schema": { "$ref": "#/components/schemas/ErrorResponse" }
      }
    }
  },
  "404": {
    "description": "Not Found",
    "content": {
      "application/json": {
        "schema": { "$ref": "#/components/schemas/ErrorResponse" }
      }
    }
  },
  "500": {
    "description": "Internal Server Error",
    "content": {
      "application/json": {
        "schema": { "$ref": "#/components/schemas/ErrorResponse" }
      }
    }
  }
}
```

## RequestBody Structure

**CRITICAL**: Every requestBody MUST have a `content` property.

### Supported Media Types

| Media Type | Mapped To |
|------------|-----------|
| `application/json` | `json_body` parameter |
| `application/x-www-form-urlencoded` | `body` parameter |
| `multipart/form-data` | `body` parameter |
| Other types | ❌ Ignored |

### Correct Structure

```json
"requestBody": {
  "required": true,
  "content": {
    "application/json": {
      "schema": {
        "$ref": "#/components/schemas/CreateRequest"
      }
    }
  }
}
```

### Common Mistake - Missing content

```json
"requestBody": {
  "required": true,
  "schema": { ... }  // WRONG - must be inside content
}
```

## Schema Default Values

**CRITICAL**: Default values MUST match the schema type exactly.

### Type Matching Rules

| Schema Type | Default Value | Correct | Wrong |
|-------------|---------------|---------|-------|
| `integer` | `0` | `"default": 0` | `"default": "0"` |
| `number` | `0.0` | `"default": 0.0` | `"default": "0.0"` |
| `boolean` | `false` | `"default": false` | `"default": "false"` |
| `string` | `"value"` | `"default": "value"` | `"default": 0` |

### Example Fix

```json
// WRONG
{
  "type": "integer",
  "default": "0"
}

// CORRECT
{
  "type": "integer",
  "default": 0
}
```

## Tags

**CRITICAL**: All tags used in operations MUST be defined in the global `tags` array.

### Define Tags Globally

```json
{
  "tags": [
    { "name": "Devices", "description": "Device management operations" },
    { "name": "Alerts", "description": "Alert management operations" }
  ]
}
```

### Use in Operations

```json
{
  "paths": {
    "/devices": {
      "get": {
        "tags": ["Devices"],  // Must match a global tag
        "operationId": "getDevices"
      }
    }
  }
}
```

## $ref References

All `$ref` references must resolve to existing components.

### Valid Reference

```json
{ "$ref": "#/components/schemas/Device" }
```

The schema `Device` MUST exist in `components.schemas.Device`.

### Common Issues

1. **Typos**: `#/components/schemas/Devcie` (misspelled)
2. **Missing schemas**: Referenced but never defined
3. **Wrong path**: `#/definitions/Device` (OpenAPI 2.0 style)

## Operation Requirements

### Minimum Requirements

Each operation MUST have at least one of:
- `operationId` - Unique identifier
- `summary` - Short description

Operations are **skipped** if both are missing. Operations with `deprecated: true` are also skipped.

### Supported HTTP Methods

`get`, `post`, `put`, `delete`, `patch`, `head`, `options`, `trace`

### Field Processing

| Field | Limit | Notes |
|-------|-------|-------|
| `summary` → action name | 100 chars | Title-cased |
| `description` | 500 chars | HTML stripped |
| `operationId` → action ref | - | Converted to snake_case |
| `tags[0]` → category | - | Title-cased, defaults to "Default" |

### Example Operation

```json
{
  "get": {
    "operationId": "getDeviceById",
    "summary": "Get device by ID",
    "description": "Retrieves a single device by its unique identifier",
    "tags": ["Devices"],
    "parameters": [...],
    "responses": {...}
  }
}
```

## Parameter Requirements

### Supported Parameter Locations

| Location | Mapped To | Notes |
|----------|-----------|-------|
| `path` | `path_params` | Auto-marked as required |
| `query` | `query_params` | Standard query string |
| `header` | `headers` | Custom headers |
| `cookie` | ❌ NOT SUPPORTED | Will be ignored |

### Path Parameters

Must be marked as `required: true`:

```json
{
  "name": "id",
  "in": "path",
  "required": true,
  "schema": {
    "type": "integer"
  },
  "description": "Device ID"
}
```

### Query Parameters

Include descriptions and defaults where appropriate:

```json
{
  "name": "limit",
  "in": "query",
  "required": false,
  "schema": {
    "type": "integer",
    "default": 50,
    "minimum": 1,
    "maximum": 1000
  },
  "description": "Maximum number of results to return"
}
```

### Forbidden Parameter Names

These names are reserved and cannot be used as parameter names or aliases:

```
json_body, body, json, type, url_path, method, results_key, results, data,
output_schema, path_params, query_params, headers, cookies, required, parameters,
response, id, name, parent, parent_id, filter, filters, filter_key, filter_keys,
filter_value, filter_values, query, queries, q
```

If your API uses these names, they will conflict with Rewst internals.

## Validation Checklist

Before uploading to Rewst, verify:

### Required
- [ ] Valid JSON/YAML syntax
- [ ] Has `openapi` version field
- [ ] Has `info.title` (will be pack name - must be unique in org)
- [ ] Every operation has `summary` OR `operationId` (at least one)

### Structure
- [ ] All responses have `content` property (OpenAPI 3.0 style)
- [ ] All requestBody entries have `content` property
- [ ] All default values match their schema types
- [ ] All $ref references resolve to existing schemas
- [ ] All path parameters have `required: true`

### Recommended
- [ ] File size under 500KB
- [ ] Has `servers[0].url` for hostname config
- [ ] Has `info.version` (or defaults to 0.0.1)
- [ ] Has `info.description` (truncated to 250 chars)
- [ ] Uses supported auth scheme (bearer, apiKey, basic, oauth2)
- [ ] No cookie parameters (not supported)
- [ ] No forbidden parameter names (see list above)
- [ ] Pagination params use standard names for auto-detection
- [ ] Response arrays use standard names (data, items, results, etc.)

## Pagination Patterns

Rewst **auto-detects** pagination parameters by matching against known naming patterns. Use these names for automatic detection.

### Page Size Parameters (Auto-Detected)

Names that trigger page size detection:
- `limit`, `max`, `size`, `count`
- `page[size]`, `page[limit]`
- Patterns: `maxSize`, `pageSize`, `batchSize`, `maximumItems`, etc.

```json
{
  "name": "limit",
  "in": "query",
  "schema": { "type": "integer", "default": 50, "maximum": 1000 },
  "description": "Maximum items to return"
}
```

### Offset/Index Parameters (Auto-Detected)

Names: `offset`, `index`, `next`, `after`, `start`, `skip`, `from`, `begin`

```json
{
  "name": "offset",
  "in": "query",
  "schema": { "type": "integer", "default": 0 },
  "description": "Number of items to skip"
}
```

### Cursor/Pointer Parameters (Auto-Detected)

Names: `pointer`, `cursor`, `bookmark`, `after`, `continuationToken`, `nextToken`

```json
{
  "name": "cursor",
  "in": "query",
  "schema": { "type": "string" },
  "description": "Pagination cursor from previous response"
}
```

### Page Number Parameters (Auto-Detected)

Names: `page`, `pagenumber`, `page_number`

```json
{
  "name": "page",
  "in": "query",
  "schema": { "type": "integer", "default": 1, "minimum": 1 },
  "description": "Page number"
}
```

### Results Key (Auto-Detected from Response Schema)

Rewst looks for these property names in response schemas to identify the results array:
- `results`, `data`, `items`, `records`, `entities`, `objects`, `values`

```json
{
  "PaginatedResponse": {
    "type": "object",
    "properties": {
      "data": { "type": "array", "items": { "$ref": "#/components/schemas/Item" } },
      "total": { "type": "integer" },
      "has_more": { "type": "boolean" }
    }
  }
}
```

## Token and Session Management

### Bearer Token from OAuth

If the API uses OAuth 2.0, users obtain tokens outside of Rewst. Document how:

```json
{
  "info": {
    "description": "## Authentication\n\nThis API uses Bearer tokens. To obtain a token:\n1. Create an OAuth app at https://example.com/settings/oauth\n2. Use client credentials flow to get access token\n3. Tokens expire after 1 hour - refresh as needed"
  }
}
```

### API Key to Bearer Conversion

Some APIs accept API keys as Bearer tokens. Document the pattern:

```json
{
  "components": {
    "securitySchemes": {
      "BearerAuth": {
        "type": "http",
        "scheme": "bearer",
        "description": "Use your API key as the Bearer token. Get your key from Settings > API Keys."
      }
    }
  }
}
```

### Session Tokens

For APIs requiring session creation first:

```json
{
  "paths": {
    "/auth/session": {
      "post": {
        "operationId": "createSession",
        "summary": "Create authentication session",
        "description": "Creates a session token. Use the returned token as Bearer auth for subsequent requests. Sessions expire after 24 hours.",
        "security": [],
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "type": "object",
                "required": ["api_key", "api_secret"],
                "properties": {
                  "api_key": { "type": "string" },
                  "api_secret": { "type": "string" }
                }
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Session created",
            "content": {
              "application/json": {
                "schema": {
                  "type": "object",
                  "properties": {
                    "session_token": { "type": "string" },
                    "expires_at": { "type": "string", "format": "date-time" }
                  }
                }
              }
            }
          }
        }
      }
    }
  }
}
```

### Token Refresh

If tokens expire, include a refresh endpoint:

```json
{
  "/auth/refresh": {
    "post": {
      "operationId": "refreshToken",
      "summary": "Refresh access token",
      "description": "Exchange a refresh token for a new access token",
      "security": [],
      "requestBody": {
        "content": {
          "application/json": {
            "schema": {
              "type": "object",
              "required": ["refresh_token"],
              "properties": {
                "refresh_token": { "type": "string" }
              }
            }
          }
        }
      },
      "responses": {
        "200": {
          "description": "New tokens",
          "content": {
            "application/json": {
              "schema": {
                "type": "object",
                "properties": {
                  "access_token": { "type": "string" },
                  "refresh_token": { "type": "string" },
                  "expires_in": { "type": "integer", "description": "Seconds until expiration" }
                }
              }
            }
          }
        }
      }
    }
  }
}
```

## Rate Limiting

Document rate limits in the spec so Rewst users know the constraints.

### In API Description

```json
{
  "info": {
    "description": "## Rate Limits\n\n- 100 requests per minute per API key\n- 429 responses include Retry-After header\n- Bulk endpoints count as multiple requests"
  }
}
```

### Rate Limit Response Headers

Document expected headers:

```json
{
  "responses": {
    "429": {
      "description": "Rate limit exceeded",
      "headers": {
        "Retry-After": {
          "description": "Seconds to wait before retrying",
          "schema": { "type": "integer" }
        },
        "X-RateLimit-Remaining": {
          "description": "Requests remaining in current window",
          "schema": { "type": "integer" }
        }
      },
      "content": {
        "application/json": {
          "schema": { "$ref": "#/components/schemas/ErrorResponse" }
        }
      }
    }
  }
}
```

## Recommended Validators

1. **swagger-parser** (Node.js): Basic OpenAPI validation
   ```bash
   npx @apidevtools/swagger-parser validate spec.json
   ```

2. **Spectral** (Node.js): Comprehensive linting
   ```bash
   npx @stoplight/spectral-cli lint spec.json
   ```

3. **Online**: https://editor.swagger.io/ (paste spec to validate)
