using Academix.Domain.Entities;
using Academix.Domain.Interfaces;
using Academix.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Infrastructure.Repositories
{
    internal class CountryRepository : GenericRepository<Country>,ICountryRepository
    {
        public CountryRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
