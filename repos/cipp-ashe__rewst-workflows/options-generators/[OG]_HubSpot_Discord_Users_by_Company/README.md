# [OG] HubSpot Discord Users by Company

## Overview
**Workflow Type:** OPTION_GENERATOR  
**Export Date:** 2025-10-11T16:10:43.619778+00:00  
**Bundle Version:** 2  
**Original Bundle File:** `workflow-018fda3d-6586-725b-81c5-12b3658e886e_20251011_161043.bundle.json`  

## Parameters

- **data** (`string`) - Optional

## Workflow Documentation

### Input Processing & Data Preparation

This functional block handles the initial workflow setup and input data processing to prepare for HubSpot searches.

**Purpose**: Parse and structure the incoming data to extract domain and server information needed for subsequent HubSpot queries.

**Tasks Included**:
- **BEGIN**: Workflow initialization and entry point
- **split_data_values**: Parses the input data to extract domain and server_name components using Jinja2 templating

**Key Operations**:
- Splits comma-separated input data into structured components
- Extracts domain (first part) and server_name (last part) from the input
- Creates a structured data object for downstream processing

**Data Flow**:
- Input: Raw comma-separated data string
- Output: Structured object with `domain` and `server_name` properties
- Next Phase: Company discovery using the extracted domain information

**Dependencies**: This block requires properly formatted input data and serves as the foundation for all subsequent HubSpot operations.

### Company Discovery & Validation

This functional block performs company lookup and validation in HubSpot using the extracted domain information.

**Purpose**: Locate and validate the target company in HubSpot's CRM system to establish the company context for contact searches.

**Tasks Included**:
- **hub_spot_search_companies**: Searches HubSpot companies using the parsed domain as the query parameter

**Key Operations**:
- Executes HubSpot company search API call using the extracted domain
- Retrieves comprehensive company properties and metadata
- Validates company existence and accessibility in the CRM
- Establishes company ID for subsequent contact filtering

**Search Strategy**:
- Uses domain-based search to find matching companies
- Retrieves full company property set for context
- Handles cases where companies may not exist or be accessible

**Data Flow**:
- Input: Structured domain information from data preparation phase
- Output: Company details including company ID for contact association
- Next Phase: Contact retrieval filtered by the discovered company

**Error Handling**: The workflow includes conditional logic to handle scenarios where no company is found, providing fallback contact search options.

### Contact Retrieval & Discord Data Processing

This functional block handles the dual-strategy contact retrieval process to find HubSpot contacts with Discord information.

**Purpose**: Retrieve contacts from HubSpot using both company-based and domain-based search strategies to maximize Discord user discovery.

**Tasks Included**:
- **hub_spot_search_contacts (Company-based)**: Searches contacts associated with the discovered company ID
- **hub_spot_search_contacts (Domain-based)**: Fallback search using domain matching for broader contact discovery

**Dual Search Strategy**:
1. **Primary Search**: Filters contacts by `associatedcompanyid` using the company discovered in the previous phase
2. **Fallback Search**: Uses `CONTAINS_TOKEN` operation on the domain to catch contacts not properly associated with companies

**Target Data Properties**:
- `discord_id`: Unique Discord user identifier
- `discord_username`: Discord display name
- `email`: Contact email for identification and validation

**Search Logic**:
- Both searches retrieve the same Discord-specific properties
- Conditional execution based on company discovery results
- Comprehensive contact property retrieval for complete user profiles

**Data Flow**:
- Input: Company ID from company discovery OR domain from data preparation
- Output: Raw contact lists with Discord properties
- Next Phase: Data formatting and result compilation

### Results Formatting & Output Generation

This functional block processes the raw contact data and formats it into a structured output for Discord user identification.

**Purpose**: Transform raw HubSpot contact data into a clean, formatted list of Discord users with proper validation and filtering.

**Tasks Included**:
- **transforms_set_variable**: Processes and formats contact data using advanced Jinja2 templating
- **END**: Workflow completion and final output delivery

**Data Processing Logic**:
- Filters contacts to include only those with valid Discord information
- Excludes contacts with missing or invalid Discord IDs ("Not Found")
- Creates structured output with user-friendly labels and identifiers

**Output Format**:
Each valid Discord user is formatted as:
```json
{
  "label": "discord_username (${USER_EMAIL})",
  "id": "discord_username,discord_id"
}
```

**Quality Assurance**:
- Validates presence of `discord_username`, `discord_id`, and `email`
- Filters out incomplete or invalid Discord data
- Ensures clean, consistent output formatting

**Conditional Flow**:
- **Results Path**: Executes when valid Discord users are found
- **No Results Path**: Handles empty result sets gracefully

**Data Flow**:
- Input: Raw contact arrays from HubSpot searches
- Output: Formatted array of Discord user objects ready for consumption
- Final State: Clean, validated Discord user list for the requesting system

## Tasks

This workflow contains 7 tasks:

### 1. BEGIN
**Description:** Action that does nothing
**Action:** ``core.noop``
**Next Tasks:** 1 transition(s) defined

### 2. END
**Description:** Action that does nothing
**Action:** ``core.noop``
**Next Tasks:** 1 transition(s) defined

### 3. hub_spot_search_companies
**Description:** search companies in hubspot
**Action:** ``hubspot.search_companies``
**Next Tasks:** 1 transition(s) defined

### 4. hub_spot_search_contacts
**Description:** search contacts in hubspot
**Action:** ``hubspot.search_contacts``
**Next Tasks:** 1 transition(s) defined

### 5. hub_spot_search_contacts
**Description:** search contacts in hubspot
**Action:** ``hubspot.search_contacts``
**Next Tasks:** 1 transition(s) defined

### 6. split_data_values
**Description:** Set a variable as an action
**Action:** ``transforms.set_variable``
**Next Tasks:** 1 transition(s) defined

### 7. transforms_set_variable
**Description:** Set a variable as an action
**Action:** ``transforms.set_variable``
**Transition Mode:** FOLLOW_FIRST
**Join:** 1 (waits for 1 incoming transitions)
**Next Tasks:** 2 transition(s) defined

## Output

This workflow produces the following output:

- **options:** `{{ CTX.format_options }}`

## Technical Details

- **Workflow Timeout:** 28800 seconds
- **Total Tasks:** 7
- **Documentation Sections:** 4

---

*This README was automatically generated from the Rewst workflow export bundle.*
## Workflow Execution Details

### Overview
- **Total Tasks**: 7
- **Workflow Type**: OPTION_GENERATOR
- **Parallel Branches**: 1
- **Join Points**: 1
- **Resolved References**: 23

### Task Flow

#### 1. BEGIN
- **Action**: `core.noop`
- **Task ID**: `018fc60be2687cf5afeb4d08a5f19a6d`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1

#### 2. END
- **Action**: `core.noop`
- **Task ID**: `018fc60be2687cf5afeb4d0777030e0d`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 0

#### 3. hub_spot_search_companies
- **Action**: `hubspot.search_companies`
- **Task ID**: `c7690c9c522240c2ad8d3a65b1416bd0`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

#### 4. hub_spot_search_contacts
- **Action**: `hubspot.search_contacts`
- **Task ID**: `90823be9ff24497bb19b3a3d3def4576`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

#### 5. hub_spot_search_contacts
- **Action**: `hubspot.search_contacts`
- **Task ID**: `6ef4a5db14fc4ea491f81542a0a06b30`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

#### 6. split_data_values
- **Action**: `transforms.set_variable`
- **Task ID**: `c793323b32db4bc58b474ab18061329d`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

#### 7. transforms_set_variable
- **Action**: `transforms.set_variable`
- **Task ID**: `018fc60be2687cf5afeb4d0a6d2186a2`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_FIRST
- **Join Count**: 1
- **Next Tasks**: 2
- **Has Input Configuration**: Yes

### Resolved References
The following references were resolved from @@@placeholder@@@ format to actual values:

- `transition_id_ref3` → `${UUID}`
- `workflow_task_id_ref3` → `c7690c9c522240c2ad8d3a65b1416bd0`
- `action_ref1` → `core.noop`
- `transition_id_ref7` → `${UUID}`
- `workflow_task_id_ref1` → `018fc60be2687cf5afeb4d08a5f19a6d`
- `workflow_task_id_ref4` → `90823be9ff24497bb19b3a3d3def4576`
- `transition_id_ref5` → `${UUID}`
- `transition_id_ref4` → `${UUID}`
- `workflow_task_id_ref7` → `018fc60be2687cf5afeb4d0a6d2186a2`
- `action_ref4` → `transforms.set_variable`
- `workflow_note_id_ref3` → `${UUID}`
- `transition_id_ref1` → `${UUID}`
- `action_ref2` → `hubspot.search_companies`
- `workflow_note_id_ref4` → `${UUID}`
- `transition_id_ref8` → `${UUID}`
- `workflow_note_id_ref1` → `${UUID}`
- `workflow_task_id_ref6` → `c793323b32db4bc58b474ab18061329d`
- `workflow_note_id_ref2` → `${UUID}`
- `workflow_task_id_ref5` → `6ef4a5db14fc4ea491f81542a0a06b30`
- `transition_id_ref6` → `${UUID}`
- `workflow_task_id_ref2` → `018fc60be2687cf5afeb4d0777030e0d`
- `action_ref3` → `hubspot.search_contacts`
- `transition_id_ref2` → `${UUID}`
