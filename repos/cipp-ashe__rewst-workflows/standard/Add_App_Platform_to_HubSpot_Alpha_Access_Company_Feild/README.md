# Add App Platform to HubSpot Alpha Access Company Feild

## Overview
**Workflow Type:** STANDARD  
**Export Date:** 2025-10-11T16:17:37.726338+00:00  
**Bundle Version:** 2  
**Original Bundle File:** `workflow-018ea9f1-ae89-74e9-88de-bf8dc0519881_20251011_161737.bundle.json`  

## Tasks

This workflow contains 4 tasks:

### 1. BEGIN
**Description:** Action that does nothing
**Action:** ``core.noop``
**Next Tasks:** 1 transition(s) defined

### 2. hs_search_companies
**Description:** search companies in hubspot
**Action:** ``hubspot.search_companies``
**Transition Mode:** FOLLOW_FIRST
**Next Tasks:** 2 transition(s) defined

### 3. hs_search_contacts
**Description:** search contacts in hubspot
**Action:** ``hubspot.search_contacts``
**Transition Mode:** FOLLOW_FIRST
**Next Tasks:** 2 transition(s) defined

### 4. hs_update_company
**Description:** update a company in hubspot
**Action:** ``hubspot.update_company``
**Next Tasks:** 1 transition(s) defined

## Technical Details

- **Workflow Timeout:** 28800 seconds
- **Total Tasks:** 4

---

*This README was automatically generated from the Rewst workflow export bundle.*
## Workflow Execution Details

### Overview
- **Total Tasks**: 4
- **Workflow Type**: STANDARD
- **Parallel Branches**: 2
- **Join Points**: 0
- **Resolved References**: 14

### Task Flow

#### 1. BEGIN
- **Action**: `core.noop`
- **Task ID**: `e3cfc3706dee4f56be746877a38cb832`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1

#### 2. hs_search_companies
- **Action**: `hubspot.search_companies`
- **Task ID**: `b02898d07b694a8fa04a295a8905d863`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_FIRST
- **Join Count**: 0
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

#### 3. hs_search_contacts
- **Action**: `hubspot.search_contacts`
- **Task ID**: `15732074f5264934ad8d6f3bb3dbb899`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_FIRST
- **Join Count**: 0
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

#### 4. hs_update_company
- **Action**: `hubspot.update_company`
- **Task ID**: `bd2e5edb8e0441bca44a732c866c28bc`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 0
- **Has Input Configuration**: Yes

### Resolved References
The following references were resolved from @@@placeholder@@@ format to actual values:

- `workflow_task_id_ref1` → `e3cfc3706dee4f56be746877a38cb832`
- `transition_id_ref1` → `${UUID}`
- `workflow_task_id_ref2` → `b02898d07b694a8fa04a295a8905d863`
- `workflow_task_id_ref4` → `bd2e5edb8e0441bca44a732c866c28bc`
- `transition_id_ref5` → `${UUID}`
- `action_ref2` → `hubspot.search_companies`
- `action_ref3` → `hubspot.search_contacts`
- `action_ref1` → `core.noop`
- `transition_id_ref3` → `${UUID}`
- `workflow_task_id_ref3` → `15732074f5264934ad8d6f3bb3dbb899`
- `transition_id_ref6` → `${UUID}`
- `transition_id_ref2` → `${UUID}`
- `transition_id_ref4` → `${UUID}`
- `action_ref4` → `hubspot.update_company`
