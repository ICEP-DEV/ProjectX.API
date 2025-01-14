using Azure;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ProjectX.Data;
using ProjectX.Data.Migrations;
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
        [Route("CountRSVPS")]
        public IActionResult CountRSVPS()
        {
            var rsvps = _alumniDbContext.RSVPs.Count();

            return Ok(rsvps);
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
                        AlumnusId = alumnus.AlumnusId,
                        AlumnusFirstName = alumnus.FirstName,
                        AlumnusLastName = alumnus.LastName,
                        AlumnusEmail = alumnus.Email,
                        AlumnusCampus = alumnus.Campus,
                        AlumnusCourse = alumnus.Course,
                        EventId = volunteer.EventId,
                        EventTitle = _alumniDbContext.Event.FirstOrDefault(e => e.Id == volunteer.EventId).Title,
                        Status = volunteer.Status,
                        VolunteerRole = volunteer.Role,
                    }
                )
                .ToList();

            // Reverse the list to make it first-in-last-out
            responses.Reverse();

            return Ok(responses);
        }
        [HttpGet]
        [Route("GetRSVPs")]
        public IActionResult GetRSVPs()
        {
            var rsvps = _alumniDbContext.RSVPs
                .Join(
                _alumniDbContext.AlumnusProfile,
                rsvp => rsvp.AlumnusId,
                alumnus => alumnus.AlumnusId,
                (rsvp, alumnus) => new
                {
                    AlumnusId = alumnus.AlumnusId,
                    AlumnusFirstName = alumnus.FirstName,
                    AlumnusLastName = alumnus.LastName,
                    AlumnusEmail = alumnus.Email,
                    AlumnusCampus = alumnus.Campus,
                    AlumnusCourse = alumnus.Course,
                    EventId = rsvp.EventId,
                    EventTitle = _alumniDbContext.Event.FirstOrDefault(e => e.Id == rsvp.EventId).Title,
                    EventVenue = _alumniDbContext.Event.FirstOrDefault(e => e.Id == rsvp.EventId).Venue,
                    EventDate = _alumniDbContext.Event.FirstOrDefault(e => e.Id == rsvp.EventId).Date,
                    
                }).ToList();


            // Reverse the list to make it first-in-last-out
            rsvps.Reverse();

            return Ok(rsvps);
        }
        [HttpPost]
        [Route("UpdateStatus")]
        public async Task<IActionResult> UpdateStatusAsync([FromBody] StatusDTO status)
        {
            if (status == null || status.AlumnusId <= 0 )
            {
                return BadRequest("Invalid request data.");
            }

            try
            {
                // Fetch the volunteer record by AlumnusId
                var volunteer = _alumniDbContext.Volunteers.FirstOrDefault(v => v.AlumnusId == status.AlumnusId && v.EventId == status.EventId && v.Role.Equals(status.Role));

                if (volunteer == null)
                {
                    return NotFound("Volunteer not found.");
                }

                // Update the status
                volunteer.Status = status.Status;
                _alumniDbContext.SaveChanges();

                // Get alumni email
                var alumniEmail = _alumniDbContext.AlumnusProfile
                    .Where(p => p.AlumnusId == volunteer.AlumnusId)
                    .Select(p => p.Email)
                    .FirstOrDefault();

                // Check if alumniEmail is null or empty
                if (string.IsNullOrWhiteSpace(alumniEmail))
                {
                    return StatusCode(400, "Alumni email not found.");
                }

                // Get event details
                var eventDetails = _alumniDbContext.Event
                    .Where(e => e.Id == status.EventId)
                    .Select(e => new
                    {
                        e.Title,
                        e.Date,
                        e.Time,
                        e.Venue,
                        e.Description
                    })
                    .FirstOrDefault(); // Ensure we get a single event or null

                // Check if eventDetails is null to avoid runtime errors
                if (eventDetails == null)
                {
                    return StatusCode(400, "Event not found.");
                }

                // Define subject and message dynamically based on the status
                // Initialize subject and message with default values
                string subject = "Volunteer Status Update";
                string message = "Dear Alumni,\n\nWe have an update regarding your volunteer request. Please contact the Alumni Office for more details.";

                // Define subject and message dynamically based on the status
                if (volunteer.Status.Equals("Approved", StringComparison.OrdinalIgnoreCase))
                {
                    subject = $"Volunteer Status for Event: {eventDetails.Title}";

                    message = $"Dear Alumni,\n\n" +
                              $"Your request to volunteer for an upcoming event hosted by Tshwane University of Technology has been approved:\n\n" +
                              $"Event: {eventDetails.Title}\n" +
                              $"Description: {eventDetails.Description}\n" +
                              $"Date: {eventDetails.Date:dddd, MMMM dd, yyyy}\n" +
                              $"Time: {eventDetails.Time}\n" +
                              $"Venue: {eventDetails.Venue}\n\n" +
                              $"For more details, please visit: http://localhost:3000/events\n\n" +
                              $"We look forward to your participation!\n\n" +
                              $"Kind Regards,\n" +
                              $"Alumni Office\n" +
                              $"Tshwane University of Technology";
                }
                else if (volunteer.Status.Equals("Rejected", StringComparison.OrdinalIgnoreCase))
                {
                    subject = $"Volunteer Status for Event: {eventDetails.Title}";

                    message = $"Dear Alumni,\n\n" +
                              $"We regret to inform you that your request to volunteer for the event titled '{eventDetails.Title}' has been declined.\n\n" +
                              $"We appreciate your interest and encourage you to stay connected for future opportunities.\n\n" +
                              $"Kind Regards,\n" +
                              $"Alumni Office\n" +
                              $"Tshwane University of Technology";
                }


                try
                {
                    // Send email
                    await _emailSender.SendEmailAsync(alumniEmail, subject, message);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Email error: {ex.Message}");
                }


                return Ok("Status updated successfully.");

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("GetNewVolunteers")]
        public IActionResult GetNewVolunteers(DateTime? lastChecked = null)
        {
            try
            {
                // Fetch count of volunteers who are awaiting approval or added after `lastChecked`
                var newVolunteersCount = _alumniDbContext.Volunteers
                    .Count(v => v.Status == "Awaiting" || (lastChecked != null && v.CreatedAt > lastChecked));

                return Ok(newVolunteersCount); // Return just the count
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
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
