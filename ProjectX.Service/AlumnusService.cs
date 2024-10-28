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

        public async Task<Alumni> VerifyAlumniByItsPin(int itsPin)
        {
            var alumni = await _alumniDbContext.Alumni.FirstOrDefaultAsync(a => a.ItsPin == itsPin);

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
        // Utility function to generate a unique reset token
        public string GenerateToken()
        {
            using (var cryptoProvider = new RNGCryptoServiceProvider())
            {
                byte[] tokenBytes = new byte[32];
                cryptoProvider.GetBytes(tokenBytes);
                return Convert.ToBase64String(tokenBytes);
            }
        }
        public void SendPasswordResetEmail(string toEmail, string resetLink)
        {
            var fromAddress = new MailAddress("fundiswakhanyi20@gmail.com", "AlumniSpace");
            var toAddress = new MailAddress(toEmail);
            const string subject = "Password Reset Request";
            string body = $"Click the following link to reset your password: {resetLink}";

            using (var smtpClient = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                Credentials = new System.Net.NetworkCredential("alumnispace208@gmail.com", "ALUMNIspace@tut4lyf")
            })
            {
                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                })
                {
                    smtpClient.Send(message);
                }
            }
            throw new NotImplementedException();
        }
    }

   
}
