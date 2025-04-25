using System.Net;

namespace Common.Server.Models
{
    public struct RequestResponse
    {
        public string content;
        public HttpStatusCode status;
        public WebHeaderCollection headers;
    }
}