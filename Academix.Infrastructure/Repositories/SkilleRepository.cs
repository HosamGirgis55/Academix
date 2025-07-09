using Academix.Domain.Entities;
using Academix.Domain.Interfaces;
using Academix.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Infrastructure.Repositories
{
    public class SkilleRepository : GenericRepository<Skill>,ISKilleRpository
    {
        public SkilleRepository(ApplicationDbContext context) : base(context) { }
    }
}
