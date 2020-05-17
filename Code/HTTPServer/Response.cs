using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{

    public enum StatusCode
    {
        OK = 200,
        InternalServerError = 500,
        NotFound = 404,
        BadRequest = 400,
        Redirect = 301
    }

    class Response
    {
        string responseString;
        public string ResponseString
        {
            get
            {
                return responseString;
            }
        }
        StatusCode code;
        List<string> headerLines = new List<string>();
        public Response(StatusCode code, string contentType, string content, string redirectoinPath)
        {
            
            // TODO: Add headlines (Content-Type, Content-Length,Date, [location if there is redirection])
            string header =  "Content-Type: " + contentType + "\r\n" +
                   "Content-Length: " + content.Length + "\r\n" +
                   "Date: " + DateTime.Now.ToString() + "\r\n";
            if (redirectoinPath != null)
                 header += "Location: " + redirectoinPath + "\r\n";
            // TODO: Create the response string
            switch (code)
            {
                case StatusCode.OK:
                    this.responseString = this.GetStatusLine(code) + " OK" + "\r\n" + header + "\r\n" + content;
                    break;
                case StatusCode.InternalServerError:
                    this.responseString = this.GetStatusLine(code) + " Internal Server Error" + "\r\n" + header + "\r\n" + content;
                    break;
                case StatusCode.NotFound:
                    this.responseString = this.GetStatusLine(code) + " Not Found" + "\r\n" + header + "\r\n" + content;
                    break;
                case StatusCode.BadRequest:
                    this.responseString = this.GetStatusLine(code) + " Bad Request" + "\r\n" + header + "\r\n" + content;
                    break;
                case StatusCode.Redirect:
                    this.responseString = this.GetStatusLine(code) + " Redirect" + "\r\n" + header + "\r\n" + content;
                    break;
            }
        }
        private string GetStatusLine(StatusCode code)
        {
            // TODO: Create the response status line and return it
            string statusLine = Configuration.ServerHTTPVersion + ((int)code).ToString();
            return statusLine;
        }
    }
}
