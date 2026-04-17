# Rewst Codex

Catalog of community-built Rewst content pulled from public GitHub and primary sources. Repos live under `repos/` as `owner__name` so the directory stays flat regardless of duplicate repo names. Contributor profiles live under `contributors/`. Reference notes live under `reference/`.

Last pull: 2026-04-17.

## How the catalog is organized

```
Rewst-Codex/
├── INDEX.md                     this file
├── repos/                       snapshots of community repos (48)
├── contributors/                short profiles of notable contributors (29)
├── reference/                   cross-cutting notes: blog resolutions, event context, Reddit authors
├── scripts/                     standalone PowerShell/Python/JS pulled out of repos (future)
├── dashboards/                  AppBuilder builds and styling assets (future)
├── workflows/                   loose .bundle.json files not tied to a repo (future)
├── blog-posts/                  saved articles, write-ups, teardowns (future)
└── discord-exports/             authorized exports from Rewst community Discord (future)
```

Everything in `repos/` is a public-repo snapshot, not a live git clone. Pulls happen by downloading the tarball and extracting, because the workspace filesystem does not cleanly host `.git` metadata. A `scripts/refresh-repos.sh` (future) will re-pull them against a list in `scripts/repo-list.txt`.

## Strategic context

Rewst had significant layoffs in January 2026. The Workflow Wizards series last posted in Feb 2025 and is almost certainly on permanent pause. The `/blog/` → `/success-stories/` URL migration landed during the same window. Rewst as a product still ships (RewstApp GitHub activity is daily through April 2026), but MSP community discussion has shifted: fewer net-new Rewst adopters, more discussion of alternatives, more weight on external consultancies. See `reference/rewst-layoffs-2026-01.md` for implications to EGI's positioning.

## Tier 1: the handful you will actually reach for

| Repo | Who | What | Why it matters |
|---|---|---|---|
| `nick-zip-rewst-pub__rewst-app-builder-starter-template` | Nick Zipse | Starter SPA for Rewst AppBuilder, with RewstDOM component library, a GraphQL wrapper (`zip-graphql-js-lib`), and a Tailwind-over-Rewst theme CSS. Compiles to a single HTML file you paste into an AppBuilder HTML component. | The "styling libraries so you can set the colours, fonts, buttons etc." kit. Drop it into any AppBuilder screen and you are not starting from a blank page. |
| `nick-zip-rewst-pub__rewst-app-builder-analytics-dashboard` | Nick Zipse | Full analytics dashboard built on the starter template. Metrics, workflow execution history, form adoption, tenant/trigger filters. | Reference build for what a polished AppBuilder app looks like. Use as a copy-paste source when someone asks for a client-facing dashboard. |
| `gigacodedev__Rewst` | Brandon Martinez (Giga), eTop Technology | Community workflow bundles. Authorized User Check, Azure Tables actions, ImmyBot Storage Monitor, ImmyBot↔ConnectWise Manage device assignment sync, SMTP2GO email, Send Adaptive Card, Send Teams Approval Card. | `[eTop]` prefixed bundles have been cleared by eTop's CEO William Pote for public release. Adaptive Card and Teams Approval Card alone are worth the trip. |
| `JohnDuprey__RewstWorkflows` | John Duprey | CIPP User Invite, CIPP Repository Diagnostic, Meme Generator. | CIPP hooks are the interop layer for anyone running Kelvin Tegelaar's CIPP alongside Rewst. |
| `RewstApp__jinja-try-catch` | Rewst (official) | `{% try %} ... {% catch %} ... {% endtry %}` extension for Jinja2 inside Rewst. | Rewst's Jinja layer normally has no exception handling. This drops in error-safe blocks. Rewst-maintained. |
| `RewstApp__jinja-comprehensions` | Rewst (official) | List/dict comprehensions, set literals, generator expressions, spread operators in Jinja2. | Lets you write `{{ [n // 2 for n in range(10)] }}` and `{{ {**stuff, 'e': 101} }}` inside a workflow. Giant ergonomics upgrade for anyone doing non-trivial data shaping. |
| `RewstApp__agent-smith-go` | Rewst (official) | Official lean command executor that runs as a system service on Windows/Linux/macOS. Open-source rewrite in Go. | The agent that lets Rewst workflows reach down to endpoints. Local source means you can audit it, fork it for restricted networks, or build custom installers. |
| `totallynotjon__rewst-buddy` | Jon Bramley | VS Code extension. Edit Rewst templates locally with git, agents, and editor tooling. Auto-sync on save, conflict detection, `Ctrl+Click` template navigation. Companion browser extension captures your session. | If EGI's Rewst work grows beyond a handful of templates, editing them in a browser tab is untenable. This is the escape hatch. |
| `shiftnerd__OpenAPISchemas` + `shiftnerd__schemadoctor` | shiftnerd | Pre-fixed OpenAPI schemas for common tools, plus a web app (`schemadoctor.com`) to clean and reduce OpenAPI specs for Rewst's custom integration import. | Integration-import pain from malformed OpenAPI specs is the single biggest blocker when onboarding a new SaaS into a Rewst workflow. These schemas solve half the cases, schemadoctor solves most of the rest. |
| `tim4net__logicmonitor-openapi` | tim4net | Pre-cleaned LogicMonitor OpenAPI spec for direct import as a Rewst custom integration. | LogicMonitor's native spec is oversized and has validation warnings that break Rewst import. This fork cleans both in a way that passes Rewst validation. |
| `platinumtechSyd__RewstAPIS` | Platinum Tech Sydney | Dumped OpenAPI specs for a handful of APIs already trimmed for Rewst import. | Parallel resource to shiftnerd's OpenAPISchemas. Use both as a first-pass lookup before authoring a spec from scratch. |
| `rbkidea11__microsoft-defender-for-endpoint-openapi` | rbkidea11 | OpenAPI spec for Microsoft Defender for Endpoint, Rewst-import-ready. | Niche but high-value. M365 security automation without this means writing the spec yourself. |
| `nikolazleo__Rewst_Workflow_Executions` | nikolazleo | LogicMonitor DataSource LogicModules that pull Rewst workflow execution data via GraphQL, plus workflow-configuration-drift monitoring. | Best observability story for Rewst in the wild. If you use LogicMonitor, this is free monitoring. |
| `tfournet__rewst_university` | Tim Fournet | Learning-path repo with lesson scaffolding, starter workflows, concept examples. | Useful as an EGI onboarding reference and as an idiom reference for how Rewst-adjacent training material is structured. |
| `tfournet__deckwing` | Tim Fournet | Rewst-adjacent tooling. Inspect before pulling into client work. | Idiomatic patterns worth reading. |

## Tier 2: workflow libraries (bundle.json, ready to import)

Drop the `.bundle.json` into Rewst via Automations → Workflows → Import Bundle.

| Repo | Contributor | Highlights |
|---|---|---|
| `PEAKE-Technology-Partners__Rewst-Workflows` | Peake Technology Partners | NinjaOne migration helpers, Meraki (licensing, org mapping, public IPs, webhook triggers), Google Forms → Rewst webhook, Org Variable Pages, db-completed listener. Heavy production focus. |
| `Tre-Eiler__Rewst` | Tre Eiler (`tre_eiler` on Discord, linkedin.com/in/treeiler) | Curated automation bundles with demo links in each folder. |
| `BezaluLLC__Rewst-Workflows` | Bezalu LLC | Escalation Workflow, PSA Ticket Handler (New), Long-Running Ticket Report, Registration Campaign Standard, Standards Wrapper, PSA Review-and-Close-Completed-Tickets, Docs Seed Default Folders, Get Group ID by name. |
| `DevonChorney__RewstProjects` | Devon Chorney | Open-sourced Rewst flows. |
| `bmsimp__My-Rewst-Workflows` | Brian Simpson (Karpel Solutions, Workflow Wizards #5) | Community-shared workflows from a profiled Workflow Wizard. |
| `bmsimp__Rewst-PowerShell-Scripts` | Brian Simpson | PowerShell scripts purpose-built for Rewst's script runner. |
| `ddunkijaco__RewstWorkflows` | ddunkijaco | Contains `RewstRewind` and `RewstStylus` subprojects. |
| `djhayes1994__Rewst-Workflows` | Daniel Hayes | Example workflows. Idiomatic Marketplace-style bundles. |
| `Mondyro__Rewst` | Mondyro | Workflow execution results and PowerShell. |
| `justinleahy__rewst-workflows` | Justin Leahy | Create Excel Spreadsheet with Table, Onboarding Copy User Licenses. |
| `cipp-ashe__rewst-workflows` | cipp-ashe (CyberDrain) | Options generators (9, with `[OG]` naming convention), 31 standard workflows, HubSpot + Discord cross-integration work. |
| `DanEatsWaffles__Workflows` | DanEatsWaffles | Workflow collection. |
| `tim-hunt303__Rewst_Workflows` | Tim Hunt | ConnectWise-focused workflows. Distinct person from Tim Fournet and tim4net. |
| `BPT-CIPP__rewst-actions-app` | BPT-CIPP | CIPP + Rewst actions surface. Second CIPP-Rewst interop vector alongside JohnDuprey's workflows. |

## Tier 3: script runners and middleware (break Rewst out to full languages)

When Rewst's built-in Jinja is not enough, these projects run PowerShell, Python, or JavaScript in an external runtime and expose them as Rewst custom integrations.

| Repo | Language / runtime | Mechanism |
|---|---|---|
| `gocovi__RewstPS` | PowerShell Core (Azure Function) | Fork, point Azure Function Deployment Center at the fork, import the `run-powershell.bundle.json`. |
| `gocovi__RewstJS` | Node 18/20 (Azure Function) | Same pattern as RewstPS. Supports npm packages. |
| `gocovi__RewstPy` | Python (Azure Function) | Same pattern. Packages via `requirements.txt`; `func azure functionapp publish` workaround documented for GitHub Actions deploy issues. |
| `ProVal-Tech__rewst-cosmos-middleware` | Cosmos DB middleware | Middleware layer between Rewst and Azure Cosmos DB. Useful when a workflow needs persistent state across runs and Rewst's Org Variables are the wrong shape. |

All three `gocovi` runners share patterns. The RewstPS docs are the most complete, so start there when standing up the first function app.

## Tier 4: developer tooling and browser extensions

| Repo | Who | What |
|---|---|---|
| `totallynotjon__rewst-buddy` | Jon Bramley | VS Code extension for Rewst template editing (see Tier 1). |
| `totallynotjon__rewst-buddy-browser` | Jon Bramley | Companion Chrome/Edge/Firefox extension that captures your Rewst session and opens templates in VS Code. |
| `jon-raven-auto__vscode-rewst-CICD` | Jon Raven Auto | Lighter VS Code extension. Auto-pushes templates to Rewst on file save. Multi-org support. Works with any filetype by using commented `create template` / `export [UUID]` keywords in line 1. |
| `nbit2__Rewst-Workflow-Folders` | nbit2 | Chrome/Edge extension that adds folder organization to the Rewst workflows list. Targets `app.rewst.asia` by default; change `manifest.json` for other regions. Assignments stored in browser local storage with export/import. |
| `ReedK-DFT__Rewst-Folders` + `ReedK-DFT__RewstFolders-App` | Reed K (DFT) | Alternative folder system. Imports as a Rewst workflow + AppBuilder app rather than a browser extension. The `-App` repo is the AppBuilder companion; use both together. |
| `ReedK-DFT__RewstWebhooks` + `ReedK-DFT__RewstDotNet` | Reed K (DFT) | .NET library for calling Rewst webhooks from .NET applications. `RewstWebhooks` is the NuGet-published library. `RewstDotNet` extends the surface area with additional .NET bindings. |
| `cipp-ashe__workflow-vizualizer` | cipp-ashe (CyberDrain) | React/TypeScript app. Upload or browse a Rewst workflow JSON and render it as an interactive node graph with sorting, filtering, SVG/PNG export. |
| `benlavalley__httpRequestLogger` | Ben LaValley | Minimal Node server that logs incoming HTTP requests to console and files. Point a Rewst webhook at it when you need to see exactly what payload Rewst is sending. |
| `mcallbosco__Rewst-Expanded-Inputs` | Mcall Bosco | Userscript (Tampermonkey/Violentmonkey) that expands the input UI on Rewst forms. Quality-of-life fix for operators who hit the default input width often. |

## Tier 5: AI tooling and skills

| Repo | Who | What |
|---|---|---|
| `AntoJUICT__roborewsty-skill` | AntoJUICT | Claude Code skill. Generates complete RoboRewsty workflow specs (goal, trigger, steps, Jinja variables, data aliases, transitions) from a plain-English description. Flags known Rewst pitfalls like the JSON body iteration bug. |
| `tim4net__claude-rewst-integration-factory` | tim4net | Claude plugin / skill for building Rewst custom integrations. OpenAPI clean-up, operationId generation, integration scaffolding. Paired with `tim4net__logicmonitor-openapi` as a worked example of the factory output. |
| `adamhancock__claude-skills` | Adam Hancock | Collection of Claude Code skills, several Rewst-focused. Authoring conventions and scope are worth reading as a template for EGI's own plugin/skill work. |
| `doomhound188__rewsty-jinja-ninja` | Andrew Nicholson (doomhound188) | Jinja2 helper toolkit specifically for Rewst. Filters, macros, patterns that Rewst's stock Jinja does not cover. |

RoboRewsty is Rewst's in-product AI workflow builder; specs generated from a well-trained skill are far tighter than hand-typing into the RoboRewsty prompt. The tim4net and adamhancock repos are the two most substantial external Claude-Code-on-Rewst efforts in the wild. This is the tier to mine for EGI's own Rewst plugin pattern library.

## Tier 6: observability and agents

| Repo | Who | What |
|---|---|---|
| `RewstApp__agent-smith-go` | Rewst (official) | The endpoint agent. Runs as a system service, talks MQTT back to Rewst, executes commands on demand. See Tier 1. |
| `RewstApp__agent-smith-tray` | Rewst (official) | Optional Windows tray icon for Agent Smith. Shows service status and exposes configurable links to end users. |
| `tfournet__install-agent-smith` | Tim Fournet | Installer tooling for Agent Smith. Useful when provisioning agents at scale or in restricted environments. |
| `nikolazleo__Rewst_Workflow_Executions` | nikolazleo | LogicMonitor DataSources for Rewst workflow execution metrics and config drift, plus LM Logs Syslog integration. Requires the Rewst GraphQL Task beta (contact Rewst support for access). |

## Contributors

Profiles under `contributors/`. Pass 2 set:

- `contributors/david-braun-egi.md` — David Braun, `GestaltWorks`. EGI principal, catalog consumer.
- `contributors/brandon-martinez-giga.md` — eTop Technology, `gigacodedev`, personal blog at `blog.gigacode.dev`
- `contributors/nick-zipse.md` — Rewst Automation Strategist, `nick-zip-rewst-pub`
- `contributors/john-duprey.md` — `JohnDuprey`, CIPP interop
- `contributors/tre-eiler.md` — `Tre-Eiler`, `tre_eiler` on Discord
- `contributors/jon-bramley-totallynotjon.md` — `totallynotjon`, Rewst Buddy VS Code author
- `contributors/peake-technology-partners.md` — `PEAKE-Technology-Partners`, Meraki + NinjaOne heavy
- `contributors/nikola-zleo.md` — `nikolazleo`, LogicMonitor integration
- `contributors/gocovi.md` — `gocovi`, Azure Function script runners
- `contributors/reed-k-dft.md` — `ReedK-DFT`, .NET webhooks + folders app + RewstDotNet
- `contributors/mendy-green.md` — Rising Tide Consulting Group founder, MSPGeek admin. High-priority consultant presence.
- `contributors/brian-simpson.md` — `bmsimp`, Karpel Solutions, Workflow Wizards #5
- `contributors/justin-leahy.md` — `justinleahy`, M365 reporting workflows
- `contributors/gareth-smith.md` — Your IT Department (Your Automation PS), Workflow Wizards #9
- `contributors/dustin-riley.md` — `derpenstiltskin`, CisCom Solutions, Workflow Wizards #6
- `contributors/doomhound188.md` — Andrew Nicholson, `rewsty-jinja-ninja`
- `contributors/mcallbosco.md` — `Rewst-Expanded-Inputs` userscript author
- `contributors/adam-hancock.md` — `adamhancock`, Claude Code Rewst skill collection
- `contributors/cipp-ashe.md` — `workflow-vizualizer` + `rewst-workflows` (CyberDrain). Same person as "Ashe" in Rewst community circles.
- `contributors/bpt-cipp.md` — `BPT-CIPP`, Rewst actions app for CIPP
- `contributors/dan-eats-waffles.md` — workflow collection author
- `contributors/tim-hunt.md` — `tim-hunt303`, ConnectWise-focused workflows
- `contributors/platinumtech-syd.md` — `platinumtechSyd`, OpenAPI dumps
- `contributors/rbkidea11.md` — Defender for Endpoint OpenAPI
- `contributors/proval-tech.md` — `ProVal-Tech`, Cosmos middleware
- `contributors/tim4net.md` — Claude-Rewst integration factory, LogicMonitor OpenAPI
- `contributors/shiftnerd.md` — Schema Doctor + OpenAPI schemas
- `contributors/tim-fournet.md` — `tfournet`, rewst_university, install-agent-smith, deckwing
- `contributors/daniel-hayes.md` — `djhayes1994`, Rewst-Workflows

## Reference notes

Under `reference/`, separate from contributor profiles. Cross-cutting notes that span multiple contributors or the broader ecosystem.

- `reference/workflow-wizards.md` — All 9 Workflow Wizards posts (series is likely on permanent pause after Feb 2025), URL migration from `/blog/` to `/success-stories/`, unresolved GitHub handles, the two Rewst marketing authors of the series (Joelle Cullimore, Angela DeClouet), patterns to steal.
- `reference/rewst-layoffs-2026-01.md` — January 2026 layoffs context. Strategic implications for EGI positioning and any long-horizon Rewst-dependent client commitments.
- `reference/gigacode-blog.md` — Resolved URL `blog.gigacode.dev`, full post inventory by topic, notable gap: no Rewst-specific post yet published (author has promised one in the Bot Framework series).
- `reference/reddit-authors.md` — r/msp authors worth tracking, with scrape technique using public Reddit JSON (no auth required). High-signal: `u/msp4msps`, `u/jackmusick`, `u/Next-Landscape-9884`, `u/bibawa`. Medium-signal: `u/cokebottle22`, `u/pjustmd`, `u/rhysfromaussie`, `u/Lime-TeGek`. Notable one-offs cited but not profiled.

## Known gaps

Areas where the catalog is incomplete and where future discovery passes will focus:

- **Rewst Discord `#kewp-workflow-talk`** and adjacent channels. Authorized exports land in `discord-exports/` when available.
- **ROC Open Mic backfill**. 2024 and 2025 session pages are programmatically reachable; enumeration is queued.
- **LinkedIn post tracking** for active Rewst practitioners. Mendy Green and similar high-volume consultant voices are the priority targets.
- **Reddit r/msp**. Continues on a quarterly refresh via public JSON.

## Source URLs

- GitHub community workflow hub: <https://github.com/gigacodedev/Rewst>
- Rewst official org on GitHub: <https://github.com/RewstApp>
- EGI GitHub: <https://github.com/GestaltWorks>
- Rewst docs: <https://docs.rewst.help>
- Rewst blog: <https://rewst.io/resources/blog>
- Rewst success stories (Workflow Wizards new home): <https://rewst.io/success-stories/>
- ROC Open Mic recap index: <https://docs.rewst.help/updates/roc-open-mics/>
- AppBuilder docs: <https://docs.rewst.help/documentation/app-builder>
- Rewst community page: <https://rewst.io/support/community>
- Schema Doctor: <https://schemadoctor.com>
- GigaCode Blog (Brandon Martinez): <https://blog.gigacode.dev/>
- Rewst layoff discussion thread: <https://reddit.com/r/msp/comments/1qcsxd9/>
