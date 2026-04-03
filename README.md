# SubscriptionManagement-PaymentsAPI

Robust RESTful API for managing subscriptions, billing, and aircraft control in SaaS environments and custom solutions. Built with .NET 10 following Clean Architecture principles, with native Stripe integration.

It includes an advanced **Just-In-Time** customer creation flow using webhooks, ensuring consistency between Stripe and the database.

---

# Tech Stack
- .NET 10 / C#
- ASP.NET Core Web API
- Entity Framework Core (PostgreSQL)
- Stripe.NET SDK
- MediatR (CQRS)
- FluentValidation
- JWT Authentication (Bearer)
- Clean Architecture (Jason Taylor template)
  
---

# Project Architecture

The project follows a strict implementation of **Clean Architecture**, ensuring low coupling and high maintainability.

### Domain
- Use cases (Commands / Queries)
- Interfaces (Contracts)
- Validations with FluentValidation
- Orchestration with MediatR
  
### Infrastructure
- Persistence with Entity Framework Core
- Implementation of external services (Stripe)
- Identity configuration (JWT)
- Data access (PostgreSQL)
  
### Web / API
- REST Controllers
- Middleware (Error handling, Multi-tenant)
- DI Configuration
- Authentication Endpoints and Webhooks
  
---

# Main features

- Authentication with JWT
- Full integration with Stripe:
  - Creating checkout sessions
  - Synchronizing subscriptions
  - Handling events via webhooks
- Just-in-Time (JIT) automatic customer creation
- Centralized error handling
- Ready for SaaS or custom solutions
  
---

# Authentication

The API uses JWT-based authentication.

Flow:

1. Log in with email and password
2. Credential validation (bcrypt)
3. JWT token generation
4. Use of the token on protected endpoints
   
---

# Prerequisites

Before running the project, make sure you have the following installed:

- .NET 10 SDK
- PostgreSQL
- Stripe CLI
