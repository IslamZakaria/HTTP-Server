using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace HTTPServer
{
    class Server
    {
        Socket serverSocket;
        string main_content_type = "text/html";
        public Server(int portNumber, string redirectionMatrixPath) // Done
        {
            //TODO: call this.LoadRedirectionRules passing redirectionMatrixPath to it
            this.LoadRedirectionRules(redirectionMatrixPath);
            //TODO: initialize this.serverSocket
            IPEndPoint server_iep = new IPEndPoint(IPAddress.Any, portNumber);
            this.serverSocket = new Socket(AddressFamily.InterNetwork , SocketType.Stream,ProtocolType.Tcp);
            this.serverSocket.Bind(server_iep);
        }
        public void StartServer() // Done
        {
            // TODO: Listen to connections, with large backlog.
            this.serverSocket.Listen(1000);
            // TODO: Accept connections in while loop and start a thread for each connection on function "Handle Connection"
            while (true)
            {
                //TODO: accept connections and start thread for each accepted connection.
                Socket client_socket = this.serverSocket.Accept();
                Console.WriteLine("New Client Connection at : {0}", client_socket.RemoteEndPoint);
                Thread _thread = new Thread(new ParameterizedThreadStart(this.HandleConnection));
                _thread.Start(client_socket);
            }
        }
        public void HandleConnection(object obj) // Done
        {
            // TODO: Create client socket 
            Socket client_socket = (Socket)obj;
            // set client socket ReceiveTimeout = 0 to indicate an infinite time-out period
            client_socket.ReceiveTimeout = 0;
            // TODO: receive requests in while true until remote client closes the socket.
            byte[] requestMsg;
            while (true)
            {
                try
                {
                    requestMsg = new byte[1024];
                    // TODO: Receive request
                    int length = client_socket.Receive(requestMsg);
                    // TODO: break the while loop if receivedLen==0
                    if (length == 0)
                    {
                        Console.WriteLine("Client Connection Ended : {0}", client_socket.RemoteEndPoint);
                        break;
                    }
                    // TODO: Create a Request object using received request string
                    Request request = new Request(Encoding.ASCII.GetString(requestMsg, 0, length));
                    // TODO: Call HandleRequest Method that returns the response
                    Response response = this.HandleRequest(request);
                    // TODO: Send Response back to client
                    client_socket.Send(Encoding.ASCII.GetBytes(response.ResponseString));
                }
                catch (Exception ex)
                {
                    // TODO: log exception using Logger class
                    Logger.LogException(ex);
                 }
            }
            // TODO: close client socket
            client_socket.Shutdown(SocketShutdown.Both);
            client_socket.Close();
        }
        Response HandleRequest(Request request) // Done
        {
            string content;
            try
            {
                //throw new Exception(); //test internal server error
                //TODO: check for bad request 
                if (! request.ParseRequest()) // bad request case
                {
                    content = this.LoadDefaultPage(Configuration.BadRequestDefaultPageName);
                    return new Response(StatusCode.BadRequest, this.main_content_type, content, null);
                }
                //TODO: map the relativeURI in request to get the physical path of the resource.
                //TODO: check for redirect
                if (Configuration.RedirectionRules.ContainsKey(request.relativeURI))
                {
                    // Redirct Response
                    string redirction_path = Configuration.RedirectionRules[request.relativeURI];
                    content = LoadDefaultPage(GetRedirectionPagePathIFExist(redirction_path));
                    return new Response(StatusCode.Redirect, this.main_content_type, content, redirction_path);
                }
                if (request.relativeURI == string.Empty)
                {
                    if(File.Exists(Path.Combine(Configuration.RootPath, Configuration.IndexDefaultPageName)))
                    {
                        content = LoadDefaultPage(Configuration.IndexDefaultPageName);
                        return new Response(StatusCode.OK, this.main_content_type, content, null);
                    }
                    else
                    {
                        content = this.FetchAllDirectories();
                        return new Response(StatusCode.OK, this.main_content_type, content , null);
                    }
                }
                //TODO: read the physical file
                content = LoadDefaultPage(request.relativeURI);
                //TODO: check file exists
                if (content != "")
                    // Create OK response
                    return new Response(StatusCode.OK, this.main_content_type, content, null);
                
                // Create 404 NotFound Response [Error]
                content = LoadDefaultPage(Configuration.NotFoundDefaultPageName);
                return new Response(StatusCode.NotFound, this.main_content_type, content, null);
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                // TODO: in case of exception, return Internal Server Error. 
                content = this.LoadDefaultPage(Configuration.InternalErrorDefaultPageName);
                return new Response(StatusCode.InternalServerError, this.main_content_type, content, null);
            }
        }
        private string FetchAllDirectories()
        {
            string[] dirs = Directory.GetDirectories(Configuration.RootPath);
            string[] files = Directory.GetFiles(Configuration.RootPath);
            string res = "<html><header><title>Index of</title><style>a{display:block;font-size:24px;margin:12px}</style></header><body><h1>Index of /fcis1</h1><hr>";
            for (int i = 0; i < dirs.Length; i++)
            {
                dirs[i] = dirs[i].Replace(Configuration.RootPath, "").Replace(@"\", "");
                res += "<a href='http://localhost:1000/" + dirs[i] + "' >" + dirs[i] + "</a>";
            }
            for (int i = 0; i < files.Length; i++)
            {
                files[i] = files[i].Replace(Configuration.RootPath, "").Replace(@"\" , "");
                res += "<a href='http://localhost:1000/" + files[i] + "' >" + files[i] + "</a>";
            }
            res += "<p>HTTP Server at localhost Port 1000 </p>";
            res += "</body></html>";
            return res;
        }
        private string GetRedirectionPagePathIFExist(string relativePath)// Done
        {
            // using Configuration.RedirectionRules return the redirected page path if exists else returns empty
            return File.Exists(Configuration.RootPath + "\\" + relativePath) ? relativePath  // true case
                                             :  string.Empty; // false case
        }
        private string LoadDefaultPage(string defaultPageName)// Done
        {
            string filePath = Path.Combine(Configuration.RootPath, defaultPageName);
            // TODO: check if filepath not exist log exception using Logger class and return empty string
            if (File.Exists(filePath))
                return File.ReadAllText(filePath);
            // else read file and return its content
            Logger.LogException(new Exception(defaultPageName + " dose not exists"));
            return string.Empty;
        }
        private void LoadRedirectionRules(string filePath)// Done
        {
            try
            {
                Configuration.RedirectionRules = new Dictionary<string, string>();
                // TODO: using the filepath paramter read the redirection rules from file
                string[] spl = new string[] { "\r\n" };
                string[] redirctionRules = File.ReadAllText(filePath).Split(spl, StringSplitOptions.RemoveEmptyEntries);
                // then fill Configuration.RedirectionRules dictionary 
                for (int i = 0; i < redirctionRules.Length; i++)
                {
                    string[] rule = redirctionRules[i].Split(',');
                    if (rule.Length > 1)
                    {
                        Configuration.RedirectionRules.Add(rule[0], rule[1]);
                    }
                }
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                  Logger.LogException(ex);
                  Environment.Exit(1);
              }
        }
    }
}
