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
        private readonly AlumnusService _alumnusService;

        public AlumnusController(AlumniDbContext alumniDbContext, AlumnusService alumnusService)
        {
            _alumniDbContext = alumniDbContext;
            _alumnusService = alumnusService;
        }

        [HttpPost]
        [Route("Registration")]
        public async Task<IActionResult> Registration(AlumnusDTO alumnusDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if the alumnus already exists
            var objAlumnus = _alumniDbContext.Alumnus.FirstOrDefault(a => a.Email == alumnusDTO.Email);

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
            }
            else
            {
                // If the alumnus already exists, return an error
                return BadRequest($"Alumnus with email '{objAlumnus.Email}' already exists. Try again.");
            }
        }


        [HttpPost]
        [Route("Login")]
        public IActionResult Login(LoginDTO loginDTO)
        {
            var alumnus = _alumniDbContext.Alumnus.FirstOrDefault(a => a.Email == loginDTO.Email && a.Password == loginDTO.Password);

            if (alumnus != null)
            {
                return Ok(alumnus);
            }
            return NoContent();
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
