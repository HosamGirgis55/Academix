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
    public class CommunicationRepository : GenericRepository<Communication>, ICommunicationRepository
    {
        public CommunicationRepository(ApplicationDbContext context) : base(context) { }
    }
}
