# 💳 SubscriptionManagement-PaymentsAPI

[![.NET](https://img.shields.io/badge/.NET-10.0-purple)](https://dotnet.microsoft.com/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-Blue)](https://www.postgresql.org/)
[![Stripe](https://img.shields.io/badge/Stripe-Integration-indigo)](https://stripe.com/)
[![Architecture](https://img.shields.io/badge/Clean_Architecture-Jason_Taylor-success)](#)

Robust RESTful API for managing subscriptions, billing, and plan control in SaaS environments and custom solutions. Built with .NET 10 following Clean Architecture principles, with native Stripe integration and ready support for multi-tenant architectures.

The project is actively evolving, incorporating continuous improvements and new capabilities geared towards real-world production scenarios.

---

## 🏗️ Tech Stack & Architecture

The project follows a strict implementation of **Clean Architecture**, ensuring low coupling and high maintainability.

| Layer | Technologies & Patterns | Purpose |
| :--- | :--- | :--- |
| **Web / API** | ASP.NET Core Web API, JWT | REST Controllers, Middlewares (Error handling, basis for multi-tenant), DI Configuration, Authentication Endpoints and Webhooks. |
| **Application** | MediatR (CQRS), FluentValidation | Orchestration with MediatR, validations with FluentValidation, and use cases (Commands / Queries). |
| **Domain** | C# / .NET 10 | Interfaces (Contracts). |
| **Infrastructure**| Entity Framework Core, Stripe.NET SDK | PostgreSQL data access, implementation of external services (Stripe), and Identity configuration (JWT). |

---

# Main features

*   **Decoupled Architecture:** Ready to evolve into microservices.
*   **Authentication:** JWT-based flow, including credential validation with bcrypt, token generation, and usage on protected endpoints.
*   **Stripe Integration:** Full support for creating checkout sessions, synchronizing subscriptions, and handling events via webhooks.
*   **Smart Customer Management:** Just-in-Time (JIT) automatic customer creation.
*   **Multi-Tenant Ready:** Structural base in active development, including prepared identity structure, initial middleware for tenant resolution, and claims in JWT (`tenantId`).
*   **Centralized Error Handling:** Global middleware catches exceptions (`401 Unauthorized` for invalid credentials, `500 Internal Server Error` for uncontrolled errors).
  
---

## 📂 Project Structure

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

Before to do migration's command, we need to check if we have `dotnet ef` installed, locally or globally.
You can check it with this command:

```bash
dotnet tool list --global
```

If the result is like this:

```bash
Package Id           Version      Commands 
-------------------------------------------
dotnet-ef            10.0.8       dotnet-ef
```

It's fine. But if this result is empty, so run the next command:

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

# 👨‍💻 Author

Fabian Cristobal
Systems Information & Software Engineering

Developed as part of a modular system geared towards SaaS and scalable enterprise solutions in .NET and Azure[cite: 1].
