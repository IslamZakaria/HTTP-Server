using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{
    class Logger
    {
        static FileStream fs;
        static StreamWriter sr;
        public static void LogException(Exception ex)
        {
            // TODO: Create log file named log.txt to log exception details in it
            //Datetime:
            //message:
            // for each exception write its details associated with datetime 
            fs = new FileStream("../../log.txt", FileMode.OpenOrCreate);
            sr = new StreamWriter(fs);
            string dateTime = DateTime.Now.ToString();
            string error = ex.Message;
            sr.WriteLine(dateTime + " : " + error);
            sr.Close();
        }
    }
}
