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

        //static public JsonData GetUserDetails(int userId)
        //{
        //    return DBService.GetUserDetails(userId);
        //}

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
    }

    
}
