# [Community] Lookup Discord Contact from Company - Ashe

## Overview
**Workflow Type:** STANDARD  
**Export Date:** 2025-10-11T16:15:19.053035+00:00  
**Bundle Version:** 2  
**Original Bundle File:** `workflow-0195dd40-2307-77cb-b7de-54b633776e9f_20251011_161519.bundle.json`  

## Parameters

- **domain** (`string`) - Optional
- **included_properties** (`string`) - Optional

## Workflow Documentation

### Workflow Initialization & Setup

This functional block handles the initial setup and entry point for the Discord contact lookup workflow.

**Purpose**: Establishes the workflow execution context and prepares for the main data retrieval operations.

**Tasks Included**:
- **BEGIN**: Entry point task that initializes the workflow execution and triggers the contact search process

**Key Function**: This block serves as the workflow's starting point, ensuring proper initialization before proceeding to the core HubSpot contact search functionality. It establishes the execution context and validates that the workflow is ready to process the incoming domain parameter.

**Flow Connection**: Upon successful initialization, this block transitions to the "Data Retrieval & Filtering" phase where the actual HubSpot contact search is performed.

### HubSpot Contact Search & Filtering

This functional block performs the core data retrieval operation by searching HubSpot contacts and filtering for those with Discord usernames.

**Purpose**: Locate and retrieve contact records from HubSpot that have associated Discord usernames, using the provided domain as a search parameter.

**Tasks Included**:
- **hubspot_search_contacts**: Executes a targeted search in HubSpot CRM to find contacts matching the domain criteria and having Discord username properties

**Key Functionality**:
- Searches HubSpot contacts using the domain parameter from workflow context (CTX.domain)
- Applies a filter to only return contacts that have the "discord_username" property populated
- Retrieves comprehensive contact properties as specified in CTX.included_properties
- Uses HAS_PROPERTY filter operation to ensure only contacts with Discord usernames are returned

**Input Requirements**:
- Domain parameter (CTX.domain) for contact search
- Properties list (CTX.included_properties) defining which contact fields to retrieve
- Filter configured for "discord_username" property existence

**Output**: Returns a collection of HubSpot contact records that match the domain search and have Discord usernames associated.

**Flow Connection**: Upon successful contact retrieval, this block passes the results to the "Output Processing & Formatting" phase for data transformation.

### Output Processing & Formatting

This functional block processes and formats the retrieved contact data into the final output structure for the workflow.

**Purpose**: Transform the raw HubSpot contact search results into a properly formatted output that extracts and presents the relevant user properties.

**Tasks Included**:
- **format_output**: Uses the Set Variable action to extract and format user properties from the Discord users found in the search results

**Key Functionality**:
- Processes the contact search results stored in CTX.find_discord_users
- Extracts user properties from each contact record using Jinja2 templating
- Creates a formatted text output containing the relevant user information
- Transforms the data structure into a consumable format for downstream processes or final output

**Data Transformation**:
- Input: Raw HubSpot contact records with Discord usernames (CTX.find_discord_users)
- Processing: Iterates through user records and extracts properties using the template: `{{ user.properties for user in CTX.find_discord_users }}`
- Output: Formatted text variable containing structured user property data

**Flow Connection**: After successful formatting, this block transitions to the "Workflow Completion" phase to finalize the process and return results.

### Workflow Completion & Finalization

This functional block handles the final completion and cleanup of the Discord contact lookup workflow.

**Purpose**: Properly terminate the workflow execution and ensure all results are finalized and available for consumption.

**Tasks Included**:
- **END**: Final task that concludes the workflow execution and ensures proper termination

**Key Function**: This block serves as the workflow's exit point, ensuring that:
- All processing has been completed successfully
- The formatted output is properly stored and accessible
- The workflow execution is cleanly terminated
- Any necessary cleanup or final state management is performed

**Final State**: At this point, the workflow has successfully:
1. Initialized and prepared for execution
2. Searched HubSpot for contacts with Discord usernames matching the domain criteria
3. Processed and formatted the contact data into structured output
4. Completed execution with results available for retrieval

**Output Availability**: The formatted contact properties are now available in the workflow context and can be accessed by calling systems or subsequent processes.

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

### 4. hubspot_search_contacts
**Description:** search contacts in hubspot
**Action:** ``hubspot.search_contacts``
**Next Tasks:** 1 transition(s) defined

## Output

This workflow produces the following output:

- **output:** `{{ CTX.output }}`

## Technical Details

- **Workflow Timeout:** 28800 seconds
- **Total Tasks:** 4
- **Documentation Sections:** 4

---

*This README was automatically generated from the Rewst workflow export bundle.*
## Workflow Execution Details

### Overview
- **Total Tasks**: 4
- **Workflow Type**: STANDARD
- **Parallel Branches**: 0
- **Join Points**: 0
- **Resolved References**: 15

### Task Flow

#### 1. BEGIN
- **Action**: `core.noop`
- **Task ID**: `00982a2a27b648128265cc5f9409d634`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1

#### 2. END
- **Action**: `core.noop`
- **Task ID**: `2670fd5be0424bd7a199ae17aa517999`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 0

#### 3. format_output
- **Action**: `transforms.set_variable`
- **Task ID**: `36146187e8a54aaa831375318f39ccbb`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

#### 4. hubspot_search_contacts
- **Action**: `hubspot.search_contacts`
- **Task ID**: `9759ad32ce0f46daa3dd4aa5708966c1`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

### Resolved References
The following references were resolved from @@@placeholder@@@ format to actual values:

- `transition_id_ref3` → `${UUID}`
- `transition_id_ref2` → `${UUID}`
- `workflow_note_id_ref1` → `${UUID}`
- `action_ref1` → `core.noop`
- `action_ref3` → `hubspot.search_contacts`
- `workflow_task_id_ref3` → `36146187e8a54aaa831375318f39ccbb`
- `workflow_note_id_ref2` → `${UUID}`
- `workflow_task_id_ref2` → `2670fd5be0424bd7a199ae17aa517999`
- `workflow_note_id_ref3` → `${UUID}`
- `workflow_task_id_ref4` → `9759ad32ce0f46daa3dd4aa5708966c1`
- `transition_id_ref1` → `${UUID}`
- `workflow_task_id_ref1` → `00982a2a27b648128265cc5f9409d634`
- `transition_id_ref4` → `${UUID}`
- `action_ref2` → `transforms.set_variable`
- `workflow_note_id_ref4` → `${UUID}`
