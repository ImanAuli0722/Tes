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
    public class SessionRepo
    {
        public bool CheckLoginExist(string username, UserSourceEnum userSource)
        {
            bool result = false;
            try
            {
                using (SqlConnection conn = new SqlConnection(AppConst.CONNECTION_DB))
                {
                    string query = "SELECT TOP 1 Id FROM T_TIAS_SECURITY_SESSIONS " +
                        "WHERE Username = @Username " +
                        "AND UserSource = @UserSource " +
                        "AND DeletedAt IS NULL " +
                        "AND (LogoutAt IS NULL " +
                        "AND ExpiredAt > CURRENT_TIMESTAMP)";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.Add(new SqlParameter("@Username", SqlDbType.VarChar, 255) { Value = username });
                        cmd.Parameters.Add(new SqlParameter("@UserSource", SqlDbType.TinyInt, 4) { Value = userSource });
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                result = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.SaveError(MethodBase.GetCurrentMethod(), ex.Message);
            }
            return result;
        }

        public string Insert(string username, UserSourceEnum userSource, string appCode, DateTime loginAt, DateTime expiredAt, string ipAddress)
        {
            string result;

            using (SqlConnection conn = new SqlConnection(AppConst.CONNECTION_DB))
            {
                conn.Open();
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        string query = "INSERT INTO T_TIAS_SECURITY_SESSIONS " +
                        "(Username, LoginAt, ApplicationCode, ExpiredAt, IpAddress, UserSource, CreatedAt, UpdatedAt) OUTPUT INSERTED.Id " +
                        "VALUES (@Username, @LoginAt, @ApplicationCode, @ExpiredAt, @IpAddress, @UserSource, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP)";
                        using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                        {
                            cmd.Parameters.Add(new SqlParameter("@Username", SqlDbType.VarChar, 255) { Value = username });
                            cmd.Parameters.Add(new SqlParameter("@ApplicationCode", SqlDbType.VarChar, 255) { Value = appCode });
                            cmd.Parameters.Add(new SqlParameter("@UserSource", SqlDbType.TinyInt) { Value = userSource });
                            cmd.Parameters.Add(new SqlParameter("@LoginAt", SqlDbType.DateTime) { Value = loginAt });
                            cmd.Parameters.Add(new SqlParameter("@ExpiredAt", SqlDbType.DateTime) { Value = expiredAt });
                            cmd.Parameters.Add(new SqlParameter("@IpAddress", SqlDbType.VarChar, 255) { Value = ipAddress });

                            result = cmd.ExecuteScalar()?.ToString();
                        }
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception($"{LogHelper.GetInfoMethod(MethodBase.GetCurrentMethod())}: {ex.Message}", ex);
                    }
                }
            }

            return result;
        }

        public void Logout(string id)
        {
            using (SqlConnection conn = new SqlConnection(AppConst.CONNECTION_DB))
            {
                conn.Open();
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        string query = @"UPDATE T_TIAS_SECURITY_SESSIONS SET LogoutAt = CURRENT_TIMESTAMP, UpdatedAt = CURRENT_TIMESTAMP WHERE Id = @Id";
                        using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                        {
                            cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.VarChar, 36) { Value = id });
                            cmd.ExecuteNonQuery();
                        }
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception($"{LogHelper.GetInfoMethod(MethodBase.GetCurrentMethod())}: {ex.Message}", ex);
                    }
                }
            }
        }

        public Session Get(string id)
        {
            try
            {
                Session result = null;
                using (SqlConnection conn = new SqlConnection(AppConst.CONNECTION_DB))
                {
                    string query = $@"SELECT
	                TOP 1
	                Username,
	                UserSource
                FROM T_TIAS_SECURITY_SESSIONS
                WHERE
	                id = @Id
                    AND LogoutAt IS NULL
                    AND ExpiredAt > CURRENT_TIMESTAMP
	                AND DeletedAt IS NULL";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.VarChar, 255) { Value = id });
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                UserSourceEnum userSource = UserSourceEnum.AD;
                                switch (reader["UserSource"].ToString())
                                {
                                    case "0":
                                        userSource = UserSourceEnum.AD;
                                        break;
                                    case "1":
                                        userSource = UserSourceEnum.DB;
                                        break;
                                    case "2":
                                        userSource = UserSourceEnum.Guest;
                                        break;
                                }
                                result = new Session
                                {
                                    Username = reader["Username"].ToString(),
                                    UserSource = userSource,
                                };
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

        public void UpdateExpiredAt(string id, DateTime expireAt)
        {
            using (SqlConnection conn = new SqlConnection(AppConst.CONNECTION_DB))
            {
                conn.Open();
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        string query = @"UPDATE T_TIAS_SECURITY_SESSIONS SET ExpiredAt = @ExpiredAt, UpdatedAt = CURRENT_TIMESTAMP WHERE Id = @Id";
                        using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                        {
                            cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.VarChar, 36) { Value = id });
                            cmd.Parameters.Add(new SqlParameter("@ExpiredAt", SqlDbType.DateTime) { Value = expireAt });
                            cmd.ExecuteNonQuery();
                        }
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception($"{LogHelper.GetInfoMethod(MethodBase.GetCurrentMethod())}: {ex.Message}", ex);
                    }
                }
            }
        }

    }
}