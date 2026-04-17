# [Discord v2] Role Addition & Removal

## Overview
**Workflow Type:** STANDARD  
**Export Date:** 2025-10-11T16:11:46.973644+00:00  
**Bundle Version:** 2  
**Original Bundle File:** `workflow-018fe9e2-f000-7417-be94-1978de1804c9_20251011_161146.bundle.json`  

## Parameters

- **user_id** (`string`) - Optional
- **username** (`string`) - Optional
- **roles_to_add** (`array`) - Optional
- **roles_to_remove** (`array`) - Optional

## Workflow Documentation

### Input Processing & Initialization

This functional block handles the initial processing of webhook input and workflow initialization.

**Purpose**: Processes incoming webhook data and prepares the workflow context with user information and role operations to be performed.

**Tasks Included**:
- `from_webhook`: Entry point that receives webhook data containing user_id, username, roles_to_add, and roles_to_remove
- `input`: Initializes the audit log array and prepares the workflow context

**Key Functionality**:
- Extracts user identification data (user_id, username) from webhook payload
- Captures role operation requests (roles_to_add, roles_to_remove)
- Initializes empty audit log array for tracking all operations
- Routes to role lookup functionality based on available data

**Inputs**: Webhook payload with user and role information
**Outputs**: Structured context variables (user_id, username, roles_to_add, roles_to_remove, audit_log)

**Flow Connection**: This block feeds into the User Identification & Lookup block to find the target Discord user.

### User Identification & Lookup

This functional block handles Discord user identification and role information retrieval.

**Purpose**: Locates the target Discord user and retrieves necessary role information for subsequent operations.

**Tasks Included**:
- `get_roles`: Retrieves all available roles from the Discord guild and converts role names to IDs
- `check_username_or_id`: Determines whether to search by user ID or username
- `discord_search_guild_members`: Searches for guild members by username query
- `user_not_found`: Handles cases where user lookup fails

**Key Functionality**:
- Fetches complete role list from Discord guild for name-to-ID mapping
- Converts role names in requests to actual Discord role IDs
- Supports user lookup by either direct user ID or username search
- Performs guild member search when username is provided
- Captures user information including join date and user details
- Logs successful user identification or failure cases

**Decision Logic**:
- If user_id is provided → proceeds directly to role operations
- If username is provided → performs guild member search
- If user found → extracts user_id and metadata, continues to role operations
- If user not found → logs error and proceeds to output

**Inputs**: user_id or username, guild role requirements
**Outputs**: validated user_id, user_info, date_joined, role ID mappings, audit log entries

**Flow Connection**: This block receives user data from Input Processing and feeds validated user information to Role Management Operations.

### Role Management Operations

This functional block executes the core Discord role management operations for adding and removing user roles.

**Purpose**: Performs the actual role addition and removal operations on Discord users, with comprehensive error handling and audit logging.

**Tasks Included**:
- `check_add_or_remove_roles`: Evaluates which role operations need to be performed
- `add_role`: Adds specified roles to the user via Discord API
- `check_if_removal_needed`: Determines if role removal operations are required
- `remove_role`: Removes specified roles from the user via Discord API
- `failed`: Handles role addition failures and logs error information

**Key Functionality**:
- Supports both role addition and removal in a single workflow execution
- Uses Discord API endpoints for role management (PUT for add, DELETE for remove)
- Implements with_items iteration for handling multiple roles per operation
- Captures operation timestamps and success/failure status
- Provides detailed audit logging for each role operation
- Handles API failures gracefully with error logging

**Operation Flow**:
1. Evaluates available role operations (add/remove)
2. If roles_to_add exists → executes role addition via Discord API
3. If roles_to_remove exists → executes role removal via Discord API
4. Logs success with timestamps and role details
5. On failure → captures error information and continues workflow

**Error Handling**:
- Role addition failures are caught and logged with detailed error information
- Workflow continues even if some operations fail
- All operations are tracked in audit log regardless of success/failure

**Inputs**: user_id, roles_to_add (IDs), roles_to_remove (IDs)
**Outputs**: Updated audit log with operation results, timestamps, success/failure status

**Flow Connection**: This block receives validated user and role data from User Identification & Lookup and feeds operation results to Audit Logging & Output.

### Audit Logging & Output Generation

This functional block handles the final audit logging and output generation for the workflow.

**Purpose**: Consolidates all operation results into a comprehensive audit log and generates the final workflow output.

**Tasks Included**:
- `output`: Generates the final workflow output containing the complete audit log

**Key Functionality**:
- Consolidates audit log entries from all workflow operations
- Provides comprehensive tracking of all user lookup and role management activities
- Generates structured output for external systems or logging
- Serves as the single source of truth for workflow execution results

**Audit Log Structure**:
Each audit log entry contains:
- `message`: Operation type description
- `code`: HTTP-style status code (200 for success, 404 for not found, 500 for errors)
- `information`: Detailed description of the operation and results
- `user_id`: Target user identifier (when available)
- `date_joined`: User's Discord join date (for successful lookups)
- `date_added`/`date_removed`: Timestamps for role operations
- `roles_added`/`roles_removed`: Lists of roles affected by operations

**Output Categories**:
- User identification results (found/not found)
- Role addition operations (success/failure)
- Role removal operations (success/failure)
- Error conditions and failure details

**Inputs**: Complete audit_log array with all workflow operations
**Outputs**: Structured audit log for external consumption

**Flow Connection**: This is the terminal block that receives results from all other functional blocks and produces the final workflow output.

## Tasks

This workflow contains 12 tasks:

### 1. add_role
**Description:** Adds the Verified User Role to the User after successfully confirming their identity via email.
**Action:** ``discord.add_role_to_user``
**Time Savings:** 30 seconds
**Join:** 1 (waits for 1 incoming transitions)
**Loop/Iteration:** Yes
**Next Tasks:** 2 transition(s) defined

### 2. check_add_or_remove_roles
**Description:** Set a variable as an action
**Action:** ``transforms.set_variable``
**Transition Mode:** FOLLOW_FIRST
**Join:** 1 (waits for 1 incoming transitions)
**Next Tasks:** 2 transition(s) defined

### 3. check_if_removal_needed
**Description:** Action that does nothing
**Action:** ``core.noop``
**Transition Mode:** FOLLOW_FIRST
**Next Tasks:** 2 transition(s) defined

### 4. check_username_or_id
**Description:** Action that does nothing
**Action:** ``core.noop``
**Transition Mode:** FOLLOW_FIRST
**Next Tasks:** 2 transition(s) defined

### 5. discord_search_guild_members
**Description:** Search for guild (serverr) members by a query
**Action:** ``discord.search_guild_members``
**Transition Mode:** FOLLOW_FIRST
**Next Tasks:** 2 transition(s) defined

### 6. failed
**Description:** Action that does nothing
**Action:** ``core.noop``
**Next Tasks:** 1 transition(s) defined

### 7. from_webhook
**Description:** Action that does nothing
**Action:** ``core.noop``
**Transition Mode:** FOLLOW_FIRST
**Next Tasks:** 2 transition(s) defined

### 8. get_roles
**Description:** Generic API call to the channel's permissions endpoint to add the newly created / updated role as having permissions to see / talk in this channel.
**Action:** ``discord.generic_request``
**Time Savings:** 30 seconds
**Next Tasks:** 1 transition(s) defined

### 9. input
**Description:** Action that does nothing
**Action:** ``core.noop``
**Timeout:** None seconds
**Next Tasks:** 1 transition(s) defined

### 10. output
**Description:** Set a variable as an action
**Action:** ``transforms.set_variable``
**Join:** 1 (waits for 1 incoming transitions)
**Next Tasks:** 1 transition(s) defined

### 11. remove_role
**Description:** Generic action for making authenticated requests against the Discord API to remove a role from a user
**Action:** ``discord.generic_request``
**Join:** 1 (waits for 1 incoming transitions)
**Loop/Iteration:** Yes
**Next Tasks:** 1 transition(s) defined

### 12. user_not_found
**Description:** Set a variable as an action
**Action:** ``transforms.set_variable``
**Next Tasks:** 1 transition(s) defined

## Output

This workflow produces the following output:

- **output:** `{{ CTX.audit_log }}`

## Technical Details

- **Workflow Timeout:** 28800 seconds
- **Total Tasks:** 12
- **Documentation Sections:** 4

---

*This README was automatically generated from the Rewst workflow export bundle.*
## Workflow Execution Details

### Overview
- **Total Tasks**: 12
- **Workflow Type**: STANDARD
- **Parallel Branches**: 6
- **Join Points**: 4
- **Resolved References**: 39

### Task Flow

#### 1. add_role
- **Action**: `discord.add_role_to_user`
- **Task ID**: `065ea644e5737d648000dc9ce1632d3c`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 1
- **Next Tasks**: 2
- **Has Input Configuration**: Yes
- **Loop/Iterator**: Yes

#### 2. check_add_or_remove_roles
- **Action**: `transforms.set_variable`
- **Task ID**: `065ea644e5737b898000f8ef2b3b7691`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_FIRST
- **Join Count**: 1
- **Next Tasks**: 2
- **Has Input Configuration**: Yes

#### 3. check_if_removal_needed
- **Action**: `core.noop`
- **Task ID**: `065ea644e55c7eb98000809f3595c6a0`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_FIRST
- **Join Count**: 0
- **Next Tasks**: 2

#### 4. check_username_or_id
- **Action**: `core.noop`
- **Task ID**: `418d57dc710a4f989419f73abe03da04`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_FIRST
- **Join Count**: 0
- **Next Tasks**: 2

#### 5. discord_search_guild_members
- **Action**: `discord.search_guild_members`
- **Task ID**: `065ea644e55c7b088000ccea23c40d05`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_FIRST
- **Join Count**: 0
- **Next Tasks**: 2
- **Has Input Configuration**: Yes

#### 6. failed
- **Action**: `core.noop`
- **Task ID**: `14f768800fe347bea17dd23dd0360258`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 0

#### 7. from_webhook
- **Action**: `core.noop`
- **Task ID**: `0b5621b2a9ac4db699234cbab43dba6e`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_FIRST
- **Join Count**: 0
- **Next Tasks**: 2

#### 8. get_roles
- **Action**: `discord.generic_request`
- **Task ID**: `92935c49958644199da4fe0e00517d5d`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

#### 9. input
- **Action**: `core.noop`
- **Task ID**: `065ea644e55d703280009143b71882d7`
- **Timeout**: Nones
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1

#### 10. output
- **Action**: `transforms.set_variable`
- **Task ID**: `065ea644e573797b8000cab2b5395a49`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 1
- **Next Tasks**: 0
- **Has Input Configuration**: Yes

#### 11. remove_role
- **Action**: `discord.generic_request`
- **Task ID**: `065ea644e55c7d218000b090a99d319d`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 1
- **Next Tasks**: 1
- **Has Input Configuration**: Yes
- **Loop/Iterator**: Yes

#### 12. user_not_found
- **Action**: `transforms.set_variable`
- **Task ID**: `065ea644e57375b98000821d9cadaf2a`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

### Resolved References
The following references were resolved from @@@placeholder@@@ format to actual values:

- `workflow_task_id_ref5` → `065ea644e55c7b088000ccea23c40d05`
- `workflow_note_id_ref1` → `${UUID}`
- `transition_id_ref3` → `${UUID}`
- `transition_id_ref2` → `${UUID}`
- `workflow_note_id_ref2` → `${UUID}`
- `workflow_task_id_ref12` → `065ea644e57375b98000821d9cadaf2a`
- `transition_id_ref15` → `${UUID}`
- `workflow_task_id_ref1` → `065ea644e5737d648000dc9ce1632d3c`
- `workflow_task_id_ref2` → `065ea644e5737b898000f8ef2b3b7691`
- `workflow_task_id_ref9` → `065ea644e55d703280009143b71882d7`
- `workflow_task_id_ref4` → `418d57dc710a4f989419f73abe03da04`
- `transition_id_ref10` → `${UUID}`
- `transition_id_ref5` → `${UUID}`
- `transition_id_ref4` → `${UUID}`
- `action_ref1` → `discord.add_role_to_user`
- `transition_id_ref17` → `${UUID}`
- `transition_id_ref6` → `${UUID}`
- `workflow_task_id_ref7` → `0b5621b2a9ac4db699234cbab43dba6e`
- `workflow_task_id_ref10` → `065ea644e573797b8000cab2b5395a49`
- `action_ref3` → `core.noop`
- `action_ref2` → `transforms.set_variable`
- `transition_id_ref16` → `${UUID}`
- `transition_id_ref8` → `${UUID}`
- `workflow_note_id_ref4` → `${UUID}`
- `transition_id_ref13` → `${UUID}`
- `transition_id_ref11` → `${UUID}`
- `action_ref4` → `discord.search_guild_members`
- `workflow_task_id_ref6` → `14f768800fe347bea17dd23dd0360258`
- `workflow_task_id_ref11` → `065ea644e55c7d218000b090a99d319d`
- `transition_id_ref1` → `${UUID}`
- `workflow_task_id_ref3` → `065ea644e55c7eb98000809f3595c6a0`
- `transition_id_ref7` → `${UUID}`
- `workflow_task_id_ref8` → `92935c49958644199da4fe0e00517d5d`
- `transition_id_ref18` → `${UUID}`
- `workflow_note_id_ref3` → `${UUID}`
- `transition_id_ref14` → `${UUID}`
- `action_ref5` → `discord.generic_request`
- `transition_id_ref9` → `${UUID}`
- `transition_id_ref12` → `${UUID}`
