# Common Issues and Fixes

This document catalogs common errors encountered when uploading OpenAPI specs to Rewst Custom Integration v2, along with their fixes.

## Error: "schema invalid: missing properties: 'content'"

### Symptoms

Apollo/GraphQL errors in Rewst UI showing:
```
schema invalid: missing properties: 'content'
```

### Causes

1. **Response without content property**
   ```json
   // WRONG
   "responses": {
     "200": {
       "description": "Success",
       "schema": { "type": "object" }
     }
   }
   ```

2. **RequestBody without content property**
   ```json
   // WRONG
   "requestBody": {
     "required": true,
     "schema": { "type": "object" }
   }
   ```

### Fixes

1. **Fix responses**
   ```json
   "responses": {
     "200": {
       "description": "Success",
       "content": {
         "application/json": {
           "schema": { "type": "object" }
         }
       }
     }
   }
   ```

2. **Fix requestBody**
   ```json
   "requestBody": {
     "required": true,
     "content": {
       "application/json": {
         "schema": { "type": "object" }
       }
     }
   }
   ```

### Bulk Fix Script

To find all occurrences in a JSON spec:
```bash
# Find responses missing content
grep -n '"responses"' spec.json | head -20

# Find requestBody missing content
grep -n '"requestBody"' spec.json | head -20
```

---

## Error: "schema invalid: missing properties: '$ref'"

### Symptoms

```
schema invalid: missing properties: '$ref'
```

### Causes

1. **Incomplete $ref object**
   ```json
   // WRONG - empty or partial ref
   { "$ref": }
   ```

2. **Schema expects $ref but has inline definition**
   ```json
   // Sometimes Rewst expects references, not inline
   "schema": {
     "type": "object",
     "properties": { ... }
   }
   ```

### Fixes

1. **Complete the reference**
   ```json
   { "$ref": "#/components/schemas/ModelName" }
   ```

2. **Extract inline schemas to components**
   ```json
   // Move to components/schemas/InlineModel
   // Then reference it
   { "$ref": "#/components/schemas/InlineModel" }
   ```

---

## Error: "component '#/components/schemas/X' does not exist"

### Symptoms

```
component '#/components/schemas/Debug' does not exist
```

### Causes

A `$ref` points to a schema that isn't defined.

### Fixes

1. **Add the missing schema**
   ```json
   "components": {
     "schemas": {
       "Debug": {
         "type": "object",
         "properties": { ... }
       }
     }
   }
   ```

2. **Remove references to unused schemas**
   - Delete paths that reference the missing schema
   - Or replace with a generic schema

3. **Fix typos in references**
   - `#/components/schemas/Devcie` → `#/components/schemas/Device`

---

## Error: "default" property type must be integer

### Symptoms

Spectral validation shows:
```
"default" property type must be integer
```

### Causes

Default value is a string when schema type is integer/number:
```json
{
  "type": "integer",
  "default": "0"  // String, not integer
}
```

### Fixes

Match default type to schema type:
```json
{
  "type": "integer",
  "default": 0  // Integer, not string
}
```

### Bulk Fix

Search for patterns like:
```bash
grep -n '"default": "0"' spec.json
grep -n '"default": "1"' spec.json
grep -n '"default": "true"' spec.json
grep -n '"default": "false"' spec.json
```

---

## Error: Operation tags not defined

### Symptoms

Spectral shows:
```
Operation tags must be defined in global tags
```

### Causes

Operation uses a tag not in the global `tags` array:
```json
{
  "tags": [
    { "name": "Devices" }
  ],
  "paths": {
    "/alerts": {
      "get": {
        "tags": ["Alerts"]  // Not defined globally
      }
    }
  }
}
```

### Fixes

Add missing tags to global array:
```json
{
  "tags": [
    { "name": "Devices", "description": "Device operations" },
    { "name": "Alerts", "description": "Alert operations" }
  ]
}
```

### Find Missing Tags

```bash
# List all tags used in operations
grep -o '"tags": \["[^"]*"\]' spec.json | sort -u

# Compare with global tags definition
```

---

## Error: File too large

### Symptoms

Rewst rejects the upload or shows size-related errors.

### Causes

Spec exceeds ~500KB limit.

### Fixes

1. **Remove unused paths**
   - Identify least-used operations
   - Remove entire path entries

2. **Remove unused schemas**
   - After removing paths, schemas may be orphaned
   - Remove schemas not referenced by remaining paths

3. **Minimize descriptions**
   - Shorten overly verbose descriptions
   - Remove redundant documentation

4. **Remove examples**
   - Response examples can be large
   - Remove if not essential

### Size Reduction Priority

1. Remove paths (biggest impact)
2. Remove orphaned schemas
3. Shorten descriptions
4. Remove examples
5. Minify JSON (remove whitespace)

---

## Error: Invalid OpenAPI version

### Symptoms

Parser errors about version or swagger field.

### Causes

Missing or malformed `openapi` field.

### Note on Version Support

Rewst CI v2 supports:
- **OpenAPI 3.0.x** - Fully supported
- **OpenAPI 3.1.x** - Fully supported
- **Swagger 2.0** - Auto-converted to OpenAPI 3.0 on upload

### Fixes

Ensure the `openapi` field is present and valid:
```json
{
  "openapi": "3.0.3",
  ...
}
```

---

## Error: Security scheme not recognized

### Symptoms

Authentication doesn't work in Rewst even though spec loads.

### Causes

Using unsupported auth type:
```json
"securityDefinitions": {
  "LMv1": {
    "type": "apiKey",
    "in": "header",
    "name": "Authorization"
  }
}
```

### Fixes

Convert to Bearer token:
```json
"securityDefinitions": {
  "BearerAuth": {
    "type": "http",
    "scheme": "bearer"
  }
}
```

Update security references throughout spec.

---

## Issue: Operations silently skipped

### Symptoms

Some operations don't appear in Rewst after upload.

### Causes

1. **Missing both summary and operationId**
   ```json
   // SKIPPED - no summary OR operationId
   "/users": {
     "get": {
       "description": "Get users",
       "responses": { ... }
     }
   }
   ```

2. **Operation marked deprecated**
   ```json
   "/old-endpoint": {
     "get": {
       "deprecated": true,  // SKIPPED
       "operationId": "oldEndpoint"
     }
   }
   ```

### Fixes

Add at least `summary` or `operationId` to every operation:
```json
"/users": {
  "get": {
    "operationId": "getUsers",
    "summary": "List all users",
    "responses": { ... }
  }
}
```

---

## Issue: Cookie parameters ignored

### Symptoms

Cookie parameters defined in spec don't appear in Rewst actions.

### Causes

Rewst doesn't support `in: cookie` parameters:
```json
{
  "name": "session_id",
  "in": "cookie",  // NOT SUPPORTED
  "schema": { "type": "string" }
}
```

### Fixes

Convert to header parameters if possible:
```json
{
  "name": "Cookie",
  "in": "header",
  "schema": { "type": "string" },
  "description": "Session cookie in format: session_id=value"
}
```

Or document that cookies must be passed via the generic request action.

---

## Error: Pack name already exists

### Symptoms

```
This name is already in use by a pack in your organization.
```

### Causes

The `info.title` value (minus " API" suffix) matches an existing pack name.

### Fixes

1. **Use a unique title**
   ```json
   "info": {
     "title": "Acme API v2"  // Different from existing "Acme API"
   }
   ```

2. **Delete the existing pack first** if you're replacing it

---

## Issue: Reserved parameter names conflict

### Symptoms

Parameters behave unexpectedly or cause errors.

### Causes

Using reserved parameter names that conflict with Rewst internals:

```
json_body, body, json, type, url_path, method, results_key, results, data,
output_schema, path_params, query_params, headers, cookies, required, parameters,
response, id, name, parent, parent_id, filter, filters, filter_key, filter_keys,
filter_value, filter_values, query, queries, q
```

### Fixes

Rename parameters to avoid conflicts:
- `id` → `resource_id` or `item_id`
- `name` → `resource_name`
- `filter` → `filter_expression`
- `query` → `search_query`

---

## Debugging Tips

### Validate Before Upload

Always run validators before uploading:

```bash
# Quick validation
npx @apidevtools/swagger-parser validate spec.json

# Comprehensive linting
npx @stoplight/spectral-cli lint spec.json
```

### Check JSON Syntax

```bash
# Verify valid JSON
python -m json.tool spec.json > /dev/null && echo "Valid JSON"
```

### Count Operations

```bash
# Count paths and operations
grep -c '"get"\|"post"\|"put"\|"patch"\|"delete"' spec.json
```

### Check File Size

```bash
ls -lh spec.json
```
