# Client Communication & Notifications

Comprehensive patterns for email templates, Teams/Slack notifications, client portal updates, and multi-channel communication workflows.

---

## Email Templates

### HTML Email Structure

```jinja
{# Professional HTML Email Template #}
{% set email_html %}
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <style>
        body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; margin: 0; padding: 0; background-color: #f4f4f4; }
        .container { max-width: 600px; margin: 0 auto; background-color: #ffffff; }
        .header { background-color: {{ ORG.VARIABLES.brand_primary_color | d('#0078D4') }}; color: white; padding: 20px; text-align: center; }
        .header img { max-height: 50px; }
        .content { padding: 30px; color: #333333; line-height: 1.6; }
        .footer { background-color: #f8f9fa; padding: 20px; text-align: center; font-size: 12px; color: #666666; }
        .button { display: inline-block; background-color: {{ ORG.VARIABLES.brand_primary_color | d('#0078D4') }}; color: white; padding: 12px 24px; text-decoration: none; border-radius: 4px; margin: 10px 0; }
        .alert-box { padding: 15px; border-radius: 4px; margin: 15px 0; }
        .alert-info { background-color: #e7f3ff; border-left: 4px solid #0078D4; }
        .alert-warning { background-color: #fff3e0; border-left: 4px solid #ff9800; }
        .alert-error { background-color: #ffebee; border-left: 4px solid #f44336; }
        .alert-success { background-color: #e8f5e9; border-left: 4px solid #4caf50; }
        table { width: 100%; border-collapse: collapse; margin: 15px 0; }
        th, td { padding: 10px; text-align: left; border-bottom: 1px solid #eeeeee; }
        th { background-color: #f8f9fa; font-weight: 600; }
    </style>
</head>
<body>
    <div class="container">
        <div class="header">
            {% if ORG.VARIABLES.company_logo_url %}
            <img src="{{ ORG.VARIABLES.company_logo_url }}" alt="{{ ORG.VARIABLES.company_name | d('MSP') }}">
            {% else %}
            <h1>{{ ORG.VARIABLES.company_name | d('Your MSP') }}</h1>
            {% endif %}
        </div>
        <div class="content">
            {{ content | safe }}
        </div>
        <div class="footer">
            <p>{{ ORG.VARIABLES.company_name | d('Your MSP') }}</p>
            <p>{{ ORG.VARIABLES.company_address | d('') }}</p>
            <p>{{ ORG.VARIABLES.support_phone | d('') }} | {{ ORG.VARIABLES.support_email | d('') }}</p>
            <p>This is an automated message. Please do not reply directly to this email.</p>
        </div>
    </div>
</body>
</html>
{% endset %}
```

### Ticket Notification Templates

```jinja
{# New Ticket Created - Customer Notification #}
{% set ticket = CTX.ticket %}
{% set content %}
<h2>Your Support Request Has Been Received</h2>

<p>Hello {{ ticket.contact_name | d('Valued Customer') }},</p>

<p>Thank you for contacting us. We have received your support request and a member of our team will be in touch shortly.</p>

<div class="alert-box alert-info">
    <strong>Ticket Details</strong><br>
    <strong>Ticket #:</strong> {{ ticket.id }}<br>
    <strong>Subject:</strong> {{ ticket.summary }}<br>
    <strong>Priority:</strong> {{ ticket.priority | d('Normal') }}<br>
    <strong>Submitted:</strong> {{ ticket.created_at | format_datetime("%B %d, %Y at %I:%M %p") }}
</div>

<p><strong>Your Request:</strong></p>
<div style="background-color: #f8f9fa; padding: 15px; border-radius: 4px;">
{{ ticket.description | truncate(500) }}
</div>

{% if ticket.portal_url %}
<p style="text-align: center;">
    <a href="{{ ticket.portal_url }}" class="button">View Ticket Status</a>
</p>
{% endif %}

<p>If you need to provide additional information, please reply to this email or contact our support team.</p>
{% endset %}
```

```jinja
{# Ticket Status Update - Customer Notification #}
{% set ticket = CTX.ticket %}
{% set update = CTX.update %}

{% set status_colors = {
    "New": "#2196F3",
    "In Progress": "#FF9800",
    "Waiting on Customer": "#9C27B0",
    "Waiting on Third Party": "#607D8B",
    "Resolved": "#4CAF50",
    "Closed": "#9E9E9E"
} %}

{% set content %}
<h2>Ticket Update: {{ ticket.summary | truncate(50) }}</h2>

<p>Hello {{ ticket.contact_name | d('Valued Customer') }},</p>

<p>Your support ticket has been updated with the following information:</p>

<div class="alert-box alert-info">
    <strong>Ticket #:</strong> {{ ticket.id }}<br>
    <strong>Status:</strong> <span style="color: {{ status_colors[ticket.status] | d('#333') }}; font-weight: bold;">{{ ticket.status }}</span><br>
    <strong>Updated:</strong> {{ update.timestamp | format_datetime("%B %d, %Y at %I:%M %p") }}<br>
    <strong>Updated By:</strong> {{ update.updated_by | d('Support Team') }}
</div>

{% if update.notes %}
<p><strong>Update Notes:</strong></p>
<div style="background-color: #f8f9fa; padding: 15px; border-radius: 4px;">
{{ update.notes | safe }}
</div>
{% endif %}

{% if ticket.status == "Waiting on Customer" %}
<div class="alert-box alert-warning">
    <strong>Action Required:</strong> We need additional information from you to proceed. Please respond to this email or update the ticket through the portal.
</div>
{% endif %}

{% if ticket.status == "Resolved" %}
<div class="alert-box alert-success">
    <strong>Issue Resolved:</strong> We believe your issue has been resolved. If you're still experiencing problems, please let us know and we'll reopen the ticket.
</div>
{% endif %}

{% if ticket.portal_url %}
<p style="text-align: center;">
    <a href="{{ ticket.portal_url }}" class="button">View Full Ticket</a>
</p>
{% endif %}
{% endset %}
```

```jinja
{# Ticket Resolution Summary #}
{% set ticket = CTX.ticket %}
{% set resolution = CTX.resolution %}

{% set content %}
<h2>Ticket Resolved: {{ ticket.summary | truncate(50) }}</h2>

<p>Hello {{ ticket.contact_name | d('Valued Customer') }},</p>

<p>Great news! Your support request has been resolved.</p>

<div class="alert-box alert-success">
    <strong>Ticket #:</strong> {{ ticket.id }}<br>
    <strong>Opened:</strong> {{ ticket.created_at | format_datetime("%B %d, %Y") }}<br>
    <strong>Resolved:</strong> {{ resolution.resolved_at | format_datetime("%B %d, %Y") }}<br>
    <strong>Resolution Time:</strong> {{ resolution.time_to_resolve }}
</div>

<p><strong>Resolution Summary:</strong></p>
<div style="background-color: #f8f9fa; padding: 15px; border-radius: 4px;">
{{ resolution.summary }}
</div>

{% if resolution.root_cause %}
<p><strong>Root Cause:</strong></p>
<p>{{ resolution.root_cause }}</p>
{% endif %}

{% if resolution.preventive_measures %}
<p><strong>Preventive Measures:</strong></p>
<ul>
{% for measure in resolution.preventive_measures %}
    <li>{{ measure }}</li>
{% endfor %}
</ul>
{% endif %}

<div class="alert-box alert-info">
    <strong>Was this helpful?</strong><br>
    We value your feedback! Please take a moment to rate your support experience.
    <p style="text-align: center; margin-top: 10px;">
        <a href="{{ CTX.survey_url }}" class="button">Rate Your Experience</a>
    </p>
</div>

<p>If you have any further questions or if the issue reoccurs, please don't hesitate to contact us.</p>
{% endset %}
```

### User Provisioning Email Templates

```jinja
{# New User Welcome Email #}
{% set user = CTX.new_user %}
{% set credentials = CTX.credentials %}

{% set content %}
<h2>Welcome to {{ ORG.VARIABLES.company_name | d('the team') }}!</h2>

<p>Hello {{ user.first_name }},</p>

<p>Your account has been created and you're all set to get started. Below you'll find your login credentials and important information to help you access your new tools.</p>

<div class="alert-box alert-info">
    <strong>Your Account Details</strong><br>
    <strong>Username:</strong> {{ user.email }}<br>
    <strong>Temporary Password:</strong> {{ credentials.temp_password }}<br>
    <strong>Email:</strong> {{ user.email }}
</div>

<div class="alert-box alert-warning">
    <strong>Important:</strong> You will be required to change your password upon first login. Please also set up Multi-Factor Authentication (MFA) for account security.
</div>

<h3>Getting Started</h3>
<table>
    <tr>
        <th>Service</th>
        <th>Access URL</th>
    </tr>
    {% if user.has_m365 %}
    <tr>
        <td>Microsoft 365 (Email, Teams, etc.)</td>
        <td><a href="https://portal.office.com">portal.office.com</a></td>
    </tr>
    {% endif %}
    {% if user.has_vpn %}
    <tr>
        <td>VPN Access</td>
        <td><a href="{{ ORG.VARIABLES.vpn_portal_url }}">{{ ORG.VARIABLES.vpn_portal_url }}</a></td>
    </tr>
    {% endif %}
    {% for app in user.additional_apps | d([]) %}
    <tr>
        <td>{{ app.name }}</td>
        <td><a href="{{ app.url }}">{{ app.url }}</a></td>
    </tr>
    {% endfor %}
</table>

<h3>Need Help?</h3>
<p>If you have any questions or need assistance, please contact our IT support team:</p>
<ul>
    <li>Email: {{ ORG.VARIABLES.support_email | d('support@company.com') }}</li>
    <li>Phone: {{ ORG.VARIABLES.support_phone | d('555-123-4567') }}</li>
    {% if ORG.VARIABLES.helpdesk_portal_url %}
    <li>Help Desk Portal: <a href="{{ ORG.VARIABLES.helpdesk_portal_url }}">{{ ORG.VARIABLES.helpdesk_portal_url }}</a></li>
    {% endif %}
</ul>

<p style="text-align: center;">
    <a href="https://portal.office.com" class="button">Login Now</a>
</p>
{% endset %}
```

```jinja
{# Password Reset Notification #}
{% set user = CTX.user %}
{% set reset = CTX.reset %}

{% set content %}
<h2>Password Reset Completed</h2>

<p>Hello {{ user.first_name | d(user.display_name) }},</p>

<p>Your password has been reset as requested. Your new temporary password is below:</p>

<div class="alert-box alert-info">
    <strong>Username:</strong> {{ user.email }}<br>
    <strong>Temporary Password:</strong> {{ reset.temp_password }}
</div>

<div class="alert-box alert-warning">
    <strong>Security Notice:</strong>
    <ul style="margin: 10px 0;">
        <li>You must change this password upon your next login</li>
        <li>This password will expire in {{ reset.expiry_hours | d(24) }} hours</li>
        <li>Never share your password with anyone</li>
        <li>If you did not request this reset, contact IT immediately</li>
    </ul>
</div>

<p style="text-align: center;">
    <a href="https://portal.office.com" class="button">Login Now</a>
</p>

<p>If you continue to experience issues, please contact the IT help desk.</p>
{% endset %}
```

```jinja
{# Account Deactivation Notice (for manager/HR) #}
{% set user = CTX.offboarded_user %}
{% set actions = CTX.completed_actions %}

{% set content %}
<h2>User Offboarding Complete: {{ user.display_name }}</h2>

<p>The following user account has been deactivated as requested:</p>

<div class="alert-box alert-info">
    <strong>User:</strong> {{ user.display_name }}<br>
    <strong>Email:</strong> {{ user.email }}<br>
    <strong>Department:</strong> {{ user.department | d('N/A') }}<br>
    <strong>Termination Date:</strong> {{ user.termination_date | format_datetime("%B %d, %Y") }}<br>
    <strong>Processed:</strong> {{ CTX.processing_timestamp | format_datetime("%B %d, %Y at %I:%M %p") }}
</div>

<h3>Completed Actions</h3>
<table>
    <tr>
        <th>Action</th>
        <th>Status</th>
        <th>Details</th>
    </tr>
    {% for action in actions %}
    <tr>
        <td>{{ action.name }}</td>
        <td style="color: {{ 'green' if action.success else 'red' }};">{{ 'Completed' if action.success else 'Failed' }}</td>
        <td>{{ action.details | d('-') }}</td>
    </tr>
    {% endfor %}
</table>

{% set failed_actions = actions | selectattr("success", "eq", false) | list %}
{% if failed_actions %}
<div class="alert-box alert-error">
    <strong>Attention Required:</strong> {{ failed_actions | length }} action(s) failed during offboarding. Please review and complete manually if necessary.
</div>
{% endif %}

{% if user.forwarding_email %}
<div class="alert-box alert-warning">
    <strong>Email Forwarding Active:</strong> Emails to {{ user.email }} are being forwarded to {{ user.forwarding_email }} for {{ user.forwarding_duration | d('30') }} days.
</div>
{% endif %}

<p>A copy of this report has been saved for compliance records.</p>
{% endset %}
```

### Alert & Incident Notifications

```jinja
{# Critical Alert Notification #}
{% set alert = CTX.alert %}

{% set severity_config = {
    "critical": {"color": "#d32f2f", "icon": "🔴", "text": "CRITICAL"},
    "high": {"color": "#f57c00", "icon": "🟠", "text": "HIGH"},
    "medium": {"color": "#fbc02d", "icon": "🟡", "text": "MEDIUM"},
    "low": {"color": "#388e3c", "icon": "🟢", "text": "LOW"}
} %}
{% set config = severity_config[alert.severity | lower] | d(severity_config.medium) %}

{% set content %}
<div style="background-color: {{ config.color }}; color: white; padding: 15px; text-align: center; border-radius: 4px 4px 0 0;">
    <h2 style="margin: 0;">{{ config.icon }} {{ config.text }} ALERT {{ config.icon }}</h2>
</div>

<div style="border: 2px solid {{ config.color }}; padding: 20px; border-radius: 0 0 4px 4px;">
    <h3>{{ alert.title }}</h3>

    <table>
        <tr>
            <td><strong>Device:</strong></td>
            <td>{{ alert.device_name }}</td>
        </tr>
        <tr>
            <td><strong>Client:</strong></td>
            <td>{{ alert.organization }}</td>
        </tr>
        <tr>
            <td><strong>Alert Type:</strong></td>
            <td>{{ alert.type }}</td>
        </tr>
        <tr>
            <td><strong>Time:</strong></td>
            <td>{{ alert.timestamp | format_datetime("%B %d, %Y at %I:%M %p %Z") }}</td>
        </tr>
        <tr>
            <td><strong>Source:</strong></td>
            <td>{{ alert.source | d('Monitoring System') }}</td>
        </tr>
    </table>

    <p><strong>Alert Details:</strong></p>
    <div style="background-color: #f8f9fa; padding: 15px; border-radius: 4px; font-family: monospace; font-size: 13px;">
        {{ alert.message }}
    </div>

    {% if alert.recommended_actions %}
    <p><strong>Recommended Actions:</strong></p>
    <ul>
        {% for action in alert.recommended_actions %}
        <li>{{ action }}</li>
        {% endfor %}
    </ul>
    {% endif %}

    {% if alert.ticket_id %}
    <div class="alert-box alert-info">
        <strong>Ticket Created:</strong> #{{ alert.ticket_id }}
        {% if alert.ticket_url %}
        <br><a href="{{ alert.ticket_url }}">View Ticket</a>
        {% endif %}
    </div>
    {% endif %}
</div>
{% endset %}
```

```jinja
{# Scheduled Maintenance Notification #}
{% set maintenance = CTX.maintenance %}

{% set content %}
<h2>Scheduled Maintenance Notice</h2>

<p>Hello,</p>

<p>We wanted to inform you about upcoming scheduled maintenance that may affect your services.</p>

<div class="alert-box alert-warning">
    <strong>Maintenance Window</strong><br>
    <strong>Date:</strong> {{ maintenance.date | format_datetime("%A, %B %d, %Y") }}<br>
    <strong>Time:</strong> {{ maintenance.start_time }} - {{ maintenance.end_time }} {{ maintenance.timezone | d('EST') }}<br>
    <strong>Expected Duration:</strong> {{ maintenance.duration }}
</div>

<p><strong>What's Being Done:</strong></p>
<p>{{ maintenance.description }}</p>

<p><strong>Services Affected:</strong></p>
<ul>
{% for service in maintenance.affected_services %}
    <li>{{ service }}</li>
{% endfor %}
</ul>

<p><strong>Expected Impact:</strong></p>
<p>{{ maintenance.impact_description | d('You may experience brief interruptions to the services listed above during the maintenance window.') }}</p>

{% if maintenance.actions_required %}
<div class="alert-box alert-info">
    <strong>Actions Required:</strong>
    <ul>
    {% for action in maintenance.actions_required %}
        <li>{{ action }}</li>
    {% endfor %}
    </ul>
</div>
{% endif %}

<p>We apologize for any inconvenience and thank you for your patience. If you have any questions or concerns, please don't hesitate to contact us.</p>
{% endset %}
```

---

## Microsoft Teams Notifications

### Adaptive Card Templates

```jinja
{# Teams Adaptive Card - Ticket Created #}
{% set ticket = CTX.ticket %}
{
    "type": "message",
    "attachments": [
        {
            "contentType": "application/vnd.microsoft.card.adaptive",
            "content": {
                "$schema": "http://adaptivecards.io/schemas/adaptive-card.json",
                "type": "AdaptiveCard",
                "version": "1.4",
                "body": [
                    {
                        "type": "Container",
                        "style": "emphasis",
                        "items": [
                            {
                                "type": "ColumnSet",
                                "columns": [
                                    {
                                        "type": "Column",
                                        "width": "auto",
                                        "items": [
                                            {
                                                "type": "Image",
                                                "url": "https://img.icons8.com/color/48/ticket.png",
                                                "size": "Small"
                                            }
                                        ]
                                    },
                                    {
                                        "type": "Column",
                                        "width": "stretch",
                                        "items": [
                                            {
                                                "type": "TextBlock",
                                                "text": "New Ticket Created",
                                                "weight": "Bolder",
                                                "size": "Medium",
                                                "color": "Accent"
                                            },
                                            {
                                                "type": "TextBlock",
                                                "text": "#{{ ticket.id }}",
                                                "spacing": "None",
                                                "isSubtle": true
                                            }
                                        ]
                                    }
                                ]
                            }
                        ]
                    },
                    {
                        "type": "Container",
                        "items": [
                            {
                                "type": "TextBlock",
                                "text": "{{ ticket.summary | replace('"', '\\"') }}",
                                "wrap": true,
                                "weight": "Bolder"
                            },
                            {
                                "type": "FactSet",
                                "facts": [
                                    {
                                        "title": "Client",
                                        "value": "{{ ticket.company_name | d('Unknown') }}"
                                    },
                                    {
                                        "title": "Contact",
                                        "value": "{{ ticket.contact_name | d('Unknown') }}"
                                    },
                                    {
                                        "title": "Priority",
                                        "value": "{{ ticket.priority | d('Normal') }}"
                                    },
                                    {
                                        "title": "Board",
                                        "value": "{{ ticket.board | d('Default') }}"
                                    },
                                    {
                                        "title": "Created",
                                        "value": "{{ ticket.created_at | format_datetime('%Y-%m-%d %H:%M') }}"
                                    }
                                ]
                            }
                        ]
                    },
                    {
                        "type": "TextBlock",
                        "text": "{{ ticket.description | truncate(200) | replace('"', '\\"') | replace('\n', ' ') }}",
                        "wrap": true,
                        "isSubtle": true
                    }
                ],
                "actions": [
                    {
                        "type": "Action.OpenUrl",
                        "title": "View Ticket",
                        "url": "{{ ticket.url }}"
                    },
                    {
                        "type": "Action.OpenUrl",
                        "title": "View Client",
                        "url": "{{ ticket.company_url }}"
                    }
                ]
            }
        }
    ]
}
```

```jinja
{# Teams Adaptive Card - Alert Notification #}
{% set alert = CTX.alert %}
{% set severity_colors = {
    "critical": "Attention",
    "high": "Warning",
    "medium": "Accent",
    "low": "Good"
} %}
{% set color = severity_colors[alert.severity | lower] | d("Accent") %}

{
    "type": "message",
    "attachments": [
        {
            "contentType": "application/vnd.microsoft.card.adaptive",
            "content": {
                "$schema": "http://adaptivecards.io/schemas/adaptive-card.json",
                "type": "AdaptiveCard",
                "version": "1.4",
                "body": [
                    {
                        "type": "Container",
                        "style": "{{ 'attention' if alert.severity | lower == 'critical' else 'emphasis' }}",
                        "items": [
                            {
                                "type": "TextBlock",
                                "text": "🚨 {{ alert.severity | upper }} ALERT",
                                "weight": "Bolder",
                                "size": "Large",
                                "color": "{{ color }}"
                            }
                        ]
                    },
                    {
                        "type": "TextBlock",
                        "text": "{{ alert.title | replace('"', '\\"') }}",
                        "wrap": true,
                        "weight": "Bolder",
                        "size": "Medium"
                    },
                    {
                        "type": "FactSet",
                        "facts": [
                            {
                                "title": "Device",
                                "value": "{{ alert.device_name }}"
                            },
                            {
                                "title": "Client",
                                "value": "{{ alert.organization }}"
                            },
                            {
                                "title": "Type",
                                "value": "{{ alert.type }}"
                            },
                            {
                                "title": "Time",
                                "value": "{{ alert.timestamp | format_datetime('%Y-%m-%d %H:%M %Z') }}"
                            }
                        ]
                    },
                    {
                        "type": "TextBlock",
                        "text": "{{ alert.message | truncate(300) | replace('"', '\\"') | replace('\n', ' ') }}",
                        "wrap": true,
                        "isSubtle": true
                    }
                ],
                "actions": [
                    {% if alert.ticket_url %}
                    {
                        "type": "Action.OpenUrl",
                        "title": "View Ticket",
                        "url": "{{ alert.ticket_url }}"
                    },
                    {% endif %}
                    {
                        "type": "Action.OpenUrl",
                        "title": "View Device",
                        "url": "{{ alert.device_url }}"
                    }
                ]
            }
        }
    ]
}
```

```jinja
{# Teams Adaptive Card - Approval Request #}
{% set request = CTX.approval_request %}
{
    "type": "message",
    "attachments": [
        {
            "contentType": "application/vnd.microsoft.card.adaptive",
            "content": {
                "$schema": "http://adaptivecards.io/schemas/adaptive-card.json",
                "type": "AdaptiveCard",
                "version": "1.4",
                "body": [
                    {
                        "type": "Container",
                        "style": "accent",
                        "items": [
                            {
                                "type": "TextBlock",
                                "text": "⏳ Approval Required",
                                "weight": "Bolder",
                                "size": "Large",
                                "color": "Accent"
                            }
                        ]
                    },
                    {
                        "type": "TextBlock",
                        "text": "{{ request.title | replace('"', '\\"') }}",
                        "wrap": true,
                        "weight": "Bolder",
                        "size": "Medium"
                    },
                    {
                        "type": "FactSet",
                        "facts": [
                            {
                                "title": "Requested By",
                                "value": "{{ request.requester_name }}"
                            },
                            {
                                "title": "Type",
                                "value": "{{ request.type }}"
                            },
                            {
                                "title": "Priority",
                                "value": "{{ request.priority | d('Normal') }}"
                            },
                            {
                                "title": "Submitted",
                                "value": "{{ request.submitted_at | format_datetime('%Y-%m-%d %H:%M') }}"
                            }
                        ]
                    },
                    {
                        "type": "TextBlock",
                        "text": "**Details:**",
                        "wrap": true
                    },
                    {
                        "type": "TextBlock",
                        "text": "{{ request.description | replace('"', '\\"') | replace('\n', ' ') }}",
                        "wrap": true,
                        "isSubtle": true
                    },
                    {
                        "type": "Input.Text",
                        "id": "comments",
                        "placeholder": "Add comments (optional)",
                        "isMultiline": true
                    }
                ],
                "actions": [
                    {
                        "type": "Action.Http",
                        "title": "✅ Approve",
                        "method": "POST",
                        "url": "{{ request.approve_webhook_url }}",
                        "body": "{\"request_id\": \"{{ request.id }}\", \"action\": \"approve\", \"comments\": \"{{comments.value}}\"}"
                    },
                    {
                        "type": "Action.Http",
                        "title": "❌ Reject",
                        "method": "POST",
                        "url": "{{ request.reject_webhook_url }}",
                        "body": "{\"request_id\": \"{{ request.id }}\", \"action\": \"reject\", \"comments\": \"{{comments.value}}\"}"
                    },
                    {
                        "type": "Action.OpenUrl",
                        "title": "View Details",
                        "url": "{{ request.details_url }}"
                    }
                ]
            }
        }
    ]
}
```

### Simple Webhook Message Format

```jinja
{# Simple Teams Webhook Message #}
{% set message = CTX.message %}
{
    "@type": "MessageCard",
    "@context": "http://schema.org/extensions",
    "themeColor": "{{ message.color | d('0078D4') }}",
    "summary": "{{ message.summary | replace('"', '\\"') }}",
    "sections": [
        {
            "activityTitle": "{{ message.title | replace('"', '\\"') }}",
            "activitySubtitle": "{{ message.subtitle | d('') | replace('"', '\\"') }}",
            "activityImage": "{{ message.image_url | d('') }}",
            "facts": [
                {% for fact in message.facts | d([]) %}
                {
                    "name": "{{ fact.name }}",
                    "value": "{{ fact.value }}"
                }{{ "," if not loop.last else "" }}
                {% endfor %}
            ],
            "markdown": true,
            "text": "{{ message.text | replace('"', '\\"') | replace('\n', '\\n') }}"
        }
    ],
    "potentialAction": [
        {% for action in message.actions | d([]) %}
        {
            "@type": "OpenUri",
            "name": "{{ action.name }}",
            "targets": [
                {
                    "os": "default",
                    "uri": "{{ action.url }}"
                }
            ]
        }{{ "," if not loop.last else "" }}
        {% endfor %}
    ]
}
```

---

## Slack Notifications

### Block Kit Templates

```jinja
{# Slack Block Kit - Ticket Notification #}
{% set ticket = CTX.ticket %}
{% set priority_emoji = {
    "Critical": "🔴",
    "High": "🟠",
    "Medium": "🟡",
    "Low": "🟢"
} %}

{
    "blocks": [
        {
            "type": "header",
            "text": {
                "type": "plain_text",
                "text": "{{ priority_emoji[ticket.priority] | d('🎫') }} New Ticket #{{ ticket.id }}",
                "emoji": true
            }
        },
        {
            "type": "section",
            "text": {
                "type": "mrkdwn",
                "text": "*{{ ticket.summary | replace('"', '\\"') }}*"
            }
        },
        {
            "type": "section",
            "fields": [
                {
                    "type": "mrkdwn",
                    "text": "*Client:*\n{{ ticket.company_name }}"
                },
                {
                    "type": "mrkdwn",
                    "text": "*Contact:*\n{{ ticket.contact_name }}"
                },
                {
                    "type": "mrkdwn",
                    "text": "*Priority:*\n{{ ticket.priority | d('Normal') }}"
                },
                {
                    "type": "mrkdwn",
                    "text": "*Board:*\n{{ ticket.board | d('Default') }}"
                }
            ]
        },
        {
            "type": "section",
            "text": {
                "type": "mrkdwn",
                "text": "{{ ticket.description | truncate(200) | replace('"', '\\"') }}"
            }
        },
        {
            "type": "divider"
        },
        {
            "type": "actions",
            "elements": [
                {
                    "type": "button",
                    "text": {
                        "type": "plain_text",
                        "text": "View Ticket",
                        "emoji": true
                    },
                    "url": "{{ ticket.url }}",
                    "style": "primary"
                },
                {
                    "type": "button",
                    "text": {
                        "type": "plain_text",
                        "text": "View Client",
                        "emoji": true
                    },
                    "url": "{{ ticket.company_url }}"
                }
            ]
        }
    ]
}
```

```jinja
{# Slack Block Kit - Alert Notification #}
{% set alert = CTX.alert %}
{% set severity_config = {
    "critical": {"emoji": "🚨", "color": "#D32F2F"},
    "high": {"emoji": "⚠️", "color": "#F57C00"},
    "medium": {"emoji": "📢", "color": "#FBC02D"},
    "low": {"emoji": "ℹ️", "color": "#388E3C"}
} %}
{% set config = severity_config[alert.severity | lower] | d(severity_config.medium) %}

{
    "attachments": [
        {
            "color": "{{ config.color }}",
            "blocks": [
                {
                    "type": "header",
                    "text": {
                        "type": "plain_text",
                        "text": "{{ config.emoji }} {{ alert.severity | upper }} ALERT",
                        "emoji": true
                    }
                },
                {
                    "type": "section",
                    "text": {
                        "type": "mrkdwn",
                        "text": "*{{ alert.title | replace('"', '\\"') }}*"
                    }
                },
                {
                    "type": "section",
                    "fields": [
                        {
                            "type": "mrkdwn",
                            "text": "*Device:*\n{{ alert.device_name }}"
                        },
                        {
                            "type": "mrkdwn",
                            "text": "*Client:*\n{{ alert.organization }}"
                        },
                        {
                            "type": "mrkdwn",
                            "text": "*Type:*\n{{ alert.type }}"
                        },
                        {
                            "type": "mrkdwn",
                            "text": "*Time:*\n{{ alert.timestamp | format_datetime('%Y-%m-%d %H:%M') }}"
                        }
                    ]
                },
                {
                    "type": "section",
                    "text": {
                        "type": "mrkdwn",
                        "text": "```{{ alert.message | truncate(500) }}```"
                    }
                },
                {
                    "type": "actions",
                    "elements": [
                        {% if alert.ticket_url %}
                        {
                            "type": "button",
                            "text": {
                                "type": "plain_text",
                                "text": "View Ticket"
                            },
                            "url": "{{ alert.ticket_url }}",
                            "style": "primary"
                        },
                        {% endif %}
                        {
                            "type": "button",
                            "text": {
                                "type": "plain_text",
                                "text": "Acknowledge"
                            },
                            "action_id": "ack_alert_{{ alert.id }}",
                            "value": "{{ alert.id }}"
                        }
                    ]
                }
            ]
        }
    ]
}
```

```jinja
{# Slack Block Kit - Daily Summary Report #}
{% set report = CTX.daily_report %}

{
    "blocks": [
        {
            "type": "header",
            "text": {
                "type": "plain_text",
                "text": "📊 Daily Operations Summary - {{ report.date | format_datetime('%B %d, %Y') }}",
                "emoji": true
            }
        },
        {
            "type": "section",
            "text": {
                "type": "mrkdwn",
                "text": "*Ticket Statistics*"
            }
        },
        {
            "type": "section",
            "fields": [
                {
                    "type": "mrkdwn",
                    "text": "*New Tickets:*\n{{ report.tickets.new }}"
                },
                {
                    "type": "mrkdwn",
                    "text": "*Resolved:*\n{{ report.tickets.resolved }}"
                },
                {
                    "type": "mrkdwn",
                    "text": "*Open:*\n{{ report.tickets.open }}"
                },
                {
                    "type": "mrkdwn",
                    "text": "*Avg Response:*\n{{ report.tickets.avg_response_time }}"
                }
            ]
        },
        {
            "type": "divider"
        },
        {
            "type": "section",
            "text": {
                "type": "mrkdwn",
                "text": "*Alert Statistics*"
            }
        },
        {
            "type": "section",
            "fields": [
                {
                    "type": "mrkdwn",
                    "text": "*Total Alerts:*\n{{ report.alerts.total }}"
                },
                {
                    "type": "mrkdwn",
                    "text": "*Critical:*\n{{ report.alerts.critical }}"
                },
                {
                    "type": "mrkdwn",
                    "text": "*Auto-Resolved:*\n{{ report.alerts.auto_resolved }}"
                },
                {
                    "type": "mrkdwn",
                    "text": "*Escalated:*\n{{ report.alerts.escalated }}"
                }
            ]
        },
        {% if report.top_issues %}
        {
            "type": "divider"
        },
        {
            "type": "section",
            "text": {
                "type": "mrkdwn",
                "text": "*Top Issues:*\n{% for issue in report.top_issues[:5] %}• {{ issue.description }} ({{ issue.count }})\n{% endfor %}"
            }
        },
        {% endif %}
        {
            "type": "context",
            "elements": [
                {
                    "type": "mrkdwn",
                    "text": "Generated at {{ report.generated_at | format_datetime('%I:%M %p %Z') }} | <{{ report.full_report_url }}|View Full Report>"
                }
            ]
        }
    ]
}
```

---

## Multi-Channel Notification Patterns

### Channel Selection Logic

```jinja
{# Determine notification channels based on event type and severity #}
{% set event = CTX.event %}

{% set channel_config = {
    "critical_alert": {
        "channels": ["teams", "slack", "email", "sms"],
        "teams_channel": ORG.VARIABLES.critical_alerts_channel,
        "slack_channel": "#critical-alerts",
        "email_group": ORG.VARIABLES.oncall_email_group,
        "sms_enabled": true
    },
    "high_alert": {
        "channels": ["teams", "slack", "email"],
        "teams_channel": ORG.VARIABLES.alerts_channel,
        "slack_channel": "#alerts",
        "email_group": ORG.VARIABLES.alerts_email_group,
        "sms_enabled": false
    },
    "ticket_created": {
        "channels": ["teams", "slack"],
        "teams_channel": ORG.VARIABLES.tickets_channel,
        "slack_channel": "#tickets",
        "sms_enabled": false
    },
    "user_provisioned": {
        "channels": ["email"],
        "email_to_user": true,
        "email_to_manager": true,
        "sms_enabled": false
    },
    "scheduled_maintenance": {
        "channels": ["teams", "slack", "email"],
        "teams_channel": ORG.VARIABLES.announcements_channel,
        "slack_channel": "#announcements",
        "email_group": ORG.VARIABLES.all_clients_email_group,
        "sms_enabled": false
    }
} %}

{% set config = channel_config[event.type] | d(channel_config.ticket_created) %}

{# Build notification list #}
{% set notifications = [] %}

{% for channel in config.channels %}
    {% if channel == "teams" and config.teams_channel %}
        {% set _ = notifications.append({
            "channel": "teams",
            "webhook_url": config.teams_channel,
            "template": event.type ~ "_teams"
        }) %}
    {% elif channel == "slack" and config.slack_channel %}
        {% set _ = notifications.append({
            "channel": "slack",
            "channel_id": config.slack_channel,
            "template": event.type ~ "_slack"
        }) %}
    {% elif channel == "email" %}
        {% set _ = notifications.append({
            "channel": "email",
            "recipients": config.email_group | d(event.recipients),
            "template": event.type ~ "_email"
        }) %}
    {% elif channel == "sms" and config.sms_enabled %}
        {% set _ = notifications.append({
            "channel": "sms",
            "recipients": ORG.VARIABLES.oncall_phone_numbers,
            "template": event.type ~ "_sms"
        }) %}
    {% endif %}
{% endfor %}

{{ notifications }}
```

### Notification Throttling

```jinja
{# Prevent notification spam with throttling #}
{% set alert = CTX.alert %}
{% set recent_notifications = CTX.recent_notifications | d([]) %}

{# Throttle configuration #}
{% set throttle_config = {
    "critical": {"max_per_hour": 10, "cooldown_minutes": 5},
    "high": {"max_per_hour": 20, "cooldown_minutes": 10},
    "medium": {"max_per_hour": 30, "cooldown_minutes": 15},
    "low": {"max_per_hour": 50, "cooldown_minutes": 30}
} %}

{% set config = throttle_config[alert.severity | lower] | d(throttle_config.medium) %}

{# Count recent notifications for this alert type #}
{% set now = CTX.current_time | as_datetime %}
{% set one_hour_ago = now | datedelta(hours=-1) %}
{% set cooldown_cutoff = now | datedelta(minutes=-config.cooldown_minutes) %}

{# Filter to same device and alert type within timeframe #}
{% set matching_recent = recent_notifications | selectattr("device_id", "eq", alert.device_id) | selectattr("alert_type", "eq", alert.type) | list %}

{% set hourly_count = matching_recent | selectattr("timestamp", "ge", one_hour_ago | string) | list | length %}
{% set in_cooldown = matching_recent | selectattr("timestamp", "ge", cooldown_cutoff | string) | list | length > 0 %}

{# Determine if should send #}
{% set should_notify = not in_cooldown and hourly_count < config.max_per_hour %}

{# Output decision #}
{
    "should_notify": {{ should_notify | lower }},
    "reason": {% if in_cooldown %}"In cooldown period ({{ config.cooldown_minutes }} minutes)"{% elif hourly_count >= config.max_per_hour %}"Hourly limit reached ({{ config.max_per_hour }})"{% else %}"OK to send"{% endif %},
    "hourly_count": {{ hourly_count }},
    "cooldown_active": {{ in_cooldown | lower }},
    "next_allowed": {% if in_cooldown %}"{{ cooldown_cutoff | datedelta(minutes=config.cooldown_minutes) | format_datetime('%H:%M:%S') }}"{% else %}"now"{% endif %}
}
```

### Notification Aggregation

```jinja
{# Aggregate multiple alerts into a single digest notification #}
{% set alerts = CTX.pending_alerts | d([]) %}
{% set aggregation_window_minutes = 15 %}

{# Group alerts by device #}
{% set alerts_by_device = {} %}
{% for alert in alerts %}
    {% set device_key = alert.device_id %}
    {% if device_key not in alerts_by_device %}
        {% set _ = alerts_by_device.update({device_key: []}) %}
    {% endif %}
    {% set _ = alerts_by_device[device_key].append(alert) %}
{% endfor %}

{# Build digest #}
{% set digest_items = [] %}
{% for device_id, device_alerts in alerts_by_device.items() %}
    {% set highest_severity = device_alerts | map(attribute="severity") | list | sort | first %}
    {% set _ = digest_items.append({
        "device_id": device_id,
        "device_name": device_alerts[0].device_name,
        "organization": device_alerts[0].organization,
        "alert_count": device_alerts | length,
        "highest_severity": highest_severity,
        "alert_types": device_alerts | map(attribute="type") | unique | list,
        "alerts": device_alerts
    }) %}
{% endfor %}

{# Sort by severity then count #}
{% set severity_order = {"critical": 0, "high": 1, "medium": 2, "low": 3} %}
{% set sorted_digest = digest_items | sort(attribute="highest_severity") %}

{# Output digest summary #}
{
    "total_alerts": {{ alerts | length }},
    "devices_affected": {{ alerts_by_device | length }},
    "critical_count": {{ alerts | selectattr("severity", "eq", "critical") | list | length }},
    "high_count": {{ alerts | selectattr("severity", "eq", "high") | list | length }},
    "digest_items": {{ sorted_digest | tojson }},
    "aggregation_window": "{{ aggregation_window_minutes }} minutes"
}
```

---

## SMS Notifications

### SMS Templates (Short)

```jinja
{# Critical Alert SMS #}
{% set alert = CTX.alert %}
🚨 CRITICAL: {{ alert.title | truncate(50) }}
Device: {{ alert.device_name }}
Client: {{ alert.organization }}
{% if alert.ticket_id %}Ticket #{{ alert.ticket_id }}{% endif %}
```

```jinja
{# On-Call Escalation SMS #}
{% set escalation = CTX.escalation %}
⚠️ ESCALATION: {{ escalation.reason | truncate(60) }}
Ticket #{{ escalation.ticket_id }}
Client: {{ escalation.organization }}
Priority: {{ escalation.priority }}
Escalated to you at {{ escalation.time | format_datetime("%H:%M") }}
```

```jinja
{# Ticket Assignment SMS #}
{% set ticket = CTX.ticket %}
📋 New Ticket Assigned
#{{ ticket.id }}: {{ ticket.summary | truncate(40) }}
Client: {{ ticket.company_name }}
Priority: {{ ticket.priority }}
```

---

## Workflow Integration Patterns

### Send Email Action Configuration

```jinja
{# Email action parameters #}
{
    "to": {{ CTX.recipients | tojson }},
    "cc": {{ CTX.cc_recipients | d([]) | tojson }},
    "bcc": {{ CTX.bcc_recipients | d([]) | tojson }},
    "subject": "{{ CTX.email_subject }}",
    "body": {{ CTX.email_html | tojson }},
    "is_html": true,
    "from_name": "{{ ORG.VARIABLES.email_from_name | d('IT Support') }}",
    "reply_to": "{{ ORG.VARIABLES.support_email | d('') }}"
}
```

### Teams Webhook Action Configuration

```jinja
{# Teams webhook action parameters #}
{
    "webhook_url": "{{ ORG.VARIABLES.teams_webhook_url }}",
    "message_payload": {{ CTX.teams_card | tojson }}
}
```

### Slack API Action Configuration

```jinja
{# Slack post message parameters #}
{
    "channel": "{{ CTX.slack_channel }}",
    "text": "{{ CTX.fallback_text }}",
    "blocks": {{ CTX.slack_blocks | tojson }},
    "unfurl_links": false,
    "unfurl_media": false
}
```

### Notification Logging

```jinja
{# Log notification for audit/tracking #}
{% set notification = CTX.notification %}
{
    "notification_id": "{{ CTX.notification_id }}",
    "timestamp": "{{ CTX.current_time }}",
    "channel": "{{ notification.channel }}",
    "recipient": "{{ notification.recipient }}",
    "event_type": "{{ notification.event_type }}",
    "reference_type": "{{ notification.reference_type }}",
    "reference_id": "{{ notification.reference_id }}",
    "status": "{{ notification.status }}",
    "organization_id": "{{ CTX.organization_id }}",
    "metadata": {
        "template_used": "{{ notification.template }}",
        "throttled": {{ notification.was_throttled | d(false) | lower }},
        "aggregated": {{ notification.was_aggregated | d(false) | lower }}
    }
}
```

---

## Common Patterns

### Recipient Resolution

```jinja
{# Resolve recipients based on ticket/alert properties #}
{% set item = CTX.item %}
{% set notification_type = CTX.notification_type %}

{% set recipients = [] %}

{# Primary contact #}
{% if item.contact_email %}
    {% set _ = recipients.append(item.contact_email) %}
{% endif %}

{# CC based on notification type #}
{% if notification_type == "escalation" %}
    {# Add manager #}
    {% if item.manager_email %}
        {% set _ = recipients.append(item.manager_email) %}
    {% endif %}
{% elif notification_type == "resolution" %}
    {# Add anyone who was CC'd on ticket #}
    {% for cc in item.cc_contacts | d([]) %}
        {% set _ = recipients.append(cc.email) %}
    {% endfor %}
{% endif %}

{# Org-level notifications #}
{% if item.notify_org_admin and ORG.VARIABLES.admin_email %}
    {% set _ = recipients.append(ORG.VARIABLES.admin_email) %}
{% endif %}

{# Dedupe and filter invalid #}
{{ recipients | unique | reject("none") | list }}
```

### Template Selection

```jinja
{# Select appropriate template based on event #}
{% set event = CTX.event %}

{% set template_map = {
    "ticket_created": {
        "internal": "ticket_created_internal",
        "customer": "ticket_created_customer"
    },
    "ticket_updated": {
        "internal": "ticket_updated_internal",
        "customer": "ticket_updated_customer"
    },
    "ticket_resolved": {
        "internal": "ticket_resolved_internal",
        "customer": "ticket_resolved_customer"
    },
    "alert_triggered": {
        "internal": "alert_triggered_internal",
        "customer": "alert_triggered_customer"
    },
    "user_created": {
        "internal": "user_created_internal",
        "user": "user_welcome",
        "manager": "user_created_manager"
    },
    "password_reset": {
        "user": "password_reset_user"
    },
    "maintenance": {
        "customer": "maintenance_notice"
    }
} %}

{% set event_templates = template_map[event.type] | d({}) %}
{% set template_name = event_templates[event.audience] | d(event.type ~ "_default") %}

{{ template_name }}
```

### Localization Support

```jinja
{# Get localized content based on client language preference #}
{% set language = CTX.client_language | d("en") %}

{% set strings = {
    "en": {
        "ticket_created_subject": "Your Support Request Has Been Received",
        "ticket_updated_subject": "Update on Your Support Request",
        "ticket_resolved_subject": "Your Support Request Has Been Resolved",
        "greeting": "Hello",
        "thank_you": "Thank you for contacting us",
        "regards": "Best regards"
    },
    "es": {
        "ticket_created_subject": "Su Solicitud de Soporte Ha Sido Recibida",
        "ticket_updated_subject": "Actualización de Su Solicitud de Soporte",
        "ticket_resolved_subject": "Su Solicitud de Soporte Ha Sido Resuelta",
        "greeting": "Hola",
        "thank_you": "Gracias por contactarnos",
        "regards": "Saludos cordiales"
    },
    "fr": {
        "ticket_created_subject": "Votre Demande d'Assistance a Été Reçue",
        "ticket_updated_subject": "Mise à Jour de Votre Demande d'Assistance",
        "ticket_resolved_subject": "Votre Demande d'Assistance a Été Résolue",
        "greeting": "Bonjour",
        "thank_you": "Merci de nous avoir contactés",
        "regards": "Cordialement"
    }
} %}

{% set locale = strings[language] | d(strings.en) %}

{# Use in templates #}
{{ locale.greeting }} {{ CTX.contact_name }},

{{ locale.thank_you }}...

{{ locale.regards }},
{{ ORG.VARIABLES.company_name }}
```

---

## Best Practices

### Email Best Practices

1. **Always include plain text version** for email clients that don't render HTML
2. **Use inline styles** - many email clients strip `<style>` tags
3. **Keep subject lines under 50 characters** for mobile
4. **Test with multiple email clients** (Outlook, Gmail, Apple Mail)
5. **Include unsubscribe link** for marketing emails
6. **Don't rely on images** - include alt text and text alternatives

### Teams/Slack Best Practices

1. **Use Adaptive Cards/Block Kit** for rich formatting
2. **Include actionable buttons** when possible
3. **Keep messages concise** - link to details
4. **Use appropriate channels** - don't spam general channels
5. **Implement throttling** for automated messages
6. **Include context** - ticket numbers, timestamps, etc.

### SMS Best Practices

1. **Keep under 160 characters** when possible
2. **Lead with severity/action required**
3. **Include reference numbers**
4. **Reserve for critical/urgent only**
5. **Provide opt-out mechanism**
6. **Test character encoding**

### General Notification Best Practices

1. **Implement throttling** to prevent notification fatigue
2. **Aggregate similar notifications** when appropriate
3. **Log all notifications** for audit trail
4. **Allow channel preferences** per user/client
5. **Have fallback channels** for critical notifications
6. **Test templates** before deploying to production
