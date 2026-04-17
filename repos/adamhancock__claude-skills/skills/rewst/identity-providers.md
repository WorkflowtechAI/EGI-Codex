# Rewst Identity Provider Patterns

Integration patterns for Okta, Azure AD B2B, JumpCloud, Google Workspace, and multi-IdP environments.

---

## Azure AD / Entra ID Deep Dive

### B2B Guest User Management

```jinja
{# Invite external guest user #}
{# POST https://graph.microsoft.com/v1.0/invitations #}
{
    "invitedUserEmailAddress": "{{ CTX.guest_email }}",
    "inviteRedirectUrl": "{{ ORG.VARIABLES.b2b_redirect_url | d('https://myapps.microsoft.com') }}",
    "invitedUserDisplayName": "{{ CTX.guest_name }}",
    "sendInvitationMessage": {{ CTX.send_invite | d(true) | lower }},
    "invitedUserMessageInfo": {
        "customizedMessageBody": "{{ CTX.custom_message | d('You have been invited to collaborate with our organization.') | escape }}"
    },
    "invitedUserType": "{{ CTX.user_type | d('Guest') }}"
}

{# Response handling #}
{% set invitation = TASKS.invite_guest.result.result.data %}
{% set guest_user_id = invitation.invitedUser.id %}
{% set redeem_url = invitation.inviteRedeemUrl %}
{% set invite_status = invitation.status %}  {# PendingAcceptance, Completed, etc. #}
```

### Conditional Access Policy Management

```jinja
{# List Conditional Access Policies #}
{# GET https://graph.microsoft.com/v1.0/identity/conditionalAccess/policies #}

{# Create CA Policy (requires Policy.ReadWrite.ConditionalAccess) #}
{# POST https://graph.microsoft.com/v1.0/identity/conditionalAccess/policies #}
{
    "displayName": "{{ CTX.policy_name }}",
    "state": "{{ CTX.state | d('disabled') }}",  {# disabled, enabled, enabledForReportingButNotEnforced #}
    "conditions": {
        "users": {
            "includeUsers": {{ CTX.include_users | to_json_string | d('["All"]') }},
            "excludeUsers": {{ CTX.exclude_users | to_json_string | d('[]') }},
            "includeGroups": {{ CTX.include_groups | to_json_string | d('[]') }},
            "excludeGroups": {{ CTX.exclude_groups | to_json_string | d('[]') }}
        },
        "applications": {
            "includeApplications": {{ CTX.include_apps | to_json_string | d('["All"]') }},
            "excludeApplications": {{ CTX.exclude_apps | to_json_string | d('[]') }}
        },
        "locations": {
            "includeLocations": {{ CTX.include_locations | to_json_string | d('["All"]') }},
            "excludeLocations": {{ CTX.exclude_locations | to_json_string | d('["AllTrusted"]') }}
        },
        "clientAppTypes": {{ CTX.client_app_types | to_json_string | d('["all"]') }},
        "signInRiskLevels": {{ CTX.sign_in_risk | to_json_string | d('[]') }},
        "userRiskLevels": {{ CTX.user_risk | to_json_string | d('[]') }}
    },
    "grantControls": {
        "operator": "{{ CTX.grant_operator | d('OR') }}",
        "builtInControls": {{ CTX.grant_controls | to_json_string | d('["mfa"]') }}
    }
    {% if CTX.session_controls %},
    "sessionControls": {{ CTX.session_controls | to_json_string }}
    {% endif %}
}
```

### Administrative Unit Management

```jinja
{# Create Administrative Unit #}
{# POST https://graph.microsoft.com/v1.0/directory/administrativeUnits #}
{
    "displayName": "{{ CTX.au_name }}",
    "description": "{{ CTX.au_description | d('') }}",
    "membershipType": "{{ CTX.membership_type | d('Assigned') }}",  {# Assigned, Dynamic #}
    {% if CTX.membership_type == "Dynamic" %}
    "membershipRule": "{{ CTX.membership_rule }}",
    "membershipRuleProcessingState": "On",
    {% endif %}
    "visibility": "{{ CTX.visibility | d('Public') }}"  {# Public, HiddenMembership #}
}

{# Add member to AU #}
{# POST https://graph.microsoft.com/v1.0/directory/administrativeUnits/{au-id}/members/$ref #}
{
    "@odata.id": "https://graph.microsoft.com/v1.0/users/{{ CTX.user_id }}"
}

{# Assign scoped role in AU #}
{# POST https://graph.microsoft.com/v1.0/directory/administrativeUnits/{au-id}/scopedRoleMembers #}
{
    "roleId": "{{ CTX.role_id }}",  {# e.g., User Administrator role ID #}
    "roleMemberInfo": {
        "id": "{{ CTX.admin_user_id }}"
    }
}
```

### Directory Role Management

```jinja
{# Common Azure AD Role IDs #}
{% set azure_roles = {
    "global_admin": "62e90394-69f5-4237-9190-012177145e10",
    "user_admin": "fe930be7-5e62-47db-91af-98c3a49a38b1",
    "helpdesk_admin": "729827e3-9c14-49f7-bb1b-9608f156bbb8",
    "exchange_admin": "29232cdf-9323-42fd-ade2-1d097af3e4de",
    "sharepoint_admin": "f28a1f50-f6e7-4571-818b-6a12f2af6b6c",
    "teams_admin": "69091246-20e8-4a56-aa4d-066075b2a7a8",
    "intune_admin": "3a2c62db-5318-420d-8d74-23affee5d9d5",
    "security_admin": "194ae4cb-b126-40b2-bd5b-6091b380977d",
    "cloud_app_admin": "158c047a-c907-4556-b7ef-446551a6b5f7",
    "application_admin": "9b895d92-2cd3-44c7-9d02-a6ac2d5ea5c3",
    "password_admin": "966707d0-3269-4727-9be2-8c3a10f19b9d",
    "license_admin": "4d6ac14f-3453-41d0-bef9-a3e0c569773a",
    "groups_admin": "fdd7a751-b60b-444a-984c-02652fe8fa1c"
} %}

{# Assign directory role #}
{# POST https://graph.microsoft.com/v1.0/directoryRoles/roleTemplateId={role-template-id}/members/$ref #}
{
    "@odata.id": "https://graph.microsoft.com/v1.0/users/{{ CTX.user_id }}"
}

{# List user's roles #}
{# GET https://graph.microsoft.com/v1.0/users/{user-id}/memberOf/microsoft.graph.directoryRole #}
```

---

## Okta Integration

### User Management

```jinja
{# Okta API Base URL format #}
{% set okta_base = "https://" ~ ORG.VARIABLES.okta_domain ~ "/api/v1" %}

{# Create Okta User #}
{# POST /api/v1/users?activate=true #}
{
    "profile": {
        "firstName": "{{ CTX.first_name }}",
        "lastName": "{{ CTX.last_name }}",
        "email": "{{ CTX.email }}",
        "login": "{{ CTX.login | d(CTX.email) }}",
        "mobilePhone": "{{ CTX.mobile | d(none) }}",
        "department": "{{ CTX.department | d(none) }}",
        "title": "{{ CTX.job_title | d(none) }}",
        "manager": "{{ CTX.manager_email | d(none) }}",
        "employeeNumber": "{{ CTX.employee_id | d(none) }}"
        {% if CTX.custom_attributes %}
        {% for key, value in CTX.custom_attributes.items() %},
        "{{ key }}": "{{ value }}"
        {% endfor %}
        {% endif %}
    },
    "credentials": {
        "password": {
            "value": "{{ CTX.temp_password }}"
        },
        "recovery_question": {
            "question": "{{ CTX.security_question | d('What is your favorite color?') }}",
            "answer": "{{ CTX.security_answer | d('blue') }}"
        }
    },
    "groupIds": {{ CTX.group_ids | to_json_string | d('[]') }}
}

{# Get User by login/email #}
{# GET /api/v1/users?search=profile.login eq "user@example.com" #}
{# GET /api/v1/users/{userId} #}

{# Update User Profile #}
{# POST /api/v1/users/{userId} #}
{
    "profile": {
        "department": "{{ CTX.new_department }}",
        "title": "{{ CTX.new_title }}",
        "manager": "{{ CTX.new_manager }}"
    }
}
```

### Okta Group Management

```jinja
{# List Groups #}
{# GET /api/v1/groups?search=profile.name sw "IT" #}

{# Create Group #}
{# POST /api/v1/groups #}
{
    "profile": {
        "name": "{{ CTX.group_name }}",
        "description": "{{ CTX.group_description | d('') }}"
    }
}

{# Add User to Group #}
{# PUT /api/v1/groups/{groupId}/users/{userId} #}

{# List Group Members #}
{# GET /api/v1/groups/{groupId}/users #}

{# Assign App to Group #}
{# PUT /api/v1/apps/{appId}/groups/{groupId} #}
{
    "priority": {{ CTX.priority | d(0) }},
    "profile": {{ CTX.app_profile | to_json_string | d('{}') }}
}
```

### Okta Application Assignment

```jinja
{# Assign Application to User #}
{# POST /api/v1/apps/{appId}/users #}
{
    "id": "{{ CTX.user_id }}",
    "scope": "{{ CTX.scope | d('USER') }}",
    "credentials": {
        "userName": "{{ CTX.app_username | d(CTX.email) }}"
    },
    "profile": {{ CTX.app_profile | to_json_string | d('{}') }}
}

{# List User's Applications #}
{# GET /api/v1/apps?filter=user.id eq "{userId}" #}

{# Remove Application from User #}
{# DELETE /api/v1/apps/{appId}/users/{userId} #}
```

### Okta Lifecycle Operations

```jinja
{# User Lifecycle State Machine #}
{% set okta_states = {
    "STAGED": "User created but not activated",
    "PROVISIONED": "Activated but never logged in",
    "ACTIVE": "Normal active state",
    "RECOVERY": "Password/unlock recovery",
    "PASSWORD_EXPIRED": "Password needs reset",
    "LOCKED_OUT": "Too many failed attempts",
    "SUSPENDED": "Admin suspended",
    "DEPROVISIONED": "Deactivated"
} %}

{# Activate User #}
{# POST /api/v1/users/{userId}/lifecycle/activate?sendEmail=true #}

{# Suspend User #}
{# POST /api/v1/users/{userId}/lifecycle/suspend #}

{# Unsuspend User #}
{# POST /api/v1/users/{userId}/lifecycle/unsuspend #}

{# Deactivate User #}
{# POST /api/v1/users/{userId}/lifecycle/deactivate #}

{# Reactivate User (from DEPROVISIONED) #}
{# POST /api/v1/users/{userId}/lifecycle/reactivate?sendEmail=true #}

{# Delete User (must be DEPROVISIONED first) #}
{# DELETE /api/v1/users/{userId} #}

{# Reset Password #}
{# POST /api/v1/users/{userId}/lifecycle/reset_password?sendEmail=true #}

{# Unlock User #}
{# POST /api/v1/users/{userId}/lifecycle/unlock #}

{# Expire Password #}
{# POST /api/v1/users/{userId}/lifecycle/expire_password #}
```

### Okta MFA Enrollment

```jinja
{# List User's Enrolled Factors #}
{# GET /api/v1/users/{userId}/factors #}

{# Enroll SMS Factor #}
{# POST /api/v1/users/{userId}/factors #}
{
    "factorType": "sms",
    "provider": "OKTA",
    "profile": {
        "phoneNumber": "{{ CTX.phone_number }}"
    }
}

{# Activate Factor (after enrollment) #}
{# POST /api/v1/users/{userId}/factors/{factorId}/lifecycle/activate #}
{
    "passCode": "{{ CTX.verification_code }}"
}

{# Reset All Factors #}
{# POST /api/v1/users/{userId}/lifecycle/reset_factors #}

{# Available Factor Types #}
{% set okta_factors = [
    "push",           {# Okta Verify with Push #}
    "sms",            {# SMS #}
    "call",           {# Voice Call #}
    "email",          {# Email #}
    "question",       {# Security Question #}
    "token:software:totp",  {# Authenticator App #}
    "token:hardware",       {# Hardware Token #}
    "webauthn"        {# WebAuthn/FIDO2 #}
] %}
```

---

## JumpCloud Integration

### User Management

```jinja
{# JumpCloud API Base #}
{% set jc_base = "https://console.jumpcloud.com/api" %}

{# Create User #}
{# POST /systemusers #}
{
    "email": "{{ CTX.email }}",
    "username": "{{ CTX.username }}",
    "firstname": "{{ CTX.first_name }}",
    "lastname": "{{ CTX.last_name }}",
    "displayname": "{{ CTX.display_name | d(CTX.first_name ~ ' ' ~ CTX.last_name) }}",
    "department": "{{ CTX.department | d('') }}",
    "jobTitle": "{{ CTX.job_title | d('') }}",
    "employeeType": "{{ CTX.employee_type | d('') }}",
    "company": "{{ CTX.company | d('') }}",
    "state": "{{ CTX.state | d('ACTIVATED') }}",
    "activated": {{ CTX.activated | d(true) | lower }},
    "password_never_expires": {{ CTX.password_never_expires | d(false) | lower }},
    "allow_public_key": {{ CTX.allow_ssh_key | d(true) | lower }},
    "sudo": {{ CTX.sudo | d(false) | lower }},
    "passwordless_sudo": {{ CTX.passwordless_sudo | d(false) | lower }},
    "enable_managed_uid": {{ CTX.managed_uid | d(false) | lower }},
    "enable_user_portal_multifactor": {{ CTX.require_mfa | d(true) | lower }},
    "ldap_binding_user": {{ CTX.ldap_binding | d(false) | lower }},
    "attributes": {{ CTX.custom_attributes | to_json_string | d('[]') }}
}

{# Get User by Email #}
{# POST /search/systemusers #}
{
    "searchFilter": {
        "and": [
            {"email": {"$eq": "{{ CTX.email }}"}}
        ]
    },
    "fields": ["email", "username", "firstname", "lastname", "id"]
}

{# Update User #}
{# PUT /systemusers/{id} #}
{
    "department": "{{ CTX.new_department }}",
    "jobTitle": "{{ CTX.new_title }}",
    "manager": "{{ CTX.new_manager_id }}"
}

{# Delete User (or set state to SUSPENDED) #}
{# DELETE /systemusers/{id} #}
{# PUT /systemusers/{id} with "state": "SUSPENDED" #}
```

### JumpCloud Group Management

```jinja
{# Create User Group #}
{# POST /v2/usergroups #}
{
    "name": "{{ CTX.group_name }}",
    "description": "{{ CTX.description | d('') }}",
    "attributes": {
        "posixGroups": {{ CTX.posix_groups | to_json_string | d('[]') }},
        "sambaEnabled": {{ CTX.samba_enabled | d(false) | lower }}
    }
}

{# Add User to Group #}
{# POST /v2/usergroups/{groupId}/members #}
{
    "op": "add",
    "type": "user",
    "id": "{{ CTX.user_id }}"
}

{# Create System Group #}
{# POST /v2/systemgroups #}
{
    "name": "{{ CTX.system_group_name }}",
    "description": "{{ CTX.description | d('') }}"
}

{# Bind User Group to System Group #}
{# POST /v2/usergroups/{userGroupId}/associations #}
{
    "op": "add",
    "type": "system_group",
    "id": "{{ CTX.system_group_id }}"
}
```

### JumpCloud Directory Sync

```jinja
{# List LDAP Directories #}
{# GET /v2/ldapservers #}

{# Sync User to LDAP Directory #}
{# Ensure user has ldap_binding_user: true #}

{# List Google Workspace Directories #}
{# GET /v2/directories #}
{% set google_dirs = TASKS.list_dirs.result.result.data | selectattr("type", "eq", "g_suite") | list %}

{# List Microsoft 365 Directories #}
{% set m365_dirs = TASKS.list_dirs.result.result.data | selectattr("type", "eq", "office_365") | list %}
```

---

## Google Workspace Integration

### User Management

```jinja
{# Google Admin SDK - Directory API #}
{% set google_base = "https://admin.googleapis.com/admin/directory/v1" %}

{# Create User #}
{# POST /users #}
{
    "name": {
        "givenName": "{{ CTX.first_name }}",
        "familyName": "{{ CTX.last_name }}"
    },
    "primaryEmail": "{{ CTX.email }}",
    "password": "{{ CTX.temp_password }}",
    "changePasswordAtNextLogin": {{ CTX.force_password_change | d(true) | lower }},
    "orgUnitPath": "{{ CTX.org_unit | d('/') }}",
    "includeInGlobalAddressList": {{ CTX.in_gal | d(true) | lower }},
    "recoveryEmail": "{{ CTX.recovery_email | d('') }}",
    "recoveryPhone": "{{ CTX.recovery_phone | d('') }}",
    "organizations": [
        {
            "department": "{{ CTX.department }}",
            "title": "{{ CTX.job_title }}",
            "primary": true
        }
    ],
    "relations": [
        {% if CTX.manager_email %}
        {
            "value": "{{ CTX.manager_email }}",
            "type": "manager"
        }
        {% endif %}
    ],
    "customSchemas": {{ CTX.custom_schemas | to_json_string | d('{}') }}
}

{# Get User #}
{# GET /users/{userKey} #}
{# userKey can be email or immutableId #}

{# Update User #}
{# PATCH /users/{userKey} #}

{# Suspend User #}
{# PATCH /users/{userKey} #}
{
    "suspended": true
}

{# Delete User #}
{# DELETE /users/{userKey} #}
```

### Google Groups

```jinja
{# Create Group #}
{# POST /groups #}
{
    "email": "{{ CTX.group_email }}",
    "name": "{{ CTX.group_name }}",
    "description": "{{ CTX.description | d('') }}"
}

{# Add Member to Group #}
{# POST /groups/{groupKey}/members #}
{
    "email": "{{ CTX.member_email }}",
    "role": "{{ CTX.role | d('MEMBER') }}"  {# OWNER, MANAGER, MEMBER #}
}

{# List Group Members #}
{# GET /groups/{groupKey}/members #}

{# Remove Member #}
{# DELETE /groups/{groupKey}/members/{memberKey} #}
```

### Google Organizational Units

```jinja
{# List OUs #}
{# GET /customer/{customerId}/orgunits?type=all #}

{# Create OU #}
{# POST /customer/{customerId}/orgunits #}
{
    "name": "{{ CTX.ou_name }}",
    "parentOrgUnitPath": "{{ CTX.parent_path | d('/') }}",
    "description": "{{ CTX.description | d('') }}"
}

{# Move User to OU #}
{# PATCH /users/{userKey} #}
{
    "orgUnitPath": "{{ CTX.new_ou_path }}"
}
```

---

## Multi-IdP Patterns

### IdP-Agnostic User Interface

```jinja
{# Unified user operations across IdPs #}
{% set idp = ORG.VARIABLES.identity_provider %}  {# azure_ad, okta, jumpcloud, google #}

{% if idp == "azure_ad" %}
    {% set user_payload = {
        "accountEnabled": CTX.enabled,
        "displayName": CTX.display_name,
        "givenName": CTX.first_name,
        "surname": CTX.last_name,
        "userPrincipalName": CTX.upn,
        "mailNickname": CTX.mail_nickname,
        "department": CTX.department,
        "jobTitle": CTX.job_title,
        "passwordProfile": {
            "password": CTX.temp_password,
            "forceChangePasswordNextSignIn": true
        }
    } %}
    {% set endpoint = "https://graph.microsoft.com/v1.0/users" %}
    {% set method = "POST" %}

{% elif idp == "okta" %}
    {% set user_payload = {
        "profile": {
            "firstName": CTX.first_name,
            "lastName": CTX.last_name,
            "email": CTX.email,
            "login": CTX.email,
            "department": CTX.department,
            "title": CTX.job_title
        },
        "credentials": {
            "password": {"value": CTX.temp_password}
        }
    } %}
    {% set endpoint = "https://" ~ ORG.VARIABLES.okta_domain ~ "/api/v1/users?activate=true" %}
    {% set method = "POST" %}

{% elif idp == "jumpcloud" %}
    {% set user_payload = {
        "email": CTX.email,
        "username": CTX.username,
        "firstname": CTX.first_name,
        "lastname": CTX.last_name,
        "department": CTX.department,
        "jobTitle": CTX.job_title,
        "activated": true
    } %}
    {% set endpoint = "https://console.jumpcloud.com/api/systemusers" %}
    {% set method = "POST" %}

{% elif idp == "google" %}
    {% set user_payload = {
        "name": {
            "givenName": CTX.first_name,
            "familyName": CTX.last_name
        },
        "primaryEmail": CTX.email,
        "password": CTX.temp_password,
        "changePasswordAtNextLogin": true,
        "organizations": [{
            "department": CTX.department,
            "title": CTX.job_title,
            "primary": true
        }]
    } %}
    {% set endpoint = "https://admin.googleapis.com/admin/directory/v1/users" %}
    {% set method = "POST" %}

{% endif %}
```

### Cross-IdP User Sync

```jinja
{# Sync user data between identity providers #}
{% set master_idp = ORG.VARIABLES.master_idp | d("azure_ad") %}
{% set sync_targets = ORG.VARIABLES.sync_targets | d([]) %}  {# ["okta", "jumpcloud"] #}

{# Build canonical user object from master #}
{% if master_idp == "azure_ad" %}
    {% set master_user = CTX.azure_user %}
    {% set canonical = {
        "id": master_user.id,
        "email": master_user.mail | d(master_user.userPrincipalName),
        "first_name": master_user.givenName,
        "last_name": master_user.surname,
        "display_name": master_user.displayName,
        "department": master_user.department,
        "job_title": master_user.jobTitle,
        "manager_email": master_user.manager.mail if master_user.manager else none,
        "enabled": master_user.accountEnabled,
        "phone": master_user.mobilePhone,
        "employee_id": master_user.employeeId
    } %}
{% endif %}

{# For each sync target, prepare update payload #}
{% for target in sync_targets %}
    {% if target == "okta" %}
        {% set okta_update = {
            "profile": {
                "firstName": canonical.first_name,
                "lastName": canonical.last_name,
                "department": canonical.department,
                "title": canonical.job_title,
                "mobilePhone": canonical.phone,
                "employeeNumber": canonical.employee_id
            }
        } %}
    {% elif target == "jumpcloud" %}
        {% set jc_update = {
            "firstname": canonical.first_name,
            "lastname": canonical.last_name,
            "department": canonical.department,
            "jobTitle": canonical.job_title,
            "employeeIdentifier": canonical.employee_id
        } %}
    {% endif %}
{% endfor %}
```

### Federated SSO Configuration

```jinja
{# Common SSO settings for documentation/tickets #}
{% set sso_config = {
    "saml": {
        "entity_id": ORG.VARIABLES.saml_entity_id,
        "sso_url": ORG.VARIABLES.saml_sso_url,
        "slo_url": ORG.VARIABLES.saml_slo_url,
        "certificate": "*** SAML Certificate ***",
        "name_id_format": "urn:oasis:names:tc:SAML:1.1:nameid-format:emailAddress"
    },
    "oidc": {
        "client_id": ORG.VARIABLES.oidc_client_id,
        "authorization_endpoint": ORG.VARIABLES.oidc_auth_endpoint,
        "token_endpoint": ORG.VARIABLES.oidc_token_endpoint,
        "userinfo_endpoint": ORG.VARIABLES.oidc_userinfo_endpoint,
        "scopes": ["openid", "profile", "email"]
    }
} %}
```

---

## Password & MFA Management

### Cross-Platform Password Reset

```jinja
{# Password reset workflow #}
{% set idp = ORG.VARIABLES.identity_provider %}
{% set user_id = CTX.user_id %}

{% if idp == "azure_ad" %}
    {# PATCH https://graph.microsoft.com/v1.0/users/{userId} #}
    {% set payload = {
        "passwordProfile": {
            "password": CTX.new_password,
            "forceChangePasswordNextSignIn": CTX.force_change | d(true)
        }
    } %}
    {% set method = "PATCH" %}

{% elif idp == "okta" %}
    {# POST /api/v1/users/{userId}/credentials/change_password #}
    {% set payload = {
        "oldPassword": {"value": CTX.old_password},
        "newPassword": {"value": CTX.new_password}
    } %}
    {# OR reset without old password (admin): #}
    {# POST /api/v1/users/{userId}/lifecycle/reset_password?sendEmail=false #}

{% elif idp == "jumpcloud" %}
    {# Password set via user update - no direct reset API #}
    {# Must use "Reset Password Email" or update user with new password #}
    {% set payload = {
        "password": CTX.new_password
    } %}
    {% set method = "PUT" %}

{% elif idp == "google" %}
    {# PATCH /users/{userKey} #}
    {% set payload = {
        "password": CTX.new_password,
        "changePasswordAtNextLogin": CTX.force_change | d(true)
    } %}
    {% set method = "PATCH" %}

{% endif %}
```

### MFA Status Checking

```jinja
{# Check MFA enrollment status across IdPs #}
{% set idp = ORG.VARIABLES.identity_provider %}

{% if idp == "azure_ad" %}
    {# GET https://graph.microsoft.com/v1.0/users/{userId}/authentication/methods #}
    {% set methods = CTX.auth_methods %}
    {% set has_mfa = methods | selectattr("@odata.type", "ne", "#microsoft.graph.passwordAuthenticationMethod") | list | length > 0 %}

    {# Method types: #}
    {# #microsoft.graph.microsoftAuthenticatorAuthenticationMethod #}
    {# #microsoft.graph.phoneAuthenticationMethod #}
    {# #microsoft.graph.fido2AuthenticationMethod #}
    {# #microsoft.graph.emailAuthenticationMethod #}

{% elif idp == "okta" %}
    {# GET /api/v1/users/{userId}/factors #}
    {% set factors = CTX.factors %}
    {% set has_mfa = factors | selectattr("status", "eq", "ACTIVE") | list | length > 0 %}

{% elif idp == "jumpcloud" %}
    {# Check user's enable_user_portal_multifactor and totp settings #}
    {% set user = CTX.jc_user %}
    {% set has_mfa = user.mfa.configured | d(false) %}

{% elif idp == "google" %}
    {# GET /users/{userKey}?projection=full #}
    {% set user = CTX.google_user %}
    {% set has_mfa = user.isEnrolledIn2Sv | d(false) %}

{% endif %}
```

---

## Directory Sync Patterns

### HR System to IdP Sync

```jinja
{# Sync from HRIS (BambooHR, Workday, etc.) to IdP #}

{# 1. Fetch employees from HRIS #}
{% set hr_employees = CTX.hr_data %}

{# 2. Fetch existing IdP users #}
{% set idp_users = CTX.idp_users %}

{# 3. Build lookup maps #}
{% set hr_by_email = {} %}
{% for emp in hr_employees %}
    {% set hr_by_email = hr_by_email | combine({emp.email | lower: emp}) %}
{% endfor %}

{% set idp_by_email = {} %}
{% for user in idp_users %}
    {% set idp_by_email = idp_by_email | combine({user.email | lower: user}) %}
{% endfor %}

{# 4. Determine actions #}
{% set to_create = [] %}
{% set to_update = [] %}
{% set to_disable = [] %}

{# New employees - in HR, not in IdP #}
{% for email, emp in hr_by_email.items() %}
    {% if email not in idp_by_email %}
        {% set _ = to_create.append(emp) %}
    {% else %}
        {# Compare fields for updates #}
        {% set idp_user = idp_by_email[email] %}
        {% if emp.department != idp_user.department or emp.title != idp_user.title %}
            {% set _ = to_update.append({"hr": emp, "idp": idp_user}) %}
        {% endif %}
    {% endif %}
{% endfor %}

{# Terminated - in IdP, not in active HR #}
{% for email, user in idp_by_email.items() %}
    {% if email not in hr_by_email and user.enabled %}
        {% set _ = to_disable.append(user) %}
    {% endif %}
{% endfor %}

{# 5. Execute sync operations #}
```

### SCIM Provisioning

```jinja
{# SCIM 2.0 User Schema #}
{
    "schemas": ["urn:ietf:params:scim:schemas:core:2.0:User"],
    "userName": "{{ CTX.username }}",
    "name": {
        "givenName": "{{ CTX.first_name }}",
        "familyName": "{{ CTX.last_name }}"
    },
    "emails": [
        {
            "value": "{{ CTX.email }}",
            "type": "work",
            "primary": true
        }
    ],
    "active": {{ CTX.active | d(true) | lower }},
    "title": "{{ CTX.job_title | d('') }}",
    "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User": {
        "department": "{{ CTX.department | d('') }}",
        "employeeNumber": "{{ CTX.employee_id | d('') }}",
        "manager": {
            "value": "{{ CTX.manager_id | d('') }}"
        }
    }
}

{# SCIM Group Schema #}
{
    "schemas": ["urn:ietf:params:scim:schemas:core:2.0:Group"],
    "displayName": "{{ CTX.group_name }}",
    "members": [
        {% for member_id in CTX.member_ids %}
        {"value": "{{ member_id }}"}{{ "," if not loop.last }}
        {% endfor %}
    ]
}
```

---

## Best Practices

```markdown
## Identity Provider Best Practices

### Security
□ Always use temporary passwords that must be changed
□ Enable MFA enrollment as part of provisioning
□ Use principle of least privilege for role assignments
□ Audit admin role assignments regularly
□ Implement just-in-time access where supported

### Sync & Integration
□ Establish clear source of truth (usually HRIS)
□ Implement bidirectional sync carefully
□ Handle conflicts with defined resolution rules
□ Log all sync operations for audit
□ Test sync in non-production first

### User Lifecycle
□ Automate provisioning from HR triggers
□ Include manager notification in workflows
□ Implement grace periods before full deprovisioning
□ Preserve compliance-required data before deletion
□ Cross-reference users across all IdPs during offboarding

### Operations
□ Monitor API rate limits for bulk operations
□ Implement retry logic for transient failures
□ Cache frequently accessed data (groups, roles)
□ Use batch APIs where available
□ Document custom attribute mappings
```

