# Rewst Workflows Repository

This repository contains a comprehensive collection of Rewst workflows, organized by type for easy navigation and management.## Options Generator Workflows

## Repository StructureThis directory contains all workflows that are explicitly designated as Options Generators in Rewst.

### 📁 **options-generators/** (9 workflows)## What are Options Generators?

Contains workflows explicitly designated as Options Generators with `"type": "OPTION_GENERATOR"` in their workflow definition.

Options Generator workflows are a specific type of Rewst workflow that:

**Purpose**: Generate dynamic option lists for forms and other workflows

- `[OG]_HubSpot_CSM_Users/` - Customer Success Manager user picker- Have `"type": "OPTION_GENERATOR"` explicitly set in their workflow definition

- `[OG]_HubSpot_Companies_w_Discord_Active/` - Active Discord company selector - Are designed to generate dynamic option lists for forms and other workflows

- `[OG]_HubSpot_Discord_Users_by_Company/` - Discord users by company lookup- Typically return structured data with `label` and `id` fields for dropdown selections

- `Check_HubSpot_for_Role_Allowance_[OG]/` - Role permission checker- Are commonly used to populate form fields with data from external systems (HubSpot, Discord, etc.)

- `Cluck_U_Lookup_User_Credentials/` - Training program user lookup

- `Discord_User_Missing_Roles_[OG]/` - Missing role identifier## Workflows in this Directory

- `HubSpot_CSM_Lookup_[OG]/` - CSM lookup service

- `List_Unverified_Discord_Users_[OG]/` - Unverified user listerAll workflows in this directory have been verified to have the explicit `OPTION_GENERATOR` type designation in their bundle JSON, making them true Options Generators rather than just workflows that happen to generate options.

- `Verify_Users_Form_Submission/` - User verification form processor

## Organization

### 📁 **standard/** (31 workflows)

Contains standard operational workflows with `"type": "STANDARD"` designation.Each workflow is organized in its own subdirectory containing:

**Categories**:- The original `.bundle.json` file (unchanged for reliable Rewst importing)

- **Customer Support & Operations**: Discord role management, HubSpot updates, ticket handling- A `README.md` file with detailed workflow analysis and reference resolution

- **Training & Education**: Event management, attendance tracking, certification processes

- **Community Management**: User verification, role assignments, channel management## Usage

- **Data Integration**: HubSpot synchronization, property updates, contact management

- **Form Processing**: User submissions, role requests, customer outreachThese workflows are typically used as:

- **Beta & Feature Management**: Access control, feature rollouts, testing workflows

- Form field data sources via `enumSourceWorkflow` configuration

## Workflow Organization- Dynamic option providers for other workflows

- Data lookup and selection interfaces for customer support and operations teams

Each workflow is organized in its own subdirectory containing:

- **`.bundle.json`** - Original Rewst export bundle (unchanged for reliable importing)This directory contains organized Rewst workflow exports that have been automatically structured for better readability and understanding.

- **`README.md`** - Detailed analysis with resolved references and documentation

## Organization Structure

## Key Features

Each workflow has been placed in its own folder named after the workflow's actual name (as defined in the bundle's `nonfunctional_fields.name`). Each folder contains:

✅ **Preserved Import Compatibility** - All original `.bundle.json` files remain unchanged

✅ **Enhanced Documentation** - Each workflow has comprehensive README with resolved `@@@references@@@` - **README.md** - A comprehensive documentation file describing the workflow

✅ **Type-Based Organization** - Clear separation between Options Generators and Standard workflows - **[WorkflowName].bundle.json** - The original Rewst export bundle file with a cleaner filename

✅ **Security Normalized** - Sensitive data replaced with placeholder variables for safe sharing

✅ **Reference Resolution** - All `@@@workflow_task_id_ref1@@@` style references resolved to actual values ## Workflow Categories

## UsageBased on the workflow names, the workflows can be categorized as follows:

### Importing Workflows### HubSpot Integration Workflows

Use the original `.bundle.json` files for reliable Rewst importing:

````bash- **[SUB] HubSpot Task Idempotency - Ashe** - Prevents duplicate task creation in HubSpot

# Import an Options Generator- **[SUB] HubSpot: Update Training Property Fields** - Updates training-related fields

rewst import options-generators/[OG]_HubSpot_CSM_Users/[OG]_HubSpot_CSM_Users.bundle.json- **[2] Update HubSpot with Attendance (Group Training)** - Records group training attendance

- **[2] Update HubSpot with Attendance (Open Mic)** - Records open mic attendance

# Import a Standard workflow  - **[3] Update HubSpot with Backfill Attendance** - Backfills attendance data

rewst import standard/Add_App_Platform_Role_to_Discord_User/Add_App_Platform_Role_to_Discord_User.bundle.json- **[2] Update HS Contact with Training Completion** - Updates contact training completion

```- **[Backfill] Update HubSpot with Meetings** - Backfills meeting data

- **Update HubSpot w/ Feature Requests** - Updates feature request information

### Documentation- **Ad-hoc HubSpot Note** - Creates ad-hoc notes

Each workflow's `README.md` contains:- **[OG] HubSpot CSM Users** - Manages CSM user data

- Workflow purpose and functionality- **[OG] HubSpot Companies w/ Discord Active** - Manages companies with Discord activity

- Input parameters and expected outputs- **[OG] HubSpot Discord Users by Company** - Maps Discord users to companies

- Task-by-task analysis with resolved references- **HubSpot CSM Lookup [OG]** - Looks up CSM information

- Integration points and dependencies- **[HubSpot] Get Companies from Property - Ashe** - Retrieves companies by property

- Usage examples and configuration notes- **[HubSpot] Get Contacts from a List** - Gets contacts from specified lists

- **[Hubspot] Find Company Property Existence** - Checks for property existence

## Statistics- **[Parent MSP] HubSpot Parent Picker** - Manages parent MSP relationships

- **Add App Platform to HubSpot Alpha Access Company Feild** - Updates alpha access fields

- **Total workflows**: 40- **Prod - Update HubSpot with Escalated Ticket Details** - Updates escalated ticket information

- **Options Generators**: 9 (22.5%)

- **Standard workflows**: 31 (77.5%) ### Discord Integration Workflows

- **Reference resolution**: 1,000+ `@@@references@@@` resolved

- **Security normalization**: 100% sensitive data protected- **Add App Platform Role to Discord User** - Manages Discord user roles

- **[Discord v2] Role Addition & Removal** - Enhanced Discord role management

## Development History- **Discord User Missing Roles [OG]** - Identifies missing Discord roles

- **Discord VC** - Manages Discord voice channels

1. **Initial Organization** - Workflows renamed and organized by actual names- **List Unverified Discord Users [OG]** - Lists unverified users

2. **Security Normalization** - Sensitive data replaced with environment variables- **Check HubSpot for Role Allowance [OG]** - Checks role permissions

3. **Reference Resolution** - All placeholder references resolved to actual values

4. **Type Analysis** - Identified true Options Generators vs behavioral patterns### Community & Contact Management

5. **Final Organization** - Separated by explicit workflow type designation

- **[Community] Lookup Discord Contact from Company - Ashe** - Links Discord to companies

---- **[Community] Lookup Contact Properties in HubSpot - Ashe** - Retrieves contact data

- **[CX Ops] Collect List of Webinar Registrants [1]** - Manages webinar registrations

*Last updated: October 11, 2025*- **[Beta Process] Add Beta Access to Community** - Manages beta access

### Form Processing Workflows

- **Generic Form Submission** - Handles generic form submissions
- **Verify Users Form Submission** - Processes user verification forms
- **[Role Alignment] Form Submission** - Handles role alignment submissions
- **[Simple Form] Send Email w/ Subject & Body to Recipient** - Simple email sending

### Utility & Infrastructure Workflows

- **Trigger Criteria Finder** - Finds trigger criteria
- **What permissions does this endpoint need** - Analyzes endpoint permissions
- **Ad-hoc Role Lookup** - Performs role lookups
- **CSP/CPV Permission Checker** - Checks CSP/CPV permissions
- **[Slack] Post in Associated Thread (Sub-workflow) - Ashe** - Slack integration

### Training & Event Management

- **Training Program - Get Events** - Retrieves training events
- **Training Program | Lookup User Credentials** - Manages user credentials

## README File Structure

Each README.md file contains:

1. **Overview** - Workflow type, export date, bundle version, and original filename
2. **Parameters** - Input parameters with types and requirements
3. **Workflow Documentation** - Detailed notes and documentation from the original workflow
4. **Tasks** - Complete list of tasks with descriptions, actions, and configuration
5. **Output** - Workflow output structure
6. **Technical Details** - Timeout, task count, and other metadata

## Usage

To understand a specific workflow:

1. Navigate to the workflow's folder
2. Open the README.md file for comprehensive documentation
3. Refer to the bundle JSON file for complete technical details

## Notes

- Some workflows appear multiple times (different export timestamps) - these may be different versions or configurations
- Sub-workflows are marked with [SUB] prefix
- Original Generation (OG) workflows are marked with [OG] suffix
- Version 2 workflows are marked with [2] or [v2] prefixes
- All workflows maintain their original export bundle for complete technical reference

---

_This organization was automatically generated from Rewst workflow export bundles on October 11, 2025._
````
