using System;
using System.IO;
using Learning.Logger.Properties;

namespace Learning.Logger
{
    public static class Logger
    {
        private const string LogFileName = "applicationLog.txt";
        private static string _logFilePath=null;
        public static void Initialize(string path)
        {
            if (_logFilePath!=null)
                return;
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            _logFilePath = Path.Combine(path, LogFileName);
            Log(Strings.Message_LogInitialized);
        }

        public static void Log(string message)
        {
            if (String.IsNullOrWhiteSpace(_logFilePath))
                return;
            TextWriter tw = new StreamWriter(_logFilePath, true);
            try
            {
                tw.WriteLine(Strings.Logger_LogTime, DateTime.UtcNow.ToString("G"));
                tw.WriteLine(Strings.Logger_LogMessage, message);
            }
            catch
            {
            }
            finally
            {
                tw.Close();
            }
        }
        public static void Log(Exception ex, string message=null)
        {
            if (String.IsNullOrWhiteSpace(_logFilePath))
                return;
            TextWriter tw = new StreamWriter(_logFilePath, true);

            try
            {
                tw.WriteLine(Strings.Logger_LogTime, DateTime.UtcNow.ToString("G"));
                if (!string.IsNullOrWhiteSpace(message))
                    tw.WriteLine(Strings.Logger_LogMessage, message);
                while (ex != null)
                {
                    tw.WriteLine(Strings.Logger_LogMessage, ex.Message);
                    tw.WriteLine(Strings.Logger_LogStackTrace, ex.StackTrace);
                    ex = ex.InnerException;
                }
            }
            catch
            {
            }
            finally
            {
                tw.Close();
            }
        }
    }
}
