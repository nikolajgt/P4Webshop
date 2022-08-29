

using P4Webshop.Models.Base;

namespace P4Webshop.Models.HTTP
{
    public class ErrorResponseModel
    {
        public ResponseCode ResponseCode { get; set; }
        public string Message { get; set; }
        public ErrorResponseModel(ResponseCode responseCode, string message)
        {
            ResponseCode = responseCode;
            Message = message;
        }
    }
}
