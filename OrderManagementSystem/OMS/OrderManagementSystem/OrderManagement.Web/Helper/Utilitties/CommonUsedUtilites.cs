using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace OrderManagement.Web.Helper.Utilitties
{
    public static class CommonUsedUtilites
    {
        /// <summary>
        /// LoggerInTextFile
        /// </summary>
        /// <param name="logText"></param>
        /// <param name="FilePath"></param>
        public static void LoggerInTextFile(string logText, string FilePath = "d:\\Google.txt")
        {
            using (StreamWriter tw = new StreamWriter(FilePath, true))
            {
                tw.WriteLine(logText);
                tw.WriteLine(DateTime.Now);
                tw.WriteLine("--------------------");
            }
        }
    }
}