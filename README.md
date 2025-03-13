# Saylo.Centrex - Clean Architecture Microservices Platform

[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://opensource.org/licenses/MIT)

## Overview

Saylo.Centrex is a robust, scalable microservices platform built on clean architecture principles with multi-tenancy support. The platform is designed to provide a comprehensive solution.

## Architecture

### Clean Architecture

This platform strictly follows clean architecture principles:

- **Domain Layer**: Core business logic and entities, free from external dependencies
- **Application Layer**: Business use cases and orchestration
- **Infrastructure Layer**: Technical implementations of interfaces defined in inner layers
- **Presentation Layer**: APIs and user interfaces

### Microservices

The platform is divided into the following microservices:

- **Identity Service**: Authentication, authorization, and user management using OpenIddict for OAuth 2.0/OpenID Connect

- **API Gateway**: Unified entry point for client applications

### Multi-tenancy

Saylo.Centrex supports multi-tenancy with the following features:

- Tenant isolation at data and service levels
- Tenant-specific configuration and customization
- Shared infrastructure with logical separation

## Getting Started

### Prerequisites

- .NET 6.0 SDK or later
- SQL Server (or compatible database)
- Git

### Setup and Installation

1. Clone the repository:
   ```
   git clone https://github.com/yourusername/Saylo.Centrex.git](https://github.com/pentest30/Centrex.Tempalte.git
   cd Centrex.Tempalte
   ```

2. Build the solution:
   ```
   dotnet build
   ```

3. Set up the database:
   ```
   cd Services/Saylo.Centrex.Identity/Saylo.Centrex.Identity.Api
   dotnet ef database update
   ```

4. Run the services:
   ```
   dotnet run --project Services/Saylo.Centrex.ApiGateway/Saylo.Centrex.ApiGateway.csproj
   ```

### Docker Deployment

```
docker-compose up -d
```

## Project Structure

```
├── BuildingBlocks/               # Shared components across services
│   ├── Saylo.Centrex.Domain/         # Domain layer entities and logic
│   ├── Saylo.Centrex.Application/    # Application layer services
│   └── Saylo.Centrex.Infrastructure/ # Infrastructure implementations
├── Services/                     # Microservices
│   ├── Saylo.Centrex.Identity/       # Identity service
│   └── Saylo.Centrex.ApiGateway/     # API Gateway
```

## Development Guidelines

### Domain-Driven Design

- Focus on the core domain and domain logic
- Collaborate with domain experts
- Create a model based on the ubiquitous language

### CQRS Pattern

- Separate command (write) and query (read) operations
- Use MediatR for in-process messaging

### Event-Driven Architecture

- Services communicate through events
- Loose coupling between services
- Event sourcing for critical data

## Multi-Tenancy Implementation

### Tenant Resolution

Tenants are identified through:
- Request headers
- Authentication tokens

### Data Isolation

Data isolation is achieved using:
- Schema-based separation
- Row-level filtering
- Database-per-tenant (for high-security requirements)

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Acknowledgments

- Clean Architecture by Robert C. Martin
- Domain-Driven Design by Eric Evans
- Microservices Patterns by Chris Richardson
- ".NET Microservices: Architecture for Containerized .NET Applications" by Microsoft (eShopOnContainers)
