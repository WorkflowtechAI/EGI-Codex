# Discord User Missing Roles [OG]

## Overview
**Workflow Type:** OPTION_GENERATOR  
**Export Date:** 2025-10-11T16:13:50.655267+00:00  
**Bundle Version:** 2  
**Original Bundle File:** `workflow-018fd9a4-7d70-70bd-8eb0-344bfeb45fd8_20251011_161350.bundle.json`  

## Parameters

- **discord_details** (`string`) - Optional
- **requested_roles** (`array`) - Optional

## Workflow Documentation

### Input Processing & Validation

This functional block handles the initial processing and validation of workflow inputs, preparing the data for subsequent Discord API operations.

**Purpose**: Validate and parse incoming form data to extract Discord user information needed for role checking.

**Tasks Included**:
- **validate_input_data**: Validates the incoming form data and ensures required fields are present
- **receive_form_options**: Receives and processes the initial form options/parameters

**Key Processing**:
- Parses the `discord_details` input to extract username and Discord ID
- Publishes structured data (`discord_username`, `discord_id`) for use by downstream tasks
- Acts as the entry point that triggers parallel data collection processes

**Outputs to Next Block**:
- `discord_username`: Parsed Discord username for member search
- `discord_id`: Discord user ID for identification
- Triggers both guild info retrieval and user search operations

**Flow Connection**: This block serves as the workflow's foundation, ensuring clean data flows to the Discord Data Collection phase.

### Discord Data Collection

This functional block handles the parallel collection of essential Discord data required for role analysis, gathering both server-level and user-specific information.

**Purpose**: Retrieve comprehensive Discord guild and member information to enable accurate role comparison and analysis.

**Tasks Included**:
- **get_guild_info_by_id**: Fetches complete guild (server) information including available roles
- **get_guild_member**: Searches for and retrieves specific user information within the guild

**Key Operations**:
- Parallel execution of guild and user data retrieval for efficiency
- Guild info provides the complete role structure and permissions
- Member search identifies the specific user and their current role assignments
- Both tasks target guild ID "${DISCORD_SERVER_ID}"

**Data Retrieved**:
- Guild roles, permissions, and server configuration
- User's current role assignments and member status
- Member profile information for verification

**Outputs to Next Block**:
- `get_roles.roles`: Complete list of available guild roles
- `get_user`: User member information including current roles
- Raw data ready for role filtering and analysis

**Flow Connection**: This block runs in parallel after input validation and feeds processed Discord data into the Role Analysis & Processing phase.

### Role Analysis & Processing

This functional block performs the core business logic of analyzing Discord roles, filtering available roles against user requirements, and identifying missing role assignments.

**Purpose**: Process and analyze Discord role data to determine which roles the user is missing from their requested set.

**Tasks Included**:
- **set_filtered_roles**: Filters guild roles against requested roles to identify relevant ones
- **set_user_role_ids**: Extracts and processes the user's current role assignments

**Key Processing Logic**:
- Filters available guild roles based on `CTX.requested_roles` criteria
- Extracts user's current role IDs from member data using `map(attribute='roles')`
- Creates a filtered list of roles that match the requested criteria
- Prepares role ID lists for comparison operations

**Data Transformations**:
- Converts role objects to ID lists for efficient comparison
- Filters role collections based on business requirements
- Structures data for the final missing role calculation

**Outputs to Next Block**:
- `filtered_role_ids`: List of relevant role IDs from guild roles
- `users_role_ids`: List of role IDs currently assigned to the user
- Processed data ready for missing role identification

**Flow Connection**: This block receives raw Discord data from the collection phase and processes it into structured formats for the Output Generation phase to determine missing roles.

### Output Generation & Results

This functional block generates the final workflow output by identifying missing roles and formatting them into a structured response suitable for user interfaces or downstream systems.

**Purpose**: Create the final output showing which Discord roles the user is missing from their requested set, formatted as selectable options.

**Tasks Included**:
- **send_missing_role_options**: Generates a structured list of missing roles with metadata for UI presentation

**Core Logic**:
- Compares `CTX.filtered_roles` against `CTX.users_role_ids` to identify gaps
- Creates role objects with ID, name, and default selection status
- Filters to only include roles the user doesn't currently have
- Formats output as JSON-compatible option objects

**Output Structure**:
Each missing role is formatted as:
```json
{
  "id": "role_id",
  "name": "Role Name", 
  "default": true
}
```

**Key Features**:
- Only includes roles the user is missing (not currently assigned)
- Provides both role ID and human-readable name
- Sets `default: true` for UI pre-selection
- Ready for form rendering or API responses

**Workflow Completion**: This block represents the final step, delivering actionable results that can be used to:
- Display missing roles to users
- Pre-populate role assignment forms
- Trigger automated role assignment processes
- Generate reports on role gaps

**Flow Connection**: This is the terminal block that consumes processed data from Role Analysis & Processing and produces the workflow's final deliverable.

## Tasks

This workflow contains 7 tasks:

### 1. get_guild_info_by_id
**Description:** Get a guild's information
**Action:** ``discord.get_guild_info``
**Next Tasks:** 1 transition(s) defined

### 2. get_guild_member
**Description:** Search for guild (serverr) members by a query
**Action:** ``discord.search_guild_members``
**Next Tasks:** 1 transition(s) defined

### 3. receive_form_options
**Description:** Action that does nothing
**Action:** ``core.noop``
**Next Tasks:** 2 transition(s) defined

### 4. send_missing_role_options
**Description:** Set a variable as an action
**Action:** ``transforms.set_variable``
**Next Tasks:** 1 transition(s) defined

### 5. set_filtered_roles
**Description:** Set a variable as an action
**Action:** ``transforms.set_variable``
**Next Tasks:** 1 transition(s) defined

### 6. set_user_role_ids
**Description:** Set a variable as an action
**Action:** ``transforms.set_variable``
**Next Tasks:** 1 transition(s) defined

### 7. validate_input_data
**Description:** Action that does nothing
**Action:** ``core.noop``
**Next Tasks:** 1 transition(s) defined

## Output

This workflow produces the following output:

- **options:** `{{ CTX.missing_roles }}`

## Technical Details

- **Workflow Timeout:** 28800 seconds
- **Total Tasks:** 7
- **Documentation Sections:** 4

---

*This README was automatically generated from the Rewst workflow export bundle.*
## Workflow Execution Details

### Overview
- **Total Tasks**: 7
- **Workflow Type**: OPTION_GENERATOR
- **Parallel Branches**: 1
- **Join Points**: 0
- **Resolved References**: 23

### Task Flow

#### 1. get_guild_info_by_id
- **Action**: `discord.get_guild_info`
- **Task ID**: `6117e0850c17489fa4db8a11dce4d781`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

#### 2. get_guild_member
- **Action**: `discord.search_guild_members`
- **Task ID**: `071b91e2088247d89c80df512a4a9ebd`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

#### 3. receive_form_options
- **Action**: `core.noop`
- **Task ID**: `48a3bb2dfd0b403ea9703a1f15b26ce7`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 2

#### 4. send_missing_role_options
- **Action**: `transforms.set_variable`
- **Task ID**: `482a57624972454093bd8f24b790b26a`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 0
- **Has Input Configuration**: Yes

#### 5. set_filtered_roles
- **Action**: `transforms.set_variable`
- **Task ID**: `7f54fe2390704c2c934545b9bd777770`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

#### 6. set_user_role_ids
- **Action**: `transforms.set_variable`
- **Task ID**: `94929f6973f346ecb0cc8811cbe64406`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

#### 7. validate_input_data
- **Action**: `core.noop`
- **Task ID**: `b09829219d6e4a3c8896b1960575fe3a`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1

### Resolved References
The following references were resolved from @@@placeholder@@@ format to actual values:

- `transition_id_ref6` → `${UUID}`
- `action_ref2` → `discord.search_guild_members`
- `action_ref3` → `core.noop`
- `transition_id_ref8` → `${UUID}`
- `workflow_task_id_ref6` → `94929f6973f346ecb0cc8811cbe64406`
- `workflow_note_id_ref2` → `${UUID}`
- `workflow_note_id_ref4` → `${UUID}`
- `workflow_task_id_ref1` → `6117e0850c17489fa4db8a11dce4d781`
- `workflow_task_id_ref4` → `482a57624972454093bd8f24b790b26a`
- `transition_id_ref4` → `${UUID}`
- `workflow_note_id_ref1` → `${UUID}`
- `workflow_note_id_ref3` → `${UUID}`
- `transition_id_ref7` → `${UUID}`
- `transition_id_ref5` → `${UUID}`
- `workflow_task_id_ref3` → `48a3bb2dfd0b403ea9703a1f15b26ce7`
- `workflow_task_id_ref7` → `b09829219d6e4a3c8896b1960575fe3a`
- `transition_id_ref2` → `${UUID}`
- `action_ref4` → `transforms.set_variable`
- `transition_id_ref3` → `${UUID}`
- `action_ref1` → `discord.get_guild_info`
- `workflow_task_id_ref5` → `7f54fe2390704c2c934545b9bd777770`
- `workflow_task_id_ref2` → `071b91e2088247d89c80df512a4a9ebd`
- `transition_id_ref1` → `${UUID}`
