using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Domain.Entities
{
    public class Country: BaseEntity
    {
        public string NameAr { get; set; }
        public string NameEn { get; set; }

        public ICollection<Student> Students { get; set; } = new List<Student>();
    }
}
