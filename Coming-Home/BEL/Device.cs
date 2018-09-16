using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEL
{
    public class Device
    {
        public int DeviceId { get; set; }
        public string DeviceName { get; set; }
        public string DeviceTypeName { get; set; }
        public int HomeId { get; set; }
        public bool IsDividedIntoRooms { get; set; }
        public int RoomId { get; set; }
        public bool IsOn { get; set; }

        public Device(int deviceId, string deviceName, string deviceTypeName, int homeId, bool isDividedIntoRooms, int roomId, bool isOn)
        {
            DeviceId = deviceId;
            DeviceName = deviceName;
            DeviceTypeName = deviceTypeName;
            HomeId = homeId;
            IsDividedIntoRooms = isDividedIntoRooms;
            RoomId = roomId;
            IsOn = isOn;
        }

        public Device(int deviceId)
        {
            DeviceId = deviceId;
        }
    }
}
