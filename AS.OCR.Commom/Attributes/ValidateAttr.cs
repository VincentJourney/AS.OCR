using System;

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
