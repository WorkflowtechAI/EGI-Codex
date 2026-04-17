# Peake Technology Partners

**GitHub:** <https://github.com/PEAKE-Technology-Partners>

## Why they matter to EGI

Peake is an MSP running their Rewst production workflows publicly. Their `Rewst-Workflows` repo is one of the most "real" workflow libraries out there, with bundles for tools MSPs actually use in production (Meraki, NinjaOne, Google Forms, Azure).

## Published content

### `PEAKE-Technology-Partners/Rewst-Workflows` (7 stars, last updated 2025-06)

Subdirectories at the root:

- **Automate-Ninja-Migration-Scripts** — migration helpers for NinjaOne. Likely useful if an EGI client is moving between RMMs.
- **IP Info Workflow** — IP lookup and enrichment.
- **Meraki-License-Workflow** — Meraki license tracking.
- **Meraki-Org-Mapping** — Meraki org structure mapping.
- **Meraki-Webhook-Workflows** — webhook triggers and handlers for Meraki events.
- **meraki-public-ips** — public IP audit for Meraki networks.
- **Org-Variable-Pages** — Rewst org variables exposed as AppBuilder pages. Useful pattern if EGI wants a UI for org-level configuration.
- **db-completed-listener** — listener for database completion events.
- **Google-Forms-Rewst-Webhook.md** — documentation for wiring Google Forms submissions to a Rewst webhook. Not a bundle, just the integration write-up.

Four separate Meraki projects. If any EGI client has Meraki infrastructure, this is the fastest starting point.

## Intel gaps

- No top-level README. Each subdirectory should be read individually to extract specific bundle imports.
- Company website and contact not confirmed in-repo.
