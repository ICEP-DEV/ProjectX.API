using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ProjectX.Data;
using ProjectX.Data.Model;
using ProjectX.Data.Model.bl;
using ProjectX.Data.Model.dto;
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
    }
}
