# Claude Rewst Integration Factory

A Claude Code plugin that helps you create custom integrations for [Rewst](https://rewst.io/).

## Install

**1. Install Claude Code** (if you don't have it):
```bash
npm install -g @anthropic-ai/claude-code
```

**2. Start Claude Code:**
```bash
claude
```

**3. Add the plugin:**
```
/plugin marketplace add tim4net/claude-plugins
/plugin install rewst-openapi@tim4net
```

## Usage

Just describe what you need:

```
help me create a rewst integration for https://terminal.shop
```

```
I have my-api.json but Rewst won't accept it, can you fix it?
```

```
check if my-api.json will work with Rewst before I upload it
```

## What It Does

- **Creates** OpenAPI specs from API documentation or from scratch
- **Fixes** specs that Rewst rejects (including auto-generating missing operationIds/summaries)
- **Validates** specs using Rewst's exact rules
- **Configures authentication** for Bearer, API Key, Basic Auth, or OAuth2
- **Sets up pagination** using Rewst's auto-detection patterns (offset/limit, cursor, page-based)
- **Shrinks** specs that are too large
- **Merges** multiple specs into one
- **Finds logos** by searching the web for SVG icons to include with your integration

## Updating

To get the latest version:
```
/plugin update rewst-openapi@tim4net
```

To enable auto-updates:
1. Run `/plugin`
2. Go to **Marketplaces** tab
3. Select **tim4net**
4. Choose **Enable auto-update**

## License

MIT
