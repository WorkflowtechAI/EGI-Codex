# Network & Firewall Automation

Comprehensive patterns for network device management, firewall automation, VPN configuration, and multi-vendor network infrastructure.

---

## Vendor Integration Overview

| Vendor | API Type | Authentication | Common Use Cases |
|--------|----------|----------------|------------------|
| Fortinet/FortiGate | REST API | API Key | Firewall rules, VPN, UTM |
| Cisco Meraki | REST API | API Key | SD-WAN, wireless, cameras |
| SonicWall | REST API | API Key | Firewall, VPN, content filtering |
| Ubiquiti UniFi | REST API | Session/API Key | Network, wireless, cameras |
| Palo Alto | REST/XML API | API Key | NGFW, threat prevention |
| pfSense | REST API (pkg) | API Key/Token | Open-source firewall |
| WatchGuard | REST API | API Key | UTM, VPN, wireless |

---

## Fortinet/FortiGate Patterns

### API Authentication

```jinja
{# FortiGate API authentication header #}
{% set fortigate_host = ORG.VARIABLES.fortigate_host %}
{% set api_key = ORG.VARIABLES.fortigate_api_key %}

{# Headers for FortiGate API requests #}
{
    "Authorization": "Bearer {{ api_key }}",
    "Content-Type": "application/json"
}
```

### Get Firewall Policies

```jinja
{# GET /api/v2/cmdb/firewall/policy #}
{# Response processing #}
{% set policies = TASKS.get_policies.result.result.data.results | d([]) %}

{% set policy_summary = [] %}
{% for policy in policies %}
    {% set _ = policy_summary.append({
        "id": policy.policyid,
        "name": policy.name,
        "srcintf": policy.srcintf | map(attribute="name") | list,
        "dstintf": policy.dstintf | map(attribute="name") | list,
        "srcaddr": policy.srcaddr | map(attribute="name") | list,
        "dstaddr": policy.dstaddr | map(attribute="name") | list,
        "service": policy.service | map(attribute="name") | list,
        "action": policy.action,
        "status": policy.status,
        "logtraffic": policy.logtraffic,
        "comments": policy.comments | d("")
    }) %}
{% endfor %}

{{ policy_summary }}
```

### Create Firewall Policy

```jinja
{# POST /api/v2/cmdb/firewall/policy #}
{% set policy = CTX.new_policy %}
{
    "name": "{{ policy.name }}",
    "srcintf": [
        {% for intf in policy.source_interfaces %}
        {"name": "{{ intf }}"}{{ "," if not loop.last else "" }}
        {% endfor %}
    ],
    "dstintf": [
        {% for intf in policy.dest_interfaces %}
        {"name": "{{ intf }}"}{{ "," if not loop.last else "" }}
        {% endfor %}
    ],
    "srcaddr": [
        {% for addr in policy.source_addresses %}
        {"name": "{{ addr }}"}{{ "," if not loop.last else "" }}
        {% endfor %}
    ],
    "dstaddr": [
        {% for addr in policy.dest_addresses %}
        {"name": "{{ addr }}"}{{ "," if not loop.last else "" }}
        {% endfor %}
    ],
    "service": [
        {% for svc in policy.services %}
        {"name": "{{ svc }}"}{{ "," if not loop.last else "" }}
        {% endfor %}
    ],
    "action": "{{ policy.action | d('accept') }}",
    "status": "{{ policy.status | d('enable') }}",
    "schedule": "{{ policy.schedule | d('always') }}",
    "logtraffic": "{{ policy.log_traffic | d('utm') }}",
    "comments": "{{ policy.comments | d('Created by Rewst automation') }}",
    "nat": "{{ policy.nat | d('disable') }}",
    "inspection-mode": "{{ policy.inspection_mode | d('flow') }}",
    "utm-status": "{{ 'enable' if policy.enable_utm else 'disable' }}",
    {% if policy.enable_utm %}
    "av-profile": "{{ policy.av_profile | d('default') }}",
    "webfilter-profile": "{{ policy.webfilter_profile | d('default') }}",
    "ips-sensor": "{{ policy.ips_sensor | d('default') }}",
    {% endif %}
    "ssl-ssh-profile": "{{ policy.ssl_profile | d('certificate-inspection') }}"
}
```

### Create Address Object

```jinja
{# POST /api/v2/cmdb/firewall/address #}
{% set address = CTX.new_address %}
{
    "name": "{{ address.name }}",
    {% if address.type == "subnet" %}
    "type": "ipmask",
    "subnet": "{{ address.subnet }}",
    {% elif address.type == "range" %}
    "type": "iprange",
    "start-ip": "{{ address.start_ip }}",
    "end-ip": "{{ address.end_ip }}",
    {% elif address.type == "fqdn" %}
    "type": "fqdn",
    "fqdn": "{{ address.fqdn }}",
    {% endif %}
    "associated-interface": "{{ address.interface | d('any') }}",
    "comment": "{{ address.comment | d('Created by Rewst') }}"
}
```

### VPN User Management

```jinja
{# Create SSL VPN user #}
{# POST /api/v2/cmdb/user/local #}
{% set vpn_user = CTX.vpn_user %}
{
    "name": "{{ vpn_user.username }}",
    "type": "password",
    "passwd": "{{ vpn_user.password }}",
    "email-to": "{{ vpn_user.email }}",
    "two-factor": "{{ 'fortitoken' if vpn_user.enable_mfa else 'disable' }}",
    {% if vpn_user.enable_mfa and vpn_user.fortitoken_serial %}
    "fortitoken": "{{ vpn_user.fortitoken_serial }}",
    {% endif %}
    "status": "enable"
}

{# Add user to VPN group #}
{# PUT /api/v2/cmdb/user/group/{group_name} #}
{% set group = CTX.vpn_group %}
{% set existing_members = TASKS.get_group.result.result.data.results[0].member | d([]) %}
{
    "member": [
        {% for member in existing_members %}
        {"name": "{{ member.name }}"},
        {% endfor %}
        {"name": "{{ vpn_user.username }}"}
    ]
}
```

### Get VPN Tunnel Status

```jinja
{# GET /api/v2/monitor/vpn/ipsec #}
{% set tunnels = TASKS.get_vpn_status.result.result.data.results | d([]) %}

{% set tunnel_status = [] %}
{% for tunnel in tunnels %}
    {% set _ = tunnel_status.append({
        "name": tunnel.name,
        "phase1_status": tunnel.proxyid[0].status if tunnel.proxyid else "unknown",
        "incoming_bytes": tunnel.incoming_bytes | d(0),
        "outgoing_bytes": tunnel.outgoing_bytes | d(0),
        "remote_gateway": tunnel.rgwy,
        "tunnel_up": tunnel.proxyid[0].status == "up" if tunnel.proxyid else false
    }) %}
{% endfor %}

{{ tunnel_status }}
```

---

## Cisco Meraki Patterns

### API Authentication

```jinja
{# Meraki API headers #}
{
    "X-Cisco-Meraki-API-Key": "{{ ORG.VARIABLES.meraki_api_key }}",
    "Content-Type": "application/json"
}
```

### Get Organizations and Networks

```jinja
{# GET /api/v1/organizations #}
{% set orgs = TASKS.get_orgs.result.result.data | d([]) %}

{# GET /api/v1/organizations/{orgId}/networks #}
{% set networks = TASKS.get_networks.result.result.data | d([]) %}

{% set network_summary = [] %}
{% for network in networks %}
    {% set _ = network_summary.append({
        "id": network.id,
        "name": network.name,
        "product_types": network.productTypes,
        "timezone": network.timeZone,
        "tags": network.tags | d([])
    }) %}
{% endfor %}

{{ network_summary }}
```

### Get Device Status

```jinja
{# GET /api/v1/organizations/{orgId}/devices/statuses #}
{% set devices = TASKS.get_device_statuses.result.result.data | d([]) %}

{% set device_summary = [] %}
{% for device in devices %}
    {% set _ = device_summary.append({
        "serial": device.serial,
        "name": device.name,
        "model": device.model,
        "network_id": device.networkId,
        "status": device.status,
        "last_reported": device.lastReportedAt,
        "public_ip": device.publicIp | d("N/A"),
        "lan_ip": device.lanIp | d("N/A"),
        "wan1_ip": device.wan1Ip | d("N/A"),
        "wan2_ip": device.wan2Ip | d("N/A")
    }) %}
{% endfor %}

{# Filter offline devices #}
{% set offline_devices = device_summary | selectattr("status", "ne", "online") | list %}

{{ {"all_devices": device_summary, "offline_devices": offline_devices} }}
```

### Create L3 Firewall Rule

```jinja
{# PUT /api/v1/networks/{networkId}/appliance/firewall/l3FirewallRules #}
{% set new_rule = CTX.firewall_rule %}
{% set existing_rules = TASKS.get_current_rules.result.result.data.rules | d([]) %}

{# Add new rule while preserving existing #}
{% set updated_rules = existing_rules[:-1] %}  {# Remove default rule #}
{% set _ = updated_rules.append({
    "comment": new_rule.comment | d("Created by Rewst"),
    "policy": new_rule.policy | d("allow"),
    "protocol": new_rule.protocol | d("any"),
    "srcPort": new_rule.src_port | d("Any"),
    "srcCidr": new_rule.src_cidr | d("Any"),
    "destPort": new_rule.dest_port | d("Any"),
    "destCidr": new_rule.dest_cidr,
    "syslogEnabled": new_rule.syslog | d(false)
}) %}

{
    "rules": {{ updated_rules | tojson }}
}
```

### Configure Site-to-Site VPN

```jinja
{# PUT /api/v1/networks/{networkId}/appliance/vpn/siteToSiteVpn #}
{% set vpn_config = CTX.vpn_config %}
{
    "mode": "{{ vpn_config.mode | d('hub') }}",
    "hubs": [
        {% for hub in vpn_config.hubs | d([]) %}
        {
            "hubId": "{{ hub.network_id }}",
            "useDefaultRoute": {{ hub.use_default_route | d(false) | lower }}
        }{{ "," if not loop.last else "" }}
        {% endfor %}
    ],
    "subnets": [
        {% for subnet in vpn_config.subnets %}
        {
            "localSubnet": "{{ subnet.cidr }}",
            "useVpn": {{ subnet.use_vpn | d(true) | lower }}
        }{{ "," if not loop.last else "" }}
        {% endfor %}
    ]
}
```

### Create Client VPN User

```jinja
{# Client VPN - uses Meraki auth or RADIUS #}
{# For Meraki-managed auth, users are added to dashboard #}

{# PUT /api/v1/networks/{networkId}/merakiAuthUsers/{merakiAuthUserId} #}
{% set vpn_user = CTX.vpn_user %}
{
    "name": "{{ vpn_user.name }}",
    "email": "{{ vpn_user.email }}",
    "password": "{{ vpn_user.password }}",
    "authorizations": [
        {
            "ssidNumber": {{ vpn_user.ssid_number | d(0) }},
            "expiresAt": "{{ vpn_user.expires_at | d('Never') }}"
        }
    ],
    "accountType": "{{ vpn_user.account_type | d('802.1X') }}",
    "emailPasswordToUser": {{ vpn_user.email_password | d(true) | lower }}
}
```

### Get Network Health

```jinja
{# GET /api/v1/organizations/{orgId}/summary/top/appliances/byUtilization #}
{# GET /api/v1/networks/{networkId}/appliance/uplinks/statuses #}

{% set uplinks = TASKS.get_uplinks.result.result.data | d([]) %}

{% set health_report = [] %}
{% for uplink in uplinks %}
    {% set _ = health_report.append({
        "serial": uplink.serial,
        "network_id": uplink.networkId,
        "uplinks": [
            {
                "interface": u.interface,
                "status": u.status,
                "ip": u.ip | d("N/A"),
                "gateway": u.gateway | d("N/A"),
                "public_ip": u.publicIp | d("N/A"),
                "primary_dns": u.primaryDns | d("N/A"),
                "signal_stat": u.signalStat | d(none)
            }
            for u in uplink.uplinks | d([])
        ]
    }) %}
{% endfor %}

{{ health_report }}
```

---

## SonicWall Patterns

### API Authentication

```jinja
{# SonicWall uses digest authentication or API tokens #}
{# POST /api/sonicos/auth #}
{
    "override": true,
    "user": "{{ ORG.VARIABLES.sonicwall_admin }}",
    "password": "{{ ORG.VARIABLES.sonicwall_password }}"
}

{# Use the returned token in subsequent requests #}
{
    "Authorization": "Bearer {{ CTX.sonicwall_token }}"
}
```

### Get Address Objects

```jinja
{# GET /api/sonicos/address-objects/ipv4 #}
{% set addresses = TASKS.get_addresses.result.result.data.address_objects | d([]) %}

{% set address_summary = [] %}
{% for addr in addresses %}
    {% set _ = address_summary.append({
        "name": addr.name,
        "zone": addr.zone,
        "type": addr.type | d("host"),
        "host": addr.host | d(none),
        "network": addr.network | d(none),
        "mask": addr.mask | d(none)
    }) %}
{% endfor %}

{{ address_summary }}
```

### Create Address Object

```jinja
{# POST /api/sonicos/address-objects/ipv4 #}
{% set address = CTX.new_address %}
{
    "address_objects": [
        {
            "ipv4": {
                "name": "{{ address.name }}",
                "zone": "{{ address.zone | d('LAN') }}",
                {% if address.type == "host" %}
                "host": {
                    "ip": "{{ address.ip }}"
                }
                {% elif address.type == "network" %}
                "network": {
                    "subnet": "{{ address.subnet }}",
                    "mask": "{{ address.mask }}"
                }
                {% elif address.type == "range" %}
                "range": {
                    "begin": "{{ address.start_ip }}",
                    "end": "{{ address.end_ip }}"
                }
                {% elif address.type == "fqdn" %}
                "fqdn": {
                    "domain": "{{ address.fqdn }}"
                }
                {% endif %}
            }
        }
    ]
}
```

### Create Firewall Rule

```jinja
{# POST /api/sonicos/access-rules/ipv4 #}
{% set rule = CTX.firewall_rule %}
{
    "access_rules": [
        {
            "ipv4": {
                "name": "{{ rule.name }}",
                "enable": true,
                "priority": {
                    "auto": true
                },
                "source": {
                    "zone": "{{ rule.source_zone }}",
                    "address": {
                        {% if rule.source_address == "any" %}
                        "any": true
                        {% else %}
                        "name": "{{ rule.source_address }}"
                        {% endif %}
                    }
                },
                "destination": {
                    "zone": "{{ rule.dest_zone }}",
                    "address": {
                        {% if rule.dest_address == "any" %}
                        "any": true
                        {% else %}
                        "name": "{{ rule.dest_address }}"
                        {% endif %}
                    }
                },
                "service": {
                    {% if rule.service == "any" %}
                    "any": true
                    {% else %}
                    "name": "{{ rule.service }}"
                    {% endif %}
                },
                "action": "{{ rule.action | d('allow') }}",
                "users": {
                    "included": {
                        "all": true
                    }
                },
                "schedule": {
                    "always_on": true
                },
                "logging": {{ rule.logging | d(true) | lower }},
                "comment": "{{ rule.comment | d('Created by Rewst') }}"
            }
        }
    ]
}
```

### Commit Configuration

```jinja
{# POST /api/sonicos/config/pending #}
{# SonicWall requires committing changes after modifications #}
{
    "commit": true
}
```

---

## Ubiquiti UniFi Patterns

### API Authentication

```jinja
{# UniFi Controller login #}
{# POST /api/login #}
{
    "username": "{{ ORG.VARIABLES.unifi_username }}",
    "password": "{{ ORG.VARIABLES.unifi_password }}"
}

{# Store session cookie for subsequent requests #}
{# Cookie: unifises={session_id}; csrf_token={token} #}
```

### Get Sites and Devices

```jinja
{# GET /api/self/sites #}
{% set sites = TASKS.get_sites.result.result.data.data | d([]) %}

{# GET /api/s/{site}/stat/device #}
{% set devices = TASKS.get_devices.result.result.data.data | d([]) %}

{% set device_summary = [] %}
{% for device in devices %}
    {% set _ = device_summary.append({
        "mac": device.mac,
        "name": device.name | d(device.mac),
        "model": device.model,
        "type": device.type,
        "state": device.state,
        "adopted": device.adopted,
        "ip": device.ip | d("N/A"),
        "uptime": device.uptime | d(0),
        "version": device.version | d("Unknown"),
        "upgradable": device.upgradable | d(false)
    }) %}
{% endfor %}

{{ device_summary }}
```

### Get Client List

```jinja
{# GET /api/s/{site}/stat/sta #}
{% set clients = TASKS.get_clients.result.result.data.data | d([]) %}

{% set client_summary = [] %}
{% for client in clients %}
    {% set _ = client_summary.append({
        "mac": client.mac,
        "hostname": client.hostname | d(client.mac),
        "ip": client.ip | d("N/A"),
        "network": client.network | d("default"),
        "is_wired": client.is_wired | d(false),
        "ap_mac": client.ap_mac | d(none),
        "signal": client.signal | d(none),
        "rx_bytes": client.rx_bytes | d(0),
        "tx_bytes": client.tx_bytes | d(0),
        "uptime": client.uptime | d(0)
    }) %}
{% endfor %}

{{ client_summary }}
```

### Create Firewall Rule (USG/UDM)

```jinja
{# POST /api/s/{site}/rest/firewallrule #}
{% set rule = CTX.firewall_rule %}
{
    "name": "{{ rule.name }}",
    "enabled": true,
    "action": "{{ rule.action | d('accept') }}",
    "ruleset": "{{ rule.ruleset | d('WAN_IN') }}",
    "rule_index": {{ rule.index | d(2000) }},
    "protocol": "{{ rule.protocol | d('all') }}",
    {% if rule.src_address %}
    "src_address": "{{ rule.src_address }}",
    {% endif %}
    {% if rule.dst_address %}
    "dst_address": "{{ rule.dst_address }}",
    {% endif %}
    {% if rule.dst_port %}
    "dst_port": "{{ rule.dst_port }}",
    {% endif %}
    "logging": {{ rule.logging | d(false) | lower }},
    "state_new": true,
    "state_established": true,
    "state_related": true
}
```

### Block/Unblock Client

```jinja
{# POST /api/s/{site}/cmd/stamgr #}
{% set client = CTX.client %}

{# Block client #}
{
    "cmd": "block-sta",
    "mac": "{{ client.mac }}"
}

{# Unblock client #}
{
    "cmd": "unblock-sta",
    "mac": "{{ client.mac }}"
}
```

### Create VLAN/Network

```jinja
{# POST /api/s/{site}/rest/networkconf #}
{% set network = CTX.new_network %}
{
    "name": "{{ network.name }}",
    "purpose": "{{ network.purpose | d('corporate') }}",
    "vlan_enabled": true,
    "vlan": {{ network.vlan_id }},
    "ip_subnet": "{{ network.subnet }}",
    "dhcpd_enabled": {{ network.dhcp_enabled | d(true) | lower }},
    {% if network.dhcp_enabled | d(true) %}
    "dhcpd_start": "{{ network.dhcp_start }}",
    "dhcpd_stop": "{{ network.dhcp_stop }}",
    "dhcpd_dns_enabled": true,
    "dhcpd_dns_1": "{{ network.dns_1 | d('8.8.8.8') }}",
    "dhcpd_dns_2": "{{ network.dns_2 | d('8.8.4.4') }}",
    "dhcpd_leasetime": {{ network.lease_time | d(86400) }},
    {% endif %}
    "domain_name": "{{ network.domain | d('local') }}",
    "networkgroup": "{{ network.network_group | d('LAN') }}"
}
```

---

## Multi-Vendor Abstraction Patterns

### Normalized Device Status

```jinja
{# Normalize device status across vendors #}
{% set raw_devices = CTX.raw_devices %}
{% set vendor = CTX.vendor %}

{% set normalized = [] %}
{% for device in raw_devices %}
    {% if vendor == "fortigate" %}
        {% set _ = normalized.append({
            "id": device.serial,
            "name": device.hostname | d(device.serial),
            "type": "firewall",
            "vendor": "Fortinet",
            "model": device.platform,
            "firmware": device.version,
            "status": "online" if device.status == "connected" else "offline",
            "ip": device.ip,
            "uptime_seconds": device.uptime | d(0),
            "last_seen": CTX.current_time
        }) %}
    {% elif vendor == "meraki" %}
        {% set _ = normalized.append({
            "id": device.serial,
            "name": device.name | d(device.serial),
            "type": device.productType | d("unknown"),
            "vendor": "Cisco Meraki",
            "model": device.model,
            "firmware": device.firmware | d("Unknown"),
            "status": device.status,
            "ip": device.lanIp | d(device.wan1Ip),
            "uptime_seconds": none,
            "last_seen": device.lastReportedAt
        }) %}
    {% elif vendor == "sonicwall" %}
        {% set _ = normalized.append({
            "id": device.serial_number,
            "name": device.hostname,
            "type": "firewall",
            "vendor": "SonicWall",
            "model": device.model,
            "firmware": device.firmware_version,
            "status": "online",
            "ip": device.wan_ip,
            "uptime_seconds": device.system_uptime | d(0),
            "last_seen": CTX.current_time
        }) %}
    {% elif vendor == "unifi" %}
        {% set _ = normalized.append({
            "id": device.mac,
            "name": device.name | d(device.mac),
            "type": device.type,
            "vendor": "Ubiquiti",
            "model": device.model,
            "firmware": device.version,
            "status": "online" if device.state == 1 else "offline",
            "ip": device.ip,
            "uptime_seconds": device.uptime | d(0),
            "last_seen": CTX.current_time if device.state == 1 else device.last_seen
        }) %}
    {% endif %}
{% endfor %}

{{ normalized }}
```

### Unified Firewall Rule Creation

```jinja
{# Abstract firewall rule creation across vendors #}
{% set rule = CTX.firewall_rule %}
{% set vendor = CTX.vendor %}

{% if vendor == "fortigate" %}
    {# FortiGate format #}
    {% set vendor_rule = {
        "name": rule.name,
        "srcintf": [{"name": rule.source_zone}],
        "dstintf": [{"name": rule.dest_zone}],
        "srcaddr": [{"name": rule.source_address | d("all")}],
        "dstaddr": [{"name": rule.dest_address | d("all")}],
        "service": [{"name": rule.service | d("ALL")}],
        "action": rule.action | d("accept"),
        "status": "enable",
        "logtraffic": "utm" if rule.logging else "disable"
    } %}
{% elif vendor == "meraki" %}
    {# Meraki format #}
    {% set vendor_rule = {
        "comment": rule.name,
        "policy": rule.action | d("allow"),
        "protocol": rule.protocol | d("any"),
        "srcCidr": rule.source_cidr | d("Any"),
        "srcPort": rule.source_port | d("Any"),
        "destCidr": rule.dest_cidr | d("Any"),
        "destPort": rule.dest_port | d("Any"),
        "syslogEnabled": rule.logging | d(false)
    } %}
{% elif vendor == "sonicwall" %}
    {# SonicWall format #}
    {% set vendor_rule = {
        "name": rule.name,
        "source": {
            "zone": rule.source_zone,
            "address": {"name": rule.source_address} if rule.source_address else {"any": true}
        },
        "destination": {
            "zone": rule.dest_zone,
            "address": {"name": rule.dest_address} if rule.dest_address else {"any": true}
        },
        "service": {"name": rule.service} if rule.service else {"any": true},
        "action": rule.action | d("allow"),
        "logging": rule.logging | d(true)
    } %}
{% elif vendor == "unifi" %}
    {# UniFi format #}
    {% set vendor_rule = {
        "name": rule.name,
        "enabled": true,
        "action": rule.action | d("accept"),
        "ruleset": rule.ruleset | d("WAN_IN"),
        "protocol": rule.protocol | d("all"),
        "src_address": rule.source_cidr | d(none),
        "dst_address": rule.dest_cidr | d(none),
        "dst_port": rule.dest_port | d(none),
        "logging": rule.logging | d(false)
    } %}
{% endif %}

{{ vendor_rule }}
```

---

## Common Automation Workflows

### Port Forward Request Automation

```jinja
{# Automated port forward workflow #}
{% set request = CTX.port_forward_request %}

{# Validate request #}
{% set validation = {
    "is_valid": true,
    "errors": []
} %}

{# Check for reserved ports #}
{% set reserved_ports = [21, 22, 23, 25, 53, 80, 443, 3389] %}
{% if request.external_port | int in reserved_ports %}
    {% set _ = validation.errors.append("External port " ~ request.external_port ~ " is reserved") %}
    {% set validation = validation | combine({"is_valid": false}) %}
{% endif %}

{# Check for existing rules with same port #}
{% set existing = CTX.existing_rules | selectattr("external_port", "eq", request.external_port | int) | list %}
{% if existing | length > 0 %}
    {% set _ = validation.errors.append("Port " ~ request.external_port ~ " is already forwarded") %}
    {% set validation = validation | combine({"is_valid": false}) %}
{% endif %}

{# Build rule if valid #}
{% if validation.is_valid %}
    {% set port_forward = {
        "name": "PF_" ~ request.service_name ~ "_" ~ request.external_port,
        "external_port": request.external_port,
        "internal_ip": request.internal_ip,
        "internal_port": request.internal_port | d(request.external_port),
        "protocol": request.protocol | d("tcp"),
        "description": request.description | d("Port forward created by automation"),
        "requester": request.requester,
        "ticket_id": request.ticket_id,
        "created_at": CTX.current_time
    } %}
{% endif %}

{{ {"validation": validation, "port_forward": port_forward if validation.is_valid else none} }}
```

### VPN User Provisioning Workflow

```jinja
{# Automated VPN user provisioning #}
{% set user = CTX.new_vpn_user %}
{% set firewall_vendor = ORG.VARIABLES.firewall_vendor %}

{# Generate secure password if not provided #}
{% set password = user.password | d(CTX.generated_password) %}

{# Build vendor-specific user config #}
{% if firewall_vendor == "fortigate" %}
    {% set vpn_config = {
        "endpoint": "/api/v2/cmdb/user/local",
        "method": "POST",
        "payload": {
            "name": user.username,
            "type": "password",
            "passwd": password,
            "email-to": user.email,
            "two-factor": "fortitoken" if user.mfa_required else "disable",
            "status": "enable"
        },
        "group_endpoint": "/api/v2/cmdb/user/group/VPN_Users",
        "group_method": "PUT"
    } %}
{% elif firewall_vendor == "sonicwall" %}
    {% set vpn_config = {
        "endpoint": "/api/sonicos/user/local",
        "method": "POST",
        "payload": {
            "local_users": [{
                "user": {
                    "name": user.username,
                    "password": password,
                    "email_id": user.email,
                    "user_type": "name",
                    "groups": ["VPN Users"]
                }
            }]
        }
    } %}
{% elif firewall_vendor == "meraki" %}
    {% set vpn_config = {
        "endpoint": "/api/v1/networks/" ~ CTX.network_id ~ "/merakiAuthUsers",
        "method": "POST",
        "payload": {
            "name": user.display_name,
            "email": user.email,
            "password": password,
            "authorizations": [{
                "ssidNumber": 0,
                "expiresAt": user.expiration | d("Never")
            }],
            "emailPasswordToUser": true
        }
    } %}
{% endif %}

{{ vpn_config }}
```

### Network Health Check Workflow

```jinja
{# Comprehensive network health check #}
{% set devices = CTX.all_devices %}
{% set uplinks = CTX.uplink_statuses %}
{% set vpn_tunnels = CTX.vpn_tunnels %}

{# Calculate health metrics #}
{% set total_devices = devices | length %}
{% set online_devices = devices | selectattr("status", "eq", "online") | list | length %}
{% set device_health = (online_devices / total_devices * 100) | round(1) if total_devices > 0 else 100 %}

{# Uplink health #}
{% set primary_uplinks = uplinks | selectattr("interface", "eq", "wan1") | list %}
{% set healthy_uplinks = primary_uplinks | selectattr("status", "eq", "active") | list | length %}
{% set uplink_health = (healthy_uplinks / (primary_uplinks | length) * 100) | round(1) if primary_uplinks | length > 0 else 100 %}

{# VPN health #}
{% set total_tunnels = vpn_tunnels | length %}
{% set active_tunnels = vpn_tunnels | selectattr("status", "eq", "up") | list | length %}
{% set vpn_health = (active_tunnels / total_tunnels * 100) | round(1) if total_tunnels > 0 else 100 %}

{# Overall score #}
{% set overall_health = ((device_health * 0.4) + (uplink_health * 0.4) + (vpn_health * 0.2)) | round(1) %}

{# Identify issues #}
{% set issues = [] %}
{% for device in devices | selectattr("status", "ne", "online") | list %}
    {% set _ = issues.append({
        "type": "device_offline",
        "severity": "high",
        "device": device.name,
        "message": device.name ~ " is offline"
    }) %}
{% endfor %}

{% for uplink in uplinks | selectattr("status", "ne", "active") | list %}
    {% set _ = issues.append({
        "type": "uplink_down",
        "severity": "critical",
        "device": uplink.device_name,
        "interface": uplink.interface,
        "message": uplink.interface ~ " on " ~ uplink.device_name ~ " is down"
    }) %}
{% endfor %}

{% for tunnel in vpn_tunnels | selectattr("status", "ne", "up") | list %}
    {% set _ = issues.append({
        "type": "vpn_down",
        "severity": "high",
        "tunnel": tunnel.name,
        "message": "VPN tunnel " ~ tunnel.name ~ " is down"
    }) %}
{% endfor %}

{
    "overall_health": {{ overall_health }},
    "health_status": "{{ 'healthy' if overall_health >= 95 else 'degraded' if overall_health >= 75 else 'critical' }}",
    "metrics": {
        "device_health": {{ device_health }},
        "uplink_health": {{ uplink_health }},
        "vpn_health": {{ vpn_health }},
        "devices_online": {{ online_devices }},
        "devices_total": {{ total_devices }},
        "uplinks_active": {{ healthy_uplinks }},
        "uplinks_total": {{ primary_uplinks | length }},
        "tunnels_up": {{ active_tunnels }},
        "tunnels_total": {{ total_tunnels }}
    },
    "issues": {{ issues | tojson }},
    "issue_count": {{ issues | length }}
}
```

---

## Security Automation

### Automated Threat Response

```jinja
{# Automated response to security threats #}
{% set threat = CTX.detected_threat %}
{% set response_policy = ORG.VARIABLES.threat_response_policy | d("isolate") %}

{# Determine response actions based on threat type #}
{% set response_actions = [] %}

{% if threat.severity == "critical" %}
    {# Critical threats get immediate isolation #}
    {% set _ = response_actions.append({
        "action": "isolate_device",
        "target": threat.source_ip,
        "reason": "Critical threat detected: " ~ threat.type
    }) %}
    {% set _ = response_actions.append({
        "action": "block_ip",
        "target": threat.destination_ip if threat.direction == "outbound" else threat.source_ip,
        "duration": "permanent"
    }) %}
    {% set _ = response_actions.append({
        "action": "create_ticket",
        "priority": "Critical",
        "notify": ["security_team", "management"]
    }) %}
{% elif threat.severity == "high" %}
    {% set _ = response_actions.append({
        "action": "quarantine",
        "target": threat.source_ip,
        "duration": "24h"
    }) %}
    {% set _ = response_actions.append({
        "action": "create_ticket",
        "priority": "High",
        "notify": ["security_team"]
    }) %}
{% else %}
    {% set _ = response_actions.append({
        "action": "log_and_monitor",
        "target": threat.source_ip,
        "duration": "7d"
    }) %}
    {% set _ = response_actions.append({
        "action": "create_ticket",
        "priority": "Medium"
    }) %}
{% endif %}

{{ response_actions }}
```

### IP Reputation Check

```jinja
{# Check IP reputation before allowing access #}
{% set ip = CTX.ip_to_check %}
{% set reputation_data = TASKS.check_reputation.result.result.data | d({}) %}

{% set risk_score = reputation_data.risk_score | d(0) %}
{% set categories = reputation_data.categories | d([]) %}

{% set blocked_categories = ["malware", "botnet", "c2", "phishing", "tor_exit"] %}
{% set suspicious_categories = ["proxy", "vpn", "hosting", "scanner"] %}

{% set should_block = false %}
{% set block_reason = none %}

{# Check for blocked categories #}
{% for category in categories %}
    {% if category in blocked_categories %}
        {% set should_block = true %}
        {% set block_reason = "IP is associated with " ~ category %}
    {% endif %}
{% endfor %}

{# Check risk score threshold #}
{% if risk_score >= 80 %}
    {% set should_block = true %}
    {% set block_reason = "IP risk score exceeds threshold: " ~ risk_score %}
{% endif %}

{
    "ip": "{{ ip }}",
    "risk_score": {{ risk_score }},
    "categories": {{ categories | tojson }},
    "should_block": {{ should_block | lower }},
    "block_reason": {{ block_reason | tojson if block_reason else "null" }},
    "recommendation": "{{ 'block' if should_block else 'allow_with_monitoring' if risk_score >= 50 else 'allow' }}"
}
```

---

## Best Practices

### API Security

1. **Store credentials securely** in Org Variables, never in workflow code
2. **Use HTTPS** for all API communications
3. **Implement rate limiting** to avoid being blocked
4. **Log all changes** for audit trail
5. **Test in non-production** before deploying

### Change Management

1. **Document all changes** with ticket references
2. **Implement approval workflows** for critical changes
3. **Create rollback procedures** for each change type
4. **Schedule maintenance windows** for risky changes
5. **Notify affected parties** before and after changes

### Multi-Vendor Environments

1. **Use abstraction layers** for common operations
2. **Normalize data formats** for consistent reporting
3. **Maintain vendor-specific documentation**
4. **Test integrations regularly**
5. **Have fallback procedures** for API failures

### Performance Considerations

1. **Cache device inventories** to reduce API calls
2. **Batch operations** where possible
3. **Use webhooks** instead of polling when available
4. **Implement retry logic** with exponential backoff
5. **Monitor API quotas** and limits
