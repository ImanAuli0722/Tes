using System;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using Tomori.TIAS.Security.WS.Helpers;
using Tomori.TIAS.Security.WS.Helpers.Consts;
using Tomori.TIAS.Security.WS.Models.Entities.DBTIASSECURITY;
using Tomori.TIAS.Security.WS.Models.Enums;

namespace Tomori.TIAS.Security.WS.Repositories
{
    public class UserDbRepo
    {
        public User Get(string username, bool withPassword = false)
        {
            try
            {
                User result = null;
                using (SqlConnection conn = new SqlConnection(AppConst.CONNECTION_DB))
                {
                    string columnPassword = withPassword ? "u.Password," : "";
                    string query = $@"SELECT
	                TOP 1
                    {columnPassword}
	                u.DisplayName,
	                isnull(u.Avatar, '') as Avatar,
	                isnull(p.Name, '') as Position
                FROM T_TIAS_SECURITY_USERS u
                LEFT JOIN T_TIAS_SECURITY_POSITIONS p
	                ON u.PositionId = p.Id
                    AND p.DeletedAt IS NULL
                WHERE
	                u.Username = @Username
	                AND u.IsActive = 1
	                AND u.DeletedAt IS NULL";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.Add(new SqlParameter("@Username", SqlDbType.VarChar, 255) { Value = username });
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                result = new User
                                {
                                    Username = username,
                                    DisplayName = reader["DisplayName"].ToString(),
                                    Avatar = reader["Avatar"] != null ? reader["Avatar"].ToString() != "" && reader["Avatar"].ToString() != "/Avatar/" ? AvatarHelper.HandleAvatarExist(reader["Avatar"].ToString()) : AvatarHelper.HandleAvatarNull(reader["DisplayName"].ToString()) : AvatarHelper.HandleAvatarNull(reader["DisplayName"].ToString()),
                                    Position = reader["Position"].ToString(),
                                    UserSource = UserSourceEnum.DB
                                };
                                if (withPassword)
                                {
                                    result.Password = reader["Password"].ToString();
                                }
                            }
                        }
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                LogHelper.SaveError(MethodBase.GetCurrentMethod(), ex.Message, username: username);
                return null;
            }
        }

        public User Get(string username, string password)
        {
            try
            {
                User result = Get(username, true);

                if (result == null) return null;

                string hashPassword = result.Password;
                if (BCrypt.Net.BCrypt.Verify(password, hashPassword) == false) return null;

                result.Password = null;
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.SaveError(MethodBase.GetCurrentMethod(), ex.Message, username: username);
                return null;
            }
        }

    }
}