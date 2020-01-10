using System;

namespace AS.OCR.Model.Business
{
    public class Store
    {
        public Guid Id { get; set; }

        public string StoreName { get; set; }

        public Guid MallId { get; set; }
    }
}
