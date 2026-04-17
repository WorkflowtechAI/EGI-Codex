# HubSpot CSM Lookup [OG]

## Overview
**Workflow Type:** OPTION_GENERATOR  
**Export Date:** 2025-10-11T16:11:38.869806+00:00  
**Bundle Version:** 2  
**Original Bundle File:** `workflow-018ff054-c3e8-7346-8e85-f5a007f99bab_20251011_161138.bundle.json`  

## Parameters

- **domain** (`string`) - Optional
- **org_id** (`string`) - Optional
- **discord** (`string`) - Optional
- **lookup_picker** (`string`) - Optional

## Workflow Documentation

### Input Processing & Routing Logic

This functional block handles the initial workflow input processing and determines the appropriate lookup path based on the available input parameters.

**Purpose**: Route the workflow execution based on input type (Discord username, organization ID, or domain) and prepare the necessary context variables for downstream processing.

**Key Tasks**:
- **begin**: Entry point that evaluates input conditions and routes to appropriate lookup paths
  - Discord lookup path: When `CTX.discord` is provided
  - Organization lookup path: When `CTX.org_id` is provided  
  - Domain lookup path: When `CTX.domain` is provided

- **rewst_get_organization**: Retrieves organization data from Rewst when org_id is provided
  - Fetches organization details including domain information
  - Enables domain-based company lookup in HubSpot

**Input Processing Logic**:
- Supports three different input types for flexible CSM lookup
- Sets appropriate context variables (`company_lookup`, `field_name`) for HubSpot searches
- Handles conditional routing to ensure the correct lookup method is used

**Flow Connections**:
- **Inputs**: Workflow context variables (discord, org_id, domain)
- **Outputs**: Prepared lookup parameters for HubSpot company/contact searches
- **Next Phase**: Company & Contact Discovery block

### Company & Contact Discovery

This functional block performs the core HubSpot data discovery operations to locate companies and contacts based on the input parameters from the routing logic.

**Purpose**: Search HubSpot for relevant company and contact records, extract owner information, and handle the discovery workflow paths including Discord-based contact lookups.

**Key Tasks**:
- **hub_spot_search_contacts**: Searches HubSpot contacts by Discord username
  - Filters contacts using `discord_username` field
  - Retrieves contact properties: discord_username, email, company, associatedcompanyid
  - Extracts email domain for company lookup when contact is found
  - Routes to company search or "not found" handling

- **hub_spot_search_companies**: Searches HubSpot companies using flexible criteria
  - Supports multiple search fields: domain, product_organization_id, or derived from contact email
  - Retrieves comprehensive company properties including CSM data
  - Extracts `hubspot_owner_id` for CSM identification
  - Uses join logic to consolidate multiple input paths

**Discovery Logic**:
- Discord path: Contact → Email domain → Company lookup
- Direct paths: Organization ID or domain → Company lookup
- Handles both successful discoveries and "not found" scenarios

**Data Extraction**:
- Company properties: name, domain, hubspot_owner_id, customer_success_manager, lifecycle stage
- Contact properties: email, discord_username, company associations
- Owner ID extraction for CSM name resolution

**Flow Connections**:
- **Inputs**: Lookup parameters from Input Processing block
- **Outputs**: Company data with owner_id for CSM resolution
- **Next Phase**: CSM Resolution & Output Generation block

### CSM Resolution & Output Generation

This functional block resolves the Customer Success Manager information and generates the final workflow output in the required format for downstream consumption.

**Purpose**: Convert HubSpot owner IDs to human-readable CSM names and format the results as structured options for UI components or further processing.

**Key Tasks**:
- **hubspot_get_users**: Retrieves all HubSpot users/owners data
  - Makes authenticated API request to `/crm/v3/owners` endpoint
  - Provides complete user directory for CSM name resolution
  - Publishes results as `hs_users` for lookup operations

- **output**: Generates successful CSM lookup results
  - Matches `owner_id` from company data against HubSpot users
  - Constructs formatted output with firstName and lastName
  - Creates value/label pairs for dropdown or selection components
  - Uses Jinja templating for dynamic name concatenation

- **contact_not_found**: Handles unsuccessful lookup scenarios
  - Generates standardized "Not Found" response
  - Maintains consistent output format for error states
  - Provides fallback when no matching records are discovered

**Output Format**:
- Successful lookup: `[{"value": "John Doe", "label": "John Doe"}]`
- Failed lookup: `[{"value": "Not Found", "label": "Not Found"}]`
- Structured for immediate use in UI components

**Resolution Logic**:
- Cross-references owner_id with complete HubSpot user directory
- Handles missing or invalid owner IDs gracefully
- Ensures consistent output format regardless of lookup success

**Flow Connections**:
- **Inputs**: Company data with owner_id from Discovery block
- **Outputs**: Formatted CSM name or "Not Found" message
- **Termination**: Workflow completion with structured results

## Tasks

This workflow contains 7 tasks:

### 1. begin
**Description:** Action that does nothing
**Action:** ``core.noop``
**Next Tasks:** 3 transition(s) defined

### 2. contact_not_found
**Description:** Set a variable as an action
**Action:** ``transforms.set_variable``
**Next Tasks:** 1 transition(s) defined

### 3. hub_spot_search_companies
**Description:** search companies in hubspot
**Action:** ``hubspot.search_companies``
**Join:** 1 (waits for 1 incoming transitions)
**Next Tasks:** 1 transition(s) defined

### 4. hub_spot_search_contacts
**Description:** search contacts in hubspot
**Action:** ``hubspot.search_contacts``
**Transition Mode:** FOLLOW_FIRST
**Next Tasks:** 2 transition(s) defined

### 5. hubspot_get_users
**Description:** Generic action for making authenticated requests against the HubSpot API
**Action:** ``hubspot.generic_request``
**Next Tasks:** 1 transition(s) defined

### 6. output
**Description:** Set a variable as an action
**Action:** ``transforms.set_variable``
**Next Tasks:** 1 transition(s) defined

### 7. rewst_get_organization
**Description:** Get data for a single organization in Rewst
**Action:** ``rewst.get_organization``
**Next Tasks:** 1 transition(s) defined

## Output

This workflow produces the following output:

- **options:** `{{ CTX.options }}`

## Technical Details

- **Workflow Timeout:** 28800 seconds
- **Total Tasks:** 7
- **Documentation Sections:** 3

---

*This README was automatically generated from the Rewst workflow export bundle.*
## Workflow Execution Details

### Overview
- **Total Tasks**: 7
- **Workflow Type**: OPTION_GENERATOR
- **Parallel Branches**: 2
- **Join Points**: 1
- **Resolved References**: 26

### Task Flow

#### 1. begin
- **Action**: `core.noop`
- **Task ID**: `9bb75b664fab41d6ad465f2210de5ee6`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 3

#### 2. contact_not_found
- **Action**: `transforms.set_variable`
- **Task ID**: `f43fc36cfc4f402e9b3c6fcd233a7d90`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 0
- **Has Input Configuration**: Yes

#### 3. hub_spot_search_companies
- **Action**: `hubspot.search_companies`
- **Task ID**: `5ac9fa6f290845fb98ed7b662a4fba4f`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 1
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

#### 4. hub_spot_search_contacts
- **Action**: `hubspot.search_contacts`
- **Task ID**: `841ad059f62b48b88810f295a4768e38`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_FIRST
- **Join Count**: 0
- **Next Tasks**: 2
- **Has Input Configuration**: Yes

#### 5. hubspot_get_users
- **Action**: `hubspot.generic_request`
- **Task ID**: `34df89eac4da4533a02ec4ac9833cd99`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

#### 6. output
- **Action**: `transforms.set_variable`
- **Task ID**: `96c936112445442fa0d6e43716c1694d`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 0
- **Has Input Configuration**: Yes

#### 7. rewst_get_organization
- **Action**: `rewst.get_organization`
- **Task ID**: `29e234a649c1472785ced7ac0f3b7548`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

### Resolved References
The following references were resolved from @@@placeholder@@@ format to actual values:

- `action_ref2` → `transforms.set_variable`
- `transition_id_ref8` → `${UUID}`
- `transition_id_ref4` → `${UUID}`
- `workflow_task_id_ref1` → `9bb75b664fab41d6ad465f2210de5ee6`
- `workflow_task_id_ref7` → `29e234a649c1472785ced7ac0f3b7548`
- `transition_id_ref5` → `${UUID}`
- `workflow_task_id_ref2` → `f43fc36cfc4f402e9b3c6fcd233a7d90`
- `workflow_task_id_ref6` → `96c936112445442fa0d6e43716c1694d`
- `transition_id_ref1` → `${UUID}`
- `transition_id_ref10` → `${UUID}`
- `workflow_task_id_ref4` → `841ad059f62b48b88810f295a4768e38`
- `workflow_note_id_ref2` → `${UUID}`
- `transition_id_ref9` → `${UUID}`
- `workflow_task_id_ref5` → `34df89eac4da4533a02ec4ac9833cd99`
- `workflow_note_id_ref1` → `${UUID}`
- `action_ref4` → `hubspot.search_contacts`
- `transition_id_ref3` → `${UUID}`
- `transition_id_ref7` → `${UUID}`
- `action_ref3` → `hubspot.search_companies`
- `workflow_task_id_ref3` → `5ac9fa6f290845fb98ed7b662a4fba4f`
- `transition_id_ref2` → `${UUID}`
- `action_ref6` → `rewst.get_organization`
- `action_ref5` → `hubspot.generic_request`
- `action_ref1` → `core.noop`
- `transition_id_ref6` → `${UUID}`
- `workflow_note_id_ref3` → `${UUID}`
