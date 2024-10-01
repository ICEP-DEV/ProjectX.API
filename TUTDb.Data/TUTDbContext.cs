using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUTDb.Data.Model;

namespace TUTDb.Data
{
    
        public class TUTDbContext : DbContext
        {
            public TUTDbContext(DbContextOptions<TUTDbContext> options) : base(options) { }

            public DbSet<Alumni> Alumni { get; set; }
        }
    
}
