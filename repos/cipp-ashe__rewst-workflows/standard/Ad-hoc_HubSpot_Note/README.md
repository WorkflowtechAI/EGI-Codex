# Ad-hoc HubSpot Note

## Overview
**Workflow Type:** STANDARD  
**Export Date:** 2025-10-11T16:12:32.239178+00:00  
**Bundle Version:** 2  
**Original Bundle File:** `workflow-018f2f39-dc63-7f9b-a916-d1f9d5064dc2_20251011_161232.bundle.json`  

## Parameters

- **email** (`string`) - Optional
- **date_submitted** (`string`) - Optional

## Workflow Documentation

### Initial Setup & Variable Configuration

This functional block handles the initial setup and variable configuration for the Training Programniversity enrollment workflow.

**Purpose**: Prepares all necessary variables, timestamps, and formatted content needed for the subsequent HubSpot operations.

**Key Tasks**:
- **set_vars**: Initializes critical workflow variables including calendar data, HTML content, timestamps, and formatted body text for the enrollment notification

**Key Variables Created**:
- `calendar_raw` & `calendar`: Calendar data for the enrollment
- `html` & `html_self_serve`: HTML content for email communications
- `hs_timestamp`: Properly formatted timestamp for HubSpot records
- `format_body`: Formatted message body indicating Training Programniversity registration

**Workflow Context**: This block runs at the beginning of the workflow and establishes all the foundational data that will be used throughout the process. The variables created here are essential for proper contact updates and note creation in HubSpot.

**Next Phase**: After setup completion, the workflow proceeds to search for the contact in HubSpot using the configured email address.

### Contact Search & Validation

This functional block handles the critical contact lookup process in HubSpot to determine if the enrolling user exists in the system.

**Purpose**: Searches for the contact using their email address and retrieves essential contact information needed for enrollment processing.

**Key Tasks**:
- **hub_spot_search_contacts**: Performs a contact search in HubSpot using the provided email address and retrieves specific properties

**Search Configuration**:
- **Query**: Uses the `CTX.email` variable to search for the contact
- **Properties Retrieved**: `id`, `associatedcompanyid`, `company`, `success_manager`
- **Search Method**: Uses HubSpot's contact search API endpoint

**Conditional Logic**:
- **Contact Found**: If search returns results (length > 0), workflow proceeds to data transformation
- **No Contact Found**: If no contact exists, workflow terminates gracefully

**Data Retrieved**:
- Contact ID for future operations
- Associated company information
- Success manager assignment
- Company name for context

**Workflow Context**: This is a critical decision point in the workflow. The success of this search determines whether the enrollment can be processed. The retrieved contact data becomes the foundation for all subsequent HubSpot operations.

**Next Phase**: If a contact is found, the workflow moves to data processing and transformation. If no contact exists, the workflow ends without further action.

### Data Processing & Transformation

This functional block transforms the raw contact search results into a structured format optimized for HubSpot operations.

**Purpose**: Processes the contact search results and creates a structured data object that can be efficiently used for batch operations on contacts and their associated records.

**Key Tasks**:
- **transforms_set_variable**: Transforms contact search results into a structured array of contact objects

**Data Transformation Logic**:
The task creates a structured array where each contact object contains:
- `contact_id`: The unique HubSpot contact identifier
- `company_id`: Associated company ID from the contact's properties
- `company_name`: Company name for reference
- `csm_id`: Customer Success Manager ID for proper assignment

**Input Source**: `CTX.hs_search_contacts` (results from the contact search)

**Output Structure**: Creates an iterable array of contact objects that enables the workflow to process multiple contacts if needed (though typically one contact per enrollment)

**Processing Pattern**: Uses Jinja2 templating to iterate through search results and extract relevant properties into a clean, consistent data structure

**Workflow Context**: This transformation step is crucial for preparing data in the exact format needed by subsequent HubSpot operations. It ensures that all necessary IDs and references are properly structured and accessible for note creation and contact updates.

**Next Phase**: The transformed data is used to create HubSpot notes with proper associations to contacts and companies.

### HubSpot Note Creation & Contact Updates

This functional block handles the core HubSpot operations, creating enrollment notes and updating contact records with Training Programniversity enrollment information.

**Purpose**: Creates a comprehensive record of the Training Programniversity enrollment in HubSpot by adding detailed notes and updating contact properties.

**Key Tasks**:

1. **post_ticket_summary_as_task**: Creates a new note in HubSpot with enrollment details
   - Uses HubSpot's generic API request action to create notes
   - Includes formatted enrollment information and email content
   - Associates the note with both the contact and their company
   - Assigns the note to the appropriate Customer Success Manager

2. **task_added**: Status tracking placeholder for note creation completion

3. **hub_spot_update_contact**: Updates the contact record with enrollment date
   - Sets the custom property `cluck_u_enrollment_date` with the current date
   - Uses the formatted timestamp from the initial setup phase

**Note Creation Details**:
- **Content**: Includes registration date, time, and the self-serve email content
- **Associations**: Links to both contact (association type 202) and company (association type 190)
- **Owner Assignment**: Assigns to the contact's designated Customer Success Manager
- **Timestamp**: Uses properly formatted HubSpot timestamp

**Contact Update Details**:
- **Property Updated**: `cluck_u_enrollment_date`
- **Value**: Current date in YYYY-MM-DD format
- **Purpose**: Tracks when the contact enrolled in Training Programniversity

**Workflow Context**: This block represents the primary business value of the workflow - creating a permanent record of the enrollment in HubSpot and ensuring the contact's profile reflects their participation in Training Programniversity.

**Next Phase**: After successful HubSpot operations, the workflow moves to completion tracking.

### Workflow Completion & Status Tracking

This functional block handles the final workflow completion and status tracking for the Training Programniversity enrollment process.

**Purpose**: Provides a clean completion point for the workflow and ensures proper status tracking after all HubSpot operations have been successfully completed.

**Key Tasks**:
- **end_contact_updated**: Final status marker indicating successful completion of contact updates and note creation

**Workflow Function**:
- Acts as a terminal point for successful workflow execution
- Provides a clear indication that all enrollment processing has been completed
- Uses a no-operation (noop) action to serve as a status checkpoint

**Execution Context**: This task only executes when:
- Contact search was successful
- Data transformation completed properly  
- HubSpot note creation succeeded
- Contact record update was successful

**Workflow Completion**: When this task executes, it indicates that:
- The contact has been successfully enrolled in Training Programniversity
- A detailed note has been created in HubSpot with enrollment information
- The contact's record has been updated with the enrollment date
- All associations (contact, company, CSM) have been properly established

**Status Tracking**: This completion marker helps with workflow monitoring and debugging, providing a clear success indicator for the entire enrollment process.

**Final State**: Upon completion of this task, the Training Programniversity enrollment workflow has successfully processed the enrollment and updated all relevant HubSpot records.

## Tasks

This workflow contains 7 tasks:

### 1. end_contact_updated
**Description:** Action that does nothing
**Action:** ``core.noop``
**Next Tasks:** 1 transition(s) defined

### 2. hub_spot_search_contacts
**Description:** search contacts in hubspot
**Action:** ``hubspot.search_contacts``
**Transition Mode:** FOLLOW_FIRST
**Join:** 1 (waits for 1 incoming transitions)
**Next Tasks:** 2 transition(s) defined

### 3. hub_spot_update_contact
**Description:** update a contact in hubspot
**Action:** ``hubspot.update_contact``
**Loop/Iteration:** Yes
**Next Tasks:** 1 transition(s) defined

### 4. post_ticket_summary_as_task
**Description:** Post a meeting in HubSpot with the latest updates and details of the escalated ticket.
**Action:** ``hubspot.generic_request``
**Join:** 1 (waits for 1 incoming transitions)
**Loop/Iteration:** Yes
**Next Tasks:** 1 transition(s) defined

### 5. set_vars
**Description:** Action that does nothing
**Action:** ``core.noop``
**Next Tasks:** 1 transition(s) defined

### 6. task_added
**Description:** Action that does nothing
**Action:** ``core.noop``
**Next Tasks:** 1 transition(s) defined

### 7. transforms_set_variable
**Description:** Set a variable as an action
**Action:** ``transforms.set_variable``
**Next Tasks:** 1 transition(s) defined

## Technical Details

- **Workflow Timeout:** 28800 seconds
- **Total Tasks:** 7
- **Documentation Sections:** 5

---

*This README was automatically generated from the Rewst workflow export bundle.*
## Workflow Execution Details

### Overview
- **Total Tasks**: 7
- **Workflow Type**: STANDARD
- **Parallel Branches**: 1
- **Join Points**: 2
- **Resolved References**: 28

### Task Flow

#### 1. end_contact_updated
- **Action**: `core.noop`
- **Task ID**: `9bf51b46543343bc9661df9c3faacbfd`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 0

#### 2. hub_spot_search_contacts
- **Action**: `hubspot.search_contacts`
- **Task ID**: `f3eb60f3e2d54b6f896103396df80f51`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_FIRST
- **Join Count**: 1
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

#### 3. hub_spot_update_contact
- **Action**: `hubspot.update_contact`
- **Task ID**: `181158fd033540c39f136d381644d954`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1
- **Has Input Configuration**: Yes
- **Loop/Iterator**: Yes

#### 4. post_ticket_summary_as_task
- **Action**: `hubspot.generic_request`
- **Task ID**: `30b5501ec0fe4c46873e2ffcac65c81c`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 1
- **Next Tasks**: 1
- **Has Input Configuration**: Yes
- **Loop/Iterator**: Yes

#### 5. set_vars
- **Action**: `core.noop`
- **Task ID**: `df90f93e92a24e3d9030c9268a4030d6`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1

#### 6. task_added
- **Action**: `core.noop`
- **Task ID**: `52bbe1801a9040f68e07f48df1302290`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1

#### 7. transforms_set_variable
- **Action**: `transforms.set_variable`
- **Task ID**: `23d945457b5244479b055090d6408199`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

### Resolved References
The following references were resolved from @@@placeholder@@@ format to actual values:

- `transition_id_ref2` → `${UUID}`
- `transition_id_ref4` → `${UUID}`
- `workflow_task_id_ref1` → `9bf51b46543343bc9661df9c3faacbfd`
- `action_ref5` → `transforms.set_variable`
- `action_ref2` → `hubspot.search_contacts`
- `transition_id_ref6` → `${UUID}`
- `workflow_note_id_ref1` → `${UUID}`
- `transition_id_ref7` → `${UUID}`
- `transition_id_ref3` → `${UUID}`
- `workflow_task_id_ref4` → `30b5501ec0fe4c46873e2ffcac65c81c`
- `action_ref3` → `hubspot.update_contact`
- `template_ref1` → `[TEMPLATE: Training Program Freshman Schedule - First 30 Days]`
- `workflow_task_id_ref5` → `df90f93e92a24e3d9030c9268a4030d6`
- `workflow_task_id_ref6` → `52bbe1801a9040f68e07f48df1302290`
- `action_ref4` → `hubspot.generic_request`
- `workflow_note_id_ref5` → `${UUID}`
- `transition_id_ref8` → `${UUID}`
- `template_ref2` → `[TEMPLATE: Training Program Freshman Training Calendar HTML]`
- `workflow_task_id_ref3` → `181158fd033540c39f136d381644d954`
- `transition_id_ref5` → `${UUID}`
- `workflow_task_id_ref2` → `f3eb60f3e2d54b6f896103396df80f51`
- `workflow_task_id_ref7` → `23d945457b5244479b055090d6408199`
- `transition_id_ref1` → `${UUID}`
- `template_ref3` → `[TEMPLATE: Training Program Freshman Training Calendar HTML - Self Serve]`
- `workflow_note_id_ref3` → `${UUID}`
- `workflow_note_id_ref4` → `${UUID}`
- `action_ref1` → `core.noop`
- `workflow_note_id_ref2` → `${UUID}`
