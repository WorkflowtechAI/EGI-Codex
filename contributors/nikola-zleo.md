# Nikola (nikolazleo)

**GitHub:** <https://github.com/nikolazleo>

## Why he matters to EGI

Built the only serious Rewst observability story I have found in a public repo. If EGI has or acquires a LogicMonitor client, this integration is effectively free monitoring for Rewst itself.

## Published content

### `nikolazleo/Rewst_Workflow_Executions` (3 stars, active 2026)

A LogicMonitor DataSource LogicModule set that pulls Rewst workflow execution data via GraphQL. Four modules:

- `Rewst - LogicMonitor Datasource - Rewst_Workflow_Execution` — execution counts, durations, status.
- `Rewst - LogicMonitor Datasource - JSON Coop` — JSON data cooperation across modules.
- `Rewst - LogicMonitor Datasource - Workflow Configuration Drift` — detects when workflow config changes in Rewst. Effectively change tracking for Rewst workflows, which Rewst itself does not surface well.
- `Rewst - LogicMonitor Datasource - LmLogs Request` — request data into LM Logs.

Plus a separate LM Logs module, `Rewst - LogicMonitor LmLogs - Workflow Syslog`, for ingesting execution logs via syslog.

The README includes an installation walkthrough with images: import the LogicModule JSON, create a LogicMonitor API bearer token, import the Rewst workflow bundle, set up Rewst org variables for the integration.

Requires: LogicMonitor, optionally LM Logs, and the Rewst GraphQL Task (Beta) — contact Rewst support for access.

## Why the GraphQL beta matters

Nikola's integration depends on the Rewst GraphQL Task beta, which is also what Nick Zipse's AppBuilder starter uses under the hood. If EGI has access to the GraphQL beta, these two repos together give you both a monitoring story (Nikola) and a dashboard story (Nick) without having to build either from scratch.

## Intel gaps

- Nikola's company/MSP affiliation not confirmed publicly.
- Check his other repos for additional monitoring or observability work.
