# Instructor Endpoint Example: Complete Implementation

This document demonstrates a complete implementation of a new entity (`Instructor`) with endpoints, following the vertical slice architecture with CQRS pattern.

## Overview

We'll create:
- A new `Instructor` entity
- Repository pattern implementation
- CQRS commands and queries
- API endpoints
- Full validation

## Step 1: Create the Entity

### Domain Entity
```csharp
// Academix.Domain/Entities/Instructor.cs
public class Instructor : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string EmployeeId { get; set; } = string.Empty;
    public DateTime HireDate { get; set; }
    public string? Office { get; set; }
    public string? Bio { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public virtual ICollection<CourseInstructor> CourseInstructors { get; set; } = new List<CourseInstructor>();
}
```

### Join Entity (Many-to-Many with Course)
```csharp
// Academix.Domain/Entities/CourseInstructor.cs
public class CourseInstructor : BaseEntity
{
    public Guid CourseId { get; set; }
    public Guid InstructorId { get; set; }
    public bool IsPrimary { get; set; } = true;
    public DateTime AssignedDate { get; set; }
    
    // Navigation properties
    public virtual Course Course { get; set; } = null!;
    public virtual Instructor Instructor { get; set; } = null!;
}
```

## Step 2: Create Repository Interface

```csharp
// Academix.Domain/Interfaces/IInstructorRepository.cs
public interface IInstructorRepository : IGenericRepository<Instructor>
{
    Task<Instructor?> GetByEmailAsync(string email);
    Task<Instructor?> GetByEmployeeIdAsync(string employeeId);
    Task<IEnumerable<Instructor>> GetInstructorsByDepartmentAsync(string department);
    Task<Instructor?> GetInstructorWithCoursesAsync(Guid instructorId);
    Task<bool> IsEmailUniqueAsync(string email, Guid? excludeInstructorId = null);
}
```

## Step 3: Implement Repository

```csharp
// Academix.Infrastructure/Repositories/InstructorRepository.cs
public class InstructorRepository : GenericRepository<Instructor>, IInstructorRepository
{
    public InstructorRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<bool> IsEmailUniqueAsync(string email, Guid? excludeInstructorId = null)
    {
        var query = _dbSet.Where(i => i.Email.ToLower() == email.ToLower());
        
        if (excludeInstructorId.HasValue)
        {
            query = query.Where(i => i.Id != excludeInstructorId.Value);
        }

        return !await query.AnyAsync();
    }

    public async Task<Instructor?> GetInstructorWithCoursesAsync(Guid instructorId)
    {
        return await _dbSet
            .Include(i => i.CourseInstructors)
                .ThenInclude(ci => ci.Course)
            .FirstOrDefaultAsync(i => i.Id == instructorId);
    }
    
    // ... other methods
}
```

## Step 4: Configure Entity Framework

```csharp
// In ApplicationDbContext.OnModelCreating
modelBuilder.Entity<Instructor>(entity =>
{
    entity.HasKey(e => e.Id);
    
    entity.Property(e => e.Email)
        .IsRequired()
        .HasMaxLength(100);
    
    entity.HasIndex(e => e.Email)
        .IsUnique();
    
    entity.HasIndex(e => e.EmployeeId)
        .IsUnique();
});

modelBuilder.Entity<CourseInstructor>(entity =>
{
    entity.HasIndex(e => new { e.CourseId, e.InstructorId })
        .IsUnique();
});
```

## Step 5: Create Command (POST Endpoint)

### Command
```csharp
// Academix.Application/Features/Instructors/Commands/CreateInstructor/CreateInstructorCommand.cs
public record CreateInstructorCommand : ICommand<Result<Guid>>
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public string Department { get; init; } = string.Empty;
    public string EmployeeId { get; init; } = string.Empty;
    public DateTime HireDate { get; init; }
    public string? Office { get; init; }
    public string? Bio { get; init; }
}
```

### Command Handler
```csharp
public class CreateInstructorCommandHandler : ICommandHandler<CreateInstructorCommand, Result<Guid>>
{
    private readonly IInstructorRepository _instructorRepository;
    private readonly IUnitOfWork _unitOfWork;

    public async Task<Result<Guid>> Handle(CreateInstructorCommand request, CancellationToken cancellationToken)
    {
        // Check uniqueness
        var isEmailUnique = await _instructorRepository.IsEmailUniqueAsync(request.Email);
        if (!isEmailUnique)
        {
            return Result<Guid>.Failure("An instructor with this email already exists.");
        }

        // Create entity
        var instructor = new Instructor
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            // ... map other properties
        };

        await _instructorRepository.AddAsync(instructor);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(instructor.Id);
    }
}
```

### Validator
```csharp
public class CreateInstructorCommandValidator : AbstractValidator<CreateInstructorCommand>
{
    public CreateInstructorCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email must be a valid email address.");

        RuleFor(x => x.EmployeeId)
            .NotEmpty().WithMessage("Employee ID is required.")
            .Matches(@"^[A-Z]{2}\d{6}$")
            .WithMessage("Employee ID must be in format: XX123456");

        RuleFor(x => x.HireDate)
            .LessThanOrEqualTo(DateTime.Today)
            .WithMessage("Hire date cannot be in the future.");
    }
}
```

### POST Endpoint
```csharp
// Academix.WebAPI/Features/Instructors/CreateInstructorEndpoint.cs
public class CreateInstructorEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/instructors", HandleAsync)
            .WithName("CreateInstructor")
            .WithTags("Instructors")
            .Produces<ResponseHelper>(201)
            .Produces<ResponseHelper>(400)
            .Produces<ResponseHelper>(409);
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] CreateInstructorCommand command,
        [FromServices] ISender mediator,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            if (result.Error?.Contains("already exists") ?? false)
            {
                return Results.Ok(new ResponseHelper()
                    .WithStatus(false)
                    .WithMassage(result.Error)
                    .WithStatusCode(409)); // Conflict
            }

            return Results.Ok(new ResponseHelper()
                .BadRequest(result.Error ?? "Failed to create instructor"));
        }

        return Results.Created(
            $"/api/instructors/{result.Value}", 
            new ResponseHelper()
                .Created(new { instructorId = result.Value })
                .WithMassage("Instructor created successfully")
        );
    }
}
```

## Step 6: Create Query (GET Endpoint)

### Query and DTOs
```csharp
// GetInstructorByIdQuery.cs
public record GetInstructorByIdQuery(Guid InstructorId) : IQuery<Result<InstructorDetailDto>>;

public class InstructorDetailDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string EmployeeId { get; set; } = string.Empty;
    public DateTime HireDate { get; set; }
    public string? Office { get; set; }
    public string? Bio { get; set; }
    public bool IsActive { get; set; }
    public List<CourseAssignmentDto> Courses { get; set; } = new();
}
```

### Query Handler
```csharp
public class GetInstructorByIdQueryHandler : IQueryHandler<GetInstructorByIdQuery, Result<InstructorDetailDto>>
{
    private readonly IInstructorRepository _instructorRepository;

    public async Task<Result<InstructorDetailDto>> Handle(
        GetInstructorByIdQuery request, 
        CancellationToken cancellationToken)
    {
        var instructor = await _instructorRepository
            .GetInstructorWithCoursesAsync(request.InstructorId);
        
        if (instructor == null)
        {
            return Result<InstructorDetailDto>.Failure(
                $"Instructor with ID {request.InstructorId} not found.");
        }

        var dto = new InstructorDetailDto
        {
            Id = instructor.Id,
            FirstName = instructor.FirstName,
            // ... map other properties
            Courses = instructor.CourseInstructors
                .Select(ci => new CourseAssignmentDto
                {
                    CourseId = ci.CourseId,
                    CourseCode = ci.Course.CourseCode,
                    CourseName = ci.Course.Name,
                    IsPrimary = ci.IsPrimary,
                    AssignedDate = ci.AssignedDate
                })
                .ToList()
        };
        
        return Result<InstructorDetailDto>.Success(dto);
    }
}
```

### GET Endpoint
```csharp
public class GetInstructorByIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/instructors/{id:guid}", HandleAsync)
            .WithName("GetInstructorById")
            .WithTags("Instructors")
            .Produces<ResponseHelper>(200)
            .Produces<ResponseHelper>(404);
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] Guid id,
        [FromServices] ISender mediator,
        CancellationToken cancellationToken = default)
    {
        var query = new GetInstructorByIdQuery(id);
        var result = await mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            return Results.Ok(new ResponseHelper()
                .NotFound(result.Error ?? "Instructor not found"));
        }

        return Results.Ok(new ResponseHelper()
            .Success(result.Value)
            .WithMassage("Instructor retrieved successfully"));
    }
}
```

## Step 7: Register Dependencies

```csharp
// In Program.cs
builder.Services.AddScoped<IInstructorRepository, InstructorRepository>();
```

## API Usage Examples

### Create Instructor
```http
POST /api/instructors
Content-Type: application/json

{
  "firstName": "John",
  "lastName": "Smith",
  "email": "john.smith@university.edu",
  "phoneNumber": "+1-555-123-4567",
  "department": "Computer Science",
  "employeeId": "CS123456",
  "hireDate": "2020-08-15",
  "office": "Building A, Room 301",
  "bio": "Professor Smith specializes in artificial intelligence..."
}
```

**Success Response (201 Created):**
```json
{
  "status": true,
  "message": "Instructor created successfully",
  "statusCode": 201,
  "data": {
    "instructorId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
  }
}
```

**Conflict Response (409):**
```json
{
  "status": false,
  "message": "An instructor with this email already exists.",
  "statusCode": 409
}
```

### Get Instructor by ID
```http
GET /api/instructors/3fa85f64-5717-4562-b3fc-2c963f66afa6
```

**Success Response (200 OK):**
```json
{
  "status": true,
  "message": "Instructor retrieved successfully",
  "statusCode": 200,
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "firstName": "John",
    "lastName": "Smith",
    "fullName": "John Smith",
    "email": "john.smith@university.edu",
    "phoneNumber": "+1-555-123-4567",
    "department": "Computer Science",
    "employeeId": "CS123456",
    "hireDate": "2020-08-15T00:00:00",
    "office": "Building A, Room 301",
    "bio": "Professor Smith specializes in artificial intelligence...",
    "isActive": true,
    "createdAt": "2024-01-15T10:30:00",
    "updatedAt": null,
    "courses": [
      {
        "courseId": "123e4567-e89b-12d3-a456-426614174000",
        "courseCode": "CS101",
        "courseName": "Introduction to Computer Science",
        "isPrimary": true,
        "assignedDate": "2024-01-20T00:00:00"
      }
    ]
  }
}
```

**Not Found Response (404):**
```json
{
  "status": false,
  "message": "Instructor with ID 3fa85f64-5717-4562-b3fc-2c963f66afa6 not found.",
  "statusCode": 404
}
```

## Key Features Demonstrated

1. **Entity Design**
   - Rich domain model with properties and relationships
   - Many-to-many relationship with courses

2. **Repository Pattern**
   - Generic repository inheritance
   - Custom query methods
   - Include related data

3. **CQRS Implementation**
   - Separate commands and queries
   - Command/Query handlers
   - Result pattern for error handling

4. **Validation**
   - FluentValidation rules
   - Format validation (email, employee ID)
   - Business rule validation

5. **API Design**
   - RESTful endpoints
   - Proper HTTP status codes
   - Consistent response format
   - Location header on creation

6. **Error Handling**
   - Different status codes for different errors
   - Meaningful error messages
   - Conflict detection for duplicates

## Benefits

1. **Maintainability**: Each feature is self-contained
2. **Testability**: Easy to unit test handlers
3. **Scalability**: Easy to add new features
4. **Consistency**: Same pattern for all entities
5. **Documentation**: Self-documenting with Swagger 