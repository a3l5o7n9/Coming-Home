using BEL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    static public class DBService
    {
        static SqlConnection con = new SqlConnection(Globals.strCon);
        static SqlCommand com = new SqlCommand();
        static SqlDataReader sdr = null;

        static public int Register(string userName, string userPassword, string firstName, string lastName)
        {
            int userId = -1;
            com = new SqlCommand("New_User", con);
            com.CommandType = CommandType.StoredProcedure;

            com.Parameters.Clear();

            com.Parameters.Add(new SqlParameter("@UserName", userName));
            com.Parameters.Add(new SqlParameter("@UserPassword", userPassword));
            com.Parameters.Add(new SqlParameter("@FirstName", firstName));
            com.Parameters.Add(new SqlParameter("@LastName", lastName));

            try
            {
                com.Connection.Open();
                sdr = com.ExecuteReader();

                if (sdr.Read())
                {
                    userId = int.Parse(sdr["User_Id"].ToString());
                }

                return userId;
            }
            catch (Exception e)
            {
                userId = -2;
                File.AppendAllText(Globals.LogFileName,
                   "ERROR in class:DBServices function:Register() - message=" + e.Message +
                   ", on the" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
            }
            finally
            {
                if (com.Connection.State == ConnectionState.Open)
                {
                    com.Connection.Close();
                }
            }

            return userId;
        }

        static public JsonData Login(string userName, string userPassword)
        {
            JsonData jd = null;
            com = new SqlCommand("Validate_Login", con);
            com.CommandType = CommandType.StoredProcedure;

            com.Parameters.Clear();
            com.Parameters.Add(new SqlParameter("@UserName", userName));
            com.Parameters.Add(new SqlParameter("@UserPassword", userPassword));

            try
            {
                com.Connection.Open();
                sdr = com.ExecuteReader();

                if (sdr.Read())
                {
                    if (sdr["Home_Id"] == null)
                    {
                        jd = new JsonData(new User(int.Parse(sdr["User_Id"].ToString()), userName, userPassword, sdr["First_Name"].ToString(), sdr["Last_Name"].ToString()));
                        return jd;
                    }

                    jd = new JsonData(new User(int.Parse(sdr["User_Id"].ToString()), userName, userPassword, sdr["First_Name"].ToString(), sdr["Last_Name"].ToString(), sdr["Home_Name"].ToString(), sdr["User_Type_Name"].ToString(), sdr["Token"].ToString()), new Home(int.Parse(sdr["Home_Id"].ToString()),sdr["Home_Name"].ToString(), int.Parse(sdr["Number_Of_Users"].ToString()), sdr["Address"].ToString()));
                }

                return jd;
            }
            catch (Exception e)
            {
                File.AppendAllText(Globals.LogFileName,
                   "ERROR in class:DBServices function:Register() - message=" + e.Message +
                   ", on the" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
            }
            finally
            {
                if (com.Connection.State == ConnectionState.Open)
                {
                    com.Connection.Close();
                }
            }

            return jd;
        }

        static public JsonData CreateHome(int userId, string homeName, string address)
        {
            JsonData jd = null;
            com = new SqlCommand("New_Home", con);
            com.CommandType = CommandType.StoredProcedure;

            com.Parameters.Clear();
            com.Parameters.Add(new SqlParameter("@UserId", userId));
            com.Parameters.Add(new SqlParameter("@HomeName", homeName));
            com.Parameters.Add(new SqlParameter("@Address", address));

            try
            {
                com.Connection.Open();
                sdr = com.ExecuteReader();

                if (sdr.Read())
                {
                    jd = new JsonData(new Home(int.Parse(sdr[0].ToString()), homeName, 1, address));
                }

                return jd;
            }
            catch (Exception e)
            {
                File.AppendAllText(Globals.LogFileName,
                   "ERROR in class:DBServices function:Register() - message=" + e.Message +
                   ", on the" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
            }
            finally
            {
                if (com.Connection.State == ConnectionState.Open)
                {
                    com.Connection.Close();
                }
            }

            return jd;
        }

        static public JsonData JoinHome(int userId, string homeName, string address)
        {
            JsonData jd = null;
            com = new SqlCommand("Join_Existing_Home", con);
            com.CommandType = CommandType.StoredProcedure;

            com.Parameters.Clear();
            com.Parameters.Add(new SqlParameter("@UserId", userId));
            com.Parameters.Add(new SqlParameter("@HomeName", homeName));
            com.Parameters.Add(new SqlParameter("@Address", address));

            try
            {
                com.Connection.Open();
                sdr = com.ExecuteReader();

                if (sdr.Read())
                {
                    jd = new JsonData(new Home(int.Parse(sdr["Home_Id"].ToString()), homeName, int.Parse(sdr["Number_Of_Users"].ToString()), address));
                }
                else
                {
                    return null;
                }

                return jd;
            }
            catch (Exception e)
            {
                File.AppendAllText(Globals.LogFileName,
                   "ERROR in class:DBServices function:Register() - message=" + e.Message +
                   ", on the" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
            }
            finally
            {
                if (com.Connection.State == ConnectionState.Open)
                {
                    com.Connection.Close();
                }
            }

            return jd;
        }

        static public int UpdateTokenForUserId(string token, int userId)
        {
            int res = -1;

            com = new SqlCommand("Update_User_Token", con);
            com.CommandType = CommandType.StoredProcedure;

            com.Parameters.Clear();
            com.Parameters.Add(new SqlParameter("@UserId", userId));
            com.Parameters.Add(new SqlParameter("@Token", token));

            try
            {
                com.Connection.Open();
                sdr = com.ExecuteReader();

                if (sdr.Read())
                {
                    res = int.Parse(sdr[0].ToString());
                }

                return res;
            }
            catch (Exception e)
            {
                File.AppendAllText(Globals.LogFileName,
                   "ERROR in class:DBServices function:Register() - message=" + e.Message +
                   ", on the" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
            }
            finally
            {
                if (com.Connection.State == ConnectionState.Open)
                {
                    com.Connection.Close();
                }
            }

            return res;
        }
    }
}
