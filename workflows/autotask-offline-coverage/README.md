# Autotask Offline-Coverage Rule Pack

Native Autotask workflow rules that keep a HelpDesk moving while the dispatcher is offline. Built for part-time HelpDesk Management engagements, where one person covers the peak window and the rules cover the rest of the business day.

EGI-authored. No Rewst required: everything here is Autotask PSA configuration.

## What the pack covers

| Rule | Catches | Fires on | Who hears about it |
| --- | --- | --- | --- |
| OC-01 Acknowledge new tickets | Client doesn't know the request landed | Ticket created | Ticket contact |
| OC-02a Critical intake, internal alert | Critical issue outside dispatcher hours | Ticket created at Critical | On-call group, owner, team channel |
| OC-02b Critical intake, client acknowledgment | Client with an outage hears nothing | Ticket created at Critical | Ticket contact |
| OC-03 Untouched urgent ticket | Critical/High ticket still in New | 1 hr since last activity | On-call group, HelpDesk manager |
| OC-04 First-response warning | First-response SLA about to slip | 0.5 hr before First Response Due | Primary resource, HelpDesk manager |
| OC-05a First-response breach | First-response SLA missed | At First Response Due | HelpDesk manager, owner |
| OC-05b Resolution breach | Resolved SLA missed | At Resolved Due | HelpDesk manager, owner, primary resource |
| OC-06 Stalled ticket | Open ticket with no activity | 48 hr since last activity | Primary resource, HelpDesk manager |
| OC-07 Waiting Customer reminder | Client hasn't answered | 72 hr in Waiting Customer | Ticket contact |
| OC-08 Waiting Customer close | Client never answered | 168 hr in Waiting Customer | Ticket contact (status set to Complete) |
| OC-09 Client reply reopens | Client replies to a waiting or closed ticket | Ticket edited by contact | Primary resource (status set to Customer Note Added) |

Companion, not a rule: the 7:30 a.m. digest (see [Morning digest](#morning-digest)).

## Prerequisites

Set these up first. The rules reference them by name.

1. **On-call distribution group** (`oncall@<msp-domain>`) in Microsoft 365 or Google Workspace. Point it at whoever is on call each week. The rules never change; only the group membership does.
2. **Urgent intake mailbox** (`urgent@<msp-domain>`), published to clients as the address for work-stopping issues.
3. **Second Incoming Email Processing mailbox** for `urgent@`, with default Priority = Critical and default Queue = your HelpDesk queue. Path: Admin › Features & Settings › Service Desk (Tickets) › Incoming Email Processing.
4. **Team channel email address** (Teams: channel › Get email address; Slack: channel email integration) so alerts land where techs already look.
5. **SLAs** defined per priority and attached to each client contract, or set as the default SLA. Path: Admin › Features & Settings › Service Desk (Tickets) › Service Level Agreements. Without them, OC-04 and OC-05 never fire.
6. **Statuses in use:** New, In Progress, Waiting Customer, Customer Note Added, Complete. These are Autotask defaults. If your instance renamed them, substitute your names throughout.
7. **Notification templates** from [`templates.md`](templates.md), created before the rules. Path: Admin › Automation › Notification Templates › Tickets.

## Where rules live

Admin › Automation › Workflow Rules › New. Older menus: Admin › Features & Settings › Service Desk (Tickets) › Ticket Workflow Rules.

Each rule has four parts on the form: **General** (name, active, description), **Events**, **Conditions**, **Actions** (updates and notifications). Name every rule with its `OC-` code so the set sorts together and is easy to audit.

Autotask checks time-based events on a recurring cycle, so time-based rules (OC-03 to OC-08) fire shortly after the threshold, not on the exact minute. Time-based thresholds count clock hours, including nights and weekends.

## Rule specs

Throughout, `HelpDesk queue` = the queue client tickets land in, `HelpDesk manager` = the dispatcher's resource account, and `Owner` = the MSP principal's resource account.

### OC-01 Acknowledge new tickets

- **Event:** Created by: Anyone
- **Conditions:**
  - Queue = HelpDesk queue
  - Priority ≠ Critical (OC-02b handles those)
  - Source = Email or Client Portal
- **Actions:** Notify Contact. Template: `TPL-ACK`.

### OC-02a Critical intake, internal alert

- **Event:** Created by: Anyone
- **Conditions:** Priority = Critical
- **Actions:**
  - Notify Other Resources: Owner, HelpDesk manager
  - Other email addresses: `oncall@`, team channel email
  - Template: `TPL-P1-INTERNAL`

Tickets a tech raises to Critical after creation don't trigger this rule. Whoever raises the priority posts in the team channel at the same time.

### OC-02b Critical intake, client acknowledgment

- **Event:** Created by: Anyone
- **Conditions:** Priority = Critical
- **Actions:** Notify Contact. Template: `TPL-P1-ACK`.

### OC-03 Untouched urgent ticket

- **Event:** Time-based: Last Activity, 1 hour after
- **Conditions:**
  - Status = New
  - Priority = Critical or High
  - Queue = HelpDesk queue
- **Actions:**
  - Notify Other Resources: HelpDesk manager
  - Other email addresses: `oncall@`
  - Template: `TPL-UNTOUCHED`

Medium and Low tickets wait for the morning digest, so on-call isn't woken for a printer.

### OC-04 First-response warning

- **Event:** Time-based: First Response Due, 0.5 hour before
- **Conditions:** Status = New
- **Actions:**
  - Notify Primary Resource; Other Resources: HelpDesk manager
  - Template: `TPL-SLA-WARN`

### OC-05a First-response breach

- **Event:** Time-based: First Response Due, 0 hours after
- **Conditions:** Status = New
- **Actions:**
  - Notify Other Resources: HelpDesk manager, Owner
  - Template: `TPL-SLA-BREACH`

### OC-05b Resolution breach

- **Event:** Time-based: Resolved Due, 0 hours after
- **Conditions:** Status ≠ Complete
- **Actions:**
  - Notify Primary Resource; Other Resources: HelpDesk manager, Owner
  - Template: `TPL-SLA-BREACH`

### OC-06 Stalled ticket

- **Event:** Time-based: Last Activity, 48 hours after
- **Conditions:**
  - Status = In Progress or Customer Note Added
  - Queue = HelpDesk queue
- **Actions:**
  - Notify Primary Resource; Other Resources: HelpDesk manager
  - Template: `TPL-STALL`

Leave Scheduled, Waiting Materials and Waiting Vendor out of the condition. Those tickets are parked on purpose.

### OC-07 Waiting Customer reminder

- **Event:** Time-based: Last Activity, 72 hours after
- **Conditions:** Status = Waiting Customer
- **Actions:** Notify Contact. Template: `TPL-WC-REMIND`.

### OC-08 Waiting Customer close

- **Event:** Time-based: Last Activity, 168 hours after
- **Conditions:** Status = Waiting Customer
- **Actions:**
  - Update Status = Complete
  - Notify Contact. Template: `TPL-WC-CLOSE`.

### OC-09 Client reply reopens

- **Event:** Edited by: Contact
- **Conditions:** Status = Waiting Customer or Complete
- **Actions:**
  - Update Status = Customer Note Added
  - Notify Primary Resource. Template: `TPL-REPLY`.

OC-09 is what makes OC-08 safe to run: a client can always reply to reopen the ticket.

## Morning digest

The digest isn't a workflow rule. It's a saved ticket view, emailed or opened at 7:30 a.m.:

- **Created since 1:00 p.m. yesterday:** a ticket search filtered by Create Date, Queue = HelpDesk queue.
- **SLA breached or due today:** a ticket search filtered on First Response Due and Resolved Due.
- **Stalled:** Last Activity more than 48 hours ago, status In Progress.
- **Unassigned:** Primary Resource is empty.

Build these as saved searches, then schedule a LiveReport for email delivery, or put the searches on a HelpDesk dashboard.

## Build order

1. Prerequisites 1–7.
2. OC-01, OC-02a, OC-02b. These give immediate client-visible value.
3. OC-09, then OC-07 and OC-08. Always build the reopen path before the auto-close.
4. OC-03 and OC-06.
5. OC-04 and OC-05a/b, once SLAs are attached to contracts.
6. Morning digest.

## Test plan

Use an internal test account and contact (for example, the MSP's own company in Autotask) so clients never see a test notification.

| Rule | Test | Pass when |
| --- | --- | --- |
| OC-01 | Email the HelpDesk mailbox from the test contact | Acknowledgment arrives with the ticket number |
| OC-02a/b | Email `urgent@` from the test contact | Ticket opens at Critical; on-call, owner and channel get the alert; the contact gets the Critical acknowledgment |
| OC-03 | Create a High ticket, leave it in New | On-call alert after about 1 hour |
| OC-04/05 | Attach a test SLA with a short first-response target, then leave the ticket in New | Warning, then breach alert |
| OC-06 | Temporarily set the threshold to 0.25 hours on an In Progress ticket | Primary resource alerted; then reset to 48 |
| OC-07/08 | Temporarily set the thresholds to 0.25 and 0.5 hours | Reminder, then Complete status with the closure notice; then reset to 72 and 168 |
| OC-09 | Reply to the closure email from the test contact | Status returns to Customer Note Added; primary resource alerted |

Disable every rule except the one under test while testing, then re-enable the full set.

## Tuning after 2 weeks

- **OC-03 too noisy:** raise the threshold to 2 hours, or narrow it to Critical only.
- **OC-06 too noisy:** 48 hours suits a small team. Move to 72 if the techs say most alerts are tickets they're already working.
- **Clients complain about auto-close:** extend OC-08 to 240 hours (10 days).
- **Alert fatigue overall:** route OC-04 and OC-06 to the team channel only, and keep email for OC-02a and OC-05.
