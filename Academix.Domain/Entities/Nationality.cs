using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Domain.Entities
{

    public class Nationality : loockupBaseEntity
    {

        public ICollection<Student> Students { get; set; } = new List<Student>();
    }
}
