namespace AS.OCR.Model.Request
{
    public class TencentCloudOCRRequest
    {
        public string Image { get; set; }
        public int? Type { get; set; } = 1;
    }
}
