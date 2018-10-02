using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEL
{
    public class UserType
    {
        public int UserTypeCode { get; set; } 
        public string UserTypeName { get; set; }

        public UserType(int userTypeCode, string userTypeName)
        {
            UserTypeCode = userTypeCode;
            UserTypeName = userTypeName;
        }
    }
}
