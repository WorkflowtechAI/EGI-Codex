# Nick Zipse

**Role:** Automation Strategist at Rewst
**GitHub:** <https://github.com/nick-zip-rewst-pub>

## Why he matters to EGI

Nick works for Rewst, so his public GitHub (`nick-zip-rewst-pub`, "pub" for "public") is the de-facto reference implementation for AppBuilder apps. The two repos he has released are the cleanest way to stand up a polished AppBuilder UI without a blank-page problem. If EGI is going to ship AppBuilder apps to clients, start here.

## Published content

### `nick-zip-rewst-pub/rewst-app-builder-starter-template`
A deliberately minimal SPA scaffold for Rewst AppBuilder. The headline features:

- **Skeleton:** collapsible sidebar, sticky header that shrinks on scroll, slide-out filter drawer on the right, page-switching system tied to sidebar clicks.
- **RewstDOM** (`src/rewst-dom-builder.js`): component library. Tables with sorting/search/pagination, metric cards, autocomplete, dropdowns, alerts, loading skeletons.
- **RewstApp** (`src/zip-graphql-js-lib-v2-optimized.js` and a newer v3): GraphQL API wrapper. Run workflows, submit forms, fetch execution data, manage orgs, all from the front end.
- **Rewst theme** (`src/rewst-override-tailwind.css`): brand colors, fonts, buttons, layered on Tailwind CSS.

Build model is refreshingly dumb. `node build.js` replaces `{{ MARKER }}` placeholders in a template HTML file with the contents of the source files and writes a single compiled HTML file to `dist/`. No npm install. You then paste the compiled HTML into an AppBuilder HTML component.

Includes a `CLAUDE.md` with step-by-step instructions for adding pages using Claude Code. This is the one worth reading before you extend.

### `nick-zip-rewst-pub/rewst-app-builder-analytics-dashboard`
Full working analytics dashboard built on the starter template. Pages:

- **Overview** — workflow execution totals, success/failure rates, trends, top workflows by count.
- **Workflow Detail** — deep view of a single workflow's history, timing, errors. Navigate by clicking the eyeball icon in any table row.
- **Form Detail** — form submission and completion analytics.
- **Insights** — aggregated patterns across automation.
- **Adoption** — org-level form usage and engagement trends.

Global filters across all pages: exclude test runs toggle, tenant filter, trigger-type filter.

Both repos carry a "community contribution, not an official Rewst product" disclaimer. They are maintained by a Rewst employee, but they are not part of Rewst's supported product surface.

## How to use these at EGI

When Capaz or any other EGI product needs an admin UI that reads from a Rewst workflow, this is the shortest path. The pattern:

1. Fork the starter template.
2. Replace `src/rewst-override-tailwind.css` with EGI brand CSS.
3. Build a page under `pages/` using the RewstDOM component patterns from the kitchen-sink demo.
4. `node build.js` and paste the compiled HTML into an AppBuilder HTML component.
5. The RewstApp GraphQL wrapper handles the data fetch.

For client-facing dashboards, the analytics dashboard repo is the reference for what a finished build looks like.

## Intel gaps

- No public posts from Nick outside these two repos. He presents at internal Rewst events but those are not indexed publicly.
- Watch for further repos under `nick-zip-rewst-pub`.
