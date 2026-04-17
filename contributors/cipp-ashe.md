# cipp-ashe

**Location:** Canada (per GitHub profile)
**GitHub:** <https://github.com/cipp-ashe>
**Company on GitHub profile:** CyberDrain

Same person as "Ashe" in Rewst community circles (confirmed by David, 2026-04-17). The earlier pass-2 guess that they were separate people was wrong.

## Why they matter to EGI

cipp-ashe publishes Rewst-adjacent tooling including `workflow-vizualizer` and `rewst-workflows`. The `workflow-vizualizer` is particularly valuable: it takes exported Rewst bundle JSONs and generates navigable visualizations, which is a pain point when reviewing complex workflows outside the Rewst editor.

Disambiguation still worth keeping: Chris Ackerman (Workflow Wizards #7, Aegis) is sometimes conflated with cipp-ashe in lookups. They are different people.

## Published content

### `cipp-ashe/workflow-vizualizer`
Pulled in pass 1. Visualizer for Rewst bundle JSONs.

### `cipp-ashe/rewst-workflows` (2025-10)
Pulled in pass 2. Large workflow collection organized by workflow type:

- `options-generators/` — 9 workflows explicitly tagged `"type": "OPTION_GENERATOR"`. Generate dynamic dropdown options. Examples include `[OG]_HubSpot_CSM_Users`, `[OG]_HubSpot_Companies_w_Discord_Active`, `HubSpot_CSM_Lookup_[OG]`, and `Cluck_U_Lookup_User_Credentials`.
- `standard/` — 31 standard workflows.

Notable: the HubSpot + Discord cross-integration plus `Cluck_U` references (Rewst's training brand) are consistent with tight community and training work across both CyberDrain and Rewst.

### `cipp-ashe/custom-gpts`
GPT prompt collection. Tangential; name-only match.

## Recommended next actions

1. The options-generators folder is gold for EGI. Copy the naming conventions; `[OG]` prefix is worth adopting.
2. Review workflow-vizualizer for integration into EGI's internal Rewst review process.
