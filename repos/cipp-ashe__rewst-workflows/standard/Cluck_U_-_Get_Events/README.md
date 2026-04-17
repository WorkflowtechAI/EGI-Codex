# Training Program - Get Events

## Overview
**Workflow Type:** STANDARD  
**Export Date:** 2025-10-11T16:10:58.234787+00:00  
**Bundle Version:** 2  
**Original Bundle File:** `workflow-018c7f83-ce53-7b89-8727-7582a56dc688_20251011_161058.bundle.json`  

## Parameters

- **test** (`boolean`) - Optional
- **course** (`string`) - Optional
- **trainer** (`string`) - Optional
- **max_start** (`string`) - Optional
- **min_start** (`string`) - Optional

## Workflow Documentation

### Workflow Initialization & User Validation

This functional block handles the initial setup and user validation for the Training Program event retrieval workflow.

**Purpose**: Initialize workflow context, validate execution mode, and determine user-specific routing based on trainer identity.

**Key Tasks**:
- **BEGIN**: Entry point that captures workflow inputs and sets up execution context
  - Distinguishes between live runs and test runs
  - Establishes date ranges (min_start/max_start) for event queries
  - Captures and filters workflow inputs for downstream processing
  - Sets trainer context to "Training Program"

- **check_user_calendar**: User validation and routing logic
  - Validates if the trainer is "Training Program" 
  - Routes to appropriate processing path based on trainer identity
  - Publishes graph_id and calendly_id for Training Program users
  - Provides fallback routing for other users

**Key Outputs**:
- workflow_inputs: Cleaned input parameters for the workflow
- Date range parameters (min_start, max_start) for calendar queries
- User-specific identifiers (graph_id, calendly_id) for Training Program trainer
- Routing decision for subsequent processing steps

**Flow Connection**: This block feeds into either the Calendar Event Retrieval process (for Training Program users) or directly to the end state (for other users).

### Calendar Event Retrieval & Filtering

This functional block manages the retrieval of calendar events from Calendly and applies filtering logic based on course requirements.

**Purpose**: Fetch scheduled events from Calendly API and filter them based on specific course criteria to identify relevant training sessions.

**Key Tasks**:
- **calendly_list_scheduled_events**: Calendly API integration
  - Makes authenticated GET request to Calendly's scheduled_events endpoint
  - Retrieves events for specific user and organization within date range
  - Transforms raw event data into structured format (id, name)
  - Handles both course-specific and general event retrieval paths
  - Uses pagination parameters (count: 100) for comprehensive event collection

- **transforms_filter_list**: Course-specific event filtering
  - Filters calendar events by course name using exact match comparison
  - Applies filter criteria: event.name == CTX.course
  - Processes the calendar_events array from previous step
  - Returns only events that match the specified course requirement

**Key Data Flow**:
- Input: User ID, organization ID, date range parameters
- Processing: API call → data transformation → course filtering
- Output: Filtered list of calendar events matching course criteria

**Conditional Logic**:
- If course is specified: Events are filtered by course name
- If no course specified: All events within date range are processed

**Flow Connection**: This block receives user context from the Initialization block and feeds filtered event data to the Event Processing block for detailed information retrieval.

### Event Processing & Workflow Completion

This functional block handles the detailed processing of filtered calendar events and manages workflow completion for different user scenarios.

**Purpose**: Retrieve comprehensive event details for matched calendar events and ensure proper workflow termination for all execution paths.

**Key Tasks**:
- **workflows_cluck_u_get_event_details**: Detailed event processing
  - Executes a sub-workflow to gather comprehensive event information
  - Processes each filtered event individually using iteration (item() function)
  - Passes critical parameters: graph_id, event_name, calendar_id
  - Retrieves detailed event data including HubSpot integration updates
  - Returns structured event details for downstream processing or storage

- **end_other_user**: Alternative completion path
  - Provides clean termination for non-Training Program users
  - Ensures workflow completes gracefully when user validation fails
  - Acts as a no-operation endpoint for alternative routing scenarios
  - Maintains workflow integrity across different user types

**Key Processing Logic**:
- Event details are retrieved iteratively for each matched calendar event
- Sub-workflow integration enables complex event processing and external system updates
- Dual completion paths ensure all user scenarios are handled appropriately

**Integration Points**:
- **HubSpot Integration**: Event details workflow includes HubSpot update functionality
- **Graph API**: Uses graph_id for Microsoft Graph integration
- **Calendar Systems**: Processes calendar_id for event-specific operations

**Flow Connection**: This block receives filtered event data from the Calendar Retrieval block and represents the final processing stage, either enriching event data through sub-workflow execution or providing clean termination for alternative user paths.

## Tasks

This workflow contains 6 tasks:

### 1. BEGIN
**Description:** Action that does nothing
**Action:** ``core.noop``
**Next Tasks:** 2 transition(s) defined

### 2. calendly_list_scheduled_events
**Description:** Generic action for making authenticated requests against a custom REST API
**Action:** ``custom.generic_request``
**Transition Mode:** FOLLOW_FIRST
**Join:** 1 (waits for 1 incoming transitions)
**Next Tasks:** 2 transition(s) defined

### 3. check_user_calendar
**Description:** Action that does nothing
**Action:** ``core.noop``
**Transition Mode:** FOLLOW_FIRST
**Join:** 1 (waits for 1 incoming transitions)
**Next Tasks:** 2 transition(s) defined

### 4. end_other_user
**Description:** Action that does nothing
**Action:** ``core.noop``
**Next Tasks:** 1 transition(s) defined

### 5. transforms_filter_list
**Description:** Filter a List by values of its attributes
**Action:** ``transforms.filter_list``
**Next Tasks:** 2 transition(s) defined

### 6. workflows_cluck_u_get_event_details
**Action:** `Unknown`
**Join:** 1 (waits for 1 incoming transitions)
**Loop/Iteration:** Yes
**Next Tasks:** 1 transition(s) defined

## Technical Details

- **Workflow Timeout:** 28800 seconds
- **Total Tasks:** 6
- **Documentation Sections:** 3

---

*This README was automatically generated from the Rewst workflow export bundle.*
## Workflow Execution Details

### Overview
- **Total Tasks**: 6
- **Workflow Type**: STANDARD
- **Parallel Branches**: 4
- **Join Points**: 3
- **Resolved References**: 24

### Task Flow

#### 1. BEGIN
- **Action**: `core.noop`
- **Task ID**: `018c7f83ce507c1d810d60847c0caee3`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 2

#### 2. calendly_list_scheduled_events
- **Action**: `custom.generic_request`
- **Task ID**: `018c7f83ce507c1d810d60832b0a9dac`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_FIRST
- **Join Count**: 1
- **Next Tasks**: 2
- **Has Input Configuration**: Yes

#### 3. check_user_calendar
- **Action**: `core.noop`
- **Task ID**: `018c7f83ce507c1d810d6085e932f5c9`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_FIRST
- **Join Count**: 1
- **Next Tasks**: 2

#### 4. end_other_user
- **Action**: `core.noop`
- **Task ID**: `018c7f83ce4f730bb19b6c7be0a657b2`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 0

#### 5. transforms_filter_list
- **Action**: `transforms.filter_list`
- **Task ID**: `fbb9e04c47c84387b79287c157e5d1f2`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

#### 6. workflows_cluck_u_get_event_details
- **Action**: `Unknown Action`
- **Task ID**: `3dc3003d9a3f4023beb5cf820f380903`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 1
- **Next Tasks**: 0
- **Has Input Configuration**: Yes
- **Loop/Iterator**: Yes

### Resolved References
The following references were resolved from @@@placeholder@@@ format to actual values:

- `workflow_ref1` → `[SUBWORKFLOW: [Training Program] Get Event Details]`
- `transition_id_ref1` → `${UUID}`
- `transition_id_ref4` → `${UUID}`
- `transition_id_ref8` → `${UUID}`
- `transition_id_ref7` → `${UUID}`
- `action_ref2` → `custom.generic_request`
- `action_ref3` → `transforms.filter_list`
- `workflow_task_id_ref1` → `018c7f83ce507c1d810d60847c0caee3`
- `workflow_task_id_ref6` → `3dc3003d9a3f4023beb5cf820f380903`
- `transition_id_ref5` → `${UUID}`
- `workflow_task_id_ref2` → `018c7f83ce507c1d810d60832b0a9dac`
- `workflow_note_id_ref1` → `${UUID}`
- `transition_id_ref2` → `${UUID}`
- `workflow_note_id_ref3` → `${UUID}`
- `pack_ref1` → `custom`
- `transition_id_ref10` → `${UUID}`
- `workflow_task_id_ref3` → `018c7f83ce507c1d810d6085e932f5c9`
- `transition_id_ref6` → `${UUID}`
- `action_ref1` → `core.noop`
- `transition_id_ref3` → `${UUID}`
- `workflow_task_id_ref5` → `fbb9e04c47c84387b79287c157e5d1f2`
- `transition_id_ref9` → `${UUID}`
- `workflow_task_id_ref4` → `018c7f83ce4f730bb19b6c7be0a657b2`
- `workflow_note_id_ref2` → `${UUID}`
