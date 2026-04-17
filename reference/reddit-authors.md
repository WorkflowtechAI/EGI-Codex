# Reddit authors worth tracking

From the 59-thread r/msp Rewst scrape (pass 2). These are community voices with multiple Rewst-adjacent posts or outbound content worth profiling.

## High-signal authors

### u/msp4msps
Multi-part Rewst + Power Automate tutorials on YouTube. Three threads cross-posted to r/msp with cumulative upvotes in the 300+ range. The 118-upvote offboarding tutorial (<https://youtu.be/2p9rh7VSCXQ>) is the highest-engagement Rewst-related Reddit post in the dataset.

Action: profile this author. Tutorials are the format EGI wants to learn from for its educational platform. YouTube channel is the next thing to find.

### u/jackmusick
Three Rewst-adjacent threads across 2023-2024:

- "Discussion and questions around Rewst" (2023-10-27, 26 comments): asked "why pay for Rewst vs continuing to self-develop?" — useful framing for EGI pricing and positioning work.
- AutoPilot configuration (2024-05-28).
- Azure Virtual Desktop CPU usage (2024-01-12).

Action: reach out or profile. Recurrent voice, engineering orientation.

### u/Next-Landscape-9884
Four Rewst-curious Reddit threads. In his 2023-08-24 "share your wishlist" thread he mentions a personal GitHub with 1000+ PowerShell/Azure/PowerAutomate scripts, publicly opening a Rewst-inclusive repo. GitHub handle not linked from Reddit profile.

Action: find the GitHub. Potential high-signal contributor.

### u/bibawa
Two Rewst billing-reconciliation threads (Autotask + Datto RMM; Ingram Cloud + Autotask). Classic Rewst use case territory.

Action: profile if active.

## Medium-signal authors

### u/cokebottle22
Two threads framed as an SMB-MSP adopter's early-Q&A. Useful for EGI positioning: exactly the persona EGI targets.

### u/pjustmd
ImmyBot evaluation thread (2024-03) with Rewst framing, and Google Partner API (2024-05). Light Rewst use but relevant adjacency.

### u/rhysfromaussie
CW Manage agreement automation for billing reconciliation (2024-10). Aussie market.

### u/Lime-TeGek
CyberDrain CTF organizer. MSPGeek + CyberDrain operator. Not Rewst-specific but the organizational overlap matters for EGI community work.

## Notable one-offs (don't profile, just cite)

- **u/RepulsiveDuck331** (2026-01-22): "If You're Struggling Post-Rewst Layoffs — Here's Our Journey and What We Switched To." 51 comments. Primary source for a migrating-off-Rewst narrative.
- **u/BryanL38** (2026-01-14): "Significant Layoffs at Rewst." 141 comments. Primary discussion thread for the layoffs event.
- **u/_phat32** (2023-09-26): "Rewst Security Concerns." 42 comments. High-signal security discussion thread.
- **u/ma1ajac1** (2025-11-08): "Any Connectwise MSP's out there that recently (in the past year) implemented REWST?" 48 comments. Feedback pool from recent CW-shop implementers.
- **u/jrmafc12** (2022-09-07): Original 2022 Rewst r/msp discussion thread. 47 comments. Historical context.

## Scrape technique (pass-2 confirmed working)

Public Reddit JSON works without auth:

```bash
curl -sfL -A "Mozilla/5.0 (compatible; research)" \
  "https://www.reddit.com/r/msp/search.json?q=rewst&restrict_sr=1&sort=new&limit=100"
```

Query variants (`rewst+bundle`, `rewst+workflow`, `rewst+github`, `rewst+crate`, `rewst+appbuilder`) returned subsets of or equal-to the base `q=rewst` scrape. Author-scoped queries (`author:etopbrandon`) returned 0 hits. The base query at `limit=100` is the superset for r/msp.

To refresh: re-run quarterly, diff against `scratch/research-pass-2/blogs-and-reddit.md`, pull any new thread URLs into new rows.
