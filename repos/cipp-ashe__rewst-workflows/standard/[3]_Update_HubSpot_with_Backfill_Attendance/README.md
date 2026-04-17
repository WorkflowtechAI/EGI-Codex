# [3] Update HubSpot with Backfill Attendance

## Overview
**Workflow Type:** STANDARD  
**Export Date:** 2025-10-11T16:14:40.888404+00:00  
**Bundle Version:** 2  
**Original Bundle File:** `workflow-018f2ab5-4f46-72d5-b91a-19422b096593_20251011_161440.bundle.json`  

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

This functional block handles the initial processing and validation of incoming attendance data for Open Mic events.

**Purpose**: Prepare and validate input data before attempting HubSpot operations.

**Key Tasks**:
- **BEGIN**: Entry point that validates email presence and extracts company domain from email address
- Calculates meeting timestamps (start/end times) based on join/leave times
- Extracts latest join date for attendance tracking

**Data Transformations**:
- Extracts company domain from email address using split operation
- Converts joined_at timestamp to proper HubSpot meeting start time format
- Calculates meeting end time (1 hour after start)
- Formats latest join date for contact property updates

**Flow Logic**: Only proceeds if email is present and valid, ensuring data quality before HubSpot operations.

**Outputs**: Provides company_domain, hs_meeting_start, hs_meeting_end, and latest_join_date for downstream processing.

### Contact Discovery & Management

This functional block manages the discovery and handling of HubSpot contact records based on email addresses.

**Purpose**: Locate existing contacts or handle cases where no contact exists in HubSpot.

**Key Tasks**:
- **hub_spot_search_contacts**: Searches HubSpot for contacts using email address
- **contact_found**: Placeholder task executed when contact is found
- **no_contact**: Placeholder task executed when no contact exists

**Search Logic**:
- Searches by email address with specific properties: id, associatedcompanyid, company, success_manager
- Branches based on search results (contact found vs. no contact found)

**Data Extraction** (when contact found):
- Captures contact_id for future updates
- Extracts company_id from associated company
- Retrieves company_name and csm_id (Customer Success Manager)

**Flow Branches**:
- **Contact Found**: Proceeds to contact update process
- **No Contact Found**: Triggers company search to potentially create new contact

**Connection Points**: Feeds into either contact update workflow or company discovery workflow based on search results.

### Company Discovery & New Contact Creation

This functional block handles company discovery and new contact creation when no existing contact is found in HubSpot.

**Purpose**: Search for company records and create new contacts when attendees don't exist in HubSpot.

**Key Tasks**:
- **hub_spot_search_companies**: Searches for companies using domain extracted from email
- **company_found**: Placeholder task executed when company is located
- **hub_spot_create_contact**: Creates new contact record with company association

**Company Search Process**:
- Searches by domain with EQ filter for exact match
- Retrieves properties: id, name, customer_success_manager, customer_journey_stage, onboarding_phase, account_status
- Extracts company_id, company_name, and csm_id when found

**Contact Creation Process**:
- Creates contact with email, company name, and domain information
- Sets lifecycle stage to "customer"
- Adds custom property "latest_open_mic_attendance" with join date
- Associates contact with found company

**Flow Logic**:
- **Company Found**: Creates new contact with company association
- **Company Not Found**: Triggers error handling workflow

**Data Flow**: Provides contact_id for newly created contact to enable meeting record creation.

### HubSpot Record Updates & Meeting Creation

This functional block handles the core HubSpot record updates and meeting activity creation for successful contact/company matches.

**Purpose**: Update existing contacts and create meeting records to track Open Mic attendance.

**Key Tasks**:
- **hub_spot_update_contact**: Updates existing contact with company website domain
- **contact_updated**: Prepares meeting data and metadata for activity creation
- **workflows_sub_hub_spot_task_idempotency**: Creates meeting/task record with idempotency
- **meeting_created**: Final confirmation task for successful meeting creation

**Contact Update Process**:
- Updates contact record with company domain website information
- Maintains data consistency between contact and company records

**Meeting Data Preparation**:
- Formats meeting title: "Attended Open Mic (YYYY-MM-DD)"
- Creates meeting body with join/leave times in Eastern timezone
- Sets meeting outcome to "ATTENDED"
- Assigns activity type as "Open Mic"
- Associates with Customer Success Manager (hubspot_owner_id)

**Idempotency Handling**:
- Uses sub-workflow to prevent duplicate meeting records
- Ensures data integrity across multiple workflow executions
- Handles meeting start/end times and activity metadata

**Final State**: Completes workflow execution with confirmed meeting record creation in HubSpot.

### Error Handling & Notification System

This functional block manages error scenarios and notifications when HubSpot records cannot be found or created.

**Purpose**: Handle cases where company records don't exist and notify administrators of data gaps.

**Key Tasks**:
- **no_company**: Placeholder task triggered when company search fails
- **slack_chat_post_message**: Sends notification to Slack channel about missing company record
- **end_no_account**: Final termination task for error scenarios

**Error Scenario Handling**:
- Triggered when company domain search returns no results
- Indicates attendee's company is not in HubSpot system
- Prevents workflow from failing while capturing the data gap

**Notification Process**:
- Sends structured message to Slack channel (C04S32BSECD)
- Includes attendee details: name and email address
- Alerts team to manual review requirement
- Uses markdown formatting for clear message presentation

**Message Content**:
- "A User had attended the Open Mic but there's no HubSpot record for their associated company"
- Provides name and email for manual investigation
- Enables team to decide on company creation or contact association

**Workflow Termination**: Ends workflow execution gracefully without creating incomplete records in HubSpot.

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

### 13. slack_chat_post_message
**Description:** Sends a message to a channel.
**Action:** ``slack.chat.postMessage``
**Next Tasks:** 1 transition(s) defined

### 14. workflows_sub_hub_spot_task_idempotency
**Action:** `Unknown`
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
- **Join Points**: 1
- **Resolved References**: 42

### Task Flow

#### 1. BEGIN
- **Action**: `core.noop`
- **Task ID**: `018f2ab54f447723bcefa5fde45fc570`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1

#### 2. company_found
- **Action**: `core.noop`
- **Task ID**: `018f2ab54f447723bcefa5f741fb48ef`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1

#### 3. contact_found
- **Action**: `core.noop`
- **Task ID**: `018f2ab54f447723bcefa5f94bbba52d`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1

#### 4. contact_updated
- **Action**: `core.noop`
- **Task ID**: `018f2ab54f447723bcefa5fca8c6c083`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 1
- **Next Tasks**: 1

#### 5. end_no_account
- **Action**: `core.noop`
- **Task ID**: `018f2ab54f447723bcefa60045e7ddaf`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 0

#### 6. hub_spot_create_contact
- **Action**: `hubspot.create_contact`
- **Task ID**: `018f2ab54f447723bcefa603cfbd709e`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

#### 7. hub_spot_search_companies
- **Action**: `hubspot.search_companies`
- **Task ID**: `018f2ab54f447723bcefa60288b19931`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_FIRST
- **Join Count**: 0
- **Next Tasks**: 2
- **Has Input Configuration**: Yes

#### 8. hub_spot_search_contacts
- **Action**: `hubspot.search_contacts`
- **Task ID**: `018f2ab54f447723bcefa5f873b05fdd`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_FIRST
- **Join Count**: 0
- **Next Tasks**: 2
- **Has Input Configuration**: Yes

#### 9. hub_spot_update_contact
- **Action**: `hubspot.update_contact`
- **Task ID**: `018f2ab54f447723bcefa5fa8872424b`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

#### 10. meeting_created
- **Action**: `core.noop`
- **Task ID**: `018f2ab54f447723bcefa5f6e6a7854d`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 0

#### 11. no_company
- **Action**: `core.noop`
- **Task ID**: `018f2ab54f447723bcefa5ff6ef5be98`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1

#### 12. no_contact
- **Action**: `core.noop`
- **Task ID**: `018f2ab54f447723bcefa5fb8cbfd4b8`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1

#### 13. slack_chat_post_message
- **Action**: `slack.chat.postMessage`
- **Task ID**: `018f2ab54f447723bcefa5fe045475d9`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

#### 14. workflows_sub_hub_spot_task_idempotency
- **Action**: `Unknown Action`
- **Task ID**: `018f2ab54f447723bcefa60134a3afd2`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

### Resolved References
The following references were resolved from @@@placeholder@@@ format to actual values:

- `workflow_task_id_ref13` → `018f2ab54f447723bcefa5fe045475d9`
- `transition_id_ref9` → `${UUID}`
- `workflow_task_id_ref5` → `018f2ab54f447723bcefa60045e7ddaf`
- `workflow_task_id_ref1` → `018f2ab54f447723bcefa5fde45fc570`
- `workflow_ref1` → `[SUBWORKFLOW: [SUB] HubSpot Task Idempotency - Ashe]`
- `transition_id_ref14` → `${UUID}`
- `workflow_note_id_ref4` → `${UUID}`
- `workflow_note_id_ref5` → `${UUID}`
- `workflow_task_id_ref11` → `018f2ab54f447723bcefa5ff6ef5be98`
- `transition_id_ref2` → `${UUID}`
- `transition_id_ref11` → `${UUID}`
- `transition_id_ref13` → `${UUID}`
- `workflow_task_id_ref8` → `018f2ab54f447723bcefa5f873b05fdd`
- `workflow_note_id_ref3` → `${UUID}`
- `transition_id_ref10` → `${UUID}`
- `workflow_task_id_ref4` → `018f2ab54f447723bcefa5fca8c6c083`
- `workflow_task_id_ref14` → `018f2ab54f447723bcefa60134a3afd2`
- `action_ref5` → `hubspot.update_contact`
- `workflow_task_id_ref10` → `018f2ab54f447723bcefa5f6e6a7854d`
- `workflow_task_id_ref9` → `018f2ab54f447723bcefa5fa8872424b`
- `transition_id_ref3` → `${UUID}`
- `transition_id_ref5` → `${UUID}`
- `workflow_note_id_ref2` → `${UUID}`
- `workflow_task_id_ref12` → `018f2ab54f447723bcefa5fb8cbfd4b8`
- `action_ref3` → `hubspot.search_companies`
- `action_ref1` → `core.noop`
- `transition_id_ref15` → `${UUID}`
- `transition_id_ref6` → `${UUID}`
- `transition_id_ref16` → `${UUID}`
- `action_ref2` → `hubspot.create_contact`
- `workflow_task_id_ref2` → `018f2ab54f447723bcefa5f741fb48ef`
- `transition_id_ref7` → `${UUID}`
- `action_ref4` → `hubspot.search_contacts`
- `workflow_task_id_ref7` → `018f2ab54f447723bcefa60288b19931`
- `transition_id_ref4` → `${UUID}`
- `workflow_task_id_ref6` → `018f2ab54f447723bcefa603cfbd709e`
- `transition_id_ref8` → `${UUID}`
- `transition_id_ref1` → `${UUID}`
- `workflow_note_id_ref1` → `${UUID}`
- `transition_id_ref12` → `${UUID}`
- `action_ref6` → `slack.chat.postMessage`
- `workflow_task_id_ref3` → `018f2ab54f447723bcefa5f94bbba52d`
