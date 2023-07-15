namespace AMS.Web.ViewModels.Responses
{
    public class ErrorApiResponse
    {
        public bool Success => false;
        public string Message { get; set; }
        public string ErrorCode { get; set; }
        public Dictionary<string, string[]> ValidationErrors { get; set; }
    }
}
