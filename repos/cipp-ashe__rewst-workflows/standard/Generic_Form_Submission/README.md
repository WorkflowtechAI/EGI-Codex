# Generic Form Submission

## Overview
**Workflow Type:** STANDARD  
**Export Date:** 2025-10-11T16:17:44.464048+00:00  
**Bundle Version:** 2  
**Original Bundle File:** `workflow-018d22eb-477e-77c4-a86d-d6cc50d6c47f_20251011_161744.bundle.json`  

## Tasks

This workflow contains 2 tasks:

### 1. FORM_SUBMISSION
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

---

*This README was automatically generated from the Rewst workflow export bundle.*
## Workflow Execution Details

### Overview
- **Total Tasks**: 2
- **Workflow Type**: STANDARD
- **Parallel Branches**: 0
- **Join Points**: 0
- **Resolved References**: 6

### Task Flow

#### 1. FORM_SUBMISSION
- **Action**: `core.noop`
- **Task ID**: `eaef3ebdaa884cc8bde661c639d0ca79`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1

#### 2. core_sendmail
- **Action**: `core.sendmail`
- **Task ID**: `4a85c02fc950486e828838f67c08649c`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 0
- **Has Input Configuration**: Yes

### Resolved References
The following references were resolved from @@@placeholder@@@ format to actual values:

- `workflow_task_id_ref2` → `4a85c02fc950486e828838f67c08649c`
- `transition_id_ref1` → `${UUID}`
- `action_ref2` → `core.sendmail`
- `action_ref1` → `core.noop`
- `transition_id_ref2` → `${UUID}`
- `workflow_task_id_ref1` → `eaef3ebdaa884cc8bde661c639d0ca79`
