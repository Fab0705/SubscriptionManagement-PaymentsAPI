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
```

## 2. Configure environment variables

For this project, User Secrets were used exclusively for Stripe configuration, specifically in the **Web** layer.

```json
{
  "Stripe": {
    "SecretKey": "your_stripe_secretKey",
    "WebhookSecret": "your_stripeCLI_webhook"
  }
}
```

## 3. Run Migrations

Before to do migrations's command, to check if we have `dotnet ef` installed, locally or globally.
You can check it with this command:

```bash
dotnet tool list --global
```

If the the result is like this:

```bash
Package Id           Version      Commands 
-------------------------------------------
dotnet-ef            10.0.8       dotnet-ef
```

It's fine. Buy if this result is empty, so run the next command:

```bash
dotnet tool install --global dotnet-ef
```

Once this is done, you can apply the migration command:

```bash
cd SubscriptionManagement-PaymentsAPI \
dotnet ef database update --project src/Infrastructure --startup-project src/Web
```

## 4. Run Application (in Web Layer) 

```bash
dotnet run --project src/Web/Web.csproj
```

The API will be available at:

```plaintext
https://localhost:5001
```

---

# Stripe Webhooks (Local Testing)

This system relies on webhooks to synchronize payment events. But not before performing a `stripe login` for it to work correctly

## Start Stripe CLI

```bash
stripe listen --forward-to https://localhost:5001/api/webhooks/stripe
```

## Supported events

- `checkout.session.completed`
- `customer.subscription.created`
- `invoice.paid`

## Test flow

1. Create a checkout session from the API
2. Complete payment in Stripe
3. Stripe sends a webhook
4. The API:
   - Create Customer (if it doesn't exist)
   - Create Subscription
   - Register Invoice

---

# Multi-Tenancy

The system architecture already includes a foundation for multi-tenant support.
- Prepared identity structure (`Tenant`, relationships)
- Middleware inicial para resolución de tenant
- Claims in JWT (`tenantId`)

---

# Error Handling

Global middleware catches exceptions:

- `401 Unauthorized` -> Invalid credentials
- `500 Internal Server Error` -> Uncontrolled error

---

# Project Structure

```plaintext
src/
 ├── Domain/
 ├── Application/
 ├── Infrastructure/
 └── Web/

tests/
 ├── Domain.UnitTests/
 ├── Application.UnitTests/
 ├── Application.FunctionalTests/
 └── Infrastructure.IntegrationTests/
```

---

# 🚀 Roadmap

- [ ] Full multi-tenancy implementation
- [ ] Customer portal (self-service)
- [ ] Advanced billing
- [ ] Idempotent webhooks
- [ ] Dockerization
- [ ] CI/CD with Azure

---

# 🤝 Feedback and Contributions

The project is constantly evolving.
Any feedback, suggestions, or improvements are welcome.

You can open an issue or submit a pull request.

---

# Author

Developed as part of a modular system geared towards SaaS and scalable enterprise solutions in .NET and Azure.
