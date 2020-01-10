namespace AS.OCR.Model.Request
{

    public class ImageRequest
    {
        public string fileName { get; set; }

        public string base64Str { get; set; }

        public string sourceSystem { get; set; }

        public string fileDescription { get; set; }
    }
}
