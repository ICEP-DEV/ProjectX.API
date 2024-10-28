using ProjectX.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectX.Service
{
    public interface IAlumnusService
    {
        Task<Alumni> TransferAlumniDataToAlumnusProfile(int alumnusId);

        Task<Alumni> VerifyAlumniByItsPin(int itsPin);

        string GenerateToken();
       // Task<Alumni> SendPasswordResetEmail(string toEmail, string resetLink);
       void SendPasswordResetEmail(string toEmail, string resetLink);

    }
    
}
