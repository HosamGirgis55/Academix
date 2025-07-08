using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Domain.Entities
{

    public class Nationality:BaseEntity
    {
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        // Navigation property to ApplicationUser
        public ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
        
             }
}
