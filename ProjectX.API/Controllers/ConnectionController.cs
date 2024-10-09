using Microsoft.AspNetCore.Mvc;
using System.Net.Sockets;

namespace ProjectX.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConnectionController : ControllerBase
    {
        private readonly string serverIp = "168.172.187.187";  // Hardcoded server IP address
        private readonly int serverPort = 5214;              // Hardcoded port

        [HttpPost("connect")]
        public IActionResult ConnectToServer()
        {
            try
            {
                // Create and start the TCP client to connect to the hardcoded server IP and port
                TCPClient client = new TCPClient();
                client.StartClient(serverIp, serverPort);

                return Ok("Connection to the server was successful.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Failed to connect to the server: {ex.Message}");
            }
        }
    }

}
