using ProjectX.Data;
using ProjectX.Data.Model;
using ProjectX.Data.Model.dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;


namespace ProjectX.Service
{
    public class GuestService: IGuestService
    {
        private readonly AlumniDbContext _alumniDbContext;

        public GuestService(AlumniDbContext alumniDbContext)
        {
            _alumniDbContext = alumniDbContext;
        }

        public async Task<Donation> CreateDonation(DonationDTO donationDTO)
        {
            try
            {
                // Create new donation object
                Donation newDonation = new Donation
                {
                    Name = donationDTO.Name,
                    Surname = donationDTO.Surname,
                    Email = donationDTO.Email,
                    Event = donationDTO.Event
                };

                // Add to the database
                _alumniDbContext.Donation.Add(newDonation);
                await _alumniDbContext.SaveChangesAsync(); // Save the new donation

                return newDonation;
            }
            catch (Exception ex)
            {
                // Optionally, log the exception and handle it as per your requirements
                throw new Exception($"Error while creating donation: {ex.Message}", ex);
            }
        }



    }
}
