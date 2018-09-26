using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEL
{
    public class ActivationMethod
    {
        public int ActivationMethodCode { get; set; }
        public string ActivationMethodName { get; set; }

        public ActivationMethod(int activationMethodCode, string activationMethodName)
        {
            ActivationMethodCode = activationMethodCode;
            ActivationMethodName = activationMethodName;
        }
    }
}
