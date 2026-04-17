# Brandon Martinez (Giga / gigacodedev)

**Role:** Senior IT Manager, eTop Technology
**Rewst Discord handle:** `@gigacode`
**GitHub:** <https://github.com/gigacodedev>
**LinkedIn:** <https://www.linkedin.com/in/etopbrandon>

## Why he matters to EGI

eTop Technology was one of the first Rewst customers and is now a Professional Services partner, which means Brandon has built more production Rewst workflows than almost anyone outside the Rewst org itself. He has authored 40+ production workflows at eTop, and his boss (William Pote, eTop CEO) has explicitly cleared a portion of them for community release. Those cleared workflows are what you find in `gigacodedev/Rewst`.

## Published content

### `gigacodedev/Rewst` (18 stars, active through 2025)
Bundles in this repo, all ready to import into Rewst via Automations → Workflows → Import Bundle:

- **Authorized User Check** — gatekeep a workflow by validating the triggering user against an authorized list.
- **Azure Tables Actions** — wrappers for reading/writing Azure Table Storage from Rewst. Useful when you want lightweight persistent state outside Rewst itself.
- **ImmyBot Storage Monitor** — monitors ImmyBot storage thresholds.
- **ImmyBot to CWM Device Assignment Sync** — keeps ConnectWise Manage device assignments in sync with ImmyBot.
- **SMTP2GO Send Email** — drop-in email sender using SMTP2GO, for orgs that do not want to route transactional mail through M365.
- **Send Adaptive Card** — posts an Adaptive Card to a Teams channel. The standard building block for any Teams-based interaction.
- **Send Teams Approval Card** — Adaptive Card variant with an approval pattern. Use this when you need a human-in-the-loop decision inside a Rewst workflow.

Workflows prefixed `[eTop]` originated from eTop's internal automation catalog and are authorized for public release.

## Methodology (from Rewst's "Workflow Wizards" feature)

His approach is worth copying verbatim. From <https://rewst.io/blog/workflow-wizards-brandon-martinez/>:

- Documentation first. He defines the process in Microsoft Word before touching Rewst. "I don't touch Rewst until halfway through the new workflow process."
- Modular design. Sub-workflows for anything reused. His example: an action to get users from Microsoft Graph does not change between processes, so it lives as its own sub-workflow.
- Peer review culture. He solicits team feedback through Microsoft Teams before implementation, treating workflow development like code review.

Two flagship non-public projects he has described:
- **Teams Helper Bot** — bot framework integrated with Rewst for 1:1 conversations, with AI-powered ticket note summaries via OpenAI.
- **Contact Sync Automation** — M365 ↔ ConnectWise Manage contact sync during onboarding/offboarding. Claims 30-40 seconds saved per transaction.

## Intel gaps

- Any Pro Services work he has done for other eTop clients that has NOT been approved for release stays private. The way in is to ask his client directly for a release.

## Related

- Personal blog inventory: `reference/gigacode-blog.md`. Resolved to <https://blog.gigacode.dev/>.
