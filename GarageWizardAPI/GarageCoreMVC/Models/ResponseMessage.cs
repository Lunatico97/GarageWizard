namespace GarageCoreAPI.Models
{
    public class ResponseMessage
    {
        public string Message { get; set; } = string.Empty;
        public bool Success { get; set; }
        public int StatusCode { get; set; }
    }
}
