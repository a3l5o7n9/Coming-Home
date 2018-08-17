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
        public Home H { get; set; }
        public Room R { get; set; }
        public Device D { get; set; }

        public JsonData(User u, Home h)
        {
            U = u;
            H = h;
        }

        public JsonData(Home h)
        {
            H = h;
        }

        public JsonData(User u, Home h, Room r)
        {
            U = u;
            H = h;
            R = r;
        }

        public JsonData(User u, Home h, Device d)
        {
            U = u;
            H = h;
            D = d;
        }

        public JsonData(User u, Home h, Room r, Device d)
        {
            U = u;
            H = h;
            R = r;
            D = d;
        }
    }
}
