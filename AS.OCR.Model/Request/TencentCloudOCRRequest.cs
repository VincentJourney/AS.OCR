using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BaiDuOCR.Model.Request
{
    public class TencentCloudOCRRequest
    {
        public string Image { get; set; }
        public int? Type { get; set; } = 1;
    }
}
