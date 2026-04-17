# [2] Update HubSpot with Attendance (Group Training)

## Overview
**Workflow Type:** STANDARD  
**Export Date:** 2025-10-11T16:13:44.530624+00:00  
**Bundle Version:** 2  
**Original Bundle File:** `workflow-8986dbb3-f475-4c50-a0c6-a55fc96f7fb7_20251011_161344.bundle.json`  

## Parameters

- **email** (`string`) - Optional
- **last_name** (`string`) - Optional
- **class_name** (`string`) - Optional
- **first_name** (`string`) - Optional
- **meeting_end_time** (`string`) - Optional
- **attendance_length** (`string`) - Optional
- **meeting_start_time** (`string`) - Optional

## Workflow Documentation

### Initial Setup & Data Processing

This functional block handles the initial workflow setup and data transformation required for processing training attendance records.

**Purpose**: Validates input data, extracts company domain from email, formats meeting timestamps, and maps training course names to HubSpot IDs.

**Key Tasks**:
- **BEGIN**: Entry point that validates email presence and transforms input data
  - Extracts company domain from attendee email
  - Converts meeting timestamps to HubSpot-compatible format
  - Maps training course names to corresponding HubSpot training IDs
  - Sets up meeting duration and attendance metadata

**Data Transformations**:
- Company domain extraction: `email.split("@")|last`
- Meeting time formatting: Rounds up to nearest hour for HubSpot compatibility
- Training course mapping: Uses predefined course ID lookup table

**Flow**: This block must complete successfully before any HubSpot operations can begin. It publishes essential context variables used throughout the workflow.

### Contact & Company Discovery

This functional block handles the discovery and validation of existing contacts and companies in HubSpot based on the training attendee's information.

**Purpose**: Searches HubSpot for existing contact and company records to determine the appropriate path for creating training attendance records.

**Key Tasks**:
- **hub_spot_search_contacts**: Searches for existing contact by email address
- **contact_found**: Routing task when contact exists - extracts contact ID, company ID, and CSM info
- **no_contact**: Routing task when no contact found - triggers company search by domain
- **hub_spot_search_companies**: Searches for company by domain when no contact exists
- **company_found**: Routing task when company exists - extracts company details and CSM
- **no_company**: Routing task when neither contact nor company found

**Decision Logic**:
1. First attempts to find existing contact by email
2. If contact found → proceeds to contact management
3. If no contact → searches for company by domain
4. If company found → creates new contact under existing company
5. If no company → triggers error notification path

**Data Extracted**:
- Contact ID, Company ID, Company Name, Customer Success Manager ID
- Used to determine ownership and association for training records

**Flow Outcomes**: Routes to either contact management (existing contact), contact creation (existing company), or error handling (no records found).

### Contact Management & Creation

This functional block manages contact records in HubSpot, either creating new contacts or updating existing ones with training attendance information.

**Purpose**: Ensures a valid contact record exists in HubSpot with updated training attendance data before creating meeting records.

**Key Tasks**:
- **hub_spot_create_contact**: Creates new contact when company exists but contact doesn't
  - Sets lifecycle stage to "customer"
  - Associates with existing company
  - Updates latest group training attendance date
- **hub_spot_update_contact**: Updates existing contact with training attendance info
  - Updates website domain and latest training attendance date
- **contact_updated**: Routing task that prepares meeting/task creation data
  - Formats meeting details for HubSpot task creation
  - Sets meeting outcome as "ATTENDED"
  - Calculates attendance duration in minutes

**Contact Properties Updated**:
- `latest_group_training_attendance`: Date of most recent training attendance
- `website`: Company domain for better data consistency
- `lifecyclestage`: Set to "customer" for new contacts

**Meeting Data Preparation**:
- Meeting title: "Attended [Course Name] ([Date])"
- Meeting body: Attendance duration in minutes
- Meeting outcome: "ATTENDED"
- Activity type: "Group Training"

**Flow**: Both creation and update paths converge to the same meeting creation process with properly formatted training data.

### Training Record Creation & Association

This functional block creates the actual training attendance records in HubSpot and associates them with the contact and training course.

**Purpose**: Creates a HubSpot task/meeting record documenting the training attendance and establishes proper associations between contact, company, and training course.

**Key Tasks**:
- **workflows_sub_hub_spot_task_idempotency**: Sub-workflow that creates HubSpot task/meeting record
  - Uses idempotency to prevent duplicate records
  - Creates meeting with training details, duration, and outcome
  - Associates meeting with contact and company
- **meeting_created**: Success routing when task creation succeeds
- **meeting_creation_failed**: Error routing when task creation fails
- **update_contact_associations**: Creates association between contact and training course
  - Uses HubSpot API to create custom association (type 26)
  - Links contact to specific training course record
- **end_training_associated**: Final success state

**HubSpot Records Created**:
- **Task/Meeting Record**: Documents the training attendance with:
  - Title: "Attended [Course] ([Date])"
  - Body: Attendance duration in minutes
  - Outcome: "ATTENDED"
  - Activity Type: "Group Training"
- **Association**: Links contact to training course (association type 26)

**Error Handling**: If task creation fails, routes to error notification. If association fails, routes to association failure handling.

**Flow**: This is the core business logic that creates the permanent record of training attendance in HubSpot's CRM system.

### Error Handling & Notifications

This functional block handles error scenarios and provides notifications when the workflow cannot complete successfully due to missing data or system failures.

**Purpose**: Ensures proper error reporting and team notification when training attendance cannot be recorded due to missing HubSpot records or system failures.

**Key Tasks**:
- **slack_chat_post_message**: Sends notification to Slack channel when no company record exists
  - Alerts team that user attended training but has no HubSpot company record
  - Includes attendee name, email, and training course information
  - Posts to specific Slack channel (C04S32BSECD)
- **end_no_account**: Terminal state for missing company scenario
- **end_association_failed**: Terminal state for association creation failures

**Error Scenarios Handled**:
1. **No Company Found**: When attendee's company domain doesn't match any HubSpot company
   - Sends detailed Slack notification with attendee info
   - Allows manual follow-up by customer success team
2. **Association Creation Failed**: When contact-training association cannot be created
   - Logs the failure for troubleshooting
   - Prevents workflow from hanging in error state

**Notification Content**:
- Training course name and date
- Attendee name and email address
- Clear indication that manual intervention is needed

**Flow**: These are terminal error states that ensure the workflow completes gracefully even when business requirements cannot be fully met due to data gaps.

### Should make the trainings get called from a template that can be kept updated instead.

## Tasks

This workflow contains 18 tasks:

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

### 5. end_association_failed
**Description:** Action that does nothing
**Action:** ``core.noop``
**Next Tasks:** 1 transition(s) defined

### 6. end_no_account
**Description:** Action that does nothing
**Action:** ``core.noop``
**Next Tasks:** 1 transition(s) defined

### 7. end_training_associated
**Description:** Action that does nothing
**Action:** ``core.noop``
**Next Tasks:** 1 transition(s) defined

### 8. hub_spot_create_contact
**Description:** create a contact in hubspot
**Action:** ``hubspot.create_contact``
**Next Tasks:** 1 transition(s) defined

### 9. hub_spot_search_companies
**Description:** search companies in hubspot
**Action:** ``hubspot.search_companies``
**Transition Mode:** FOLLOW_FIRST
**Next Tasks:** 2 transition(s) defined

### 10. hub_spot_search_contacts
**Description:** search contacts in hubspot
**Action:** ``hubspot.search_contacts``
**Transition Mode:** FOLLOW_FIRST
**Next Tasks:** 2 transition(s) defined

### 11. hub_spot_update_contact
**Description:** update a contact in hubspot
**Action:** ``hubspot.update_contact``
**Next Tasks:** 1 transition(s) defined

### 12. meeting_created
**Description:** Action that does nothing
**Action:** ``core.noop``
**Next Tasks:** 1 transition(s) defined

### 13. meeting_creation_failed
**Description:** Action that does nothing
**Action:** ``core.noop``
**Next Tasks:** 1 transition(s) defined

### 14. no_company
**Description:** Action that does nothing
**Action:** ``core.noop``
**Next Tasks:** 1 transition(s) defined

### 15. no_contact
**Description:** Action that does nothing
**Action:** ``core.noop``
**Next Tasks:** 1 transition(s) defined

### 16. slack_chat_post_message
**Description:** Sends a message to a channel.
**Action:** ``slack.chat.postMessage``
**Next Tasks:** 1 transition(s) defined

### 17. update_contact_associations
**Description:** Post a meeting in HubSpot with the latest updates and details of the escalated ticket.
**Action:** ``hubspot.generic_request``
**Join:** 1 (waits for 1 incoming transitions)
**Next Tasks:** 2 transition(s) defined

### 18. workflows_sub_hub_spot_task_idempotency
**Action:** `Unknown`
**Transition Mode:** FOLLOW_FIRST
**Next Tasks:** 2 transition(s) defined

## Technical Details

- **Workflow Timeout:** 28800 seconds
- **Total Tasks:** 18
- **Documentation Sections:** 6

---

*This README was automatically generated from the Rewst workflow export bundle.*
## Workflow Execution Details

### Overview
- **Total Tasks**: 18
- **Workflow Type**: STANDARD
- **Parallel Branches**: 4
- **Join Points**: 2
- **Resolved References**: 54

### Task Flow

#### 1. BEGIN
- **Action**: `core.noop`
- **Task ID**: `de8983ba54074bcd9353745bb28b9376`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1

#### 2. company_found
- **Action**: `core.noop`
- **Task ID**: `37811bbb21494a338e8586ad8a6fef20`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1

#### 3. contact_found
- **Action**: `core.noop`
- **Task ID**: `be07d68677b04217ad5ccc7027919a80`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1

#### 4. contact_updated
- **Action**: `core.noop`
- **Task ID**: `244513cca20f439982b10d7c460230ff`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 1
- **Next Tasks**: 1

#### 5. end_association_failed
- **Action**: `core.noop`
- **Task ID**: `0d9e85426e474608b7d32c3b2fcf8c58`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 0

#### 6. end_no_account
- **Action**: `core.noop`
- **Task ID**: `100dc4d63fde4088980f4c605a3f2b67`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 0

#### 7. end_training_associated
- **Action**: `core.noop`
- **Task ID**: `824e7f8579294288935b5dab00d4cd66`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 0

#### 8. hub_spot_create_contact
- **Action**: `hubspot.create_contact`
- **Task ID**: `b74ed6af75734127b253ff2699251c36`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

#### 9. hub_spot_search_companies
- **Action**: `hubspot.search_companies`
- **Task ID**: `2435aad1d6d24738988e09d5b1dd0456`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_FIRST
- **Join Count**: 0
- **Next Tasks**: 2
- **Has Input Configuration**: Yes

#### 10. hub_spot_search_contacts
- **Action**: `hubspot.search_contacts`
- **Task ID**: `affaf35061554b86bdc2c6e2ddf80b2c`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_FIRST
- **Join Count**: 0
- **Next Tasks**: 2
- **Has Input Configuration**: Yes

#### 11. hub_spot_update_contact
- **Action**: `hubspot.update_contact`
- **Task ID**: `4d0f0bdd65f344dd9ac52e52d0ae91d6`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

#### 12. meeting_created
- **Action**: `core.noop`
- **Task ID**: `81a2cd6877c34c52a60804d985ba90b1`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1

#### 13. meeting_creation_failed
- **Action**: `core.noop`
- **Task ID**: `2cf28a48e96244f4915ca40556535374`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1

#### 14. no_company
- **Action**: `core.noop`
- **Task ID**: `be2e9ed20823489f9b1696ed2f34cfa2`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1

#### 15. no_contact
- **Action**: `core.noop`
- **Task ID**: `3d411be4bc4548a8a880a36a9ecdbc67`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1

#### 16. slack_chat_post_message
- **Action**: `slack.chat.postMessage`
- **Task ID**: `1c40a5c33b614cc1abe169afdee9bb3d`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

#### 17. update_contact_associations
- **Action**: `hubspot.generic_request`
- **Task ID**: `51ea09e6f2544076978de8786cb467fa`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 1
- **Next Tasks**: 2
- **Has Input Configuration**: Yes

#### 18. workflows_sub_hub_spot_task_idempotency
- **Action**: `Unknown Action`
- **Task ID**: `f1fed8f787074895bd9f1d9946908528`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_FIRST
- **Join Count**: 0
- **Next Tasks**: 2
- **Has Input Configuration**: Yes

### Resolved References
The following references were resolved from @@@placeholder@@@ format to actual values:

- `workflow_task_id_ref18` → `f1fed8f787074895bd9f1d9946908528`
- `transition_id_ref19` → `${UUID}`
- `workflow_task_id_ref3` → `be07d68677b04217ad5ccc7027919a80`
- `workflow_task_id_ref17` → `51ea09e6f2544076978de8786cb467fa`
- `transition_id_ref9` → `${UUID}`
- `transition_id_ref4` → `${UUID}`
- `transition_id_ref12` → `${UUID}`
- `workflow_task_id_ref15` → `3d411be4bc4548a8a880a36a9ecdbc67`
- `transition_id_ref20` → `${UUID}`
- `transition_id_ref13` → `${UUID}`
- `transition_id_ref14` → `${UUID}`
- `workflow_task_id_ref10` → `affaf35061554b86bdc2c6e2ddf80b2c`
- `action_ref3` → `hubspot.search_companies`
- `transition_id_ref2` → `${UUID}`
- `transition_id_ref22` → `${UUID}`
- `workflow_task_id_ref7` → `824e7f8579294288935b5dab00d4cd66`
- `transition_id_ref8` → `${UUID}`
- `transition_id_ref15` → `${UUID}`
- `action_ref5` → `hubspot.update_contact`
- `workflow_note_id_ref3` → `${UUID}`
- `workflow_task_id_ref16` → `1c40a5c33b614cc1abe169afdee9bb3d`
- `workflow_note_id_ref1` → `${UUID}`
- `workflow_task_id_ref9` → `2435aad1d6d24738988e09d5b1dd0456`
- `transition_id_ref16` → `${UUID}`
- `workflow_note_id_ref6` → `${UUID}`
- `workflow_task_id_ref14` → `be2e9ed20823489f9b1696ed2f34cfa2`
- `workflow_task_id_ref1` → `de8983ba54074bcd9353745bb28b9376`
- `workflow_task_id_ref11` → `4d0f0bdd65f344dd9ac52e52d0ae91d6`
- `transition_id_ref3` → `${UUID}`
- `transition_id_ref21` → `${UUID}`
- `transition_id_ref17` → `${UUID}`
- `transition_id_ref1` → `${UUID}`
- `action_ref1` → `core.noop`
- `transition_id_ref7` → `${UUID}`
- `workflow_task_id_ref4` → `244513cca20f439982b10d7c460230ff`
- `transition_id_ref5` → `${UUID}`
- `workflow_note_id_ref2` → `${UUID}`
- `workflow_note_id_ref4` → `${UUID}`
- `transition_id_ref11` → `${UUID}`
- `workflow_task_id_ref2` → `37811bbb21494a338e8586ad8a6fef20`
- `workflow_task_id_ref5` → `0d9e85426e474608b7d32c3b2fcf8c58`
- `workflow_task_id_ref8` → `b74ed6af75734127b253ff2699251c36`
- `action_ref6` → `slack.chat.postMessage`
- `workflow_task_id_ref6` → `100dc4d63fde4088980f4c605a3f2b67`
- `action_ref2` → `hubspot.create_contact`
- `workflow_task_id_ref12` → `81a2cd6877c34c52a60804d985ba90b1`
- `transition_id_ref18` → `${UUID}`
- `workflow_task_id_ref13` → `2cf28a48e96244f4915ca40556535374`
- `workflow_ref1` → `[SUBWORKFLOW: [SUB] HubSpot Task Idempotency - Ashe]`
- `action_ref4` → `hubspot.search_contacts`
- `workflow_note_id_ref5` → `${UUID}`
- `transition_id_ref6` → `${UUID}`
- `action_ref7` → `hubspot.generic_request`
- `transition_id_ref10` → `${UUID}`
