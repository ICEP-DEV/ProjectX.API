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
    public class BusinessLogic
    {
        public Response Login(Admin admin, SqlConnection connection)
        {
            // Create a SqlDataAdapter to execute a SQL query that selects all columns
            // from the Admin table where the AdminId and Password match the provided values.
            SqlDataAdapter da = new SqlDataAdapter(
                "SELECT * FROM Admin WHERE AdminId = '" + admin.AdminId +
                "' AND Password= '" + admin.Password + "'",
                connection
            );

            // Create a DataTable to hold the results of the query.
            DataTable dt = new DataTable();

            // Fill the DataTable with the results of the SQL query executed by the SqlDataAdapter.
            da.Fill(dt);

            // Create a new Response object to store the result of the login attempt.
            Response response = new Response();

            // Check if any rows were returned from the query (meaning the login was successful).
            if (dt.Rows.Count > 0)
            {
                // Set the status code to 200 (indicating success).
                response.StatusCode = 200;

                // Set the status message to indicate that login was successful.
                response.StatusMessage = "Login Successful";

                // Create a new Admin object to store the admin details from the query result.
                Admin admin1 = new Admin();

                // Populate the AdminId of the admin object from the first row of the result set.
                admin1.AdminId = Convert.ToInt32(dt.Rows[0]["AdminId"]);

                // Populate the Name of the admin object from the first row of the result set.
                admin1.Name = Convert.ToString(dt.Rows[0]["Name"]);

                // Assign the populated admin object to the response.
                response.admin = admin1;
            }
            else
            {
                // If no rows were returned, set the status code to 100 (indicating failure).
                response.StatusCode = 100;

                // Set the status message to indicate that login was unsuccessful.
                response.StatusMessage = "Login Unsuccessful";

                // Set the admin property of the response to null since login failed.
                response.admin = null;
            }

            // Return the response object containing the status and admin information.
            return response;
        }

      
    }
}
