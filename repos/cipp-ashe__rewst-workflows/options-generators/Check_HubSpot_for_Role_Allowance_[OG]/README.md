# Check HubSpot for Role Allowance [OG]

## Overview
**Workflow Type:** OPTION_GENERATOR  
**Export Date:** 2025-10-11T16:14:55.136361+00:00  
**Bundle Version:** 2  
**Original Bundle File:** `workflow-018fda62-3ccb-75ce-8a93-3c8266637d72_20251011_161455.bundle.json`  

## Parameters

- **data** (`string`) - Optional
- **discord_details** (`string`) - Optional
- **requested_roles** (`string`) - Optional

## Workflow Documentation

### Input Validation & Data Preparation

## Purpose
This block handles the initial validation and preparation of input data required for the HubSpot role allowance check process.

## Tasks Included
- **validate_input_data**: Validates incoming data and extracts key components
- **Begin**: Initiates the workflow and routes to parallel search operations

## Key Functions
- Parses comma-separated input data to extract domain and server name
- Extracts Discord username and ID from discord_details input
- Publishes structured variables for use in subsequent workflow phases:
  - `domain`: First part of comma-separated data input
  - `server_name`: Last part of comma-separated data input  
  - `discord_username`: First part of discord_details input
  - `discord_id`: Last part of discord_details input

## Workflow Connection
This block serves as the foundation for the entire workflow, preparing clean, structured data that feeds into both the user search and company search operations. The Begin task specifically routes the workflow into two parallel branches for comprehensive data retrieval.

## Input Requirements
- `data`: Comma-separated string containing domain and server information
- `discord_details`: Comma-separated string containing Discord username and ID

### HubSpot Data Retrieval Operations

## Purpose
This block performs parallel searches in HubSpot to retrieve both user contact information and company details needed for role determination.

## Tasks Included
- **hub_spot_search_contacts**: Searches for user contact records by Discord username
- **hub_spot_search_companies**: Searches for company records by domain

## Key Functions

### Contact Search Operations
- Searches HubSpot contacts using the extracted Discord username
- Retrieves user properties including certification level and Discord details
- Identifies certified users and publishes certified role data when applicable
- Properties retrieved: email, hs_email_domain, certified_level, discord_username, discord_id, discord_join_date, beta_participant

### Company Search Operations  
- Searches HubSpot companies using the extracted domain
- Retrieves comprehensive company properties including access levels and onboarding status
- Identifies companies with beta access (alpha_access property) and Community access (onboarding_phase property)
- Properties retrieved: domain, name, type, alpha_access, customer_success_manager, onboarding_phase, discord_server, and more

## Conditional Logic
- **Certified Users**: When contact has certified_level property, publishes "Certified Rewster" role
- **Beta Access**: When company has alpha_access property, processes beta role template and access list
- **Community Access**: When company has onboarding_phase property, assigns "Community Members" role

## Workflow Connection
This block runs in parallel after input validation and feeds role data into the final role compilation phase. Both searches can execute simultaneously, improving workflow efficiency.

### Role Compilation & Final Output

## Purpose
This block consolidates all discovered role assignments from the HubSpot searches and compiles them into a final, unified role list for Discord assignment.

## Tasks Included
- **transforms_set_variable**: Combines all role types into a single comprehensive list

## Key Functions

### Role Aggregation Logic
The task uses a Jinja2 template to merge three potential role sources:
- `CTX.beta_roles`: Roles derived from company beta access permissions
- `CTX.certified_role`: Role assigned to certified users (Certified Rewster)
- `CTX.community_role`: Role assigned to Community members based on onboarding phase

### Template Processing
```jinja2
{{ CTX.beta_roles|d([])+CTX.certified_role|d([])+CTX.community_role|d([]) }}
```

This template safely combines all role arrays, using default empty arrays (`|d([])`) to handle cases where certain role types weren't assigned.

## Role Types Processed

### Beta Roles
- Dynamically determined based on company's alpha_access list
- Matched against beta role template to assign appropriate beta access roles
- Each role includes: id, name, and default status

### Certified Role
- Static assignment: "Certified Rewster" (ID: ${SNOWFLAKE_ID})
- Assigned when user has certified_level property in HubSpot

### Community Role  
- Static assignment: "Community Members" (ID: ${SNOWFLAKE_ID})
- Assigned when company has onboarding_phase property

## Final Output
The workflow produces a consolidated array of Discord role objects, each containing:
- `id`: Discord role ID
- `name`: Human-readable role name  
- `default`: Boolean indicating if this is a default assignment

## Workflow Connection
This is the final processing step that receives input from both HubSpot search branches and produces the complete role assignment list for the requesting Discord server.

## Tasks

This workflow contains 5 tasks:

### 1. Begin
**Description:** Action that does nothing
**Action:** ``core.noop``
**Next Tasks:** 2 transition(s) defined

### 2. hub_spot_search_companies
**Description:** search companies in hubspot
**Action:** ``hubspot.search_companies``
**Next Tasks:** 2 transition(s) defined

### 3. hub_spot_search_contacts
**Description:** search contacts in hubspot
**Action:** ``hubspot.search_contacts``
**Transition Mode:** FOLLOW_FIRST
**Next Tasks:** 2 transition(s) defined

### 4. transforms_set_variable
**Description:** Set a variable as an action
**Action:** ``transforms.set_variable``
**Next Tasks:** 1 transition(s) defined

### 5. validate_input_data
**Description:** Action that does nothing
**Action:** ``core.noop``
**Next Tasks:** 1 transition(s) defined

## Output

This workflow produces the following output:

- **options:** `{{ CTX.results }}`

## Technical Details

- **Workflow Timeout:** 28800 seconds
- **Total Tasks:** 5
- **Documentation Sections:** 3

---

*This README was automatically generated from the Rewst workflow export bundle.*
## Workflow Execution Details

### Overview
- **Total Tasks**: 5
- **Workflow Type**: OPTION_GENERATOR
- **Parallel Branches**: 3
- **Join Points**: 0
- **Resolved References**: 21

### Task Flow

#### 1. Begin
- **Action**: `core.noop`
- **Task ID**: `713fd7226dc44208863fb5e2d3c340fb`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 2

#### 2. hub_spot_search_companies
- **Action**: `hubspot.search_companies`
- **Task ID**: `ad3452350e3741748b1ba3162f935328`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 2
- **Has Input Configuration**: Yes

#### 3. hub_spot_search_contacts
- **Action**: `hubspot.search_contacts`
- **Task ID**: `31867406e6d54fcc861e56d7e3a3ba3d`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_FIRST
- **Join Count**: 0
- **Next Tasks**: 2
- **Has Input Configuration**: Yes

#### 4. transforms_set_variable
- **Action**: `transforms.set_variable`
- **Task ID**: `06938a4d3308454fa4856c0fae9310d8`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 0
- **Has Input Configuration**: Yes

#### 5. validate_input_data
- **Action**: `core.noop`
- **Task ID**: `8d8804e6379542e9b2f9d568e42f4a76`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1

### Resolved References
The following references were resolved from @@@placeholder@@@ format to actual values:

- `action_ref1` → `core.noop`
- `workflow_task_id_ref2` → `ad3452350e3741748b1ba3162f935328`
- `workflow_task_id_ref3` → `31867406e6d54fcc861e56d7e3a3ba3d`
- `action_ref4` → `transforms.set_variable`
- `transition_id_ref3` → `${UUID}`
- `action_ref3` → `hubspot.search_contacts`
- `workflow_note_id_ref3` → `${UUID}`
- `transition_id_ref4` → `${UUID}`
- `workflow_task_id_ref5` → `8d8804e6379542e9b2f9d568e42f4a76`
- `transition_id_ref2` → `${UUID}`
- `template_ref1` → `[TEMPLATE: Community Beta Programs]`
- `transition_id_ref7` → `${UUID}`
- `workflow_note_id_ref2` → `${UUID}`
- `transition_id_ref8` → `${UUID}`
- `action_ref2` → `hubspot.search_companies`
- `transition_id_ref6` → `${UUID}`
- `transition_id_ref5` → `${UUID}`
- `workflow_task_id_ref1` → `713fd7226dc44208863fb5e2d3c340fb`
- `transition_id_ref1` → `${UUID}`
- `workflow_task_id_ref4` → `06938a4d3308454fa4856c0fae9310d8`
- `workflow_note_id_ref1` → `${UUID}`
