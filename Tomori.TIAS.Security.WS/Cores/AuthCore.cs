using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Tomori.TIAS.Security.WS.Helpers;
using Tomori.TIAS.Security.WS.Models.DTO;
using Tomori.TIAS.Security.WS.Models.Entities.DBTIASSECURITY;
using Tomori.TIAS.Security.WS.Models.Enums;
using Tomori.TIAS.Security.WS.Repositories;
using Module = Tomori.TIAS.Security.WS.Models.Entities.DBTIASSECURITY.Module;

namespace Tomori.TIAS.Security.WS.Cores
{
    public class AuthCore
    {
        private readonly UserAdRepo userAdRepo = new UserAdRepo();
        private readonly UserDbRepo userDbRepo = new UserDbRepo();
        private readonly ConfigRepo configRepo = new ConfigRepo();
        private readonly SessionRepo sessionRepo = new SessionRepo();
        private readonly UserAccessRepo userAccessRepo = new UserAccessRepo();

        public ResponseLogin LoginAD(string username, string password, string applicationCode, int timeoutDuration, string ipAddress)
        {
            ResponseLogin result = new ResponseLogin();

            try
            {
                // validate user and get user if validate is true
                User user = userAdRepo.Get(username, password);
                if (user == null) return result;

                // check user already logged in or not
                if (CheckAlreadyLoggedIn(username, UserSourceEnum.AD)) return result;

                // get access module
                List<Module> modules = GetModuleList(username, UserSourceEnum.AD, applicationCode);
                if (modules == null || modules.Count == 0) return result;

                // store session and get token data (token and expired datetime)
                //Dictionary<string, dynamic> tokenData = InsertSession(username, UserSourceEnum.AD, applicationCode, timeoutDuration, ipAddress);

                result.IsSuccess = true;
                result.Message = "Success";
                //result.Token = tokenData["token"];
                //result.Expires = tokenData["expiredAt"];
                result.Path = modules[0].Path;
                result.AccessModule = modules;

                LogHelper.SaveInformation($"{user.DisplayName} 1 successfully logged in via the application with app code {applicationCode}", username);
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.SaveError(MethodBase.GetCurrentMethod(), ex.Message, username: username);
                return result;
            }
        }

        public ResponseLogin LoginDB(string username, string password, string applicationCode, int timeoutDuration, string ipAddress)
        {
            ResponseLogin result = new ResponseLogin();

            try
            {
                // validate user and get user if validate is true
                User user = userDbRepo.Get(username, password);
                if (user == null) return result;

                // check user already logged in or not
                if (CheckAlreadyLoggedIn(username, UserSourceEnum.DB)) return result;

                // get access module
                List<Module> modules = GetModuleList(username, UserSourceEnum.DB, applicationCode);
                if (modules == null || modules.Count == 0) return result;

                // store session and get token data (token and expired datetime)
                //Dictionary<string, dynamic> tokenData = InsertSession(username, UserSourceEnum.DB, applicationCode, timeoutDuration, ipAddress);

                result.IsSuccess = true;
                result.Message = "Success";
                //result.Token = tokenData["token"];
                //result.Expires = tokenData["expiredAt"];
                result.Path = modules[0].Path;
                result.AccessModule = modules;

                LogHelper.SaveInformation($"{user.DisplayName} successfully logged in via the application with app code {applicationCode}", username);
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.SaveError(MethodBase.GetCurrentMethod(), ex.Message, username: username);
                return result;
            }
        }

        public ResponseLogin LoginGuest(string applicationCode, int timeoutDuration, string ipAddress)
        {
            ResponseLogin result = new ResponseLogin();

            try
            {
                // get access module
                List<Module> modules = userAccessRepo.GetModuleGuestList(applicationCode);
                if (modules == null || modules.Count == 0)
                {
                    LogHelper.SaveError($"The user does not have access to the application with the app code {applicationCode}", $"guest ({ipAddress})");
                    return result;
                }

                // store session and get token data (token and expired datetime)
                Dictionary<string, dynamic> tokenData = InsertSession("guest", UserSourceEnum.Guest, applicationCode, timeoutDuration, ipAddress);

                result.IsSuccess = true;
                result.Message = "Success";
                result.Token = tokenData["token"];
                result.Expires = tokenData["expiredAt"];
                result.Path = modules[0].Path;
                result.AccessModule = modules;

                LogHelper.SaveInformation($"Guest ({ipAddress}) successfully logged in via the application with app code {applicationCode}", $"guest ({ipAddress})");
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.SaveError(MethodBase.GetCurrentMethod(), ex.Message, $"guest ({ipAddress})");
                return result;
            }
        }

        public ResponseLogin StoreSession(string username, string userSource, string applicationCode, int timeoutDuration, string ipAddress)
        {
            ResponseLogin result = new ResponseLogin();
            try
            {
                UserSourceEnum uSource = userSource == "AD" ? UserSourceEnum.AD : UserSourceEnum.DB;
                List<Module> modules = GetModuleList(username, uSource, applicationCode);
                if (modules == null || modules.Count == 0) return result;
                Dictionary<string, dynamic> tokenData = InsertSession(username, uSource, applicationCode, timeoutDuration, ipAddress);

                result.IsSuccess = true;
                result.Message = "Success";
                result.Token = tokenData["token"];
                result.Expires = tokenData["expiredAt"];
                result.Path = modules[0].Path;
                result.AccessModule = modules;
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.SaveError(MethodBase.GetCurrentMethod(), ex.Message, username: username);
                return result;
            }
        }

        // Check config canLoginAllDevice and check user already logged in or not
        // IF true THEN user can't login
        // IF false THEN user can login
        private bool CheckAlreadyLoggedIn(string username, UserSourceEnum userSource)
        {
            // check can login all device
            bool canLoginAllDevice = configRepo.GetCanLoginAllDevice();
            if (canLoginAllDevice) return false;

            // check user already logged in or not
            return sessionRepo.CheckLoginExist(username, userSource);
        }

        // Get access module by application code
        private List<Module> GetModuleList(string username, UserSourceEnum userSource, string appCode)
        {
            List<Module> modules = userAccessRepo.GetModuleList(username, userSource, appCode);
            if (modules == null || modules.Count == 0)
                LogHelper.SaveError($"The user does not have access to the application with the app code {appCode}", username);
            return modules;
        }

        // Insert new session
        private Dictionary<string, dynamic> InsertSession(string username, UserSourceEnum userSource, string appCode, int timeoutDuration, string ipAddress)
        {
            DateTime loginAt = DateTimeHelper.GetNow();
            DateTime expiredAt = loginAt.AddSeconds(timeoutDuration);
            string token = sessionRepo.Insert(username, userSource, appCode, loginAt, expiredAt, ipAddress);

            Dictionary<string, dynamic> result = new Dictionary<string, dynamic>();
            result["token"] = token;
            result["expiredAt"] = DateTimeHelper.Convert(expiredAt);
            return result;
        }

        public void Logout(string id)
        {
            try
            {
                sessionRepo.Logout(id);
            }
            catch (Exception ex)
            {
                LogHelper.SaveError(MethodBase.GetCurrentMethod(), ex.Message);
            }
        }
    }
}