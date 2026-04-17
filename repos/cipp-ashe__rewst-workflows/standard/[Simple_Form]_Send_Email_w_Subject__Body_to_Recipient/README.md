# [Simple Form] Send Email w/ Subject & Body to Recipient

## Overview
**Workflow Type:** STANDARD  
**Export Date:** 2025-10-11T16:08:50.072465+00:00  
**Bundle Version:** 2  
**Original Bundle File:** `workflow-01945230-4c3d-75e8-b862-d039486e9624_20251011_160850.bundle.json`  

## Parameters

- **body** (`string`) - Optional
- **subject** (`string`) - Optional
- **recipient** (`string`) - Optional

## Workflow Documentation

### Workflow Initialization & Setup

## Purpose
This block handles the initial setup and preparation phase of the email workflow execution.

## Tasks Included
- **START**: A no-operation action that serves as the workflow entry point and initialization trigger

## Functionality
The START task acts as the workflow's entry point, ensuring proper initialization of the execution context before proceeding to the main email delivery functionality. This task:
- Establishes the workflow execution context
- Validates that the workflow is properly triggered
- Prepares the execution environment for subsequent tasks
- Serves as a clean separation between workflow initiation and business logic

## Flow Connection
This initialization block connects directly to the Email Delivery block, passing control once the workflow environment is properly established.

## Key Inputs/Outputs
- **Inputs**: Workflow trigger context and any initial parameters
- **Outputs**: Initialized execution context ready for email processing

### Email Delivery & Communication

## Purpose
This block handles the core email delivery functionality, sending formatted emails to specified recipients using dynamic context data.

## Tasks Included
- **core_sendmail**: Sends an email with customizable recipient, subject, and body content

## Functionality
The email delivery block processes and sends emails with the following capabilities:
- **Dynamic Recipients**: Uses context variable `{{ CTX.recipient }}` for flexible recipient targeting
- **Custom Subject Lines**: Leverages `{{ CTX.subject }}` for personalized email subjects
- **Rich Content**: Supports markdown rendering in the email body via `{{ CTX.body }}`
- **Professional Sender**: Uses 'noreply' sender address for automated communications
- **Content Formatting**: Automatically renders markdown as HTML for enhanced email presentation

## Technical Configuration
- **Sender**: ${NOREPLY_EMAIL} (automated system emails)
- **Markdown Support**: Enabled for rich text formatting
- **Template Variables**: Fully dynamic using workflow context
- **Email Structure**: Title and message fields populated from context

## Flow Connection
This block receives control from the Workflow Initialization block and represents the final action in the workflow, completing the email delivery process.

## Key Inputs/Outputs
- **Inputs**: 
  - `CTX.recipient`: Target email address
  - `CTX.subject`: Email subject line
  - `CTX.body`: Email content (supports markdown)
- **Outputs**: Email delivery confirmation and status

## Tasks

This workflow contains 2 tasks:

### 1. START
**Description:** Action that does nothing
**Action:** ``core.noop``
**Next Tasks:** 1 transition(s) defined

### 2. core_sendmail
**Description:** This sends an email
**Action:** ``core.sendmail``
**Next Tasks:** 1 transition(s) defined

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
- **Parallel Branches**: 0
- **Join Points**: 0
- **Resolved References**: 8

### Task Flow

#### 1. START
- **Action**: `core.noop`
- **Task ID**: `9fffb6d4ca1a4d6db93f865db9a94d6b`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1

#### 2. core_sendmail
- **Action**: `core.sendmail`
- **Task ID**: `cbac2b92a08b4738897893ffa38ff369`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 0
- **Has Input Configuration**: Yes

### Resolved References
The following references were resolved from @@@placeholder@@@ format to actual values:

- `workflow_task_id_ref2` → `cbac2b92a08b4738897893ffa38ff369`
- `workflow_task_id_ref1` → `9fffb6d4ca1a4d6db93f865db9a94d6b`
- `workflow_note_id_ref2` → `${UUID}`
- `action_ref2` → `core.sendmail`
- `action_ref1` → `core.noop`
- `transition_id_ref1` → `${UUID}`
- `transition_id_ref2` → `${UUID}`
- `workflow_note_id_ref1` → `${UUID}`
