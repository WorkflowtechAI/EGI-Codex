# Update HubSpot w/ Feature Requests

## Overview
**Workflow Type:** STANDARD  
**Export Date:** 2025-10-11T16:11:53.791473+00:00  
**Bundle Version:** 2  
**Original Bundle File:** `workflow-018d9e8e-bc40-74a6-9c9e-30dc12c1f88b_20251011_161153.bundle.json`  

## Workflow Documentation

### Webhook Data Processing & Validation

This functional block handles the initial processing and validation of incoming webhook data from Canny when a user votes on a feature request.

**Purpose**: Extract and validate key information from the Canny webhook payload to determine the type of submission and prepare data for downstream processing.

**Tasks Included**:
- **core_noop**: Serves as the entry point that receives the webhook payload from Canny
- **submission_type**: Analyzes the incoming data to determine if this is a "vote" submission and extracts the voter's email

**Key Data Extraction**:
- Action date, object type, voter information
- Board name, request details (ID, title, description, score)
- Request submitter information
- Training association label

**Flow Logic**: 
- Publishes extracted data to workflow context for use by subsequent tasks
- Only proceeds to contact lookup if the submission type is identified as a "vote"
- Acts as a data validation gate - ensures we only process vote-related webhooks

**Outputs**: Provides structured data including voter email, request details, and submission metadata to the contact lookup process.

### HubSpot Contact Discovery & Validation

This functional block is responsible for finding and validating the HubSpot contact associated with the Canny voter, and determining the appropriate workflow path based on contact status.

**Purpose**: Locate the HubSpot contact record for the voter and extract relevant contact/company information to determine how to proceed with the feature request processing.

**Tasks Included**:
- **hub_spot_search_contacts**: Searches HubSpot for a contact matching the voter's email address
- **contact_found**: Decision point that determines the workflow path based on whether a contact was found

**Contact Data Retrieved**:
- Contact ID and associated company information
- Company name and Customer Success Manager (CSM) assignment
- Existing Canny ID (if previously linked)

**Workflow Branching Logic**:
- **Contact Found → New Feature**: Proceeds to create HubSpot task for new feature requests
- **Contact Found → Existing Request**: Terminates workflow (request already processed)
- **No Contact Found**: Workflow terminates (voter not in HubSpot system)

**Key Decision Point**: This block acts as the primary routing mechanism - only known HubSpot contacts with new feature requests will proceed to the task creation phase.

**Outputs**: Provides contact_id, company details, CSM information, and existing Canny ID status to subsequent processing blocks.

### HubSpot Task Creation & Feature Request Tracking

This functional block handles the creation of HubSpot tasks and training objects to track feature requests and escalated tickets from Canny.

**Purpose**: Create actionable items in HubSpot to ensure feature requests are properly tracked and can be followed up on by the appropriate team members.

**Tasks Included**:
- **list_feedback_requests**: Makes a HubSpot API request to retrieve or create training objects for tracking feature requests
- **create_hubspot_task**: Creates a HubSpot task associated with the escalated ticket/feature request

**HubSpot Integration Details**:
- **API Endpoint**: `/crm/v3/objects/trainings/11359839362` - accesses training objects for feature request tracking
- **Task Creation**: Generates actionable tasks that can be assigned to CSMs or product teams
- **Meeting/Update Posting**: Creates meeting records with latest updates and escalated ticket details

**Error Handling**:
- **Task Creation Success**: Proceeds to Canny ID management
- **Task Not Created**: Workflow terminates if task creation fails

**Business Value**: 
- Ensures feature requests don't get lost in the system
- Creates trackable items for customer success and product teams
- Maintains audit trail of customer feedback and requests

**Outputs**: Provides task creation status and training object associations for the final contact update phase.

### Canny ID Management & Contact Synchronization

This functional block manages the bidirectional synchronization between Canny and HubSpot by ensuring contact records are properly linked with Canny IDs for future reference.

**Purpose**: Establish and maintain the connection between HubSpot contacts and their corresponding Canny user profiles to enable seamless future integrations and prevent duplicate processing.

**Tasks Included**:
- **check_canny_id**: Evaluates whether the contact already has a Canny ID stored in HubSpot
- **add_canny_id_to_contact**: Adds the Canny user ID to the contact if not already present
- **hub_spot_update_contact**: Updates the HubSpot contact record with the Canny ID in custom properties
- **end_success**: Marks successful completion of the workflow

**Conditional Logic Flow**:
- **Canny ID Exists**: Skips update process and proceeds directly to workflow completion
- **No Canny ID**: Updates contact record with Canny ID, then completes workflow

**Data Synchronization**:
- **Custom Property**: Stores Canny ID in HubSpot's `canny_id` custom property field
- **Bidirectional Linking**: Enables future lookups from either system
- **Duplicate Prevention**: Prevents reprocessing of requests from the same user

**Business Benefits**:
- Maintains clean data relationships between systems
- Enables efficient future processing of requests from known users
- Supports customer journey tracking across platforms
- Reduces manual data entry and potential errors

**Completion**: Workflow terminates successfully after ensuring proper contact-to-Canny linkage is established.

## Tasks

This workflow contains 10 tasks:

### 1. add_canny_id_to_contact
**Description:** Action that does nothing
**Action:** ``core.noop``
**Next Tasks:** 1 transition(s) defined

### 2. check_canny_id
**Description:** Action that does nothing
**Action:** ``core.noop``
**Transition Mode:** FOLLOW_FIRST
**Next Tasks:** 2 transition(s) defined

### 3. contact_found
**Description:** Action that does nothing
**Action:** ``core.noop``
**Transition Mode:** FOLLOW_FIRST
**Next Tasks:** 2 transition(s) defined

### 4. core_noop
**Description:** Action that does nothing
**Action:** ``core.noop``
**Timeout:** None seconds
**Next Tasks:** 1 transition(s) defined

### 5. create_hubspot_task
**Description:** Action that does nothing
**Action:** ``core.noop``
**Transition Mode:** FOLLOW_FIRST
**Next Tasks:** 2 transition(s) defined

### 6. end_success
**Description:** Action that does nothing
**Action:** ``core.noop``
**Join:** 1 (waits for 1 incoming transitions)
**Next Tasks:** 1 transition(s) defined

### 7. hub_spot_search_contacts
**Description:** search contacts in hubspot
**Action:** ``hubspot.search_contacts``
**Transition Mode:** FOLLOW_FIRST
**Next Tasks:** 2 transition(s) defined

### 8. hub_spot_update_contact
**Description:** update a contact in hubspot
**Action:** ``hubspot.update_contact``
**Next Tasks:** 1 transition(s) defined

### 9. list_feedback_requests
**Description:** Post a meeting in HubSpot with the latest updates and details of the escalated ticket.
**Action:** ``hubspot.generic_request``
**Join:** 1 (waits for 1 incoming transitions)
**Next Tasks:** 1 transition(s) defined

### 10. submission_type
**Description:** Action that does nothing
**Action:** ``core.noop``
**Next Tasks:** 1 transition(s) defined

## Technical Details

- **Workflow Timeout:** 28800 seconds
- **Total Tasks:** 10
- **Documentation Sections:** 4

---

*This README was automatically generated from the Rewst workflow export bundle.*
## Workflow Execution Details

### Overview
- **Total Tasks**: 10
- **Workflow Type**: STANDARD
- **Parallel Branches**: 4
- **Join Points**: 2
- **Resolved References**: 32

### Task Flow

#### 1. add_canny_id_to_contact
- **Action**: `core.noop`
- **Task ID**: `a4d55f377bfb4d178321f5ef0b70f822`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1

#### 2. check_canny_id
- **Action**: `core.noop`
- **Task ID**: `22bc2d9c272c426f964e1827153f10e4`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_FIRST
- **Join Count**: 0
- **Next Tasks**: 2

#### 3. contact_found
- **Action**: `core.noop`
- **Task ID**: `9628b00fd71e4f2080b05686b88ddf2b`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_FIRST
- **Join Count**: 0
- **Next Tasks**: 1

#### 4. core_noop
- **Action**: `core.noop`
- **Task ID**: `ad087d0ea26f4c339936e0e3db9fe379`
- **Timeout**: Nones
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: None
- **Next Tasks**: 1

#### 5. create_hubspot_task
- **Action**: `core.noop`
- **Task ID**: `9825f8caad64411db9e0c7e28ef4459a`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_FIRST
- **Join Count**: 0
- **Next Tasks**: 1

#### 6. end_success
- **Action**: `core.noop`
- **Task ID**: `54f3dd4aea9045e382d606dbc6fa9b57`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 1
- **Next Tasks**: 0

#### 7. hub_spot_search_contacts
- **Action**: `hubspot.search_contacts`
- **Task ID**: `066095ea5bb74fb881e7b9094787af11`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_FIRST
- **Join Count**: 0
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

#### 8. hub_spot_update_contact
- **Action**: `hubspot.update_contact`
- **Task ID**: `1cc09c64c7964e4da79fccf57ab93093`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

#### 9. list_feedback_requests
- **Action**: `hubspot.generic_request`
- **Task ID**: `c2b17d3951f8458cabf618205cb525ed`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 1
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

#### 10. submission_type
- **Action**: `core.noop`
- **Task ID**: `9483ee8f8363400380599025f1d7bd54`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1

### Resolved References
The following references were resolved from @@@placeholder@@@ format to actual values:

- `workflow_task_id_ref5` → `9825f8caad64411db9e0c7e28ef4459a`
- `workflow_note_id_ref1` → `${UUID}`
- `action_ref3` → `hubspot.update_contact`
- `transition_id_ref12` → `${UUID}`
- `transition_id_ref3` → `${UUID}`
- `workflow_task_id_ref7` → `066095ea5bb74fb881e7b9094787af11`
- `transition_id_ref8` → `${UUID}`
- `workflow_task_id_ref10` → `9483ee8f8363400380599025f1d7bd54`
- `action_ref1` → `core.noop`
- `transition_id_ref2` → `${UUID}`
- `workflow_task_id_ref4` → `ad087d0ea26f4c339936e0e3db9fe379`
- `transition_id_ref6` → `${UUID}`
- `workflow_task_id_ref1` → `a4d55f377bfb4d178321f5ef0b70f822`
- `transition_id_ref14` → `${UUID}`
- `transition_id_ref5` → `${UUID}`
- `workflow_note_id_ref2` → `${UUID}`
- `transition_id_ref1` → `${UUID}`
- `workflow_task_id_ref3` → `9628b00fd71e4f2080b05686b88ddf2b`
- `action_ref4` → `hubspot.generic_request`
- `transition_id_ref4` → `${UUID}`
- `workflow_note_id_ref3` → `${UUID}`
- `transition_id_ref11` → `${UUID}`
- `workflow_note_id_ref4` → `${UUID}`
- `transition_id_ref13` → `${UUID}`
- `transition_id_ref9` → `${UUID}`
- `workflow_task_id_ref8` → `1cc09c64c7964e4da79fccf57ab93093`
- `workflow_task_id_ref2` → `22bc2d9c272c426f964e1827153f10e4`
- `workflow_task_id_ref9` → `c2b17d3951f8458cabf618205cb525ed`
- `transition_id_ref10` → `${UUID}`
- `workflow_task_id_ref6` → `54f3dd4aea9045e382d606dbc6fa9b57`
- `action_ref2` → `hubspot.search_contacts`
- `transition_id_ref7` → `${UUID}`
