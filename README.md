# SubscriptionManagement-PaymentsAPI

Robust RESTful API for managing subscriptions, billing, and plan control in SaaS environments and custom solutions. Built with .NET 10 following Clean Architecture principles, with native Stripe integration and ready support for multi-tenant architectures.

The project is actively evolving, incorporating continuous improvements and new capabilities geared towards real-world production scenarios.

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
- Middlewares (Error handling, basis for multi-tenant)
- DI Configuration
- Authentication Endpoints and Webhooks
  
---

# Main features

- Decoupled architecture ready to evolve into microservices
- Authentication with JWT
- Full integration with Stripe:
  - Creating checkout sessions
  - Synchronizing subscriptions
  - Handling events via webhooks
- Just-in-Time (JIT) automatic customer creation
- Centralized error handling
- Ready for SaaS or custom solutions
- ⚙️ Structural base for multi-tenant support (in active development)
  
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

---

# ⚙️ Local Settings

## 1. Clone Repository

```bash
git clone https://github.com/tu-usuario/SubscriptionManagement-PaymentsAPI.git
cd SubscriptionManagement-PaymentsAPI
