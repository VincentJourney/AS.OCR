using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace AS.OCR.Model.Entity
{
    public abstract class AbstractEntity
    {
        [ExplicitKey]
        public Guid Id { get; set; }
    }
}
