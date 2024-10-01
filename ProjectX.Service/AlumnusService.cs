using Microsoft.EntityFrameworkCore;
using ProjectX.Data;
using ProjectX.Data.Model;
using TUTDb.Data;

namespace ProjectX.Service
{
    public class AlumnusService
    {
        private readonly TUTDbContext _tutDbContext;
        private readonly AlumniDbContext _alumniDbContext;

        public AlumnusService(TUTDbContext tutDbContext, AlumniDbContext alumniDbContext)
        {
            _tutDbContext = tutDbContext;
            _alumniDbContext = alumniDbContext;
        }

        public async Task TransferAlumniDataToAlumnusProfile(int alumnusId)
        {
            // Fetch the alumnus details from the Alumnus table (AlumnusDb)
            var alumnus = await _alumniDbContext.Alumnus.FindAsync(alumnusId);
            if (alumnus == null)
            {
                throw new Exception("Alumnus not found.");
            }

            // Fetch the related alumni details from the Alumni table (tutDb)
            var alumniDetails = await _tutDbContext.Alumni.FirstOrDefaultAsync(a => a.AlumnusId == alumnusId);
            if (alumniDetails == null)
            {
                throw new Exception("Alumni details not found.");
            }

            // Create an AlumnusProfile object and populate it with data from Alumni
            AlumnusProfile alumnusProfile = new AlumnusProfile();
          
            alumnusProfile.AlumnusId = alumnusId;
            alumnusProfile.FirstName = alumniDetails.FirstName;
            alumnusProfile.LastName = alumniDetails.LastName;
            alumnusProfile.Course = alumniDetails.Course;
            alumnusProfile.GraduationYear = alumniDetails.GraduationYear;
            alumnusProfile.Campus = alumniDetails.Campus;
            alumnusProfile.Faculty = alumniDetails.Faculty;

            // Save the new AlumnusProfile to the AlumnusDb
            _alumniDbContext.AlumnusProfile.Add(alumnusProfile);
            await _alumniDbContext.SaveChangesAsync();
        }
    }
}
