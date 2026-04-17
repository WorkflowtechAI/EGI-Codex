# Ad-hoc Role Lookup

## Overview
**Workflow Type:** STANDARD  
**Export Date:** 2025-10-11T16:17:27.177339+00:00  
**Bundle Version:** 2  
**Original Bundle File:** `workflow-018ee20e-0c4d-7ac4-8ac6-bbd532c5d8e4_20251011_161727.bundle.json`  

## Tasks

This workflow contains 3 tasks:

### 1. BEGIN
**Description:** Action that does nothing
**Action:** ``core.noop``
**Transition Mode:** FOLLOW_FIRST
**Next Tasks:** 2 transition(s) defined

### 2. END
**Description:** Action that does nothing
**Action:** ``core.noop``
**Join:** 1 (waits for 1 incoming transitions)
**Next Tasks:** 1 transition(s) defined

### 3. discord_list_guild_channels
**Description:** List all channels in a guild (server)
**Action:** ``discord.list_guild_channels``
**Transition Mode:** FOLLOW_FIRST
**Join:** 1 (waits for 1 incoming transitions)
**Next Tasks:** 2 transition(s) defined

## Technical Details

- **Workflow Timeout:** 28800 seconds
- **Total Tasks:** 3

---

*This README was automatically generated from the Rewst workflow export bundle.*
## Workflow Execution Details

### Overview
- **Total Tasks**: 3
- **Workflow Type**: STANDARD
- **Parallel Branches**: 2
- **Join Points**: 2
- **Resolved References**: 10

### Task Flow

#### 1. BEGIN
- **Action**: `core.noop`
- **Task ID**: `4b0280810907413d8dde677f982f4542`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_FIRST
- **Join Count**: 0
- **Next Tasks**: 2

#### 2. END
- **Action**: `core.noop`
- **Task ID**: `3d7d72750e1d479ea82860d02ad82e09`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 1
- **Next Tasks**: 0

#### 3. discord_list_guild_channels
- **Action**: `discord.list_guild_channels`
- **Task ID**: `9d13699f2f4a48659e14feee26f77261`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_FIRST
- **Join Count**: 1
- **Next Tasks**: 2
- **Has Input Configuration**: Yes

### Resolved References
The following references were resolved from @@@placeholder@@@ format to actual values:

- `transition_id_ref3` → `${UUID}`
- `workflow_task_id_ref3` → `9d13699f2f4a48659e14feee26f77261`
- `transition_id_ref4` → `${UUID}`
- `transition_id_ref5` → `${UUID}`
- `action_ref1` → `core.noop`
- `transition_id_ref1` → `${UUID}`
- `workflow_task_id_ref2` → `3d7d72750e1d479ea82860d02ad82e09`
- `workflow_task_id_ref1` → `4b0280810907413d8dde677f982f4542`
- `transition_id_ref2` → `${UUID}`
- `action_ref2` → `discord.list_guild_channels`
