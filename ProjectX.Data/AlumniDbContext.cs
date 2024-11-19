using Microsoft.EntityFrameworkCore;
using ProjectX.Data.Model;

namespace ProjectX.Data
{
    public class AlumniDbContext: DbContext
    {
        public AlumniDbContext(DbContextOptions<AlumniDbContext> options) : base(options) { }

        public DbSet<Admin> Admin { get; set; }
        public DbSet<AlumnusProfile> AlumnusProfile { get; set; }
        public DbSet<Alumnus> Alumnus { get; set; }
        public DbSet<Alumni> Alumni { get; set; }
        public DbSet<Donation> Donation { get; set; }
        public DbSet<Event> Event { get; set; }
        public DbSet<Job> Job{ get; set; }

    }
}
