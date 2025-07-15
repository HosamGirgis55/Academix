using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Domain.Entities
{
    public class Interview : BaseEntity
    {
        public Guid Id { get; set; }
        public string Email { get; set; }

        public string Name { get; set; }

        public DateTime Date { get; set; }

        public string Link { get; set; }

        public TimeSpan Time { get; set; }

        public Guid TeacherId { get; set; }

        public virtual Teacher Teacher { get; set; } = null;

    }
}
