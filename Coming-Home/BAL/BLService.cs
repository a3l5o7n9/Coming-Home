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

        static public int JoinHome(int userId, string homeName, string address)
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

        static public int UpdateUserTypeInHome(int appUserId, int userToUpdateId, int homeId, string appUserTypeName, string currentUserTypeName, string updatedUserTypeName)
        {
            return DBService.UpdateUserTypeInHome(appUserId, userToUpdateId, homeId, appUserTypeName, currentUserTypeName, updatedUserTypeName);
        }

        static public int UpdateUserDevicePermissions(int appUserId, int userToUpdateId, int homeId, string appUserTypeName, string userToUpdateTypeName, int deviceId, int roomId, bool hasPermission)
        {
            return DBService.UpdateUserDevicePermissions(appUserId, userToUpdateId, homeId, appUserTypeName, userToUpdateTypeName, deviceId, roomId, hasPermission);
        }

        static public int UpdateUserRoomPermissions(int appUserId, int userToUpdateId, int homeId, string appUserTypeName, string userToUpdateTypeName, int roomId, bool hasAccess)
        {
            return DBService.UpdateUserRoomPermissions(appUserId, userToUpdateId, homeId, appUserTypeName, userToUpdateTypeName, roomId, hasAccess);
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
    }
}
