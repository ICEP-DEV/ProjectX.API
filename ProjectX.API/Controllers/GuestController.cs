using Microsoft.AspNetCore.Mvc;
using ProjectX.Data.Model;
using Microsoft.AspNetCore.Cors;
using ProjectX.Data.Model.dto;
using System.Data.SqlClient;
using ProjectX.Service;
using ProjectX.Data;
using Microsoft.EntityFrameworkCore;


namespace ProjectX.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [EnableCors("corspolicy")]
    public class GuestController : ControllerBase
    {
      
        private readonly IGuestService _guestService;

        public GuestController( IGuestService guestService)
        {
            
            _guestService = guestService;
        }
        [HttpPost]
        [Route("CaptureDonation")]
        public async Task<IActionResult> CaptureDonation([FromBody] DonationDTO donationDTO)
        {
            try
            {
                // Call the service to capture the donation
                await _guestService.CreateDonation(donationDTO);

                // Return a success message
                return Ok("Donation captured successfully");
            }
            catch (Exception ex)
            {
                // Handle the exception and return an appropriate status code
                return StatusCode(500, $"Donation transfer failed: {ex.Message}");
            }
        }

    }
}
