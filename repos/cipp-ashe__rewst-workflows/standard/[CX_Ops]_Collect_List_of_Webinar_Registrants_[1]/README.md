# [CX Ops] Collect List of Webinar Registrants [1]

## Overview
**Workflow Type:** STANDARD  
**Export Date:** 2025-10-11T16:12:27.521590+00:00  
**Bundle Version:** 2  
**Original Bundle File:** `workflow-018d653e-2a7e-7f1a-b2f2-f7e5720ad209_20251011_161227.bundle.json`  

## Workflow Documentation

### Data Initialization & Setup

## Purpose
This block initializes the workflow with sample webinar registrant data, setting up the foundation for the entire customer outreach process.

## Tasks Included
- **core_noop**: Provides the initial CSV dataset containing webinar registrant information

## Functionality
- Publishes a comprehensive CSV dataset with customer details (first_name, last_name, email, company_name)
- Contains 34 sample registrant records from various IT consulting companies
- Serves as the data source that feeds into the subsequent processing pipeline

## Key Outputs
- `csv_data`: Raw CSV string containing all registrant information with headers
- Data includes companies like ITECH Solutions, Afineol IT Consulting, VersaTrust, and others

## Workflow Connection
This block provides the foundational data that flows into the Data Processing block, where the CSV will be parsed and structured for individual customer processing.

### Data Processing & Transformation

## Purpose
This block transforms the raw CSV data into structured JSON format, making individual customer records accessible for processing and enabling iteration over each registrant.

## Tasks Included
- **transforms_parse_csv**: Converts CSV string data into structured JSON array with individual customer objects

## Functionality
- Parses the incoming CSV data using comma delimiter
- Transforms the first line into JSON keys (first_name, last_name, email, company_name)
- Converts each subsequent line into individual customer record objects
- Prepares data structure for individual customer processing

## Key Processing Details
- **Input**: Raw CSV string from the Data Initialization block (`{{ CTX.csv_data }}`)
- **Delimiter**: Comma separator for proper field parsing
- **Output**: Structured JSON array where each element represents one webinar registrant

## Data Transformation
Converts from:
```
first_name,last_name,email,company_name
Alden,Wilson,${USER_EMAIL},ITECH Solutions
```

To structured JSON objects ready for individual processing.

## Workflow Connection
This block receives raw CSV data from the Data Initialization block and outputs structured customer records that feed into the Customer Engagement block for individual webinar invitations.

### Customer Engagement & Webinar Outreach

## Purpose
This block executes the core business objective by sending personalized webinar invitations to each customer in the registrant list, completing the customer outreach automation.

## Tasks Included
- **workflows_invite_customer_to_webinar**: Processes individual customer records and sends webinar invitations

## Functionality
- Iterates through each parsed customer record from the previous block
- Extracts individual customer details (first_name, last_name, email, company_name)
- Executes personalized webinar invitation workflow for each registrant
- Handles the complete customer engagement process

## Customer Data Processing
For each customer record, the task processes:
- **Email**: Customer's email address for invitation delivery
- **First Name**: For personalized greeting
- **Last Name**: For complete customer identification  
- **Company Name**: For business context and personalization

## Iteration Logic
Uses `{{ item() }}` context to access individual customer records:
- `{{ item().email }}` - Customer email
- `{{ item().first_name }}` - Customer first name
- `{{ item().last_name }}` - Customer last name
- `{{ item().company_name }}` - Customer company

## Business Impact
This block delivers the primary value of the workflow by:
- Automating personalized customer outreach
- Ensuring all 34 registrants receive webinar invitations
- Maintaining professional, personalized communication with business context

## Workflow Connection
This is the final execution block that receives structured customer data from the Data Processing block and completes the end-to-end customer engagement process.

## Tasks

This workflow contains 3 tasks:

### 1. core_noop
**Description:** Action that does nothing
**Action:** ``core.noop``
**Timeout:** None seconds
**Next Tasks:** 1 transition(s) defined

### 2. transforms_parse_csv
**Description:** Parse CSV Data into JSON. The first line will be keys, and subsequent lines will be values
**Action:** ``transforms.parse_csv``
**Next Tasks:** 1 transition(s) defined

### 3. workflows_invite_customer_to_webinar
**Action:** `Unknown`
**Loop/Iteration:** Yes
**Next Tasks:** 1 transition(s) defined

## Technical Details

- **Workflow Timeout:** 28800 seconds
- **Total Tasks:** 3
- **Documentation Sections:** 3

---

*This README was automatically generated from the Rewst workflow export bundle.*
## Workflow Execution Details

### Overview
- **Total Tasks**: 3
- **Workflow Type**: STANDARD
- **Parallel Branches**: 0
- **Join Points**: 0
- **Resolved References**: 12

### Task Flow

#### 1. core_noop
- **Action**: `core.noop`
- **Task ID**: `d3d847a3bb474fc78b91ba10b76312f2`
- **Timeout**: Nones
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: None
- **Next Tasks**: 1

#### 2. transforms_parse_csv
- **Action**: `transforms.parse_csv`
- **Task ID**: `65917810e70c42c2a789933fd4b04b37`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

#### 3. workflows_invite_customer_to_webinar
- **Action**: `Unknown Action`
- **Task ID**: `f277415b01764af7b881004537f83f0b`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 0
- **Has Input Configuration**: Yes
- **Loop/Iterator**: Yes

### Resolved References
The following references were resolved from @@@placeholder@@@ format to actual values:

- `workflow_task_id_ref2` → `65917810e70c42c2a789933fd4b04b37`
- `transition_id_ref2` → `${UUID}`
- `workflow_note_id_ref2` → `${UUID}`
- `workflow_ref1` → `[SUBWORKFLOW: Invite Customer to Webinar]`
- `workflow_task_id_ref1` → `d3d847a3bb474fc78b91ba10b76312f2`
- `workflow_task_id_ref3` → `f277415b01764af7b881004537f83f0b`
- `action_ref1` → `core.noop`
- `workflow_note_id_ref3` → `${UUID}`
- `action_ref2` → `transforms.parse_csv`
- `workflow_note_id_ref1` → `${UUID}`
- `transition_id_ref1` → `${UUID}`
- `transition_id_ref3` → `${UUID}`
