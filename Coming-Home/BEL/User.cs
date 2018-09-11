using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEL
{
    public class User
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserPassword { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int HomeId { get; set; }
        public string UserTypeName { get; set; }
        public string Token { get; set; }

        public User(int userId, string userName, string userPassword, string firstName, string lastName)
        {
            UserId = userId;
            UserName = userName;
            UserPassword = userPassword;
            FirstName = firstName;
            LastName = lastName;
        }

        public User(int userId, string userName, string userPassword, string firstName, string lastName, int homeId, string userTypeName)
        {
            UserId = userId;
            UserName = userName;
            UserPassword = userPassword;
            FirstName = firstName;
            LastName = lastName;
            HomeId = homeId;
            UserTypeName = userTypeName;
        }

        public User(int userId, string userName, string userPassword, string firstName, string lastName, int homeId, string userTypeName, string token)
        {
            UserId = userId;
            UserName = userName;
            UserPassword = userPassword;
            FirstName = firstName;
            LastName = lastName;
            HomeId = homeId;
            UserTypeName = userTypeName;
            Token = token;
        }

        public User(int userId)
        {
            UserId = userId;
        }
    }
}
