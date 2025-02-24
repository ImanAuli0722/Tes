using System;
using System.Reflection;
using System.Web.Services;
using System.Web.Services.Protocols;
using Tomori.TIAS.Security.WS.Cores;
using Tomori.TIAS.Security.WS.Helpers;
using Tomori.TIAS.Security.WS.Helpers.Consts;
using Tomori.TIAS.Security.WS.Models.DTO;
using Tomori.TIAS.Security.WS.Models.Entities.DBTIASSECURITY;

namespace Tomori.TIAS.Security.WS.Services
{
    [WebService(Namespace = "https://api.job-tomori.com")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class SecurityService : AuthHelper
    {
        private readonly AuthCore authCore = new AuthCore();
        private readonly SessionCore sessionCore = new SessionCore();
        private readonly ConfigCore configCore = new ConfigCore();

        // Login user AD
        [WebMethod]
        [SoapHeader("userCredential")]
        public Response<ResponseLogin> LoginAD(Request<RequestLogin> request)
        {
            Response<ResponseLogin> response = new Response<ResponseLogin>();
            try
            {
                ValidateHeader(MethodBase.GetCurrentMethod()?.ToString());

                ResponseLogin responseLogin = authCore.LoginAD(
                    request.PropertyData.Username,
                    request.PropertyData.Password,
                    request.PropertyData.ApplicationCode,
                    request.PropertyData.TimeoutDuration,
                    request.PropertyData.IpAddress
                );
                response.Data.ContentData = responseLogin;
                if (responseLogin.IsSuccess)
                {
                    response.Status = EventIdConst.OK;
                    response.Data.ContentData.UserSource = "AD";
                }
                return response;
            }
            catch (Exception ex)
            {
                LogHelper.SaveError(ex.Message);
                response.Status = EventIdConst.INTERNAL_SERVER_ERROR;
                response.Data.ContentData = new ResponseLogin();
                return response;
            }
        }

        // Login user DB
        [WebMethod]
        [SoapHeader("userCredential")]
        public Response<ResponseLogin> LoginDB(Request<RequestLogin> request)
        {
            Response<ResponseLogin> response = new Response<ResponseLogin>();
            try
            {
                ValidateHeader(MethodBase.GetCurrentMethod()?.ToString());

                ResponseLogin responseLogin = authCore.LoginDB(
                    request.PropertyData.Username,
                    request.PropertyData.Password,
                    request.PropertyData.ApplicationCode,
                    request.PropertyData.TimeoutDuration,
                    request.PropertyData.IpAddress
                );
                response.Data.ContentData = responseLogin;
                if (responseLogin.IsSuccess)
                {
                    response.Status = EventIdConst.OK;
                    response.Data.ContentData.UserSource = "DB";
                }
                return response;
            }
            catch (Exception ex)
            {
                LogHelper.SaveError(ex.Message);
                response.Status = EventIdConst.INTERNAL_SERVER_ERROR;
                response.Data.ContentData = new ResponseLogin();
                return response;
            }
        }

        [WebMethod]
        [SoapHeader("userCredential")]
        public Response<ResponseLogin> StoreSession(Request<RequestLogin> request)
        {
            Response<ResponseLogin> response = new Response<ResponseLogin>();
            try
            {
                ValidateHeader(MethodBase.GetCurrentMethod()?.ToString());

                ResponseLogin responseLogin = authCore.StoreSession(
                    request.PropertyData.Username,
                    request.PropertyData.UserSource,
                    request.PropertyData.ApplicationCode,
                    request.PropertyData.TimeoutDuration,
                    request.PropertyData.IpAddress
                );
                if (responseLogin.IsSuccess)
                {
                    response.Status = EventIdConst.OK;
                }
                response.Data.ContentData = responseLogin;
                return response;
            }
            catch (Exception ex)
            {
                LogHelper.SaveError(ex.Message);
                response.Status = EventIdConst.INTERNAL_SERVER_ERROR;
                response.Data.ContentData = new ResponseLogin();
                return response;
            }
        }

        // Login user guest
        [WebMethod]
        [SoapHeader("userCredential")]
        public Response<ResponseLogin> LoginGuest(Request<RequestLogin> request)
        {
            Response<ResponseLogin> response = new Response<ResponseLogin>();
            try
            {
                ValidateHeader(MethodBase.GetCurrentMethod()?.ToString());

                ResponseLogin responseLogin = authCore.LoginGuest(
                    request.PropertyData.ApplicationCode,
                    request.PropertyData.TimeoutDuration,
                    request.PropertyData.IpAddress
                );
                if (responseLogin.IsSuccess)
                {
                    response.Status = EventIdConst.OK;
                }
                response.Data.ContentData = responseLogin;
                return response;
            }
            catch (Exception ex)
            {
                LogHelper.SaveError(ex.Message);
                response.Status = EventIdConst.INTERNAL_SERVER_ERROR;
                response.Data.ContentData = new ResponseLogin();
                return response;
            }
        }

        // Logout, update logout_at to CURRENT TIMESTAMP
        [WebMethod]
        [SoapHeader("userCredential")]
        public Response<bool> Logout(Request<string> request)
        {
            Response<bool> response = new Response<bool>();
            try
            {
                ValidateHeader(MethodBase.GetCurrentMethod()?.ToString());

                authCore.Logout(request.PropertyData);
                response.Status = EventIdConst.OK;
                response.Data.ContentData = true;
                return response;
            }
            catch (Exception ex)
            {
                LogHelper.SaveError(ex.Message);
                response.Status = EventIdConst.INTERNAL_SERVER_ERROR;
                response.Data.ContentData = false;
                return response;
            }
        }

        // Get User with access module by id session
        [WebMethod]
        [SoapHeader("userCredential")]
        public Response<User> GetUserFromSession(Request<RequestSession> request)
        {
            Response<User> response = new Response<User>();
            try
            {
                ValidateHeader(MethodBase.GetCurrentMethod()?.ToString());

                User user = sessionCore.GetUserFromSession(request.PropertyData.Id, request.PropertyData.ApplicationCode);
                if (user != null) response.Status = EventIdConst.OK;

                response.Data.ContentData = user;

                return response;
            }
            catch (Exception ex)
            {
                LogHelper.SaveError(ex.Message);
                response.Status = EventIdConst.INTERNAL_SERVER_ERROR;
                response.Data.ContentData = null;
                return response;
            }
        }

        // Update expired time session
        [WebMethod]
        [SoapHeader("userCredential")]
        public Response<string> UpdateSession(Request<RequestSession> request)
        {
            Response<string> response = new Response<string>();

            try
            {
                ValidateHeader(MethodBase.GetCurrentMethod()?.ToString());

                string expiredAt = sessionCore.UpdateSession(request.PropertyData.Id, request.PropertyData.TimeoutDuration);
                if (expiredAt != null) response.Status = EventIdConst.OK;
                response.Data.ContentData = expiredAt;
                return response;
            }
            catch (Exception ex)
            {
                LogHelper.SaveError(ex.Message);
                response.Status = EventIdConst.INTERNAL_SERVER_ERROR;
                response.Data.ContentData = null;
                return response;
            }
        }

        // Get value config
        [WebMethod]
        [SoapHeader("userCredential")]
        public Response<string> GetConfigValue(Request<string> request)
        {
            Response<string> response = new Response<string>();

            try
            {
                // validate header
                var methodBase = MethodBase.GetCurrentMethod();
                if (methodBase != null)
                {
                    ValidateHeader(methodBase.ToString());
                }
                else
                {
                    // Handle the case when methodBase is null, perhaps log or throw a specific error
                    LogHelper.SaveError("MethodBase.GetCurrentMethod() returned null");
                }

                string configValue = configCore.GetValue(request.PropertyData);
                if (configValue != null) response.Status = EventIdConst.OK;
                response.Data.ContentData = configValue;

                return response;
            }
            catch (Exception ex)
            {
                LogHelper.SaveError(ex.Message);
                response.Status = EventIdConst.INTERNAL_SERVER_ERROR;
                response.Data.ContentData = null;
                return response;
            }
        }
    }
}
