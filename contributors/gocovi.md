# gocovi

**GitHub:** <https://github.com/gocovi>

## Why he matters to EGI

When Rewst's built-in Jinja and template language is not enough, gocovi's three Azure Functions projects are the cleanest way to break out to a real programming language. All three follow the same pattern: fork the repo, deploy to an Azure Function App, wire it up as a Rewst custom integration, import the provided workflow bundle.

## Published content

### `gocovi/RewstPS` (6 stars)
Run raw PowerShell Core in an Azure Function, callable from Rewst. This is the flagship of the three and has the most complete README, including a worked `Write-HelloRewst` example showing how Rewst `json_dump` filter output interacts with PowerShell's `ConvertFrom-JSON`.

Key detail: single quotes are important when passing objects from Rewst Jinja to PowerShell, because `json_dump` uses double quotes in the resulting string.

Use this when you have PowerShell scripts that depend on modules not available on a Rewst agent, or when you want to run something centrally rather than on an endpoint.

### `gocovi/RewstJS` (3 stars)
Same pattern, Node.js 18 or 20. Packages via npm in the forked repo.

Use this for anything JavaScript-native: signature verification, HMAC, crypto operations that are awkward in Jinja.

### `gocovi/RewstPy` (2 stars)
Same pattern, Python (latest). Packages via `requirements.txt`, with a documented workaround for a GitHub Actions deploy issue (use Azure Functions Tools and deploy locally with `func azure functionapp publish`).

Use this for anything data-science-heavy, or when you need libraries like `pandas`, `openpyxl`, or any ML package.

## Setup (applies to all three)

1. Fork the repo.
2. Create an Azure Function App. Linux. Runtime matches the language.
3. In the Function App, Deployment Center → connect to the forked GitHub repo.
4. Get the Function URL and the default Function key.
5. In Rewst: Configuration → Integrations → Custom Integrations → add new, using `yourfunctionapp.azurewebsites.net` as the hostname, the key as the API key, `API Key` auth, `x-functions-key` as the header name.
6. Import the provided bundle (`run-powershell.bundle.json`, `run-javascript.bundle.json`, `run-python.bundle.json`).
7. In the `run_script` action's Advanced tab, set an Integration Override for your new integration and publish.

One Function App can host only one of the three. If you want all three, plan for three Function Apps (or change the function name in the fork).

## Recommended next actions

1. If EGI commits to using Rewst heavily, stand up at least `RewstPS` up front. The pattern of "drop into full PowerShell for this one tricky bit" resolves a very common friction point.
2. Read gocovi's other repos for anything else Rewst-adjacent that did not surface in the first-pass search.

## Intel gaps

- gocovi's real name and company not confirmed.
- Licensing terms on each repo to verify before adopting in client-facing work.
