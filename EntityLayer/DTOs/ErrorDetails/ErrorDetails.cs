using System;

namespace EntityLayer.DTOs.ErrorDetails
{
    public class ErrorDetails
    {
        public string Message { get; set; }

        public override string ToString()
        {
            return $"{{\"Message\": \"{Message}\"}}";
        }
    }
}