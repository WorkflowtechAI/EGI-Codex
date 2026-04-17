# [SUB] HubSpot: Update Training Property Fields

## Overview
**Workflow Type:** STANDARD  
**Export Date:** 2025-10-11T16:11:15.585160+00:00  
**Bundle Version:** 2  
**Original Bundle File:** `workflow-018f3bb0-b68d-7540-b743-bce1f444b01d_20251011_161115.bundle.json`  

## Parameters

- **training_id** (`string`) - Optional

## Workflow Documentation

### Workflow Initialization & Setup

This functional block handles the initial setup and preparation phase of the workflow.

**Purpose**: Establishes the workflow execution context and prepares for the main HubSpot API operations.

**Tasks Included**:
- **BEGIN**: Standard workflow initialization task that sets up the execution environment and triggers the main workflow logic

**Key Characteristics**:
- Entry point for all workflow executions
- No-operation action that serves as a clean starting point
- Automatically transitions to the data retrieval phase upon successful completion

**Flow Connection**: This block serves as the foundation that enables the subsequent HubSpot data retrieval operations. It ensures proper workflow state initialization before any external API calls are made.

### HubSpot Training Data Retrieval

This functional block handles the core business logic of retrieving training property data from HubSpot CRM.

**Purpose**: Fetches specific training record information from HubSpot to update training property fields.

**Tasks Included**:
- **list_trainings**: Makes authenticated GET request to HubSpot CRM API to retrieve training object data using the provided training_id

**Key Operations**:
- Constructs API endpoint: `/crm/v3/objects/trainings/{{ CTX.training_id }}`
- Uses GET method with authentication handled by HubSpot integration
- Configured with pagination disabled for single record retrieval
- Requires 2xx status code for successful completion
- Results published as 'search_trainings' for downstream use

**Input Requirements**:
- `CTX.training_id`: The specific training record identifier to retrieve

**Output Handling**:
- **Success Path**: Training data retrieved and made available for property field updates
- **Failure Path**: Error handling for failed API requests or invalid training IDs

**Flow Connection**: This block represents the core data retrieval operation that enables the workflow's primary function of updating HubSpot training property fields. The retrieved training data serves as the foundation for any subsequent property updates or processing.

## Tasks

This workflow contains 2 tasks:

### 1. BEGIN
**Description:** Action that does nothing
**Action:** ``core.noop``
**Next Tasks:** 1 transition(s) defined

### 2. list_trainings
**Action:** ``hubspot.generic_request``
**Transition Mode:** FOLLOW_FIRST
**Join:** 1 (waits for 1 incoming transitions)
**Next Tasks:** 2 transition(s) defined

## Technical Details

- **Workflow Timeout:** 28800 seconds
- **Total Tasks:** 2
- **Documentation Sections:** 2

---

*This README was automatically generated from the Rewst workflow export bundle.*
## Workflow Execution Details

### Overview
- **Total Tasks**: 2
- **Workflow Type**: STANDARD
- **Parallel Branches**: 1
- **Join Points**: 1
- **Resolved References**: 9

### Task Flow

#### 1. BEGIN
- **Action**: `core.noop`
- **Task ID**: `d4d37cc8626245a7858ac18a50584b04`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1

#### 2. list_trainings
- **Action**: `hubspot.generic_request`
- **Task ID**: `0ee56517bff0464e8adf0df3d664812a`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_FIRST
- **Join Count**: 1
- **Next Tasks**: 0
- **Has Input Configuration**: Yes

### Resolved References
The following references were resolved from @@@placeholder@@@ format to actual values:

- `workflow_note_id_ref2` → `${UUID}`
- `transition_id_ref2` → `${UUID}`
- `action_ref2` → `hubspot.generic_request`
- `transition_id_ref3` → `${UUID}`
- `workflow_note_id_ref1` → `${UUID}`
- `action_ref1` → `core.noop`
- `workflow_task_id_ref1` → `d4d37cc8626245a7858ac18a50584b04`
- `workflow_task_id_ref2` → `0ee56517bff0464e8adf0df3d664812a`
- `transition_id_ref1` → `${UUID}`
