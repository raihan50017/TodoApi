# TodoApi (.NET 8)

A complete Todo REST API with JWT authentication, CRUD, pagination, repository pattern, AutoMapper, global exception handling, validation, and SQL Server (EF Core code-first).

## Features

- **Auth**
  - `POST /api/auth/signup` — register
  - `POST /api/auth/login` — returns JWT
  - `POST /api/auth/refresh` — Get a new access token using a valid refresh token
- **Todos** (JWT required)
  - `GET /api/todos?pageNumber=1&pageSize=10`
  - `GET /api/todos/{id}`
  - `POST /api/todos`
  - `PUT /api/todos/{id}`
  - `DELETE /api/todos/{id}`

## Tech

- .NET 8 Web API
- EF Core (SQL Server), **code-first** (auto create schema on first run via `EnsureCreated()`)
- JWT + Refresh Tokens — Secure authentication & token renewal
- Repository Pattern + Unit of Work
- AutoMapper
- FluentValidation + DataAnnotations
- Global Exception Middleware
- Swagger/OpenAPI

> **Note:** For real projects prefer EF Core Migrations and call `db.Database.Migrate()` at startup. Here `EnsureCreated()` is used for simplicity. Add migrations with:
>
> ```bash
> dotnet ef migrations add InitialCreate
> dotnet ef database update
> ```
> and switch the code in `Program.cs` to `db.Database.Migrate();`

## Prerequisites

- .NET SDK 8
- SQL Server instance
- Update **appsettings.json**:
  - `ConnectionStrings:DefaultConnection`
  - `Jwt:Key` — long random secret

## Run

```bash
dotnet restore
dotnet run
```

Swagger UI: `https://localhost:5001/swagger` (or the shown port).

## Authentication

1. `POST /api/auth/signup`
   ```json
   { "email": "user@example.com", "password": "P@ssw0rd!" }
   ```
2. `POST /api/auth/login` → copy `token` from response
3. In Swagger, click **Authorize**, input `Bearer {token}`

## Postman

Import the collection at `postman/TodoApi.postman_collection.json`. Set `{{baseUrl}}` (e.g., `https://localhost:5001`).

## Folder Structure

```
TodoApi/
  Controllers/
  Data/
  Dtos/
  Entities/
  Mapping/
  Middleware/
  Services/
  Repositories/
  Validators/
  Program.cs
  appsettings.json
  TodoApi.csproj
```

## Security & Best Practices

- Short-lived JWTs, long-lived refresh tokens.
- Strong JWT secret, rotate regularly.
- Store refresh tokens securely (DB only).
- Hash passwords with BCrypt.
- Input validation via FluentValidation.
- Consistent error handling via middleware.


