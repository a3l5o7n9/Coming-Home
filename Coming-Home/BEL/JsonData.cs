using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEL
{
    public class JsonData
    {
        public int UserId { get; set; }
        public List<User> LU { get; set; }
        public List<Home> LH { get; set; }
        public List<Room> LR { get; set; }
        public List<Device> LD { get; set; }

        public JsonData(int userId, List<User> lu, List<Home> lh)
        {
            UserId = userId;
            LU = lu;
            LH = lh;
        }

        public JsonData(int userId, List<User> lu)
        {
            UserId = userId;
            LU = lu;
        }

        public JsonData(int userId, List<Home> lh)
        {
            UserId = userId;
            LH = lh;
        }

        public JsonData(int userId, List<User> lu, List<Home> lh, List<Room> lr)
        {
            UserId = userId;
            LU = lu;
            LH = lh;
            LR = lr;
        }

        public JsonData(int userId, List<User> lu, List<Home> lh, List<Device> ld)
        {
            UserId = userId;
            LU = lu;
            LH = lh;
            LD = ld;
        }

        public JsonData(int userId, List<User> lu, List<Home> lh, List<Room> lr, List<Device> ld)
        {
            UserId = userId;
            LU = lu;
            LH = lh;
            LR = lr;
            LD = ld;
        }
    }
}
