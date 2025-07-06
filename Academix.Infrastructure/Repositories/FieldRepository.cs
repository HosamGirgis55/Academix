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
    public class FieldRepository:GenericRepository<Field>,IFieldRepository
    {
        public FieldRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
