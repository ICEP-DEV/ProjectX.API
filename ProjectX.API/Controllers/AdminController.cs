using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
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
        private readonly IEmailSender _emailSender;


        public AdminController(IConfiguration configuration, AlumniDbContext alumniDbContext, IEmailSender emailSender)
        {
            _configuration = configuration;
            _alumniDbContext = alumniDbContext;
            _emailSender = emailSender;
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
        [HttpGet]
        [Route("CountVolunteers")]
        public IActionResult CountVolunteers()
        {
            var volunteers = _alumniDbContext.Volunteers.Count();

            return Ok(volunteers);
        }

        [HttpGet]
        [Route("GetEvents")]
        public IActionResult GetEvents()
        {
            var events = _alumniDbContext.Event
                .Select(e => new
                {
                    e.Id,
                    e.Title,
                    e.Date,
                    e.Time,
                    e.Venue,
                    e.Description,
                    ResponseCount = _alumniDbContext.Volunteers.Count(v => v.EventId == e.Id)
                })
                .ToList();

            return Ok(events);
        }
        [HttpGet]
        [Route("GetEventResponses")]
        public IActionResult GetEventResponses()
        {
            var responses = _alumniDbContext.Volunteers
                .Join(
                    _alumniDbContext.AlumnusProfile,
                    volunteer => volunteer.AlumnusId,
                    alumnus => alumnus.AlumnusId,
                    (volunteer, alumnus) => new
                    {
                        
                        AlumnusFirstName = alumnus.FirstName,
                        AlumnusLastName = alumnus.LastName,
                        AlumnusEmail = alumnus.Email,
                        AlumnusCampus = alumnus.Campus,
                        AlumnusCourse = alumnus.Course,
                        AlumnusLinkedInProfile = alumnus.LinkedInProfile,
                        EventId = volunteer.EventId,
                        EventTitle = _alumniDbContext.Event.FirstOrDefault(e => e.Id == volunteer.EventId).Title,
                        EventDate = _alumniDbContext.Event.FirstOrDefault(e => e.Id == volunteer.EventId).Date,
                        VolunteerRole = volunteer.Role,
                    }
                )
                .ToList();

            return Ok(responses);
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

            // Retrieve all alumni emails
            var alumniEmails = await _alumniDbContext.AlumnusProfile
                                    .Select(a => a.Email)
                                    .ToListAsync();


            // Define subject and message dynamically based on NewsType
            string subject;
            string message;


            subject = "Upcoming Event Invitation";

            message = "Dear Alumni,\n\n" +

            $"You are cordially invited to an upcoming event hosted by the Tshwane University of Technology:\n\n"+

            $"Event: {eventsDTO.Title}  \n" +
            $"Description: {eventsDTO.Description} \n"+
            $"Date: {eventsDTO.Date:dddd, MMMM dd, yyyy}\n"+  
            $"Time: {eventsDTO.Time}  \n"+
            $"Venue: {eventsDTO.Venue} \n\n"+ 

            $"For more details, please visit: http://localhost:3000/events \n\n"+

            $"We look forward to your participation!\n\n"+

           $"Kind Regards,  \n"+
           $" Alumni Office \n"+   
           $"Tshwane University of Technology";


            try
            {
                foreach (var email in alumniEmails)
                {
                    await _emailSender.SendEmailAsync(email, subject, message);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Email error: {ex.Message}");
            }

            return Ok("Event added successfully");
        }

        [HttpPut]
        [Route("UpdateEvents/{id}")]
        public async Task<IActionResult> UpdateEvents(int id, [FromBody] EventsDTO eventsDTO)
        {
            var existingEvent = await _alumniDbContext.Event.FindAsync(id);
            if (existingEvent == null)
            {
                return NotFound(new { message = "Event not found" });
            }

            try
            {
                // Update fields regardless of media presence
                existingEvent.Title = eventsDTO.Title;
                existingEvent.Description = eventsDTO.Description;
                
                existingEvent.Date = eventsDTO.Date;
                existingEvent.Time = eventsDTO.Time;
                existingEvent.Venue = eventsDTO.Venue;

                // Handle media if a new image is uploaded
                if (!string.IsNullOrEmpty(eventsDTO.Media))
                {
                    byte[] mediaBytes;
                    try
                    {
                        string base64Data = eventsDTO.Media.Contains(",")
                            ? eventsDTO.Media.Substring(eventsDTO.Media.IndexOf(",") + 1)
                            : eventsDTO.Media;

                        mediaBytes = Convert.FromBase64String(base64Data);
                        existingEvent.Media = mediaBytes;
                    }
                    catch (FormatException)
                    {
                        return BadRequest(new { message = "Invalid media format" });
                    }
                }

                await _alumniDbContext.SaveChangesAsync();
                return Ok(new { message = "Event updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
            
        }

        [HttpDelete]
        [Route("DeleteEvent/{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            // Find the existing news item in the database
            var existingEvent = await _alumniDbContext.Event.FindAsync(id);
            if (existingEvent == null)
            {
                return NotFound(new { message = "Event not found" });
            }

            try
            {
                _alumniDbContext.Event.Remove(existingEvent); // Correct entity deletion
                await _alumniDbContext.SaveChangesAsync(); // Save changes to the database

                return Ok(new { message = "Event deleted successfully" }); // Return a success message
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }

        [HttpPost]
        [Route("UploadJob")]
        public async Task<IActionResult> UploadJob([FromBody] JobsDTO jobsDTO)
        {
            // Create a new job entity
            var newJob = new Job
            {
                Faculty = jobsDTO.Faculty,
                Type = jobsDTO.Type,
                Vacancy = jobsDTO.Vacancy,
                Location = jobsDTO.Location,
                Closingdate = jobsDTO.Closingdate,
                Link = jobsDTO.Link,
            };

            _alumniDbContext.Add(newJob);
            await _alumniDbContext.SaveChangesAsync();

            // Retrieve alumni emails filtered by faculty
            var alumniEmails = await _alumniDbContext.AlumnusProfile
                                       .Where(a => a.Faculty == newJob.Faculty) // Safe comparison
                                       .Select(a => a.Email)
                                       .ToListAsync();

            // Define subject and message dynamically
            string subject = $"New Job Opportunity: {newJob.Vacancy}";


               string message = $"Dear {newJob.Faculty} Alumni,\n\n" +

               $"A new job opportunity has been posted that may be of interest to you:\n\n" +

               $"Job Title: {newJob.Vacancy} \n" +
               $"Type: {newJob.Type} \n" +
               $"Location: {newJob.Location}\n" +
               $"Closing Date: {newJob.Closingdate:dddd, MMMM dd, yyyy} \n" + 

               $"For more details and to apply, please visit: {newJob.Link} \n\n" +

              $"Kind Regards,  \n" +
              $" Alumni Office \n" +
              $"Tshwane University of Technology";


            // Send emails
            try
            {
                foreach (var email in alumniEmails)
                {
                    await _emailSender.SendEmailAsync(email, subject, message);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Email error: {ex.Message}");
            }

            return Ok("Job uploaded successfully");
        }

        [HttpPut]
        [Route("UpdateJob/{id}")]
        public async Task<IActionResult> UpdateJob(int id, [FromBody] JobsDTO jobsDTO)
        {
            var existingJob = await _alumniDbContext.Job.FindAsync(id);
            if (existingJob == null)
            {
                return NotFound(new {message = "Job not found" });
            }

            //update job fields
            existingJob.Faculty = jobsDTO.Faculty;
            existingJob.Type = jobsDTO.Type;
            existingJob.Vacancy = jobsDTO.Vacancy;
            existingJob.Location = jobsDTO.Location;
            existingJob.Closingdate = jobsDTO.Closingdate;
            existingJob.Link = jobsDTO.Link;

            await _alumniDbContext.SaveChangesAsync();
            return Ok(new { message = "Job updated successfully" });
        }

        [HttpDelete]
        [Route("DeleteJob/{id}")]
        public async Task<IActionResult> DeleteJob(int id)
        {
            // Find the existing news item in the database
            var existingJob = await _alumniDbContext.Job.FindAsync(id);
            if (existingJob == null)
            {
                return NotFound(new { message = "Job not found" });
            }

            try
            {
                _alumniDbContext.Job.Remove(existingJob); // Correct entity deletion
                await _alumniDbContext.SaveChangesAsync(); // Save changes to the database

                return Ok(new { message = "Job deleted successfully" }); // Return a success message
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }

        [HttpPost]
        [Route("UploadNews")]
        public async Task<IActionResult> UploadNews([FromForm] NewsDTO newsDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (newsDTO.NewsType != "general" && newsDTO.NewsType != "magazine")
            {
                return BadRequest("Invalid News type");
            }

            byte[] mediaBytes;
            try
            {
                // Remove prefix if it exists
                string base64Data = newsDTO.Media.Contains(",") ? newsDTO.Media.Substring(newsDTO.Media.IndexOf(",") + 1) : newsDTO.Media;
                mediaBytes = Convert.FromBase64String(base64Data);
            }
            catch (FormatException)
            {
                return BadRequest("Invalid media format");
            }

            var newNews = new News
            {
                NewsType = newsDTO.NewsType,
                Headline = newsDTO.Headline,
                Description = newsDTO.Description,
                Publisher = newsDTO.Publisher,
                PublishedDate = newsDTO.PublishedDate,
                Link = newsDTO.Link,
                Media = mediaBytes
            };

            _alumniDbContext.Add(newNews);
            await _alumniDbContext.SaveChangesAsync();

            // Retrieve all alumni emails
            var alumniEmails = await _alumniDbContext.AlumnusProfile
                                    .Select(a => a.Email)
                                    .ToListAsync();


            // Define subject and message dynamically based on NewsType
            string subject;
            string message;

            if (newsDTO.NewsType == "magazine")
            {
                subject = "New Magazine Release";
                message = "Dear Alumni,\n" +
                           "A new magazine issue has been published:\n" +
                           $"{newsDTO.Description}\n\n" +
                           $"{newsDTO.Link} - Read more\n\n" +
                           $"Kind Regards,  \n" +
                           $" Alumni Office \n" +
                           $"Tshwane University of Technology";
            }
            else  // For general news
            {
                subject = $"New {newsDTO.NewsType} News: {newsDTO.Headline}";
                message = "Dear Alumni,\n" +
                          $"New {newsDTO.NewsType} news has been published: {newsDTO.Headline}\n" +
                          $"{newsDTO.Description}\n" +
                          $"Published by: {newsDTO.Publisher}\n\n" +
                          "http://localhost:3000/news - Read more\n\n" +
                          $"Kind Regards,  \n" +
                           $" Alumni Office \n" +
                           $"Tshwane University of Technology";

            }

            try
            {
                foreach (var email in alumniEmails)
                {
                    await _emailSender.SendEmailAsync(email, subject, message);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Email error: {ex.Message}");
            }

            return Ok(new { message = "News uploaded and emails sent successfully" });
        }


        [HttpPut]
        [Route("UpdateNews/{id}")]
        public async Task<IActionResult> UpdateNews(int id, [FromBody] NewsDTO newsDTO)
        {
            var existingNews = await _alumniDbContext.News.FindAsync(id);
            if (existingNews == null)
            {
                return NotFound(new { message = "News not found" });
            }

            try
            {
                // Update fields regardless of media presence
                existingNews.Headline = newsDTO.Headline;
                existingNews.Description = newsDTO.Description;
                existingNews.Publisher = newsDTO.Publisher;
                existingNews.PublishedDate = newsDTO.PublishedDate;
                existingNews.Link = newsDTO.Link;

                // Handle media if a new image is uploaded
                if (!string.IsNullOrEmpty(newsDTO.Media))
                {
                    byte[] mediaBytes;
                    try
                    {
                        string base64Data = newsDTO.Media.Contains(",")
                            ? newsDTO.Media.Substring(newsDTO.Media.IndexOf(",") + 1)
                            : newsDTO.Media;

                        mediaBytes = Convert.FromBase64String(base64Data);
                        existingNews.Media = mediaBytes;
                    }
                    catch (FormatException)
                    {
                        return BadRequest(new { message = "Invalid media format" });
                    }
                }

                await _alumniDbContext.SaveChangesAsync();
                return Ok(new { message = "News updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }


        [HttpDelete]
        [Route("DeleteNews/{id}")]
        public async Task<IActionResult> DeleteNews(int id)
        {
            // Find the existing news item in the database
            var existingNews = await _alumniDbContext.News.FindAsync(id);
            if (existingNews == null)
            {
                return NotFound(new { message = "News not found" });
            }

            try
            {
                _alumniDbContext.News.Remove(existingNews); // Correct entity deletion
                await _alumniDbContext.SaveChangesAsync(); // Save changes to the database

                return Ok(new { message = "News deleted successfully" }); // Return a success message
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }



    }
}
