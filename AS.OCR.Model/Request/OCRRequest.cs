using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AS.OCR.Model.Request
{
    public class ReceiptRequest
    {
        public string imageUrl { get; set; }
        public int? Type { get; set; }
    }
}
