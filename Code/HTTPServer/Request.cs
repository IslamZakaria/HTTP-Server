using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTTPServer
{
    public enum RequestMethod
    {
        GET,
        POST,
        HEAD
    }

    public enum HTTPVersion
    {
        HTTP10,
        HTTP11,
        HTTP09
    }

    class Request
    {
        string[] requestLines;
        RequestMethod method;
        public string relativeURI;
        Dictionary<string, string> headerLines;

        public Dictionary<string, string> HeaderLines
        {
            get { return headerLines; }
        }

        HTTPVersion httpVersion;
        string requestString;
        string[] contentLines;
        public Request(string requestString) // Done
        {
            this.requestString = requestString;
        }
        /// <summary>
        /// Parses the request string and loads the request line, header lines and content, returns false if there is a parsing error
        /// </summary>
        /// <returns>True if parsing succeeds, false otherwise.</returns>
        public bool ParseRequest() // Done
        {
            //TODO: parse the receivedRequest using the \r\n delimeter   

            // check that there is atleast 3 lines: Request line, Host Header, Blank line (usually 4 lines with the last empty line for empty content)

            // Parse Request line

            // Validate blank line exists

            // Load header lines into HeaderLines dictionary
            // return false; // testing bad request

            if (!ParseRequestLine() || !ValidateBlankLine() || !LoadHeaderLines())
                return false;
            else
                return true;
        }
        private bool ParseRequestLine() // Done
        {
            this.contentLines = this.requestString.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            if (this.contentLines.Length < 4)
                return false;
            else
            {
                this.requestLines = this.contentLines[0].Split(' ');
                if (this.requestLines.Length != 3) return false;
                switch (this.requestLines[0])
                {
                    case "GET":
                        method = RequestMethod.GET;
                        break;
                    case "POST":
                        method = RequestMethod.POST;
                        break;
                    default:
                        method = RequestMethod.HEAD;
                        break;
                }
                if (!this.ValidateIsURI(this.requestLines[1]))
                    return false;
                this.relativeURI = this.requestLines[1].Remove(0, 1);
                switch (this.requestLines[2])
                {
                    case "HTTP/1.1":
                        this.httpVersion = HTTPVersion.HTTP11;
                        break;
                    case "HTTP/1.0":
                        this.httpVersion = HTTPVersion.HTTP10;
                        break;
                    default:
                        this.httpVersion = HTTPVersion.HTTP09;
                        break;
                }
            }
            return true;
        }
        private bool ValidateIsURI(string uri) // Done
        {
            return Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute);
        }
        private bool LoadHeaderLines() // Done
        {

            string[] headerSepartors = new string[] { ": " };
            this.headerLines = new Dictionary<string, string>();
            for (int i = 1; i < this.contentLines.Length; i++)
            {
                string[] header = this.contentLines[i].Split(headerSepartors, StringSplitOptions.RemoveEmptyEntries);
                if (header.Length > 0)
                {
                    this.headerLines.Add(header[0] , header[1]);
                }
            }

            return this.headerLines.Count > 1 ;
        }
        private bool ValidateBlankLine() // Done
        {
            return this.requestString.EndsWith("\r\n\r\n");
        }
    }
}
