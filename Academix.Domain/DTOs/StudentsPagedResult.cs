using System;
using System.Collections.Generic;

namespace Academix.Domain.DTOs
{
    public class StudentsPagedResult
    {
        public List<StudentDto> Students { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
    }
} 