using System.Collections.Generic;
using Tomori.TIAS.Security.WS.Models.Enums;

namespace Tomori.TIAS.Security.WS.Models.Entities.DBTIASSECURITY
{
    public class User
    {
        public string Username;
        public string DisplayName;
        public string Avatar;
        public UserSourceEnum UserSource;
        public string Position;
        public string Password;
        public List<Module> AccessModule;
    }
}