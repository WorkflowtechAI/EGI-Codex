---
name: merge
description: Combine multiple OpenAPI specs into one Rewst-compatible spec
argument-hint: <spec1.json> <spec2.json> [spec3.json...] -o <output.json>
---

# Merge OpenAPI Specs for Rewst

You are combining multiple OpenAPI specifications into a single Rewst-compatible spec.

## Your Task

Merge specs from: `$ARGUMENTS`

Parse arguments:
- Input specs: all .json files before `-o` flag
- Output path: file after `-o` flag (default: `merged-rewst.json`)

## Use Cases

- **Multi-service APIs**: Some vendors split specs by service area
- **Version combining**: Merge v1 and v2 endpoints
- **Custom + vendor**: Add custom endpoints to vendor spec

## Process

### Phase 1: Load and Analyze

For each input spec, report:

```
## Input Specs Analysis

### Spec 1: [filename]
- Operations: X
- Schemas: X
- Size: X KB
- Auth: [type]
- Tags: [list]

### Spec 2: [filename]
- Operations: X
- Schemas: X
- Size: X KB
- Auth: [type]
- Tags: [list]

### Combined Estimate
- Total operations: X
- Total schemas: X
- Estimated size: X KB
- Size limit: 500 KB
- Status: OK / OVER LIMIT
```

### Phase 2: Conflict Detection

Identify conflicts:

```
## Potential Conflicts

### Path Conflicts
Operations defined in multiple specs:
- GET /devices - defined in spec1.json AND spec2.json

### Schema Conflicts
Schemas with same name but different definitions:
- Device schema differs between spec1 and spec2

### Tag Conflicts
- Same tag name, different descriptions
```

### Phase 3: Resolve Conflicts

For each conflict, ask user or apply rules:

**Path conflicts:**
- Ask user which version to keep
- Or keep both with modified operationIds

**Schema conflicts:**
- If identical: keep one
- If different: ask user, or rename one (e.g., `DeviceV1`, `DeviceV2`)

**Tag conflicts:**
- Merge descriptions
- Or ask user for preferred description

### Phase 4: Merge

Create merged spec:

1. **Base structure** from first spec:
   - `openapi`: Use highest 3.0.x version
   - `info`: Combine titles, use merged description
   - `servers`: Merge unique servers

2. **Security**:
   - Prefer Bearer token auth
   - Warn if specs use different auth

3. **Tags**:
   - Combine all unique tags
   - Resolve description conflicts

4. **Paths**:
   - Add all paths from all specs
   - Handle conflicts per user preference

5. **Schemas**:
   - Add all schemas
   - Rename conflicting schemas
   - Update $refs to renamed schemas

### Phase 5: Post-Merge Fixes

Apply standard Rewst fixes:
- Ensure response content structure
- Ensure requestBody content structure
- Fix default value types
- Verify all $refs resolve

### Phase 6: Size Check

If over limit:
```
## Size Warning

Merged spec is X KB (limit: 500 KB)

Options:
1. Run /rewst-openapi:subset on the merged spec
2. Remove specific operations now
3. Merge fewer specs

Which would you prefer?
```

### Phase 7: Output

Write merged spec and report:

```
## Merge Complete

### Sources
- spec1.json: X operations, X schemas
- spec2.json: X operations, X schemas

### Result
- File: merged-rewst.json
- Size: X KB
- Operations: X
- Schemas: X

### Conflicts Resolved
- X path conflicts (kept from: [source])
- X schema renames (OldName → NewName)

### Validation
- JSON: Valid
- OpenAPI: Valid
- Rewst compatible: Yes/No
```

## Merge Strategies

### Conservative (default)
- Keep first definition for conflicts
- Preserve all operations
- Warn about duplicates

### Additive
- Keep all operations, rename duplicates
- Suffix with source: `getDevice_spec1`, `getDevice_spec2`
- Useful for truly different endpoints

### Latest Wins
- Later specs override earlier ones
- Useful for version upgrades
- Spec order matters

## Example Usage

```bash
# Merge two specs
/rewst-openapi:merge api-v1.json api-v2.json -o combined.json

# Merge three specs with specific output
/rewst-openapi:merge devices.json alerts.json reports.json -o full-api.json
```
