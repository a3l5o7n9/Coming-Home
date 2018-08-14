using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEL
{
    public class Home
    {
        public int HomeId { get; set; }
        public string HomeName { get; set; }
        public int NumOfUsers { get; set; }
        public string Address { get; set; }

        public Home(int homeId, string homeName, int numOfUsers, string address)
        {
            HomeId = homeId;
            HomeName = homeName;
            NumOfUsers = numOfUsers;
            Address = address;
        }
    }
}
