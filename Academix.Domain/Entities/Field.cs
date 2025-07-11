using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Domain.Entities
{
    public class Field:BaseEntity
    {
         public string NameAr { get; set; }
        public string NameEn { get; set; }
        public List<LearningInterestsStudent>? LearningInterests { get; set; } = new();
    }
}
