using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Domain.Entities
{
    public class Connections:BaseEntity
    {
        public string ConnectionId { get; set; }
        public string UserId { get; set; }
    }
}
