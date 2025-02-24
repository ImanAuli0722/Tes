using System;
using System.Reflection;
using Tomori.TIAS.Security.WS.Helpers;
using Tomori.TIAS.Security.WS.Models.Entities.DBTIASSECURITY;
using Tomori.TIAS.Security.WS.Models.Enums;
using Tomori.TIAS.Security.WS.Repositories;

namespace Tomori.TIAS.Security.WS.Cores
{
    public class SessionCore
    {
        private readonly SessionRepo sessionRepo = new SessionRepo();
        private readonly UserAdRepo userAdRepo = new UserAdRepo();
        private readonly UserDbRepo userDbRepo = new UserDbRepo();
        private readonly UserAccessRepo userAccessRepo = new UserAccessRepo();

        public User GetUserFromSession(string id, string applicationCode)
        {
            // Get user data
            User user = null;
            try
            {
                // Get session not expired data
                Session session = sessionRepo.Get(id);
                if (session == null) return user;

                
                switch (session.UserSource)
                {
                    case UserSourceEnum.AD:
                        user = userAdRepo.Get(session.Username);
                        user.AccessModule = userAccessRepo.GetModuleList(session.Username, session.UserSource, applicationCode);
                        break;
                    case UserSourceEnum.DB:
                        user = userDbRepo.Get(session.Username);
                        user.AccessModule = userAccessRepo.GetModuleList(session.Username, session.UserSource, applicationCode);
                        break;
                    default:
                        user = new User
                        {
                            Username = "guest",
                            DisplayName = "Guest",
                            Avatar = AvatarHelper.HandleAvatarNull("Guest"),
                            Position = "Guest",
                            UserSource = UserSourceEnum.Guest,
                        };
                        user.AccessModule = userAccessRepo.GetModuleGuestList(applicationCode);
                        break;
                }

                return user;
            }
            catch (Exception ex)
            {
                LogHelper.SaveError(MethodBase.GetCurrentMethod(), ex.Message);
                return user;
            }
        }

        public string UpdateSession(string id, int timeoutDuration)
        {
            try
            {
                DateTime expiredAt = DateTimeHelper.GetNow().AddSeconds(timeoutDuration);
                sessionRepo.UpdateExpiredAt(id, expiredAt);

                return DateTimeHelper.Convert(expiredAt);
            }
            catch (Exception ex)
            {
                LogHelper.SaveError(MethodBase.GetCurrentMethod(), ex.Message);
                return null;
            }
        }

    }
}