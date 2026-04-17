---
name: validate
description: Check if an OpenAPI spec will work with Rewst (uses Rewst's exact validation rules)
argument-hint: <spec.json>
---

# Validate OpenAPI Spec for Rewst

You're checking if an OpenAPI spec is ready to upload to Rewst Custom Integration v2.

## Your Task

Validate: `$ARGUMENTS`

If no file provided, ask which file to check.

## How to Validate

### Option 1: Use the Linter (Recommended)

Run the included linter which uses **Rewst's exact validation rules**:

```bash
node tools/rewst-lint.js <spec-file>
```

This runs the same Spectral ruleset that Rewst uses internally. If it passes, the spec will work in Rewst.

### Option 2: Manual Checks

If the linter isn't available, check these items:

#### Must Pass (errors)
- [ ] Valid JSON
- [ ] Has `openapi` field (must be 3.0.x)
- [ ] Has `info.title`
- [ ] File size under ~500KB

#### Should Pass (Spectral rules that are still enabled)
- [ ] Valid OpenAPI 3.0 structure
- [ ] All `$ref` references resolve
- [ ] Required fields present in schemas

#### Not Checked by Rewst (these rules are disabled)
Rewst is permissive about these - they won't cause upload failures:
- Operation descriptions (not required)
- Parameter descriptions (not required)
- Examples (not required)
- Unused components (allowed)
- Path naming conventions (flexible)
- Response definitions (flexible)
- Tags (not required to be defined)

## Output

Provide a clear, friendly report:

```
## Validation: [filename]

**Result:** ✓ Ready for Rewst / ✗ Needs fixes

**Size:** X KB (limit: ~500KB)

### Issues to Fix
[List any errors that will prevent upload]

### Warnings (Optional)
[List any warnings - these won't prevent upload but might be worth fixing]

### Next Steps
[What to do - either "Upload to Rewst" or "Run /rewst-openapi:fix to repair"]
```

## Common Issues and What They Mean

**"The OpenAPI document must have an openapi field"**
→ The file might be Swagger 2.0 (which uses `swagger` instead of `openapi`). Use `/rewst-openapi:transform` to convert it.

**"The OpenAPI document must have a title"**
→ Add `info.title` to your spec. This becomes the integration name in Rewst.

**File too large**
→ Use `/rewst-openapi:subset` to select which operations to keep.

## Reference

The linter uses Rewst's exact Spectral ruleset from:
`tools/.spectral-rewst.yaml`

See also:
- @knowledge/rewst-requirements.md
- @knowledge/common-issues.md
