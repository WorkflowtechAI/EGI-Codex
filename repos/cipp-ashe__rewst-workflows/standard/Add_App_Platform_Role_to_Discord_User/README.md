# Add App Platform Role to Discord User

## Overview
**Workflow Type:** STANDARD  
**Export Date:** 2025-10-11T16:14:36.148485+00:00  
**Bundle Version:** 2  
**Original Bundle File:** `workflow-018ea9d5-81d6-7b50-9aa1-daeb13103012_20251011_161436.bundle.json`  

## Parameters

- **email** (`string`) - Optional
- **username** (`string`) - Optional
- **alpha_role** (`string`) - Optional
- **channel_name** (`string`) - Optional
- **alpha_channel** (`string`) - Optional

## Workflow Documentation

### Channel Discovery & User Lookup

## Purpose
This functional block handles the initial discovery and validation phase of the workflow, locating the appropriate Discord channel and searching for the target user.

## Tasks Included
- **BEGIN** (`b512ca6136da41278cc6901528ef6737`): Workflow initialization and entry point
- **discord_list_channels** (`9eb517f3b51244f48a816149b7c6010d`): Retrieves all channels from the Discord guild to find the company-specific channel
- **discord_search_guild_members** (`ef8538ad451c4714876ec566449e6ea1`): Searches for the target user by username in the Discord server
- **collected_channel_details** (`d85a08cd364e4468958c9843fa2308fb`): Processes channel information and extracts user IDs with specific permissions

## Workflow Logic
1. **Channel Discovery**: Lists all channels in the Discord guild and attempts to find a channel matching the company name
2. **User Search**: If no company channel is found, searches for the individual user by username
3. **Permission Analysis**: When a company channel is found, analyzes permission overwrites to identify users with view channel permissions

## Key Outputs
- `channel_id`: ID of the discovered company channel
- `channel_name`: Name of the company channel
- `user_id`: ID of the target user (from search results)
- `user_ids`: Array of user IDs with channel access permissions

## Flow Control
- **Channel Found**: Proceeds to bulk role assignment for all channel members
- **User Found**: Proceeds to individual role assignment and thread creation
- **User Not Found**: Proceeds to failure notification workflow

### Role Assignment & Thread Management

## Purpose
This functional block handles the core Discord operations: assigning the App Platform Alpha role to users and creating dedicated communication threads for personalized onboarding.

## Tasks Included
- **discord_add_alpha_role** (`91f5e8e6591d47adbcbba09e92b61f91`): Adds the App Platform Alpha role to individual users found through search
- **discord_add_alpha_role** (`12a14c1a21ea4f08a7d46e0c49293dcc`): Bulk role assignment for all users in a company channel (with_items iteration)
- **discord_create_thread** (`15dc116bd4c14fe4b4c0536bc7325805`): Creates a personalized welcome thread in the Alpha channel
- **discord_add_user** (`103d608fc3c0496b95b8ede79ab4c1ed`): Adds the target user to the newly created thread

## Workflow Logic
1. **Role Assignment**: Assigns the App Platform Alpha role (ID: ${ROLE_ID_APP_PLATFORM_BETA}) to users
   - Individual assignment for users found via search
   - Bulk assignment for all users with permissions in company channels
2. **Thread Creation**: Creates a personalized welcome thread with the user's name in the App Platform Alpha channel
3. **Thread Membership**: Adds the target user to their welcome thread for direct communication

## Key Features
- **Conditional Execution**: Role assignment method depends on whether a company channel was found
- **Bulk Processing**: Uses `with_items` to process multiple users from company channels
- **Personalization**: Thread names include the username for personalized experience
- **Guild Configuration**: All operations target the specific Discord guild (ID: ${DISCORD_SERVER_ID})

## Key Outputs
- `thread_id`: ID of the created welcome thread for subsequent messaging
- Role assignment confirmations for tracking success/failure

## Integration Points
- **Input**: Receives user IDs from the discovery phase
- **Output**: Provides thread ID for the communication phase

### User Communication & Notifications

## Purpose
This functional block handles all user-facing communications, delivering welcome messages through Discord and sending email notifications to administrators about the workflow outcomes.

## Tasks Included
- **discord_post_thread_message** (`08532735f16c4cc9ad64adf0c1e24578`): Sends personalized welcome message in the user's thread
- **discord_message_channel** (`c9ebcb6fdd044462bb88333a0da4e038`): Sends team-wide welcome message in company channels
- **core_sendmail** (`11bf4403d9b8408da01fe223240ce4b5`): Sends success notification email to administrators
- **core_sendmail** (`15d44cb9b04b47b28a57e4ac281f98d5`): Sends failure notification email when user cannot be found

## Workflow Logic
1. **Discord Communications**: Delivers contextual welcome messages based on user discovery results
   - **Individual Thread**: Personal welcome message with App Platform resources and direct support contacts
   - **Company Channel**: Team-wide announcement about App Platform access with channel-specific guidance
2. **Email Notifications**: Provides administrative feedback about workflow execution
   - **Success Email**: Confirms role addition and communication method used
   - **Failure Email**: Reports inability to find user or company channel

## Message Content Features
- **Resource Links**: Includes App Platform overview video and open mic recordings
- **Support Contacts**: Provides specific user mentions for technical assistance
- **Channel References**: Uses dynamic channel IDs for proper Discord linking
- **Personalization**: Incorporates usernames and company names throughout messages

## Administrative Tracking
- **Success Tracking**: Records where the user was notified (thread vs company channel)
- **Failure Reporting**: Documents search attempts and provides guidance for manual resolution
- **Email Recipients**: Sends notifications to the requesting administrator's email

## Key Variables
- `updated`: Dynamic text describing where the user was contacted
- `alpha_channel`: Reference to the main App Platform Alpha channel
- `username`: Target user's Discord username
- `channel_name`: Company channel name (when applicable)

## Integration Points
- **Input**: Receives thread IDs, channel IDs, and user information from previous phases
- **Output**: Completes the workflow with user onboarding and administrative confirmation

## Tasks

This workflow contains 12 tasks:

### 1. BEGIN
**Description:** Action that does nothing
**Action:** ``core.noop``
**Transition Mode:** FOLLOW_FIRST
**Next Tasks:** 1 transition(s) defined

### 2. collected_channel_details
**Description:** Set a variable as an action
**Action:** ``transforms.set_variable``
**Next Tasks:** 1 transition(s) defined

### 3. core_sendmail
**Description:** This sends an email
**Action:** ``core.sendmail``
**Join:** 1 (waits for 1 incoming transitions)
**Next Tasks:** 1 transition(s) defined

### 4. core_sendmail
**Description:** This sends an email
**Action:** ``core.sendmail``
**Next Tasks:** 1 transition(s) defined

### 5. discord_add_alpha_role
**Description:** Add a role to a user
**Action:** ``discord.add_role_to_user``
**Next Tasks:** 1 transition(s) defined

### 6. discord_add_alpha_role
**Description:** Add a role to a user
**Action:** ``discord.add_role_to_user``
**Loop/Iteration:** Yes
**Next Tasks:** 1 transition(s) defined

### 7. discord_add_user
**Description:** Generic action for making authenticated requests against the Discord API
**Action:** ``discord.generic_request``
**Join:** 1 (waits for 1 incoming transitions)
**Next Tasks:** 1 transition(s) defined

### 8. discord_create_thread
**Description:** Generic action for making authenticated requests against the Discord API
**Action:** ``discord.generic_request``
**Join:** 1 (waits for 1 incoming transitions)
**Next Tasks:** 1 transition(s) defined

### 9. discord_list_channels
**Description:** List all channels in a guild (server)
**Action:** ``discord.list_guild_channels``
**Transition Mode:** FOLLOW_FIRST
**Next Tasks:** 2 transition(s) defined

### 10. discord_message_channel
**Action:** ``discord.create_channel_message``
**Time Savings:** 30 seconds
**Join:** 1 (waits for 1 incoming transitions)
**Next Tasks:** 1 transition(s) defined

### 11. discord_post_thread_message
**Description:** Create a message in a channel
**Action:** ``discord.create_channel_message``
**Next Tasks:** 1 transition(s) defined

### 12. discord_search_guild_members
**Description:** Search for guild (serverr) members by a query
**Action:** ``discord.search_guild_members``
**Transition Mode:** FOLLOW_FIRST
**Next Tasks:** 2 transition(s) defined

## Technical Details

- **Workflow Timeout:** 28800 seconds
- **Total Tasks:** 12
- **Documentation Sections:** 3

---

*This README was automatically generated from the Rewst workflow export bundle.*
## Workflow Execution Details

### Overview
- **Total Tasks**: 12
- **Workflow Type**: STANDARD
- **Parallel Branches**: 2
- **Join Points**: 4
- **Resolved References**: 37

### Task Flow

#### 1. BEGIN
- **Action**: `core.noop`
- **Task ID**: `b512ca6136da41278cc6901528ef6737`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_FIRST
- **Join Count**: 0
- **Next Tasks**: 1

#### 2. collected_channel_details
- **Action**: `transforms.set_variable`
- **Task ID**: `d85a08cd364e4468958c9843fa2308fb`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

#### 3. core_sendmail
- **Action**: `core.sendmail`
- **Task ID**: `11bf4403d9b8408da01fe223240ce4b5`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 1
- **Next Tasks**: 0
- **Has Input Configuration**: Yes

#### 4. core_sendmail
- **Action**: `core.sendmail`
- **Task ID**: `15d44cb9b04b47b28a57e4ac281f98d5`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 0
- **Has Input Configuration**: Yes

#### 5. discord_add_alpha_role
- **Action**: `discord.add_role_to_user`
- **Task ID**: `91f5e8e6591d47adbcbba09e92b61f91`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

#### 6. discord_add_alpha_role
- **Action**: `discord.add_role_to_user`
- **Task ID**: `12a14c1a21ea4f08a7d46e0c49293dcc`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1
- **Has Input Configuration**: Yes
- **Loop/Iterator**: Yes

#### 7. discord_add_user
- **Action**: `discord.generic_request`
- **Task ID**: `103d608fc3c0496b95b8ede79ab4c1ed`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 1
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

#### 8. discord_create_thread
- **Action**: `discord.generic_request`
- **Task ID**: `15dc116bd4c14fe4b4c0536bc7325805`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 1
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

#### 9. discord_list_channels
- **Action**: `discord.list_guild_channels`
- **Task ID**: `9eb517f3b51244f48a816149b7c6010d`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_FIRST
- **Join Count**: 0
- **Next Tasks**: 2
- **Has Input Configuration**: Yes

#### 10. discord_message_channel
- **Action**: `discord.create_channel_message`
- **Task ID**: `c9ebcb6fdd044462bb88333a0da4e038`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 1
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

#### 11. discord_post_thread_message
- **Action**: `discord.create_channel_message`
- **Task ID**: `08532735f16c4cc9ad64adf0c1e24578`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

#### 12. discord_search_guild_members
- **Action**: `discord.search_guild_members`
- **Task ID**: `ef8538ad451c4714876ec566449e6ea1`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_FIRST
- **Join Count**: 0
- **Next Tasks**: 2
- **Has Input Configuration**: Yes

### Resolved References
The following references were resolved from @@@placeholder@@@ format to actual values:

- `workflow_task_id_ref1` → `b512ca6136da41278cc6901528ef6737`
- `workflow_task_id_ref7` → `103d608fc3c0496b95b8ede79ab4c1ed`
- `transition_id_ref12` → `${UUID}`
- `workflow_task_id_ref6` → `12a14c1a21ea4f08a7d46e0c49293dcc`
- `action_ref1` → `core.noop`
- `workflow_note_id_ref2` → `${UUID}`
- `workflow_task_id_ref3` → `11bf4403d9b8408da01fe223240ce4b5`
- `transition_id_ref13` → `${UUID}`
- `transition_id_ref7` → `${UUID}`
- `action_ref3` → `core.sendmail`
- `workflow_task_id_ref5` → `91f5e8e6591d47adbcbba09e92b61f91`
- `workflow_task_id_ref8` → `15dc116bd4c14fe4b4c0536bc7325805`
- `workflow_task_id_ref12` → `ef8538ad451c4714876ec566449e6ea1`
- `workflow_task_id_ref11` → `08532735f16c4cc9ad64adf0c1e24578`
- `action_ref2` → `transforms.set_variable`
- `transition_id_ref6` → `${UUID}`
- `workflow_task_id_ref4` → `15d44cb9b04b47b28a57e4ac281f98d5`
- `action_ref7` → `discord.create_channel_message`
- `workflow_task_id_ref10` → `c9ebcb6fdd044462bb88333a0da4e038`
- `transition_id_ref11` → `${UUID}`
- `workflow_task_id_ref9` → `9eb517f3b51244f48a816149b7c6010d`
- `transition_id_ref10` → `${UUID}`
- `workflow_note_id_ref3` → `${UUID}`
- `transition_id_ref4` → `${UUID}`
- `transition_id_ref5` → `${UUID}`
- `workflow_note_id_ref1` → `${UUID}`
- `transition_id_ref14` → `${UUID}`
- `transition_id_ref9` → `${UUID}`
- `transition_id_ref8` → `${UUID}`
- `workflow_task_id_ref2` → `d85a08cd364e4468958c9843fa2308fb`
- `transition_id_ref2` → `${UUID}`
- `action_ref4` → `discord.add_role_to_user`
- `action_ref8` → `discord.search_guild_members`
- `transition_id_ref1` → `${UUID}`
- `transition_id_ref3` → `${UUID}`
- `action_ref6` → `discord.list_guild_channels`
- `action_ref5` → `discord.generic_request`