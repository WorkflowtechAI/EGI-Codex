# [OG] HubSpot CSM Users

## Overview
**Workflow Type:** OPTION_GENERATOR  
**Export Date:** 2025-10-11T16:11:42.724665+00:00  
**Bundle Version:** 2  
**Original Bundle File:** `workflow-018fc71f-079a-740b-ae0b-81a91f640c46_20251011_161142.bundle.json`  

## Workflow Documentation

### Data Retrieval & API Integration

This functional block handles the initial data collection from HubSpot's API to retrieve all user/owner records.

**Purpose**: Fetch comprehensive user data from HubSpot that will be processed to identify Customer Success team members.

**Tasks Included**:
- **hubspot_get_users**: Makes an authenticated GET request to HubSpot's `/crm/v3/owners` endpoint to retrieve all user/owner records with their team associations and profile information.

**Key Functionality**:
- Uses HubSpot's generic API request action with proper authentication
- Retrieves paginated results to ensure all users are captured
- Sets up the foundation data (CTX.hs_users) that subsequent processing tasks will filter and transform

**Inputs**: HubSpot API credentials (configured in the integration)
**Outputs**: Complete list of HubSpot users/owners stored in CTX.hs_users

**Flow Connection**: This block provides the raw user data that feeds into the Data Processing & Filtering block for CS team member identification.

### Data Processing & CS Team Filtering

This functional block processes the raw HubSpot user data to identify Customer Success team members and formats them for output.

**Purpose**: Filter the complete user list to extract only CS team members and transform their data into a user-friendly format suitable for selection interfaces.

**Tasks Included**:
- **filter_cs_users**: Uses Jinja2 templating to filter users based on team membership, identifying those who belong to Customer Success teams (matching CTX.cs_team_ids)
- **format_options**: Transforms the filtered user objects to create display-friendly options with combined first/last names as labels and user IDs as values

**Key Functionality**:
- **Smart Filtering**: Uses conditional logic to check if users have team assignments and if any of their teams match the predefined CS team IDs
- **Data Transformation**: Combines firstName and lastName fields into a single "label" field for better user experience
- **ID Preservation**: Maintains the original user ID for system integration purposes

**Processing Logic**:
```
Filter: user for user in CTX.hs_users 
        if user.teams and any(team.id in CTX.cs_team_ids for team in user.teams)
Transform: firstName + " " + lastName → label, id → id
```

**Inputs**: 
- CTX.hs_users (from Data Retrieval block)
- CTX.cs_team_ids (predefined CS team identifiers)

**Outputs**: 
- CTX.filtered_cs_output (CS team members only)
- Final formatted user options with label/id structure

**Flow Connection**: This block takes the raw user data from the API Integration block and produces the final filtered and formatted CS user list that the workflow returns.

### Workflow Control & Execution Flow

This functional block manages the overall workflow execution flow, providing entry and exit points with proper task sequencing.

**Purpose**: Control the workflow's start, intermediate transitions, and completion states to ensure proper execution order and clean termination.

**Tasks Included**:
- **BEGIN**: Workflow entry point that initiates the execution sequence
- **END (Intermediate)**: Transition control task that manages the flow between data retrieval and processing phases
- **END (Final)**: Workflow termination point that cleanly concludes the execution

**Key Functionality**:
- **Execution Sequencing**: Ensures tasks execute in the correct order: BEGIN → Data Retrieval → Processing → Final END
- **Flow Control**: The intermediate END task provides a decision point that routes execution to the data processing phase
- **Clean Termination**: Final END task ensures the workflow completes properly and returns results

**Execution Flow**:
```
BEGIN → hubspot_get_users → END (Intermediate) → filter_cs_users → format_options → END (Final)
```

**Control Logic**:
- BEGIN triggers the HubSpot API call
- Intermediate END evaluates success and routes to filtering phase
- Final END completes the workflow after data transformation

**Inputs**: Workflow trigger/initiation
**Outputs**: Workflow completion status and final results

**Flow Connection**: This block orchestrates the entire workflow, ensuring the Data Retrieval and Data Processing blocks execute in proper sequence and the workflow terminates correctly with the formatted CS user results.

## Tasks

This workflow contains 6 tasks:

### 1. BEGIN
**Description:** Action that does nothing
**Action:** ``core.noop``
**Next Tasks:** 1 transition(s) defined

### 2. END
**Description:** Action that does nothing
**Action:** ``core.noop``
**Next Tasks:** 1 transition(s) defined

### 3. END
**Description:** Action that does nothing
**Action:** ``core.noop``
**Transition Mode:** FOLLOW_FIRST
**Next Tasks:** 2 transition(s) defined

### 4. filter_cs_users
**Description:** Set a variable as an action
**Action:** ``transforms.set_variable``
**Next Tasks:** 1 transition(s) defined

### 5. format_options
**Description:** Transform a list by manipulating the values of its attributes.
**Action:** ``transforms.transform_fields``
**Next Tasks:** 1 transition(s) defined

### 6. hubspot_get_users
**Description:** Generic action for making authenticated requests against the HubSpot API
**Action:** ``hubspot.generic_request``
**Next Tasks:** 1 transition(s) defined

## Output

This workflow produces the following output:

- **options:** `{{ CTX.format_options }}`

## Technical Details

- **Workflow Timeout:** 28800 seconds
- **Total Tasks:** 6
- **Documentation Sections:** 3

---

*This README was automatically generated from the Rewst workflow export bundle.*
## Workflow Execution Details

### Overview
- **Total Tasks**: 6
- **Workflow Type**: OPTION_GENERATOR
- **Parallel Branches**: 1
- **Join Points**: 0
- **Resolved References**: 20

### Task Flow

#### 1. BEGIN
- **Action**: `core.noop`
- **Task ID**: `07dc85b92a24498f8300ea81c8fce383`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1

#### 2. END
- **Action**: `core.noop`
- **Task ID**: `5cafdd2d782d4507a25ccbac55f3fb00`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 0

#### 3. END
- **Action**: `core.noop`
- **Task ID**: `a085e55f5ebb44acb714480752030aed`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_FIRST
- **Join Count**: 0
- **Next Tasks**: 1

#### 4. filter_cs_users
- **Action**: `transforms.set_variable`
- **Task ID**: `7de04715bf8e4880bf09ec2c7f61e9bb`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

#### 5. format_options
- **Action**: `transforms.transform_fields`
- **Task ID**: `30c01e80c4054f1eb01adcf13863afa1`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

#### 6. hubspot_get_users
- **Action**: `hubspot.generic_request`
- **Task ID**: `1be471ede4374d2f82a805565fb62481`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

### Resolved References
The following references were resolved from @@@placeholder@@@ format to actual values:

- `workflow_note_id_ref2` → `${UUID}`
- `workflow_task_id_ref4` → `7de04715bf8e4880bf09ec2c7f61e9bb`
- `action_ref2` → `transforms.set_variable`
- `transition_id_ref6` → `${UUID}`
- `transition_id_ref2` → `${UUID}`
- `workflow_note_id_ref3` → `${UUID}`
- `workflow_task_id_ref1` → `07dc85b92a24498f8300ea81c8fce383`
- `workflow_task_id_ref5` → `30c01e80c4054f1eb01adcf13863afa1`
- `transition_id_ref4` → `${UUID}`
- `workflow_task_id_ref2` → `5cafdd2d782d4507a25ccbac55f3fb00`
- `action_ref4` → `hubspot.generic_request`
- `action_ref1` → `core.noop`
- `transition_id_ref3` → `${UUID}`
- `action_ref3` → `transforms.transform_fields`
- `workflow_task_id_ref6` → `1be471ede4374d2f82a805565fb62481`
- `workflow_task_id_ref3` → `a085e55f5ebb44acb714480752030aed`
- `transition_id_ref7` → `${UUID}`
- `transition_id_ref5` → `${UUID}`
- `transition_id_ref1` → `${UUID}`
- `workflow_note_id_ref1` → `${UUID}`
