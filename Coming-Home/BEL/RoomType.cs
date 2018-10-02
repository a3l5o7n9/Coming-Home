using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEL
{
    public class RoomType
    {
        public int RoomTypeCode { get; set; }
        public string RoomTypeName { get; set; }

        public RoomType(int roomTypeCode, string roomTypeName)
        {
            RoomTypeCode = roomTypeCode;
            RoomTypeName = roomTypeName;
        }
    }
}
