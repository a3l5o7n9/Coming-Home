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
            //Try to create a new user in the DB.
            int userId = -1;
            //This procedure will first check if such a user exists. If not, it will add it to the DB
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
            //Get From the DB the user and homes they belong to, if a user with these userName and password exists
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
                    lh.Add(new Home(int.Parse(sdr["Home_Id"].ToString()), sdr["Home_Name"].ToString(), int.Parse(sdr["Number_Of_Users"].ToString()), sdr["Address"].ToString(), double.Parse(sdr["Latitude"].ToString()), double.Parse(sdr["Longitude"].ToString()), double.Parse(sdr["Altitude"].ToString()), double.Parse(sdr["Accuracy"].ToString())));
                }

                jd = new JsonData(au, lu, lh, "Data");
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

                if (jd.AU != null)
                {
                    List<Room> lr = new List<Room>();
                    lr = GetAllUserRooms(jd.AU.UserId);

                    jd.LR = lr;

                    List<Device> ld = new List<Device>();
                    ld = GetAllUserDevices(jd.AU.UserId);

                    jd.LD = ld;

                    List<ActivationCondition> lActCon = new List<ActivationCondition>();
                    lActCon = GetAllUserActivationConditions(jd.AU.UserId);

                    jd.LActCon = lActCon;
                }
            }

            return jd;
        }

        static public int CreateHome(int userId, string homeName, string address, double latitude, double longitude, double altitude, double accuracy)
        {
            int homeId = -1;
            //This procedure will check whether a home with those name and address already exists. If not, the home will be added to the DB
            com = new SqlCommand("New_Home", con);
            com.CommandType = CommandType.StoredProcedure;

            com.Parameters.Clear();
            com.Parameters.Add(new SqlParameter("@UserId", userId));
            com.Parameters.Add(new SqlParameter("@HomeName", homeName));
            com.Parameters.Add(new SqlParameter("@Address", address));
            com.Parameters.Add(new SqlParameter("@Latitude", latitude));
            com.Parameters.Add(new SqlParameter("@Longitude", longitude));
            com.Parameters.Add(new SqlParameter("@Altitude", altitude));
            com.Parameters.Add(new SqlParameter("@Accuracy", accuracy));

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

        static public JsonData JoinHome(int userId, string homeName, string address, double latitude, double longitude, double altitude, double accuracy)
        {
            string resMes = "No Data";
            int homeId = -1;
            JsonData jd = null;
            Home h = null;
            //This procedure will search for a home matching the parameters. If found, the user will be registered as a member of that home
            //and will be added to the tables managing permissions in this home
            com = new SqlCommand("Join_Existing_Home", con);
            com.CommandType = CommandType.StoredProcedure;

            com.Parameters.Clear();
            com.Parameters.Add(new SqlParameter("@UserId", userId));
            com.Parameters.Add(new SqlParameter("@HomeName", homeName));
            com.Parameters.Add(new SqlParameter("@Address", address));
            com.Parameters.Add(new SqlParameter("@Latitude", latitude));
            com.Parameters.Add(new SqlParameter("@Altitude", altitude));
            com.Parameters.Add(new SqlParameter("@Accuracy", accuracy));

            try
            {
                com.Connection.Open();
                sdr = com.ExecuteReader();

                if (sdr.Read())
                {
                    homeId = int.Parse(sdr["Home_Id"].ToString());

                    switch (homeId)
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
                                h = new Home(homeId, homeName, int.Parse(sdr["Number_Of_Users"].ToString()), address, latitude, longitude, altitude, accuracy);
                                break;
                            }
                    }
                }

                //Returns JsonData with a Home object
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
            //Tries to add a room to the DB, if such a room dose not exist yet
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
            //Tries to add a device to the DB, if such a device does not exist yet
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

        static public int CreateActivationCondition(string conditionName, bool turnOn, int userId, int homeId, int deviceId, int roomId, string activationMethodName, string distanceOrTimeParam, string activationParam)
        {
            //Tries to add an activation condition to the DB, if such a condition does not exist yet
            int conditionId = -1;
            com = new SqlCommand("New_Activation_Condition", con);
            com.CommandType = CommandType.StoredProcedure;

            com.Parameters.Clear();
            com.Parameters.Add(new SqlParameter("@ConditionName", conditionName));
            com.Parameters.Add(new SqlParameter("@TurnOn", turnOn));
            com.Parameters.Add(new SqlParameter("@UserId", userId));
            com.Parameters.Add(new SqlParameter("@HomeId", homeId));
            com.Parameters.Add(new SqlParameter("@DeviceId", deviceId));
            com.Parameters.Add(new SqlParameter("@RoomId", roomId));
            com.Parameters.Add(new SqlParameter("@ActivationMethodName", activationMethodName));

            if (distanceOrTimeParam == "null")
            {
                com.Parameters.Add(new SqlParameter("@DistanceOrTimeParam", DBNull.Value));
            }
            else
            {
                com.Parameters.Add(new SqlParameter("@DistanceOrTimeParam", distanceOrTimeParam));
            }

            if (activationParam == "null")
            {
                com.Parameters.Add(new SqlParameter("@ActivationParam", DBNull.Value));
            }
            else
            {
                com.Parameters.Add(new SqlParameter("@ActivationParam", activationParam));
            }

            try
            {
                com.Connection.Open();
                sdr = com.ExecuteReader();

                if (sdr.Read())
                {
                    conditionId = int.Parse(sdr[0].ToString());
                }

                return conditionId;
            }
            catch (Exception e)
            {
                File.AppendAllText(Globals.LogFileName,
                   "ERROR in class:DBService function:CreateActivationCondition() - message=" + e.Message +
                   ", on the " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + Environment.NewLine);
            }
            finally
            {
                if (com.Connection.State == ConnectionState.Open)
                {
                    com.Connection.Close();
                }
            }

            return conditionId;
        }

        static public int UpdateTokenForUserId(string token, int userId)
        {
            //Updates the user's token in the database in order to send push notifications
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
            //Tries to associate between a room and a device, unless they are already linked to each other
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

        static public string UpdateUserDetails(int appUserId, int userToUpdateId, string newUserName, string newUserPassword, string newFirstName, string newLastName)
        {
            string res = "Update Failed";
            //Tries to update the user's log in the DB, only if the appUserId is equal to userToUpdateId
            com = new SqlCommand("Update_User_Details", con);
            com.CommandType = CommandType.StoredProcedure;

            com.Parameters.Clear();
            com.Parameters.Add(new SqlParameter("@AppUserId", appUserId));
            com.Parameters.Add(new SqlParameter("@UserToUpdateId", userToUpdateId));

            if (newUserName == "null")
            {
                com.Parameters.Add(new SqlParameter("@NewUserName", DBNull.Value));
            }
            else
            {
                com.Parameters.Add(new SqlParameter("@NewUserName", newUserName));
            }

            if (newUserPassword == "null")
            {
                com.Parameters.Add(new SqlParameter("@NewUserPassword", DBNull.Value));
            }
            else
            {
                com.Parameters.Add(new SqlParameter("@NewUserPassword", newUserPassword));
            }

            if (newFirstName == "null")
            {
                com.Parameters.Add(new SqlParameter("@NewFirstName", DBNull.Value));
            }
            else
            {
                com.Parameters.Add(new SqlParameter("@NewFirstName", newFirstName));
            }

            if (newLastName == "null")
            {
                com.Parameters.Add(new SqlParameter("@NewLastName", DBNull.Value));
            }
            else
            {
                com.Parameters.Add(new SqlParameter("@NewLastName", newLastName));
            }

            try
            {
                com.Connection.Open();
                sdr = com.ExecuteReader();

                if (sdr.Read())
                {
                    res = sdr[0].ToString();
                }

                return res;
            }
            catch (Exception e)
            {
                File.AppendAllText(Globals.LogFileName,
                   "ERROR in class:DBService function:UpdateUserDetails() - message=" + e.Message +
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

        static public string UpdateHomeDetails(int appUserId, int homeId, string newHomeName, string newAddress, string newLatitude, string newLongitude, string newAltitude, string newAccuracy)
        {
            string res = "Update Failed";

            //Tries to update the home's log in the DB, only if the user is registered in it and is a main user there
            com = new SqlCommand("Update_Home_Details", con);
            com.CommandType = CommandType.StoredProcedure;

            com.Parameters.Clear();
            com.Parameters.Add(new SqlParameter("@AppUserId", appUserId));
            com.Parameters.Add(new SqlParameter("@HomeId", homeId));

            if (newHomeName == "null")
            {
                com.Parameters.Add(new SqlParameter("@NewHomeName", DBNull.Value));
            }
            else
            {
                com.Parameters.Add(new SqlParameter("@NewHomeName", newHomeName));
            }

            if (newAddress == "null")
            {
                com.Parameters.Add(new SqlParameter("@NewAddress", DBNull.Value));
            }
            else
            {
                com.Parameters.Add(new SqlParameter("@NewAddress", newAddress));
            }

            if (newLatitude == "null")
            {
                com.Parameters.Add(new SqlParameter("@NewLatitude", DBNull.Value));
            }
            else
            {
                com.Parameters.Add(new SqlParameter("@NewLatitude", double.Parse(newLatitude)));
            }

            if (newLongitude == "null")
            {
                com.Parameters.Add(new SqlParameter("@NewLongitude", DBNull.Value));
            }
            else
            {
                com.Parameters.Add(new SqlParameter("@NewLongitude", double.Parse(newLongitude)));
            }

            if (newAltitude == "null")
            {
                com.Parameters.Add(new SqlParameter("@NewAltitude", DBNull.Value));
            }
            else
            {
                com.Parameters.Add(new SqlParameter("@NewAltitude", double.Parse(newAltitude)));
            }

            if (newAccuracy == "null")
            {
                com.Parameters.Add(new SqlParameter("@NewAccuracy", DBNull.Value));
            }
            else
            {
                com.Parameters.Add(new SqlParameter("@NewAcuracy", double.Parse(newAccuracy)));
            }

            try
            {
                com.Connection.Open();
                sdr = com.ExecuteReader();

                if (sdr.Read())
                {
                    res = sdr[0].ToString();
                }

                return res;
            }
            catch (Exception e)
            {
                File.AppendAllText(Globals.LogFileName,
                   "ERROR in class:DBService function:UpdateHomeDetails() - message=" + e.Message +
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

        static public string UpdateRoomDetails(int appUserId, int homeId, int roomId, string newRoomName, string newRoomTypeCode, string newShareStatus)
        {
            string res = "Update Failed";

            //Tries to update the room's log in the DB, only if the user belongs to that home, has access to the room and is a main user there
            com = new SqlCommand("Update_Room_Details", con);
            com.CommandType = CommandType.StoredProcedure;

            com.Parameters.Clear();
            com.Parameters.Add(new SqlParameter("@AppUserId", appUserId));
            com.Parameters.Add(new SqlParameter("@HomeId", homeId));
            com.Parameters.Add(new SqlParameter("@RoomId", roomId));

            if (newRoomName == "null")
            {
                com.Parameters.Add(new SqlParameter("@NewRoomName", DBNull.Value));
            }
            else
            {
                com.Parameters.Add(new SqlParameter("@NewRoomName", newRoomName));
            }

            if (newRoomTypeCode == "null")
            {
                com.Parameters.Add(new SqlParameter("@NewRoomTypeCode", DBNull.Value));
            }
            else
            {
                com.Parameters.Add(new SqlParameter("@NewRoomTypeCode", int.Parse(newRoomTypeCode)));
            }

            if (newShareStatus == "null")
            {
                com.Parameters.Add(new SqlParameter("@NewShareStatus", DBNull.Value));
            }
            else
            {
                com.Parameters.Add(new SqlParameter("@NewShareStatus", bool.Parse(newShareStatus)));
            }

            try
            {
                com.Connection.Open();
                sdr = com.ExecuteReader();

                if (sdr.Read())
                {
                    res = sdr[0].ToString();
                }

                return res;
            }
            catch (Exception e)
            {
                File.AppendAllText(Globals.LogFileName,
                   "ERROR in class:DBService function:UpdateRoomDetails() - message=" + e.Message +
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

        static public string UpdateDeviceDetails(int appUserId, int homeId, int deviceId, string newDeviceName, string newDeviceTypeCode, string newDivideStatus)
        {
            string res = "Update Failed";

            //Tries to update the device's log in the DB, only if the user belongs to that home, has permission to modify the device and is a main user there
            com = new SqlCommand("Update_Device_Details", con);
            com.CommandType = CommandType.StoredProcedure;

            com.Parameters.Clear();
            com.Parameters.Add(new SqlParameter("@AppUserId", appUserId));
            com.Parameters.Add(new SqlParameter("@HomeId", homeId));
            com.Parameters.Add(new SqlParameter("@DeviceId", deviceId));

            if (newDeviceName == "null")
            {
                com.Parameters.Add(new SqlParameter("@NewDeviceName", DBNull.Value));
            }
            else
            {
                com.Parameters.Add(new SqlParameter("@NewDeviceName", newDeviceName));
            }

            if (newDeviceTypeCode == "null")
            {
                com.Parameters.Add(new SqlParameter("@NewDeviceTypeCode", DBNull.Value));
            }
            else
            {
                com.Parameters.Add(new SqlParameter("@NewDeviceTypeCode", int.Parse(newDeviceTypeCode)));
            }

            if (newDivideStatus == "null")
            {
                com.Parameters.Add(new SqlParameter("@NewDivideStatus", DBNull.Value));
            }
            else
            {
                com.Parameters.Add(new SqlParameter("@NewDivideStatus", bool.Parse(newDivideStatus)));
            }

            try
            {
                com.Connection.Open();
                sdr = com.ExecuteReader();

                if (sdr.Read())
                {
                    res = sdr[0].ToString();
                }

                return res;
            }
            catch (Exception e)
            {
                File.AppendAllText(Globals.LogFileName,
                   "ERROR in class:DBService function:UpdateDeviceDetails() - message=" + e.Message +
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

        static public string UpdateActivationConditionDetails(int appUserId, int homeId, int conditionId, string newDeviceId, string newRoomId, string newConditionName, string newStatus, string newActivationMethodCode, string newDistanceOrTimeParam, string newActivationParam)
        {
            string res = "Update Failed";

            //Tries to update the condition's log in the DB, only if the user is a member of that home,
            //has permission to modify the target device in the target room, and was the same user to create
            // the condition in the first place
            com = new SqlCommand("Update_Activation_Condition_Details", con);
            com.CommandType = CommandType.StoredProcedure;

            com.Parameters.Clear();
            com.Parameters.Add(new SqlParameter("@AppUserId", appUserId));
            com.Parameters.Add(new SqlParameter("@HomeId", homeId));
            com.Parameters.Add(new SqlParameter("@ConditionId", conditionId));

            if (newDeviceId == "null")
            {
                com.Parameters.Add(new SqlParameter("@NewDeviceId", DBNull.Value));
            }
            else
            {
                com.Parameters.Add(new SqlParameter("@NewDeviceId", int.Parse(newDeviceId)));
            }

            if (newRoomId == "null")
            {
                com.Parameters.Add(new SqlParameter("@NewRoomId", DBNull.Value));
            }
            else
            {
                com.Parameters.Add(new SqlParameter("@NewRoomId", int.Parse(newRoomId)));
            }

            if (newConditionName == "null")
            {
                com.Parameters.Add(new SqlParameter("@NewConditionName", DBNull.Value));
            }
            else
            {
                com.Parameters.Add(new SqlParameter("@NewConditionName", newConditionName));
            }

            if (newStatus == "null")
            {
                com.Parameters.Add(new SqlParameter("@NewStatus", DBNull.Value));
            }
            else
            {
                com.Parameters.Add(new SqlParameter("@NewStatus", bool.Parse(newStatus)));
            }

            if (newActivationMethodCode == "null")
            {
                com.Parameters.Add(new SqlParameter("@NewActivationMethodCode", DBNull.Value));
            }
            else
            {
                com.Parameters.Add(new SqlParameter("@NewActivationMethodCode", int.Parse(newActivationMethodCode)));
            }

            if (newDistanceOrTimeParam == "null")
            {
                com.Parameters.Add(new SqlParameter("@NewDistanceOrTimeParam", DBNull.Value));
            }
            else
            {
                com.Parameters.Add(new SqlParameter("@NewDistanceOrTimeParam", newDistanceOrTimeParam));
            }

            if (newActivationParam == "null")
            {
                com.Parameters.Add(new SqlParameter("@NewActivationParam", DBNull.Value));
            }
            else
            {
                com.Parameters.Add(new SqlParameter("@NewActivationParam", newActivationParam));
            }

            try
            {
                com.Connection.Open();
                sdr = com.ExecuteReader();

                if (sdr.Read())
                {
                    res = sdr[0].ToString();
                }

                return res;
            }
            catch (Exception e)
            {
                File.AppendAllText(Globals.LogFileName,
                   "ERROR in class:DBService function:UpdateActivationConditionDetails() - message=" + e.Message +
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

        static public int UpdateUserTypeInHome(int appUserId, int userToUpdateId, int homeId, string updatedUserTypeName)
        {
            int res = -1;

            //Checks whether the user has the authority to change the target user's type,
            //and verifies that the home will always remain with at least 1 MainUser
            com = new SqlCommand("Update_User_Type_In_Home", con);
            com.CommandType = CommandType.StoredProcedure;

            com.Parameters.Clear();
            com.Parameters.Add(new SqlParameter("@AppUserId", appUserId));
            com.Parameters.Add(new SqlParameter("@UserIdToUpdate", userToUpdateId));
            com.Parameters.Add(new SqlParameter("@HomeId", homeId));
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

        static public int UpdateUserDevicePermissions(int appUserId, int userToUpdateId, int homeId, int deviceId, int roomId, bool hasPermission)
        {
            int res = -1;

            //Checks if the user has the authority to change the target user's permissions
            //before trying to change the requested permission
            com = new SqlCommand("Update_User_Device_Permissions", con);
            com.CommandType = CommandType.StoredProcedure;

            com.Parameters.Clear();
            com.Parameters.Add(new SqlParameter("@AppUserId", appUserId));
            com.Parameters.Add(new SqlParameter("@UserIdToUpdate", userToUpdateId));
            com.Parameters.Add(new SqlParameter("@HomeId", homeId));
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

        static public int UpdateUserRoomPermissions(int appUserId, int userToUpdateId, int homeId, int roomId, bool hasAccess)
        {
            int res = -1;

            //Checks if the user has the authority to change the target user's permissions
            //before trying to change the requested permission
            com = new SqlCommand("Update_User_Room_Permissions", con);
            com.CommandType = CommandType.StoredProcedure;

            com.Parameters.Clear();
            com.Parameters.Add(new SqlParameter("@AppUserId", appUserId));
            com.Parameters.Add(new SqlParameter("@UserIdToUpdate", userToUpdateId));
            com.Parameters.Add(new SqlParameter("@HomeId", homeId));
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

            //Retrieves the requested User from the DB, if suach a user exists
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

                if (sdr.Read())
                {
                    uId = int.Parse(sdr["User_Id"].ToString());

                    switch (uId)
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

            //Retrieves the requested Home from the DB, if such a home exists 
            //and if the uesr is associated with it
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

                    switch (hId)
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
                                h = new Home(homeId, sdr["Home_Name"].ToString(), int.Parse(sdr["Number_Of_Users"].ToString()), sdr["Address"].ToString(), double.Parse(sdr["Latitude"].ToString()), double.Parse(sdr["Longitude"].ToString()), double.Parse(sdr["Altitude"].ToString()), double.Parse(sdr["Accuracy"].ToString()));
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

            //Retrieves the requested Device from the DB, if the user has permission to it
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
                    switch (dId)
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
                                d = new Device(deviceId, sdr["Device_Name"].ToString(), sdr["Device_Type_Name"].ToString(), homeId, bool.Parse(sdr["Is_Divided_Into_Rooms"].ToString()), roomId, bool.Parse(sdr["Is_On"].ToString()));
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

            //Retrieves the requested Room, if the user has access to it
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
                    switch (rId)
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

        static public JsonData GetActivationCondition(int conditionId, int userId, int homeId, int deviceId, int roomId)
        {
            JsonData jd = null;
            string resMes = "No Data";
            int actConId = 0;

            //Retrieves the requested Condition from the DB, if the user has permission
            //to the device and access to the room
            com = new SqlCommand("Get_Activation_Condition", con);
            com.CommandType = CommandType.StoredProcedure;

            com.Parameters.Clear();
            com.Parameters.Add(new SqlParameter("@ConditionId", conditionId));
            com.Parameters.Add(new SqlParameter("@UserId", userId));
            com.Parameters.Add(new SqlParameter("@HomeId", homeId));
            com.Parameters.Add(new SqlParameter("@DeviceId", deviceId));
            com.Parameters.Add(new SqlParameter("@RoomId", roomId));

            try
            {
                com.Connection.Open();
                sdr = com.ExecuteReader();
                ActivationCondition actCon = null;

                if (sdr.Read())
                {
                    actConId = int.Parse(sdr["Condition_Id"].ToString());
                    switch (actConId)
                    {
                        case -1:
                            {
                                resMes = "Error! Condition not found!";
                                break;
                            }
                        default:
                            {
                                actCon = new ActivationCondition(int.Parse(sdr["Condition_Id"].ToString()), sdr["Condition_Name"].ToString(), bool.Parse(sdr["Turn_On"].ToString()), int.Parse(sdr["Created_By_User_Id"].ToString()), int.Parse(sdr["Home_Id"].ToString()), int.Parse(sdr["Device_Id"].ToString()), int.Parse(sdr["Room_Id"].ToString()), sdr["Activation_Method_Name"].ToString(), bool.Parse(sdr["Is_Active"].ToString()));

                                if (sdr["Distance_Or_Time_Param"].ToString() != "")
                                {
                                    actCon.DistanceOrTimeParam = sdr["Distance_Or_Time_Param"].ToString();
                                }

                                if (sdr["Activation_Param"].ToString() != "")
                                {
                                    actCon.ActivationParam = sdr["Activation_Param"].ToString();
                                }

                                resMes = "Data";
                                break;
                            }
                    }
                }

                jd = new JsonData(actCon, resMes);

            }
            catch (Exception e)
            {
                File.AppendAllText(Globals.LogFileName,
                 "ERROR in class:DBService function:GetActivationCondition() - message=" + e.Message +
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

        static public JsonData GetActivationConditionDetails(ActivationCondition actCon)
        {
            JsonData jd = new JsonData(actCon, "Data");

            JsonData temp = GetUser(actCon.CreatedByUserId, actCon.HomeId);

            jd.U = temp.U;
            jd.ResultMessage = temp.ResultMessage;

            temp = GetHome(actCon.CreatedByUserId, actCon.HomeId);

            jd.H = temp.H;
            jd.ResultMessage = temp.ResultMessage;

            temp = GetRoom(actCon.CreatedByUserId, actCon.HomeId, actCon.RoomId);

            jd.R = temp.R;
            jd.ResultMessage = temp.ResultMessage;

            temp = GetDevice(actCon.CreatedByUserId, actCon.HomeId, actCon.DeviceId, actCon.RoomId);

            jd.D = temp.D;
            jd.ResultMessage = temp.ResultMessage;

            return jd;
        }

        static public JsonData GetUsersInHome(int userId, int homeId)
        {
            JsonData jd = null;
            string resMes = "No Data";

            //Retrieves all the users in that home, if the user belongs to it
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

                    switch (uId)
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

            //Retrieves all the rooms in that home the user has access to
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

                    switch (rId)
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
                        case 0:
                            {
                                resMes = "No Data";
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

            //Retrieves all the devices in that home in the rooms that the user has permission 
            //and access to
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

                    switch (dId)
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
                        case 0:
                            {
                                resMes = "No Data";
                                break;
                            }
                        default:
                            {
                                ld.Add(new Device(int.Parse(sdr["Device_Id"].ToString()), sdr["Device_Name"].ToString(), sdr["Device_Type_Name"].ToString(), homeId, bool.Parse(sdr["Is_Divided_Into_Rooms"].ToString()), int.Parse(sdr["Room_Id"].ToString()), bool.Parse(sdr["Is_On"].ToString())));
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

        static public JsonData GetUserActivationConditionsInHome(int userId, int homeId)
        {
            JsonData jd = null;
            string resMes = "No Data";
            int actConId = 0;

            com = new SqlCommand("Get_User_Activation_Conditions_In_Home", con);
            com.CommandType = CommandType.StoredProcedure;

            com.Parameters.Clear();
            com.Parameters.Add(new SqlParameter("@UserId", userId));
            com.Parameters.Add(new SqlParameter("@HomeId", homeId));

            try
            {
                com.Connection.Open();
                sdr = com.ExecuteReader();
                List<ActivationCondition> lActCon = new List<ActivationCondition>();

                while (sdr.Read())
                {
                    actConId = int.Parse(sdr["Condition_Id"].ToString());

                    switch (actConId)
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
                        case 0:
                            {
                                resMes = "No Data";
                                break;
                            }
                        default:
                            {
                                ActivationCondition actCon = new ActivationCondition(int.Parse(sdr["Condition_Id"].ToString()), sdr["Condition_Name"].ToString(), bool.Parse(sdr["Turn_On"].ToString()), int.Parse(sdr["Created_By_User_Id"].ToString()), int.Parse(sdr["Home_Id"].ToString()), int.Parse(sdr["Device_Id"].ToString()), int.Parse(sdr["Room_Id"].ToString()), sdr["Activation_Method_Name"].ToString(), bool.Parse(sdr["Is_Active"].ToString()));

                                if (sdr["Distance_Or_Time_Param"].ToString() != "")
                                {
                                    actCon.DistanceOrTimeParam = sdr["Distance_Or_Time_Param"].ToString();
                                }

                                if (sdr["Activation_Param"].ToString() != "")
                                {
                                    actCon.ActivationParam = sdr["Activation_Param"].ToString();
                                }

                                lActCon.Add(actCon);
                                resMes = "Data";
                                break;
                            }
                    }
                }

                jd = new JsonData(lActCon, resMes);

            }
            catch (Exception e)
            {
                File.AppendAllText(Globals.LogFileName,
                 "ERROR in class:DBService function:GetUserActivationConditionsInHome() - message=" + e.Message +
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

        static public List<ActivationCondition> GetAllActivationConditions()
        {
            List<ActivationCondition> lActCon = new List<ActivationCondition>();

            //Retrieves all activation conditions from the DB
            com = new SqlCommand("Get_All_Activation_Conditions", con);
            com.CommandType = CommandType.StoredProcedure;

            com.Parameters.Clear();

            try
            {
                com.Connection.Open();
                sdr = com.ExecuteReader();

                while (sdr.Read())
                {
                    ActivationCondition actCon = new ActivationCondition(int.Parse(sdr["Condition_Id"].ToString()), sdr["Condition_Name"].ToString(), bool.Parse(sdr["Turn_On"].ToString()), int.Parse(sdr["Created_By_User_Id"].ToString()), int.Parse(sdr["Home_Id"].ToString()), int.Parse(sdr["Device_Id"].ToString()), int.Parse(sdr["Room_Id"].ToString()), sdr["Activation_Method_Name"].ToString(), bool.Parse(sdr["Is_Active"].ToString()));

                    if (sdr["Distance_Or_Time_Param"].ToString() != "")
                    {
                        actCon.DistanceOrTimeParam = sdr["Distance_Or_Time_Param"].ToString();
                    }

                    if (sdr["Activation_Param"].ToString() != "")
                    {
                        actCon.ActivationParam = sdr["Activation_Param"].ToString();
                    }

                    lActCon.Add(actCon);
                }

                return lActCon;
            }
            catch (Exception e)
            {
                File.AppendAllText(Globals.LogFileName,
                 "ERROR in class:DBService function:GetAllActivationConditions() - message=" + e.Message +
                 ", on the " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + Environment.NewLine);
            }
            finally
            {
                if (com.Connection.State == ConnectionState.Open)
                {
                    com.Connection.Close();
                }
            }

            return lActCon;
        }

        static public List<Room> GetAllUserRooms(int userId)
        {
            List<Room> lr = new List<Room>();
            int rId = -1;

            //Retrieves all rooms this user has access to from the DB
            com = new SqlCommand("Get_All_User_Rooms", con);
            com.CommandType = CommandType.StoredProcedure;

            com.Parameters.Clear();
            com.Parameters.Add(new SqlParameter("@UserId", userId));

            try
            {
                com.Connection.Open();
                sdr = com.ExecuteReader();

                while (sdr.Read())
                {
                    rId = int.Parse(sdr["Room_Id"].ToString());


                    if (rId == 0)
                    {
                        return null;
                    }
                    else
                    {
                        lr.Add(new Room(int.Parse(sdr["Room_Id"].ToString()), sdr["Room_Name"].ToString(), sdr["Room_Type_Name"].ToString(), int.Parse(sdr["Home_Id"].ToString()), bool.Parse(sdr["Is_Shared"].ToString()), int.Parse(sdr["Number_Of_Devices"].ToString())));
                    }
                }
            }
            catch (Exception e)
            {
                File.AppendAllText(Globals.LogFileName,
                 "ERROR in class:DBService function:GetAllUserRooms() - message=" + e.Message +
                 ", on the " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + Environment.NewLine);
            }
            finally
            {
                if (com.Connection.State == ConnectionState.Open)
                {
                    com.Connection.Close();
                }
            }

            return lr;
        }

        static public List<Device> GetAllUserDevices(int userId)
        {
            List<Device> ld = new List<Device>();
            int dId = -1;

            //Retrieves all the devices with the rooms in which this user has permissions to 
            //them from the DB
            com = new SqlCommand("Get_All_User_Devices", con);
            com.CommandType = CommandType.StoredProcedure;

            com.Parameters.Clear();
            com.Parameters.Add(new SqlParameter("@UserId", userId));

            try
            {
                com.Connection.Open();
                sdr = com.ExecuteReader();

                while (sdr.Read())
                {
                    dId = int.Parse(sdr["Device_Id"].ToString());

                    if (dId == 0)
                    {
                        return null;
                    }
                    else
                    {
                        ld.Add(new Device(int.Parse(sdr["Device_Id"].ToString()), sdr["Device_Name"].ToString(), sdr["Device_Type_Name"].ToString(), int.Parse(sdr["Home_Id"].ToString()), bool.Parse(sdr["Is_Divided_Into_Rooms"].ToString()), int.Parse(sdr["Room_Id"].ToString()), bool.Parse(sdr["Is_On"].ToString())));
                    }
                }
            }
            catch (Exception e)
            {
                File.AppendAllText(Globals.LogFileName,
                 "ERROR in class:DBService function:GetAllUserDevices() - message=" + e.Message +
                 ", on the " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + Environment.NewLine);
            }
            finally
            {
                if (com.Connection.State == ConnectionState.Open)
                {
                    com.Connection.Close();
                }
            }

            return ld;
        }

        static public List<ActivationCondition> GetAllUserActivationConditions(int userId)
        {
            List<ActivationCondition> lActCon = new List<ActivationCondition>();

            //Retrieves all activation conditions created by that user from the DB
            com = new SqlCommand("Get_All_User_Activation_Conditions", con);
            com.CommandType = CommandType.StoredProcedure;

            com.Parameters.Clear();
            com.Parameters.Add(new SqlParameter("@UserId", userId));

            try
            {
                com.Connection.Open();
                sdr = com.ExecuteReader();

                while (sdr.Read())
                {
                    ActivationCondition actCon = new ActivationCondition(int.Parse(sdr["Condition_Id"].ToString()), sdr["Condition_Name"].ToString(), bool.Parse(sdr["Turn_On"].ToString()), int.Parse(sdr["Created_By_User_Id"].ToString()), int.Parse(sdr["Home_Id"].ToString()), int.Parse(sdr["Device_Id"].ToString()), int.Parse(sdr["Room_Id"].ToString()), sdr["Activation_Method_Name"].ToString(), bool.Parse(sdr["Is_Active"].ToString()));

                    if (sdr["Distance_Or_Time_Param"].ToString() != "")
                    {
                        actCon.DistanceOrTimeParam = sdr["Distance_Or_Time_Param"].ToString();
                    }

                    if (sdr["Activation_Param"].ToString() != "")
                    {
                        actCon.ActivationParam = sdr["Activation_Param"].ToString();
                    }

                    lActCon.Add(actCon);
                }

                return lActCon;
            }
            catch (Exception e)
            {
                File.AppendAllText(Globals.LogFileName,
                 "ERROR in class:DBService function:GetAllUserActivationConditions() - message=" + e.Message +
                 ", on the " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + Environment.NewLine);
            }
            finally
            {
                if (com.Connection.State == ConnectionState.Open)
                {
                    com.Connection.Close();
                }
            }

            return lActCon;
        }

        static public JsonData GetUserHomeDetails(int userId, int homeId)
        {
            //Returns a JsonData object consisting of the values returned by
            //GetUsersInHome, GetUserRoomsInHome, GetUserDevicesInHome and
            //GetUserActivationConditionsInHome
            JsonData jd = null;
            string resMes = "No Data";
            List<User> lu = null;
            List<Room> lr = null;
            List<Device> ld = null;
            List<ActivationCondition> lActCon = null;

            JsonData usersData = GetUsersInHome(userId, homeId);

            if (usersData.ResultMessage == "Data")
            {
                lu = usersData.LU;
                resMes = "Data";
            }

            JsonData roomsData = GetUserRoomsInHome(userId, homeId);

            if (roomsData.ResultMessage == "Data")
            {
                lr = roomsData.LR;
                resMes = "Data";
            }

            JsonData devicesData = GetUserDevicesInHome(userId, homeId);

            if (devicesData.ResultMessage == "Data")
            {
                ld = devicesData.LD;
                resMes = "Data";
            }

            JsonData activationConditionsData = GetUserActivationConditionsInHome(userId, homeId);

            if (activationConditionsData.ResultMessage == "Data")
            {
                lActCon = activationConditionsData.LActCon;
                resMes = "Data";
            }

            jd = new JsonData(lu, lr, ld, lActCon, resMes);

            return jd;
        }

        static public int ChangeDeviceStatus(int userId, int deviceId, int roomId, bool turnOn, int activationMethodCode, string statusDetails, string conditionId)
        {
            int res = -1;

            //Tries to turn device On or Off according to turnOn
            //and updates Table DevicesStatusLog if successful
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

        static public int ChangeConditionStatus(int userId, int homeId, int deviceId, int roomId, int conditionId, bool newStatus)
        {
            int res = -1;

            //Tries to change whether the condition is active in accordance with newStatus
            com = new SqlCommand("Change_Condition_Status", con);
            com.CommandType = CommandType.StoredProcedure;

            com.Parameters.Clear();
            com.Parameters.Add(new SqlParameter("@UserId", userId));
            com.Parameters.Add(new SqlParameter("@HomeId", homeId));
            com.Parameters.Add(new SqlParameter("@DeviceId", deviceId));
            com.Parameters.Add(new SqlParameter("@RoomId", roomId));
            com.Parameters.Add(new SqlParameter("@ConditionId", conditionId));
            com.Parameters.Add(new SqlParameter("@NewStatus", newStatus));

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
                  "ERROR in class:DBService function:ChangeConditionStatus() - message=" + e.Message +
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

        static public List<UserType> GetUserTypes()
        {
            List<UserType> userTypes = new List<UserType>();

            /*Retrieve all rows from table "UserTypes" in DataBase*/
            com = new SqlCommand("Get_User_Types", con);
            com.CommandType = CommandType.StoredProcedure;

            com.Parameters.Clear();

            try
            {
                com.Connection.Open();
                sdr = com.ExecuteReader();


                while (sdr.Read())
                {
                    userTypes.Add(new UserType(int.Parse(sdr["User_Type_Code"].ToString()), sdr["User_Type_Name"].ToString()));
                }

                return userTypes;
            }
            catch (Exception e)
            {
                File.AppendAllText(Globals.LogFileName,
                   "ERROR in class:DBService function:GetUserTypes() - message=" + e.Message +
                   ", on the " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + Environment.NewLine);
            }
            finally
            {
                if (com.Connection.State == ConnectionState.Open)
                {
                    com.Connection.Close();
                }
            }

            return userTypes;
        }

        static public List<RoomType> GetRoomTypes()
        {
            List<RoomType> roomTypes = new List<RoomType>();

            /*Retrieve all rows from table "RoomTypes" in DataBase*/
            com = new SqlCommand("Get_Room_Types", con);
            com.CommandType = CommandType.StoredProcedure;

            com.Parameters.Clear();

            try
            {
                com.Connection.Open();
                sdr = com.ExecuteReader();


                while (sdr.Read())
                {
                    roomTypes.Add(new RoomType(int.Parse(sdr["Room_Type_Code"].ToString()), sdr["Room_Type_Name"].ToString()));
                }

                return roomTypes;
            }
            catch (Exception e)
            {
                File.AppendAllText(Globals.LogFileName,
                   "ERROR in class:DBService function:GetRoomTypes() - message=" + e.Message +
                   ", on the " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + Environment.NewLine);
            }
            finally
            {
                if (com.Connection.State == ConnectionState.Open)
                {
                    com.Connection.Close();
                }
            }

            return roomTypes;
        }

        static public List<DeviceType> GetDeviceTypes()
        {
            List<DeviceType> deviceTypes = new List<DeviceType>();

            /*Retrieve all rows from table "DeviceTypes" in DataBase*/
            com = new SqlCommand("Get_Device_Types", con);
            com.CommandType = CommandType.StoredProcedure;

            com.Parameters.Clear();

            try
            {
                com.Connection.Open();
                sdr = com.ExecuteReader();


                while (sdr.Read())
                {
                    deviceTypes.Add(new DeviceType(int.Parse(sdr["Device_Type_Code"].ToString()), sdr["Device_Type_Name"].ToString()));
                }

                return deviceTypes;
            }
            catch (Exception e)
            {
                File.AppendAllText(Globals.LogFileName,
                   "ERROR in class:DBService function:GetDeviceTypes() - message=" + e.Message +
                   ", on the " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + Environment.NewLine);
            }
            finally
            {
                if (com.Connection.State == ConnectionState.Open)
                {
                    com.Connection.Close();
                }
            }

            return deviceTypes;
        }

        static public List<ActivationMethod> GetActivationMethods()
        {
            List<ActivationMethod> activationMethods = new List<ActivationMethod>();

            /*Retrieve all rows from table "ActivationMethods" in DataBase*/
            com = new SqlCommand("Get_Activation_Methods", con);
            com.CommandType = CommandType.StoredProcedure;

            com.Parameters.Clear();

            try
            {
                com.Connection.Open();
                sdr = com.ExecuteReader();


                while (sdr.Read())
                {
                    activationMethods.Add(new ActivationMethod(int.Parse(sdr["Activation_Method_Code"].ToString()), sdr["Activation_Method_Name"].ToString()));
                }

                return activationMethods;
            }
            catch (Exception e)
            {
                File.AppendAllText(Globals.LogFileName,
                   "ERROR in class:DBService function:GetActivationMethods() - message=" + e.Message +
                   ", on the " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + Environment.NewLine);
            }
            finally
            {
                if (com.Connection.State == ConnectionState.Open)
                {
                    com.Connection.Close();
                }
            }

            return activationMethods;
        }

        static public int DeleteUser(int userId)
        {
            int res = -1;

            //Tries to delete the user and all related records in the DB
            com = new SqlCommand("Delete_User", con);
            com.CommandType = CommandType.StoredProcedure;

            com.Parameters.Clear();
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
                   "ERROR in class:DBService function:DeleteUser() - message=" + e.Message +
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

        static public int DeleteHome(int userId, int homeId)
        {
            int res = -1;

            //Tries to delete all records of that home in the DB 
            //only if the user is a main user in that home or a system administrator 
            com = new SqlCommand("Delete_Home", con);
            com.CommandType = CommandType.StoredProcedure;

            com.Parameters.Clear();
            com.Parameters.Add(new SqlParameter("@UserId", userId));
            com.Parameters.Add(new SqlParameter("@HomeId", homeId));

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
                   "ERROR in class:DBService function:DeleteHome() - message=" + e.Message +
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

        static public int DeleteRoom(int userId, int homeId, int roomId)
        {
            int res = -1;

            //Tries to delete all records of that room in the DB 
            //only if the user is a main user in that home or a system administrator 
            com = new SqlCommand("Delete_Room", con);
            com.CommandType = CommandType.StoredProcedure;

            com.Parameters.Clear();
            com.Parameters.Add(new SqlParameter("@UserId", userId));
            com.Parameters.Add(new SqlParameter("@HomeId", homeId));
            com.Parameters.Add(new SqlParameter("@RoomId", roomId));

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
                   "ERROR in class:DBService function:DeleteRoom() - message=" + e.Message +
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

        static public int DeleteDevice(int userId, int homeId, int deviceId)
        {
            int res = -1;

            //Tries to delete all records of that device in the DB 
            //only if the user is a main user in that home or a system administrator 
            com = new SqlCommand("Delete_Device", con);
            com.CommandType = CommandType.StoredProcedure;

            com.Parameters.Clear();
            com.Parameters.Add(new SqlParameter("@UserId", userId));
            com.Parameters.Add(new SqlParameter("@HomeId", homeId));
            com.Parameters.Add(new SqlParameter("@DeviceId", deviceId));

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
                   "ERROR in class:DBService function:DeleteDevice() - message=" + e.Message +
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

        static public int DeleteActivationCondition(int userId, int homeId, int activationConditionId)
        {
            int res = -1;

            //Tries to delete all records of that condition in the DB 
            //only if the user is a main user in that home or a system administrator 
            com = new SqlCommand("Delete_Activation_Condition", con);
            com.CommandType = CommandType.StoredProcedure;

            com.Parameters.Clear();
            com.Parameters.Add(new SqlParameter("@UserId", userId));
            com.Parameters.Add(new SqlParameter("@HomeId", homeId));
            com.Parameters.Add(new SqlParameter("@ActivationConditionId", activationConditionId));

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
                   "ERROR in class:DBService function:DeleteActivationCondition() - message=" + e.Message +
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

        static public int LeaveHome(int userId, int homeId)
        {
            int res = -1;

            com = new SqlCommand("Leave_Home", con);
            com.CommandType = CommandType.StoredProcedure;

            com.Parameters.Clear();
            com.Parameters.Add(new SqlParameter("@UserId", userId));
            com.Parameters.Add(new SqlParameter("@HomeId", homeId));

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
                   "ERROR in class:DBService function:LeaveHome() - message=" + e.Message +
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

        static public int UnbindDeviceFromRoom(int roomId, int deviceId, int userId, int homeId)
        {
            int res = -1;

            com = new SqlCommand("Unbind_Device_From_Room", con);
            com.CommandType = CommandType.StoredProcedure;

            com.Parameters.Clear();
            com.Parameters.Add(new SqlParameter("@RoomId", roomId));
            com.Parameters.Add(new SqlParameter("@DeviceId", deviceId));
            com.Parameters.Add(new SqlParameter("@UserId", userId));
            com.Parameters.Add(new SqlParameter("@HomeId", homeId));

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
                   "ERROR in class:DBService function:UnbindDeviceFromRoom() - message=" + e.Message +
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
