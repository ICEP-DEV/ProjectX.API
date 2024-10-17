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
    public class GuestService : IGuestService
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
                // Map DonationDTO to the actual Donation entity
                Donation newDonation = new Donation
                {
                    Name = donationDTO.Name,
                    Surname = donationDTO.Surname,
                    Email = donationDTO.Email,
                    Phone = donationDTO.Phone,
                    EventOptions = string.Join(", ", donationDTO.EventOptions
                                                .Where(opt => opt.Value) // Get selected events
                                                .Select(opt => opt.Key)) // Convert to string
                };

                // Add to the database
                _alumniDbContext.Donation.Add(newDonation);
                await _alumniDbContext.SaveChangesAsync(); // Save to the database

                return newDonation;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while creating donation: {ex.Message}", ex);
            }
        }
    }

}
