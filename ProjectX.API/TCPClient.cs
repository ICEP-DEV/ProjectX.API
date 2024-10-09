namespace ProjectX.API
{
    
    public class TCPClient
    {
        public void StartClient(string serverIp, int port)
        {
            try
            {
                // Establish connection with the server using hardcoded IP and port
                using (var client = new System.Net.Sockets.TcpClient(serverIp, port))
                {
                    // Connection logic here
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error connecting to the server: {ex.Message}");
            }
        }
    }

}
