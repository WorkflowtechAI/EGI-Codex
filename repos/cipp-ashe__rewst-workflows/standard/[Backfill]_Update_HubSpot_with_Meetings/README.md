# [Backfill] Update HubSpot with Meetings

## Overview
**Workflow Type:** STANDARD  
**Export Date:** 2025-10-11T16:11:50.398075+00:00  
**Bundle Version:** 2  
**Original Bundle File:** `workflow-2113a594-0fd4-4ce2-aec5-e9e42391fdf6_20251011_161150.bundle.json`  

## Parameters

- **email** (`string`) - Optional
- **left_at** (`string`) - Optional
- **joined_at** (`string`) - Optional
- **last_name** (`string`) - Optional
- **first_name** (`string`) - Optional
- **meeting_end_time** (`string`) - Optional
- **meeting_start_time** (`string`) - Optional

## Workflow Documentation

### Initial Data Processing & Validation

This functional block handles the initial processing and validation of incoming meeting attendance data from Open Mic sessions.

**Purpose**: Validates input data and prepares key variables needed throughout the workflow.

**Key Tasks**:
- **BEGIN**: Entry point that validates email presence and extracts company domain from email
- Calculates meeting timestamps (start/end times) based on join time
- Prepares formatted data for HubSpot integration

**Data Transformations**:
- Extracts company domain from email address
- Converts join time to HubSpot-compatible meeting start/end times
- Sets up context variables for downstream processing

**Flow Control**: Only proceeds if email is present and valid, otherwise workflow terminates.

**Outputs**: Provides company_domain, meeting timestamps, and formatted data for subsequent HubSpot operations.

### Contact & Company Discovery Process

This functional block searches for existing contacts and companies in HubSpot to determine the appropriate processing path.

**Purpose**: Locates existing HubSpot records to avoid duplicates and gather necessary IDs for meeting association.

**Search Strategy**:
1. **Contact Search First**: Searches for contact by email address
2. **Company Search**: If no contact found, searches for company by domain
3. **Data Extraction**: Retrieves contact/company IDs and associated metadata

**Key Tasks**:
- **hub_spot_search_contacts**: Searches HubSpot contacts by email, retrieves contact ID, company ID, and CSM information
- **hub_spot_search_companies**: Searches HubSpot companies by domain when no contact exists
- **contact_found**: Processing marker for successful contact discovery
- **company_found**: Processing marker for successful company discovery

**Decision Points**:
- If contact found → Proceeds to contact update path
- If no contact but company found → Creates new contact under existing company
- If neither found → Triggers error notification path

**Critical Data Retrieved**:
- Contact ID, Company ID, Company Name, Customer Success Manager ID
- Used for proper association of meeting records and ownership assignment

### Contact Record Management

This functional block handles the creation or updating of contact records in HubSpot based on the discovery results.

**Purpose**: Ensures proper contact records exist in HubSpot with updated information and correct associations.

**Processing Paths**:

**New Contact Creation Path**:
- **hub_spot_create_contact**: Creates new contact when company exists but contact doesn't
- Sets lifecycle stage to "customer"
- Associates with existing company
- Includes name, email, and company domain information

**Existing Contact Update Path**:
- **hub_spot_update_contact**: Updates existing contact with company domain information
- Maintains existing contact data while adding missing website/domain info

**Data Preparation**:
- **contact_updated**: Prepares meeting-specific data including:
  - Meeting timestamps and duration
  - Meeting title with date
  - HubSpot owner assignment (CSM)
  - Meeting body with join/leave times
  - Meeting outcome and activity type

**Key Features**:
- Preserves existing contact information during updates
- Ensures proper company associations
- Prepares standardized meeting metadata for consistent reporting
- Handles timezone conversions for meeting times

### Meeting Record Creation & Association

This functional block creates the actual meeting record in HubSpot and properly associates it with both contact and company records.

**Purpose**: Creates a permanent record of Open Mic attendance in HubSpot with proper associations and metadata.

**Meeting Record Creation**:
- **post_ticket_summary_as_task**: Creates HubSpot meeting record using generic API request
- Includes comprehensive meeting details:
  - Meeting timestamp and duration
  - Meeting title with attendance date
  - Detailed meeting body with join/leave times (timezone converted)
  - Meeting outcome ("ATTENDED")
  - Activity type ("Open Mic")

**Critical Associations**:
- **Contact Association**: Links meeting to specific contact (association type 200)
- **Company Association**: Links meeting to company record (association type 188)
- Ensures meeting appears in both contact and company timelines

**Data Structure**:
- Uses HubSpot's v3 meetings API endpoint
- Follows HubSpot's association schema for proper relationship mapping
- Maintains data consistency across contact and company records

**Completion Marker**:
- **meeting_created**: Indicates successful meeting record creation
- Provides engagement_id for potential future reference
- Marks successful completion of the primary workflow objective

### Error Handling & Notification System

This functional block handles error scenarios where HubSpot records cannot be found or created, providing notifications for manual intervention.

**Purpose**: Manages workflow paths when automated processing cannot complete due to missing HubSpot records.

**Error Scenarios Handled**:

**No Contact Found**:
- **no_contact**: Triggered when contact search returns no results
- Leads to company search as fallback option

**No Company Found**:
- **no_company**: Triggered when both contact and company searches fail
- Indicates attendee's company is not in HubSpot system

**Notification Process**:
- **slack_chat_post_message**: Sends alert to designated Slack channel (C04S32BSECD)
- Includes attendee details: name and email
- Alerts team that manual HubSpot record creation may be needed

**Message Content**:
- Clear indication that Open Mic attendee has no HubSpot company record
- Provides attendee's name and email for manual lookup/creation
- Enables team to follow up and create missing records

**Workflow Termination**:
- **end_no_account**: Clean termination point for unprocessable records
- Ensures workflow doesn't continue with incomplete data
- Maintains data integrity by preventing partial record creation

**Business Impact**: Ensures no attendee data is lost and provides visibility into gaps in the HubSpot database that require manual attention.

## Tasks

This workflow contains 14 tasks:

### 1. BEGIN
**Description:** Action that does nothing
**Action:** ``core.noop``
**Next Tasks:** 1 transition(s) defined

### 2. company_found
**Description:** Action that does nothing
**Action:** ``core.noop``
**Next Tasks:** 1 transition(s) defined

### 3. contact_found
**Description:** Action that does nothing
**Action:** ``core.noop``
**Next Tasks:** 1 transition(s) defined

### 4. contact_updated
**Description:** Action that does nothing
**Action:** ``core.noop``
**Join:** 1 (waits for 1 incoming transitions)
**Next Tasks:** 1 transition(s) defined

### 5. end_no_account
**Description:** Action that does nothing
**Action:** ``core.noop``
**Next Tasks:** 1 transition(s) defined

### 6. hub_spot_create_contact
**Description:** create a contact in hubspot
**Action:** ``hubspot.create_contact``
**Next Tasks:** 1 transition(s) defined

### 7. hub_spot_search_companies
**Description:** search companies in hubspot
**Action:** ``hubspot.search_companies``
**Transition Mode:** FOLLOW_FIRST
**Next Tasks:** 2 transition(s) defined

### 8. hub_spot_search_contacts
**Description:** search contacts in hubspot
**Action:** ``hubspot.search_contacts``
**Transition Mode:** FOLLOW_FIRST
**Next Tasks:** 2 transition(s) defined

### 9. hub_spot_update_contact
**Description:** update a contact in hubspot
**Action:** ``hubspot.update_contact``
**Next Tasks:** 1 transition(s) defined

### 10. meeting_created
**Description:** Action that does nothing
**Action:** ``core.noop``
**Next Tasks:** 1 transition(s) defined

### 11. no_company
**Description:** Action that does nothing
**Action:** ``core.noop``
**Next Tasks:** 1 transition(s) defined

### 12. no_contact
**Description:** Action that does nothing
**Action:** ``core.noop``
**Next Tasks:** 1 transition(s) defined

### 13. post_ticket_summary_as_task
**Description:** Post a meeting in HubSpot with the latest updates and details of the escalated ticket.
**Action:** ``hubspot.generic_request``
**Join:** 1 (waits for 1 incoming transitions)
**Next Tasks:** 1 transition(s) defined

### 14. slack_chat_post_message
**Description:** Sends a message to a channel.
**Action:** ``slack.chat.postMessage``
**Next Tasks:** 1 transition(s) defined

## Technical Details

- **Workflow Timeout:** 28800 seconds
- **Total Tasks:** 14
- **Documentation Sections:** 5

---

*This README was automatically generated from the Rewst workflow export bundle.*
## Workflow Execution Details

### Overview
- **Total Tasks**: 14
- **Workflow Type**: STANDARD
- **Parallel Branches**: 2
- **Join Points**: 2
- **Resolved References**: 42

### Task Flow

#### 1. BEGIN
- **Action**: `core.noop`
- **Task ID**: `59652af0be8e4a5291ddc54ca5858278`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1

#### 2. company_found
- **Action**: `core.noop`
- **Task ID**: `6b5ab885f1234af88c6701dea9f08ee4`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1

#### 3. contact_found
- **Action**: `core.noop`
- **Task ID**: `a4f49a36b9744943961d84a6972e1365`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1

#### 4. contact_updated
- **Action**: `core.noop`
- **Task ID**: `0ced5400fb7041bc975b4417def010b4`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 1
- **Next Tasks**: 1

#### 5. end_no_account
- **Action**: `core.noop`
- **Task ID**: `a0f454dfecd84e48b9d3ab4e6a8290ee`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 0

#### 6. hub_spot_create_contact
- **Action**: `hubspot.create_contact`
- **Task ID**: `0ba73e993d3a413a8f8aa6dfce033b10`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

#### 7. hub_spot_search_companies
- **Action**: `hubspot.search_companies`
- **Task ID**: `6798a852f98a4fe0a11d43a5e8b08fbc`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_FIRST
- **Join Count**: 0
- **Next Tasks**: 2
- **Has Input Configuration**: Yes

#### 8. hub_spot_search_contacts
- **Action**: `hubspot.search_contacts`
- **Task ID**: `e3df5c89863f4a119f918bf4c58e2efc`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_FIRST
- **Join Count**: 0
- **Next Tasks**: 2
- **Has Input Configuration**: Yes

#### 9. hub_spot_update_contact
- **Action**: `hubspot.update_contact`
- **Task ID**: `2e20a17a5efa49a7a38411288c266270`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

#### 10. meeting_created
- **Action**: `core.noop`
- **Task ID**: `e87e62a0e0ab4b3c9562ffd74b421cd7`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 0

#### 11. no_company
- **Action**: `core.noop`
- **Task ID**: `c60ba2d316fc4d3fb6b367cb9da166f8`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1

#### 12. no_contact
- **Action**: `core.noop`
- **Task ID**: `901879de9cfa4b2badc48161c75f3c87`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1

#### 13. post_ticket_summary_as_task
- **Action**: `hubspot.generic_request`
- **Task ID**: `2087513d0e3f47e5a63a0a7d636ca775`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 1
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

#### 14. slack_chat_post_message
- **Action**: `slack.chat.postMessage`
- **Task ID**: `06457c62bed1455397aa347a66944777`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

### Resolved References
The following references were resolved from @@@placeholder@@@ format to actual values:

- `workflow_note_id_ref5` → `${UUID}`
- `transition_id_ref12` → `${UUID}`
- `workflow_task_id_ref5` → `a0f454dfecd84e48b9d3ab4e6a8290ee`
- `transition_id_ref13` → `${UUID}`
- `workflow_task_id_ref9` → `2e20a17a5efa49a7a38411288c266270`
- `transition_id_ref3` → `${UUID}`
- `workflow_task_id_ref4` → `0ced5400fb7041bc975b4417def010b4`
- `workflow_task_id_ref1` → `59652af0be8e4a5291ddc54ca5858278`
- `workflow_task_id_ref12` → `901879de9cfa4b2badc48161c75f3c87`
- `workflow_note_id_ref1` → `${UUID}`
- `workflow_note_id_ref2` → `${UUID}`
- `workflow_task_id_ref10` → `e87e62a0e0ab4b3c9562ffd74b421cd7`
- `workflow_task_id_ref2` → `6b5ab885f1234af88c6701dea9f08ee4`
- `transition_id_ref7` → `${UUID}`
- `transition_id_ref9` → `${UUID}`
- `transition_id_ref11` → `${UUID}`
- `action_ref2` → `hubspot.create_contact`
- `transition_id_ref2` → `${UUID}`
- `workflow_note_id_ref3` → `${UUID}`
- `workflow_task_id_ref13` → `2087513d0e3f47e5a63a0a7d636ca775`
- `workflow_task_id_ref11` → `c60ba2d316fc4d3fb6b367cb9da166f8`
- `transition_id_ref8` → `${UUID}`
- `workflow_task_id_ref3` → `a4f49a36b9744943961d84a6972e1365`
- `action_ref4` → `hubspot.search_contacts`
- `transition_id_ref15` → `${UUID}`
- `transition_id_ref5` → `${UUID}`
- `transition_id_ref6` → `${UUID}`
- `workflow_task_id_ref7` → `6798a852f98a4fe0a11d43a5e8b08fbc`
- `action_ref3` → `hubspot.search_companies`
- `action_ref5` → `hubspot.update_contact`
- `workflow_task_id_ref14` → `06457c62bed1455397aa347a66944777`
- `action_ref7` → `slack.chat.postMessage`
- `action_ref6` → `hubspot.generic_request`
- `workflow_task_id_ref8` → `e3df5c89863f4a119f918bf4c58e2efc`
- `transition_id_ref16` → `${UUID}`
- `transition_id_ref10` → `${UUID}`
- `transition_id_ref14` → `${UUID}`
- `workflow_note_id_ref4` → `${UUID}`
- `transition_id_ref1` → `${UUID}`
- `transition_id_ref4` → `${UUID}`
- `workflow_task_id_ref6` → `0ba73e993d3a413a8f8aa6dfce033b10`
- `action_ref1` → `core.noop`
