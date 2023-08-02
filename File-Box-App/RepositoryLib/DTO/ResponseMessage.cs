namespace RepositoryLib.DTO
{
    public class ResponseMessage
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public dynamic? Data { get; set; }

        public ResponseMessage(bool success, string message, dynamic? data) 
        {
            Success = success;
            Message = message;
            Data = data;
        }
    }
}
