using System;
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
        public int NumOfDevices { get; set; }

        public Room(int roomId, string roomName, string roomTypeName, int homeId, int numOfDevices)
        {
            RoomId = roomId;
            RoomName = roomName;
            RoomTypeName = roomTypeName;
            HomeId = homeId;
            NumOfDevices = numOfDevices;
        }
    }
}
