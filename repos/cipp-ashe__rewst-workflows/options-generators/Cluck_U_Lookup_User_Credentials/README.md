# Training Program | Lookup User Credentials

## Overview
**Workflow Type:** OPTION_GENERATOR  
**Export Date:** 2025-10-11T16:13:53.691878+00:00  
**Bundle Version:** 2  
**Original Bundle File:** `workflow-0190e0a6-8d38-7dc8-befe-e504ba1d04c0_20251011_161353.bundle.json`  

## Parameters

- **discord_user** (`string`) - Optional

## Workflow Documentation

### Input Processing & User Identification

## Purpose
This functional block handles the initial processing of Discord user input and identifies the corresponding user in HubSpot's contact database.

## Tasks Included
- **core_noop**: Processes the raw Discord user input and splits it into username and ID components
- **hub_spot_search_contacts**: Searches HubSpot contacts using the Discord username to find matching user records

## Key Functionality
- Parses Discord user input format (username,id) into separate components
- Queries HubSpot CRM to locate the user's contact record
- Validates that the user has certified status before proceeding
- Extracts the user's email address for subsequent lookups

## Data Flow
- **Input**: Raw Discord user string (format: "username,id")
- **Processing**: Splits input and searches HubSpot contacts by Discord username
- **Output**: User email address and certified status validation
- **Next Step**: Passes email to recipient lookup process

## Conditional Logic
The workflow only proceeds to credential lookup if the user is found in HubSpot AND has a certified_level property, ensuring only authorized users can access credentials.

### Recipient Lookup & Validation

## Purpose
This functional block validates that the identified user exists as a recipient in the credential management system and retrieves their unique recipient ID.

## Tasks Included
- **get_recipient**: Searches the credential system's recipient database using the user's email address

## Key Functionality
- Performs recipient search in the credential management API using email as the search term
- Validates that the user exists in the recipient database
- Extracts the recipient ID required for credential lookups
- Implements conditional branching based on search results

## Data Flow
- **Input**: User email address from HubSpot contact lookup
- **Processing**: POST request to /recipient/search endpoint with email search term
- **Output**: Recipient ID if found, or workflow termination if not found
- **Next Step**: Passes recipient ID to credential retrieval process

## API Integration Details
- **Endpoint**: POST /recipient/search
- **Headers**: api-version: "3.1"
- **Payload**: JSON with startIndex, length, and searchTerm (email)
- **Response**: Returns recipient records with unique ID

## Conditional Logic
The workflow only continues if `CTX.get_recipient.data.total > 0`, meaning at least one recipient record was found. If no recipient is found, the workflow terminates gracefully.

### Credential Retrieval & Data Transformation

## Purpose
This functional block retrieves the user's credentials from the credential management system and transforms the data into a structured format suitable for presentation or further processing.

## Tasks Included
- **get_credential**: Fetches credential records associated with the validated recipient ID
- **transforms_set_options**: Transforms credential data into a structured format with ID/label pairs

## Key Functionality
- Queries the credential management API using the recipient ID
- Retrieves all credentials associated with the user
- Transforms raw credential data into a user-friendly format
- Creates structured options with certificate links and descriptive labels

## Data Flow
- **Input**: Recipient ID from the recipient lookup process
- **Processing**: 
  1. POST request to /credential/search with recipient ID
  2. Data transformation using Jinja2 templating
- **Output**: Formatted credential options with ID/label structure
- **Final Result**: Array of credential objects ready for user selection

## API Integration Details
- **Endpoint**: POST /credential/search
- **Headers**: api-version: "3.1"
- **Payload**: JSON with startIndex, length, and recipientIds array
- **Response**: Returns credential records with certificate details

## Data Transformation Logic
The transform task creates a structured format:
```
{
  "id": certificateImageLink,
  "label": "campaignTitle: certificateNO - createDate (name)"
}
```

This provides users with meaningful credential descriptions including:
- Campaign title and certificate number
- Creation date (formatted as YYYY-MM-DD)
- Recipient name

## Conditional Logic
The workflow only proceeds with transformation if `CTX.get_credential.data.total > 0`, ensuring credentials exist before attempting to format them.

## Tasks

This workflow contains 5 tasks:

### 1. core_noop
**Description:** Action that does nothing
**Action:** ``core.noop``
**Timeout:** None seconds
**Next Tasks:** 1 transition(s) defined

### 2. get_credential
**Description:** Generic action for making authenticated requests against a custom REST API
**Action:** ``custom.generic_request``
**Transition Mode:** FOLLOW_FIRST
**Next Tasks:** 2 transition(s) defined

### 3. get_recipient
**Description:** Generic action for making authenticated requests against a custom REST API
**Action:** ``custom.generic_request``
**Transition Mode:** FOLLOW_FIRST
**Next Tasks:** 2 transition(s) defined

### 4. hub_spot_search_contacts
**Description:** search contacts in hubspot
**Action:** ``hubspot.search_contacts``
**Transition Mode:** FOLLOW_FIRST
**Next Tasks:** 2 transition(s) defined

### 5. transforms_set_options
**Description:** Set a variable as an action
**Action:** ``transforms.set_variable``
**Next Tasks:** 1 transition(s) defined

## Output

This workflow produces the following output:

- **options:** `{{ CTX.options }}`

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

#### 1. core_noop
- **Action**: `core.noop`
- **Task ID**: `9f2d7222e00045629a645b0899965e36`
- **Timeout**: Nones
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: None
- **Next Tasks**: 1

#### 2. get_credential
- **Action**: `custom.generic_request`
- **Task ID**: `7282340a27374e67b5ac0d2eb6f42cdc`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_FIRST
- **Join Count**: 0
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

#### 3. get_recipient
- **Action**: `custom.generic_request`
- **Task ID**: `8d1b7888fd4c4474b8edda0258943917`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_FIRST
- **Join Count**: 0
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

#### 4. hub_spot_search_contacts
- **Action**: `hubspot.search_contacts`
- **Task ID**: `18617249275840c9b827f1ae60a0824c`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_FIRST
- **Join Count**: 0
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

#### 5. transforms_set_options
- **Action**: `transforms.set_variable`
- **Task ID**: `a767854a732743ecb86e183ee88d1a4a`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 0
- **Has Input Configuration**: Yes

### Resolved References
The following references were resolved from @@@placeholder@@@ format to actual values:

- `workflow_note_id_ref3` → `${UUID}`
- `workflow_task_id_ref4` → `18617249275840c9b827f1ae60a0824c`
- `action_ref2` → `custom.generic_request`
- `workflow_task_id_ref1` → `9f2d7222e00045629a645b0899965e36`
- `workflow_task_id_ref5` → `a767854a732743ecb86e183ee88d1a4a`
- `transition_id_ref6` → `${UUID}`
- `workflow_note_id_ref2` → `${UUID}`
- `transition_id_ref5` → `${UUID}`
- `action_ref3` → `hubspot.search_contacts`
- `transition_id_ref1` → `${UUID}`
- `workflow_note_id_ref1` → `${UUID}`
- `action_ref4` → `transforms.set_variable`
- `transition_id_ref3` → `${UUID}`
- `transition_id_ref4` → `${UUID}`
- `transition_id_ref2` → `${UUID}`
- `workflow_task_id_ref3` → `8d1b7888fd4c4474b8edda0258943917`
- `transition_id_ref8` → `${UUID}`
- `pack_ref1` → `custom`
- `action_ref1` → `core.noop`
- `transition_id_ref7` → `${UUID}`
- `workflow_task_id_ref2` → `7282340a27374e67b5ac0d2eb6f42cdc`
