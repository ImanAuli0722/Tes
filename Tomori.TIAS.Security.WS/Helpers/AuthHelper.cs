using System.Web.Services.Protocols;
using YLib.General;
using YLib.General.Model;

namespace Tomori.TIAS.Security.WS.Helpers
{
    public class AuthHelper
    {
        public UserCredential userCredential;

        protected void ValidateHeader(string method)
        {
            // Check null authentication
            if (userCredential == null)
            {
                throw new SoapException("Header Null", SoapException.ClientFaultCode);
            }

            // Validasi YLib
            var pl = LogonWS.login(userCredential.Username, userCredential.Password, method, "TIAS_SECURITY_SERVICE", userCredential.AppName, userCredential.AppName);

            if (!pl.Status)
            {
                throw new SoapException("invalid authentication header", SoapException.ClientFaultCode);
            }
        }
    }
}