using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using ProjectX.Data.Model.dto;


namespace ProjectX.Data.Model.bl
{
    public class Dal
    {
        public Response AlumnusRegistration(AlumnusRegistration alumnusRegistration, SqlConnection connection)
        {
            Response response = new Response();
            SqlCommand cmd = new SqlCommand("INSERT INTO AlumnusRegistrations(StudentNum,Email,Password) VALUES('" + alumnusRegistration.StudentNum + "','" + alumnusRegistration.Email + "', '" + alumnusRegistration.Password + "')", connection);
            connection.Open();
            int i = cmd.ExecuteNonQuery();
            connection.Close();

            if (i > 0)
            {
                response.StatusCode = 200;
                response.StatusMessage = "Signup successful";
            }
            else
            {
                response.StatusCode = 100;
                response.StatusMessage = "Signup failed";
            }


            return response;
        }

        public Response Login(AlumnusRegistration alumnusRegistration, SqlConnection connection)
        {
            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM AlumnusRegistrations WHERE Email = '" + alumnusRegistration.StudentNum + "' AND Password = '" + alumnusRegistration.Password + "'", connection);
            DataTable dt = new DataTable();
            da.Fill(dt);
            Response response = new Response();

            if (dt.Rows.Count > 0)
            {
                response.StatusCode = 200;
                response.StatusMessage = "Login successful";
                AlumnusRegistration alumnusReg = new AlumnusRegistration();
                alumnusReg.Id = Convert.ToInt32(dt.Rows[0]["Id"]);
                alumnusReg.Email = Convert.ToString(dt.Rows[0]["Email"]);
                response.AlumnusRegistration = alumnusReg;
            }
            else
            {
                response.StatusCode = 100;
                response.StatusMessage = "Login failed";
                response.AlumnusRegistration = null;
            }
            return response;
        }

        public Response AlumnusApproval(AlumnusRegistration alumnusRegistration, SqlConnection connection)
        {

            Response response = new Response();
            SqlCommand cmd = new SqlCommand("UPDATE AlumnusRegistrations SET IsApproved = 1 WHERE StudentNum = '" + alumnusRegistration.StudentNum + "' ", connection);
            connection.Open();
            int i = cmd.ExecuteNonQuery();
            connection.Close();

            if (i > 0)
            {
                response.StatusCode = 200;
                response.StatusMessage = "Alumnus Verified";
            }
            else
            {
                response.StatusCode = 100;
                response.StatusMessage = "Alumnus Verification failed";
            }

            return response;
        }
    }
}
