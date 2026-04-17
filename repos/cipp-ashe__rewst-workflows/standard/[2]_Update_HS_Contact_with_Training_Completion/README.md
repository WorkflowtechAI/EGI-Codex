# [2] Update HS Contact with Training Completion

## Overview
**Workflow Type:** STANDARD  
**Export Date:** 2025-10-11T16:18:05.847624+00:00  
**Bundle Version:** 2  
**Original Bundle File:** `workflow-fce22f7f-8854-4bd5-99cd-5484d31291f3_20251011_161805.bundle.json`  

## Parameters

- **email** (`string`) - Optional
- **training_id** (`string`) - Optional

## Tasks

This workflow contains 13 tasks:

### 1. BEGIN
**Description:** Action that does nothing
**Action:** ``core.noop``
**Next Tasks:** 1 transition(s) defined

### 2. association_updated
**Description:** Action that does nothing
**Action:** ``core.noop``
**Join:** 1 (waits for 1 incoming transitions)
**Next Tasks:** 1 transition(s) defined

### 3. company_found
**Description:** Action that does nothing
**Action:** ``core.noop``
**Next Tasks:** 1 transition(s) defined

### 4. contact_created
**Description:** Action that does nothing
**Action:** ``core.noop``
**Next Tasks:** 1 transition(s) defined

### 5. contact_found
**Description:** Action that does nothing
**Action:** ``core.noop``
**Next Tasks:** 1 transition(s) defined

### 6. end_no_account
**Description:** Action that does nothing
**Action:** ``core.noop``
**Next Tasks:** 1 transition(s) defined

### 7. hub_spot_create_contact
**Description:** create a contact in hubspot
**Action:** ``hubspot.create_contact``
**Next Tasks:** 1 transition(s) defined

### 8. hub_spot_search_companies
**Description:** search companies in hubspot
**Action:** ``hubspot.search_companies``
**Transition Mode:** FOLLOW_FIRST
**Next Tasks:** 2 transition(s) defined

### 9. hub_spot_search_contacts
**Description:** search contacts in hubspot
**Action:** ``hubspot.search_contacts``
**Transition Mode:** FOLLOW_FIRST
**Next Tasks:** 2 transition(s) defined

### 10. no_company
**Description:** Action that does nothing
**Action:** ``core.noop``
**Next Tasks:** 1 transition(s) defined

### 11. no_contact
**Description:** Action that does nothing
**Action:** ``core.noop``
**Next Tasks:** 1 transition(s) defined

### 12. slack_chat_post_message
**Description:** Sends a message to a channel.
**Action:** ``slack.chat.postMessage``
**Next Tasks:** 1 transition(s) defined

### 13. update_contact_associations
**Description:** Post a meeting in HubSpot with the latest updates and details of the escalated ticket.
**Action:** ``hubspot.generic_request``
**Join:** 1 (waits for 1 incoming transitions)
**Next Tasks:** 1 transition(s) defined

## Technical Details

- **Workflow Timeout:** 28800 seconds
- **Total Tasks:** 13

---

*This README was automatically generated from the Rewst workflow export bundle.*
## Workflow Execution Details

### Overview
- **Total Tasks**: 13
- **Workflow Type**: STANDARD
- **Parallel Branches**: 2
- **Join Points**: 2
- **Resolved References**: 34

### Task Flow

#### 1. BEGIN
- **Action**: `core.noop`
- **Task ID**: `a4b0114a4b1043fa88d045d221dc429e`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1

#### 2. association_updated
- **Action**: `core.noop`
- **Task ID**: `8b208e97b37f4217b56b994a823a2ebd`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 1
- **Next Tasks**: 0

#### 3. company_found
- **Action**: `core.noop`
- **Task ID**: `a8937674f4214321bf7385934f2de9e2`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1

#### 4. contact_created
- **Action**: `core.noop`
- **Task ID**: `a1d7d9649d464273912a1b4e3618dd38`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1

#### 5. contact_found
- **Action**: `core.noop`
- **Task ID**: `2b7dfa1c11534f5695739ff0f94ba146`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1

#### 6. end_no_account
- **Action**: `core.noop`
- **Task ID**: `d7748d3800ec4b2f9cc0d4156a339f18`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 0

#### 7. hub_spot_create_contact
- **Action**: `hubspot.create_contact`
- **Task ID**: `cfe044bece8f4947aeec6b111f456731`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

#### 8. hub_spot_search_companies
- **Action**: `hubspot.search_companies`
- **Task ID**: `892ce8bf5ef0484fb07b3d9e1cfefa4c`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_FIRST
- **Join Count**: 0
- **Next Tasks**: 2
- **Has Input Configuration**: Yes

#### 9. hub_spot_search_contacts
- **Action**: `hubspot.search_contacts`
- **Task ID**: `772006ffdbfd481aa80d07ce560eb6ef`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_FIRST
- **Join Count**: 0
- **Next Tasks**: 2
- **Has Input Configuration**: Yes

#### 10. no_company
- **Action**: `core.noop`
- **Task ID**: `bc3bbd22d14846179f373df4fe71f199`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1

#### 11. no_contact
- **Action**: `core.noop`
- **Task ID**: `f60baee993874bb5bf76e9e0d1b039aa`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1

#### 12. slack_chat_post_message
- **Action**: `slack.chat.postMessage`
- **Task ID**: `b834e1c55a174c3b9c5b3318cddcb246`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

#### 13. update_contact_associations
- **Action**: `hubspot.generic_request`
- **Task ID**: `bc0f54e7057549c8a9ea272aaeda72fb`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 1
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

### Resolved References
The following references were resolved from @@@placeholder@@@ format to actual values:

- `transition_id_ref8` → `${UUID}`
- `action_ref4` → `hubspot.search_contacts`
- `workflow_task_id_ref3` → `a8937674f4214321bf7385934f2de9e2`
- `workflow_task_id_ref2` → `8b208e97b37f4217b56b994a823a2ebd`
- `transition_id_ref9` → `${UUID}`
- `workflow_task_id_ref11` → `f60baee993874bb5bf76e9e0d1b039aa`
- `transition_id_ref3` → `${UUID}`
- `workflow_task_id_ref4` → `a1d7d9649d464273912a1b4e3618dd38`
- `action_ref6` → `hubspot.generic_request`
- `transition_id_ref14` → `${UUID}`
- `transition_id_ref15` → `${UUID}`
- `transition_id_ref2` → `${UUID}`
- `workflow_task_id_ref1` → `a4b0114a4b1043fa88d045d221dc429e`
- `workflow_task_id_ref7` → `cfe044bece8f4947aeec6b111f456731`
- `action_ref5` → `slack.chat.postMessage`
- `transition_id_ref12` → `${UUID}`
- `transition_id_ref10` → `${UUID}`
- `action_ref3` → `hubspot.search_companies`
- `workflow_task_id_ref13` → `bc0f54e7057549c8a9ea272aaeda72fb`
- `workflow_task_id_ref6` → `d7748d3800ec4b2f9cc0d4156a339f18`
- `transition_id_ref1` → `${UUID}`
- `workflow_task_id_ref5` → `2b7dfa1c11534f5695739ff0f94ba146`
- `action_ref2` → `hubspot.create_contact`
- `workflow_task_id_ref10` → `bc3bbd22d14846179f373df4fe71f199`
- `workflow_task_id_ref12` → `b834e1c55a174c3b9c5b3318cddcb246`
- `transition_id_ref11` → `${UUID}`
- `transition_id_ref7` → `${UUID}`
- `workflow_task_id_ref9` → `772006ffdbfd481aa80d07ce560eb6ef`
- `transition_id_ref13` → `${UUID}`
- `transition_id_ref4` → `${UUID}`
- `workflow_task_id_ref8` → `892ce8bf5ef0484fb07b3d9e1cfefa4c`
- `action_ref1` → `core.noop`
- `transition_id_ref5` → `${UUID}`
- `transition_id_ref6` → `${UUID}`
