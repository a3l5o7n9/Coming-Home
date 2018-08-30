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
        public List<Home> LH { get; set; }
        public List<Room> LR { get; set; }
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
    }
}
