# mcallbosco

**GitHub:** <https://github.com/mcallbosco>

## Why they matter to EGI

Authored `Rewst-Expanded-Inputs`, a Tampermonkey userscript that makes the Rewst web UI tolerable when working with long Jinja templates or JSON blobs. This is a QoL tool every EGI engineer should install on day one.

## Published content

### `mcallbosco/Rewst-Expanded-Inputs` (2025-12)
Userscript that patches `app.rewst.io`:

- Adds an **Edit** button to small input fields (including "Default Value" and "Value"). Clicking opens a large pop-up editor, making long text and code workable.
- Adds a **View** button to data-table cells that contain long content.
- Auto-formats JSON content for readability.

Install: Tampermonkey → open `script.user.js` raw → click Install.

## Recommended next actions

1. Make this a standard install for anyone on EGI doing Rewst workflow editing.
2. Document it in EGI's Rewst onboarding doc.

## Intel gaps

- Author identity not public. Not cataloged as a multi-project contributor; this one script is the full artifact.
