# [Hubspot] Find Company Property Existence

## Overview
**Workflow Type:** STANDARD  
**Export Date:** 2025-10-11T16:13:29.339913+00:00  
**Bundle Version:** 2  
**Original Bundle File:** `workflow-0195dfeb-78fb-7d78-bd08-0e57e2d62cdb_20251011_161329.bundle.json`  

## Parameters

- **lookup_property_name** (`string`) - Optional

## Workflow Documentation

### Input Processing & Lookup Preparation

This functional block handles the initial setup and preparation of the property lookup process.

**Purpose**: Prepares the input property name for matching by breaking it into searchable components and establishing lookup parameters.

**Key Tasks**:
- **BEGIN**: Workflow entry point that initiates two parallel processing paths
- **establish_lookup_parts**: Splits the input property name by underscores to create searchable parts for similarity matching

**Process Flow**:
The workflow starts with the BEGIN task which branches into two parallel paths - one for the main lookup process and another for a simplified matching approach. The establish_lookup_parts task prepares the lookup_property_name by splitting it into components that will be used later for finding similar field names if an exact match isn't found.

**Outputs**: 
- lookup_parts: Array of property name components for similarity searching
- Triggers the main HubSpot API data retrieval process

### HubSpot API Data Retrieval

This functional block handles the retrieval of company property data from the HubSpot API.

**Purpose**: Fetches all available company properties from HubSpot to enable property existence checking and matching.

**Key Tasks**:
- **list_hs_company_properties** (Path 1): Primary API call to retrieve HubSpot company properties with comprehensive error handling
- **list_hs_company_properties** (Path 2): Secondary API call in the parallel processing path for simplified matching

**API Details**:
- **Endpoint**: `/crm/v3/properties/company`
- **Method**: GET
- **Features**: Pagination enabled, follows redirects, requires 2xx status codes
- **Authentication**: Uses HubSpot integration credentials

**Error Handling**:
Both API calls include robust error handling that captures failure details and routes to error processing tasks when the API call fails.

**Success Conditions**:
- API returns 2xx status code
- Response contains iterable company properties data
- Properties list has at least one item

**Outputs**:
- hs_company_properties: Complete list of HubSpot company property objects
- Triggers property name extraction and matching logic on success
- Routes to error collection on API failure

### Property Matching & Validation Logic

This functional block performs the core property matching logic to determine if the requested property exists in HubSpot.

**Purpose**: Extracts property names from the API response and performs exact matching against the lookup property name.

**Key Tasks**:
- **get_property_names**: Extracts all property names from the HubSpot API response into a searchable list
- **match_details**: Performs case-insensitive exact matching with comprehensive error handling using try/catch blocks
- **found_property_output**: Formats successful match results for output

**Matching Logic**:
1. **Name Extraction**: Filters and extracts valid property names from the API response
2. **Exact Match Check**: Compares the lookup property name against all available property names
3. **Case Sensitivity**: Performs case-insensitive matching for better user experience
4. **Result Formatting**: Returns matching property details or prepares error information

**Decision Points**:
- **Match Found**: Routes to found_property_output when exact match is discovered
- **No Match**: Routes to similarity search when no exact match exists
- **Error Handling**: Captures and formats any lookup errors with detailed context

**Success Output**:
Returns the complete property object(s) that match the requested property name, enabling downstream processes to access full property metadata.

**Connection to Next Block**:
On successful match, flows to output generation. On no match, triggers the similarity search and error handling block.

### Similarity Search & Alternative Suggestions

This functional block handles scenarios where no exact property match is found by providing intelligent similarity suggestions.

**Purpose**: When exact matching fails, this block searches for similar property names to help users identify potential alternatives or correct typos.

**Key Tasks**:
- **get_similar_fields**: Implements intelligent similarity matching using property name components
- **no_match_output**: Formats the "no match found" response with helpful alternative suggestions

**Similarity Algorithm**:
The similarity search uses a sophisticated matching approach:
1. **Component Matching**: Compares lookup_parts (from the split property name) against existing property names
2. **Prefix/Suffix Matching**: Checks if property names start or end with any of the lookup components
3. **Partial Matching**: Identifies properties that contain similar word components

**Error Response Structure**:
```json
{
  "error": "NoMatchFound",
  "message": "No company property matched the provided field name. Check the similar fields below...",
  "lookup_value": "[original search term]",
  "similar_fields": ["array", "of", "suggested", "alternatives"]
}
```

**User Experience Benefits**:
- **Typo Tolerance**: Helps users identify properties when they have slight naming errors
- **Discovery**: Suggests related properties that might meet the user's needs
- **Guidance**: Provides clear feedback about what went wrong and potential solutions

**Connection to Output**:
Always routes to the workflow termination after providing similarity suggestions, ensuring users receive helpful feedback even when exact matches fail.

### Error Handling & Recovery

This functional block manages API failures and system errors, ensuring graceful error handling throughout the workflow.

**Purpose**: Captures and formats detailed error information when HubSpot API calls fail, providing comprehensive debugging context.

**Key Tasks**:
- **collect_error_details** (Path 1): Handles API failures from the primary property retrieval path
- **collect_error_details** (Path 2): Handles API failures from the secondary property retrieval path

**Error Capture Strategy**:
Both error handling tasks use identical logic to ensure consistent error reporting:
- **Error Context**: Captures the original API error details from CTX.error_capture
- **User Context**: Includes the original lookup_property_name for debugging
- **Structured Format**: Returns errors in a consistent JSON structure

**Error Response Structure**:
```json
{
  "error": "Error while calling API to access HubSpot Properties",
  "details": "[original API error details]",
  "lookup_value": "[property name being searched]"
}
```

**Failure Scenarios Handled**:
- **Authentication Failures**: Invalid or expired HubSpot credentials
- **Network Issues**: Connectivity problems or timeouts
- **API Rate Limits**: HubSpot API quota exceeded
- **Permission Errors**: Insufficient access to company properties
- **Service Outages**: HubSpot API temporarily unavailable

**Recovery Approach**:
- **No Retry Logic**: Errors are captured and reported rather than retried
- **Graceful Degradation**: Provides meaningful error messages instead of system failures
- **Debug Information**: Includes sufficient context for troubleshooting

**Connection to Output**:
All error paths route to workflow termination with structured error information, ensuring users receive clear feedback about what went wrong.

### Workflow Termination & Output

This functional block handles the final termination of the workflow and ensures proper output delivery.

**Purpose**: Provides clean workflow termination points for all possible execution paths, ensuring consistent completion regardless of success or failure scenarios.

**Key Tasks**:
- **END** (Path 1): Terminates the primary workflow path after property matching or similarity search completion
- **END** (Path 2): Terminates the secondary workflow path after simplified matching or error handling

**Termination Scenarios**:
1. **Successful Match**: Workflow ends after returning found property details
2. **No Match with Suggestions**: Workflow ends after providing similar property alternatives
3. **API Error**: Workflow ends after capturing and formatting error details
4. **Lookup Error**: Workflow ends after handling unexpected processing errors

**Output Delivery**:
By the time these END tasks execute, the workflow has already set the appropriate output variables:
- **Success Case**: Property match details in structured format
- **No Match Case**: Error message with similarity suggestions
- **Failure Case**: Detailed error information for debugging

**Workflow Architecture Benefits**:
- **Dual Path Design**: Two parallel processing paths provide redundancy and different matching approaches
- **Consistent Termination**: All execution paths converge to proper workflow completion
- **Clean State Management**: No hanging processes or incomplete executions

**Final State**:
The workflow completes with one of the following outcomes:
- Property found and returned with full metadata
- No exact match found but alternatives provided
- System error captured with debugging context

This design ensures users always receive a meaningful response, whether successful or not.

## Tasks

This workflow contains 13 tasks:

### 1. BEGIN
**Description:** Action that does nothing
**Action:** ``core.noop``
**Next Tasks:** 2 transition(s) defined

### 2. END
**Description:** Action that does nothing
**Action:** ``core.noop``
**Join:** 1 (waits for 1 incoming transitions)
**Next Tasks:** 1 transition(s) defined

### 3. END
**Description:** Action that does nothing
**Action:** ``core.noop``
**Join:** 1 (waits for 1 incoming transitions)
**Next Tasks:** 1 transition(s) defined

### 4. collect_error_details
**Description:** Set a variable as an action
**Action:** ``transforms.set_variable``
**Next Tasks:** 1 transition(s) defined

### 5. collect_error_details
**Description:** Set a variable as an action
**Action:** ``transforms.set_variable``
**Next Tasks:** 1 transition(s) defined

### 6. establish_lookup_parts
**Description:** Set a variable as an action
**Action:** ``transforms.set_variable``
**Next Tasks:** 1 transition(s) defined

### 7. found_property_output
**Description:** Set a variable as an action
**Action:** ``transforms.set_variable``
**Next Tasks:** 1 transition(s) defined

### 8. get_property_names
**Description:** Set a variable as an action
**Action:** ``transforms.set_variable``
**Transition Mode:** FOLLOW_FIRST
**Next Tasks:** 2 transition(s) defined

### 9. get_similar_fields
**Description:** Set a variable as an action
**Action:** ``transforms.set_variable``
**Next Tasks:** 1 transition(s) defined

### 10. list_hs_company_properties
**Description:** Look for Custom Field in HubSpot.
**Action:** ``hubspot.generic_request``
**Join:** 1 (waits for 1 incoming transitions)
**Next Tasks:** 2 transition(s) defined

### 11. list_hs_company_properties
**Description:** Look for Custom Field in HubSpot.
**Action:** ``hubspot.generic_request``
**Transition Mode:** FOLLOW_FIRST
**Join:** 1 (waits for 1 incoming transitions)
**Next Tasks:** 2 transition(s) defined

### 12. match_details
**Description:** Set a variable as an action
**Action:** ``transforms.set_variable``
**Next Tasks:** 1 transition(s) defined

### 13. no_match_output
**Description:** Set a variable as an action
**Action:** ``transforms.set_variable``
**Next Tasks:** 1 transition(s) defined

## Output

This workflow produces the following output:

- **result:** `{{ CTX.output }}`

## Technical Details

- **Workflow Timeout:** 28800 seconds
- **Total Tasks:** 13
- **Documentation Sections:** 6

---

*This README was automatically generated from the Rewst workflow export bundle.*
## Workflow Execution Details

### Overview
- **Total Tasks**: 13
- **Workflow Type**: STANDARD
- **Parallel Branches**: 4
- **Join Points**: 4
- **Resolved References**: 39

### Task Flow

#### 1. BEGIN
- **Action**: `core.noop`
- **Task ID**: `eb2980dec5d34269a88a4088af823019`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 2

#### 2. END
- **Action**: `core.noop`
- **Task ID**: `061bf978c6a549be9653c7ea053fae10`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 1
- **Next Tasks**: 0

#### 3. END
- **Action**: `core.noop`
- **Task ID**: `04b74d05941c4481aaa294da8ed2002d`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 1
- **Next Tasks**: 0

#### 4. collect_error_details
- **Action**: `transforms.set_variable`
- **Task ID**: `c0232091861649a58c145d268ccc2a36`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

#### 5. collect_error_details
- **Action**: `transforms.set_variable`
- **Task ID**: `a67e16ccdb174032be54a889c7159478`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

#### 6. establish_lookup_parts
- **Action**: `transforms.set_variable`
- **Task ID**: `1065cabe16c54c378305b5a2948bed0c`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

#### 7. found_property_output
- **Action**: `transforms.set_variable`
- **Task ID**: `48179c89fb72453e98e3a17590c8e395`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

#### 8. get_property_names
- **Action**: `transforms.set_variable`
- **Task ID**: `dda44dd940124ff4a9f38994634a0826`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_FIRST
- **Join Count**: 0
- **Next Tasks**: 2
- **Has Input Configuration**: Yes

#### 9. get_similar_fields
- **Action**: `transforms.set_variable`
- **Task ID**: `b6c8978551674b45abbe93cbbd6520ac`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

#### 10. list_hs_company_properties
- **Action**: `hubspot.generic_request`
- **Task ID**: `eee654ceb6ff406fb37b87686257d6b3`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 1
- **Next Tasks**: 2
- **Has Input Configuration**: Yes

#### 11. list_hs_company_properties
- **Action**: `hubspot.generic_request`
- **Task ID**: `a689a2e8a068449cae89c1842c9020e9`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_FIRST
- **Join Count**: 1
- **Next Tasks**: 2
- **Has Input Configuration**: Yes

#### 12. match_details
- **Action**: `transforms.set_variable`
- **Task ID**: `0d0cc16a154f4f0183bc87d553150ca5`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

#### 13. no_match_output
- **Action**: `transforms.set_variable`
- **Task ID**: `b98fb0e571754a94b676d93c7765f1e4`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

### Resolved References
The following references were resolved from @@@placeholder@@@ format to actual values:

- `workflow_task_id_ref11` → `a689a2e8a068449cae89c1842c9020e9`
- `workflow_note_id_ref1` → `${UUID}`
- `workflow_task_id_ref2` → `061bf978c6a549be9653c7ea053fae10`
- `transition_id_ref8` → `${UUID}`
- `transition_id_ref17` → `${UUID}`
- `workflow_task_id_ref4` → `c0232091861649a58c145d268ccc2a36`
- `workflow_note_id_ref6` → `${UUID}`
- `workflow_task_id_ref7` → `48179c89fb72453e98e3a17590c8e395`
- `action_ref3` → `hubspot.generic_request`
- `transition_id_ref7` → `${UUID}`
- `workflow_task_id_ref12` → `0d0cc16a154f4f0183bc87d553150ca5`
- `transition_id_ref5` → `${UUID}`
- `transition_id_ref3` → `${UUID}`
- `transition_id_ref13` → `${UUID}`
- `workflow_task_id_ref5` → `a67e16ccdb174032be54a889c7159478`
- `transition_id_ref16` → `${UUID}`
- `action_ref2` → `transforms.set_variable`
- `transition_id_ref9` → `${UUID}`
- `transition_id_ref11` → `${UUID}`
- `workflow_task_id_ref9` → `b6c8978551674b45abbe93cbbd6520ac`
- `transition_id_ref14` → `${UUID}`
- `transition_id_ref10` → `${UUID}`
- `transition_id_ref1` → `${UUID}`
- `workflow_task_id_ref8` → `dda44dd940124ff4a9f38994634a0826`
- `transition_id_ref12` → `${UUID}`
- `workflow_note_id_ref4` → `${UUID}`
- `workflow_task_id_ref10` → `eee654ceb6ff406fb37b87686257d6b3`
- `transition_id_ref4` → `${UUID}`
- `workflow_task_id_ref6` → `1065cabe16c54c378305b5a2948bed0c`
- `transition_id_ref2` → `${UUID}`
- `workflow_task_id_ref13` → `b98fb0e571754a94b676d93c7765f1e4`
- `workflow_note_id_ref2` → `${UUID}`
- `transition_id_ref15` → `${UUID}`
- `transition_id_ref6` → `${UUID}`
- `workflow_note_id_ref5` → `${UUID}`
- `workflow_task_id_ref1` → `eb2980dec5d34269a88a4088af823019`
- `action_ref1` → `core.noop`
- `workflow_task_id_ref3` → `04b74d05941c4481aaa294da8ed2002d`
- `workflow_note_id_ref3` → `${UUID}`
