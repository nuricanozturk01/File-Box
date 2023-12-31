﻿using System.Text.Json.Serialization;

namespace Repository.Exceptions
{
    public class MessageResponse
    {
        public string Message { get; set; }
        public int Status { get; set; }
        public string Detail { get; set; }

        [JsonIgnore]
        public object Data { get; set; }

        public string Token { get; set; }
        public MessageResponse(string message, int status, string detail)
        {
            Message = message;
            Status = status;
            Detail = detail;
        }

        public MessageResponse(string message, int status, string detail, string token)
        {
            Message = message;
            Status = status;
            Detail = detail;
            Token = token;
        }

        public MessageResponse(string message, int status, string detail, object data)
        {
            Data = data;
            Message = message;
            Status = status;
            Detail = detail;
        }
    }
}
