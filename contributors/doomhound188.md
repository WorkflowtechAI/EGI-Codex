# doomhound188 (Andrew Nicholson)

**Real name:** Andrew Nicholson
**Location:** Ontario, CA
**GitHub:** <https://github.com/doomhound188>

## Why he matters to EGI

Andrew publishes Jinja-and-Rewst-adjacent tooling. His `rewsty-jinja-ninja` repo packages the Rewst docs Jinja section as a knowledge-base drop for LLM assistants across Open WebUI, Google Gemini, Mistral LeChat, Copilot, and M365 Copilot. Pattern is directly relevant to EGI's AI-fluency curriculum and chatbot work: feeding Rewst's own docs into an LLM's grounding context.

## Published content

### `doomhound188/rewsty-jinja-ninja` (1 star)
Setup instructions for integrating "Rewsty Jinja Ninja" with various LLM platforms. Uses `gitingest.com` to flatten the Rewst docs Jinja folder at `https://github.com/RewstApp/docs.rewst.help/tree/main/documentation/jinja` into a single text file that gets uploaded as grounding context. Platforms covered: Open WebUI, Google Gemini, Mistral LeChat, GitHub Copilot, Microsoft 365 Copilot.

This is a pragmatic, copyable pattern. Any EGI-built Rewst assistant should use the same grounding approach.

### `doomhound188/rewst-api-server`
Local API server for running PowerShell and Python. Described generically — unclear if it is a Rewst Agent Smith alternative or a generic runner. Needs a 30-second README peek to decide cataloging.

## Recommended next actions

1. Apply the `gitingest.com` pattern for EGI's internal Rewst-docs knowledge base.
2. If EGI builds a Rewst-specific Claude skill, follow the same "flatten the docs repo into a knowledge file" approach.

## Intel gaps

- Whether `rewst-api-server` is Rewst-directed or generic.
