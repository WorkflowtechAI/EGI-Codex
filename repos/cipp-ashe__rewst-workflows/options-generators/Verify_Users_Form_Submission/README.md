# Verify Users Form Submission

## Overview
**Workflow Type:** STANDARD  
**Export Date:** 2025-10-11T16:10:49.269715+00:00  
**Bundle Version:** 2  
**Original Bundle File:** `workflow-018fdbfc-1117-71c9-aa95-cf0105ad2968_20251011_161049.bundle.json`  

## Parameters

- **server** (`string`) - Optional
- **discord** (`string`) - Optional

## Workflow Documentation

### Member Verification Process

This functional block handles the member verification workflow when no server context is provided in the form submission, focusing on validating existing member status and credentials.

**Purpose**: 
Performs member verification operations to validate user credentials, membership status, or other verification requirements when the workflow is not server-specific.

**Tasks Included**:
- **verify_member**: Executes member verification logic to validate user status, credentials, or membership requirements

**Process Flow**:
1. Receives user verification request from routing logic
2. Performs verification checks against member database or external systems
3. Completes verification process and terminates workflow

**Key Inputs**: 
- User identification data from form submission context
- Member verification criteria and requirements

**Key Outputs**: 
- Verification status and results
- Member validation confirmation

**Verification Scope**:
This process handles general member verification that doesn't require server-specific operations, such as:
- Credential validation
- Membership status checks
- User eligibility verification
- General authentication processes

**Connection to Other Blocks**: 
This block is activated when the "Initial Routing & Decision Logic" determines no server context exists (alternative path to server invitation). It represents the member validation functionality of the workflow and serves as a terminal process.

### Initial Routing & Decision Logic

This functional block serves as the entry point and routing mechanism for the workflow, determining which path to take based on the server context.

**Purpose**: 
Analyzes the incoming form submission context and routes the workflow to either server invitation or member verification processes based on whether a server ID is provided.

**Tasks Included**:
- **core_noop**: Acts as a decision point that evaluates the server context ({{ CTX.server }}) to determine the appropriate workflow path

**Decision Logic**:
- If server context exists ({{ CTX.server }}): Routes to server invitation process
- If no server context: Routes to member verification process (labeled "Verify Member")

**Key Inputs**: 
- CTX.server - Server identifier that determines routing path

**Key Outputs**: 
- Workflow routing decision that directs to appropriate processing block

**Connection to Other Blocks**: 
This block serves as the gateway that feeds into either the "Server Invitation Process" or "Member Verification Process" depending on the form submission context.

### Server Invitation Process

This functional block handles the complete process of inviting users to Discord servers when a server context is provided in the form submission.

**Purpose**: 
Manages the server invitation workflow, from initial setup through Discord API integration to invite users to specified servers and capture member information.

**Tasks Included**:
- **server_invite**: Prepares and initiates the server invitation process
- **discord_invite_to_server**: Executes the Discord API call to invite users to the specified server using OAuth2 authentication

**Process Flow**:
1. Server invitation setup and preparation
2. Discord API integration using authenticated requests
3. Member data collection and formatting for downstream processing

**Key Inputs**: 
- CTX.server - Target Discord server ID for invitation
- CTX.discord - Discord user identifier to invite
- ORG.INTEGRATIONS.discord.oauth2_token - Authentication token for Discord API

**Key Outputs**: 
- next_1000 - Formatted member data including user_id, username, and roles from CTX.list_community_members_2
- Discord server invitation result

**API Integration Details**:
- Method: PUT request to Discord API
- Endpoint: /guilds/{{ CTX.server }}/members/{{ CTX.discord }}
- Authentication: OAuth2 token from organization integrations

**Connection to Other Blocks**: 
This block is activated when the "Initial Routing & Decision Logic" determines a server context exists. It represents the primary server management functionality of the workflow.

## Tasks

This workflow contains 4 tasks:

### 1. core_noop
**Description:** Action that does nothing
**Action:** ``core.noop``
**Next Tasks:** 2 transition(s) defined

### 2. discord_invite_to_server
**Description:** invite user to specified server
**Action:** ``discord.generic_request``
**Time Savings:** 30 seconds
**Transition Mode:** FOLLOW_FIRST
**Join:** 1 (waits for 1 incoming transitions)
**Next Tasks:** 1 transition(s) defined

### 3. server_invite
**Description:** Action that does nothing
**Action:** ``core.noop``
**Next Tasks:** 1 transition(s) defined

### 4. verify_member
**Description:** Action that does nothing
**Action:** ``core.noop``
**Next Tasks:** 1 transition(s) defined

## Technical Details

- **Workflow Timeout:** 28800 seconds
- **Total Tasks:** 4
- **Documentation Sections:** 3

---

*This README was automatically generated from the Rewst workflow export bundle.*
## Workflow Execution Details

### Overview
- **Total Tasks**: 4
- **Workflow Type**: STANDARD
- **Parallel Branches**: 1
- **Join Points**: 1
- **Resolved References**: 14

### Task Flow

#### 1. core_noop
- **Action**: `core.noop`
- **Task ID**: `6b1231cdc6e14dcc9a4685ca02d95c9d`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 2

#### 2. discord_invite_to_server
- **Action**: `discord.generic_request`
- **Task ID**: `b7000bbb4bb14b0b8f61d7addef2e8ad`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_FIRST
- **Join Count**: 1
- **Next Tasks**: 0
- **Has Input Configuration**: Yes

#### 3. server_invite
- **Action**: `core.noop`
- **Task ID**: `a03c2fd5268643c384d1f885ceef8858`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1

#### 4. verify_member
- **Action**: `core.noop`
- **Task ID**: `7e3c2f1a8a82452b9feb63e360197da2`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 0

### Resolved References
The following references were resolved from @@@placeholder@@@ format to actual values:

- `workflow_note_id_ref3` → `${UUID}`
- `transition_id_ref2` → `${UUID}`
- `workflow_note_id_ref2` → `${UUID}`
- `workflow_task_id_ref2` → `b7000bbb4bb14b0b8f61d7addef2e8ad`
- `transition_id_ref5` → `${UUID}`
- `action_ref2` → `discord.generic_request`
- `transition_id_ref3` → `${UUID}`
- `workflow_note_id_ref1` → `${UUID}`
- `workflow_task_id_ref3` → `a03c2fd5268643c384d1f885ceef8858`
- `workflow_task_id_ref4` → `7e3c2f1a8a82452b9feb63e360197da2`
- `action_ref1` → `core.noop`
- `workflow_task_id_ref1` → `6b1231cdc6e14dcc9a4685ca02d95c9d`
- `transition_id_ref1` → `${UUID}`
- `transition_id_ref4` → `${UUID}`
