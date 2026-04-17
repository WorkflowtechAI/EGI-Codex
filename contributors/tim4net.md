# tim4net

**Real name:** Not publicly stated. Distinct from Tim Fournet.
**GitHub:** <https://github.com/tim4net>

## Why he matters to EGI

tim4net is producing some of the most directly-useful Rewst adjacent tooling outside Rewst itself: a Claude Code plugin for generating Rewst-compatible OpenAPI specs, and a hand-curated Rewst-optimized LogicMonitor spec. That is exactly the kind of work that sits one layer up from what Rewst ships, and overlaps precisely with what EGI wants to build.

## Published content

### `tim4net/claude-rewst-integration-factory` (2026-01)
Claude Code plugin for creating Rewst custom integrations. Described in its README as answering prompts like "help me create a rewst integration for https://terminal.shop" or "I have my-api.json but Rewst won't accept it, can you fix it?" Installs via Claude Code marketplace (`tim4net/claude-plugins`).

This is in EGI's wheelhouse. Anything EGI builds to automate integration generation for Rewst should study this first.

### `tim4net/logicmonitor-openapi` (2026-01)
Complete OpenAPI 3.0 spec for LogicMonitor REST API v3. Enriched with:

- Parameter descriptions for all 856 parameters.
- Schema descriptions for 673 data models.
- Standardized error responses.
- Multi-tenant server URL templating.
- Both LMv1-signature and Bearer-token authentication variants.

Critically, the repo ships two Rewst-optimized forks of the spec:

| Version | File | Ops | Size |
|---|---|---|---|
| Standard | `logicmonitor-rewst.json` | 64 | 124KB |
| Advanced | `logicmonitor-rewst-advanced.json` | 177 | 472KB |

Start with Standard unless you need role management, API token lifecycle, or config backup. Both use Bearer auth (not LMv1 HMAC, which Rewst Custom Integration v2 does not support) and fit under Rewst's size limit for custom integrations. The full LogicMonitor spec (353 operations, ~2 MB) would be rejected by the importer.

### `tim4net/claude-plugins`
The marketplace that hosts the integration-factory plugin. Peripheral but worth linking from Cataloged tooling.

## Recommended next actions

1. Pull down `logicmonitor-rewst.json` and keep it as EGI's starting point whenever a client needs LogicMonitor integration.
2. Install the Claude Code plugin and run it through a real OpenAPI. Compare output quality against what EGI would produce on its own. If it outperforms, consider adopting rather than reinventing.
3. Track the repo for updates, especially `claude-plugins` marketplace additions.

## Intel gaps

- No LinkedIn located. No Discord handle confirmed.
- A peripheral repo `tim4net/rightofboom2026` is conference-related (not Rewst-specific) and a small `claude-plugins` marketplace scaffold. Not cataloged as a contributor artifact.
