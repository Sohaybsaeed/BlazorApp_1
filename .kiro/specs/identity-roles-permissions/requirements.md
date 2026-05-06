# Requirements Document

## Introduction

This feature extends the existing **FirstBlazorApp** Blazor Server application to implement a complete, production-ready ASP.NET Core Identity Roles and Permissions system. The project already uses `AspNetUsers` (via `ApplicationUser : IdentityUser<int>`) and has a custom permission system built around `PermissionTemplate`, `PermissionTemplateDetail`, `ApplicationFunctionalities`, and a `DbPermissionHandler` / `PermissionPolicyProvider` pipeline.

The goal is to fully activate and integrate the remaining standard ASP.NET Identity tables — `AspNetRoles`, `AspNetUserRoles`, `AspNetRoleClaims`, `AspNetUserClaims`, `AspNetUserLogins`, and `AspNetUserTokens` — through **Blazor UI management screens** inside the existing application. All authorization must be **fully dynamic**: no hardcoded `[Authorize(Policy = "...")]` attribute strings in Razor pages. Instead, the `PermissionPolicyProvider` / `DbPermissionHandler` pipeline already in place will be extended so that every protected page checks permissions resolved at runtime from the database.

This document serves as the reference specification so the team understands how each Identity table is used and can apply the same patterns in their own projects.

---

## Glossary

- **Identity_System**: The ASP.NET Core Identity subsystem (`Microsoft.AspNetCore.Identity`) responsible for user management, authentication, and authorization, already wired into `IBRetailDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>`.
- **Role_Manager**: The `RoleManager<ApplicationRole>` service used to create, update, delete, and query `AspNetRoles` rows.
- **User_Manager**: The `UserManager<ApplicationUser>` service used to manage users, claims, logins, and tokens.
- **ApplicationUser**: The existing custom class extending `IdentityUser<int>`, stored in `AspNetUsers`.
- **ApplicationRole**: The existing custom class extending `IdentityRole<int>`, stored in `AspNetRoles`.
- **Role**: A named grouping stored in `AspNetRoles`, managed through `Role_Manager`.
- **User_Role**: An assignment linking an `ApplicationUser` to an `ApplicationRole`, stored in `AspNetUserRoles`, managed through `User_Manager`.
- **Role_Claim**: A key-value claim attached to a role in `AspNetRoleClaims`. All users assigned to that role inherit the claim at sign-in.
- **User_Claim**: A key-value claim attached directly to a specific user in `AspNetUserClaims`, overriding or supplementing role-level claims.
- **Permission**: A string value carried in a claim of type `"permission"` that maps to an `ActionMethodName` in `ApplicationFunctionalities`, controlling access to a specific Blazor page or action.
- **Permission_Template**: The existing `PermissionTemplate` / `PermissionTemplateDetail` structure that groups allowed `ApplicationFunctionalities` entries for a named role template.
- **Dynamic_Authorization**: The runtime authorization model where the `PermissionPolicyProvider` builds policies on demand and `DbPermissionHandler` resolves them against the database — with no hardcoded policy strings in Razor `@attribute` directives.
- **Role_Permission_Checker**: The existing `IRolePermissionChecker` / `RolePermissionChecker` service that checks whether the current user's role template grants a given `ActionMethodName`.
- **External_Login**: A record linking a user account to an external OAuth/OpenID provider, stored in `AspNetUserLogins`.
- **Token**: A named string value (e.g., email-confirmation token, password-reset token) stored per user and provider in `AspNetUserTokens`.
- **Seed_Data**: Initial roles, claims, and user-role assignments created at application startup for demonstration purposes.
- **IBRetailDbContext**: The existing `IdentityDbContext<ApplicationUser, ApplicationRole, int>` that already owns all Identity tables.
- **Management_UI**: The set of new Blazor Server pages added under the existing `/users` navigation hub for managing roles, claims, and assignments.

---

## Requirements

### Requirement 1: Role Management UI (AspNetRoles)

**User Story:** As an administrator, I want a Blazor UI screen to create, rename, and deactivate Identity roles, so that I can manage the named role groups that users are assigned to.

#### Acceptance Criteria

1. THE Management_UI SHALL provide a Blazor page at `/users/roles` that lists all existing `ApplicationRole` records retrieved via `Role_Manager`.
2. WHEN an administrator submits a valid role name on the create form, THE Role_Manager SHALL create a new `ApplicationRole` record in `AspNetRoles` and THE Management_UI SHALL display the updated list without a full page reload.
3. WHEN an administrator submits a role name that already exists (case-insensitive), THE Management_UI SHALL display a validation error message and THE Role_Manager SHALL not create a duplicate record.
4. WHEN an administrator submits an empty or whitespace-only role name, THE Management_UI SHALL display a validation error and SHALL NOT invoke `Role_Manager`.
5. WHEN an administrator clicks the edit action for an existing role, THE Management_UI SHALL allow the role name to be changed and SHALL invoke `Role_Manager` to persist the update.
6. WHEN an administrator deletes a role that has no users assigned, THE Role_Manager SHALL remove the `AspNetRoles` record and THE Management_UI SHALL reflect the removal.
7. IF a role deletion is attempted for a role that still has users assigned, THEN THE Management_UI SHALL display a descriptive error message and THE Role_Manager SHALL NOT delete the role.

---

### Requirement 2: User-Role Assignment UI (AspNetUserRoles)

**User Story:** As an administrator, I want a Blazor UI screen to assign and remove Identity roles for individual users, so that I can control which role groups each user belongs to.

#### Acceptance Criteria

1. THE Management_UI SHALL provide a Blazor page at `/users/role-assignments` that displays a list of `ApplicationUser` records and their currently assigned `ApplicationRole` entries, retrieved via `User_Manager`.
2. WHEN an administrator selects a user and a role and confirms the assignment, THE User_Manager SHALL add the `AspNetUserRoles` record and THE Management_UI SHALL reflect the new assignment immediately.
3. WHEN an administrator attempts to assign a role that the selected user already holds, THE Management_UI SHALL display an informational message and THE User_Manager SHALL NOT create a duplicate `AspNetUserRoles` record.
4. WHEN an administrator removes a role assignment, THE User_Manager SHALL delete the corresponding `AspNetUserRoles` record and THE Management_UI SHALL update the displayed assignments.
5. IF the `User_Manager` returns an error during assignment or removal, THEN THE Management_UI SHALL display the error details returned by `User_Manager` and SHALL NOT leave the UI in an inconsistent state.

---

### Requirement 3: Role-Level Claims UI (AspNetRoleClaims)

**User Story:** As an administrator, I want a Blazor UI screen to add and remove claims on roles, so that every user in a role automatically inherits those claims at sign-in.

#### Acceptance Criteria

1. THE Management_UI SHALL provide a Blazor page at `/users/role-claims` that lists all `AspNetRoleClaims` records for a selected role, retrieved via `Role_Manager`.
2. WHEN an administrator selects a role and submits a claim type and claim value, THE Role_Manager SHALL add the claim to `AspNetRoleClaims` and THE Management_UI SHALL display the updated claim list.
3. WHEN an administrator submits a claim with an empty claim type or empty claim value, THE Management_UI SHALL display a validation error and SHALL NOT invoke `Role_Manager`.
4. WHEN an administrator removes a role claim, THE Role_Manager SHALL delete the `AspNetRoleClaims` record and THE Management_UI SHALL update the displayed list.
5. IF `Role_Manager` returns an error when adding or removing a claim, THEN THE Management_UI SHALL display the error message returned by `Role_Manager`.

---

### Requirement 4: User-Level Claims UI (AspNetUserClaims)

**User Story:** As an administrator, I want a Blazor UI screen to add and remove claims directly on individual users, so that I can grant or restrict permissions for a specific user independently of their role.

#### Acceptance Criteria

1. THE Management_UI SHALL provide a Blazor page at `/users/user-claims` that lists all `AspNetUserClaims` records for a selected user, retrieved via `User_Manager`.
2. WHEN an administrator selects a user and submits a claim type and claim value, THE User_Manager SHALL add the claim to `AspNetUserClaims` and THE Management_UI SHALL display the updated claim list.
3. WHEN an administrator submits a claim with an empty claim type or empty claim value, THE Management_UI SHALL display a validation error and SHALL NOT invoke `User_Manager`.
4. WHEN an administrator removes a user claim, THE User_Manager SHALL delete the `AspNetUserClaims` record and THE Management_UI SHALL update the displayed list.
5. IF `User_Manager` returns an error when adding or removing a claim, THEN THE Management_UI SHALL display the error message returned by `User_Manager`.

---

### Requirement 5: Dynamic Permission Enforcement (No Hardcoded Policies)

**User Story:** As a developer, I want all Blazor page authorization to be resolved dynamically from the database at runtime, so that no permission strings are hardcoded in Razor `@attribute` directives and access control can be changed without redeployment.

#### Acceptance Criteria

1. THE Identity_System SHALL enforce authorization on protected Blazor pages exclusively through the existing `PermissionPolicyProvider` and `DbPermissionHandler` pipeline — no Blazor page SHALL use a hardcoded `[Authorize(Policy = "Permission:SomeAction")]` attribute.
2. WHEN a user navigates to a protected Blazor page, THE Dynamic_Authorization system SHALL resolve the required `ActionMethodName` from the page's route or a page-level metadata value and check it against the database via `Role_Permission_Checker`.
3. WHEN `Role_Permission_Checker` determines the current user does not hold the required permission, THE Identity_System SHALL redirect the user to the existing `/not-authorized` page.
4. WHEN an administrator updates role claims or user claims through the Management_UI, THE Role_Permission_Checker SHALL reflect the updated permissions on the next request without requiring an application restart.
5. THE Dynamic_Authorization system SHALL support permission checks driven by both `AspNetRoleClaims` (role-level) and `AspNetUserClaims` (user-level), with user-level claims taking precedence when both exist for the same claim type and value.
6. WHILE a user's session is active, THE Identity_System SHALL re-evaluate permissions from the database on each protected page navigation so that revoked permissions take effect within one navigation cycle.

---

### Requirement 6: Claims Propagation at Sign-In

**User Story:** As a developer, I want role claims and user claims to be loaded into the authentication principal at sign-in, so that the ASP.NET Core authorization middleware can evaluate them on every request.

#### Acceptance Criteria

1. WHEN a user successfully authenticates, THE Identity_System SHALL load all `AspNetRoleClaims` for every role the user belongs to and add them to the `ClaimsIdentity`.
2. WHEN a user successfully authenticates, THE Identity_System SHALL load all `AspNetUserClaims` for that user and add them to the `ClaimsIdentity`.
3. WHEN a user holds both a role claim and a user claim with the same type and value, THE Identity_System SHALL include the claim once in the `ClaimsIdentity` without duplication.
4. IF claim loading fails during sign-in due to a database error, THEN THE Identity_System SHALL deny authentication and return a descriptive error to the login page rather than signing in with incomplete claims.

---

### Requirement 7: External Login Tracking UI (AspNetUserLogins)

**User Story:** As an administrator, I want a Blazor UI screen to view and remove external login providers linked to a user account, so that I can audit and manage third-party authentication connections.

#### Acceptance Criteria

1. THE Management_UI SHALL provide a Blazor page at `/users/external-logins` that lists all `AspNetUserLogins` records for a selected user, showing `LoginProvider`, `ProviderDisplayName`, and `ProviderKey`, retrieved via `User_Manager`.
2. WHEN an administrator selects a user, THE Management_UI SHALL display all external login records associated with that user.
3. WHEN an administrator removes an external login record, THE User_Manager SHALL delete the corresponding `AspNetUserLogins` row and THE Management_UI SHALL update the displayed list.
4. IF `User_Manager` returns an error during removal, THEN THE Management_UI SHALL display the error message and SHALL NOT remove the record from the displayed list.

---

### Requirement 8: Token Management UI (AspNetUserTokens)

**User Story:** As an administrator, I want a Blazor UI screen to view and revoke tokens stored for a user, so that I can manage email-confirmation tokens, password-reset tokens, and other named tokens.

#### Acceptance Criteria

1. THE Management_UI SHALL provide a Blazor page at `/users/tokens` that lists all `AspNetUserTokens` records for a selected user, showing `LoginProvider`, `Name`, and a masked `Value`, retrieved via `User_Manager`.
2. WHEN an administrator selects a user, THE Management_UI SHALL display all token records associated with that user.
3. WHEN an administrator revokes a token, THE User_Manager SHALL remove the corresponding `AspNetUserTokens` row and THE Management_UI SHALL update the displayed list.
4. IF `User_Manager` returns an error during token removal, THEN THE Management_UI SHALL display the error message and SHALL NOT remove the record from the displayed list.
5. THE Management_UI SHALL mask the `Value` column (display as `••••••••`) to prevent accidental exposure of sensitive token values on screen.

---

### Requirement 9: Seed Data for Demonstration

**User Story:** As a developer, I want the application to seed a baseline set of roles, claims, and user-role assignments on startup, so that the POC is immediately demonstrable without manual database setup.

#### Acceptance Criteria

1. WHEN the application starts and the `AspNetRoles` table contains no rows, THE Identity_System SHALL seed at least three roles (e.g., `Admin`, `Manager`, `Viewer`) using `Role_Manager`.
2. WHEN the application starts and seed roles are present, THE Identity_System SHALL attach at least one `permission` claim to each seeded role via `Role_Manager`, mapping to an existing `ActionMethodName` in `ApplicationFunctionalities`.
3. WHEN the application starts and a designated seed user exists in `AspNetUsers`, THE Identity_System SHALL assign the seed user to the `Admin` role using `User_Manager` if the assignment does not already exist.
4. IF any seed operation fails, THEN THE Identity_System SHALL log the failure details and SHALL continue application startup rather than throwing an unhandled exception.
5. THE Seed_Data process SHALL be idempotent: running it multiple times SHALL NOT create duplicate roles, claims, or user-role assignments.

---

### Requirement 10: Navigation Integration

**User Story:** As an administrator, I want all new Identity management screens to be accessible from the existing `/users` navigation hub, so that role and claims management is discoverable alongside existing user management features.

#### Acceptance Criteria

1. THE Management_UI SHALL add navigation entries for Roles, Role Assignments, Role Claims, User Claims, External Logins, and Token Management to the existing `/users` menu page (`Users.razor`).
2. WHEN an authenticated user without the required permission navigates to any Management_UI page, THE Dynamic_Authorization system SHALL redirect the user to `/not-authorized`.
3. THE Management_UI pages SHALL use the existing `AuthenticatedLayout` and SHALL be consistent in visual style with the existing `UserPermissions.razor` and `ViewAndEditUsers.razor` pages.
4. WHILE a user is unauthenticated, THE Identity_System SHALL redirect any attempt to access Management_UI pages to `/login`.

---

### Requirement 11: Developer Documentation

**User Story:** As a developer, I want inline code comments and a summary README in the spec folder explaining how each Identity table is used, so that the team can understand and replicate the patterns in other projects.

#### Acceptance Criteria

1. THE Identity_System implementation SHALL include XML doc comments on all new service classes and interfaces explaining the purpose of each method and which Identity table it operates on.
2. THE Identity_System implementation SHALL include a `README.md` in the spec folder that describes the role of each Identity table (`AspNetRoles`, `AspNetUserRoles`, `AspNetRoleClaims`, `AspNetUserClaims`, `AspNetUserLogins`, `AspNetUserTokens`), the dynamic authorization flow, and how to extend the system.
3. THE Identity_System implementation SHALL include at least one code comment per new Blazor page explaining the authorization pattern used and why no hardcoded policy attribute is present.
