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
                List<User> lu = new List<User>();
                List<Home> lh = new List<Home>();

                while (sdr.Read())
                {
                    if (sdr["Home_Id"].ToString() == "")
                    {
                        au = new AppUser(int.Parse(sdr["User_Id"].ToString()), userName, userPassword, sdr["First_Name"].ToString(), sdr["Last_Name"].ToString());
                        lu = null;
                        lh = null;

                        jd = new JsonData(au, lu, lh, "Data");
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

        static public int JoinHome(int userId, string homeName, string address)
        {
            int homeId = -1;
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
                    homeId = int.Parse(sdr[0].ToString());
                    return homeId;
                }

                return homeId;
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

            return homeId;
        }

        static public int CreateRoom(string roomName, int homeId, string roomTypeName, bool isShared, int userId)
        {
            int roomId = -1;
            com = new SqlCommand("New_Room", con);
            com.CommandType = CommandType.StoredProcedure;

            com.Parameters.Clear();
            com.Parameters.Add(new SqlParameter("@RoomName", roomName));
            com.Parameters.Add(new SqlParameter("@HomeId", homeId));
            com.Parameters.Add(new SqlParameter("@RoomTypeName", roomTypeName));
            com.Parameters.Add(new SqlParameter("@IsShared", isShared));
            com.Parameters.Add(new SqlParameter("@UserId", userId));

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

        static public int CreateDevice(string deviceName, int homeId, string deviceTypeName, bool isDividedIntoRooms, int userId, int roomId)
        {
            int deviceId = -1;
            com = new SqlCommand("New_Device", con);
            com.CommandType = CommandType.StoredProcedure;

            com.Parameters.Clear();
            com.Parameters.Add(new SqlParameter("@DeviceName", deviceName));
            com.Parameters.Add(new SqlParameter("@HomeId", homeId));
            com.Parameters.Add(new SqlParameter("@DeviceTypeName", deviceTypeName));
            com.Parameters.Add(new SqlParameter("@IsDividedIntoRooms", isDividedIntoRooms));
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

        static public int UpdateUserTypeInHome(int appUserId, int userToUpdateId, int homeId, string appUserTypeName, string currentUserTypeName, string updatedUserTypeName)
        {
            int res = -1;

            com = new SqlCommand("Update_User_Type_In_Home", con);
            com.CommandType = CommandType.StoredProcedure;

            com.Parameters.Clear();
            com.Parameters.Add(new SqlParameter("@AppUserId", appUserId));
            com.Parameters.Add(new SqlParameter("@UserIdToUpdate", userToUpdateId));
            com.Parameters.Add(new SqlParameter("@HomeId", homeId));
            com.Parameters.Add(new SqlParameter("@AppUserTypeName", appUserTypeName));
            com.Parameters.Add(new SqlParameter("@CurrentUserTypeName", currentUserTypeName));
            com.Parameters.Add(new SqlParameter("@UpdatedUserTypeName", updatedUserTypeName));

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
                  "ERROR in class:DBService function:UpdateUserTypeInHome() - message=" + e.Message +
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

        static public int UpdateUserDevicePermissions(int appUserId, int userToUpdateId, int homeId, string appUserTypeName, string userToUpdateTypeName, int deviceId, int roomId, bool hasPermission)
        {
            int res = -1;

            com = new SqlCommand("Update_User_Device_Permissions", con);
            com.CommandType = CommandType.StoredProcedure;

            com.Parameters.Clear();
            com.Parameters.Add(new SqlParameter("@AppUserId", appUserId));
            com.Parameters.Add(new SqlParameter("@UserIdToUpdate", userToUpdateId));
            com.Parameters.Add(new SqlParameter("@HomeId", homeId));
            com.Parameters.Add(new SqlParameter("@AppUserTypeName", appUserTypeName));
            com.Parameters.Add(new SqlParameter("@UserToUpdateTypeName", userToUpdateTypeName));
            com.Parameters.Add(new SqlParameter("@DeviceId", deviceId));
            com.Parameters.Add(new SqlParameter("@RoomId", roomId));
            com.Parameters.Add(new SqlParameter("@HasPermission", hasPermission));

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
                  "ERROR in class:DBService function:UpdateUserDevicePermissions() - message=" + e.Message +
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

        static public int UpdateUserRoomPermissions(int appUserId, int userToUpdateId, int homeId, string appUserTypeName, string userToUpdateTypeName, int roomId, bool hasAccess)
        {
            int res = -1;

            com = new SqlCommand("Update_User_Room_Permissions", con);
            com.CommandType = CommandType.StoredProcedure;

            com.Parameters.Clear();
            com.Parameters.Add(new SqlParameter("@AppUserId", appUserId));
            com.Parameters.Add(new SqlParameter("@UserIdToUpdate", userToUpdateId));
            com.Parameters.Add(new SqlParameter("@HomeId", homeId));
            com.Parameters.Add(new SqlParameter("@AppUserTypeName", appUserTypeName));
            com.Parameters.Add(new SqlParameter("@UserToUpdateTypeName", userToUpdateTypeName));
            com.Parameters.Add(new SqlParameter("@RoomId", roomId));
            com.Parameters.Add(new SqlParameter("@HasAccess", hasAccess));

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
                  "ERROR in class:DBService function:UpdateUserRoomPermissions() - message=" + e.Message +
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

        static public JsonData GetUser(int userId, int homeId)
        {
            JsonData jd = null;

            com = new SqlCommand("Get_User", con);
            com.CommandType = CommandType.StoredProcedure;

            com.Parameters.Clear();
            com.Parameters.Add(new SqlParameter("@UserId", userId));
            com.Parameters.Add(new SqlParameter("@HomeId", homeId));

            try
            {
                com.Connection.Open();
                sdr = com.ExecuteReader();
                User u = null;

                if(sdr.Read())
                {
                    u = new User(userId, sdr["User_Name"].ToString(), sdr["User_Password"].ToString(), sdr["First_Name"].ToString(), sdr["Last_Name"].ToString(), int.Parse(sdr["Home_Id"].ToString()), sdr["User_Type_Name"].ToString(), sdr["Token"].ToString());
                    jd = new JsonData(u, "Data");
                }
            }
            catch (Exception e)
            {
                File.AppendAllText(Globals.LogFileName,
                 "ERROR in class:DBService function:GetUser() - message=" + e.Message +
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

        static public JsonData GetHome(int userId, int homeId)
        {
            JsonData jd = null;

            com = new SqlCommand("Get_Home", con);
            com.CommandType = CommandType.StoredProcedure;

            com.Parameters.Clear();
            com.Parameters.Add(new SqlParameter("@HomeId", homeId));
            com.Parameters.Add(new SqlParameter("@UserId", userId));

            try
            {
                com.Connection.Open();
                sdr = com.ExecuteReader();
                Home h = null;

                if (sdr.Read())
                {
                    h = new Home(homeId, sdr["Home_Name"].ToString(), int.Parse(sdr["Number_Of_Users"].ToString()), sdr["Address"].ToString());
                    jd = new JsonData(h, "Data");
                }
            }
            catch (Exception e)
            {
                File.AppendAllText(Globals.LogFileName,
                 "ERROR in class:DBService function:GetHome() - message=" + e.Message +
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

        static public JsonData GetDevice(int userId, int homeId, int deviceId, int roomId)
        {
            JsonData jd = null;

            com = new SqlCommand("Get_Device", con);
            com.CommandType = CommandType.StoredProcedure;

            com.Parameters.Clear();
            com.Parameters.Add(new SqlParameter("@UserId", userId));
            com.Parameters.Add(new SqlParameter("@HomeId", homeId));
            com.Parameters.Add(new SqlParameter("@DeviceId", deviceId));
            com.Parameters.Add(new SqlParameter("@RoomId", roomId));

            try
            {
                com.Connection.Open();
                sdr = com.ExecuteReader();
                Device d = null;

                if (sdr.Read())
                {
                    d = new Device(deviceId, sdr["Device_Name"].ToString(), sdr["Device_Type_Name"].ToString(), homeId, bool.Parse(sdr["Is_Divided_Into_Rooms"].ToString()), roomId);
                    jd = new JsonData(d, "Data");
                }
            }
            catch (Exception e)
            {
                File.AppendAllText(Globals.LogFileName,
                 "ERROR in class:DBService function:GetDevice() - message=" + e.Message +
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

        static public JsonData GetRoom(int userId, int homeId, int roomId)
        {
            JsonData jd = null;

            com = new SqlCommand("Get_Room", con);
            com.CommandType = CommandType.StoredProcedure;

            com.Parameters.Clear();
            com.Parameters.Add(new SqlParameter("@UserId", userId));
            com.Parameters.Add(new SqlParameter("@HomeId", homeId));
            com.Parameters.Add(new SqlParameter("@RoomId", roomId));

            try
            {
                com.Connection.Open();
                sdr = com.ExecuteReader();
                Room r = null;

                if (sdr.Read())
                {
                    r = new Room(roomId, sdr["Room_Name"].ToString(), sdr["Room_Type_Name"].ToString(), homeId, bool.Parse(sdr["Is_Shared"].ToString()), int.Parse(sdr["Number_Of_Devices"].ToString()));
                    jd = new JsonData(r, "Data");
                }
            }
            catch (Exception e)
            {
                File.AppendAllText(Globals.LogFileName,
                 "ERROR in class:DBService function:GetRoom() - message=" + e.Message +
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

        static public JsonData GetUsersInHome(int userId, int homeId)
        {
            JsonData jd = null;

            com = new SqlCommand("Get_Users_In_Home", con);
            com.CommandType = CommandType.StoredProcedure;

            com.Parameters.Clear();
            com.Parameters.Add(new SqlParameter("@UserId", userId));
            com.Parameters.Add(new SqlParameter("@HomeId", homeId));

            try
            {
                com.Connection.Open();
                sdr = com.ExecuteReader();
                List<User> lu = new List<User>();

                while (sdr.Read())
                {
                    lu.Add(new User(userId, sdr["User_Name"].ToString(), sdr["User_Password"].ToString(), sdr["First_Name"].ToString(), sdr["Last_Name"].ToString(), int.Parse(sdr["Home_Id"].ToString()), sdr["User_Type_Name"].ToString(), sdr["Token"].ToString()));
                    jd = new JsonData(lu, "Data");
                }
            }
            catch (Exception e)
            {
                File.AppendAllText(Globals.LogFileName,
                 "ERROR in class:DBService function:GetUsersInHome() - message=" + e.Message +
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

        static public JsonData GetUserRoomsInHome(int userId, int homeId)
        {
            JsonData jd = null;

            com = new SqlCommand("Get_User_Rooms_In_Home", con);
            com.CommandType = CommandType.StoredProcedure;

            com.Parameters.Clear();
            com.Parameters.Add(new SqlParameter("@UserId", userId));
            com.Parameters.Add(new SqlParameter("@HomeId", homeId));

            try
            {
                com.Connection.Open();
                sdr = com.ExecuteReader();
                List<Room> lr = new List<Room>();

                while (sdr.Read())
                {
                    lr.Add(new Room(int.Parse(sdr["Room_Id"].ToString()), sdr["Room_Name"].ToString(), sdr["Room_Type_Name"].ToString(), homeId, bool.Parse(sdr["Is_Shared"].ToString()), int.Parse(sdr["Number_Of_Devices"].ToString())));
                    jd = new JsonData(lr, "Data");
                }
            }
            catch (Exception e)
            {
                File.AppendAllText(Globals.LogFileName,
                 "ERROR in class:DBService function:GetUserRoomsInHome() - message=" + e.Message +
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

        static public JsonData GetUserDevicesInHome(int userId, int homeId)
        {
            JsonData jd = null;

            com = new SqlCommand("Get_User_Devices_In_Home", con);
            com.CommandType = CommandType.StoredProcedure;

            com.Parameters.Clear();
            com.Parameters.Add(new SqlParameter("@UserId", userId));
            com.Parameters.Add(new SqlParameter("@HomeId", homeId));

            try
            {
                com.Connection.Open();
                sdr = com.ExecuteReader();
                List<Device> ld = new List<Device>();

                while (sdr.Read())
                {
                    ld.Add(new Device(int.Parse(sdr["Device_Id"].ToString()), sdr["Device_Name"].ToString(), sdr["Device_Type_Name"].ToString(), homeId, bool.Parse(sdr["Is_Divided_Into_Rooms"].ToString()), int.Parse(sdr["Room_Id"].ToString())));
                    jd = new JsonData(ld, "Data");
                }
            }
            catch (Exception e)
            {
                File.AppendAllText(Globals.LogFileName,
                 "ERROR in class:DBService function:GetUserDevicesInHome() - message=" + e.Message +
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
    }
}
