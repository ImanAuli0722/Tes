using System.Collections.Generic;
using Tomori.TIAS.Security.WS.Models.Entities.DBTIASSECURITY;

namespace Tomori.TIAS.Security.WS.Models.DTO
{
    public class ResponseLogin
    {
        public bool IsSuccess;
        public string Message = "Invalid Username & Password";
        public string Token;
        public string Path;
        public string Expires;
        public string UserSource;
        public List<Module> AccessModule;
    }
}