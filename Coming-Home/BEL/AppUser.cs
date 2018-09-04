using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEL
{
    public class AppUser
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserPassword { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Token { get; set; }

        public AppUser(int userId, string userName, string userPassword, string firstName, string lastName)
        {
            UserId = userId;
            UserName = userName;
            UserPassword = userPassword;
            FirstName = firstName;
            LastName = lastName;
        }

        public AppUser(int userId, string userName, string userPassword, string firstName, string lastName, string token)
        {
            UserId = userId;
            UserName = userName;
            UserPassword = userPassword;
            FirstName = firstName;
            LastName = lastName;
            Token = token;
        }
    }
}
