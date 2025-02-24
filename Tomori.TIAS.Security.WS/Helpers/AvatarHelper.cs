using System.Configuration;

namespace Tomori.TIAS.Security.WS.Helpers
{
    public static class AvatarHelper
    {
        public static string HandleAvatarNull(string displayName)
        {
            return $"{ConfigurationManager.AppSettings["AVATAR_NULL"]}{displayName.Replace(" ", "+")}";
        }

        public static string HandleAvatarExist(string avatar)
        {
            return $"{ConfigurationManager.AppSettings["AVATAR_EXIST"]}{avatar}";
        }
    }
}