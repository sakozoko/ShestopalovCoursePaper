using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CourseWork.Services
{
    public class FileLogger : ILogger
    {
        private const string LogPath = "log.txt";
        public FileLogger()
        {
            if (!File.Exists(LogPath))
            {
                File.Create(LogPath);
            }
        }
        public void Log(string message)
        {
            using var writer = new StreamWriter(LogPath, true);
            writer.WriteLine($"{DateTime.Now}: {message}");
        }
    }   
}