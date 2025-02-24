using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using Tomori.TIAS.Security.WS.Helpers;
using Tomori.TIAS.Security.WS.Helpers.Consts;
using Tomori.TIAS.Security.WS.Models.Enums;
using Module = Tomori.TIAS.Security.WS.Models.Entities.DBTIASSECURITY.Module;

namespace Tomori.TIAS.Security.WS.Repositories
{
    public class UserAccessRepo
    {
        public List<Module> GetModuleList(string username, UserSourceEnum userSource, string appCode)
        {
            try
            {
                var result = new List<Module>();
                using (SqlConnection conn = new SqlConnection(AppConst.CONNECTION_DB))
                {
                    string query = @"SELECT
	                m.ModuleCode,
	                m.Path
                FROM T_TIAS_SECURITY_MODULES m
                INNER JOIN T_TIAS_SECURITY_APPLICATIONS a
	                ON m.ApplicationId = a.Id
	                AND a.DeletedAt IS NULL
                WHERE a.ApplicationCode = @AppCode
	                AND (
		                m.Id IN (SELECT um.ModuleId FROM T_TIAS_SECURITY_USER_MAP_MODULES um WHERE um.Username = @Username AND um.UserType = @UserSource AND um.DeletedAt IS NULL)
		                OR m.Id IN (SELECT pm.ModuleId FROM T_TIAS_SECURITY_PACKAGE_MAP_MODULES pm
			                INNER JOIN T_TIAS_SECURITY_USER_MAP_PACKAGES up
				                ON pm.PackageId = up.PackageId AND up.Username = @Username AND UserType = @UserSource AND up.DeletedAt IS NULL
			                AND pm.DeletedAt IS NULL
			                )
		                )
	                AND m.DeletedAt IS NULL
                ORDER BY m.MenuIndex ASC";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.Add(new SqlParameter("@AppCode", SqlDbType.VarChar, 50) { Value = appCode });
                        cmd.Parameters.Add(new SqlParameter("@Username", SqlDbType.VarChar, 255) { Value = username });
                        cmd.Parameters.Add(new SqlParameter("@UserSource", SqlDbType.TinyInt, 4) { Value = userSource });
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result.Add(new Module
                                {
                                    ModuleCode = reader["ModuleCode"].ToString(),
                                    Path = reader["Path"].ToString(),
                                });
                            }
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.SaveError(MethodBase.GetCurrentMethod(), ex.Message);
                return null;
            }
        }

        public List<Module> GetModuleGuestList(string appCode)
        {
            try
            {
                var result = new List<Module>();
                using (SqlConnection conn = new SqlConnection(AppConst.CONNECTION_DB))
                {
                    string query = @"SELECT
	                m.ModuleCode,
	                m.Path
                FROM T_TIAS_SECURITY_MODULES m
                INNER JOIN T_TIAS_SECURITY_APPLICATIONS a
	                ON m.ApplicationId = a.Id
	                AND a.DeletedAt IS NULL
                WHERE a.ApplicationCode = @AppCode
	                AND m.DeletedAt IS NULL
                    AND m.AllowGuest = 1
                ORDER BY m.MenuIndex ASC";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.Add(new SqlParameter("@AppCode", SqlDbType.VarChar, 50) { Value = appCode });
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result.Add(new Module
                                {
                                    ModuleCode = reader["ModuleCode"].ToString(),
                                    Path = reader["Path"].ToString(),
                                });
                            }
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.SaveError(MethodBase.GetCurrentMethod(), ex.Message);
                return null;
            }
        }

    }
}