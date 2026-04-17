# Rewst Consulting Playbook

Expert consulting patterns, discovery frameworks, and value demonstration.

---

## Client Discovery Framework

### Initial Assessment Questions

**Current State:**
1. What PSA/RMM/documentation tools are you using?
2. How many technicians? What's their hourly cost?
3. What repetitive tasks consume the most time?
4. What processes cause the most errors/rework?
5. What manual steps exist in your ticketing workflow?

**Pain Points:**
1. Where do tickets get stuck or delayed?
2. What tasks require senior tech time that shouldn't?
3. What information do you repeatedly look up manually?
4. What processes vary between technicians?
5. Where do mistakes happen most often?

**Goals:**
1. What would you automate first if you could?
2. What's your timeline for ROI?
3. What's your automation maturity level?
4. Who will maintain automations long-term?
5. What's your budget for automation tools + consulting?

---

## Automation Opportunity Matrix

### High-Value Quick Wins

| Process | Time Saved | Complexity | ROI |
|---------|------------|------------|-----|
| User onboarding | 2-4 hrs/user | Medium | Very High |
| Password resets | 15-30 min each | Low | High |
| User offboarding | 1-3 hrs/user | Medium | Very High |
| License assignment | 15-30 min each | Low | High |
| Out-of-office setup | 10-15 min each | Low | Medium |
| Ticket triage | 5-10 min/ticket | Low | Very High |
| Group management | 10-20 min each | Low | Medium |

### Medium-Value Projects

| Process | Time Saved | Complexity | ROI |
|---------|------------|------------|-----|
| Security incident response | 1-2 hrs/incident | Medium | High |
| Compliance reporting | 4-8 hrs/month | Medium | Medium |
| Documentation sync | 2-4 hrs/week | Medium | Medium |
| License compliance | 2-4 hrs/month | Medium | Medium |
| Backup verification | 1-2 hrs/day | Low | Medium |

### Strategic Initiatives

| Process | Time Saved | Complexity | ROI |
|---------|------------|------------|-----|
| Full client onboarding | Days | High | Very High |
| vCIO reporting | 4-8 hrs/month | High | Medium |
| Multi-tenant management | Varies | High | High |
| Security posture automation | Varies | High | High |

---

## Scoping & Estimation

### Discovery Phase Checklist

```
□ Identify all integrations needed
□ Map current manual process steps
□ Document decision points and exceptions
□ Identify data sources and destinations
□ List stakeholders and approvers
□ Define success criteria
□ Estimate volume/frequency
□ Identify testing requirements
```

### Complexity Scoring

| Factor | Low (1) | Medium (2) | High (3) |
|--------|---------|------------|----------|
| Integrations | 1-2 | 3-4 | 5+ |
| Decision points | 1-2 | 3-5 | 6+ |
| Exception handling | Minimal | Moderate | Complex |
| Approvals needed | None | 1 level | Multi-level |
| Data transformation | Simple | Moderate | Complex |
| Error handling | Standard | Custom | Advanced |

**Total Score:**
- 6-8: Simple (~4-8 hours)
- 9-12: Moderate (~8-16 hours)
- 13-15: Complex (~16-32 hours)
- 16+: Advanced (~32+ hours)

---

## Pricing Models

### Hourly Consulting

| Tier | Rate Range | Typical Tasks |
|------|------------|---------------|
| Basic | $150-200/hr | Simple automations, training |
| Advanced | $200-275/hr | Complex workflows, integrations |
| Expert | $275-400/hr | Architecture, optimization, custom dev |

### Project-Based

| Project Type | Typical Range |
|--------------|---------------|
| Simple automation | $1,000-2,500 |
| Standard workflow | $2,500-5,000 |
| Complex integration | $5,000-10,000 |
| Enterprise solution | $10,000-25,000+ |
| Full platform setup | $15,000-50,000+ |

### Managed Automation Services

| Tier | Monthly Fee | Includes |
|------|-------------|----------|
| Basic | $500-1,000 | Monitoring, minor fixes, 2 hrs support |
| Standard | $1,000-2,000 | Above + 1 automation/month, 5 hrs |
| Premium | $2,000-4,000 | Above + priority support, 10 hrs |
| Enterprise | $4,000-8,000+ | Dedicated support, unlimited changes |

---

## ROI Calculations

### Time Savings Formula

```
Annual Savings = (Manual Time) x (Frequency) x (Hourly Rate) x 52

Example:
- User onboarding takes 3 hours manually
- 4 onboardings per week average
- Tech hourly cost: $75

Annual Savings = 3 x 4 x $75 x 52 = $46,800
```

### Common Benchmarks

| Automation | Manual Time | Automated Time | Savings |
|------------|-------------|----------------|---------|
| User onboarding | 2-4 hours | 10-30 min | 85-95% |
| User offboarding | 1-3 hours | 5-15 min | 90-95% |
| Password reset | 15-30 min | 2-5 min | 85-90% |
| Ticket triage | 5-10 min | 0-2 min | 80-100% |
| License audit | 4-8 hours | 15-30 min | 95% |
| Backup check | 30-60 min/day | 5 min | 90% |

### Building the Business Case

```markdown
## Automation ROI Summary

**Current State:**
- Process: User Onboarding
- Manual time: 3 hours average
- Frequency: 50 users/month
- Tech cost: $75/hour
- Annual manual cost: $135,000

**Automated State:**
- Automated time: 15 minutes average
- Annual automated cost: $11,250
- Net annual savings: $123,750

**Investment:**
- Initial build: $8,000
- Annual maintenance: $2,400

**ROI:**
- First year savings: $113,350
- ROI: 1,417%
- Payback period: < 1 month
```

---

## Implementation Approach

### Phase 1: Foundation (Week 1-2)

```
□ Configure Rewst instance
□ Set up integrations
□ Define org variables
□ Create naming conventions
□ Set up testing org
□ Document architecture
```

### Phase 2: Quick Wins (Week 2-4)

```
□ Deploy 2-3 high-value crates
□ Customize for client needs
□ Test with pilot group
□ Document procedures
□ Train first users
□ Measure initial results
```

### Phase 3: Custom Development (Week 4-8)

```
□ Build client-specific automations
□ Create custom forms
□ Implement approval workflows
□ Set up reporting
□ Comprehensive testing
□ Full deployment
```

### Phase 4: Optimization (Ongoing)

```
□ Monitor execution success rates
□ Gather user feedback
□ Optimize performance
□ Add error handling
□ Extend functionality
□ Regular reviews
```

---

## Common Pitfalls to Avoid

### Technical Pitfalls

| Pitfall | Prevention |
|---------|------------|
| Hardcoded values | Always use org variables |
| No error handling | Add failure paths |
| Ignoring rate limits | Implement delays/batching |
| Missing logging | Add debug tasks |
| Over-engineering | Start simple, iterate |
| No testing | Use test org first |

### Project Pitfalls

| Pitfall | Prevention |
|---------|------------|
| Scope creep | Document requirements upfront |
| Unclear ownership | Assign maintainers |
| No documentation | Document as you build |
| Skipping training | Budget for knowledge transfer |
| Ignoring edge cases | Map exceptions early |
| Rushing deployment | Test thoroughly |

---

## Client Training Framework

### Level 1: End Users (1-2 hours)

- Submitting forms
- Understanding automation status
- What to expect (timing, notifications)
- How to report issues

### Level 2: Power Users (4-6 hours)

- Navigating Rewst interface
- Viewing execution results
- Understanding workflow basics
- Troubleshooting common issues
- Managing org variables

### Level 3: Administrators (8-12 hours)

- Building simple workflows
- Creating forms
- Setting up triggers
- Managing integrations
- Basic Jinja concepts
- Troubleshooting workflows

### Level 4: Developers (16-24 hours)

- Advanced Jinja patterns
- Complex workflow design
- API integrations
- Custom development
- Performance optimization
- Best practices

---

## Documentation Templates

### Automation Runbook Template

```markdown
# [Automation Name]

## Overview
[Brief description of what this automation does]

## Trigger
[How the automation is initiated]

## Inputs
| Field | Type | Required | Description |
|-------|------|----------|-------------|

## Process Flow
1. [Step 1]
2. [Step 2]
...

## Outputs
| Field | Description |
|-------|-------------|

## Error Handling
| Error | Cause | Resolution |
|-------|-------|------------|

## Dependencies
- Integration: [List]
- Org Variables: [List]

## Maintenance
- Owner: [Name]
- Review frequency: [Monthly/Quarterly]
- Last updated: [Date]
```

### Project Handoff Document

```markdown
# [Project Name] Handoff

## Deliverables
- [ ] [Workflow 1]
- [ ] [Workflow 2]
- [ ] [Form 1]

## Configuration
- Org variables configured: [Yes/No]
- Triggers enabled: [Yes/No]
- Testing completed: [Yes/No]

## Training
- [ ] Admin training completed
- [ ] User documentation provided
- [ ] Support process established

## Support
- Support contact: [Name]
- SLA: [Response time]
- Escalation path: [Process]

## Next Steps
1. [Recommendation 1]
2. [Recommendation 2]
```

---

## Value Demonstration

### Monthly Report Template

```markdown
# Automation Value Report - [Month Year]

## Executive Summary
Total time saved: [X] hours
Estimated value: $[X]
Automations executed: [X]

## Top Performing Automations
| Automation | Executions | Time Saved | Value |
|------------|------------|------------|-------|

## Success Rate
Overall: [X]%
By automation:
- [Name]: [X]%

## Issues Resolved
- [Issue 1]: [Resolution]

## Recommendations
- [Recommendation 1]

## Next Month Focus
- [Priority 1]
```

### Quarterly Business Review Agenda

```
1. Review automation performance metrics
2. Discuss ROI vs. projections
3. Identify new automation opportunities
4. Review roadmap priorities
5. Discuss training needs
6. Plan next quarter initiatives
```

---

## Competitive Differentiation

### Why Rewst Over Scripts

| Factor | Scripts | Rewst |
|--------|---------|-------|
| Visibility | Limited | Full execution tracking |
| Maintenance | Developer required | GUI-based |
| Multi-tenant | Complex | Built-in |
| Audit trail | Manual | Automatic |
| Error handling | Custom | Standardized |
| Integrations | Build each | Pre-built |

### Why Rewst Over Competitors

| Factor | Rewst | Others |
|--------|-------|--------|
| MSP focus | Purpose-built | Generic |
| Multi-tenant | Native | Limited/complex |
| Marketplace | MSP-specific crates | Generic templates |
| Community | Active MSP community | Varied |
| Learning curve | Moderate | Often steeper |
| PSA/RMM integrations | Deep, native | Basic/middleware |

---

## Expert Positioning

### Building Credibility

1. **Get certified** - Complete Cluck University
2. **Share knowledge** - Blog, community participation
3. **Specialize** - Pick 2-3 verticals
4. **Document wins** - Case studies with metrics
5. **Stay current** - Attend ROC calls, follow updates

### Service Offerings Menu

| Service | Description | Typical Price |
|---------|-------------|---------------|
| Assessment | 2-4 hr discovery + recommendations | $500-1,500 |
| Quick Start | 3-5 automations + training | $3,000-8,000 |
| Custom Project | Scoped automation work | $2,500-25,000 |
| Managed Services | Ongoing support + development | $1,000-5,000/mo |
| Training | On-site or virtual sessions | $1,500-3,000/day |
| Optimization | Performance + best practices | $2,000-5,000 |
