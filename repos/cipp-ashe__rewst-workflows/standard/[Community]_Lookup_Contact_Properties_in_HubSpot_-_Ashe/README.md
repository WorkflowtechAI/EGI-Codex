# [Community] Lookup Contact Properties in HubSpot - Ashe

## Overview
**Workflow Type:** STANDARD  
**Export Date:** 2025-10-11T16:15:15.555380+00:00  
**Bundle Version:** 2  
**Original Bundle File:** `workflow-0195f2dc-53d6-78a3-9674-76bd5bf09317_20251011_161515.bundle.json`  

## Parameters

- **contact_id** (`string`) - Optional
- **included_properties** (`string`) - Optional

## Workflow Documentation

### Workflow Initialization & Setup

This block handles the initial setup and preparation phase of the workflow.

**Purpose**: Establishes the workflow execution context and prepares for HubSpot contact data retrieval.

**Tasks Included**:
- **BEGIN**: Entry point that initializes the workflow execution and triggers the contact lookup process

**Key Functionality**:
- Serves as the workflow entry point
- Validates that required input parameters (contact_id, included_properties) are available in the workflow context
- Transitions control to the HubSpot data retrieval phase

**Inputs Expected**:
- `contact_id`: The HubSpot contact identifier to lookup
- `included_properties`: Comma-separated list of contact properties to retrieve

**Outputs Provided**:
- Workflow context initialization
- Triggers the next phase (HubSpot API interaction)

**Connection to Next Block**: Upon successful initialization, this block passes control to the "HubSpot Data Retrieval" block to fetch the actual contact information.

### HubSpot Data Retrieval

This block handles the core data retrieval functionality by interfacing with the HubSpot CRM API.

**Purpose**: Fetches contact information from HubSpot using the provided contact ID and property specifications.

**Tasks Included**:
- **hubspot_get_contact**: Executes HubSpot API call to retrieve contact data with specified properties

**Key Functionality**:
- Makes authenticated API call to HubSpot's `/crm/v3/objects/contacts/{contact_id}` endpoint
- Retrieves contact properties as specified in the `included_properties` parameter
- Handles HubSpot API response and error conditions
- Supports extensive property list including standard and custom contact fields

**API Integration Details**:
- **Endpoint**: HubSpot CRM v3 Contacts API
- **Method**: GET request to fetch contact by ID
- **Authentication**: Uses configured HubSpot integration credentials
- **Properties**: Supports 200+ standard HubSpot contact properties plus custom fields

**Inputs Required**:
- `contact_id`: HubSpot contact identifier (from workflow context)
- `included_properties`: Comma-separated string of property names to retrieve

**Outputs Provided**:
- `properties`: Complete contact data object containing all requested properties
- Contact metadata and system fields
- Raw API response data for downstream processing

**Error Handling**:
- API authentication failures
- Invalid contact ID scenarios
- Network connectivity issues
- Property access permission errors

**Connection to Next Block**: Upon successful data retrieval, passes the complete contact properties object to the "Data Processing & Output" block for formatting and final output preparation.

### Data Processing & Output

This block handles the final processing and output formatting of the retrieved HubSpot contact data.

**Purpose**: Processes the raw HubSpot API response and formats it for workflow output, then completes the workflow execution.

**Tasks Included**:
- **format_output**: Transforms and formats the contact properties data for final output
- **END**: Completes the workflow execution and finalizes the output

**Key Functionality**:
- **Data Transformation**: Extracts the `properties` object from the HubSpot API response
- **Output Formatting**: Uses the Set Variable action to format the contact properties as the workflow's final output
- **Workflow Completion**: Properly terminates the workflow execution with formatted results

**Data Processing Details**:
- **Input Source**: `CTX.find_contacts.properties` (contact data from HubSpot API)
- **Transformation**: Direct pass-through of properties object (no modification)
- **Output Format**: Structured contact properties object ready for consumption

**Outputs Provided**:
- **Final Workflow Output**: Complete contact properties object containing all requested HubSpot contact fields
- **Structured Data**: Properties formatted as key-value pairs for easy access
- **Workflow Status**: Successful completion indicator

**Use Cases for Output**:
- Contact information display in applications
- Data synchronization with other systems
- Contact property analysis and reporting
- Integration with downstream workflows or processes

**Connection from Previous Block**: Receives the complete contact data object from the "HubSpot Data Retrieval" block and processes it for final output.

**Workflow Completion**: This block represents the final stage of the workflow, ensuring clean termination and proper output formatting for consuming applications or users.

## Tasks

This workflow contains 4 tasks:

### 1. BEGIN
**Description:** Action that does nothing
**Action:** ``core.noop``
**Next Tasks:** 1 transition(s) defined

### 2. END
**Description:** Action that does nothing
**Action:** ``core.noop``
**Next Tasks:** 1 transition(s) defined

### 3. format_output
**Description:** Set a variable as an action
**Action:** ``transforms.set_variable``
**Next Tasks:** 1 transition(s) defined

### 4. hubspot_get_contact
**Description:** get a hubspot contact by id
**Action:** ``hubspot.get_contact``
**Next Tasks:** 1 transition(s) defined

## Output

This workflow produces the following output:

- **output:** `{{ CTX.output }}`

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
- **Parallel Branches**: 0
- **Join Points**: 0
- **Resolved References**: 14

### Task Flow

#### 1. BEGIN
- **Action**: `core.noop`
- **Task ID**: `0195f2dc53d678a3967476bbeaf064c5`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1

#### 2. END
- **Action**: `core.noop`
- **Task ID**: `6fb5350d183d4af6913308c9e12af783`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 0

#### 3. format_output
- **Action**: `transforms.set_variable`
- **Task ID**: `0195f2dc53d678a3967476baf5a55fd0`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

#### 4. hubspot_get_contact
- **Action**: `hubspot.get_contact`
- **Task ID**: `c314d6affe4340f1896bd273ab30d0bc`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

### Resolved References
The following references were resolved from @@@placeholder@@@ format to actual values:

- `workflow_task_id_ref2` → `6fb5350d183d4af6913308c9e12af783`
- `action_ref2` → `transforms.set_variable`
- `workflow_task_id_ref4` → `c314d6affe4340f1896bd273ab30d0bc`
- `transition_id_ref1` → `${UUID}`
- `workflow_note_id_ref2` → `${UUID}`
- `workflow_task_id_ref3` → `0195f2dc53d678a3967476baf5a55fd0`
- `transition_id_ref3` → `${UUID}`
- `action_ref1` → `core.noop`
- `action_ref3` → `hubspot.get_contact`
- `transition_id_ref4` → `${UUID}`
- `workflow_task_id_ref1` → `0195f2dc53d678a3967476bbeaf064c5`
- `workflow_note_id_ref1` → `${UUID}`
- `workflow_note_id_ref3` → `${UUID}`
- `transition_id_ref2` → `${UUID}`
