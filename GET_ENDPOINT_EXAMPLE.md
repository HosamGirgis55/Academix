# GET Endpoint Example: Entity to Endpoint

This document demonstrates a complete GET endpoint implementation for retrieving a list of courses with filtering, sorting, and pagination.

## Overview

**Endpoint**: `GET /api/courses`

**Features**:
- Pagination
- Filtering by department, search term, and active status
- Sorting by multiple fields
- Include related data (enrollment count)

## Step-by-Step Implementation

### 1. Domain Entity

```csharp
// Academix.Domain/Entities/Course.cs
public class Course : BaseEntity
{
    public string CourseCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Credits { get; set; }
    public string Department { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
```

### 2. Repository Interface

```csharp
// Academix.Domain/Interfaces/ICourseRepository.cs
public interface ICourseRepository : IGenericRepository<Course>
{
    Task<Course?> GetByCourseCodeAsync(string courseCode);
    Task<IEnumerable<Course>> GetCoursesByDepartmentAsync(string department);
    Task<IEnumerable<Course>> GetActiveCoursesAsync();
    Task<Course?> GetCourseWithEnrollmentsAsync(Guid courseId);
}
```

### 3. Repository Implementation

```csharp
// Academix.Infrastructure/Repositories/CourseRepository.cs
public class CourseRepository : GenericRepository<Course>, ICourseRepository
{
    public CourseRepository(ApplicationDbContext context) : base(context)
    {
    }

    // Custom query methods
    public async Task<IEnumerable<Course>> GetActiveCoursesAsync()
    {
        return await _dbSet
            .Where(c => c.IsActive)
            .OrderBy(c => c.Department)
            .ThenBy(c => c.CourseCode)
            .ToListAsync();
    }

    // Override to include default sorting
    public new async Task<IQueryable<Course>> GetAllAsync()
    {
        return await Task.FromResult(_dbSet
            .OrderBy(c => c.Department)
            .ThenBy(c => c.CourseCode)
            .AsQueryable());
    }
}
```

### 4. Query Object (Request)

```csharp
// Academix.Application/Features/Courses/Queries/GetCoursesList/GetCoursesListQuery.cs
public record GetCoursesListQuery : IQuery<Result<CoursesListVm>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? Department { get; init; }
    public string? SearchTerm { get; init; }
    public bool? IsActive { get; init; }
    public string? SortBy { get; init; } = "courseCode";
    public bool IsDescending { get; init; } = false;
}
```

### 5. Response DTOs

```csharp
// View Model for the response
public class CoursesListVm
{
    public List<CourseDto> Courses { get; set; } = new();
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public bool HasPreviousPage { get; set; }
    public bool HasNextPage { get; set; }
}

// DTO for individual course
public class CourseDto
{
    public Guid Id { get; set; }
    public string CourseCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Credits { get; set; }
    public string Department { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int EnrollmentCount { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

### 6. Query Handler (Business Logic)

```csharp
// Academix.Application/Features/Courses/Queries/GetCoursesList/GetCoursesListQueryHandler.cs
public class GetCoursesListQueryHandler : IQueryHandler<GetCoursesListQuery, Result<CoursesListVm>>
{
    private readonly ICourseRepository _courseRepository;

    public async Task<Result<CoursesListVm>> Handle(GetCoursesListQuery request, CancellationToken cancellationToken)
    {
        // 1. Get base query
        var coursesQuery = await _courseRepository.GetAllAsync();

        // 2. Apply filters
        if (!string.IsNullOrWhiteSpace(request.Department))
        {
            coursesQuery = coursesQuery.Where(c => 
                c.Department.ToLower() == request.Department.ToLower());
        }

        if (request.IsActive.HasValue)
        {
            coursesQuery = coursesQuery.Where(c => c.IsActive == request.IsActive.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            coursesQuery = coursesQuery.Where(c =>
                c.CourseCode.ToLower().Contains(searchTerm) ||
                c.Name.ToLower().Contains(searchTerm) ||
                c.Description.ToLower().Contains(searchTerm));
        }

        // 3. Apply sorting
        coursesQuery = ApplySorting(coursesQuery, request.SortBy, request.IsDescending);

        // 4. Get total count
        var totalCount = await coursesQuery.CountAsync(cancellationToken);

        // 5. Apply pagination and project to DTO
        var courses = await coursesQuery
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Include(c => c.Enrollments)
            .Select(c => new CourseDto
            {
                Id = c.Id,
                CourseCode = c.CourseCode,
                Name = c.Name,
                Description = c.Description,
                Credits = c.Credits,
                Department = c.Department,
                IsActive = c.IsActive,
                EnrollmentCount = c.Enrollments.Count,
                CreatedAt = c.CreatedAt
            })
            .ToListAsync(cancellationToken);

        // 6. Build response
        return Result<CoursesListVm>.Success(new CoursesListVm
        {
            Courses = courses,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize),
            HasPreviousPage = request.PageNumber > 1,
            HasNextPage = request.PageNumber < totalPages
        });
    }
}
```

### 7. API Endpoint

```csharp
// Academix.WebAPI/Features/Courses/GetCoursesListEndpoint.cs
public class GetCoursesListEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/courses", HandleAsync)
            .WithName("GetCoursesList")
            .WithTags("Courses")
            .Produces<ResponseHelper>(200)
            .Produces<ResponseHelper>(400);
    }

    private static async Task<IResult> HandleAsync(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? department = null,
        [FromQuery] string? searchTerm = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] string? sortBy = "courseCode",
        [FromQuery] bool isDescending = false,
        [FromServices] ISender mediator = null!,
        CancellationToken cancellationToken = default)
    {
        var query = new GetCoursesListQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            Department = department,
            SearchTerm = searchTerm,
            IsActive = isActive,
            SortBy = sortBy,
            IsDescending = isDescending
        };

        var result = await mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            return Results.Ok(new ResponseHelper()
                .BadRequest(result.Error ?? "Failed to retrieve courses"));
        }

        return Results.Ok(new ResponseHelper()
            .Success(result.Value));
    }
}
```

## Request Flow Diagram

```
1. HTTP GET Request
   ↓
2. Endpoint receives query parameters
   ↓
3. Create GetCoursesListQuery object
   ↓
4. Send to MediatR
   ↓
5. Handler retrieves data from repository
   ↓
6. Apply filters (department, search, active)
   ↓
7. Apply sorting
   ↓
8. Count total records
   ↓
9. Apply pagination
   ↓
10. Project to DTOs
    ↓
11. Return paginated response
```

## Example API Calls

### Basic Request
```http
GET /api/courses
```

**Response:**
```json
{
  "status": true,
  "message": "Operation completed successfully",
  "statusCode": 200,
  "data": {
    "courses": [
      {
        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "courseCode": "CS101",
        "name": "Introduction to Computer Science",
        "description": "Basic concepts of computer science",
        "credits": 3,
        "department": "Computer Science",
        "isActive": true,
        "enrollmentCount": 25,
        "createdAt": "2024-01-15T10:00:00Z"
      }
    ],
    "pageNumber": 1,
    "pageSize": 10,
    "totalCount": 50,
    "totalPages": 5,
    "hasPreviousPage": false,
    "hasNextPage": true
  }
}
```

### Advanced Request with Filters
```http
GET /api/courses?department=Computer Science&searchTerm=programming&isActive=true&pageSize=5&sortBy=name&isDescending=false
```

### Request with Pagination
```http
GET /api/courses?pageNumber=2&pageSize=20
```

## Key Features Demonstrated

### 1. **Filtering**
- By department (exact match)
- By search term (contains in code, name, or description)
- By active status

### 2. **Sorting**
- Multiple sort fields supported
- Ascending/descending order
- Default sorting by course code

### 3. **Pagination**
- Page number and page size
- Total count and pages
- Navigation helpers (hasPrevious/hasNext)

### 4. **Performance Optimizations**
- Queryable pattern for deferred execution
- Single database query with includes
- Projection to DTO in the query

### 5. **Error Handling**
- Try-catch in handler
- Result pattern for consistent responses
- Meaningful error messages

## Testing the Endpoint

### Unit Test Example
```csharp
[Test]
public async Task GetCoursesList_WithFilters_ReturnsFilteredResults()
{
    // Arrange
    var mockRepo = new Mock<ICourseRepository>();
    var handler = new GetCoursesListQueryHandler(mockRepo.Object);
    
    var query = new GetCoursesListQuery
    {
        Department = "Computer Science",
        IsActive = true,
        PageSize = 10
    };

    // Act
    var result = await handler.Handle(query, CancellationToken.None);

    // Assert
    Assert.IsTrue(result.IsSuccess);
    Assert.IsNotNull(result.Value);
}
```

### Integration Test Example
```csharp
[Test]
public async Task GetCourses_EndToEnd_ReturnsPagedResults()
{
    // Arrange
    var client = _factory.CreateClient();

    // Act
    var response = await client.GetAsync("/api/courses?pageSize=5");

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.OK);
    var content = await response.Content.ReadAsStringAsync();
    var result = JsonSerializer.Deserialize<ResponseHelper>(content);
    result.Data.Courses.Should().HaveCountLessOrEqualTo(5);
}
```

## Benefits of This Implementation

1. **Separation of Concerns**: Each layer handles its specific responsibility
2. **Flexibility**: Easy to add new filters or sorting options
3. **Performance**: Efficient database queries with proper pagination
4. **Maintainability**: Clean code structure with single responsibility
5. **Testability**: Each component can be tested independently
6. **Scalability**: Can handle large datasets with pagination

## Common Extensions

1. **Add Caching**: Cache frequently accessed data
2. **Add Specifications**: Use specification pattern for complex queries
3. **Add Export**: Add CSV/Excel export functionality
4. **Add Faceted Search**: Return filter counts
5. **Add Full-Text Search**: Integrate with search engines 