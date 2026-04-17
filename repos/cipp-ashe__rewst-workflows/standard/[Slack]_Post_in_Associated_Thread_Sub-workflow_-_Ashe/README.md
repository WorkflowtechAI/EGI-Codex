# [Slack] Post in Associated Thread (Sub-workflow) - Ashe

## Overview
**Workflow Type:** STANDARD  
**Export Date:** 2025-10-11T16:15:10.669318+00:00  
**Bundle Version:** 2  
**Original Bundle File:** `workflow-018f6a3d-d7b5-7f25-b739-d4b385597b4c_20251011_161510.bundle.json`  

## Parameters

- **text** (`string`) - Optional
- **blocks** (`string`) - Optional
- **org_id** (`string`) - Optional
- **channel** (`string`) - Optional
- **broadcast** (`string`) - Optional
- **identifier** (`string`) - Optional

## Workflow Documentation

### Thread Reference Lookup & Validation

## Purpose
This functional block handles the initial phase of the workflow by looking up existing Slack thread references and validating whether a message should be posted to an existing thread or to the main channel.

## Key Tasks
- **BEGIN**: Workflow initialization with no-op action
- **rewst_get_external_reference**: Retrieves external thread reference using organization ID and identifier

## Process Flow
1. Workflow starts with BEGIN task (initialization)
2. Attempts to retrieve external reference for the given identifier and organization
3. Evaluates whether a thread reference exists:
   - **Reference Exists Path**: If thread ID found, sets `thread_id` and creates reference log with association details
   - **No Reference Path**: If no thread found, sets empty `thread_id` and logs "No thread ID found - send to channel"

## Key Outputs
- `thread_id`: Either the found thread ID or empty string
- `reference_log`: Detailed logging object containing identifier, reference_id, association_id, execution_id, and org_id

## Connection to Next Block
This block determines the posting behavior for the message posting block - whether to post as a thread reply or as a new channel message.

### Slack Message Posting

## Purpose
This functional block handles the core Slack message posting operation, sending the message either to a thread (if thread_id exists) or to the main channel, and manages the immediate response handling.

## Key Task
- **slack_chat_post_message**: Executes the Slack API call to post the message with comprehensive configuration options

## Process Flow
1. Receives inputs from the thread reference lookup block
2. Posts message to Slack using the chat.postMessage API with:
   - Text content and/or blocks for rich formatting
   - Channel targeting
   - Thread targeting (if thread_id provided)
   - Custom emoji icon (:cluck-u:)
   - Broadcast settings for thread replies
3. Handles two outcome paths:
   - **Success Path**: Captures message timestamp and creates success log
   - **Failure Path**: Creates failure log with "Not Created" status

## Key Inputs
- `text`: Message text content
- `blocks`: Rich formatting blocks
- `channel`: Target Slack channel
- `thread_id`: Optional thread timestamp for replies
- `broadcast`: Whether thread replies should be visible to all

## Key Outputs
- `message_id`: Slack message timestamp (on success)
- `slack_log`: Logging object with identifier and message creation status

## Connection to Next Block
This block feeds its results (success/failure status and logging data) to the result processing block for final workflow completion and audit logging.

### Result Processing & Audit Logging

## Purpose
This functional block handles the final workflow phase by consolidating all execution data, merging logging information from different workflow stages, and providing comprehensive audit trails for both successful and failed message posting attempts.

## Key Tasks
- **END_SUCCESS**: Processes successful message posting outcomes and merges audit logs
- **END_FAIL**: Handles failed message posting scenarios and consolidates error information

## Process Flow
1. Receives execution results from the message posting block
2. Both success and failure paths perform identical data consolidation:
   - Merges `slack_log` (message posting results) with `reference_log` (thread lookup results)
   - Uses outer join method to preserve all information from both sources
   - Matches records based on the `identifier` field
3. Creates comprehensive audit record containing:
   - Original workflow inputs and identifiers
   - Thread reference lookup results
   - Message posting outcomes
   - Execution metadata and timestamps

## Data Consolidation Strategy
- **Merge Method**: Outer join to retain all entries from both log sources
- **Matching Key**: `identifier` field ensures proper correlation
- **Result**: Single consolidated record with complete workflow execution history

## Key Outputs
- Merged audit log containing complete workflow execution trace
- Unified data structure for downstream reporting and analysis
- Preservation of both successful operations and failure scenarios

## Workflow Completion
This block represents the terminal phase of the workflow, ensuring all execution data is properly consolidated and available for audit, reporting, and troubleshooting purposes.

## Tasks

This workflow contains 5 tasks:

### 1. BEGIN
**Description:** Action that does nothing
**Action:** ``core.noop``
**Next Tasks:** 1 transition(s) defined

### 2. END_FAIL
**Description:** Merge two lists by matching items based on a specified key attribute.
**Action:** ``transforms.merge_lists``
**Next Tasks:** 1 transition(s) defined

### 3. END_SUCCESS
**Description:** Merge two lists by matching items based on a specified key attribute.
**Action:** ``transforms.merge_lists``
**Join:** 1 (waits for 1 incoming transitions)
**Next Tasks:** 1 transition(s) defined

### 4. rewst_get_external_reference
**Action:** ``rewst.get_foreign_object_reference``
**Transition Mode:** FOLLOW_FIRST
**Next Tasks:** 2 transition(s) defined

### 5. slack_chat_post_message
**Description:** Sends a message to a channel.
**Action:** ``slack.chat.postMessage``
**Next Tasks:** 2 transition(s) defined

## Output

This workflow produces the following output:

- **output:** `{{ CTX.output|first }}`

## Technical Details

- **Workflow Timeout:** 28800 seconds
- **Total Tasks:** 5
- **Documentation Sections:** 3

---

*This README was automatically generated from the Rewst workflow export bundle.*
## Workflow Execution Details

### Overview
- **Total Tasks**: 5
- **Workflow Type**: STANDARD
- **Parallel Branches**: 2
- **Join Points**: 1
- **Resolved References**: 19

### Task Flow

#### 1. BEGIN
- **Action**: `core.noop`
- **Task ID**: `5987449a4ddc4c808ab2fa478df3e137`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1

#### 2. END_FAIL
- **Action**: `transforms.merge_lists`
- **Task ID**: `5a660c8cdd5f4473bf20f2b8a40fcd0e`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 0
- **Has Input Configuration**: Yes

#### 3. END_SUCCESS
- **Action**: `transforms.merge_lists`
- **Task ID**: `6cb9b193ec544725b212d3e44a1f2f62`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 1
- **Next Tasks**: 0
- **Has Input Configuration**: Yes

#### 4. rewst_get_external_reference
- **Action**: `rewst.get_foreign_object_reference`
- **Task ID**: `4402a3620b234e2598b508d8964507bb`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_FIRST
- **Join Count**: 0
- **Next Tasks**: 2
- **Has Input Configuration**: Yes

#### 5. slack_chat_post_message
- **Action**: `slack.chat.postMessage`
- **Task ID**: `804f82eb052f432cafdf78b237510fe9`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 2
- **Has Input Configuration**: Yes

### Resolved References
The following references were resolved from @@@placeholder@@@ format to actual values:

- `workflow_task_id_ref3` → `6cb9b193ec544725b212d3e44a1f2f62`
- `workflow_note_id_ref1` → `${UUID}`
- `action_ref1` → `core.noop`
- `workflow_task_id_ref1` → `5987449a4ddc4c808ab2fa478df3e137`
- `action_ref3` → `rewst.get_foreign_object_reference`
- `transition_id_ref3` → `${UUID}`
- `action_ref2` → `transforms.merge_lists`
- `workflow_note_id_ref2` → `${UUID}`
- `transition_id_ref1` → `${UUID}`
- `transition_id_ref2` → `${UUID}`
- `transition_id_ref5` → `${UUID}`
- `transition_id_ref7` → `${UUID}`
- `workflow_task_id_ref2` → `5a660c8cdd5f4473bf20f2b8a40fcd0e`
- `workflow_task_id_ref5` → `804f82eb052f432cafdf78b237510fe9`
- `workflow_note_id_ref3` → `${UUID}`
- `workflow_task_id_ref4` → `4402a3620b234e2598b508d8964507bb`
- `action_ref4` → `slack.chat.postMessage`
- `transition_id_ref6` → `${UUID}`
- `transition_id_ref4` → `${UUID}`
