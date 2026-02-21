# WIN – Student Support & Case Management System

### Jump to the [Project Home Page](https://opelly27.github.io/WinStudentGoalTracker/Index.html)

## Overview

**WIN (Wellness & Intervention Network)** is a secure, role-based case management application designed to support student intervention documentation, service tracking, and compliance workflows.

The system centralizes student support cases while maintaining auditability and regulatory documentation alignment.

> ⚠️ Features marked as **TBD** are planned but not yet implemented.

---

## Current Status

- **Version:** 0.1 (MVP Development)
- **Environment:** Development
- **Production Deployment:** TBD
- **SIS Integration:** TBD

---

## Implemented Features (MVP)

- Student profile creation
- Case creation and assignment
- Case notes logging
- Basic service tracking
- Basic role-based access control (RBAC)
- Basic activity logging

---

## Planned Features (Not Yet Implemented)

- Advanced reporting dashboards – TBD
- Workflow automation – TBD
- Notification engine – TBD
- SIS integration – TBD
- Automated compliance validation – TBD
- Consent tracking module – TBD
- Document versioning – TBD
- Analytics & risk scoring – TBD
- External agency portal – TBD

---

## Regulatory & Compliance Alignment

WIN is designed to support documentation workflows aligned with:

- IDEA (Individuals with Disabilities Education Act)
- FERPA (Family Educational Rights and Privacy Act)
- FAPE documentation requirements

### Current Compliance Capabilities

- Timestamped case notes
- User action logging
- Role-based access restrictions

### Planned Compliance Enhancements (TBD)

- Consent management tracking
- Immutable record locking
- Automated compliance reporting
- Formal audit export reports
- Record retention automation

---

## User Roles

| Role | Status | Capabilities |
|------|--------|-------------|
| Administrator | Implemented | User management, full access |
| Case Manager | Implemented | Manage cases, add notes |
| Reviewer | Partial | Read-only access (expanded permissions TBD) |
| Service Provider | Partial | Service logging (expanded features TBD) |

---

## Technical Architecture

### Backend
- RESTful API – Implemented
- Authentication (JWT-based) – Implemented
- Role-based authorization – Implemented
- Audit logging middleware – Basic version implemented

### Database

Relational schema includes:

- Students
- Cases
- Case Notes
- Services
- Users
- Roles
- Audit Logs (basic)

Additional schema validation and optimization – TBD

---

## Security Controls

### Implemented

- HTTPS (environment dependent)
- Role-based access control
- Session authentication

### Planned (TBD)

- Encryption at rest verification
- Fine-grained field-level access control
- File storage encryption
- Automated security monitoring
- Periodic security testing process

---

## Installation

```bash
git clone https://github.com/your-org/win-app.git
cd win-app
npm install
