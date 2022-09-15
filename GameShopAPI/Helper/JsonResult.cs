namespace GameShopAPI.Helper
{
    public class JsonResult
    {
        public string status { get; set; }
        public string? auth_token { get; set; }
        public string? message { get; set; }
        public bool? is_admin { get; set; }
    }
}
