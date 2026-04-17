# Prod - Update HubSpot with Escalated Ticket Details

## Overview
**Workflow Type:** STANDARD  
**Export Date:** 2025-10-11T16:12:22.212631+00:00  
**Bundle Version:** 2  
**Original Bundle File:** `workflow-de74111a-7f0c-4f40-8ac6-12c780017049_20251011_161222.bundle.json`  

## Parameters

- **risk_ticket** (`string`) - Optional

## Workflow Documentation

### Workflow Initialization & Ticket Data Collection

## Purpose
This functional block handles the initial workflow trigger and retrieves comprehensive ticket information from HaloPSA to establish the foundation for the escalation process.

## Tasks Included
- **form_submission**: Workflow entry point that captures the ticket ID through a form submission
- **halo_psa_get_ticket**: Retrieves detailed ticket information from HaloPSA including ticket details, status, and metadata

## Key Functionality
- Initiates the workflow with a ticket ID input
- Fetches complete ticket details from HaloPSA including:
  - Ticket summary, details, and status
  - User information and assignment details
  - Dates (created, assigned, last action)
  - Custom fields and success criteria
  - Last action information

## Data Flow
The form submission provides the ticket ID (CTX.risk_ticket) which is then used to retrieve comprehensive ticket details that feed into subsequent processing blocks.

## Outputs to Next Block
- Complete ticket details (CTX.risk_details)
- Ticket ID for further API calls
- Foundation data for parallel processing of agent, status, and actions

### HaloPSA Data Enrichment & Parallel Processing

## Purpose
This block performs parallel data enrichment by gathering additional context from HaloPSA including agent details, ticket actions history, and current status information to build a complete picture of the escalated ticket.

## Tasks Included
- **halo_psa_get_agent**: Retrieves information about the agent assigned to the ticket
- **halo_psa_list_actions**: Fetches all actions/updates associated with the ticket
- **halo_psa_get_status**: Gets the current status details of the ticket

## Key Functionality
- **Agent Information**: Collects assigned agent details including name and contact information
- **Action History**: Retrieves chronological list of all ticket actions, updates, and status changes
- **Status Details**: Fetches current ticket status information and metadata

## Parallel Execution
These three tasks execute simultaneously after ticket details are retrieved, optimizing workflow performance by gathering related data concurrently.

## Data Outputs
- Agent details (CTX.get_agent) - name, email, team information
- Complete action history (CTX.halo_actions) - timestamps, outcomes, notes, status changes
- Current status information (CTX.current_status) - status name, type, and properties

## Connection to Next Block
This enriched data feeds into the data formatting and transformation block where it's prepared for HubSpot integration.

### Data Transformation & HubSpot Preparation

## Purpose
This functional block transforms and formats the collected HaloPSA data into structures suitable for HubSpot integration, including formatting ticket actions and preparing meeting content templates.

## Tasks Included
- **format_actions**: Formats ticket actions into HTML for display in HubSpot meetings
- **select_halo_attributes**: Extracts and publishes key ticket attributes as workflow variables
- **hub_spot_search_contacts**: Searches HubSpot for the contact associated with the ticket
- **select_hubspot_attributes**: Prepares HubSpot-specific data structures and meeting templates

## Key Transformations

### Action Formatting
- Converts raw HaloPSA actions into formatted HTML
- Sorts actions chronologically (most recent first)
- Structures action data with timestamps, outcomes, status changes, and notes

### Attribute Mapping
- Maps HaloPSA ticket fields to workflow variables
- Converts dates to appropriate formats (display and epoch timestamps)
- Calculates ticket age and formats URLs

### HubSpot Integration Prep
- Searches for existing HubSpot contacts by email
- Extracts HubSpot contact and company IDs
- Prepares meeting titles and body content for both original and escalated ticket meetings

## Data Flow
1. Raw HaloPSA data → Formatted action history
2. Ticket details → Structured workflow variables
3. Contact email → HubSpot contact lookup
4. Combined data → Meeting templates ready for HubSpot API calls

## Outputs to Final Block
- Formatted ticket actions (CTX.ticket_actions_formatted)
- HubSpot contact and company IDs
- Meeting titles and body content for HubSpot meetings

### HubSpot Meeting Creation & Final Output

## Purpose
This final functional block creates meeting records in HubSpot to document the escalated ticket details and maintain a comprehensive audit trail of the escalation process.

## Tasks Included
- **post_latest_update_as_meeting**: Creates a HubSpot meeting with current escalation details and ticket status
- **post_ticket_as_meeting**: Creates a HubSpot meeting with original ticket submission details (conditional execution)

## Meeting Creation Strategy

### Primary Meeting (Always Executed)
- **Title**: "Escalated: [Ticket Summary] - [ROC Reference]"
- **Content**: Current ticket status, team assignment, ticket age, and formatted action history
- **Timestamp**: Uses last action date to maintain chronological accuracy
- **Associations**: Links to both the contact and company in HubSpot

### Original Ticket Meeting (Conditional)
- **Title**: "[Ticket Summary] - [ROC Reference]"
- **Content**: Original ticket details, success criteria, and initial request information
- **Timestamp**: Uses ticket creation date
- **Execution**: Currently disabled (condition: 1 == 3) but available for activation

## HubSpot Integration Details
- Creates meetings with proper associations to contacts and companies
- Uses epoch timestamps for accurate scheduling
- Includes comprehensive ticket information in meeting bodies
- Maintains HubSpot owner assignments for proper visibility

## Workflow Completion
Upon successful meeting creation, the workflow completes, having successfully:
1. Retrieved escalated ticket details from HaloPSA
2. Enriched data with agent, status, and action information
3. Formatted data for HubSpot consumption
4. Created comprehensive meeting records in HubSpot for audit and tracking purposes

## Business Value
This block ensures that escalated tickets are properly documented in HubSpot, providing visibility to account managers and maintaining a complete customer interaction history.

## Tasks

This workflow contains 11 tasks:

### 1. form_submission
**Description:** Workflow kicks off by retrieving the Ticket ID wanting to be looked up in Halo through a form
**Action:** ``core.noop``
**Next Tasks:** 1 transition(s) defined

### 2. format_actions
**Description:** format actions
**Action:** ``core.noop``
**Next Tasks:** 1 transition(s) defined

### 3. halo_psa_get_agent
**Description:** returns the ROC member assigned to the ticket
**Action:** ``halo_psa.get_agent``
**Next Tasks:** 1 transition(s) defined

### 4. halo_psa_get_status
**Description:** Return the latest status of the requested ticket from Halo.
**Action:** ``halo_psa.get_status``
**Next Tasks:** 1 transition(s) defined

### 5. halo_psa_get_ticket
**Description:** Collect the details of the Halo ticket that was requested through the form
**Action:** ``halo_psa.get_ticket``
**Next Tasks:** 1 transition(s) defined

### 6. halo_psa_list_actions
**Description:** Returns an object containing the actions associated with the ticket # provided.
**Action:** ``halo_psa.list_actions``
**Next Tasks:** 1 transition(s) defined

### 7. hub_spot_search_contacts
**Description:** Look up the contact associated with the ticket in HubSpot and pass info on to be scrutenized.
**Action:** ``hubspot.search_contacts``
**Next Tasks:** 1 transition(s) defined

### 8. post_latest_update_as_meeting
**Description:** Post a meeting in HubSpot with the latest updates and details of the escalated ticket.
**Action:** ``hubspot.generic_request``
**Next Tasks:** 1 transition(s) defined

### 9. post_ticket_as_meeting
**Description:** Post the initial ticket details as a meeting in HubSpot from when it was created.
**Action:** ``hubspot.generic_request``
**Next Tasks:** 1 transition(s) defined

### 10. select_halo_attributes
**Description:** Capture all the relevant Halo ticket fields for updating HubSpot and publishing them as Aliases for the template later.
**Action:** ``core.noop``
**Next Tasks:** 1 transition(s) defined

### 11. select_hubspot_attributes
**Description:** Model the data collected up to this point for feeling into the body and titles of the HubSpot activities to be created.
**Action:** ``core.noop``
**Transition Mode:** FOLLOW_FIRST
**Next Tasks:** 2 transition(s) defined

## Technical Details

- **Workflow Timeout:** 28800 seconds
- **Total Tasks:** 11
- **Documentation Sections:** 4

---

*This README was automatically generated from the Rewst workflow export bundle.*
## Workflow Execution Details

### Overview
- **Total Tasks**: 11
- **Workflow Type**: STANDARD
- **Parallel Branches**: 1
- **Join Points**: 0
- **Resolved References**: 34

### Task Flow

#### 1. form_submission
- **Action**: `core.noop`
- **Task ID**: `0b6a63ee777242d79e3b6a7a648d11b5`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1

#### 2. format_actions
- **Action**: `core.noop`
- **Task ID**: `00a774fef6bd43d2be5f625c234db3ef`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1

#### 3. halo_psa_get_agent
- **Action**: `halo_psa.get_agent`
- **Task ID**: `ceee5b171a4b4b2696cd9253a8e4302f`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

#### 4. halo_psa_get_status
- **Action**: `halo_psa.get_status`
- **Task ID**: `8c9181387d364925bdd7d8d23e027529`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

#### 5. halo_psa_get_ticket
- **Action**: `halo_psa.get_ticket`
- **Task ID**: `8185dbc2c13e4638b7e5239eab816c8f`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 3
- **Has Input Configuration**: Yes

#### 6. halo_psa_list_actions
- **Action**: `halo_psa.list_actions`
- **Task ID**: `604915b73411421dbb60da1bc17941f1`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

#### 7. hub_spot_search_contacts
- **Action**: `hubspot.search_contacts`
- **Task ID**: `ec0a6f4f659b47b1b1411921c9e5caae`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

#### 8. post_latest_update_as_meeting
- **Action**: `hubspot.generic_request`
- **Task ID**: `3853b2faaaf145cab05d3fd236e53cf9`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 0
- **Has Input Configuration**: Yes

#### 9. post_ticket_as_meeting
- **Action**: `hubspot.generic_request`
- **Task ID**: `73c230265cf743008b11c2b9fc0f2041`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 0
- **Has Input Configuration**: Yes

#### 10. select_halo_attributes
- **Action**: `core.noop`
- **Task ID**: `219888ef70704c8fa915215d42e9af86`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1

#### 11. select_hubspot_attributes
- **Action**: `core.noop`
- **Task ID**: `41f526f9af14435bb85dc199714e77e9`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_FIRST
- **Join Count**: 0
- **Next Tasks**: 2

### Resolved References
The following references were resolved from @@@placeholder@@@ format to actual values:

- `action_ref6` → `hubspot.search_contacts`
- `workflow_note_id_ref2` → `${UUID}`
- `workflow_task_id_ref5` → `8185dbc2c13e4638b7e5239eab816c8f`
- `transition_id_ref3` → `${UUID}`
- `transition_id_ref9` → `${UUID}`
- `transition_id_ref11` → `${UUID}`
- `workflow_task_id_ref10` → `219888ef70704c8fa915215d42e9af86`
- `workflow_task_id_ref6` → `604915b73411421dbb60da1bc17941f1`
- `workflow_task_id_ref8` → `3853b2faaaf145cab05d3fd236e53cf9`
- `transition_id_ref6` → `${UUID}`
- `transition_id_ref8` → `${UUID}`
- `transition_id_ref12` → `${UUID}`
- `action_ref2` → `halo_psa.get_agent`
- `workflow_task_id_ref9` → `73c230265cf743008b11c2b9fc0f2041`
- `workflow_note_id_ref4` → `${UUID}`
- `workflow_task_id_ref1` → `0b6a63ee777242d79e3b6a7a648d11b5`
- `transition_id_ref10` → `${UUID}`
- `workflow_task_id_ref3` → `ceee5b171a4b4b2696cd9253a8e4302f`
- `workflow_task_id_ref11` → `41f526f9af14435bb85dc199714e77e9`
- `workflow_task_id_ref7` → `ec0a6f4f659b47b1b1411921c9e5caae`
- `workflow_task_id_ref2` → `00a774fef6bd43d2be5f625c234db3ef`
- `transition_id_ref1` → `${UUID}`
- `action_ref4` → `halo_psa.get_ticket`
- `workflow_task_id_ref4` → `8c9181387d364925bdd7d8d23e027529`
- `transition_id_ref5` → `${UUID}`
- `action_ref7` → `hubspot.generic_request`
- `action_ref5` → `halo_psa.list_actions`
- `transition_id_ref2` → `${UUID}`
- `action_ref1` → `core.noop`
- `transition_id_ref7` → `${UUID}`
- `workflow_note_id_ref1` → `${UUID}`
- `transition_id_ref4` → `${UUID}`
- `workflow_note_id_ref3` → `${UUID}`
- `action_ref3` → `halo_psa.get_status`
