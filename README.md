# ecom-paymob-api

A clean and modular **E-commerce Backend API** built using **ASP.NET Core**, following **Clean Architecture**, and integrated with **Paymob** for online payments and webhook handling.

This project is part of a learning journey to build production-style backend systems using solid backend engineering practices.

## 🚀 Features (Planned & In Progress)

- Clean Architecture (Domain → Application → Infrastructure → API)
- Strong business domain modeling (Entities, Value Objects, Domain Rules)
- E-commerce modules:
  - Users
  - Products & Categories
  - Cart
  - Orders & Order Items
  - Payment Transactions
- Paymob Integration:
  - Create payment orders
  - Handle callbacks & webhooks
  - Idempotent payment events
- JWT Authentication (for customers/admins)
- EF Core + Migrations + Repository Pattern
- Structured Logging (Serilog)
- Validation (FluentValidation)
- Unit Tests (xUnit, Moq)


## 🧱 Project Structure (Planned)

Ecom.Paymob.sln
├─ Ecom.Api             → Presentation layer (Controllers, Endpoints, Middlewares)
├─ Ecom.Application     → Use cases, DTOs, Interfaces (Ports), Validation
├─ Ecom.Domain          → Entities, Value Objects, Domain Exceptions & Rules
├─ Ecom.Infrastructure  → EF Core, Repositories, Paymob Client, Migrations
└─ tests/               → Unit & Integration Tests


## 🌱 Branching Strategy

- `main` → stable branch  
- Create a feature branch for each feature:

  feature/money-vo  
  feature/order-domain  
  feature/product-crud  

- Open a Pull Request into `main` for every feature.


## 🧪 Testing (planned)

- Unit tests for:
  - Value Objects
  - Domain rules (order logic, stock rules)
  - Payment flow rules

- Integration tests using:
  - EF Core InMemory
  - Testcontainers (optional)


## 📌 Notes
This repository is built step-by-step as part of a structured learning process.  
Some advanced features (Domain Events, Multi-Tenancy, Advanced Webhook Strategies) will be added later after the core system is complete.