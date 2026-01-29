# Hotel Management System API

A RESTful API for managing hotel operations, including rooms, guests, and reservations. Built with .NET 9.0, Entity Framework Core, and SQLite.

## Project Structure

- **Hotel.Api**: The web API layer, containing controllers, DTOs, and validators.
- **Hotel.Application**: The core business logic layer, containing domain models, repository interfaces, and use cases.
- **Hotel.Infrastructure**: The data access layer, containing the database context, entity configurations, and repository implementations.
- **Hotel.Tests**: The testing layer, containing unit and integration tests.

## Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Entity Framework Core Tools](https://learn.microsoft.com/en-us/ef/core/cli/dotnet) (install with `dotnet tool install --global dotnet-ef`)

## Getting Started

### 1. Database Setup

To create the SQLite database and apply migrations, run the following command from the root directory:

```bash
dotnet ef database update \
  --project Hotel.Infrastructure \
  --startup-project Hotel.Api
```

### 2. Running the Application

To start the API server, run:

```bash
dotnet run --project Hotel.Api
```

The API will be available at `http://localhost:5069` (or the port specified in your `launchSettings.json`). You can access the Swagger UI at `/swagger` to explore the endpoints.

## Testing

The project includes unit tests for Use Cases and integration tests for Controllers using an in-memory SQLite database.

To run all tests, execute:

```bash
dotnet test
```
