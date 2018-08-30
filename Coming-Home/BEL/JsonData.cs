using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEL
{
    public class JsonData
    {
        public User U { get; set; }
        public List<User> LU { get; set; }
        public Home H { get; set; }
        public List<Home> LH { get; set; }
        public Room R { get; set; }
        public List<Room> LR { get; set; }
        public Device D { get; set; }
        public List<Device> LD { get; set; }
        public string ResultMessage { get; set; }

        public JsonData(User u, List<User> lu, List<Home> lh, string resMes)
        {
            U= u;
            LU = lu;
            LH = lh;
            ResultMessage = resMes;
        }

        public JsonData(User u, List<User> lu, string resMes)
        {
            U = u;
            LU = lu;
            ResultMessage = resMes;
        }

        public JsonData(User u, List<Home> lh, string resMes)
        {
            U = u;
            LH = lh;
            ResultMessage = resMes;
        }

        public JsonData(User u, List<User> lu, List<Home> lh, List<Room> lr, string resMes)
        {
            U = u;
            LU = lu;
            LH = lh;
            LR = lr;
            ResultMessage = resMes;
        }

        public JsonData(User u, List<User> lu, List<Home> lh, List<Device> ld, string resMes)
        {
            U = u;
            LU = lu;
            LH = lh;
            LD = ld;
            ResultMessage = resMes;
        }

        public JsonData(User u, List<User> lu, List<Home> lh, List<Room> lr, List<Device> ld, string resMes)
        {
            U = u;
            LU = lu;
            LH = lh;
            LR = lr;
            LD = ld;
            ResultMessage = resMes;
        }

        public JsonData(string resMes)
        {
            ResultMessage = resMes;
        }

        public JsonData(User u, Home h, string resMes)
        {
            U = u;
            H = h;
            ResultMessage = resMes;
        }

        public JsonData(User u, string resMes)
        {
            U = u;
            ResultMessage = resMes;
        }

        public JsonData(User u, Home h, Room r, string resMes)
        {
            U = u;
            H = h;
            R = r;
            ResultMessage = resMes;
        }

        public JsonData(User u, Room r, string resMes)
        {
            U = u;
            R = r;
            ResultMessage = resMes;
        }

        public JsonData(User u, Home h, Device d, string resMes)
        {
            U = u;
            H = h;
            D = d;
            ResultMessage = resMes;
        }

        public JsonData(User u, Device d, string resMes)
        {
            U = u;
            D = d;
            ResultMessage = resMes;
        }
    }
}
