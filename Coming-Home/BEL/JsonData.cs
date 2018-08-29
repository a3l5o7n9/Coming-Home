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

        public JsonData(User u, List<User> lu, List<Home> lh)
        {
            U= u;
            LU = lu;
            LH = lh;
        }

        public JsonData(User u, List<User> lu)
        {
            U = u;
            LU = lu;
        }

        public JsonData(User u, List<Home> lh)
        {
            U = u;
            LH = lh;
        }

        public JsonData(User u, List<User> lu, List<Home> lh, List<Room> lr)
        {
            U = u;
            LU = lu;
            LH = lh;
            LR = lr;
        }

        public JsonData(User u, List<User> lu, List<Home> lh, List<Device> ld)
        {
            U = u;
            LU = lu;
            LH = lh;
            LD = ld;
        }

        public JsonData(User u, List<User> lu, List<Home> lh, List<Room> lr, List<Device> ld)
        {
            U = u;
            LU = lu;
            LH = lh;
            LR = lr;
            LD = ld;
        }
    }
}
