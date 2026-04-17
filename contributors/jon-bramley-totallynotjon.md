# Jon Bramley (totallynotjon / JBramley)

**GitHub:** <https://github.com/totallynotjon>
**VS Code Marketplace:** <https://marketplace.visualstudio.com/items?itemName=JBramley.rewst-buddy>

## Why he matters to EGI

Jon builds the serious developer tooling for Rewst. If EGI's Rewst work grows past a few templates, the browser-tab editing experience becomes untenable. His VS Code extension is the escape hatch.

## Published content

### `totallynotjon/rewst-buddy`
VS Code extension. Edit Rewst templates locally in VS Code instead of the browser. Core features:

- Auto-sync on save with conflict detection so two people editing the same template do not silently overwrite each other.
- Auto-fetch on open to pick up remote changes.
- `Ctrl+Click` template navigation and hover info on `template('UUID')` calls, so you can jump from a template reference to its source.
- Template bundles — dependency-based grouping in the Explorer sidebar.
- Smart template opening reuses existing linked files rather than creating untitled scratch docs.
- File rename support with automatic stale link cleanup.
- Multi-region support.

Installation is "search rewst-buddy in VS Code Extensions, install." Connection is a paste of your `appSession` cookie, or use the companion browser extension to do it automatically.

Contains a `CLAUDE.md` for Claude Code usage alongside the extension. Worth studying as a pattern for shipping a VS Code extension with first-class Claude support.

### `totallynotjon/rewst-buddy-browser`
Companion Chrome/Edge/Firefox extension. Captures your Rewst session and hands it to VS Code. Also adds an "Open in VS Code" button on template and script pages.

Not on the Chrome Web Store yet. Install via sideload with developer mode. Multi-region support.

## Recommended next actions

1. Install `rewst-buddy` in VS Code for anyone at EGI who touches Rewst templates.
2. Read its `CLAUDE.md` and the `docs/quickstart.md`.
3. Sideload the browser extension to avoid the manual `appSession` cookie paste.

## Intel gaps

- Jon's company affiliation not confirmed publicly. Ask on Discord if relevant.
