namespace Tomori.TIAS.Security.WS.Models.DTO
{
    public class RequestLogin
    {
        public string Username;
        public string Password;
        public string UserSource;
        public string ApplicationCode;
        public int TimeoutDuration;
        public string IpAddress;
    }
}