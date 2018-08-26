﻿using BEL;
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

        static public JsonData CreateHome(int userId, string homeName, string address)
        {
            return DBService.CreateHome(userId, homeName, address);
        }

        static public JsonData JoinHome(int userId, string homeName, string address)
        {
            return DBService.JoinHome(userId, homeName, address);
        }

        static public JsonData CreateRoom(string roomName, int homeId, string roomTypeName)
        {
            return DBService.CreateRoom(roomName, homeId, roomTypeName);
        }

        static public JsonData CreateDevice(string deviceName, int homeId, string deviceTypeName, int userId, int roomId)
        {
            return DBService.CreateDevice(deviceName, homeId, deviceTypeName, userId, roomId);
        }

        public static void InitErrorLogPath(string v)
        {
            DBService.InitErrorLogPath(v);
        }

        static public int UpdateTokenForUserId(string token, int userId)
        {
            return DBService.UpdateTokenForUserId(token, userId);
        }
    }

    
}
