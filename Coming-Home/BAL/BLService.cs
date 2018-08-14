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

        static public User Login(string userName, string userPassword)
        {
            return DBService.Login(userName, userPassword);
        }

        static public int UpdateTokenForUserId(string token, int userId)
        {
            return DBService.UpdateTokenForUserId(token, userId);
        }
    }

    
}
