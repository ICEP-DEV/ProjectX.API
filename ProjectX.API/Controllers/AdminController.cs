using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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

        public AdminController(IConfiguration configuration)
        {
            _configuration = configuration;
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

        //public IActionResult Index()
        //{
        //    return View();
        //}
    }
}
