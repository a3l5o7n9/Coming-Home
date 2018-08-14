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
                    userId = int.Parse(sdr["User_ID"].ToString());
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

        static public User Login(string userName, string userPassword)
        {
            User u = null;
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
                    u = new User(int.Parse(sdr["User_Id"].ToString()), userName, userPassword, sdr["First_Name"].ToString(), sdr["Last_Name"].ToString());
                }

                return u;
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

            return u;
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
