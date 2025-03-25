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
using System.Security.Cryptography;



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

            // Step 1: Verify if the student exists in the TutDb
            var studentInTutDb = await _alumniDbContext.Alumni.FirstOrDefaultAsync(s => s.AlumnusId == alumnusDTO.StudentNum);
            if (studentInTutDb == null)
            {
                return BadRequest("You are not a TUT alumni. Registration denied.");
            }

            // Step 2: Verify the alumni using the ITS Pin before proceeding
            try
            {
                var verifiedAlumni = await _alumnusService.VerifyAlumniByItsPin(alumnusDTO.ItsPin, alumnusDTO.StudentNum);
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


            // Step 6: Set session variables

            HttpContext.Session.SetString("UserId", newAlumnus.AlumnusId.ToString());



            // Step 7: Transfer alumni data after successful registration
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
            var itsIdString = HttpContext.Session.GetString("UserId");
            Console.WriteLine("UserId in session: " + itsIdString);

            if (string.IsNullOrEmpty(itsIdString))
            {
                return Unauthorized("Alumnus does not exist.");
            }

            if (!int.TryParse(itsIdString, out int userId))
            {
                return BadRequest("Invalid ITS pin.");
            }

            // Fetch the AlumnusProfile using the AlumnusId stored in the session
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

        [HttpPut]
        [Route("UpdateProfile")]
        public async Task<IActionResult> UpdateProfile([FromBody] AlumnusDTO alumnusDTO)
        {
            // Step 1: Find the alumnus profile by StudentNum (AlumnusId)
            var alumni = await _alumniDbContext.AlumnusProfile
                .FirstOrDefaultAsync(a => a.AlumnusId == alumnusDTO.StudentNum);

            if (alumni == null)
            {
                return NotFound("Alumnus Profile not found.");
            }

            // Step 2: Update the LinkedIn profile
            alumni.LinkedInProfile = alumnusDTO.LinkedInProfile;


            //Step 3 Update profile image if it's provided
            if (alumnusDTO.ProfilePicture != null && alumnusDTO.ProfilePicture.Length > 0)
            {

                alumni.ProfilePicture = alumnusDTO.ProfilePicture;

            }

            _alumniDbContext.AlumnusProfile.Update(alumni);
            await _alumniDbContext.SaveChangesAsync();

            return Ok("Profile Updated");
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
                        return Unauthorized("Incorrect credentials.");
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
                        return Unauthorized("Incorrect credentials.");
                    }
                }
                else
                {
                    return NotFound("Alumnus not found. Please sign up.");
                }
            }

            return BadRequest("Invalid role specified.");
        }

        [HttpPost]
        [Route("RSVP")]
        public async Task<IActionResult> RSVP([FromBody] RSVPDTO rSVPDTO)
        {
            if (rSVPDTO == null || rSVPDTO.EventId <= 0  || rSVPDTO.AlumnusId <= 0)
            {
                return BadRequest("Invalid request data. Please provide valid Event ID and Role.");
            }

            var alumnus = await _alumniDbContext.AlumnusProfile.FirstOrDefaultAsync(a => a.AlumnusId == rSVPDTO.AlumnusId);
            if (alumnus == null)
            {
                return NotFound("Alumnus not found.");
            }

            var eventDetails = await _alumniDbContext.Event.FirstOrDefaultAsync(e => e.Id == rSVPDTO.EventId);
            if (eventDetails == null)
            {
                return NotFound("Event not found.");
            }

            var existingRSVP = await _alumniDbContext.RSVPs
                .FirstOrDefaultAsync(v => v.AlumnusId == rSVPDTO.AlumnusId && v.EventId == rSVPDTO.EventId);

            if (existingRSVP != null)
            {
                return BadRequest("You have already RSVP'd for this event with the selected role.");
            }

            var newRSVP = new RSVP
            {
                AlumnusId = rSVPDTO.AlumnusId,
                EventId = rSVPDTO.EventId,               

            };

            Console.WriteLine($"New RSVP: {rSVPDTO.AlumnusId} " + "  :" + rSVPDTO.EventId  + " : " + alumnus.FirstName);


            _alumniDbContext.RSVPs.AddAsync(newRSVP);
            await _alumniDbContext.SaveChangesAsync();

            return Ok(new
            {
                message = "Successfully registered for the event.",
                eventTitle = eventDetails.Title,
               
            });
        }

        [HttpPost]
        [Route("Volunteer")]
        public async Task<IActionResult> Volunteer([FromBody] VolunteerDTO volunteerDTO)
        {
            if (volunteerDTO == null || volunteerDTO.EventId <= 0 || string.IsNullOrEmpty(volunteerDTO.Role) || volunteerDTO.AlumnusId <=0)
            {
                return BadRequest("Invalid request data. Please provide valid Event ID and Role.");
            }

            var alumnus = await _alumniDbContext.AlumnusProfile.FirstOrDefaultAsync(a => a.AlumnusId == volunteerDTO.AlumnusId);
            if (alumnus == null)
            {
                return NotFound("Alumnus not found.");
            }

            var eventDetails = await _alumniDbContext.Event.FirstOrDefaultAsync(e => e.Id == volunteerDTO.EventId);
            if (eventDetails == null)
            {
                return NotFound("Event not found.");
            }
            
            var existingVolunteer = await _alumniDbContext.Volunteers
                .FirstOrDefaultAsync(v => v.AlumnusId == volunteerDTO.AlumnusId && v.EventId == volunteerDTO.EventId && v.Role == volunteerDTO.Role);

            if (existingVolunteer != null)
            {
                return BadRequest("You have already volunteered for this event with the selected role.");
            }

            var newVolunteer = new Volunteer
            {
                AlumnusId = volunteerDTO.AlumnusId,
                EventId = volunteerDTO.EventId,
                Role = volunteerDTO.Role,
               
            };

            Console.WriteLine($"New Vonteer: {volunteerDTO.AlumnusId} " + "  :"+ volunteerDTO.EventId + " : " + volunteerDTO.Role + " : " + alumnus.FirstName);


            _alumniDbContext.Volunteers.AddAsync(newVolunteer);
            await _alumniDbContext.SaveChangesAsync();

            return Ok(new
            {
                message = "Successfully registered for the event.",
                eventTitle = eventDetails.Title,
                role = volunteerDTO.Role
            });
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

        [HttpPut]
        [Route("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] AlumnusDTO alumnusDTO)
        {
            // Step 1: Verify if a record with the specified Email and ItsPin exists for the same alumni
            var alumni = (from a in _alumniDbContext.Alumnus
                          join p in _alumniDbContext.Alumni on a.AlumnusId equals p.AlumnusId
                          where a.Email == alumnusDTO.Email && p.ItsPin == alumnusDTO.ItsPin
                          select a).FirstOrDefault();

            if(alumni==null)
            {
                return BadRequest("Invalid email or ITS pin!");
            }

            //step 3:update password
            alumni.Password = alumnusDTO.Password;
            

            _alumniDbContext.Alumnus.Update(alumni);
            await _alumniDbContext.SaveChangesAsync(); // Save the new password

            return Ok("Password reset successful");
        }

        [HttpGet]
        [Route("GetEvents")]
        public IActionResult GetEvent()
        {
            var events = _alumniDbContext.Event
                .Select(e => new EventsDTO
                {
                    Id = e.Id,
                    Title = e.Title,
                    Description = e.Description,
                    Date = e.Date,
                    Time = e.Time,
                    Venue = e.Venue,
                    VolunteerRoles = e.VolunteerRoles,
                    Media = Convert.ToBase64String(e.Media) // Convert byte[] to Base64 string
                })
                .ToList();

            return Ok(events);
        }
        [HttpGet]
        [Route("GetJobsByFaculty/{faculty}")]
        public IActionResult GetJobsByFaculty(string faculty)
        {
            // Filter jobs by the specified faculty
            var jobs = _alumniDbContext.Job
                .Where(a => a.Faculty.Equals(faculty)) // Case-insensitive comparison
                .ToList();

            // Check if no jobs were found
            if (!jobs.Any())
            {
                return NotFound($"No jobs found for faculty: {faculty}");
            }

            // Return the filtered jobs
            return Ok(jobs);
        }

        [HttpGet]
        [Route("GetNews/{newstype}")]
        public IActionResult GetNewsByType(string newstype)
        {
            // Filter news by type
            var news = _alumniDbContext.News
                .Where(a => a.NewsType.Equals(newstype))
                .Select(a => new NewsDTO
                {
                   Id= a.Id,
                   Headline= a.Headline,
                   Description= a.Description,
                   Publisher=  a.Publisher,
                   PublishedDate= a.PublishedDate,
                   Link= a.Link,
                   NewsType= a.NewsType,
                   Media = Convert.ToBase64String(a.Media)  // Convert byte[] to Base64
                })
                .ToList();

            // Check if no news found
            if (!news.Any())
            {
                return NotFound($"No news found for news type: {newstype}");
            }

            // Return the filtered news with images as Base64
            return Ok(news);
        }
        [HttpGet]
        [Route("GetLatestNews")]
        public IActionResult GetLatestNews()
        {
            var newsType = "general";
            var latest = _alumniDbContext.News
                .Where(a => a.NewsType.Equals(newsType))
                .OrderByDescending(a => a.PublishedDate) // Order by latest news
                .Take(3)  // Limit to first 3 news items
                .Select(a => new NewsDTO
                {
                    Id = a.Id,
                    Headline = a.Headline,
                    Description = a.Description,
                    Publisher = a.Publisher,
                    PublishedDate = a.PublishedDate,
                    Link = a.Link,
                    NewsType = a.NewsType,
                    Media = Convert.ToBase64String(a.Media)  // Convert byte[] to Base64
                })
                .ToList();

            // Check if no news found
            if (!latest.Any())
            {
                return NotFound($"No news found for news type");
            }

            // Return the filtered news with images as Base64
            return Ok(latest);
        }

        [HttpGet]
        [Route("GetBlogs")]
        public IActionResult GetBlogs()
        {
            //get blogs
            var blogs = _alumniDbContext.Blogs
                .Select(a => new BlogsDTO
                {
                    Id= a.Id,
                    Name= a.Name,
                    Role= a.Role,
                    Link= a.Link,
                    Image = Convert.ToBase64String(a.Image) // convert byte[] to Base64 string

                }).ToList();
                
            blogs.Reverse();

            return Ok(blogs);
        }
        

        [HttpPost]
        [Route("Logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Clear all session data
            return Ok(new { message = "Logged out successfully." });
        }


        

        
    }
}
