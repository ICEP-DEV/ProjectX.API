using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectX.Data.Model;
using Microsoft.AspNetCore.Cors;
using ProjectX.Data.Model.dto;
using ProjectX.Data.Model.bl;
using System.Data.SqlClient;
using ProjectX.Service;
using ProjectX.Data;




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
        public async Task<IActionResult> Registration([FromBody] AlumnusDTO alumnusDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if the student number exists in the TutDb
            var studentInTutDb = _alumniDbContext.Alumni.FirstOrDefault(s => s.AlumnusId == alumnusDTO.StudentNum);

            if (studentInTutDb == null)
            {
                return BadRequest("You are not a TUT alumni. Registration denied.");
            }

            // Check if the alumnus already exists
            var objAlumnus = _alumniDbContext.Alumnus.FirstOrDefault(a => a.AlumnusId == alumnusDTO.StudentNum);

            if (objAlumnus == null)
            {
                // Create a new alumnus and save it in the Alumnus table
                var newAlumnus = new Alumnus
                {
                    AlumnusId = alumnusDTO.StudentNum,
                    Email = alumnusDTO.Email,
                    Password = alumnusDTO.Password
                };

                _alumniDbContext.Alumnus.Add(newAlumnus);
                await _alumniDbContext.SaveChangesAsync(); // Save changes to the AlumnusDb

                // After successful registration, transfer the alumni data
                 try
                 {
                     // Call the service to transfer alumni data from tutDb to AlumnusProfile
                     await _alumnusService.TransferAlumniDataToAlumnusProfile(newAlumnus.AlumnusId);

                     return Ok($"Alumnus {alumnusDTO.StudentNum} has registered successfully, and data has been transferred.");
                 }
                 catch (Exception ex)
                 {
                     // If the data transfer fails, return an error response
                     return StatusCode(500, $"Alumnus registered, but data transfer failed: {ex.Message}");
                 }

               // return Ok($"Alumnus {alumnusDTO.StudentNum} has registered successfully, and data has been transferred.");
            }
            else
            {
                // If the alumnus already exists, return an error
                return BadRequest($"Alumnus '{objAlumnus.AlumnusId}' already exists.");
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

        [HttpGet]
        [Route("GetAlumnus")]
        public IActionResult GetAlumnus(int id)
        {
            var alumnus = _alumniDbContext.Alumnus.FirstOrDefault(a => a.AlumnusId == id);

            if (alumnus != null)
            {
                return Ok(alumnus);
            }
            else
            {
                return NoContent();
            }

        }
    }
}
