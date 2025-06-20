# Academix Codebase Index

## Project Overview
Academix is a .NET 8.0 web application built with a clean architecture pattern, featuring:
- ASP.NET Core Web API
- Entity Framework Core with SQL Server
- Firebase/Google Cloud Storage integration for file uploads
- JWT Bearer authentication support
- Clean Architecture with Domain, Application, Infrastructure, and API layers

## Solution Structure

### Root Files
- **Academix.sln** - Visual Studio solution file containing all projects
- **README.md** - Basic project documentation (currently minimal)
- **.gitignore** - Git ignore file for .NET projects

### Projects Overview

#### 1. **Academix.Domain** (Core Business Layer)
- **Target Framework**: .NET 8.0
- **Purpose**: Contains domain entities, interfaces, and business logic
- **Key Files**:
  - `Interfaces/IGenericRepository.cs` - Generic repository interface with CRUD operations
  - `Class1.cs` - Placeholder class (not yet implemented)

#### 2. **Academix.Application** (Application Services Layer)
- **Target Framework**: .NET 8.0
- **Purpose**: Contains application services and business logic orchestration
- **Dependencies**: References Domain project
- **Key Files**:
  - `Class1.cs` - Placeholder class (not yet implemented)

#### 3. **Academix.Infrastructure** (Data Access Layer)
- **Target Framework**: .NET 8.0
- **Purpose**: Implements data access, external services, and infrastructure concerns
- **Dependencies**:
  - Microsoft.EntityFrameworkCore (8.0.17)
  - Microsoft.EntityFrameworkCore.SqlServer (8.0.17)
  - Microsoft.AspNetCore.Identity.EntityFrameworkCore (8.0.17)
  - References Domain and Application projects
- **Key Files**:
  - `Repositories/GenericRepository.cs` - Implementation of IGenericRepository (currently throws NotImplementedException)
  - `Class1.cs` - Placeholder class
- **Folders** (empty):
  - `Data/` - Intended for DbContext and database configurations
  - `Services/` - Intended for external service implementations

#### 4. **Academix.Helpers** (Utility/Helper Classes)
- **Target Framework**: .NET 8.0
- **Purpose**: Contains helper classes and utilities
- **Dependencies**:
  - FirebaseAdmin (3.2.0)
  - Google.Apis.Auth (1.70.0)
  - Google.Cloud.Storage.V1 (4.13.0)
  - Microsoft.AspNetCore.Mvc packages
  - Microsoft.Extensions.Identity.Core (8.0.17)
- **Key Files**:
  - `FileUploaderHelper.cs` - Handles file uploads to Google Cloud Storage/Firebase
  - `ResponseHelper.cs` - Standardized API response wrapper with builder pattern

#### 5. **Academix.WebAPI** (Presentation Layer)
- **Target Framework**: .NET 8.0
- **Type**: ASP.NET Core Web API
- **Dependencies**:
  - Microsoft.AspNetCore.Authentication.JwtBearer (8.0.17)
  - Swashbuckle.AspNetCore (6.6.2)
  - References all other projects
- **Key Files**:
  - `Program.cs` - Application entry point and configuration
  - `Controllers/HeplerClassesController.cs` - API endpoint for file uploads
  - `appsettings.json` - Application configuration
  - `appsettings.Development.json` - Development environment configuration
  - `Properties/launchSettings.json` - Launch profiles for development

#### 6. **Academix.Helper** (Duplicate Project - Should be removed)
- **Note**: This appears to be a duplicate of Academix.Helpers with incomplete implementation
- Contains an older version of FileUploaderHelper.cs

## Key Components

### 1. Generic Repository Pattern
```csharp
public interface IGenericRepository<T> where T : class
{
    Task<IQueryable<T>> GetAllAsync();
    Task<T> GetByIdAsync(Guid id);
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
}
```

### 2. File Upload System
- **FileUploaderHelper**: Integrates with Google Cloud Storage
  - Reads Firebase configuration from appsettings.json
  - Uploads files to bucket with public read access
  - Returns public URL for uploaded files

### 3. Response Helper
- Provides consistent API response format
- Supports various HTTP status codes
- Includes validation error handling
- Builder pattern for fluent configuration

### 4. API Endpoints
- **POST /api/HeplerClasses/upload** - File upload endpoint

## Configuration Structure
The application uses standard ASP.NET Core configuration with:
- Firebase configuration section for Google Cloud Storage
- JWT Bearer authentication setup (configured but not fully implemented)
- Swagger/OpenAPI for API documentation

## Current Implementation Status

### Implemented:
- ✅ Clean architecture project structure
- ✅ File upload functionality with Google Cloud Storage
- ✅ Standardized API response format
- ✅ Basic API controller setup
- ✅ Swagger documentation

### Not Yet Implemented:
- ❌ Entity Framework DbContext
- ❌ Domain entities
- ❌ Repository pattern implementation (methods throw NotImplementedException)
- ❌ Authentication/Authorization logic
- ❌ Application services
- ❌ Unit tests

## Recommendations

1. **Remove Duplicate Project**: Delete `Academix.Helper` project and update references to use `Academix.Helpers`
2. **Implement Domain Entities**: Create actual domain models instead of placeholder Class1.cs files
3. **Complete Repository Implementation**: Implement the generic repository with Entity Framework
4. **Add DbContext**: Create ApplicationDbContext in Infrastructure/Data
5. **Implement Services**: Add application services in the Application layer
6. **Security**: Implement JWT authentication properly
7. **Testing**: Add unit and integration test projects

## Dependencies Summary

### NuGet Packages:
- Entity Framework Core 8.0.17
- ASP.NET Core Identity 8.0.17
- JWT Bearer Authentication 8.0.17
- Swashbuckle (Swagger) 6.6.2
- Firebase Admin SDK 3.2.0
- Google Cloud Storage 4.13.0

### Project References:
- WebAPI → All other projects
- Infrastructure → Application, Domain
- Application → Domain
- Helpers → Standalone (no project references) 