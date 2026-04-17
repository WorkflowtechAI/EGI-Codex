# What permissions does this endpoint need

## Overview
**Workflow Type:** STANDARD  
**Export Date:** 2025-10-11T16:12:13.169333+00:00  
**Bundle Version:** 2  
**Original Bundle File:** `workflow-018cd5d1-0df4-7d82-8b43-2d1c8d7b48dc_20251011_161213.bundle.json`  

## Parameters

- **endpoints** (`array`) - **Required** (multiline)
- **returnAll** (`boolean`) - Optional

## Workflow Documentation

### Data Retrieval & CSV Processing

This functional block handles the initial data acquisition and parsing phase of the workflow.

**Purpose**: Fetch the Microsoft Graph permissions CSV from the external repository and convert it into a structured JSON format for further processing.

**Tasks Included**:
- `get_permissions_csv`: Performs HTTP GET request to retrieve the permissions CSV file from the GitHub repository (https://raw.githubusercontent.com/merill/graphpermissions.github.io/main/permissions.csv)
- `convert_csv_to_object`: Parses the raw CSV data into JSON objects, using the first line as keys and subsequent lines as values

**Key Functionality**:
- Establishes the data foundation for the entire workflow
- Handles external data dependency with proper HTTP timeout (5 seconds)
- Transforms unstructured CSV data into structured objects for programmatic access
- Creates the base dataset that contains all Microsoft Graph API permissions

**Data Flow**:
- Input: External CSV URL
- Output: Structured JSON array with permission objects containing fields like PermissionName, ApplicationPermission, DelegatePermission, API, etc.
- Connects to: Permission filtering logic based on the `returnAll` context variable

**Critical Dependencies**:
- External network access to GitHub repository
- CSV format consistency from the source
- Proper parsing of permission attributes for downstream filtering

### Permission Classification & Filtering Logic

This functional block implements the core business logic for filtering and classifying Microsoft Graph permissions based on delegation type and endpoint matching.

**Purpose**: Apply conditional logic to filter permissions based on whether all permissions are requested or only delegated permissions, then match them against specific API endpoints.

**Tasks Included**:
- `delegated_only`: Filters permissions to include only those with DelegatePermission = "True"
- `all_permissions`: Includes all permissions regardless of delegation type when returnAll context is true
- `filter_permissionlist_based_on_input`: Matches permissions against provided endpoints using complex Jinja2 logic

**Key Functionality**:
- **Conditional Branching**: Uses `CTX.returnAll` to determine whether to return all permissions or only delegated ones
- **Permission Filtering**: Filters the complete permission set based on delegation type
- **Endpoint Matching**: Sophisticated matching logic that:
  - Iterates through provided endpoints from `CTX.endpoints`
  - Parses API field to extract HTTP method and endpoint path
  - Matches endpoint patterns against permission API definitions
  - Removes duplicates to ensure unique permission sets

**Business Logic**:
- Supports two modes: comprehensive (all permissions) vs. restricted (delegated only)
- Handles complex API endpoint matching where permissions are associated with specific HTTP methods and paths
- Maintains data integrity by preventing duplicate permissions in results

**Data Transformations**:
- Input: Structured permission objects + endpoint list + returnAll flag
- Processing: Conditional filtering + pattern matching + deduplication
- Output: Filtered permission list matching specified endpoints and delegation requirements

**Critical Decision Points**:
- Branch selection based on `CTX.returnAll` context variable
- API string parsing to extract method and endpoint components
- Permission uniqueness validation during endpoint matching

### Results Aggregation & Output Formatting

This functional block handles the final stage of data processing, transforming filtered permissions into the structured output format required by consumers.

**Purpose**: Aggregate filtered permissions by API endpoint and format them into a comprehensive, organized structure that groups permissions by their associated APIs.

**Tasks Included**:
- `return_required_app_permissions`: Intermediate step that prepares for final output formatting
- `permissions_per_endpoint`: Complex aggregation logic that groups permissions by API and creates the final structured output

**Key Functionality**:
- **Permission Aggregation**: Groups permissions by their API endpoint to eliminate redundancy
- **Data Restructuring**: Transforms flat permission list into hierarchical structure organized by API
- **Metadata Preservation**: Maintains important attributes like DocUri and IsBeta flags for each API group
- **Deduplication**: Ensures unique permission names within each API group

**Output Structure Creation**:
The final task creates a comprehensive data structure with:
- **API**: The specific Microsoft Graph API endpoint
- **Permissions**: Array of unique permission names required for that endpoint
- **DocUri**: Documentation URL for the API endpoint
- **IsBeta**: Boolean flag indicating if the API is in beta status

**Data Transformation Logic**:
- Input: Filtered permission objects from previous filtering stage
- Processing: 
  - Creates dictionary grouped by API endpoint
  - Aggregates permission names per API
  - Preserves metadata (DocUri, IsBeta) for each API group
  - Ensures permission uniqueness within each group
- Output: Array of API objects with associated permissions and metadata

**Business Value**:
- Provides clear mapping between API endpoints and required permissions
- Organizes complex permission data into consumable format
- Maintains traceability with documentation links
- Supports API lifecycle management with beta status indicators

**Final Workflow Output**:
This block produces the ultimate deliverable: a structured list showing exactly which permissions are needed for each Microsoft Graph API endpoint, formatted for easy consumption by developers and administrators.

## Tasks

This workflow contains 7 tasks:

### 1. all_permissions
**Description:** Action that does nothing
**Action:** ``core.noop``
**Next Tasks:** 1 transition(s) defined

### 2. convert_csv_to_object
**Description:** Parse CSV Data into JSON. The first line will be keys, and subsequent lines will be values
**Action:** ``transforms.parse_csv``
**Transition Mode:** FOLLOW_FIRST
**Next Tasks:** 2 transition(s) defined

### 3. delegated_only
**Description:** Action that does nothing
**Action:** ``core.noop``
**Next Tasks:** 1 transition(s) defined

### 4. filter_permissionlist_based_on_input
**Description:** Set a variable as an action
**Action:** ``transforms.set_variable``
**Join:** 1 (waits for 1 incoming transitions)
**Next Tasks:** 1 transition(s) defined

### 5. get_permissions_csv
**Description:** Perform an HTTP request
**Action:** ``core.http_request``
**Next Tasks:** 1 transition(s) defined

### 6. permissions_per_endpoint
**Description:** Set a variable as an action
**Action:** ``transforms.set_variable``
**Join:** 1 (waits for 1 incoming transitions)
**Next Tasks:** 1 transition(s) defined

### 7. return_required_app_permissions
**Description:** Action that does nothing
**Action:** ``core.noop``
**Next Tasks:** 1 transition(s) defined

## Technical Details

- **Workflow Timeout:** 28800 seconds
- **Total Tasks:** 7
- **Documentation Sections:** 3

---

*This README was automatically generated from the Rewst workflow export bundle.*
## Workflow Execution Details

### Overview
- **Total Tasks**: 7
- **Workflow Type**: STANDARD
- **Parallel Branches**: 1
- **Join Points**: 2
- **Resolved References**: 22

### Task Flow

#### 1. all_permissions
- **Action**: `core.noop`
- **Task ID**: `b71c8e3f745943f5a24b7a9ead2e40b9`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1

#### 2. convert_csv_to_object
- **Action**: `transforms.parse_csv`
- **Task ID**: `d3a88b1763a04529b0e08fc268bdf3b1`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_FIRST
- **Join Count**: 0
- **Next Tasks**: 2
- **Has Input Configuration**: Yes

#### 3. delegated_only
- **Action**: `core.noop`
- **Task ID**: `523754f77a7c4c15b4cc5306821142ac`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1

#### 4. filter_permissionlist_based_on_input
- **Action**: `transforms.set_variable`
- **Task ID**: `efd4ea1ba0e243f395a7b1878c97fdbc`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 1
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

#### 5. get_permissions_csv
- **Action**: `core.http_request`
- **Task ID**: `06596fe0b96c72f48000064aa048dd7a`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

#### 6. permissions_per_endpoint
- **Action**: `transforms.set_variable`
- **Task ID**: `0320780ca8bc4bc99222bd0548241337`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 1
- **Next Tasks**: 0
- **Has Input Configuration**: Yes

#### 7. return_required_app_permissions
- **Action**: `core.noop`
- **Task ID**: `06596fe0b96c75a58000cecb3af6a12c`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1

### Resolved References
The following references were resolved from @@@placeholder@@@ format to actual values:

- `transition_id_ref3` → `${UUID}`
- `workflow_task_id_ref3` → `523754f77a7c4c15b4cc5306821142ac`
- `transition_id_ref2` → `${UUID}`
- `workflow_task_id_ref4` → `efd4ea1ba0e243f395a7b1878c97fdbc`
- `transition_id_ref1` → `${UUID}`
- `action_ref1` → `core.noop`
- `workflow_task_id_ref2` → `d3a88b1763a04529b0e08fc268bdf3b1`
- `workflow_task_id_ref1` → `b71c8e3f745943f5a24b7a9ead2e40b9`
- `action_ref2` → `transforms.parse_csv`
- `transition_id_ref4` → `${UUID}`
- `transition_id_ref8` → `${UUID}`
- `workflow_note_id_ref1` → `${UUID}`
- `workflow_note_id_ref3` → `${UUID}`
- `transition_id_ref5` → `${UUID}`
- `action_ref4` → `core.http_request`
- `workflow_note_id_ref2` → `${UUID}`
- `workflow_task_id_ref5` → `06596fe0b96c72f48000064aa048dd7a`
- `action_ref3` → `transforms.set_variable`
- `transition_id_ref7` → `${UUID}`
- `workflow_task_id_ref7` → `06596fe0b96c75a58000cecb3af6a12c`
- `transition_id_ref6` → `${UUID}`
- `workflow_task_id_ref6` → `0320780ca8bc4bc99222bd0548241337`
