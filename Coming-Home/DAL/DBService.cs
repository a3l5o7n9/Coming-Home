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
                    userId = int.Parse(sdr[0].ToString());
                }

                return userId;
            }
            catch (Exception e)
            {
                userId = -2;
                File.AppendAllText(Globals.LogFileName,
                   "ERROR in class:DBService function:Register() - message=" + e.Message +
                   ", on the " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + Environment.NewLine);
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

        public static void InitErrorLogPath(string v)
        {
            Globals.LogFileName = v + "\\Errors_Log.txt";
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
                bool isFirstLine = true;

                AppUser au = null;
                User u = null;
                List<User> lu = new List<User>();
                List<Home> lh = new List<Home>();

                while (sdr.Read())
                {
                    if (sdr["Home_Id"].ToString() == "")
                    {
                        au = new AppUser(int.Parse(sdr["User_Id"].ToString()), userName, userPassword, sdr["First_Name"].ToString(), sdr["Last_Name"].ToString());
                  
                        jd = new JsonData(au, "Data");
                        return jd;
                    }

                    if (isFirstLine == true)
                    {
                        au = new AppUser(int.Parse(sdr["User_Id"].ToString()), userName, userPassword, sdr["First_Name"].ToString(), sdr["Last_Name"].ToString());
                        isFirstLine = false;
                    }

                    lu.Add(new User(int.Parse(sdr["User_Id"].ToString()), userName, userPassword, sdr["First_Name"].ToString(), sdr["Last_Name"].ToString(), int.Parse(sdr["Home_Id"].ToString()), sdr["User_Type_Name"].ToString(), sdr["Token"].ToString()));
                    lh.Add(new Home(int.Parse(sdr["Home_Id"].ToString()), sdr["Home_Name"].ToString(), int.Parse(sdr["Number_Of_Users"].ToString()), sdr["Address"].ToString()));
                }

                jd = new JsonData(au, lu, lh, "Data");

                return jd;
            }
            catch (Exception e)
            {
                File.AppendAllText(Globals.LogFileName,
                   "ERROR in class:DBService function:Login() - message=" + e.Message +
                   ", on the " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + Environment.NewLine);
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

        //static public JsonData GetUserDetails(int userId)
        //{
        //    JsonData jd = null;

        //    com = new SqlCommand("Get_User_Details", con);
        //    com.CommandType = CommandType.StoredProcedure;

        //    com.Parameters.Clear();
        //    com.Parameters.Add(new SqlParameter("@UserId", userId));

        //    try
        //    {
        //        com.Connection.Open();
        //        sdr = com.ExecuteReader();
        //        bool isFirstLine = true;

        //        User u = null;
        //        List<User> lu = new List<User>();
        //        List<Home> lh = new List<Home>();

        //        while (sdr.Read())
        //        {
        //            if (sdr["Home_Id"].ToString() == "")
        //            {
        //                u = new User(int.Parse(sdr["User_Id"].ToString()), sdr["User_Name"].ToString(), sdr["User_Password"].ToString(), sdr["First_Name"].ToString(), sdr["Last_Name"].ToString());
        //                lu.Add(u);

        //                jd = new JsonData(u, lu, "Data");
        //                return jd;
        //            }

        //            if (isFirstLine == true)
        //            {
        //                u = new User(int.Parse(sdr["User_Id"].ToString()), sdr["User_Name"].ToString(), sdr["User_Password"].ToString(), sdr["First_Name"].ToString(), sdr["Last_Name"].ToString(), sdr["User_Type_Name"].ToString(), sdr["Token"].ToString());
        //                isFirstLine = false;
        //            }

        //            lu.Add(new User(int.Parse(sdr["User_Id"].ToString()), sdr["User_Name"].ToString(), sdr["User_Password"].ToString(), sdr["First_Name"].ToString(), sdr["Last_Name"].ToString(), sdr["User_Type_Name"].ToString(), sdr["Token"].ToString()));
        //            lh.Add(new Home(int.Parse(sdr["Home_Id"].ToString()), sdr["Home_Name"].ToString(), int.Parse(sdr["Number_Of_Users"].ToString()), sdr["Address"].ToString()));
        //        }

        //        jd = new JsonData(u, lu, lh, "Data");

        //        return jd;
        //    }
        //    catch (Exception e)
        //    {
        //        File.AppendAllText(Globals.LogFileName,
        //           "ERROR in class:DBService function:GetUserDetails() - message=" + e.Message +
        //           ", on the " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + Environment.NewLine);
        //    }
        //    finally
        //    {
        //        if (com.Connection.State == ConnectionState.Open)
        //        {
        //            com.Connection.Close();
        //        }
        //    }

        //    return jd;
        //}

        static public int CreateHome(int userId, string homeName, string address)
        {
            int homeId = -1;
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
                    homeId = int.Parse(sdr[0].ToString());
                }

                return homeId;
            }
            catch (Exception e)
            {
                File.AppendAllText(Globals.LogFileName,
                   "ERROR in class:DBService function:CreateHome() - message=" + e.Message +
                   ", on the " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + Environment.NewLine);
            }
            finally
            {
                if (com.Connection.State == ConnectionState.Open)
                {
                    com.Connection.Close();
                }
            }

            return homeId;
        }

        static public JsonData JoinHome(int userId, string homeName, string address)
        {
            JsonData jd = null;
            AppUser au = null;
            User u = null;
            Home h = null;
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
                    h = new Home(int.Parse(sdr["Home_Id"].ToString()), homeName, int.Parse(sdr["Number_Of_Users"].ToString()), address);
                    jd = new JsonData(au, u, h, "Data");
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
                   "ERROR in class:DBService function:JoinHome() - message=" + e.Message +
                   ", on the " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + Environment.NewLine);
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

        static public int CreateRoom(string roomName, int homeId, string roomTypeName)
        {
            int roomId = -1;
            com = new SqlCommand("New_Room", con);
            com.CommandType = CommandType.StoredProcedure;

            com.Parameters.Clear();
            com.Parameters.Add(new SqlParameter("@RoomName", roomName));
            com.Parameters.Add(new SqlParameter("@HomeId", homeId));
            com.Parameters.Add(new SqlParameter("@RoomTypeName", roomTypeName));

            try
            {
                com.Connection.Open();
                sdr = com.ExecuteReader();

                if (sdr.Read())
                {
                    roomId = int.Parse(sdr[0].ToString());
                }

                return roomId;
            }
            catch (Exception e)
            {
                File.AppendAllText(Globals.LogFileName,
                   "ERROR in class:DBService function:CreateRoom() - message=" + e.Message +
                   ", on the " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + Environment.NewLine);
            }
            finally
            {
                if (com.Connection.State == ConnectionState.Open)
                {
                    com.Connection.Close();
                }
            }

            return roomId;
        }

        static public int CreateDevice(string deviceName, int homeId, string deviceTypeName, int userId, int roomId)
        {
            int deviceId = -1;
            com = new SqlCommand("New_Device", con);
            com.CommandType = CommandType.StoredProcedure;

            com.Parameters.Clear();
            com.Parameters.Add(new SqlParameter("@DeviceName", deviceName));
            com.Parameters.Add(new SqlParameter("@HomeId", homeId));
            com.Parameters.Add(new SqlParameter("@DeviceTypeName", deviceTypeName));
            com.Parameters.Add(new SqlParameter("@UserId", userId));
            com.Parameters.Add(new SqlParameter("@RoomId", roomId));

            try
            {
                com.Connection.Open();
                sdr = com.ExecuteReader();

                if (sdr.Read())
                {
                    deviceId = int.Parse(sdr[0].ToString());
                }

                return deviceId;
            }
            catch (Exception e)
            {
                File.AppendAllText(Globals.LogFileName,
                   "ERROR in class:DBService function:CreateDevice() - message=" + e.Message +
                   ", on the " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + Environment.NewLine);
            }
            finally
            {
                if (com.Connection.State == ConnectionState.Open)
                {
                    com.Connection.Close();
                }
            }

            return deviceId;
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
                   "ERROR in class:DBService function:UpdateTokenForUserId() - message=" + e.Message +
                   ", on the " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + Environment.NewLine);
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

        static public int BindDeviceToRoom(int roomId, int deviceId, int userId)
        {
            int res = -1;

            com = new SqlCommand("Bind_Device_To_Room", con);
            com.CommandType = CommandType.StoredProcedure;

            com.Parameters.Clear();
            com.Parameters.Add(new SqlParameter("@RoomId", roomId));
            com.Parameters.Add(new SqlParameter("@DeviceId", deviceId));
            com.Parameters.Add(new SqlParameter("@UserId", userId));

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
                   "ERROR in class:DBService function:BindDeviceToRoom() - message=" + e.Message +
                   ", on the " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + Environment.NewLine);
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
