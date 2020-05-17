using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{
    class Program
    {
        static string redirectionMatrixPath = "redirectionRules.txt";
        static void Main(string[] args)
        {
            Console.Title = "HTTP Server";
            Console.WriteLine("Server started on port:1000");
            // TODO: Call CreateRedirectionRulesFile() function to create the rules of redirection 
            CreateRedirectionRulesFile();
            //Start server
            // 1) Make server object on port 1000
            Server server = new Server(1000 , redirectionMatrixPath);
            // 2) Start Server
            server.StartServer();
        }
        static void CreateRedirectionRulesFile()
        {
            // TODO: Create file named redirectionRules.txt
            // means that when making request to aboustus.html,, it redirects me to aboutus2
            // each line in the file specify a redirection rule
            FileStream fs = new FileStream(redirectionMatrixPath, FileMode.Create);
            StreamWriter writter = new StreamWriter(fs);
            // example: "aboutus.html,aboutus2.html"
            writter.WriteLine("aboutus.html,aboutus2.html");
            writter.Close();
        }
    }
}
