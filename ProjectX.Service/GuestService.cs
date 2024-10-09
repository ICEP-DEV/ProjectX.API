using ProjectX.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectX.Service
{
    public class GuestService: IGuestService
    {
        private readonly AlumniDbContext _alumniDbContext;

        public GuestService(AlumniDbContext alumniDbContext)
        {
            _alumniDbContext = alumniDbContext;
        }

        public async Task<Donation> GetDonation([FromBody] DonationDTO donationDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //create new donation object
            var newDonation = new Donation
            {
                Name = donationDTO.Name,
                Surname = donationDTO.Surname,
                Email = donationDTO.Email,
                Event = donationDTO.Event
            }

            _alumniDbContext.Alumnus.Add(newDonation);
            await _alumniDbContext.SaveChangesAsync(); // Save the new donation
        }
    }
}
