using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ProjectX.Data;
using ProjectX.Data.Model;
using ProjectX.Data.Model.bl;
using ProjectX.Data.Model.dto;
using ProjectX.Service;
using System.Data.SqlClient;


namespace ProjectX.API.Controllers
{
    [Route("api/[Controller]/[action]")]
    [EnableCors("corspolicy")]

    public class AdminController : Controller
    {

        private readonly IConfiguration _configuration;
        private readonly AlumniDbContext _alumniDbContext;
        

        public AdminController(IConfiguration configuration, AlumniDbContext alumniDbContext)
        {
            _configuration = configuration;
            _alumniDbContext = alumniDbContext;
           
        }

        [HttpPost]
        //[Route("Login")]

        public Response Login(Admin admin)
        {
            Response response = new Response();

            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("AlumniDb").ToString());

            BusinessLogic bl = new BusinessLogic();
            response = bl.Login(admin, connection);

            return response;
        }

        [HttpGet]
        [Route("GetAlumnis")]
        public IActionResult GetAlumnis()
        {
            return Ok(_alumniDbContext.AlumnusProfile.ToList());
        }

        [HttpGet]
        [Route("CountAlumniPerFaculty")]
        public IActionResult CountAlumniPerFaculty()
        {
            // Group by faculty and count the number of alumni in each faculty
            var facultyCounts = _alumniDbContext.AlumnusProfile
                .GroupBy(a => a.Faculty)
                .Select(g => new
                {
                    Faculty = g.Key,
                    RegisteredAlumni = g.Count()
                })
                .ToList();

            return Ok(facultyCounts);
        }

        [HttpGet]
        [Route("CountAlumniPerCampus")]
        public IActionResult CountAlumniPerCampus()
        {
            // Group by faculty and count the number of alumni in each faculty
            var facultyCounts = _alumniDbContext.AlumnusProfile
                .GroupBy(a => a.Campus)
                .Select(g => new
                {
                    Campus = g.Key,
                    RegisteredAlumni = g.Count()
                })
                .ToList();

            return Ok(facultyCounts);
        }

        [HttpGet]
        [Route("TrackAlumni")]
        public IActionResult TrackAlumni()
        {
            var track = _alumniDbContext.AlumnusProfile
                .GroupBy(a => a.GraduationYear)
                .Select(g => new
                {
                    Graduated = g.Key,
                    GraduatedAlumni = g.Count()
                }).ToList();

            return Ok(track);
        }

        [HttpGet]
        [Route("CountAlumni")]
        public IActionResult CountAlumni()
        {
            var alumni = _alumniDbContext.AlumnusProfile.Count();

            return Ok(alumni);
        }
        [HttpPost]
        [Route("UploadEvents")]
        public async Task<IActionResult> UploadEvents([FromBody] EventsDTO eventsDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Convert base64 string to byte[]
            byte[] mediaBytes = Convert.FromBase64String(eventsDTO.Media);

            // Add event with media as byte[]
            var newEvent = new Event
            {
                Title = eventsDTO.Title,
                Description = eventsDTO.Description,
                Date = eventsDTO.Date,
                Time = eventsDTO.Time,
                Venue = eventsDTO.Venue,
                VolunteerRoles = eventsDTO.VolunteerRoles,
                Media = mediaBytes,
            };

            _alumniDbContext.Add(newEvent);
            await _alumniDbContext.SaveChangesAsync();

            return Ok("Event added successfully");
        }

        [HttpPost]
        [Route("UploadJob")]
        public async Task<IActionResult> UploadJob([FromBody] JobsDTO jobsDTO)
        {
            var newJob = new Job
            {
                Faculty = jobsDTO.Faculty,
                Type = jobsDTO.Type,
                Vacancy = jobsDTO.Vacancy,
                Location = jobsDTO.Location,
                Closingdate = jobsDTO.Closingdate,
                Link = jobsDTO.Link,
            };

           _alumniDbContext. Add(newJob);
            await _alumniDbContext.SaveChangesAsync();
       
            return Ok("Job uploaded successfully");
        }

        [HttpPost]
        [Route("UploadNews")]
        public IActionResult UploadNews([FromForm] NewsDTO newsDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (newsDTO.NewsType != "general" || newsDTO.NewsType != "magazine")
            {
                // Process general news
                return BadRequest("Invalid News type");
            }

            // Convert base64 string to byte[]
            byte[] mediaBytes = Convert.FromBase64String(newsDTO.Media);

            var news = new News
            {
                Headline = newsDTO.Headline,
                Description = newsDTO.Description,
                Publisher = newsDTO.Publisher,
                PublishedDate  = newsDTO.PublishedDate,
                Media = mediaBytes

            };
            
            
            return Ok(new { message = "News uploaded successfully" });
        }


    }
}
