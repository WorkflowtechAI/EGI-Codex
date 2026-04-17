# [HubSpot] Get Companies from Property - Ashe

## Overview
**Workflow Type:** STANDARD  
**Export Date:** 2025-10-11T16:15:22.882196+00:00  
**Bundle Version:** 2  
**Original Bundle File:** `workflow-0195f445-6b17-768f-af54-9e803e33a1c0_20251011_161522.bundle.json`  

## Parameters

- **domain** (`string`) - Optional
- **property_name** (`string`) - Optional
- **property_value** (`string`) - Optional
- **included_properties** (`string`) - Optional

## Workflow Documentation

### HubSpot Company Data Retrieval

## Purpose
This functional block handles the initial setup and execution of the HubSpot company search operation. It establishes the workflow foundation and performs the core data retrieval from HubSpot's CRM system.

## Tasks Included
- **BEGIN**: Workflow initialization task that starts the execution flow
- **hub_spot_search_companies**: Core search operation that queries HubSpot companies based on specified criteria

## Functionality Overview
The workflow begins with a standard initialization step, then immediately proceeds to search for companies in HubSpot using the provided search parameters. The search operation uses:
- Query parameter from workflow context (`CTX.domain`)
- Property-based filtering (`CTX.property_name` and `CTX.property_value`)
- Comprehensive property list for data retrieval (`CTX.included_properties`)

## Key Inputs
- `CTX.domain`: Search query parameter
- `CTX.property_name`: Property field to filter on
- `CTX.property_value`: Value to match for the specified property
- `CTX.included_properties`: List of company properties to include in results

## Key Outputs
- `company_list`: Retrieved company data from HubSpot containing all specified properties
- Search results are passed to the next functional block for processing

## Connection to Next Block
Upon successful completion, this block passes the retrieved company data to the "Data Processing & Output" block for formatting and final output preparation.

### Data Processing & Output Formatting

## Purpose
This functional block handles the transformation and formatting of the retrieved HubSpot company data into the final output format. It processes the raw company data and extracts only the properties portion for streamlined consumption.

## Tasks Included
- **format_output**: Transforms the company data by extracting properties from each company record using Jinja templating

## Functionality Overview
This block takes the complete company records retrieved from HubSpot and performs a focused data transformation. Using the Jinja template expression `{{ company.properties for company in CTX.company_list }}`, it extracts only the properties section from each company record, creating a cleaner, more focused dataset.

## Data Transformation Details
- **Input**: Full company objects from HubSpot search results (`CTX.company_list`)
- **Processing**: Jinja list comprehension to extract properties from each company
- **Output**: Streamlined list containing only the properties data for each company

## Key Inputs
- `CTX.company_list`: Complete company records from the previous HubSpot search operation

## Key Outputs
- Formatted company properties list ready for consumption by downstream systems or users
- Simplified data structure focusing on company properties without metadata overhead

## Connection from Previous Block
This block receives the complete company data from the "HubSpot Company Data Retrieval" block and transforms it into the final workflow output.

## Business Value
By extracting only the properties data, this block provides a clean, focused output that eliminates unnecessary HubSpot metadata and system fields, making the data more consumable for reporting, analysis, or integration purposes.

## Tasks

This workflow contains 3 tasks:

### 1. BEGIN
**Description:** Action that does nothing
**Action:** ``core.noop``
**Next Tasks:** 1 transition(s) defined

### 2. format_output
**Description:** Set a variable as an action
**Action:** ``transforms.set_variable``
**Next Tasks:** 1 transition(s) defined

### 3. hub_spot_search_companies
**Description:** search companies in hubspot
**Action:** ``hubspot.search_companies``
**Transition Mode:** FOLLOW_FIRST
**Next Tasks:** 1 transition(s) defined

## Output

This workflow produces the following output:

- **output:** `{{ CTX.output }}`

## Technical Details

- **Workflow Timeout:** 28800 seconds
- **Total Tasks:** 3
- **Documentation Sections:** 2

---

*This README was automatically generated from the Rewst workflow export bundle.*
## Workflow Execution Details

### Overview
- **Total Tasks**: 3
- **Workflow Type**: STANDARD
- **Parallel Branches**: 0
- **Join Points**: 0
- **Resolved References**: 11

### Task Flow

#### 1. BEGIN
- **Action**: `core.noop`
- **Task ID**: `0bc73a9595c840358b65858a19c1efa1`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1

#### 2. format_output
- **Action**: `transforms.set_variable`
- **Task ID**: `b2ec4e63d90e40cd9a337054132d24d0`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 0
- **Has Input Configuration**: Yes

#### 3. hub_spot_search_companies
- **Action**: `hubspot.search_companies`
- **Task ID**: `4f5a2ffa6552405ca26a5d11a530e9a5`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_FIRST
- **Join Count**: 0
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

### Resolved References
The following references were resolved from @@@placeholder@@@ format to actual values:

- `workflow_task_id_ref1` → `0bc73a9595c840358b65858a19c1efa1`
- `workflow_note_id_ref1` → `${UUID}`
- `workflow_task_id_ref3` → `4f5a2ffa6552405ca26a5d11a530e9a5`
- `transition_id_ref3` → `${UUID}`
- `workflow_note_id_ref2` → `${UUID}`
- `transition_id_ref1` → `${UUID}`
- `action_ref3` → `hubspot.search_companies`
- `workflow_task_id_ref2` → `b2ec4e63d90e40cd9a337054132d24d0`
- `action_ref1` → `core.noop`
- `transition_id_ref2` → `${UUID}`
- `action_ref2` → `transforms.set_variable`
