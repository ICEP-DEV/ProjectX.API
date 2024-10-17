using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectX.Data.Model;
using Microsoft.AspNetCore.Cors;
using ProjectX.Data.Model.dto;
using ProjectX.Data.Model.bl;
using System.Data.SqlClient;
using ProjectX.Service;
using ProjectX.Data;
using Microsoft.EntityFrameworkCore;




namespace ProjectX.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [EnableCors("corspolicy")]

    public class AlumnusController : ControllerBase
    {
        private readonly AlumniDbContext _alumniDbContext;
        private readonly IAlumnusService _alumnusService;
        
        private readonly ILogger<AlumnusController> _logger;

        public AlumnusController(AlumniDbContext alumniDbContext, IAlumnusService alumnusService, ILogger<AlumnusController> logger)
        {
            _alumniDbContext = alumniDbContext;
            _alumnusService = alumnusService;
           
            _logger = logger;
        }



        [HttpPost]
        [Route("Registration")]
        public async Task<IActionResult> Registration([FromBody] AlumnusDTO alumnusDTO,VerifyDTO verifyDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Step 1: Verify if the student exists in the TutDb
            var studentInTutDb = await _alumniDbContext.Alumni.FirstOrDefaultAsync(s => s.AlumnusId == alumnusDTO.StudentNum);
            if (studentInTutDb == null)
            {
                return BadRequest("You are not a TUT alumni. Registration denied.");
            }

            // Step 2: Verify the alumni using the ITS Pin before proceeding
            try
            {
                var verifiedAlumni = await _alumnusService.VerifyAlumniByItsPin(verifyDTO.ItsId);
                if (verifiedAlumni == null)
                {
                    return BadRequest("Registration failed. Invalid ITS Pin.");
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message); // Return an unauthorized response if verification fails
            }

            // Step 3: Check if the alumnus already exists in the Alumnus table
            var objAlumnus = await _alumniDbContext.Alumnus.FirstOrDefaultAsync(a => a.AlumnusId == alumnusDTO.StudentNum);
            if (objAlumnus != null)
            {
                return BadRequest($"Alumnus '{objAlumnus.AlumnusId}' already exists.");
            }

            //step 4: Check if email does not belong to any registered alumni
            var alumnusEmail = await _alumniDbContext.Alumnus.FirstOrDefaultAsync(a => a.Email == alumnusDTO.Email);
            if (alumnusEmail != null)
            {
                return BadRequest($"Email already belongs to another alumni");
            }

            // Step 5: Create a new alumnus and add it to the Alumnus table
            var newAlumnus = new Alumnus
            {
                AlumnusId = alumnusDTO.StudentNum,
                Email = alumnusDTO.Email,
                Password = alumnusDTO.Password
            };

            _alumniDbContext.Alumnus.Add(newAlumnus);
            await _alumniDbContext.SaveChangesAsync(); // Save the new alumnus

            // Step 6: Transfer alumni data after successful registration
            try
            {
                // Call the service to transfer alumni data from TutDb to AlumnusProfile
                await _alumnusService.TransferAlumniDataToAlumnusProfile(newAlumnus.AlumnusId);

                return Ok($"Alumnus {alumnusDTO.StudentNum} has registered successfully, and data has been transferred.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Alumnus registered, but data transfer failed: {ex.Message}");
            }
        }


        [HttpGet]
        [Route("GetAlumnusProfile")]
        public IActionResult GetAlumnusProfile()
        {
            var itsIdString = HttpContext.Session.GetString("ItsId");

            if (string.IsNullOrEmpty(itsIdString))
            {
                return Unauthorized("Alumnus does not exist");
            }

            // Convert UserId to int since AlumnusId is of type int in the database
            if (!int.TryParse(itsIdString, out int userId))
            {
                return BadRequest("Invalid ITS pin.");
            }

            // Fetch the AlumnusProfile using the AlumnusId from session
            var alumnusProfile = _alumniDbContext.AlumnusProfile.FirstOrDefault(a => a.AlumnusId == userId);

            if (alumnusProfile != null)
            {
                return Ok(alumnusProfile);
            }
            else
            {
                return NoContent(); // Return no content if the profile is not found
            }
        }

        [HttpPost]
        [Route("Login")]
        public IActionResult Login([FromBody] LoginDTO loginDTO)
        {
            if (loginDTO.Role == "admin")
            {
                // Check if admin exists by UserId
                var admin = _alumniDbContext.Admin.FirstOrDefault(a => a.AdminId == loginDTO.UserId);

                if (admin != null)
                {
                    // If admin exists, check if the password is correct
                    if (admin.Password == loginDTO.Password)
                    {
                        // Set session variables
                        HttpContext.Session.SetString("UserId", admin.AdminId.ToString());
                        HttpContext.Session.SetString("UserName", admin.Name);
                        HttpContext.Session.SetString("UserRole", "admin");
                        return Ok(admin);
                    }
                    else
                    {
                        return Unauthorized("Incorrect password.");
                    }
                }
                else
                {
                    return NotFound("Admin not found.");
                }
            }
            else if (loginDTO.Role == "alumni")
            {
                // Check if alumnus exists by UserId
                var alumnus = _alumniDbContext.Alumnus.FirstOrDefault(a => a.AlumnusId == loginDTO.UserId);

                if (alumnus != null)
                {
                    // If alumnus exists, check if the password is correct
                    if (alumnus.Password == loginDTO.Password)
                    {
                        // Set session variables
                        HttpContext.Session.SetString("UserId", alumnus.AlumnusId.ToString());
                        HttpContext.Session.SetString("UserEmail", alumnus.Email);
                        HttpContext.Session.SetString("UserRole", "alumni");
                        return Ok(alumnus);
                    }
                    else
                    {
                        return Unauthorized("Incorrect password.");
                    }
                }
                else
                {
                    return NotFound("Alumnus not found. Please sign up.");
                }
            }

            return BadRequest("Invalid role specified.");
        }


        [HttpGet]
        [Route("IsLoggedIn")]
        public IActionResult IsLoggedIn()
        {
            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("IsLoggedIn: User not logged in.");
                return Unauthorized(new { message = "User not logged in." });
            }

            var email = HttpContext.Session.GetString("UserEmail");
            var role = HttpContext.Session.GetString("UserRole");

            _logger.LogInformation($"IsLoggedIn: UserId = {userId}, Email = {email}, Role = {role}");

            return Ok(new
            {
                UserId = userId,
                Email = email,
                Role = role
            });
        }

        [HttpGet]
        [Route("GetLoggedAlumnusProfile")]
        public IActionResult GetLoggedAlumnusProfile()
        {
            var userIdString = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userIdString))
            {
                return Unauthorized("User not logged in.");
            }

            // Convert UserId to int since AlumnusId is of type int in the database
            if (!int.TryParse(userIdString, out int userId))
            {
                return BadRequest("Invalid UserId.");
            }

            // Fetch the AlumnusProfile using the AlumnusId from session
            var alumnusProfile = _alumniDbContext.AlumnusProfile.FirstOrDefault(a => a.AlumnusId == userId);

            if (alumnusProfile != null)
            {
                return Ok(alumnusProfile);
            }
            else
            {
                return NoContent(); // Return no content if the profile is not found
            }
        }

        [HttpPost]
        [Route("Logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Clear all session data
            return Ok(new { message = "Logged out successfully." });
        }


        [HttpGet]
        [Route("GetAlumnis")]
        public IActionResult GetAlumnis()
        {
            return Ok(_alumniDbContext.Alumnus.ToList());
        }

        
    }
}
