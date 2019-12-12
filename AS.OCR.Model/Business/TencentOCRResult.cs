using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AS.OCR.Model.Business
{
    public class TencentOCRResult
    {
        public List<Word> WordList { get; set; }
    }

    public class Word
    {
        public string text { get; set; }
    }
}
