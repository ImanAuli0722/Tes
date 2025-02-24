using System.Configuration;

namespace Tomori.TIAS.Security.WS.Helpers.Consts
{
    public static class AppConst
    {
        public static readonly string APP_NAME = ConfigurationManager.AppSettings["APP_NAME"];
        public static readonly string SOURCE = $"{ConfigurationManager.AppSettings["APP_NAME"]} WS";
        public static readonly string CONNECTION_DB = ConfigurationManager.ConnectionStrings["DEFAULT_CONNECTION"].ConnectionString;
        public static readonly string DOMAIN_AD_NAME = ConfigurationManager.AppSettings["AD_DOMAIN"];
    }
}