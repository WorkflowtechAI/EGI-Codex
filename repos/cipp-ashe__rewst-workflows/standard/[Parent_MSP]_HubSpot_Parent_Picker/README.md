# [Parent MSP] HubSpot Parent Picker

## Overview
**Workflow Type:** STANDARD  
**Export Date:** 2025-10-11T16:11:33.638662+00:00  
**Bundle Version:** 2  
**Original Bundle File:** `workflow-0194b567-25e9-79d2-ae37-5f3c39c32ce0_20251011_161133.bundle.json`  

## Parameters

- **email** (`string`) - Optional

## Workflow Documentation

### Contact Discovery & Validation

## Purpose
This block handles the initial contact lookup and validation process in HubSpot, determining whether a contact exists and routing the workflow accordingly.

## Tasks Included
- **BEGIN**: Workflow initialization and entry point
- **hub_spot_search_contacts**: Searches HubSpot for existing contacts using the provided email address

## Process Flow
1. The workflow starts with the BEGIN task as the entry point
2. Immediately searches HubSpot contacts using the input email address
3. Retrieves key contact properties: id, email, firstname, lastname, associatedcompanyid, product_user_id, discord_username
4. Creates two conditional paths:
   - **Contact Found**: Routes to contact data processing if contact exists
   - **Contact Not Found**: Routes to domain extraction for company lookup

## Key Inputs
- `CTX.email`: The email address to search for in HubSpot contacts

## Key Outputs
- `CTX.get_hs_contact`: Array of matching contact records with their properties
- Conditional routing based on contact existence

## Connection to Next Block
- If contact found: Proceeds to "Data Processing & Preparation" block
- If contact not found: Proceeds to "Company Research & Analysis" block via domain extraction

### Data Processing & Preparation

## Purpose
This block handles data transformation and preparation, extracting domain information from email addresses and formatting contact data for downstream processing.

## Tasks Included
- **set_domain_from_email**: Extracts the domain portion from an email address for company lookup
- **set_contact_options**: Formats and structures contact data with additional context information

## Process Flow
1. **Domain Extraction**: When no contact is found, extracts domain from the email address using `(CTX.email.split('@'))|last`
2. **Contact Data Formatting**: When contact is found, creates a structured data object containing:
   - Core contact properties (email, firstname, lastname, discord_username, associatedcompanyid, product_user_id)
   - Additional context (submitter information with user.username and organization.name)

## Key Data Transformations
- **Domain Extraction**: Converts "${USER_EMAIL}" → "company.com"
- **Contact Structuring**: Merges contact properties with submitter metadata for comprehensive data package

## Key Inputs
- `CTX.email`: Source email for domain extraction
- `CTX.get_hs_contact[0].properties`: Contact properties from HubSpot search
- `CTX.user.username`: Current user information
- `CTX.organization.name`: Organization context

## Key Outputs
- `CTX.domain`: Extracted domain for company search
- `CTX.contact_options`: Formatted contact data structure with submitter context

## Connection to Next Block
- Domain extraction flows to "Company Research & Analysis" block
- Contact formatting provides structured data for workflow completion

### Company Research & Analysis

## Purpose
This block performs company lookup and analysis in HubSpot to identify parent-child company relationships and organizational structure, which is critical for the Parent Picker functionality.

## Tasks Included
- **search_hs_company**: Searches HubSpot companies using the extracted domain to find matching organizations

## Process Flow
1. Uses the domain extracted from the email address to search HubSpot companies
2. Retrieves comprehensive company properties focused on organizational hierarchy:
   - **Identity**: id, type, createdate, closedate
   - **Hierarchy**: hs_parent_company_id, hs_num_child_companies
   - **Structure**: Determines parent-child relationships for MSP scenarios

## Key Company Properties Retrieved
- `id`: Unique company identifier
- `hs_parent_company_id`: Critical for identifying parent company relationships
- `type`: Company classification
- `hs_num_child_companies`: Number of subsidiary companies
- `createdate`, `closedate`: Company lifecycle information

## Search Strategy
- Domain-based search using `CTX.domain` to find companies associated with the email domain
- Comprehensive property retrieval to support parent company selection logic

## Key Inputs
- `CTX.domain`: Email domain extracted from previous block

## Key Outputs
- `CTX.search_hs_company`: Array of matching companies with hierarchy information
- Company relationship data for parent picker functionality

## Workflow Context
This is the final processing block in the workflow, providing the company data needed for MSP (Managed Service Provider) parent company selection. The retrieved hierarchy information enables proper organizational structure mapping.

## Tasks

This workflow contains 5 tasks:

### 1. BEGIN
**Description:** Action that does nothing
**Action:** ``core.noop``
**Next Tasks:** 1 transition(s) defined

### 2. hub_spot_search_contacts
**Description:** search contacts in hubspot
**Action:** ``hubspot.search_contacts``
**Transition Mode:** FOLLOW_FIRST
**Next Tasks:** 2 transition(s) defined

### 3. search_hs_company
**Description:** search companies in hubspot
**Action:** ``hubspot.search_companies``
**Next Tasks:** 1 transition(s) defined

### 4. set_contact_options
**Description:** Set a variable as an action
**Action:** ``transforms.set_variable``
**Next Tasks:** 1 transition(s) defined

### 5. set_domain_from_email
**Description:** Set a variable as an action
**Action:** ``transforms.set_variable``
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
- **Resolved References**: 18

### Task Flow

#### 1. BEGIN
- **Action**: `core.noop`
- **Task ID**: `319160f7b24b4e1e823d7a164558e8a8`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1

#### 2. hub_spot_search_contacts
- **Action**: `hubspot.search_contacts`
- **Task ID**: `996265a010f040d9aad14fed9365daa4`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_FIRST
- **Join Count**: 0
- **Next Tasks**: 2
- **Has Input Configuration**: Yes

#### 3. search_hs_company
- **Action**: `hubspot.search_companies`
- **Task ID**: `147b17d3d21d493298f5c713bdf61754`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 0
- **Has Input Configuration**: Yes

#### 4. set_contact_options
- **Action**: `transforms.set_variable`
- **Task ID**: `b75c470d96134a80bff10dff8450b2c2`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 0
- **Has Input Configuration**: Yes

#### 5. set_domain_from_email
- **Action**: `transforms.set_variable`
- **Task ID**: `1d9519eef02040b4955022ba6f3d3bb8`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

### Resolved References
The following references were resolved from @@@placeholder@@@ format to actual values:

- `transition_id_ref4` → `${UUID}`
- `workflow_task_id_ref3` → `147b17d3d21d493298f5c713bdf61754`
- `action_ref4` → `transforms.set_variable`
- `transition_id_ref5` → `${UUID}`
- `workflow_task_id_ref1` → `319160f7b24b4e1e823d7a164558e8a8`
- `transition_id_ref1` → `${UUID}`
- `workflow_note_id_ref3` → `${UUID}`
- `workflow_task_id_ref2` → `996265a010f040d9aad14fed9365daa4`
- `workflow_task_id_ref4` → `b75c470d96134a80bff10dff8450b2c2`
- `action_ref1` → `core.noop`
- `action_ref3` → `hubspot.search_companies`
- `workflow_task_id_ref5` → `1d9519eef02040b4955022ba6f3d3bb8`
- `workflow_note_id_ref1` → `${UUID}`
- `transition_id_ref3` → `${UUID}`
- `workflow_note_id_ref2` → `${UUID}`
- `transition_id_ref2` → `${UUID}`
- `transition_id_ref6` → `${UUID}`
- `action_ref2` → `hubspot.search_contacts`
