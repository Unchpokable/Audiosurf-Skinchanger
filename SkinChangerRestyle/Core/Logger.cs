﻿namespace ChangerAPI.Utilities
{
    using System;
    using System.IO;
    using System.Text;

    public class Logger
    {
        private string LogFilePath;

        public Logger()
        {
            LogFilePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Audiosurf SkinChanger Logs\log.txt";
        }

        public Logger(string path)
        {
            LogFilePath = path;
        }

        public void Log(string logTitle, string message)
        {
            if (!File.Exists(LogFilePath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(LogFilePath));
                File.Create(LogFilePath);
            }

            using (var logStream = new FileStream(LogFilePath, FileMode.Append))
            using (var writer = new StreamWriter(logStream, Encoding.UTF8))
            {
                writer.WriteLine(FormatMessage(logTitle, message));
            }
        }

        private string FormatMessage(string logTitle, string message)
        {
            return $"[{DateTime.Now}]::[{logTitle}]\n{message}\n";
        }
    }
}
