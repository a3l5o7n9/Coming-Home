using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEL
{
    public class JsonData
    {
        public List<User> LU { get; set; }
        public List<Home> LH { get; set; }
        public List<Room> LR { get; set; }
        public List<Device> LD { get; set; }

        public JsonData(List<User> lu, List<Home> lh)
        {
            LU = lu;
            LH = lh;
        }

        public JsonData(List<User> lu)
        {
            LU = lu;
        }

        public JsonData(List<Home> lh)
        {
            LH = lh;
        }

        public JsonData(List<User> lu, List<Home> lh, List<Room> lr)
        {
            LU = lu;
            LH = lh;
            LR = lr;
        }

        public JsonData(List<User> lu, List<Home> lh, List<Device> ld)
        {
            LU = lu;
            LH = lh;
            LD = ld;
        }

        public JsonData(List<User> lu, List<Home> lh, List<Room> lr, List<Device> ld)
        {
            LU = lu;
            LH = lh;
            LR = lr;
            LD = ld;
        }
    }
}
