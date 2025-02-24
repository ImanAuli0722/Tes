using System;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Reflection;
using Tomori.TIAS.Security.WS.Helpers;
using Tomori.TIAS.Security.WS.Helpers.Consts;
using Tomori.TIAS.Security.WS.Models.Entities.DBTIASSECURITY;
using Tomori.TIAS.Security.WS.Models.Enums;

namespace Tomori.TIAS.Security.WS.Repositories
{
    public class UserAdRepo
    {
        // Get user by username
        public User Get(string username)
        {
            try
            {
                using (PrincipalContext context = new PrincipalContext(ContextType.Domain, AppConst.DOMAIN_AD_NAME))
                {
                    return Get(context, username);
                }
            }
            catch (Exception ex)
            {
                LogHelper.SaveError(MethodBase.GetCurrentMethod(), ex.Message, username: username);
                return null;
            }
        }

        // Validate user, if validate == true then return object user
        public User Get(string username, string password)
        {
            try
            {
                using (PrincipalContext context = new PrincipalContext(ContextType.Domain, AppConst.DOMAIN_AD_NAME))
                {
                    // validate user 
                    if (!context.ValidateCredentials(username, password))
                    {
                        LogHelper.SaveError("Invalid username & password! ValidateCredentials AD = false", username: username);
                        return null;
                    }

                    return Get(context, username);
                }
            }
            catch (Exception ex)
            {
                LogHelper.SaveError(MethodBase.GetCurrentMethod(), ex.Message, username: username);
                return null;
            }
        }

        // Get user from AD and then mapping to User Model
        private User Get(PrincipalContext context, string username)
        {
            try
            {
                UserPrincipal userPrincipal = UserPrincipal.FindByIdentity(context, username);
                if (userPrincipal == null)
                {
                    return null;
                }

                User user = new User();
                user.Username = username;
                user.DisplayName = userPrincipal.DisplayName;
                user.UserSource = UserSourceEnum.DB;

                if (userPrincipal.GetUnderlyingObject() is DirectoryEntry directoryEntry)
                {
                    if (directoryEntry.Properties.Contains("thumbnailPhoto"))
                    {
                        byte[] photoBytes = directoryEntry.Properties["thumbnailPhoto"].Value as byte[];
                        if (photoBytes != null)
                        {
                            user.Avatar = $"data:image/jpeg;base64,{Convert.ToBase64String(photoBytes)}";
                        }
                        else
                        {
                            user.Avatar = AvatarHelper.HandleAvatarNull(user.DisplayName);
                        }
                    }
                    else
                    {
                        user.Avatar = AvatarHelper.HandleAvatarNull(user.DisplayName);
                    }
                    user.Position = directoryEntry.Properties["title"].Value?.ToString() ?? "-";
                }
                else
                {
                    user.Avatar = AvatarHelper.HandleAvatarNull(user.DisplayName);
                }

                return user;
            }
            catch (Exception ex)
            {
                LogHelper.SaveError(MethodBase.GetCurrentMethod(), ex.Message, username: username);
                return null;
            }
        }

    }
}