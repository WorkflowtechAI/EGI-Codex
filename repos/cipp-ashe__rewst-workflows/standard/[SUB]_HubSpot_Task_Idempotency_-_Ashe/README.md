# [SUB] HubSpot Task Idempotency - Ashe

## Overview
**Workflow Type:** STANDARD  
**Export Date:** 2025-10-11T16:17:22.073258+00:00  
**Bundle Version:** 2  
**Original Bundle File:** `workflow-5fc69cd1-1e9d-41e0-abbb-d476c00b3783_20251011_161722.bundle.json`  

## Parameters

- **company_id** (`string`) - Optional
- **contact_id** (`string`) - Optional
- **training_id** (`string`) - Optional
- **hs_timestamp** (`string`) - Optional
- **hs_meeting_body** (`string`) - Optional
- **hs_activity_type** (`string`) - Optional
- **hs_meeting_title** (`string`) - Optional
- **meeting_end_time** (`string`) - Optional
- **hs_meeting_outcome** (`string`) - Optional
- **meeting_start_time** (`string`) - Optional

## Workflow Documentation

### Task Existence Check & Routing Logic

This functional block handles the core idempotency logic by checking if a HubSpot meeting/task already exists and routing the workflow accordingly.

**Purpose**: Prevent duplicate task creation by searching for existing meetings with matching timestamp and title criteria.

**Tasks Included**:
- **BEGIN**: Workflow initialization and entry point
- **list_tasks**: Searches HubSpot for existing meetings using timestamp and title filters via POST to /crm/v3/objects/meetings/search
- **CHECK_FOR_TASK**: Decision point that evaluates search results to determine next action

**Logic Flow**:
1. Workflow starts at BEGIN task
2. list_tasks performs a filtered search for meetings matching the input criteria (hs_timestamp and hs_meeting_title)
3. CHECK_FOR_TASK evaluates if any results were found (CTX.search_tasks.results|length>0)
   - If task exists → routes to TASK_EXISTS path
   - If no task found → routes to TASK_NEEDED path

**Key Outputs**:
- Search results stored in CTX.search_tasks for downstream decision making
- Conditional routing based on existence check results

**Error Handling**: 
- list_tasks failure routes directly to END_FAIL for graceful error handling

### New Task Creation Process

This functional block handles the creation of new HubSpot meeting/task records when no existing task is found during the idempotency check.

**Purpose**: Create a new meeting record in HubSpot with all required properties and associations when the task doesn't already exist.

**Tasks Included**:
- **TASK_NEEDED**: Routing task that initiates the creation process when no existing task is found
- **post_attendance_as_task**: Creates the new meeting record with comprehensive data and associations

**Creation Process**:
1. TASK_NEEDED serves as the entry point for new task creation
2. post_attendance_as_task performs a POST request to /crm/v3/objects/meetings with:
   - Meeting properties (timestamp, title, body, outcome, start/end times, activity type)
   - Automatic associations to contact (type 200) and company (type 188)
   - Conditional association to training record if training_id is provided (user-defined type 8)

**Key Features**:
- **Rich Meeting Data**: Includes all standard HubSpot meeting properties
- **Multi-Entity Associations**: Automatically links to contacts, companies, and optionally training records
- **Template-Based JSON**: Uses Jinja2 templating for dynamic data insertion
- **Conditional Logic**: Handles optional training association with {% if CTX.training_id %} logic

**Success Path**: 
- Sets engagement_id from created meeting (CTX.create_task.id)
- Sets task_result to "task created successfully"
- Routes to END_SUCCESS

**Error Handling**: 
- Creation failure routes to END_FAIL with "task creation failed" message

### Existing Task Association Management

This functional block manages associations for existing HubSpot meeting/task records when a duplicate is detected during the idempotency check.

**Purpose**: When a meeting already exists, ensure it has the proper associations to contacts and companies by adding any missing relationships.

**Tasks Included**:
- **TASK_EXISTS**: Entry point when an existing task is found, extracts engagement_id from search results
- **add_contact_association**: Creates association between existing meeting and contact via PUT request
- **add_company_association**: Creates association between existing meeting and company via PUT request

**Association Process**:
1. TASK_EXISTS extracts the existing meeting ID (CTX.search_tasks.results[0].id) and stores as engagement_id
2. Both association tasks run in parallel to link the meeting to:
   - **Contact Association**: PUT to /crm/v3/objects/meetings/{engagement_id}/associations/contacts/{contact_id}/200
   - **Company Association**: PUT to /crm/v3/objects/meetings/{engagement_id}/associations/companies/{company_id}/188

**Key Features**:
- **Parallel Execution**: Both associations run simultaneously for efficiency
- **Standard Association Types**: Uses HubSpot standard association type IDs (200 for contacts, 188 for companies)
- **Idempotent Operations**: PUT requests are safe to retry and won't create duplicates
- **Existing ID Reuse**: Leverages the found meeting ID rather than creating new records

**Success Paths**: 
- Both tasks set task_result to "task associated successfully"
- Both route to END_SUCCESS upon completion

**Error Handling**: 
- Either association failure routes to END_FAIL with "task association failed" message
- Maintains workflow integrity even if one association fails

### Workflow Completion & Result Handling

This functional block handles the final workflow completion states and result formatting for both successful and failed execution paths.

**Purpose**: Provide consistent output formatting and proper workflow termination regardless of success or failure scenarios.

**Tasks Included**:
- **END_SUCCESS**: Successful completion endpoint that formats positive results
- **END_FAIL**: Failure completion endpoint that formats error results

**Result Processing**:
Both endpoints use core.noop actions but serve as critical result aggregation points:

**Success Output Structure**:
```json
{
  "result": CTX.task_result,
  "engagement_id": CTX.engagement_id
}
```

**Failure Output Structure**:
```json
{
  "result": CTX.task_result,
  "engagement_id": CTX.engagement_id|d  // Uses default filter for safety
}
```

**Key Features**:
- **Consistent Output Format**: Both success and failure paths return structured JSON with result and engagement_id
- **Safe Failure Handling**: END_FAIL uses Jinja2 default filter (|d) to handle cases where engagement_id might not be set
- **Result Context Preservation**: Maintains task_result messages set by upstream tasks for debugging and monitoring
- **Workflow State Clarity**: Clear distinction between successful completion and error states

**Input Sources**:
- **task_result**: Set by various upstream tasks with descriptive messages:
  - "task created successfully" (from new task creation)
  - "task associated successfully" (from association operations)  
  - "task creation failed" / "task association failed" (from error scenarios)
- **engagement_id**: Meeting/task ID from either search results or newly created records

**Usage Context**: These endpoints serve as the final data collection points for external systems consuming this workflow's results.

## Tasks

This workflow contains 10 tasks:

### 1. BEGIN
**Description:** Action that does nothing
**Action:** ``core.noop``
**Next Tasks:** 1 transition(s) defined

### 2. CHECK_FOR_TASK
**Description:** Action that does nothing
**Action:** ``core.noop``
**Transition Mode:** FOLLOW_FIRST
**Next Tasks:** 2 transition(s) defined

### 3. END_FAIL
**Description:** Action that does nothing
**Action:** ``core.noop``
**Join:** 1 (waits for 1 incoming transitions)
**Next Tasks:** 1 transition(s) defined

### 4. END_SUCCESS
**Description:** Action that does nothing
**Action:** ``core.noop``
**Join:** 1 (waits for 1 incoming transitions)
**Next Tasks:** 1 transition(s) defined

### 5. TASK_EXISTS
**Description:** Action that does nothing
**Action:** ``core.noop``
**Next Tasks:** 1 transition(s) defined

### 6. TASK_NEEDED
**Description:** Action that does nothing
**Action:** ``core.noop``
**Next Tasks:** 1 transition(s) defined

### 7. add_company_association
**Description:** Post a meeting in HubSpot with the latest updates and details of the escalated ticket.
**Action:** ``hubspot.generic_request``
**Transition Mode:** FOLLOW_FIRST
**Join:** 1 (waits for 1 incoming transitions)
**Next Tasks:** 2 transition(s) defined

### 8. add_contact_association
**Description:** Post a meeting in HubSpot with the latest updates and details of the escalated ticket.
**Action:** ``hubspot.generic_request``
**Transition Mode:** FOLLOW_FIRST
**Join:** 1 (waits for 1 incoming transitions)
**Next Tasks:** 2 transition(s) defined

### 9. list_tasks
**Description:** Post a meeting in HubSpot with the latest updates and details of the escalated ticket.
**Action:** ``hubspot.generic_request``
**Transition Mode:** FOLLOW_FIRST
**Join:** 1 (waits for 1 incoming transitions)
**Next Tasks:** 2 transition(s) defined

### 10. post_attendance_as_task
**Description:** Post a meeting in HubSpot with the latest updates and details of the escalated ticket.
**Action:** ``hubspot.generic_request``
**Transition Mode:** FOLLOW_FIRST
**Join:** 1 (waits for 1 incoming transitions)
**Next Tasks:** 2 transition(s) defined

## Output

This workflow produces the following output:

- **output:** `{{ CTX.output }}`

## Technical Details

- **Workflow Timeout:** 28800 seconds
- **Total Tasks:** 10
- **Documentation Sections:** 4

---

*This README was automatically generated from the Rewst workflow export bundle.*
## Workflow Execution Details

### Overview
- **Total Tasks**: 10
- **Workflow Type**: STANDARD
- **Parallel Branches**: 5
- **Join Points**: 6
- **Resolved References**: 31

### Task Flow

#### 1. BEGIN
- **Action**: `core.noop`
- **Task ID**: `176565b24f504f999ee13f900ee892ac`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1

#### 2. CHECK_FOR_TASK
- **Action**: `core.noop`
- **Task ID**: `c05684c5c318404e9cd115e6b166d671`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_FIRST
- **Join Count**: 0
- **Next Tasks**: 2

#### 3. END_FAIL
- **Action**: `core.noop`
- **Task ID**: `78a766b27e4c4dc8916a033d60887e5d`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 1
- **Next Tasks**: 0

#### 4. END_SUCCESS
- **Action**: `core.noop`
- **Task ID**: `0a2625de5e444b9f90535dc296be7ba6`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 1
- **Next Tasks**: 0

#### 5. TASK_EXISTS
- **Action**: `core.noop`
- **Task ID**: `6f79c031dc7c42d6bc6da144f6d4a742`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 2

#### 6. TASK_NEEDED
- **Action**: `core.noop`
- **Task ID**: `db6a961e745342ecaa2e63b771412471`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1

#### 7. add_company_association
- **Action**: `hubspot.generic_request`
- **Task ID**: `366b4426b1d1407da7ca73521e0d095a`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_FIRST
- **Join Count**: 1
- **Next Tasks**: 2
- **Has Input Configuration**: Yes

#### 8. add_contact_association
- **Action**: `hubspot.generic_request`
- **Task ID**: `62c98051fea34444bac1496577dcc858`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_FIRST
- **Join Count**: 1
- **Next Tasks**: 2
- **Has Input Configuration**: Yes

#### 9. list_tasks
- **Action**: `hubspot.generic_request`
- **Task ID**: `6811fa61665d4844a5dd07b2b303d33b`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_FIRST
- **Join Count**: 1
- **Next Tasks**: 2
- **Has Input Configuration**: Yes

#### 10. post_attendance_as_task
- **Action**: `hubspot.generic_request`
- **Task ID**: `a2117fb980634eb9848cbd5d5d16d1d2`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_FIRST
- **Join Count**: 1
- **Next Tasks**: 2
- **Has Input Configuration**: Yes

### Resolved References
The following references were resolved from @@@placeholder@@@ format to actual values:

- `workflow_note_id_ref2` → `${UUID}`
- `workflow_task_id_ref8` → `62c98051fea34444bac1496577dcc858`
- `action_ref1` → `core.noop`
- `workflow_task_id_ref1` → `176565b24f504f999ee13f900ee892ac`
- `transition_id_ref14` → `${UUID}`
- `transition_id_ref15` → `${UUID}`
- `transition_id_ref12` → `${UUID}`
- `transition_id_ref4` → `${UUID}`
- `transition_id_ref10` → `${UUID}`
- `workflow_task_id_ref2` → `c05684c5c318404e9cd115e6b166d671`
- `transition_id_ref1` → `${UUID}`
- `transition_id_ref6` → `${UUID}`
- `workflow_task_id_ref3` → `78a766b27e4c4dc8916a033d60887e5d`
- `transition_id_ref7` → `${UUID}`
- `transition_id_ref9` → `${UUID}`
- `workflow_task_id_ref4` → `0a2625de5e444b9f90535dc296be7ba6`
- `workflow_task_id_ref6` → `db6a961e745342ecaa2e63b771412471`
- `workflow_task_id_ref7` → `366b4426b1d1407da7ca73521e0d095a`
- `action_ref2` → `hubspot.generic_request`
- `workflow_note_id_ref1` → `${UUID}`
- `workflow_note_id_ref3` → `${UUID}`
- `workflow_note_id_ref4` → `${UUID}`
- `transition_id_ref2` → `${UUID}`
- `transition_id_ref11` → `${UUID}`
- `transition_id_ref5` → `${UUID}`
- `transition_id_ref3` → `${UUID}`
- `workflow_task_id_ref9` → `6811fa61665d4844a5dd07b2b303d33b`
- `transition_id_ref13` → `${UUID}`
- `transition_id_ref8` → `${UUID}`
- `workflow_task_id_ref5` → `6f79c031dc7c42d6bc6da144f6d4a742`
- `workflow_task_id_ref10` → `a2117fb980634eb9848cbd5d5d16d1d2`
