# Standard Workflows

This directory contains all workflows that are designated as Standard workflows in Rewst.

## What are Standard Workflows?

Standard workflows are the primary type of Rewst workflow that:
- Have `"type": "STANDARD"` set in their workflow definition
- Perform specific business logic, automation tasks, or integrations
- Are triggered by forms, schedules, webhooks, or other workflows
- Handle the core operational processes and customer interactions

## Workflow Categories

The workflows in this directory serve various purposes including:
- **Customer Support & Operations**: Discord role management, HubSpot updates, ticket handling
- **Training & Education**: Event management, attendance tracking, certification processes  
- **Community Management**: User verification, role assignments, channel management
- **Data Integration**: HubSpot synchronization, property updates, contact management
- **Form Processing**: User submissions, role requests, customer outreach
- **Beta & Feature Management**: Access control, feature rollouts, testing workflows

## Organization

Each workflow is organized in its own subdirectory containing:
- The original `.bundle.json` file (unchanged for reliable Rewst importing)
- A `README.md` file with detailed workflow analysis and reference resolution

## Usage

These workflows handle the day-to-day operations and are typically:
- Triggered by user actions via forms
- Scheduled to run at specific intervals
- Called by webhooks from external systems
- Orchestrated as part of larger business processes
