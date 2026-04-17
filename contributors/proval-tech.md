# ProVal Tech

**Type:** Organization (MSP tooling vendor)
**GitHub:** <https://github.com/ProVal-Tech>

## Why they matter to EGI

ProVal Tech is an established MSP-tooling vendor. Their `rewst-cosmos-middleware` is a production-shaped integration piece (Azure Function) rather than a one-off script, which matters when evaluating architectural patterns for Rewst persistence. If ProVal's org publishes more Rewst-adjacent work, it will usually be similarly engineered.

## Published content

### `ProVal-Tech/rewst-cosmos-middleware` (2025-08)
Azure Function that acts as middleware between Rewst and Azure Cosmos DB. Use case: Rewst does not ship a native Cosmos integration, so this gives Rewst workflows a way to read and write Cosmos documents through an HTTP action that targets this Azure Function.

Treat as a reference pattern whenever Rewst needs persistent key-value state beyond what Rewst itself provides (e.g. Azure Table Storage via `gigacodedev/Rewst` or Cosmos via this middleware).

## Recommended next actions

1. Deep-scan ProVal-Tech's full GitHub org for additional Rewst-adjacent repos on next pass.
2. If an EGI client needs Rewst+Cosmos, this is the starting point rather than building from scratch.

## Intel gaps

- Full ProVal-Tech org repo inventory not enumerated yet.
