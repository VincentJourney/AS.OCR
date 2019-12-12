using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AS.OCR.Commom.Attributes
{
    public class ValidateAttr : Attribute
    {
        public ValidateAttr()
        {

        }

        public string IgnoreKey { get; set; }
    }
}
