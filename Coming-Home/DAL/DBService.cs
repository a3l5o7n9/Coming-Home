﻿using BEL;
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

        static public JsonData JoinHome(int userId, string homeName, string address)
        {
            string resMes = "No Data";
            int homeId = -1;
            JsonData jd = null;
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
                    homeId = int.Parse(sdr["Home_Id"].ToString());
                    
                    switch(homeId)
                    {
                        case -1:
                            {
                                resMes = "Home Not Found";
                                break;
                            }
                        case 0:
                            {
                                resMes = "User is already registered as a member of this home";
                                break;
                            }
                        default:
                            {
                                resMes = "Data";
                                h = new Home(homeId, homeName, int.Parse(sdr["Number_Of_Users"].ToString()), address);
                                break;
                            }
                    }
                }

                jd = new JsonData(h, resMes);
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
            string resMes = "No Data";
            int uId = 0;

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
                    uId = int.Parse(sdr["User_Id"].ToString());

                    switch(uId)
                    {
                        case -3:
                            {
                                resMes = "Error! You are not registered as a member of this home, and so, cannot see its users";
                                break;
                            }
                        case -2:
                            {
                                resMes = "Error! Home not found";
                                break;
                            }
                        case -1:
                            {
                                resMes = "Error! User not found";
                                break;
                            }
                        default:
                            {
                                u = new User(userId, sdr["User_Name"].ToString(), sdr["User_Password"].ToString(), sdr["First_Name"].ToString(), sdr["Last_Name"].ToString(), int.Parse(sdr["Home_Id"].ToString()), sdr["User_Type_Name"].ToString(), sdr["Token"].ToString());
                                resMes = "Data";
                                break;
                            }
                    }
                }

                jd = new JsonData(u, resMes);

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
            string resMes = "No Data";
            int hId = 0;

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
                    hId = int.Parse(sdr["Home_Id"].ToString());

                    switch(hId)
                    {
                        case -3:
                            {
                                resMes = "Error! You are not registered as a member of this home";
                                break;
                            }
                        case -2:
                            {
                                resMes = "Error! Home not found";
                                break;
                            }
                        case -1:
                            {
                                resMes = "Error! User not found";
                                break;
                            }
                        default:
                            {
                                h = new Home(homeId, sdr["Home_Name"].ToString(), int.Parse(sdr["Number_Of_Users"].ToString()), sdr["Address"].ToString());
                                resMes = "Data";
                                break;
                            }
                    }
                }

                jd = new JsonData(h, resMes);

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
            string resMes = "No Data";
            int dId = 0;

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
                    dId = int.Parse(sdr["Device_Id"].ToString());
                    switch(dId)
                    {
                        case -6:
                            {
                                resMes = "Error! You do not have permission to access this device";
                                break;
                            }
                        case -5:
                            {
                                resMes = "Error! Device not found";
                                break;
                            }
                        case -4:
                            {
                                resMes = "Error! Room not found";
                                break;
                            }
                        case -3:
                            {
                                resMes = "Error! You are not registered as a member of this home";
                                break;
                            }
                        case -2:
                            {
                                resMes = "Error! Home not found";
                                break;
                            }
                        case -1:
                            {
                                resMes = "Error! User not found";
                                break;
                            }
                        default:
                            {
                                d = new Device(deviceId, sdr["Device_Name"].ToString(), sdr["Device_Type_Name"].ToString(), homeId, bool.Parse(sdr["Is_Divided_Into_Rooms"].ToString()), roomId);
                                resMes = "Data";
                                break;
                            }
                    }
                }

                jd = new JsonData(d, resMes);

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
            string resMes = "No Data";
            int rId = 0;

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
                    rId = int.Parse(sdr["Room_Id"].ToString());
                    switch(rId)
                    {
                        case -5:
                            {
                                resMes = "Error! You do not have access to this room";
                                break;
                            }
                        case -4:
                            {
                                resMes = "Error! Room not found";
                                break;
                            }
                        case -3:
                            {
                                resMes = "Error! You are not registered as a member of this home";
                                break;
                            }
                        case -2:
                            {
                                resMes = "Error! Home not found";
                                break;
                            }
                        case -1:
                            {
                                resMes = "Error! User not found";
                                break;
                            }
                        default:
                            {
                                r = new Room(roomId, sdr["Room_Name"].ToString(), sdr["Room_Type_Name"].ToString(), homeId, bool.Parse(sdr["Is_Shared"].ToString()), int.Parse(sdr["Number_Of_Devices"].ToString()));
                                resMes = "Data";
                                break;
                            }
                    }                
                }

                jd = new JsonData(r, resMes);

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
            string resMes = "No Data";

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
                int uId = 0;

                while (sdr.Read())
                {
                    uId = int.Parse(sdr["User_Id"].ToString());

                    switch(uId)
                    {
                        case -3:
                            {
                                resMes = "Error! You are not registered as a member of this home";
                                break;
                            }
                        case -2:
                            {
                                resMes = "Error! Home not found";
                                break;
                            }
                        case -1:
                            {
                                resMes = "Error! User not found";
                                break;
                            }
                        default:
                            {
                                lu.Add(new User(int.Parse(sdr["User_Id"].ToString()), sdr["User_Name"].ToString(), sdr["User_Password"].ToString(), sdr["First_Name"].ToString(), sdr["Last_Name"].ToString(), int.Parse(sdr["Home_Id"].ToString()), sdr["User_Type_Name"].ToString(), sdr["Token"].ToString()));
                                resMes = "Data";
                                break;
                            }
                    }
                }

                jd = new JsonData(lu, resMes);

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
            string resMes = "No Data";
            int rId = 0;

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
                    rId = int.Parse(sdr["Room_Id"].ToString());

                    switch(rId)
                    {
                        case -3:
                            {
                                resMes = "Error! You are not registered as a member of this home";
                                break;
                            }
                        case -2:
                            {
                                resMes = "Error! Home not found";
                                break;
                            }
                        case -1:
                            {
                                resMes = "Error! User not found";
                                break;
                            }
                        default:
                            {
                                lr.Add(new Room(int.Parse(sdr["Room_Id"].ToString()), sdr["Room_Name"].ToString(), sdr["Room_Type_Name"].ToString(), homeId, bool.Parse(sdr["Is_Shared"].ToString()), int.Parse(sdr["Number_Of_Devices"].ToString())));
                                resMes = "Data";
                                break;
                            }
                    }                   
                }

                jd = new JsonData(lr, resMes);

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
            string resMes = "No Data";
            int dId = 0;

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
                    dId = int.Parse(sdr["Device_Id"].ToString());

                    switch(dId)
                    {
                        case -3:
                            {
                                resMes = "Error! You are not registered as a member of this home";
                                break;
                            }
                        case -2:
                            {
                                resMes = "Error! Home not found";
                                break;
                            }
                        case -1:
                            {
                                resMes = "Error! User not found";
                                break;
                            }
                        default:
                            {
                                ld.Add(new Device(int.Parse(sdr["Device_Id"].ToString()), sdr["Device_Name"].ToString(), sdr["Device_Type_Name"].ToString(), homeId, bool.Parse(sdr["Is_Divided_Into_Rooms"].ToString()), int.Parse(sdr["Room_Id"].ToString())));
                                resMes = "Data";
                                break;
                            }
                    }                    
                }

                jd = new JsonData(ld, resMes);

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

        static public int ChangeDeviceStatus(int userId, int deviceId, int roomId, bool turnOn, int activationMethodCode, string statusDetails, string conditionId)
        {
            int res = -1;

            com = new SqlCommand("Change_Device_Status", con);
            com.CommandType = CommandType.StoredProcedure;

            com.Parameters.Clear();
            com.Parameters.Add(new SqlParameter("@UserId", userId));
            com.Parameters.Add(new SqlParameter("@DeviceId", deviceId));
            com.Parameters.Add(new SqlParameter("@RoomId", roomId));
            com.Parameters.Add(new SqlParameter("@TurnOn", turnOn));
            com.Parameters.Add(new SqlParameter("@ActivationMethodCode", activationMethodCode));

            if (statusDetails == "null")
            {
                com.Parameters.Add(new SqlParameter("@StatusDetails", DBNull.Value));
            }
            else
            {
                com.Parameters.Add(new SqlParameter("@StatusDetails", statusDetails));
            }

            if (conditionId == "null")
            { 
                com.Parameters.Add(new SqlParameter("@ConditionId", DBNull.Value));
            }
            else
            {
                com.Parameters.Add(new SqlParameter("@ConditionId", int.Parse(conditionId)));
            }

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
                  "ERROR in class:DBService function:ChangeDeviceStatus() - message=" + e.Message +
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
