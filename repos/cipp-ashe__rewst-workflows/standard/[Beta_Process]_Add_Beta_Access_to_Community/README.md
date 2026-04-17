# [Beta Process] Add Beta Access to Community

## Overview
**Workflow Type:** STANDARD  
**Export Date:** 2025-10-11T16:11:29.614133+00:00  
**Bundle Version:** 2  
**Original Bundle File:** `workflow-018d2009-77ec-739e-93ab-80aa6b60f2dd_20251011_161129.bundle.json`  

## Parameters

- **email** (`string`) - Optional

## Workflow Documentation

### Contact & Company Discovery Process

This functional block handles the initial discovery and validation of contact and company information from HubSpot. It serves as the foundation for the entire beta access workflow by establishing the key identities and relationships needed for subsequent processing.

**Key Tasks Included:**
- **begin**: Workflow initialization point
- **hs_search_contacts**: Searches HubSpot for the contact using the provided email address
- **hs_search_companies**: Locates the associated company in HubSpot using the email domain
- **rewst_get_organization**: Retrieves the corresponding Rewst organization data

**Process Flow:**
The workflow starts by searching for the contact in HubSpot using their email address. If found, it extracts critical information including the email domain, Discord username, company name, and product user ID. Using the email domain, it then searches for the associated company in HubSpot to retrieve the company ID and organization ID. Finally, it validates that the organization exists in Rewst.

**Key Outputs:**
- Contact details (name, Discord username, product user ID)
- Company information (name, domain, HubSpot company ID)
- Rewst organization ID for access provisioning
- Channel name derived from email domain

**Error Handling:**
The workflow includes fallback paths for scenarios where contacts or companies cannot be found, routing to appropriate "no contact" or "no company" endpoints to gracefully handle missing data.

### Platform Access Provisioning

This functional block manages the core access provisioning process, enabling App Platform (beta) access for the organization and updating the company record to reflect the new access status.

**Key Tasks Included:**
- **enable_app_platform_access**: Makes a GraphQL API call to Rewst to enable the App Platform feature flag for the organization
- **hs_update_company**: Updates the HubSpot company record to mark "App Platform" as the alpha access status

**Process Flow:**
Once the organization is validated, this block enables the App Platform feature flag in Rewst using a GraphQL mutation. The request targets the specific organization ID and sets the feature flag (ID: ${FEATURE_FLAG_APP_PLATFORM}) to enabled. Upon successful activation, it updates the company record in HubSpot to reflect the new access status by setting the "alpha_access" custom property to "App Platform".

**Key Inputs:**
- Organization ID from the discovery process
- Company ID from HubSpot search
- Company name and email domain for record updates

**Success Criteria:**
The access is considered successfully provisioned when:
- The GraphQL response confirms `isEnabled: true` for the feature flag
- The HubSpot company record is updated with the alpha access status

**Integration Points:**
This block connects the Rewst platform configuration with HubSpot CRM tracking, ensuring both systems reflect the customer's new access level. The success of this block determines whether the workflow proceeds to Discord notification or routes to error handling.

### Discord Integration & Community Notification

This functional block handles the Discord community integration, managing role assignments and sending welcome notifications to newly granted beta access users.

**Key Tasks Included:**
- **discord_list_channels**: Retrieves all channels from the Discord guild to locate the appropriate team channel
- **collected_channel_details**: Processes channel data to extract channel ID, name, and authorized user IDs from permission overwrites
- **discord_add_alpha_role**: Assigns the alpha role to authorized users in the Discord server
- **discord_message_channel**: Sends a comprehensive welcome message to the team's Discord channel

**Process Flow:**
After successful access provisioning, the workflow queries Discord to find all channels in the guild (server ID: ${DISCORD_SERVER_ID}). It then locates the specific team channel matching the channel name derived from the email domain. The workflow extracts user IDs from channel permission overwrites (users with '1024' allow permission) and assigns them the alpha role. Finally, it sends a detailed welcome message to the team channel.

**Channel Discovery Logic:**
The workflow uses a sophisticated channel matching system that:
- Searches for channels by name matching the derived channel name
- Extracts user IDs from permission overwrites where type=1 (user) and allow='1024' (view channel permission)
- Publishes the channel ID for subsequent message delivery

**Welcome Message Content:**
The notification includes:
- Congratulatory message about App Platform access
- Link to platform overview recording
- Reference to latest open mic session
- Instructions for support channels
- Note about SSL certificate delays (10-20 minutes for new apps/domains)

**Error Handling:**
Includes fallback paths for scenarios where Discord channels cannot be found or accessed, routing to "no_discord" endpoints to handle integration failures gracefully.

### Error Handling & Workflow Termination

This functional block manages error scenarios and workflow termination points, ensuring graceful handling of various failure conditions throughout the beta access process.

**Key Tasks Included:**
- **no_company**: Handles cases where no matching company is found in HubSpot
- **no_org**: Manages scenarios where the Rewst organization cannot be located or accessed
- **no_alpha**: Addresses situations where App Platform access cannot be enabled
- **no_discord**: Handles Discord integration failures or missing channels
- **no_contact**: Manages cases where the contact cannot be found in HubSpot
- **end**: Successful workflow completion endpoint

**Error Scenarios Covered:**

**Contact/Company Discovery Failures:**
- Contact not found in HubSpot search results
- Company not located using email domain
- Missing or invalid organization mapping

**Access Provisioning Failures:**
- Organization not found in Rewst
- Feature flag activation unsuccessful
- GraphQL API errors or authentication issues

**Discord Integration Failures:**
- Guild channels not accessible
- Target channel not found by name
- Permission issues with role assignment
- Message delivery failures

**Workflow Design Pattern:**
Each major functional block includes conditional branching that routes failed operations to appropriate "no_*" tasks. These tasks use the core.noop action, which serves as a clean termination point without causing workflow errors. This pattern ensures that partial failures don't crash the entire process and provides clear audit trails for troubleshooting.

**Monitoring & Debugging:**
The error handling structure makes it easy to identify where failures occur in the beta access process, enabling quick resolution of common issues like missing HubSpot records or Discord permission problems.

## Tasks

This workflow contains 16 tasks:

### 1. begin
**Description:** Action that does nothing
**Action:** ``core.noop``
**Next Tasks:** 1 transition(s) defined

### 2. collected_channel_details
**Description:** Set a variable as an action
**Action:** ``transforms.set_variable``
**Next Tasks:** 1 transition(s) defined

### 3. discord_add_alpha_role
**Description:** Add a role to a user
**Action:** ``discord.add_role_to_user``
**Loop/Iteration:** Yes
**Next Tasks:** 1 transition(s) defined

### 4. discord_list_channels
**Description:** List all channels in a guild (server)
**Action:** ``discord.list_guild_channels``
**Transition Mode:** FOLLOW_FIRST
**Next Tasks:** 2 transition(s) defined

### 5. discord_message_channel
**Action:** ``discord.create_channel_message``
**Time Savings:** 30 seconds
**Join:** 1 (waits for 1 incoming transitions)
**Next Tasks:** 1 transition(s) defined

### 6. enable_app_platform_access
**Description:** Perform an HTTP request
**Action:** ``core.http_request``
**Transition Mode:** FOLLOW_FIRST
**Next Tasks:** 2 transition(s) defined

### 7. end
**Description:** Action that does nothing
**Action:** ``core.noop``
**Next Tasks:** 1 transition(s) defined

### 8. hs_search_companies
**Description:** search companies in hubspot
**Action:** ``hubspot.search_companies``
**Transition Mode:** FOLLOW_FIRST
**Next Tasks:** 2 transition(s) defined

### 9. hs_search_contacts
**Description:** search contacts in hubspot
**Action:** ``hubspot.search_contacts``
**Transition Mode:** FOLLOW_FIRST
**Next Tasks:** 2 transition(s) defined

### 10. hs_update_company
**Description:** update a company in hubspot
**Action:** ``hubspot.update_company``
**Next Tasks:** 1 transition(s) defined

### 11. no_alpha
**Description:** Action that does nothing
**Action:** ``core.noop``
**Next Tasks:** 1 transition(s) defined

### 12. no_company
**Description:** Action that does nothing
**Action:** ``core.noop``
**Next Tasks:** 1 transition(s) defined

### 13. no_contact
**Description:** Action that does nothing
**Action:** ``core.noop``
**Next Tasks:** 1 transition(s) defined

### 14. no_discord
**Description:** Action that does nothing
**Action:** ``core.noop``
**Next Tasks:** 1 transition(s) defined

### 15. no_org
**Description:** Action that does nothing
**Action:** ``core.noop``
**Next Tasks:** 1 transition(s) defined

### 16. rewst_get_organization
**Description:** Get data for a single organization in Rewst
**Action:** ``rewst.get_organization``
**Transition Mode:** FOLLOW_FIRST
**Next Tasks:** 2 transition(s) defined

## Technical Details

- **Workflow Timeout:** 28800 seconds
- **Total Tasks:** 16
- **Documentation Sections:** 4

---

*This README was automatically generated from the Rewst workflow export bundle.*
## Workflow Execution Details

### Overview
- **Total Tasks**: 16
- **Workflow Type**: STANDARD
- **Parallel Branches**: 5
- **Join Points**: 1
- **Resolved References**: 51

### Task Flow

#### 1. begin
- **Action**: `core.noop`
- **Task ID**: `d195a24e3dea4ac88b3d5f52a0e9a3ef`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1

#### 2. collected_channel_details
- **Action**: `transforms.set_variable`
- **Task ID**: `bfacc78d186f48299d37bb5a9efc5b3e`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

#### 3. discord_add_alpha_role
- **Action**: `discord.add_role_to_user`
- **Task ID**: `68bccda0e36e4656af2ea48b9a903c51`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1
- **Has Input Configuration**: Yes
- **Loop/Iterator**: Yes

#### 4. discord_list_channels
- **Action**: `discord.list_guild_channels`
- **Task ID**: `9f115bd9df434ce185cf381b211210b3`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_FIRST
- **Join Count**: 0
- **Next Tasks**: 2
- **Has Input Configuration**: Yes

#### 5. discord_message_channel
- **Action**: `discord.create_channel_message`
- **Task ID**: `5e831d109925464d84b0d2fd1b4d941a`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 1
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

#### 6. enable_app_platform_access
- **Action**: `core.http_request`
- **Task ID**: `6a1786dc46a9424b87086acaf837a050`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_FIRST
- **Join Count**: 0
- **Next Tasks**: 2
- **Has Input Configuration**: Yes

#### 7. end
- **Action**: `core.noop`
- **Task ID**: `dcd25fef8fa44c588bb5223249a8d7e0`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 0

#### 8. hs_search_companies
- **Action**: `hubspot.search_companies`
- **Task ID**: `28a5fdca6f5b4d3a90ed3924e57f25e5`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_FIRST
- **Join Count**: 0
- **Next Tasks**: 2
- **Has Input Configuration**: Yes

#### 9. hs_search_contacts
- **Action**: `hubspot.search_contacts`
- **Task ID**: `8743f19a1dba4d84b55b20189b8c581b`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_FIRST
- **Join Count**: 0
- **Next Tasks**: 2
- **Has Input Configuration**: Yes

#### 10. hs_update_company
- **Action**: `hubspot.update_company`
- **Task ID**: `4b296bcf261749de9415f646dbf7d79c`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

#### 11. no_alpha
- **Action**: `core.noop`
- **Task ID**: `cce508c6b8df454a8e1b579c3577fadf`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 0

#### 12. no_company
- **Action**: `core.noop`
- **Task ID**: `ee4ead0fb59a4fcf8c98a36bc401ec3c`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 0

#### 13. no_contact
- **Action**: `core.noop`
- **Task ID**: `a49a210c78a240248e5658bc05d53ad1`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 0

#### 14. no_discord
- **Action**: `core.noop`
- **Task ID**: `6623669284b345bb9353ffe2ef86a395`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 0

#### 15. no_org
- **Action**: `core.noop`
- **Task ID**: `a42c856a97ba4a93a6dc2f327e54d036`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 0

#### 16. rewst_get_organization
- **Action**: `rewst.get_organization`
- **Task ID**: `f035e40021154204ae35c8ab0d37829f`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_FIRST
- **Join Count**: 0
- **Next Tasks**: 2
- **Has Input Configuration**: Yes

### Resolved References
The following references were resolved from @@@placeholder@@@ format to actual values:

- `transition_id_ref15` → `${UUID}`
- `workflow_task_id_ref2` → `bfacc78d186f48299d37bb5a9efc5b3e`
- `workflow_task_id_ref12` → `ee4ead0fb59a4fcf8c98a36bc401ec3c`
- `transition_id_ref9` → `${UUID}`
- `workflow_task_id_ref9` → `8743f19a1dba4d84b55b20189b8c581b`
- `workflow_task_id_ref14` → `6623669284b345bb9353ffe2ef86a395`
- `transition_id_ref10` → `${UUID}`
- `workflow_task_id_ref10` → `4b296bcf261749de9415f646dbf7d79c`
- `transition_id_ref20` → `${UUID}`
- `transition_id_ref7` → `${UUID}`
- `workflow_task_id_ref7` → `dcd25fef8fa44c588bb5223249a8d7e0`
- `action_ref2` → `transforms.set_variable`
- `action_ref9` → `hubspot.update_company`
- `transition_id_ref17` → `${UUID}`
- `workflow_task_id_ref3` → `68bccda0e36e4656af2ea48b9a903c51`
- `action_ref7` → `hubspot.search_companies`
- `workflow_task_id_ref5` → `5e831d109925464d84b0d2fd1b4d941a`
- `transition_id_ref1` → `${UUID}`
- `transition_id_ref6` → `${UUID}`
- `transition_id_ref8` → `${UUID}`
- `action_ref4` → `discord.list_guild_channels`
- `action_ref5` → `discord.create_channel_message`
- `transition_id_ref2` → `${UUID}`
- `workflow_task_id_ref11` → `cce508c6b8df454a8e1b579c3577fadf`
- `transition_id_ref5` → `${UUID}`
- `transition_id_ref18` → `${UUID}`
- `workflow_task_id_ref8` → `28a5fdca6f5b4d3a90ed3924e57f25e5`
- `workflow_task_id_ref15` → `a42c856a97ba4a93a6dc2f327e54d036`
- `workflow_task_id_ref6` → `6a1786dc46a9424b87086acaf837a050`
- `workflow_note_id_ref3` → `${UUID}`
- `action_ref3` → `discord.add_role_to_user`
- `action_ref10` → `rewst.get_organization`
- `transition_id_ref12` → `${UUID}`
- `transition_id_ref4` → `${UUID}`
- `transition_id_ref13` → `${UUID}`
- `transition_id_ref16` → `${UUID}`
- `transition_id_ref11` → `${UUID}`
- `workflow_task_id_ref4` → `9f115bd9df434ce185cf381b211210b3`
- `action_ref1` → `core.noop`
- `transition_id_ref19` → `${UUID}`
- `workflow_task_id_ref13` → `a49a210c78a240248e5658bc05d53ad1`
- `workflow_task_id_ref1` → `d195a24e3dea4ac88b3d5f52a0e9a3ef`
- `workflow_note_id_ref4` → `${UUID}`
- `action_ref8` → `hubspot.search_contacts`
- `workflow_task_id_ref16` → `f035e40021154204ae35c8ab0d37829f`
- `transition_id_ref3` → `${UUID}`
- `transition_id_ref14` → `${UUID}`
- `transition_id_ref21` → `${UUID}`
- `workflow_note_id_ref1` → `${UUID}`
- `action_ref6` → `core.http_request`
- `workflow_note_id_ref2` → `${UUID}`
