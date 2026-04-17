# [OG] HubSpot Companies w/ Discord Active

## Overview
**Workflow Type:** OPTION_GENERATOR  
**Export Date:** 2025-10-11T16:13:56.809505+00:00  
**Bundle Version:** 2  
**Original Bundle File:** `workflow-018fc5cc-57a0-7d9d-bece-4bf2a6401627_20251011_161356.bundle.json`  

## Workflow Documentation

### Data Retrieval & Filtering

This functional block handles the initial data collection from HubSpot, specifically targeting companies that are active on Discord.

**Purpose**: Retrieve and filter HubSpot companies based on Discord activity and customer status criteria.

**Key Tasks**:
- **hub_spot_search_companies**: Searches HubSpot CRM for companies meeting specific criteria:
  - Must have `discord_active` property set to "true"
  - Excludes companies with type "Termination Pending" or "Terminated Customer"
  - Retrieves comprehensive company properties including contact info, revenue data, Discord server details, and organizational metadata

**Filtering Criteria**:
- Discord Active: Only companies actively using Discord
- Customer Status: Excludes terminated or pending termination customers
- Comprehensive Data: Pulls 50+ company properties for complete company profiles

**Output**: The filtered company data is published as `filtered_company_output` and serves as input for the data transformation phase.

**Connection to Next Block**: The retrieved company data flows directly to the transformation block where it gets formatted for Discord display purposes.

### Data Transformation & Output Formatting

This functional block transforms the raw HubSpot company data into a structured format suitable for Discord server selection and display.

**Purpose**: Convert company data into user-friendly dropdown options with proper labeling and identification for Discord integration.

**Key Tasks**:
- **transforms_set_variable**: Creates formatted dropdown options from company data using Jinja2 templating:
  - Generates human-readable labels combining company name, domain, and Discord server
  - Creates unique identifiers using domain and Discord server information
  - Handles default Discord server assignment ("REWST-COMMUNITY") for companies without specific servers
  - Formats domains by replacing dots with dashes for compatibility

**Transformation Logic**:
- **Label Format**: "Company Name, domain-format (DISCORD-SERVER)"
- **ID Format**: "domain.com,DISCORD-SERVER"
- **Default Handling**: Uses "REWST-COMMUNITY" as fallback Discord server
- **Domain Processing**: Converts dots to dashes in domain names for display

**Output Structure**:
Each company becomes an object with:
- `label`: User-friendly display name
- `id`: Unique identifier for backend processing

**Final Output**: The transformed data is published as `format_options` and represents the final workflow result - a list of formatted company options ready for Discord server selection interfaces.

**Connection from Previous Block**: Receives `filtered_company_output` from the data retrieval block and processes each company record through the transformation logic.

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

### 3. hub_spot_search_companies
**Description:** search companies in hubspot
**Action:** ``hubspot.search_companies``
**Next Tasks:** 1 transition(s) defined

### 4. transforms_set_variable
**Description:** Set a variable as an action
**Action:** ``transforms.set_variable``
**Next Tasks:** 1 transition(s) defined

## Output

This workflow produces the following output:

- **options:** `{{ CTX.format_options }}`

## Technical Details

- **Workflow Timeout:** 28800 seconds
- **Total Tasks:** 4
- **Documentation Sections:** 2

---

*This README was automatically generated from the Rewst workflow export bundle.*
## Workflow Execution Details

### Overview
- **Total Tasks**: 4
- **Workflow Type**: OPTION_GENERATOR
- **Parallel Branches**: 0
- **Join Points**: 0
- **Resolved References**: 13

### Task Flow

#### 1. BEGIN
- **Action**: `core.noop`
- **Task ID**: `018fc5cc579f7c1290016e0d3a29d0d1`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1

#### 2. END
- **Action**: `core.noop`
- **Task ID**: `018fc5cc579f7c1290016e0ff142d2dc`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 0

#### 3. hub_spot_search_companies
- **Action**: `hubspot.search_companies`
- **Task ID**: `687da05d338f4f39b586f243ef692cb9`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

#### 4. transforms_set_variable
- **Action**: `transforms.set_variable`
- **Task ID**: `74462433f0af4cf696092201a8e4ab44`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

### Resolved References
The following references were resolved from @@@placeholder@@@ format to actual values:

- `action_ref3` → `transforms.set_variable`
- `workflow_note_id_ref1` → `${UUID}`
- `workflow_note_id_ref2` → `${UUID}`
- `action_ref1` → `core.noop`
- `workflow_task_id_ref3` → `687da05d338f4f39b586f243ef692cb9`
- `workflow_task_id_ref4` → `74462433f0af4cf696092201a8e4ab44`
- `action_ref2` → `hubspot.search_companies`
- `transition_id_ref2` → `${UUID}`
- `transition_id_ref3` → `${UUID}`
- `workflow_task_id_ref2` → `018fc5cc579f7c1290016e0ff142d2dc`
- `transition_id_ref4` → `${UUID}`
- `workflow_task_id_ref1` → `018fc5cc579f7c1290016e0d3a29d0d1`
- `transition_id_ref1` → `${UUID}`
