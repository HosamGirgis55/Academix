using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Academix.Domain.Entities;

namespace Academix.Domain.DTOs
{
    public class LookupItemDto:BaseEntity
    {
        public string Name { get; set; }
        public string Code { get; set; }
    }
}
