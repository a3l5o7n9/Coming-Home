using BEL;
using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL
{
    static public class BLService
    {
        static public int Register(string userName, string userPassword, string firstName, string lastName)
        {
            return DBService.Register(userName, userPassword, firstName, lastName);
        }

        static public JsonData Login(string userName, string userPassword)
        {
            return DBService.Login(userName, userPassword);
        }

        static public int CreateHome(int userId, string homeName, string address)
        {
            return DBService.CreateHome(userId, homeName, address);
        }

        static public JsonData JoinHome(int userId, string homeName, string address)
        {
            return DBService.JoinHome(userId, homeName, address);
        }

        static public int CreateRoom(string roomName, int homeId, string roomTypeName, bool isShared, int userId)
        {
            return DBService.CreateRoom(roomName, homeId, roomTypeName, isShared, userId);
        }

        static public int CreateDevice(string deviceName, int homeId, string deviceTypeName, bool isDividedIntoRooms, int userId, int roomId)
        {
            return DBService.CreateDevice(deviceName, homeId, deviceTypeName, isDividedIntoRooms, userId, roomId);
        }

        static public int CreateActivationCondition(string conditionName, bool turnOn, int userId, int homeId, int deviceId, int roomId, string activationMethodName, string distanceOrTimeParam, string activationParam)
        {
            return DBService.CreateActivationCondition(conditionName, turnOn, userId, homeId, deviceId, roomId, activationMethodName, distanceOrTimeParam, activationParam);
        }

        public static void InitErrorLogPath(string v)
        {
            DBService.InitErrorLogPath(v);
        }

        static public int UpdateTokenForUserId(string token, int userId)
        {
            return DBService.UpdateTokenForUserId(token, userId);
        }

        static public int BindDeviceToRoom(int roomId, int deviceId, int userId)
        {
            return DBService.BindDeviceToRoom(roomId, deviceId, userId);
        }

        static public int UpdateUserTypeInHome(int appUserId, int userToUpdateId, int homeId, string updatedUserTypeName)
        {
            return DBService.UpdateUserTypeInHome(appUserId, userToUpdateId, homeId, updatedUserTypeName);
        }

        static public int UpdateUserDevicePermissions(int appUserId, int userToUpdateId, int homeId, int deviceId, int roomId, bool hasPermission)
        {
            return DBService.UpdateUserDevicePermissions(appUserId, userToUpdateId, homeId, deviceId, roomId, hasPermission);
        }

        static public int UpdateUserRoomPermissions(int appUserId, int userToUpdateId, int homeId, int roomId, bool hasAccess)
        {
            return DBService.UpdateUserRoomPermissions(appUserId, userToUpdateId, homeId, roomId, hasAccess);
        }

        static public JsonData GetUser(int userId, int homeId)
        {
            return DBService.GetUser(userId, homeId);
        }

        static public JsonData GetHome(int userId, int homeId)
        {
            return DBService.GetHome(userId, homeId);
        }

        static public JsonData GetDevice(int userId, int homeId, int deviceId, int roomId)
        {
            return DBService.GetDevice(userId, homeId, deviceId, roomId);
        }

        static public JsonData GetRoom(int userId, int homeId, int roomId)
        {
            return DBService.GetRoom(userId, homeId, roomId);
        }

        static public JsonData GetActivationCondition(int conditionId, int userId, int homeId, int deviceId, int roomId)
        {
            return DBService.GetActivationCondition(conditionId, userId, homeId, deviceId, roomId);
        }

        static public JsonData GetActivationConditionDetails(ActivationCondition actCon)
        {
            return DBService.GetActivationConditionDetails(actCon);
        }

        static public JsonData GetUsersInHome(int userId, int homeId)
        {
            return DBService.GetUsersInHome(userId, homeId);
        }

        static public JsonData GetUserRoomsInHome(int userId, int homeId)
        {
            return DBService.GetUserRoomsInHome(userId, homeId);
        }

        static public JsonData GetUserDevicesInHome(int userId, int homeId)
        {
            return DBService.GetUserDevicesInHome(userId, homeId);
        }

        static public JsonData GetUserActivationConditionsInHome(int userId, int homeId)
        {
            return DBService.GetUserActivationConditionsInHome(userId, homeId);
        }

        static public JsonData GetUserHomeDetails(int userId, int homeId)
        {
            return DBService.GetUserHomeDetails(userId, homeId);
        }

        static public List<ActivationCondition> GetAllActivationConditions()
        {
            return DBService.GetAllActivationConditions();
        }

        static public int CheckActivationCondition(ActivationCondition actCon)
        {
            int res = -1;

            if (actCon.ActivationMethodName == "מתוזמנת")
            {
                if (actCon.DistanceOrTimeParam == DateTime.Now.ToShortTimeString())
                {
                    res = ChangeDeviceStatus(actCon.CreatedByUserId, actCon.DeviceId, actCon.RoomId, actCon.TurnOn, 2, actCon.ActivationParam.ToString(), actCon.ConditionId.ToString());
                }
            }

            return res;
        }

        static public int ChangeDeviceStatus(int userId, int deviceId, int roomId, bool turnOn, int activationMethodCode, string statusDetails, string conditionId)
        {
            return DBService.ChangeDeviceStatus(userId, deviceId, roomId, turnOn, activationMethodCode, statusDetails, conditionId);
        }

        static public int ChangeConditionStatus(int userId, int homeId, int deviceId, int roomId, int conditionId, bool newStatus)
        {
            return DBService.ChangeConditionStatus(userId, homeId, deviceId, roomId, conditionId, newStatus);
        }

        static public List<UserType> GetUserTypes()
        {
            return DBService.GetUserTypes();
        }

        static public List<RoomType> GetRoomTypes()
        {
            return DBService.GetRoomTypes();
        }

        static public List<DeviceType> GetDeviceTypes()
        {
            return DBService.GetDeviceTypes();
        }

        static public List<ActivationMethod> GetActivationMethods()
        {
            return DBService.GetActivationMethods();
        }

        static public int DeleteUser(int userId)
        {
            return DBService.DeleteUser(userId);
        }

        static public int DeleteHome(int userId, int homeId)
        {
            return DBService.DeleteHome(userId, homeId);
        }

        static public int DeleteRoom(int userId, int homeId, int roomId)
        {
            return DBService.DeleteRoom(userId, homeId, roomId);
        }

        static public int DeleteDevice(int userId, int homeId, int deviceId)
        {
            return DBService.DeleteDevice(userId, homeId, deviceId);
        }

        static public int DeleteActivationCondition(int userId, int homeId, int conditionId)
        {
            return DBService.DeleteActivationCondition(userId, homeId, conditionId);
        }
    }
}
