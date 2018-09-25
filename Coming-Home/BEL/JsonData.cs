using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEL
{
    public class JsonData
    {
        public AppUser AU {get; set;}
        public User U { get; set; }
        public List<User> LU { get; set; }
        public Home H { get; set; }
        public List<Home> LH { get; set; }
        public Room R { get; set; }
        public List<Room> LR { get; set; }
        public Device D { get; set; }
        public List<Device> LD { get; set; }
        public ActivationCondition ActCon { get; set; }
        public List<ActivationCondition> LActCon { get; set; }
        public string ResultMessage { get; set; }

        public JsonData(AppUser au, string resMes)
        {
            AU = au;
            ResultMessage = resMes;
        }

        public JsonData(AppUser au, List<User> lu, List<Home> lh, string resMes)
        {

            AU = au;
            LU = lu;
            LH = lh;
            ResultMessage = resMes;
        }

        public JsonData(List<User> lu, string resMes)
        {
            LU = lu;
            ResultMessage = resMes;
        }

        public JsonData(List<Home> lh, string resMes)
        {
            LH = lh;
            ResultMessage = resMes;
        }

        public JsonData(List<Room> lr, string resMes)
        {
            LR = lr;
            ResultMessage = resMes;
        }

        public JsonData(List<Device> ld, string resMes)
        {
            LD = ld;
            ResultMessage = resMes;
        }

        public JsonData(List<ActivationCondition> lActCon, string resMes)
        {
            LActCon = lActCon;
            ResultMessage = resMes;
        }

        public JsonData(List<User> lu, List<Home> lh, List<Room> lr, string resMes)
        {
            LU = lu;
            LH = lh;
            LR = lr;
            ResultMessage = resMes;
        }

        public JsonData(List<User> lu, List<Home> lh, List<Device> ld, string resMes)
        {
            LU = lu;
            LH = lh;
            LD = ld;
            ResultMessage = resMes;
        }

        public JsonData(List<User> lu, List<Room> lr, List<Device> ld, string resMes)
        {
            LU = lu;
            LR = lr;
            LD = ld;
            ResultMessage = resMes;
        }

        //public JsonData(List<User> lu, List<Home> lh, List<Room> lr, List<Device> ld, string resMes)
        //{
        //    LU = lu;
        //    LH = lh;
        //    LR = lr;
        //    LD = ld;
        //    ResultMessage = resMes;
        //}

        public JsonData(string resMes)
        {
            ResultMessage = resMes;
        }

        //public JsonData(AppUser au, User u, Home h, string resMes)
        //{
        //    AU = au;
        //    U = u;
        //    H = h;
        //    ResultMessage = resMes;
        //}

        //public JsonData(AppUser au, User u, Home h, Room r, string resMes)
        //{
        //    AU = au;
        //    U = u;
        //    H = h;
        //    R = r;
        //    ResultMessage = resMes;
        //}

        //public JsonData(AppUser au, User u, Room r, string resMes)
        //{
        //    AU = au;
        //    U = u;
        //    R = r;
        //    ResultMessage = resMes;
        //}

        //public JsonData(AppUser au, User u, Home h, Device d, string resMes)
        //{
        //    AU = au;
        //    U = u;
        //    H = h;
        //    D = d;
        //    ResultMessage = resMes;
        //}

        //public JsonData(AppUser au, User u, Device d, string resMes)
        //{
        //    AU = au;
        //    U = u;
        //    D = d;
        //    ResultMessage = resMes;
        //}

        public JsonData(User u, string resMes)
        {
            U = u;
            ResultMessage = resMes;
        }

        public JsonData(Home h, string resMes)
        {
            H = h;
            ResultMessage = resMes;
        }

        public JsonData(Room r, string resMes)
        {
            R = r;
            ResultMessage = resMes;
        }

        public JsonData(Device d, string resMes)
        {
            D = d;
            ResultMessage = resMes;
        }

        public JsonData(ActivationCondition actCon, string resMes)
        {
            ActCon = actCon;
            ResultMessage = resMes;
        }
    }
}
