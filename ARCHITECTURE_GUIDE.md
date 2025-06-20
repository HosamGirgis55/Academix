# Academix Architecture Guide: Vertical Slice Architecture with CQRS

## Overview

This guide describes how to implement Vertical Slice Architecture with CQRS (Command Query Responsibility Segregation) in the Academix project, combining the benefits of both patterns for a highly maintainable and scalable application.

## Architecture Patterns

### 1. Vertical Slice Architecture
Instead of organizing code by technical layers (Controllers, Services, Repositories), we organize by features. Each feature contains all the code needed to implement a specific use case.

### 2. CQRS (Command Query Responsibility Segregation)
- **Commands**: Operations that change state (Create, Update, Delete)
- **Queries**: Operations that read state (Get, List, Search)

## Project Structure

```
Academix/
├── Academix.Domain/
│   ├── Entities/
│   │   ├── BaseEntity.cs
│   │   ├── Student.cs
│   │   ├── Course.cs
│   │   └── Enrollment.cs
│   ├── Interfaces/
│   │   ├── IGenericRepository.cs
│   │   ├── IStudentRepository.cs
│   │   ├── ICourseRepository.cs
│   │   └── IUnitOfWork.cs
│   └── ValueObjects/
│       └── Address.cs
├── Academix.Application/
│   ├── Common/
│   │   ├── Interfaces/
│   │   │   ├── ICommand.cs
│   │   │   ├── IQuery.cs
│   │   │   └── IApplicationDbContext.cs
│   │   ├── Models/
│   │   │   ├── Result.cs
│   │   │   └── PaginatedList.cs
│   │   ├── Behaviors/
│   │   │   ├── ValidationBehavior.cs
│   │   │   ├── LoggingBehavior.cs
│   │   │   └── PerformanceBehavior.cs
│   │   └── Mappings/
│   │       └── MappingProfile.cs
│   └── Features/
│       ├── Students/
│       │   ├── Commands/
│       │   │   ├── CreateStudent/
│       │   │   │   ├── CreateStudentCommand.cs
│       │   │   │   ├── CreateStudentCommandHandler.cs
│       │   │   │   └── CreateStudentCommandValidator.cs
│       │   │   ├── UpdateStudent/
│       │   │   └── DeleteStudent/
│       │   └── Queries/
│       │       ├── GetStudentById/
│       │       │   ├── GetStudentByIdQuery.cs
│       │       │   └── GetStudentByIdQueryHandler.cs
│       │       ├── GetStudentsList/
│       │       └── SearchStudents/
│       ├── Courses/
│       └── Enrollments/
├── Academix.Infrastructure/
│   ├── Data/
│   │   ├── ApplicationDbContext.cs
│   │   ├── Configurations/
│   │   └── Migrations/
│   ├── Repositories/
│   │   ├── GenericRepository.cs
│   │   ├── StudentRepository.cs
│   │   └── UnitOfWork.cs
│   └── Services/
│       ├── DateTimeService.cs
│       └── EmailService.cs
├── Academix.WebAPI/
│   ├── Features/
│   │   ├── Students/
│   │   │   ├── CreateStudentEndpoint.cs
│   │   │   ├── UpdateStudentEndpoint.cs
│   │   │   ├── DeleteStudentEndpoint.cs
│   │   │   ├── GetStudentByIdEndpoint.cs
│   │   │   └── GetStudentsListEndpoint.cs
│   │   ├── Courses/
│   │   │   ├── CreateCourseEndpoint.cs
│   │   │   ├── UpdateCourseEndpoint.cs
│   │   │   └── GetCoursesListEndpoint.cs
│   │   ├── Enrollments/
│   │   │   ├── EnrollStudentEndpoint.cs
│   │   │   └── GetEnrollmentsEndpoint.cs
│   │   └── Files/
│   │       └── UploadFileEndpoint.cs
│   ├── Common/
│   │   ├── IEndpoint.cs
│   │   ├── EndpointExtensions.cs
│   │   ├── Middleware/
│   │   └── Filters/
│   └── Program.cs
└── Academix.Helpers/
    ├── FileUploaderHelper.cs
    └── ResponseHelper.cs
```

## Implementation Guide

### Step 1: Install Required NuGet Packages

Update your project files with these packages:

```xml
<!-- Academix.Application.csproj -->
<ItemGroup>
  <PackageReference Include="MediatR" Version="12.2.0" />
  <PackageReference Include="FluentValidation" Version="11.9.0" />
  <PackageReference Include="FluentValidation.DependencyInjectionExtensions" Version="11.9.0" />
  <PackageReference Include="AutoMapper" Version="13.0.1" />
</ItemGroup>

<!-- Academix.WebAPI.csproj -->
<ItemGroup>
  <PackageReference Include="MediatR.Extensions.Microsoft.DependencyInjection" Version="11.1.0" />
  <PackageReference Include="AutoMapper.Extensions.Microsoft.DependencyInjection" Version="12.0.1" />
</ItemGroup>
```

### Step 2: Configure Services in Program.cs

```csharp
using Academix.Application.Common.Behaviors;
using FluentValidation;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

// Add MediatR
builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(Application.AssemblyReference).Assembly);
});

// Add FluentValidation
builder.Services.AddValidatorsFromAssembly(typeof(Application.AssemblyReference).Assembly);

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(Application.AssemblyReference).Assembly);

// Add MediatR Pipeline Behaviors
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));

// Configure Entity Framework
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register repositories
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();

var app = builder.Build();

// Map all endpoints automatically using reflection
app.MapEndpoints();

// Or map specific features
// app.MapFeatureEndpoints<Features.Students.CreateStudentEndpoint>();

app.Run();
```

### Step 3: Implement Pipeline Behaviors

**ValidationBehavior.cs**:
```csharp
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, 
        CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);
            var validationResults = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken)));
            
            var failures = validationResults
                .SelectMany(r => r.Errors)
                .Where(f => f != null)
                .ToList();

            if (failures.Count != 0)
                throw new ValidationException(failures);
        }

        return await next();
    }
}
```

### Step 4: Create Feature Slices

For each feature, follow this pattern:

1. **Command/Query**: Define the request
2. **Handler**: Implement the business logic
3. **Validator**: Add validation rules
4. **Endpoint**: Expose via API (one file per endpoint)

Example for updating a student:

```csharp
// UpdateStudentCommand.cs
public record UpdateStudentCommand : ICommand<Result>
{
    public Guid Id { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    // ... other properties
}

// UpdateStudentCommandHandler.cs
public class UpdateStudentCommandHandler : ICommandHandler<UpdateStudentCommand, Result>
{
    private readonly IStudentRepository _studentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateStudentCommandHandler(IStudentRepository studentRepository, IUnitOfWork unitOfWork)
    {
        _studentRepository = studentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateStudentCommand request, CancellationToken cancellationToken)
    {
        var student = await _studentRepository.GetByIdAsync(request.Id);
        
        if (student == null)
            return Result.Failure($"Student with ID {request.Id} not found.");

        // Update properties
        student.FirstName = request.FirstName;
        student.LastName = request.LastName;
        student.UpdatedAt = DateTime.UtcNow;

        _studentRepository.Update(student);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
```

### Step 5: Implement Read and Write Models Separation

For complex scenarios, separate read and write models:

```csharp
// Write Model (Domain Entity)
public class Student : BaseEntity
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    // Complex business logic here
}

// Read Model (DTO)
public class StudentListDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public int Age { get; set; }
}

// Query Handler using Dapper for reads
public class GetStudentsListQueryHandler : IQueryHandler<GetStudentsListQuery, Result<List<StudentListDto>>>
{
    private readonly IDbConnection _dbConnection;

    public async Task<Result<List<StudentListDto>>> Handle(GetStudentsListQuery request, 
        CancellationToken cancellationToken)
    {
        var sql = @"
            SELECT 
                Id,
                FirstName + ' ' + LastName as FullName,
                Email,
                DATEDIFF(year, DateOfBirth, GETDATE()) as Age
            FROM Students
            WHERE IsActive = 1
            ORDER BY LastName, FirstName";

        var students = await _dbConnection.QueryAsync<StudentListDto>(sql);
        
        return Result<List<StudentListDto>>.Success(students.ToList());
    }
}
```

## Endpoint Pattern

Each endpoint is implemented in its own file following the `IEndpoint` interface:

```csharp
public class CreateStudentEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/students", HandleAsync)
            .WithName("CreateStudent")
            .WithTags("Students")
            .WithOpenApi()
            .Produces<ResponseHelper>(201)
            .Produces<ResponseHelper>(400);
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] CreateStudentCommand command,
        [FromServices] ISender mediator,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(command, cancellationToken);
        // Handle response
    }
}
```

### Benefits of File-Per-Endpoint:
- **Single Responsibility**: Each file has one clear purpose
- **Easy Navigation**: Finding endpoints is straightforward
- **No Merge Conflicts**: Teams can work on different endpoints without conflicts
- **Better Organization**: Related request/response DTOs can live in the same file
- **Automatic Discovery**: Endpoints are discovered and registered via reflection

## Best Practices

### 1. Feature Organization
- Keep all related code in the same feature folder
- Each feature should be self-contained
- Minimize dependencies between features

### 2. Command/Query Guidelines
- Commands should return minimal data (just success/failure or created ID)
- Queries should use DTOs, not domain entities
- Consider using separate read models for complex queries

### 3. Validation
- Use FluentValidation for complex validation rules
- Keep validation close to the command/query
- Return user-friendly error messages

### 4. Testing
```csharp
// Unit test for handler
[TestClass]
public class CreateStudentCommandHandlerTests
{
    [TestMethod]
    public async Task Handle_ValidCommand_CreatesStudent()
    {
        // Arrange
        var repository = new Mock<IStudentRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var handler = new CreateStudentCommandHandler(repository.Object, unitOfWork.Object);
        
        var command = new CreateStudentCommand
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        repository.Verify(x => x.AddAsync(It.IsAny<Student>()), Times.Once);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
```

### 5. Performance Considerations
- Use pagination for list queries
- Consider caching for frequently accessed data
- Use async/await throughout
- Implement proper database indexing

### 6. Security
- Validate all inputs
- Use authorization policies
- Audit sensitive operations
- Implement rate limiting

## Migration Strategy

To migrate from the current architecture:

1. Start with new features - implement them using vertical slices
2. Gradually refactor existing features one at a time
3. Keep both patterns during transition
4. Remove old code once feature is migrated

## Benefits

1. **High Cohesion**: All code for a feature is in one place
2. **Low Coupling**: Features are independent
3. **Easy to Understand**: Clear feature boundaries
4. **Parallel Development**: Teams can work on different features
5. **Performance**: Optimized queries for reads
6. **Scalability**: Easy to scale reads and writes independently

## Common Pitfalls to Avoid

1. Don't share handlers between features
2. Avoid complex inheritance hierarchies
3. Don't put business logic in controllers
4. Keep queries simple - complex logic belongs in domain
5. Don't reuse commands/queries for different purposes

## Resources

- [MediatR Documentation](https://github.com/jbogard/MediatR)
- [FluentValidation Documentation](https://fluentvalidation.net/)
- [Vertical Slice Architecture](https://jimmybogard.com/vertical-slice-architecture/)
- [CQRS Pattern](https://martinfowler.com/bliki/CQRS.html) 