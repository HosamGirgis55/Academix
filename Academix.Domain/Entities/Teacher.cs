using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Domain.Entities
{
    public class Teacher: BaseEntity
    {
        // Navigation property to ApplicationUser
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
    
}
