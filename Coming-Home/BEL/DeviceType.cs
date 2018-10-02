using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEL
{
    public class DeviceType
    {
        public int DeviceTypeCode { get; set; }
        public string DeviceTypeName { get; set; }

        public DeviceType(int deviceTypeCode, string deviceTypeName)
        {
            DeviceTypeCode = deviceTypeCode;
            DeviceTypeName = deviceTypeName;
        }
    }
}
