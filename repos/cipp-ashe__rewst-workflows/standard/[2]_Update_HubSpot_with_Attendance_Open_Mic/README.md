# [2] Update HubSpot with Attendance (Open Mic)

## Overview
**Workflow Type:** STANDARD  
**Export Date:** 2025-10-11T16:15:27.576404+00:00  
**Bundle Version:** 2  
**Original Bundle File:** `workflow-d2b44931-cf51-4d76-bfc0-ee1616a7e3c0_20251011_161527.bundle.json`  

## Parameters

- **email** (`string`) - Optional
- **left_at** (`string`) - Optional
- **joined_at** (`string`) - Optional
- **last_name** (`string`) - Optional
- **first_name** (`string`) - Optional
- **meeting_end_time** (`string`) - Optional
- **meeting_start_time** (`string`) - Optional

## Tasks

This workflow contains 14 tasks:

### 1. BEGIN
**Description:** Action that does nothing
**Action:** ``core.noop``
**Next Tasks:** 1 transition(s) defined

### 2. company_found
**Description:** Action that does nothing
**Action:** ``core.noop``
**Next Tasks:** 1 transition(s) defined

### 3. contact_found
**Description:** Action that does nothing
**Action:** ``core.noop``
**Next Tasks:** 1 transition(s) defined

### 4. contact_updated
**Description:** Delays the workflow for a certain amount of time.
**Action:** ``core.delay_by_time``
**Timeout:** 3600 seconds
**Join:** 1 (waits for 1 incoming transitions)
**Next Tasks:** 1 transition(s) defined

### 5. end_no_account
**Description:** Action that does nothing
**Action:** ``core.noop``
**Next Tasks:** 1 transition(s) defined

### 6. hub_spot_create_contact
**Description:** create a contact in hubspot
**Action:** ``hubspot.create_contact``
**Next Tasks:** 1 transition(s) defined

### 7. hub_spot_search_companies
**Description:** search companies in hubspot
**Action:** ``hubspot.search_companies``
**Transition Mode:** FOLLOW_FIRST
**Next Tasks:** 2 transition(s) defined

### 8. hub_spot_search_contacts
**Description:** search contacts in hubspot
**Action:** ``hubspot.search_contacts``
**Transition Mode:** FOLLOW_FIRST
**Next Tasks:** 2 transition(s) defined

### 9. hub_spot_update_contact
**Description:** update a contact in hubspot
**Action:** ``hubspot.update_contact``
**Next Tasks:** 1 transition(s) defined

### 10. meeting_created
**Description:** Action that does nothing
**Action:** ``core.noop``
**Next Tasks:** 1 transition(s) defined

### 11. no_company
**Description:** Action that does nothing
**Action:** ``core.noop``
**Next Tasks:** 1 transition(s) defined

### 12. no_contact
**Description:** Action that does nothing
**Action:** ``core.noop``
**Next Tasks:** 1 transition(s) defined

### 13. slack_chat_post_message
**Description:** Sends a message to a channel.
**Action:** ``slack.chat.postMessage``
**Next Tasks:** 1 transition(s) defined

### 14. workflows_sub_hub_spot_task_idempotency
**Action:** `Unknown`
**Next Tasks:** 1 transition(s) defined

## Technical Details

- **Workflow Timeout:** 28800 seconds
- **Total Tasks:** 14

---

*This README was automatically generated from the Rewst workflow export bundle.*
## Workflow Execution Details

### Overview
- **Total Tasks**: 14
- **Workflow Type**: STANDARD
- **Parallel Branches**: 2
- **Join Points**: 1
- **Resolved References**: 38

### Task Flow

#### 1. BEGIN
- **Action**: `core.noop`
- **Task ID**: `f11234685f814070ad1ec6e93a1e2b10`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1

#### 2. company_found
- **Action**: `core.noop`
- **Task ID**: `5bbd9f3a7c5948eda34f1cc12648abd1`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1

#### 3. contact_found
- **Action**: `core.noop`
- **Task ID**: `a8bf924478a94b9bac2007a7a0b86749`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1

#### 4. contact_updated
- **Action**: `core.delay_by_time`
- **Task ID**: `9e5a92d37dd9408e807dd3f0e2713317`
- **Timeout**: 3600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 1
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

#### 5. end_no_account
- **Action**: `core.noop`
- **Task ID**: `b182ab7df4e9431b9b6a3cc7168facca`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 0

#### 6. hub_spot_create_contact
- **Action**: `hubspot.create_contact`
- **Task ID**: `ba5f46c9fd114409ac7eaf2312f5303e`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

#### 7. hub_spot_search_companies
- **Action**: `hubspot.search_companies`
- **Task ID**: `25ba82281c774e318cb7a2bb0c8b77e1`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_FIRST
- **Join Count**: 0
- **Next Tasks**: 2
- **Has Input Configuration**: Yes

#### 8. hub_spot_search_contacts
- **Action**: `hubspot.search_contacts`
- **Task ID**: `65a4fc78106a4e799fcb2e691c305319`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_FIRST
- **Join Count**: 0
- **Next Tasks**: 2
- **Has Input Configuration**: Yes

#### 9. hub_spot_update_contact
- **Action**: `hubspot.update_contact`
- **Task ID**: `bcbbbaa8e02041aeb866f60181bed413`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

#### 10. meeting_created
- **Action**: `core.noop`
- **Task ID**: `29481e65047145cfa15fbebc1f239b22`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 0

#### 11. no_company
- **Action**: `core.noop`
- **Task ID**: `6d04b7ef19b447fd956bc4fff09e38c6`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1

#### 12. no_contact
- **Action**: `core.noop`
- **Task ID**: `bd56fdb63c064539be7d5f88c5b0044b`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1

#### 13. slack_chat_post_message
- **Action**: `slack.chat.postMessage`
- **Task ID**: `bd1dc37fa0c7407dbfdeb1e8e3aefef3`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

#### 14. workflows_sub_hub_spot_task_idempotency
- **Action**: `Unknown Action`
- **Task ID**: `70970eff21b94a58b34a55a353664c12`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

### Resolved References
The following references were resolved from @@@placeholder@@@ format to actual values:

- `workflow_task_id_ref1` → `f11234685f814070ad1ec6e93a1e2b10`
- `transition_id_ref1` → `${UUID}`
- `action_ref2` → `core.delay_by_time`
- `action_ref3` → `hubspot.create_contact`
- `workflow_task_id_ref7` → `25ba82281c774e318cb7a2bb0c8b77e1`
- `workflow_task_id_ref14` → `70970eff21b94a58b34a55a353664c12`
- `transition_id_ref2` → `${UUID}`
- `transition_id_ref4` → `${UUID}`
- `workflow_task_id_ref5` → `b182ab7df4e9431b9b6a3cc7168facca`
- `transition_id_ref12` → `${UUID}`
- `action_ref6` → `hubspot.update_contact`
- `transition_id_ref11` → `${UUID}`
- `transition_id_ref10` → `${UUID}`
- `transition_id_ref3` → `${UUID}`
- `workflow_task_id_ref11` → `6d04b7ef19b447fd956bc4fff09e38c6`
- `workflow_task_id_ref10` → `29481e65047145cfa15fbebc1f239b22`
- `workflow_task_id_ref4` → `9e5a92d37dd9408e807dd3f0e2713317`
- `transition_id_ref6` → `${UUID}`
- `action_ref4` → `hubspot.search_companies`
- `transition_id_ref8` → `${UUID}`
- `action_ref5` → `hubspot.search_contacts`
- `action_ref7` → `slack.chat.postMessage`
- `transition_id_ref15` → `${UUID}`
- `transition_id_ref9` → `${UUID}`
- `transition_id_ref16` → `${UUID}`
- `workflow_task_id_ref8` → `65a4fc78106a4e799fcb2e691c305319`
- `workflow_task_id_ref3` → `a8bf924478a94b9bac2007a7a0b86749`
- `workflow_task_id_ref6` → `ba5f46c9fd114409ac7eaf2312f5303e`
- `action_ref1` → `core.noop`
- `workflow_task_id_ref9` → `bcbbbaa8e02041aeb866f60181bed413`
- `transition_id_ref5` → `${UUID}`
- `transition_id_ref14` → `${UUID}`
- `transition_id_ref13` → `${UUID}`
- `transition_id_ref7` → `${UUID}`
- `workflow_task_id_ref2` → `5bbd9f3a7c5948eda34f1cc12648abd1`
- `workflow_task_id_ref13` → `bd1dc37fa0c7407dbfdeb1e8e3aefef3`
- `workflow_task_id_ref12` → `bd56fdb63c064539be7d5f88c5b0044b`
- `workflow_ref1` → `[SUBWORKFLOW: [SUB] HubSpot Task Idempotency - Ashe]`
