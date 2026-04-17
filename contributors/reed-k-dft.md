# Reed K (DFT)

**GitHub:** <https://github.com/ReedK-DFT>
**NuGet:** <https://www.nuget.org/packages/RewstWebhooks/>

## Why he matters to EGI

Reed publishes library-quality code for Rewst, not one-off scripts. If EGI builds any .NET-based tooling that needs to call Rewst webhooks, his `RewstWebhooks` NuGet package is ready-made scaffolding. His folders app is an alternative to the browser-extension approach for Rewst workflow organization.

## Published content

### `ReedK-DFT/RewstWebhooks` (1 star, NuGet published)
A .NET library for managing and calling Rewst webhooks from .NET applications (VB or C#).

- Register named secrets on a `WebhookClient`.
- Declare webhooks in a common JSON schema with name, description (for AI agents), URL, method, parameters, secret.
- Import a collection of webhook definitions as a `WebhookCollection`.
- Also supports ad-hoc calls via `CallRawWebhookAsync` overloads that accept `JsonObject`, key-value pairs, or string tuples directly.

Includes a worked Windows Forms example in VB.NET showing a real client/webhook invocation. Source targets VB primarily but works in C#.

On NuGet, so you can `dotnet add package RewstWebhooks` without pulling the source.

### `ReedK-DFT/Rewst-Folders` (1 star)
An alternative to nbit2's browser-extension approach for folder organization. This version imports as a Rewst workflow plus an AppBuilder app rather than a browser extension. The app component lives in a companion repo: `ReedK-DFT/RewstFolders-App`.

Decide between this and `nbit2/Rewst-Workflow-Folders` based on whether you want the folder system inside Rewst itself (Reed's approach) or only in the browser tabs of people using it (nbit2's approach).

### `ReedK-DFT/RewstFolders-App` (pulled pass 2, 2025-06)
Companion lightweight .NET desktop app (VB .NET). Provides access to Rewst workflows in a folder view generated from workflow tags including a `[term]` prefix. V1.2 added Forms support. V1.1 added right-click on the folder tree, path textbox, unhandled-exception handling, and error handling for workflow execution.

Author is candid in the README that the code started as a throwaway algorithm harness and is not production-polished. They note going through "two different object data models for the file system, and three different parsing routines." Useful for understanding the tag-tree algorithm, not a template for production code structure.

### `ReedK-DFT/RewstDotNet` (pulled pass 2, 2024-07)
Small .NET library for calling Rewst webhooks. Distinct from `RewstWebhooks` (the NuGet package). Appears to be an earlier or parallel implementation. If using .NET with Rewst, prefer `RewstWebhooks` from NuGet first; treat `RewstDotNet` as a reference.

## Intel gaps

- Reed's company affiliation not confirmed. "DFT" in the handle is the clue — ask on Discord.
