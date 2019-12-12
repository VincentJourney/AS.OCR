using Dapper.Contrib.Extensions;
using System;

namespace AS.OCR.Model.Entity
{
    [Table("ApplyPictureRecongnize")]
    public class ApplyPictureRecongnize : AbstractEntity
    {
        [ExplicitKey]
        public Guid id { get; set; }
        public Guid applyid { get; set; }
        public int Lineno { get; set; }
        public string LineContent { get; set; }

        public string OCRResult { get; set; }
        public DateTime AddedTime { get; set; }
    }
}
