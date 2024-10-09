using ProjectX.Service;

namespace ProjectX.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [EnableCors("corspolicy")]
    public class GuestCotroller : ControllerBase
    {
        private readonly AlumniDbContext _alumniDbContext;
        private readonly IGuestService _guestService;

        public GuestCotroller(AlumniDbContext alumniDbContext, IGuestService guestService)
        {
            _alumniDbContext = alumniDbContext;
            _guestService = guestService;
        }
        [HttpPost]
        [Route("GetDonations")]
        public async Task<Donation> GetDonations([FromBody] DonationDTO donationDTO)
        {
            try
            {
                // Call the service to get donations
                await _guestService.GetDonation(donationDTO);

                return Ok($"Donation captured successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $" Donation transfer failed: {ex.Message}");
            }
        }
    }
}
