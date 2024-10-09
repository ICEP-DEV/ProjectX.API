using Microsoft.EntityFrameworkCore;
using ProjectX.Data;
using ProjectX.Data.Model;


namespace ProjectX.Service
{
    public class AlumnusService: IAlumnusService
    {
        
        private readonly AlumniDbContext _alumniDbContext;

        public AlumnusService( AlumniDbContext alumniDbContext)
        {
            
            _alumniDbContext = alumniDbContext;
        }

       public async Task<Alumni> TransferAlumniDataToAlumnusProfile(int alumnusId)
{
            // Fetch the alumnus details from the Alumnus table (AlumnusDb)
            var alumnus = await _alumniDbContext.Alumnus.FirstOrDefaultAsync(a => a.AlumnusId == alumnusId);

            if (alumnus == null)
            {
                throw new Exception("Alumnus not found.");
            }

            // Fetch the related alumni details from the Alumni table (tutDb)
            var alumniDetails = await _alumniDbContext.Alumni.FirstOrDefaultAsync(a => a.AlumnusId == alumnusId);

            if (alumniDetails == null)
            {
                throw new Exception("Alumni details not found.");
            }

            // Create an AlumnusProfile object and populate it with data from Alumni
            AlumnusProfile alumnusProfile = new AlumnusProfile
            {
                AlumnusId = alumnusId,
                FirstName = alumniDetails.FirstName,
                LastName = alumniDetails.LastName,
                Course = alumniDetails.Course,
                GraduationYear = alumniDetails.GraduationYear,
                Campus = alumniDetails.Campus,
                Faculty = alumniDetails.Faculty
            };

            // Save the new AlumnusProfile to the AlumnusDb
            _alumniDbContext.AlumnusProfile.Add(alumnusProfile);
            await _alumniDbContext.SaveChangesAsync();

            // Return the alumni details
            return alumniDetails;
        }

    }
}
