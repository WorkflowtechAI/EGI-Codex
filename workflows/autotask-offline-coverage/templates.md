# Notification Templates

Create these in Admin › Automation › Notification Templates › Tickets before building the rules. Text in `[brackets]` is a variable: insert it with the template editor's Variables picker, so Autotask fills it from the ticket. Text in `<angle brackets>` is a value you type once, such as your hours or urgent number.

Client-facing templates (`TPL-ACK`, `TPL-P1-ACK`, `TPL-WC-*`) go out under the HelpDesk mailbox address, so replies thread back onto the ticket.

## TPL-ACK (OC-01)

**Subject:** We've got it: [Ticket Number] [Ticket Title]

> Hi [Contact First Name],
>
> We've received your request and logged it as ticket [Ticket Number]. A technician will pick it up and you'll hear from us with an update.
>
> HelpDesk hours are <Mon–Fri, 8:00 a.m.–5:00 p.m. PT>. If work has stopped, email <urgent@msp-domain> or call <urgent number> and our on-call technician will respond.
>
> Reply to this email to add details to your ticket.
>
> <MSP name> HelpDesk

## TPL-P1-ACK (OC-02b)

**Subject:** Urgent request received: [Ticket Number]

> Hi [Contact First Name],
>
> We've received your urgent request, ticket [Ticket Number], and alerted our on-call technician. Expect a call or email within <15 minutes>.
>
> If you can, reply with any error messages, the number of people affected, and the best phone number to reach you.
>
> <MSP name> HelpDesk

## TPL-P1-INTERNAL (OC-02a)

**Subject:** CRITICAL [Ticket Number] [Account Name]: [Ticket Title]

> Critical ticket opened.
>
> Client: [Account Name]
> Contact: [Contact Name], [Contact Phone]
> Ticket: [Ticket Number] [Ticket Title]
> Description: [Ticket Description]
>
> On-call: acknowledge in the team channel and call the contact within 15 minutes.

## TPL-UNTOUCHED (OC-03)

**Subject:** Unpicked [Priority] ticket: [Ticket Number] [Account Name]

> [Ticket Number] ([Priority]) has sat in New for over an hour.
>
> Client: [Account Name]. Contact: [Contact Name], [Contact Phone].
> [Ticket Title]
>
> Take it, or post in the team channel who will.

## TPL-SLA-WARN (OC-04)

**Subject:** First response due in 30 min: [Ticket Number]

> [Ticket Number] for [Account Name] needs a first response by [First Response Due Date/Time].
>
> A short client note counts. Use the acknowledgment or progress update template.

## TPL-SLA-BREACH (OC-05a, OC-05b)

**Subject:** SLA missed: [Ticket Number] [Account Name]

> [Ticket Number] ([Priority]) has missed its SLA target.
>
> Client: [Account Name]
> Primary resource: [Primary Resource]
> Status: [Status]
> Last activity: [Last Activity Date/Time]
>
> HelpDesk manager: contact the client today with a dated next step.

## TPL-STALL (OC-06)

**Subject:** No activity for 2 days: [Ticket Number]

> [Ticket Number] for [Account Name] has had no activity for 48 hours.
>
> Add a note with the next step and its date, update the client, or move the ticket to the status that matches reality.

## TPL-WC-REMIND (OC-07)

**Subject:** Checking in on [Ticket Number] [Ticket Title]

> Hi [Contact First Name],
>
> We're waiting on a reply from you to keep ticket [Ticket Number] moving. Reply to this email with the details we asked for, or a good time to connect.
>
> If this is already resolved, no action is needed. We'll close the ticket in 4 days.
>
> <MSP name> HelpDesk

## TPL-WC-CLOSE (OC-08)

**Subject:** Closed: [Ticket Number] [Ticket Title]

> Hi [Contact First Name],
>
> We haven't heard back, so we've closed ticket [Ticket Number]. If you still need help, reply to this email and we'll reopen it right away.
>
> <MSP name> HelpDesk

## TPL-REPLY (OC-09)

**Subject:** Client replied: [Ticket Number] [Account Name]

> [Contact Name] replied on [Ticket Number], which was Waiting Customer or Complete. The ticket is now Customer Note Added.
>
> Read the note and respond to the client today.
