using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectX.Data.Model;
using Microsoft.AspNetCore.Cors;
using ProjectX.Data.Model.dto;
using ProjectX.Data.Model.bl;
using System.Data.SqlClient;



namespace ProjectX.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [EnableCors("corspolicy")]

    public class AlumnusRegistrationController : Controller
    {
        private readonly IConfiguration _configuration;

        public AlumnusRegistrationController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        //[Route("AlumnusRegistration")]

        public Response AlumnusRegistration(AlumnusRegistration alumnusRegistration)
        {
            Response response = new Response();
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("AlumniDb").ToString());
            Dal dal = new Dal();
            response = dal.AlumnusRegistration(alumnusRegistration, connection);

            return response;
        }

        [HttpPost]
        //[Route("Login")]
        public Response Login(AlumnusRegistration alumnusRegistration)
        {
            Response response = new Response();
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("AlumniDb").ToString());
            Dal dal = new Dal();
            response = dal.Login(alumnusRegistration, connection);
            return response;
        }


        [HttpPost]
        //[Route("AlumnusApproval")]
        public Response AlumnusApproval(AlumnusRegistration alumnusRegistration)
        {
            Response response = new Response();
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("AlumniDb").ToString());
            Dal dal = new Dal();
            response = dal.AlumnusApproval(alumnusRegistration, connection);
            return response;
        }
    }
}
