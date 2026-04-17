<p align="center">
  <img src="assets/logicmonitor-logo.svg" alt="LogicMonitor" width="300">
</p>

# LogicMonitor OpenAPI Specification

[![OpenAPI 3.0](https://img.shields.io/badge/OpenAPI-3.0-green.svg)](https://www.openapis.org/)
[![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg)](LICENSE)

A complete, fully compliant OpenAPI 3.0 specification for the [LogicMonitor](https://www.logicmonitor.com/) REST API v3.

## Overview

This repository contains an enhanced OpenAPI specification for LogicMonitor's monitoring platform API. The spec has been enriched with:

- Full parameter descriptions for all 856 parameters
- Schema descriptions for all 673 data models
- Response examples for all endpoints
- Standardized error responses (400, 401, 403, 404, 429, 500)
- Proper server URL templating for multi-tenant access
- Both LMv1 signature and Bearer token authentication

## Rewst Custom Integration v2

<img src="assets/rewst-icon.svg" alt="Rewst" width="32" align="left" style="margin-right: 10px;">

Two Rewst-compatible specs are available for [Rewst Custom Integration v2](https://docs.rewst.io/):

| Version | File | Operations | Size | Best For |
|---------|------|------------|------|----------|
| **Standard** | `logicmonitor-rewst.json` | 64 | 124KB | Common monitoring workflows |
| **Advanced** | `logicmonitor-rewst-advanced.json` | 177 | 472KB | Full MSP management with RBAC, audit, config |

**Start with Standard** unless you need advanced features like role management, API token lifecycle, or configuration backup operations.

### Why Separate Specs?

The full LogicMonitor OpenAPI spec (353 operations, ~2MB) exceeds Custom Integration v2's size limits and uses LMv1 HMAC authentication which isn't supported. Both Rewst versions use:

- **Bearer token auth** instead of LMv1 signatures
- **Server variables** for multi-tenant configuration
- **Compact size** optimized for Rewst's limits

### Quick Setup

1. **Generate a Bearer Token** in LogicMonitor:
   - Navigate to **Settings → Users → API Tokens**
   - Create a new token and copy the value

2. **Import into Rewst:**
   - Go to **Configuration → Integrations → Custom Integrations**
   - Click **Add Custom Integration**
   - Upload `logicmonitor-rewst.json`
   - Configure the `company` variable (your LogicMonitor subdomain)
   - Enter your Bearer token

### Included Endpoints

| Category | Operations | Count |
|----------|------------|-------|
| **Devices** | List, Get, Create, Update, Delete, Get Properties | 6 |
| **Device Groups** | List, Get, Create, Update, Delete, Get Devices | 6 |
| **Alerts** | List, Get, Acknowledge, Add Note | 4 |
| **Collectors** | List, Get | 2 |
| **Collector Groups** | List, Get | 2 |
| **SDTs** | List, Get, Create, Update, Delete | 5 |
| **Dashboards** | List, Get | 2 |
| **Dashboard Groups** | List, Get | 2 |
| **Admins** | List, Get | 2 |
| **Websites** | List, Get, Create, Update, Delete | 5 |
| **Website Groups** | List, Get, Create, Update, Delete | 5 |
| **Reports** | List, Get, Generate | 3 |
| **Datasources** | List, Get, List Device Datasources | 3 |
| **Datasource Instances** | List Instances, Get Instance Data | 2 |
| **Alert Rules** | List, Get | 2 |
| **Escalation Chains** | List, Get | 2 |
| **Netscans** | List, Get | 2 |
| **OpsNotes** | List, Get, Create, Update, Delete | 5 |
| | **Total** | **64** |

### Available Specs

| File | Operations | Size | Use Case |
|------|------------|------|----------|
| `logicmonitor-rewst.json` | 64 | 124KB | Standard monitoring workflows |
| `logicmonitor-rewst-advanced.json` | 177 | 472KB | Advanced MSP with RBAC, audit, config management |

---

## Advanced Spec Details

The advanced spec (`logicmonitor-rewst-advanced.json`) includes everything in the standard spec plus 113 additional operations for comprehensive MSP management.

### Additional Categories in Advanced Spec

| Category | Operations | Description |
|----------|------------|-------------|
| **Roles** | List, Get, Create, Update, Delete, CRUD | Role-based access control |
| **API Tokens** | List all, List by admin, Get, Create, Delete | Token lifecycle management |
| **Access Groups** | List, Get, Create, Update, Add, Map/Unmap | Resource access control |
| **Access Logs** | List, Get | Audit trail and compliance |
| **Recipient Groups** | List, Get, Create, Update, Delete | Alert routing configuration |
| **ConfigSources** | List, Get, Update reasons | Configuration backup management |
| **EventSources** | List, Get | Event monitoring definitions |
| **PropertyRules** | List, Get | Property assignment automation |
| **LogSources** | List, Get | Log ingestion configuration |
| **TopologySources** | List, Get | Network topology mapping |
| **Widgets** | List, Get, Create, Update, Delete, Get Data | Dashboard widget management |
| **Report Groups** | List, Get, Create, Update, Delete | Report organization |
| **Log Partitions** | List, Get, Update, Actions, Retentions | Log storage management |
| **Log Query Groups** | List, Get, Create, Update, Delete, Move | Saved log queries |
| **Batch Jobs** | List, Get | Batch job monitoring |
| **AppliesToFunctions** | List, Get, Create, Update, Delete | AppliesTo helper functions |
| **OIDs** | List, Get | SNMP OID management |
| **Collector Management** | Ack down, Installers, Versions | Advanced collector ops |
| **Device Properties** | Get/Set individual properties | Granular property management |
| **Device Group Properties** | List, Get, Create, Update, Delete | Group-level properties |
| **SDT History** | Device, Group, Website SDT history | Historical SDT tracking |
| **Resource SDTs** | SDTs per device, group, website | Resource-specific SDT management |
| **Alert Settings** | Device, Instance, Group alert config | Threshold customization |
| **Instance Config** | Config collection, diff, history | ConfigSource instance data |
| **Device Datasource Groups** | Instance grouping and data | Datasource organization |
| **Unmonitored Devices** | List discovered but unmonitored | Discovery management |
| **Device Delta** | Change tracking | Device change detection |
| **Cost Optimization** | Recommendations, Categories | Cost management |
| **Usage Metrics** | Usage, Summary, Contract info | License and capacity |
| **Graph Data** | Instance, widget, website graphs | Data visualization |
| **Diagnostics** | Diagnostic sources | Troubleshooting tools |
| **Integration Audit** | Integration audit logs | Integration monitoring |

---

## Files

| File | Description |
|------|-------------|
| `openapi.json` | OpenAPI 3.0 specification (JSON format) |
| `openapi.yaml` | OpenAPI 3.0 specification (YAML format) |
| `logicmonitor-rewst.json` | Rewst CI v2 spec - standard (64 operations) |
| `logicmonitor-rewst-advanced.json` | Rewst CI v2 spec - advanced MSP (177 operations) |
| `source/` | Original specifications from LogicMonitor |

## Quick Start

### Using with API Clients

**Postman:**
```
Import > Link > https://raw.githubusercontent.com/tim4net/logicmonitor-openapi/main/openapi.json
```

**Insomnia:**
```
Import/Export > Import Data > From URL
```

### Code Generation

Generate client libraries using [OpenAPI Generator](https://openapi-generator.tech/):

```bash
# Python
openapi-generator-cli generate -i openapi.yaml -g python -o ./clients/python

# TypeScript
openapi-generator-cli generate -i openapi.yaml -g typescript-axios -o ./clients/typescript

# Go
openapi-generator-cli generate -i openapi.yaml -g go -o ./clients/go
```

## API Statistics

| Metric | Count |
|--------|-------|
| Total Endpoints | 353 |
| GET endpoints | 151 |
| POST endpoints | 77 |
| PUT endpoints | 44 |
| PATCH endpoints | 43 |
| DELETE endpoints | 38 |
| Data Models | 673 |

## API Categories

The API covers these major areas:

- **Monitoring**: Devices, Device Groups, Collectors, Datasources
- **Alerting**: Alerts, Alert Rules, Escalation Chains
- **Visualization**: Dashboards, Widgets, Reports
- **Configuration**: ConfigSources, EventSources, PropertySources
- **Web Monitoring**: Websites, Website Groups
- **Administration**: Users, Roles, API Tokens, Audit Logs
- **Scheduling**: SDTs (Scheduled Downtime)
- **Discovery**: Netscans, Auto-discovery

## Authentication

The API supports two authentication methods:

### LMv1 Token Authentication (Recommended)

Uses HMAC-SHA256 signature-based authentication:

```
Authorization: LMv1 AccessId:Signature:Timestamp
```

### Bearer Token Authentication

JWT-based authentication:

```
Authorization: Bearer <token>
```

## Server URL

The API uses a multi-tenant URL pattern:

```
https://{company}.logicmonitor.com/santaba/rest
```

Replace `{company}` with your LogicMonitor account name.

## Contributing

Contributions are welcome! Please see [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines.

### Areas for Improvement

- [ ] Add more detailed response examples from live API
- [ ] Document rate limiting behavior
- [ ] Add webhook payload schemas
- [ ] Include x-code-samples for common operations

## License

This project is licensed under the Apache License 2.0 - see the [LICENSE](LICENSE) file for details.

## Disclaimer

This is an unofficial, community-maintained specification. LogicMonitor is a trademark of LogicMonitor, Inc. This project is not affiliated with or endorsed by LogicMonitor, Inc.

## Resources

- [LogicMonitor Official Documentation](https://www.logicmonitor.com/support)
- [LogicMonitor REST API v3 Docs](https://www.logicmonitor.com/support/rest-api-v3-swagger-documentation)
- [LogicMonitor Community](https://community.logicmonitor.com/)
