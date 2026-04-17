# CSP/CPV Permission Checker

## Overview
**Workflow Type:** STANDARD  
**Export Date:** 2025-10-11T16:12:17.096599+00:00  
**Bundle Version:** 2  
**Original Bundle File:** `workflow-39d00a4a-3e92-4594-b74a-fbb2a880ce28_20251011_161217.bundle.json`  

## Workflow Documentation

### Workflow Initialization & Configuration

This functional block handles the initial setup and configuration of the CSP/CPV Permission Checker workflow.

**Purpose**: Establishes the baseline configuration by defining the expected privileged roles that need to be monitored and validated within the Microsoft 365 environment.

**Key Tasks**:
- **BEGIN**: Initializes the workflow execution and publishes the predefined list of critical administrative roles

**Expected Roles Configured**:
The workflow monitors 12 high-privilege administrative roles including:
- Application Administrator
- User Administrator  
- Intune Administrator
- Exchange Administrator
- Security Administrator
- Cloud App Security Administrator
- Cloud Device Administrator
- Teams Administrator
- SharePoint Administrator
- Authentication Policy Administrator
- Privileged Role Administrator
- Privileged Authentication Administrator

**Data Flow**: 
- Input: None (workflow start)
- Output: `expected_roles` context variable containing the complete list of roles to be analyzed

**Connection to Next Block**: Upon successful initialization, this block triggers the Permission Validation process to verify organizational access permissions before proceeding with role analysis.

### Permission Validation & Authentication Check

This functional block validates organizational permissions and handles authentication verification before proceeding with detailed role analysis.

**Purpose**: Ensures the workflow has the necessary permissions to access Microsoft Graph API endpoints and retrieve user/role information from the target organization.

**Key Tasks**:
- **microsoft_graph_graph_api_request**: Performs an initial test request to the `/users` endpoint using delegated admin permissions to verify organizational access
- **end_with_error**: Handles authentication failures by gracefully terminating the workflow with appropriate error context

**Authentication Strategy**:
- Uses delegated admin permissions (`use_delegated_admin: true`)
- Targets the `/users` endpoint as a permission validation test
- Implements conditional branching based on authentication success/failure

**Error Handling**:
- On SUCCESS: Proceeds to role assignment analysis
- On FAILURE: Publishes error context and terminates workflow execution
- Error information is captured in `CTX.check_org_permission.error`

**Data Flow**:
- Input: Organizational context and authentication credentials
- Output: `check_org_permission` result containing validation status
- Error Output: Error details published to workflow context

**Connection to Next Block**: Upon successful permission validation, this block enables the Role Assignment Analysis process to begin collecting detailed role assignment data for each configured administrative role.

### Role Assignment Analysis & Data Collection

This functional block performs comprehensive analysis of role assignments for all configured administrative roles and consolidates the collected data into a structured format.

**Purpose**: Systematically retrieves detailed role assignment information for each privileged administrative role, including principal details (users/groups assigned to each role) and consolidates this data for analysis or reporting.

**Key Tasks**:
- **microsoft_graph_graph_api_request_1**: Executes parallel API calls to retrieve role assignments for each administrative role using the Microsoft Graph `/roleManagement/directory/roleAssignments` endpoint
- **transforms_beta_append_with_items_results**: Processes and consolidates the collected role assignment data by appending detailed results to the original role configuration

**Data Collection Strategy**:
- Uses `with_items` iteration over `expected_roles` to process each role individually
- Queries role assignments with filtering: `roleDefinitionId eq '{{ item().Id }}'`
- Expands principal information to include user/group details: `$expand=principal`
- Executes with concurrency of 1 to manage API rate limits
- Uses standard permissions (`use_delegated_admin: false`)

**Data Processing**:
- Collects results from all parallel role queries into `check_roles` context
- Transforms and appends detailed role assignment data to the original `expected_roles` list
- Creates enriched dataset with `role_detail` attribute containing assignment information
- Final consolidated data stored in `collected_role_data` context

**Data Flow**:
- Input: `expected_roles` list from initialization block
- Processing: Individual role assignment queries with principal expansion
- Output: `collected_role_data` containing comprehensive role assignment analysis

**Key Insights Generated**:
- Complete inventory of users/groups assigned to each privileged role
- Principal details including user information and assignment scope
- Consolidated view of administrative access across the organization

**Workflow Completion**: This block represents the final data collection phase, producing the complete CSP/CPV permission analysis dataset for downstream consumption or reporting.

## Tasks

This workflow contains 5 tasks:

### 1. BEGIN
**Description:** Action that does nothing
**Action:** ``core.noop``
**Next Tasks:** 1 transition(s) defined

### 2. end_with_error
**Description:** Action that does nothing
**Action:** ``core.noop``
**Next Tasks:** 1 transition(s) defined

### 3. microsoft_graph_graph_api_request
**Description:** Send a request to any Microsoft Graph endpoint
**Action:** ``microsoft_graph.graph_api_request``
**Transition Mode:** FOLLOW_FIRST
**Next Tasks:** 2 transition(s) defined

### 4. microsoft_graph_graph_api_request_1
**Description:** Send a request to any Microsoft Graph endpoint
**Action:** ``microsoft_graph.graph_api_request``
**Loop/Iteration:** Yes
**Next Tasks:** 1 transition(s) defined

### 5. transforms_beta_append_with_items_results
**Description:** This function extracts a nested 'result.result' list from each item in a specified list (Collected List) and appends it to another specified list (Base List) as an attribute with a provided name (Attribute Name).
**Action:** ``transforms.append_collected_results``
**Next Tasks:** 1 transition(s) defined

## Technical Details

- **Workflow Timeout:** 28800 seconds
- **Total Tasks:** 5
- **Documentation Sections:** 3

---

*This README was automatically generated from the Rewst workflow export bundle.*
## Workflow Execution Details

### Overview
- **Total Tasks**: 5
- **Workflow Type**: STANDARD
- **Parallel Branches**: 1
- **Join Points**: 0
- **Resolved References**: 17

### Task Flow

#### 1. BEGIN
- **Action**: `core.noop`
- **Task ID**: `c7c74c7026d342e0b93e71e3549b8133`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1

#### 2. end_with_error
- **Action**: `core.noop`
- **Task ID**: `926aacb8c7524f14abc43f832e3a64c4`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 0

#### 3. microsoft_graph_graph_api_request
- **Action**: `microsoft_graph.graph_api_request`
- **Task ID**: `a1a710fd6f9d414682fa8b5a81cb1312`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_FIRST
- **Join Count**: 0
- **Next Tasks**: 2
- **Has Input Configuration**: Yes

#### 4. microsoft_graph_graph_api_request_1
- **Action**: `microsoft_graph.graph_api_request`
- **Task ID**: `6524f0333eb24a629077b2649cc94984`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1
- **Has Input Configuration**: Yes
- **Loop/Iterator**: Yes

#### 5. transforms_beta_append_with_items_results
- **Action**: `transforms.append_collected_results`
- **Task ID**: `fc4af94ba6044f02b07cc294c4b73f0e`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 0
- **Has Input Configuration**: Yes

### Resolved References
The following references were resolved from @@@placeholder@@@ format to actual values:

- `workflow_note_id_ref3` → `${UUID}`
- `workflow_task_id_ref4` → `6524f0333eb24a629077b2649cc94984`
- `workflow_task_id_ref5` → `fc4af94ba6044f02b07cc294c4b73f0e`
- `action_ref2` → `microsoft_graph.graph_api_request`
- `workflow_task_id_ref3` → `a1a710fd6f9d414682fa8b5a81cb1312`
- `transition_id_ref2` → `${UUID}`
- `workflow_task_id_ref2` → `926aacb8c7524f14abc43f832e3a64c4`
- `workflow_task_id_ref1` → `c7c74c7026d342e0b93e71e3549b8133`
- `transition_id_ref1` → `${UUID}`
- `transition_id_ref5` → `${UUID}`
- `transition_id_ref6` → `${UUID}`
- `transition_id_ref4` → `${UUID}`
- `workflow_note_id_ref2` → `${UUID}`
- `action_ref3` → `transforms.append_collected_results`
- `action_ref1` → `core.noop`
- `transition_id_ref3` → `${UUID}`
- `workflow_note_id_ref1` → `${UUID}`
