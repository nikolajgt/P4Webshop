

using P4Webshop.Models.Base;

namespace P4Webshop.Models.HTTP
{
    public class ResponseModel
    {
        public ResponseCode ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public object DataSet { get; set; }
    }
}
