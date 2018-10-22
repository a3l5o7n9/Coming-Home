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
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Altitude { get; set; }
        public double Accuracy { get; set; }

        public Home(int homeId, string homeName, int numOfUsers, string address, double latitude, double longitude, double altitude, double accuracy)
        {
            HomeId = homeId;
            HomeName = homeName;
            NumOfUsers = numOfUsers;
            Address = address;
            Latitude = latitude;
            Longitude = longitude;
            Altitude = altitude;
            Accuracy = accuracy;
        }

        public Home(string homeName, int numOfUsers, string address, double latitude, double longitude, double altitude, double accuracy)
        {
            HomeName = homeName;
            NumOfUsers = numOfUsers;
            Address = address;
            Latitude = latitude;
            Longitude = longitude;
            Altitude = altitude;
            Accuracy = accuracy;
        }

        public Home(int homeId)
        {
            HomeId = homeId;
        }
    }
}
