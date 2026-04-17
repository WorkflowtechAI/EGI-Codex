# Trigger Criteria Finder

## Overview
**Workflow Type:** STANDARD  
**Export Date:** 2025-10-11T16:17:52.482387+00:00  
**Bundle Version:** 2  
**Original Bundle File:** `workflow-018bf96a-8ccb-7012-902b-586d9710c47a_20251011_161752.bundle.json`  

## Tasks

This workflow contains 2 tasks:

### 1. BEGIN
**Description:** Action that does nothing
**Action:** ``core.noop``
**Next Tasks:** 1 transition(s) defined

### 2. core_http_request
**Description:** Perform an HTTP request
**Action:** ``core.http_request``
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

#### 1. BEGIN
- **Action**: `core.noop`
- **Task ID**: `430b8ceedf0747069e7579f33d5631d5`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1

#### 2. core_http_request
- **Action**: `core.http_request`
- **Task ID**: `e737c6b50fb248b5bec948f2a1821a50`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 0
- **Has Input Configuration**: Yes

### Resolved References
The following references were resolved from @@@placeholder@@@ format to actual values:

- `transition_id_ref2` → `${UUID}`
- `action_ref2` → `core.http_request`
- `transition_id_ref1` → `${UUID}`
- `workflow_task_id_ref1` → `430b8ceedf0747069e7579f33d5631d5`
- `workflow_task_id_ref2` → `e737c6b50fb248b5bec948f2a1821a50`
- `action_ref1` → `core.noop`
