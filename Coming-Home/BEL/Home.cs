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
        public string CityName { get; set; }
        public string StreetName { get; set; }
        public int HomeNo { get; set; } 
        public string PostalCode { get; set; }

        public Home(int homeId, string homeName, int numOfUsers, string cityName, string streetName, int homeNo, string postalCode)
        {
            HomeId = homeId;
            HomeName = homeName;
            NumOfUsers = numOfUsers;
            CityName = cityName;
            StreetName = streetName;
            HomeNo = homeNo;
            PostalCode = postalCode;
        }
    }
}
