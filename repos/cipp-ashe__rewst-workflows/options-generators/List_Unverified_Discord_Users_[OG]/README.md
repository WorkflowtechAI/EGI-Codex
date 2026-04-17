# List Unverified Discord Users [OG]

## Overview
**Workflow Type:** OPTION_GENERATOR  
**Export Date:** 2025-10-11T16:14:19.966507+00:00  
**Bundle Version:** 2  
**Original Bundle File:** `workflow-018fdbae-fa41-78fe-b3c6-d1287262b7fb_20251011_161419.bundle.json`  

## Workflow Documentation

### Workflow Initialization

This block handles the initial startup of the workflow execution.

**Purpose**: Provides a clean entry point for the workflow and ensures proper initialization before proceeding to data collection operations.

**Tasks Included**:
- `core_noop`: A no-operation task that serves as the workflow entry point and initialization step

**Workflow Connection**: 
- **Input**: Workflow trigger/start
- **Output**: Signals successful initialization to begin Discord data collection
- **Next Block**: Flows into "Discord Data Collection & Pagination" block

**Key Function**: Acts as a reliable starting point that can be used for workflow orchestration, logging, or future initialization logic without affecting the core Discord API operations.

### Discord Data Collection & Pagination

This block handles the retrieval of Discord server member data using paginated API requests to collect comprehensive user information.

**Purpose**: Fetches all Discord server members by making multiple API calls to handle Discord's pagination limits (1000 users per request).

**Tasks Included**:
- `discord_list_users` (First Request): Retrieves the first 1000 server members from Discord API, transforms the response into a structured format with user ID, username, and roles, and captures the last user ID for pagination
- `discord_list_users` (Second Request): Fetches the next 1000 server members using the "after" parameter with the last ID from the previous request

**Data Transformation**: 
Each API response is transformed into a standardized format:
```json
{
  "id": "user_id",
  "username": "username", 
  "roles": ["role_id_1", "role_id_2"]
}
```

**Workflow Connection**:
- **Input**: Initialization signal from the startup block
- **Output**: Two separate lists of user data (`first_1000` and `next_1000`) ready for merging
- **Next Block**: Flows into "Data Processing & Filtering" block

**Key Function**: Ensures complete data collection from Discord's API while respecting rate limits and pagination constraints. The two-step approach handles servers with more than 1000 members effectively.

### Data Processing & Filtering

This block processes the collected Discord user data by merging paginated results and applying role-based filtering to identify unverified users.

**Purpose**: Combines all collected user data and filters it to find users who have a specific role but are missing the verification role, effectively identifying unverified Discord users.

**Tasks Included**:
- `transforms_beta_merge_lists`: Merges the two user lists (`first_1000` and `next_1000`) using an outer join on the user ID field, creating a comprehensive list of all server members without duplicates
- `transforms_set_variable`: Applies role-based filtering logic to identify unverified users by finding members who have role "${ROLE_ID_MEMBER}" but do NOT have role "${ROLE_ID_VERIFIED}"

**Filtering Logic**:
The workflow identifies unverified users using this criteria:
- **Has Role**: `${ROLE_ID_MEMBER}` (likely a general member or participant role)
- **Missing Role**: `${ROLE_ID_VERIFIED}` (likely the verified user role)

**Workflow Connection**:
- **Input**: Two separate user lists from the Discord data collection block
- **Output**: Final filtered list of unverified Discord users (`all_community_members` variable)
- **Next Block**: This is the final processing block - results are ready for consumption

**Key Function**: Transforms raw Discord API data into actionable insights by identifying users who need verification, enabling targeted moderation or engagement actions.

## Tasks

This workflow contains 5 tasks:

### 1. core_noop
**Description:** Action that does nothing
**Action:** ``core.noop``
**Transition Mode:** FOLLOW_FIRST
**Join:** 1 (waits for 1 incoming transitions)
**Next Tasks:** 1 transition(s) defined

### 2. discord_list_users
**Description:** get all users of server
**Action:** ``discord.generic_request``
**Time Savings:** 30 seconds
**Transition Mode:** FOLLOW_FIRST
**Join:** 1 (waits for 1 incoming transitions)
**Next Tasks:** 1 transition(s) defined

### 3. discord_list_users
**Description:** get all users of server
**Action:** ``discord.generic_request``
**Time Savings:** 30 seconds
**Transition Mode:** FOLLOW_FIRST
**Join:** 1 (waits for 1 incoming transitions)
**Next Tasks:** 1 transition(s) defined

### 4. transforms_beta_merge_lists
**Description:** Merge two lists by matching items based on a specified key attribute.
**Action:** ``transforms.merge_lists``
**Next Tasks:** 1 transition(s) defined

### 5. transforms_set_variable
**Description:** Set a variable as an action
**Action:** ``transforms.set_variable``
**Next Tasks:** 1 transition(s) defined

## Output

This workflow produces the following output:

- **options:** `{{ CTX.vertification_needed }}`

## Technical Details

- **Workflow Timeout:** 28800 seconds
- **Total Tasks:** 5
- **Documentation Sections:** 3

---

*This README was automatically generated from the Rewst workflow export bundle.*
## Workflow Execution Details

### Overview
- **Total Tasks**: 5
- **Workflow Type**: OPTION_GENERATOR
- **Parallel Branches**: 0
- **Join Points**: 3
- **Resolved References**: 17

### Task Flow

#### 1. core_noop
- **Action**: `core.noop`
- **Task ID**: `57532e5ee75e49ca9819dec4c7270848`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_FIRST
- **Join Count**: 1
- **Next Tasks**: 1

#### 2. discord_list_users
- **Action**: `discord.generic_request`
- **Task ID**: `737d85027f584c8983eeb1171fe99ea4`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_FIRST
- **Join Count**: 1
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

#### 3. discord_list_users
- **Action**: `discord.generic_request`
- **Task ID**: `b495e8ed77c44103816ece835f5f6dc2`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_FIRST
- **Join Count**: 1
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

#### 4. transforms_beta_merge_lists
- **Action**: `transforms.merge_lists`
- **Task ID**: `74e86c0539404be589712731e22d9c1e`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

#### 5. transforms_set_variable
- **Action**: `transforms.set_variable`
- **Task ID**: `b7897203e4aa458cbe58b4987f50a2b2`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 0
- **Has Input Configuration**: Yes

### Resolved References
The following references were resolved from @@@placeholder@@@ format to actual values:

- `action_ref2` → `discord.generic_request`
- `action_ref4` → `transforms.set_variable`
- `transition_id_ref3` → `${UUID}`
- `workflow_task_id_ref3` → `b495e8ed77c44103816ece835f5f6dc2`
- `workflow_note_id_ref2` → `${UUID}`
- `workflow_task_id_ref1` → `57532e5ee75e49ca9819dec4c7270848`
- `transition_id_ref1` → `${UUID}`
- `transition_id_ref2` → `${UUID}`
- `workflow_task_id_ref2` → `737d85027f584c8983eeb1171fe99ea4`
- `workflow_note_id_ref3` → `${UUID}`
- `action_ref1` → `core.noop`
- `action_ref3` → `transforms.merge_lists`
- `transition_id_ref4` → `${UUID}`
- `workflow_task_id_ref5` → `b7897203e4aa458cbe58b4987f50a2b2`
- `transition_id_ref5` → `${UUID}`
- `workflow_note_id_ref1` → `${UUID}`
- `workflow_task_id_ref4` → `74e86c0539404be589712731e22d9c1e`
