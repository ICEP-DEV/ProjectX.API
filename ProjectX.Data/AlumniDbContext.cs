using Microsoft.EntityFrameworkCore;
using ProjectX.Data.Model;

namespace ProjectX.Data
{
    public class AlumniDbContext: DbContext
    {
        public AlumniDbContext(DbContextOptions<AlumniDbContext> options) : base(options) { }

        public DbSet<Admin> admin { get; set; }
        public DbSet<AlumnusProfile> AlumnusProfile { get; set; }
        public DbSet<Alumnus> Alumnus { get; set; }
    }
}
