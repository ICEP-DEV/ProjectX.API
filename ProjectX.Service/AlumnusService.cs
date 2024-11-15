using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ProjectX.Data;
using ProjectX.Data.Model;
using System.Net.Mail;
using System.Security.Cryptography;

namespace ProjectX.Service
{
    public class AlumnusService : IAlumnusService
    {
        private readonly AlumniDbContext _alumniDbContext;
        // private readonly IHttpContextAccessor _httpContextAccessor;

        public AlumnusService(AlumniDbContext alumniDbContext)
        {
            _alumniDbContext = alumniDbContext;
            //_httpContextAccessor = HttpContextAccessor;
        }

        public async Task<Alumni> TransferAlumniDataToAlumnusProfile(int alumnusId)
        {
            var alumnus = await _alumniDbContext.Alumnus.FirstOrDefaultAsync(a => a.AlumnusId == alumnusId);

            if (alumnus == null)
            {
                throw new Exception("Alumnus not found.");
            }

            var alumniDetails = await _alumniDbContext.Alumni.FirstOrDefaultAsync(a => a.AlumnusId == alumnusId);

            if (alumniDetails == null)
            {
                throw new Exception("Alumni details not found.");
            }

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

            _alumniDbContext.AlumnusProfile.Add(alumnusProfile);
            await _alumniDbContext.SaveChangesAsync();

            return alumniDetails;
        }

        public async Task<Alumni> VerifyAlumniByItsPin(int itsPin, int alumnusID)
        {
            var alumni = await _alumniDbContext.Alumni.FirstOrDefaultAsync( a => a.ItsPin == itsPin && a.AlumnusId == alumnusID);

            return alumni;
        }

        public async Task<AlumnusProfile> GetAlumnus(int alumnusId)
        {
            var alumnusProfile = await _alumniDbContext.AlumnusProfile.FirstOrDefaultAsync(a => a.AlumnusId == alumnusId);

            if (alumnusProfile == null)
            {
                throw new Exception("Alumnus profile not found.");
            }

            return alumnusProfile;
        }

        
    }
   
}
