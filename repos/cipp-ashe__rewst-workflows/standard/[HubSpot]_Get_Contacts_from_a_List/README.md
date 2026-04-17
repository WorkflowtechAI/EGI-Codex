# [HubSpot] Get Contacts from a List

## Overview
**Workflow Type:** STANDARD  
**Export Date:** 2025-10-11T16:15:45.801111+00:00  
**Bundle Version:** 2  
**Original Bundle File:** `workflow-0195f425-d653-7257-8fab-3eeef4868e9d_20251011_161545.bundle.json`  

## Parameters

- **list_name** (`string`) - Optional

## Tasks

This workflow contains 8 tasks:

### 1. BEGIN
**Description:** Action that does nothing
**Action:** ``core.noop``
**Next Tasks:** 1 transition(s) defined

### 2. END
**Description:** Action that does nothing
**Action:** ``core.noop``
**Join:** 1 (waits for 1 incoming transitions)
**Next Tasks:** 1 transition(s) defined

### 3. contact_list_output
**Description:** Set a variable as an action
**Action:** ``transforms.set_variable``
**Next Tasks:** 1 transition(s) defined

### 4. error_output
**Description:** Set a variable as an action
**Action:** ``transforms.set_variable``
**Next Tasks:** 1 transition(s) defined

### 5. get_next_page
**Description:** Action that does nothing
**Action:** ``core.noop``
**Next Tasks:** 1 transition(s) defined

### 6. hubspot_get_list
**Description:** Generic action for making authenticated requests against the HubSpot API
**Action:** ``hubspot.generic_request``
**Transition Mode:** FOLLOW_FIRST
**Join:** 1 (waits for 1 incoming transitions)
**Next Tasks:** 2 transition(s) defined

### 7. hubspot_get_list_contacts
**Description:** Generic action for making authenticated requests against the HubSpot API
**Action:** ``hubspot.generic_request``
**Transition Mode:** FOLLOW_FIRST
**Join:** 1 (waits for 1 incoming transitions)
**Next Tasks:** 3 transition(s) defined

### 8. list_not_found_output
**Description:** Set a variable as an action
**Action:** ``transforms.set_variable``
**Next Tasks:** 1 transition(s) defined

## Output

This workflow produces the following output:

- **output:** `{{ CTX.output }}`

## Technical Details

- **Workflow Timeout:** 28800 seconds
- **Total Tasks:** 8

---

*This README was automatically generated from the Rewst workflow export bundle.*
## Workflow Execution Details

### Overview
- **Total Tasks**: 8
- **Workflow Type**: STANDARD
- **Parallel Branches**: 2
- **Join Points**: 3
- **Resolved References**: 22

### Task Flow

#### 1. BEGIN
- **Action**: `core.noop`
- **Task ID**: `297f4bbcf6d041acb45d8e2cab8b3c42`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1

#### 2. END
- **Action**: `core.noop`
- **Task ID**: `5c8d6df4a45a4578b975753ec000efff`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 1
- **Next Tasks**: 0

#### 3. contact_list_output
- **Action**: `transforms.set_variable`
- **Task ID**: `e370686ab548439083f862ee328a3c1d`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

#### 4. error_output
- **Action**: `transforms.set_variable`
- **Task ID**: `123dfaae41fd4825922d37e70a741c22`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

#### 5. get_next_page
- **Action**: `core.noop`
- **Task ID**: `55f77e848128441782f15242cdb73876`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1

#### 6. hubspot_get_list
- **Action**: `hubspot.generic_request`
- **Task ID**: `86c9c14d62f0463c9c86479facff0bfd`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_FIRST
- **Join Count**: 1
- **Next Tasks**: 2
- **Has Input Configuration**: Yes

#### 7. hubspot_get_list_contacts
- **Action**: `hubspot.generic_request`
- **Task ID**: `b2d2ee196018408c84446f781b26aed6`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_FIRST
- **Join Count**: 1
- **Next Tasks**: 3
- **Has Input Configuration**: Yes

#### 8. list_not_found_output
- **Action**: `transforms.set_variable`
- **Task ID**: `e2b0790f1d7147d9882ff88e893bdac7`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

### Resolved References
The following references were resolved from @@@placeholder@@@ format to actual values:

- `transition_id_ref7` → `${UUID}`
- `transition_id_ref6` → `${UUID}`
- `transition_id_ref5` → `${UUID}`
- `transition_id_ref10` → `${UUID}`
- `workflow_task_id_ref8` → `e2b0790f1d7147d9882ff88e893bdac7`
- `workflow_task_id_ref2` → `5c8d6df4a45a4578b975753ec000efff`
- `action_ref3` → `hubspot.generic_request`
- `transition_id_ref8` → `${UUID}`
- `transition_id_ref1` → `${UUID}`
- `transition_id_ref3` → `${UUID}`
- `workflow_task_id_ref6` → `86c9c14d62f0463c9c86479facff0bfd`
- `workflow_task_id_ref5` → `55f77e848128441782f15242cdb73876`
- `transition_id_ref9` → `${UUID}`
- `workflow_task_id_ref1` → `297f4bbcf6d041acb45d8e2cab8b3c42`
- `action_ref2` → `transforms.set_variable`
- `workflow_task_id_ref4` → `123dfaae41fd4825922d37e70a741c22`
- `workflow_task_id_ref3` → `e370686ab548439083f862ee328a3c1d`
- `transition_id_ref4` → `${UUID}`
- `workflow_task_id_ref7` → `b2d2ee196018408c84446f781b26aed6`
- `transition_id_ref2` → `${UUID}`
- `action_ref1` → `core.noop`
- `transition_id_ref11` → `${UUID}`
