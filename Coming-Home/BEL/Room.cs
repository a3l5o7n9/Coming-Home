﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEL
{
    public class Room
    {
        public int RoomId { get; set; }
        public string RoomName { get; set; }
        public string RoomTypeName { get; set; }
        public int HomeId { get; set; }
        public bool IsShared { get; set; }
        public int NumOfDevices { get; set; }
        public bool HasAccess { get; set; }

        public Room(int roomId, string roomName, string roomTypeName, int homeId, bool isShared , int numOfDevices, bool hasAccess)
        {
            RoomId = roomId;
            RoomName = roomName;
            RoomTypeName = roomTypeName;
            HomeId = homeId;
            IsShared = isShared;
            NumOfDevices = numOfDevices;
            HasAccess = hasAccess;
        }

        public Room(int roomId)
        {
            RoomId = roomId;
        }
    }
}
