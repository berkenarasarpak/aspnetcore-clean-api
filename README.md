# ASP.NET Core Clean API

Professional ASP.NET Core Web API built with Clean Architecture, featuring JWT authentication, Repository Pattern, Service Layer, Swagger documentation, and comprehensive middleware.

## Architecture Overview

```
┌─────────────────────────────────────────────────────────────┐
│                         API Layer                           │
│  (Controllers, DTOs, Program.cs, Middleware Registration)   │
└──────────────────────┬──────────────────────────────────────┘
                       │
┌──────────────────────▼──────────────────────────────────────┐
│                    Infrastructure Layer                     │
│  (Repository Implementation, EF Core, JWT, Services)        │
└──────────────────────┬──────────────────────────────────────┘
                       │
┌──────────────────────▼──────────────────────────────────────┐
│                     Application Layer                       │
│  (Service Interfaces, DTOs, Validation, Business Logic)     │
└──────────────────────┬──────────────────────────────────────┘
                       │
┌──────────────────────▼──────────────────────────────────────┐
│                       Domain Layer                          │
│          (Entities, Repository Interfaces)                  │
└─────────────────────────────────────────────────────────────┘
```

## Features

- **Clean Architecture**: Separation of concerns with Domain, Application, Infrastructure, and API layers
- **Repository Pattern**: Generic repository with Unit of Work pattern
- **Service Layer**: Business logic separation from controllers
- **JWT Authentication**: Secure token-based authentication with role-based authorization
- **Swagger/OpenAPI**: Interactive API documentation with JWT support
- **Middleware**: Custom exception handling middleware
- **Entity Framework Core**: Code-first approach with SQL Server
- **Dependency Injection**: Proper DI container configuration
- **DTO Pattern**: Data Transfer Objects for API contracts
- **Pagination**: Generic pagination support for list endpoints
- **Validation**: Input validation and error handling
- **Seed Data**: Pre-populated database with sample data

## Tech Stack

- **.NET 8.0**: Latest LTS version
- **Entity Framework Core 8.0**: ORM for database operations
- **SQL Server**: Primary database
- **JWT Bearer**: Authentication middleware
- **BCrypt**: Password hashing
- **Swagger**: API documentation
- **Serilog**: Logging (configured in Infrastructure)

## Project Structure

```
aspnetcore-clean-api/
├── src/
│   ├── Domain/                    # Core entities and interfaces
│   │   ├── Entities/
│   │   │   ├── User.cs
│   │   │   ├── Product.cs
│   │   │   └── Order.cs
│   │   └── Interfaces/
│   │       ├── IGenericRepository.cs
│   │       └── IUnitOfWork.cs
│   │
│   ├── Application/               # Business logic and contracts
│   │   ├── DTOs/
│   │   │   ├── UserDto.cs
│   │   │   └── ProductDto.cs
│   │   └── Interfaces/
│   │       ├── IAuthService.cs
│   │       ├── IProductService.cs
│   │       └── IUserService.cs
│   │
│   ├── Infrastructure/            # Data access and external services
│   │   ├── Data/
│   │   │   └── AppDbContext.cs
│   │   ├── Repositories/
│   │   │   ├── GenericRepository.cs
│   │   │   └── UnitOfWork.cs
│   │   ├── Services/
│   │   │   ├── JwtService.cs
│   │   │   ├── AuthService.cs
│   │   │   ├── ProductService.cs
│   │   │   └── UserService.cs
│   │   └── Middleware/
│   │       └── ExceptionHandlingMiddleware.cs
│   │
│   └── API/                       # Controllers and configuration
│       ├── Controllers/
│       │   ├── AuthController.cs
│       │   ├── ProductsController.cs
│       │   └── UsersController.cs
│       ├── Program.cs
│       └── appsettings.json
│
├── database.sql                   # SQL Server setup script
├── CleanApi.sln                   # Solution file
└── README.md                      # Documentation
```

## Installation

### Prerequisites

- .NET 8.0 SDK or later
- SQL Server (LocalDB, Express, or full version)
- Visual Studio 2022 or VS Code

### Setup

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd aspnetcore-clean-api
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Setup database**
   ```bash
   # Option 1: Run SQL script in SQL Server Management Studio
   # Open database.sql and execute
   
   # Option 2: Use EF Core migrations (after creating them)
   dotnet ef database update
   ```

4. **Update connection string** (if needed)
   Edit `src/API/appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=YOUR_SERVER;Database=CleanApiDb;Trusted_Connection=True;..."
   }
   ```

5. **Run the application**
   ```bash
   cd src/API
   dotnet run
   ```

6. **Access Swagger UI**
   - Navigate to: `https://localhost:7001/swagger` or `http://localhost:5000/swagger`

## API Endpoints

### Authentication

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/api/auth/login` | Authenticate user | No |
| POST | `/api/auth/register` | Register new user | No |
| GET | `/api/auth/me` | Get current user | Yes |

### Products

| Method | Endpoint | Description | Auth Required | Roles |
|--------|----------|-------------|---------------|-------|
| GET | `/api/products` | Get all products (paged) | No | - |
| GET | `/api/products/{id}` | Get product by ID | No | - |
| POST | `/api/products` | Create product | Yes | Admin |
| PUT | `/api/products/{id}` | Update product | Yes | Admin |
| DELETE | `/api/products/{id}` | Delete product | Yes | Admin |
| GET | `/api/products/categories` | Get categories | No | - |

### Users (Admin Only)

| Method | Endpoint | Description | Auth Required | Roles |
|--------|----------|-------------|---------------|-------|
| GET | `/api/users` | Get all users | Yes | Admin |
| GET | `/api/users/{id}` | Get user by ID | Yes | Admin |
| PUT | `/api/users/{id}` | Update user | Yes | Admin |
| DELETE | `/api/users/{id}` | Delete user | Yes | Admin |

## Demo Credentials

```
Admin User:
  Email: admin@cleanapi.com
  Password: password

Regular User:
  Email: user@cleanapi.com
  Password: password
```

## Design Patterns Implemented

### 1. Repository Pattern
- **Generic Repository**: `IGenericRepository<T>` for CRUD operations
- **Unit of Work**: `IUnitOfWork` for transaction management
- **Benefits**: Testability, separation of concerns, single responsibility

### 2. Service Layer Pattern
- **Service Interfaces**: Define contracts in Application layer
- **Service Implementation**: Implement in Infrastructure layer
- **Benefits**: Business logic separation, testability, reusability

### 3. DTO Pattern
- **Request/Response DTOs**: Separate API contracts from entities
- **Benefits**: Data hiding, validation, version control

### 4. Dependency Injection
- **Constructor Injection**: All dependencies injected via constructors
- **Service Registration**: Configured in `Program.cs`
- **Benefits**: Loose coupling, testability, maintainability

### 5. Middleware Pattern
- **Exception Handling**: Global exception handling middleware
- **Benefits**: Centralized error handling, consistent error responses

## Security Features

- **JWT Authentication**: Stateless authentication with tokens
- **Password Hashing**: BCrypt for secure password storage
- **Role-Based Authorization**: Admin/User role separation
- **Input Validation**: Request validation using Data Annotations
- **HTTPS Redirection**: Secure communication
