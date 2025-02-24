using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using Tomori.TIAS.Security.WS.Helpers;
using Tomori.TIAS.Security.WS.Helpers.Consts;

namespace Tomori.TIAS.Security.WS.Repositories
{
    public class ConfigRepo
    {
        public string GetValue(string key)
        {
            try
            {
                string result = null;
                using (SqlConnection conn = new SqlConnection(AppConst.CONNECTION_DB))
                {
                    string query = "SELECT TOP 1 ConfigValue FROM T_TIAS_SECURITY_CONFIGS WHERE ConfigKey = @Key AND DeletedAt IS NULL";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.Add(new SqlParameter("@Key", SqlDbType.VarChar, 50) { Value = key });
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                result = reader["ConfigValue"].ToString();
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

        public bool GetCanLoginAllDevice()
        {
            try
            {
                string value = GetValue(ConfigurationManager.AppSettings["CAN_LOGIN_ALL_DEVICE"]);
                if (value == null) return false;
                return value == "1" || value.ToUpper() == "TRUE";
            }
            catch (Exception ex)
            {
                LogHelper.SaveError(MethodBase.GetCurrentMethod(), ex.Message);
                return false;
            }
        }

    }
}