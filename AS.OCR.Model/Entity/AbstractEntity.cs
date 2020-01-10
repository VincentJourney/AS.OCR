using Dapper.Contrib.Extensions;
using System;

namespace AS.OCR.Model.Entity
{
    public abstract class AbstractEntity
    {
        [ExplicitKey]
        public Guid Id { get; set; }
    }
}
