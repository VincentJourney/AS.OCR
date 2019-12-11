using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AS.OCR.Model.Entity
{
    [Table("Mall")]
    public class Mall
    {
        [ExplicitKey]
        public Guid MallID { get; set; }
        public string MallName { get; set; }
        public string MallCode { get; set; }
        public Guid? OrgID { get; set; }
    }
}
