# [Role Alignment] Form Submission

## Overview
**Workflow Type:** STANDARD  
**Export Date:** 2025-10-11T16:12:03.322088+00:00  
**Bundle Version:** 2  
**Original Bundle File:** `workflow-e9b84669-0796-4032-a425-fc9614613c1d_20251011_161203.bundle.json`  

## Workflow Documentation

### Data Processing & Analysis Engine

## Overview
This functional block serves as the foundation of the Role Alignment workflow, responsible for capturing, processing, and analyzing form submission data to determine user personas and characteristics.

## Key Objectives
- Extract and clean form input data from user submissions
- Map user responses to predefined role personas (Code Architect, Task Master, Auto Mage)
- Perform comprehensive statistical analysis of role alignment patterns
- Prepare structured data for downstream AI processing

## Included Tasks
- **core_noop**: Initial data capture and context preparation
  - Extracts form inputs while filtering out system variables
  - Identifies the form submitter for personalization
  - Establishes trial definitions and persona mappings
  - Creates merged data structure linking responses to personas

## Data Transformations
This block performs several critical data transformations:
- **Form Input Sanitization**: Removes system variables (execution_id, organization, etc.) to focus on user responses
- **Response-to-Persona Mapping**: Links each trial response to corresponding persona characteristics
- **Statistical Analysis**: Calculates role distribution, dominant patterns, and deviations
- **Data Structuring**: Prepares clean, analyzable datasets for AI consumption

## Key Outputs
- `form_inputs`: Cleaned user response data
- `form_submitter`: User identification for personalization
- `merged_data`: Structured trial-to-persona mappings
- `form_analysis`: Comprehensive statistical breakdown including dominant roles, deviations, and patterns

## Connection Points
This block feeds directly into the AI Content Generation phase, providing the structured data foundation needed for personalized content creation. The analysis results become the primary input for AI-driven insights and recommendations.

### AI Content Generation & Personalization

## Overview
This functional block leverages OpenAI's GPT models to transform raw role alignment data into personalized, insightful content tailored to each user's unique persona profile and trial responses.

## Key Objectives
- Generate personalized analysis of user's role alignment results
- Create comprehensive email content with professional formatting
- Provide contextual insights based on trial responses and persona patterns
- Deliver actionable recommendations aligned with user's identified roles

## Included Tasks
- **open_ai_create_chat_completion_2 (First Instance)**: Analytical Content Generation
  - Processes form inputs and merged trial data
  - Generates friendly, bullet-point analysis of role patterns
  - Creates insightful anecdotes based on user responses
  - Focuses on statistical breakdown and pattern recognition

- **open_ai_create_chat_completion_2 (Second Instance)**: Professional Email Formatting
  - Transforms analysis into structured email template
  - Applies professional formatting standards and best practices
  - Personalizes content based on dominant role identification
  - Creates comprehensive journey summary with actionable next steps

## AI Processing Approach
The dual-AI approach ensures both analytical depth and professional presentation:
- **First AI Call**: Focuses on data analysis, pattern recognition, and insight generation
- **Second AI Call**: Emphasizes professional communication, formatting, and user experience

## Content Personalization Features
- **Role-Specific Addressing**: Greets users by their dominant role or hybrid designation
- **Trial-Specific Insights**: References specific trials and user choices
- **Pattern Analysis**: Highlights unique combinations and deviations
- **Contextual Recommendations**: Provides next steps based on identified persona

## Key Outputs
- `ai_object`: Initial analytical content with insights and patterns
- `content`: Final formatted email content ready for delivery

## Connection Points
This block receives structured data from the Data Processing phase and transforms it into human-readable, personalized content. The generated content flows directly to the Communication & Delivery block for final user outreach.

### Communication & Results Delivery

## Overview
This functional block serves as the final delivery mechanism, ensuring that personalized role alignment results reach users through professional email communication with proper formatting and presentation.

## Key Objectives
- Deliver personalized role alignment results directly to users
- Ensure professional presentation with markdown rendering
- Provide immediate feedback on completed assessments
- Complete the user journey with actionable insights and next steps

## Included Tasks
- **core_sendmail**: Professional Email Delivery
  - Sends personalized results to the form submitter
  - Applies professional email formatting with markdown rendering
  - Uses branded sender address (${NOREPLY_EMAIL})
  - Delivers comprehensive role alignment analysis and recommendations

## Communication Features
- **Personalized Addressing**: Email sent directly to the form submitter's address
- **Professional Branding**: Uses Rewst's official no-reply sender address
- **Rich Formatting**: Markdown rendering enabled for enhanced readability
- **Clear Subject Line**: "Your Rewst Role Alignment Results" for immediate recognition
- **Comprehensive Content**: Full analysis, insights, and recommendations included

## Email Structure
The delivered email contains:
- Personalized greeting based on identified role
- Statistical breakdown of role alignment patterns
- Detailed trial-by-trial analysis
- Insights into unique persona combinations
- Actionable next steps for Rewst platform engagement
- Professional formatting with clear sections and bullet points

## Quality Assurance
- Automatic markdown rendering ensures consistent formatting
- Professional sender address maintains brand credibility
- Clear subject line ensures high open rates
- Personalized content increases engagement and relevance

## Key Inputs
- `form_submitter`: Target email address for delivery
- `ai_object`: Personalized content generated by AI processing

## Workflow Completion
This block represents the successful completion of the Role Alignment workflow, delivering valuable insights directly to users and enabling them to better understand their automation personas within the Rewst ecosystem.

## Connection Points
This is the terminal block of the workflow, receiving finalized content from the AI Content Generation phase and delivering it to end users. No downstream processing occurs after successful email delivery.

## Tasks

This workflow contains 4 tasks:

### 1. core_noop
**Description:** Action that does nothing
**Action:** ``core.noop``
**Transition Mode:** FOLLOW_FIRST
**Next Tasks:** 2 transition(s) defined

### 2. core_sendmail
**Description:** This sends an email
**Action:** ``core.sendmail``
**Next Tasks:** 1 transition(s) defined

### 3. open_ai_create_chat_completion_2
**Description:** Given a chat conversation, the model will return a chat completion response.
**Action:** ``openai.create_chat_completion``
**Next Tasks:** 1 transition(s) defined

### 4. open_ai_create_chat_completion_2
**Description:** Given a chat conversation, the model will return a chat completion response.
**Action:** ``openai.create_chat_completion``
**Next Tasks:** 1 transition(s) defined

## Technical Details

- **Workflow Timeout:** 28800 seconds
- **Total Tasks:** 4
- **Documentation Sections:** 3

---

*This README was automatically generated from the Rewst workflow export bundle.*
## Workflow Execution Details

### Overview
- **Total Tasks**: 4
- **Workflow Type**: STANDARD
- **Parallel Branches**: 1
- **Join Points**: 0
- **Resolved References**: 15

### Task Flow

#### 1. core_noop
- **Action**: `core.noop`
- **Task ID**: `7fcc8878abc24044a1ce007ba5254b68`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_FIRST
- **Join Count**: 0
- **Next Tasks**: 2

#### 2. core_sendmail
- **Action**: `core.sendmail`
- **Task ID**: `bff070e2b82d422cbc4801ef4f57768a`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 0
- **Has Input Configuration**: Yes

#### 3. open_ai_create_chat_completion_2
- **Action**: `openai.create_chat_completion`
- **Task ID**: `863cf124077f485fb67edb011b540764`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 0
- **Has Input Configuration**: Yes

#### 4. open_ai_create_chat_completion_2
- **Action**: `openai.create_chat_completion`
- **Task ID**: `0b1a36b6207f414fba99509194bc0fa6`
- **Timeout**: 600s
- **Transition Mode**: FOLLOW_ALL
- **Join Count**: 0
- **Next Tasks**: 1
- **Has Input Configuration**: Yes

### Resolved References
The following references were resolved from @@@placeholder@@@ format to actual values:

- `action_ref2` → `core.sendmail`
- `transition_id_ref5` → `${UUID}`
- `action_ref3` → `openai.create_chat_completion`
- `workflow_task_id_ref3` → `863cf124077f485fb67edb011b540764`
- `workflow_note_id_ref3` → `${UUID}`
- `workflow_task_id_ref4` → `0b1a36b6207f414fba99509194bc0fa6`
- `workflow_task_id_ref1` → `7fcc8878abc24044a1ce007ba5254b68`
- `action_ref1` → `core.noop`
- `transition_id_ref2` → `${UUID}`
- `transition_id_ref3` → `${UUID}`
- `transition_id_ref4` → `${UUID}`
- `transition_id_ref1` → `${UUID}`
- `workflow_note_id_ref2` → `${UUID}`
- `workflow_task_id_ref2` → `bff070e2b82d422cbc4801ef4f57768a`
- `workflow_note_id_ref1` → `${UUID}`
