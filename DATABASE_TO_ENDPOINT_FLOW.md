# Database to Endpoint Flow Example

This document demonstrates the complete flow from database to API endpoint using the Student entity as an example.

## Architecture Overview

```
[Database] → [Entity Framework] → [Repository] → [Unit of Work] → [Handler] → [MediatR] → [Endpoint] → [Client]
```

## Layer-by-Layer Implementation

### 1. Database Layer (SQL Server)

```sql
-- Students table created by Entity Framework
CREATE TABLE Students (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    StudentNumber NVARCHAR(20) NOT NULL UNIQUE,
    DateOfBirth DATETIME2 NOT NULL,
    CreatedAt DATETIME2 NOT NULL,
    UpdatedAt DATETIME2 NULL,
    CreatedBy NVARCHAR(MAX) NULL,
    UpdatedBy NVARCHAR(MAX) NULL
);
```

### 2. Domain Layer (`Academix.Domain`)

**Entity:**
```csharp
// Entities/Student.cs
public class Student : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string StudentNumber { get; set; } = string.Empty;
    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
```

**Repository Interface:**
```csharp
// Interfaces/IStudentRepository.cs
public interface IStudentRepository : IGenericRepository<Student>
{
    Task<Student?> GetByEmailAsync(string email);
    Task<Student?> GetByStudentNumberAsync(string studentNumber);
}
```

### 3. Infrastructure Layer (`Academix.Infrastructure`)

**DbContext Configuration:**
```csharp
// Data/ApplicationDbContext.cs
public class ApplicationDbContext : DbContext
{
    public DbSet<Student> Students { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.StudentNumber).IsUnique();
            // ... other configurations
        });
    }
}
```

**Repository Implementation:**
```csharp
// Repositories/StudentRepository.cs
public class StudentRepository : GenericRepository<Student>, IStudentRepository
{
    public async Task<Student?> GetByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(s => s.Email.ToLower() == email.ToLower());
    }
}
```

### 4. Application Layer (`Academix.Application`)

**Command:**
```csharp
// Features/Students/Commands/CreateStudent/CreateStudentCommand.cs
public record CreateStudentCommand : ICommand<Result<Guid>>
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public DateTime DateOfBirth { get; init; }
    public string StudentNumber { get; init; } = string.Empty;
}
```

**Command Handler:**
```csharp
// Features/Students/Commands/CreateStudent/CreateStudentCommandHandler.cs
public class CreateStudentCommandHandler : ICommandHandler<CreateStudentCommand, Result<Guid>>
{
    private readonly IStudentRepository _studentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public async Task<Result<Guid>> Handle(CreateStudentCommand request, CancellationToken cancellationToken)
    {
        // 1. Validate business rules
        var existingStudent = await _studentRepository.GetByEmailAsync(request.Email);
        if (existingStudent != null)
            return Result<Guid>.Failure("A student with this email already exists.");

        // 2. Create entity
        var student = new Student
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            DateOfBirth = request.DateOfBirth,
            StudentNumber = request.StudentNumber,
            CreatedAt = DateTime.UtcNow
        };

        // 3. Save to database
        await _studentRepository.AddAsync(student);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(student.Id);
    }
}
```

**Validator:**
```csharp
// Features/Students/Commands/CreateStudent/CreateStudentCommandValidator.cs
public class CreateStudentCommandValidator : AbstractValidator<CreateStudentCommand>
{
    public CreateStudentCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().EmailAddress();
        
        RuleFor(x => x.StudentNumber)
            .Matches(@"^\d{8}$").WithMessage("Student number must be 8 digits");
    }
}
```

### 5. API Layer (`Academix.WebAPI`)

**Endpoint:**
```csharp
// Features/Students/CreateStudentEndpoint.cs
public class CreateStudentEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/students", HandleAsync)
            .WithName("CreateStudent")
            .WithTags("Students");
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] CreateStudentCommand command,
        [FromServices] ISender mediator,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return Results.Ok(new ResponseHelper().BadRequest(result.Error));

        return Results.Created($"/api/students/{result.Value}", 
            new ResponseHelper().Created(new { studentId = result.Value }));
    }
}
```

### 6. Dependency Injection Setup (`Program.cs`)

```csharp
// Configure services
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(ICommand).Assembly);
});

builder.Services.AddValidatorsFromAssembly(typeof(ICommand).Assembly);
builder.Services.AddAutoMapper(typeof(ICommand).Assembly);

// Map endpoints
app.MapEndpoints();
```

## Complete Request Flow

1. **Client Request**
   ```http
   POST /api/students
   Content-Type: application/json
   
   {
     "firstName": "John",
     "lastName": "Doe",
     "email": "john.doe@example.com",
     "dateOfBirth": "2000-01-15",
     "studentNumber": "20240001"
   }
   ```

2. **Endpoint receives request** → Creates command object → Sends to MediatR

3. **MediatR Pipeline**:
   - Validation behavior runs validator
   - If valid, calls command handler
   - If invalid, returns validation errors

4. **Command Handler**:
   - Injects repository and unit of work
   - Checks business rules (email uniqueness)
   - Creates entity
   - Calls repository to add
   - Calls unit of work to save changes

5. **Repository**:
   - Uses Entity Framework DbContext
   - Adds entity to DbSet
   - Tracks changes

6. **Unit of Work**:
   - Calls `SaveChangesAsync` on DbContext
   - Entity Framework generates SQL INSERT
   - Executes against database

7. **Response flows back**:
   - Handler returns Result with new ID
   - Endpoint formats response
   - Returns HTTP 201 Created with location header

## Query Example (Get Student by ID)

**Query Flow:**
```
GET /api/students/{id} → Endpoint → Query → Handler → Repository → EF Core → Database
```

**Implementation:**
```csharp
// Query
public record GetStudentByIdQuery(Guid StudentId) : IQuery<Result<StudentDto>>;

// Handler
public async Task<Result<StudentDto>> Handle(GetStudentByIdQuery request, CancellationToken cancellationToken)
{
    var student = await _studentRepository.GetByIdAsync(request.StudentId);
    
    if (student == null)
        return Result<StudentDto>.Failure("Student not found");
    
    var dto = _mapper.Map<StudentDto>(student);
    return Result<StudentDto>.Success(dto);
}
```

## Testing the Flow

### 1. Unit Test (Handler)
```csharp
[Test]
public async Task CreateStudent_WithValidData_ReturnsSuccessWithId()
{
    // Arrange
    var mockRepo = new Mock<IStudentRepository>();
    var mockUoW = new Mock<IUnitOfWork>();
    var handler = new CreateStudentCommandHandler(mockRepo.Object, mockUoW.Object);
    
    // Act
    var result = await handler.Handle(new CreateStudentCommand { ... }, CancellationToken.None);
    
    // Assert
    Assert.IsTrue(result.IsSuccess);
    mockRepo.Verify(x => x.AddAsync(It.IsAny<Student>()), Times.Once);
}
```

### 2. Integration Test (Full Flow)
```csharp
[Test]
public async Task CreateStudent_EndToEnd_CreatesStudentInDatabase()
{
    // Arrange
    var client = _factory.CreateClient();
    
    // Act
    var response = await client.PostAsJsonAsync("/api/students", new { ... });
    
    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.Created);
    // Verify in database
}
```

## Benefits of This Architecture

1. **Separation of Concerns**: Each layer has a specific responsibility
2. **Testability**: Easy to mock dependencies and test in isolation
3. **Maintainability**: Changes in one layer don't affect others
4. **Scalability**: Can easily add caching, change database, etc.
5. **Clean Code**: Business logic is separate from infrastructure

## Common Patterns Used

- **Repository Pattern**: Abstracts data access
- **Unit of Work**: Manages transactions
- **CQRS**: Separates reads from writes
- **MediatR**: Decouples handlers from endpoints
- **Result Pattern**: Consistent error handling
- **Vertical Slicing**: Feature-based organization 