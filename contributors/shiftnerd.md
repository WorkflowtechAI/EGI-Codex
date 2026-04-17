# shiftnerd

**GitHub:** <https://github.com/shiftnerd>
**Web app:** <https://schemadoctor.com>

## Why he matters to EGI

Integration-import pain is the single biggest wall when onboarding a new SaaS into a Rewst workflow. OpenAPI specs in the wild are often broken, oversized, or missing `operationId`s, all of which Rewst's custom integration importer chokes on. shiftnerd has built both a schema library and a web app specifically to solve this.

## Published content

### `shiftnerd/OpenAPISchemas` (3 stars, active 2026-03)
A collection of OpenAPI schemas gathered from various sources and modified to work cleanly with Rewst's custom integration importer. Not original creations, but fixed and reduced versions of upstream specs.

Worth checking first whenever you need to add a new SaaS integration to Rewst, before hand-fixing a vendor's raw spec.

### `shiftnerd/schemadoctor` (3 stars, active 2026-03)
The web app at `schemadoctor.com`. Runs entirely client-side (no backend). Features:

- Load an OpenAPI schema from URL (JSON or YAML), file upload, or paste.
- Convert YAML to JSON.
- Validate JSON schema.
- Beautify.
- Set default descriptions for empty description fields (Rewst's importer requires these).
- Add missing `operationId` values (Rewst requires these too).
- Fix paths ending with a trailing slash.
- Handle circular references.
- Filter endpoints by HTTP method, path pattern (regex), or tag, then generate a reduced schema.
- Dark/light theme, change log of modifications.

The "generate reduced schema" flow is the killer feature. Most vendor OpenAPI specs are 10+ MB and Rewst's importer either chokes or imports hundreds of operations you do not need. schemadoctor lets you cut it down to the 5-10 endpoints you actually care about.

## Recommended next actions

1. Bookmark `schemadoctor.com`.
2. Check `shiftnerd/OpenAPISchemas` before fighting with any vendor's OpenAPI spec.
3. If a schema fix is not in the repo, do it in schemadoctor, save the result back, and PR it upstream.
