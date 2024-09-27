using Microsoft.EntityFrameworkCore;
using ProjectX.Data.Model;

namespace ProjectX.Data
{
    public class AlumniDbContext: DbContext
    {
        public AlumniDbContext(DbContextOptions<AlumniDbContext> options) : base(options) { }

        public DbSet<Admin> Admins { get; set; }
        public DbSet<Alumnus> Alumnus { get; set; }
        public DbSet<AlumnusProfile> AlumnusProfiles { get; set; }
        public DbSet<AlumnusRegistration> AlumnusRegistrations { get; set; }
    }
}
