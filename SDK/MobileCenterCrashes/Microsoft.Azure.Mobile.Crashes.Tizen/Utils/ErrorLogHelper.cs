using Microsoft.Azure.Mobile.Crashes.Ingestion.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Tizen.Applications;
using System.IO;
using Microsoft.Azure.Mobile.Ingestion.Models;

namespace Microsoft.Azure.Mobile.Crashes.Utils
{
    public static class ErrorLogHelper
    {
        public static string ERROR_DIRECTORY = "error/";

        public static string LOG_FILE_EXTENSION = ".json";

        public static string EXCEPTION_FILE_EXTENSION = ".exception";

        private static string ErrorDirectoryPath = null;

        public static void InitializeErrorDirectoryPath()
        {
            try
            {
                Directory.CreateDirectory(Application.Current.DirectoryInfo.Data + ERROR_DIRECTORY);
                ErrorDirectoryPath = Application.Current.DirectoryInfo.Data + ERROR_DIRECTORY;
            }
            catch (Exception exc)
            {
                MobileCenterLog.Debug(Crashes.LogTag, $"Error in Initializing Crashes Error Log Directory!!! {exc.Message}");
            }
        }

        public static void WriteErrorLogToFile(ManagedErrorLog log)
        {
            if (ErrorDirectoryPath == null)
            {
                MobileCenterLog.Debug(Crashes.LogTag, "Unable to write to file. Directory Creation failed");
                return;
            }

            string filePath = ErrorDirectoryPath + log.Id.ToString() + LOG_FILE_EXTENSION;
            string logString = LogSerializer.Serialize(log);

            StreamWriter writer = File.CreateText(filePath);
            writer.Write(logString);
            writer.Close();
        }

        public static void WriteExceptionToFile(Exception exception, Guid logId)
        {
            if (ErrorDirectoryPath == null)
            {
                MobileCenterLog.Debug(Crashes.LogTag, "Unable to write to file. Directory Creation failed");
                return;
            }

            string filePath = ErrorDirectoryPath + logId.ToString() + EXCEPTION_FILE_EXTENSION;

            FileStream file = new FileStream(filePath, FileMode.Create);
            byte[] exceptionBytes = CrashesUtils.SerializeException(exception);

            file.Write(exceptionBytes, 0, exceptionBytes.Length);
            file.Close();
        }

        public static ManagedErrorLog ReadErrorLogFromFile(string filePath)
        {
            StreamReader reader = File.OpenText(filePath);
            string logString = reader.ReadToEnd();
            return (ManagedErrorLog)LogSerializer.DeserializeLog(logString);
        }

        public static Exception ReadExceptionFromFile(Guid logId)
        {
            string filePath = ErrorDirectoryPath + logId.ToString() + EXCEPTION_FILE_EXTENSION;
            FileStream file = new FileStream(filePath, FileMode.Open);

            int bytes = (int)file.Length;
            int offset = 0;

            byte[] exceptionBytes = new byte[bytes];

            while (bytes > 0)
            {
                int readBytes = file.Read(exceptionBytes, offset, bytes);

                if (readBytes == 0)
                    break;

                offset += readBytes;
                bytes -= readBytes;
            }

            return CrashesUtils.DeserializeException(exceptionBytes);
        }

        public static IEnumerable<string> GetErrorLogFileNames()
        {
            return Directory.EnumerateFiles(ErrorDirectoryPath, "*" + LOG_FILE_EXTENSION);
        }

        public static string GetLastAddedLogFile()
        {
            var logFiles = GetErrorLogFileNames();

            DateTime max = DateTime.MinValue;
            string maxFile = null;

            foreach (var file in logFiles)
            {
                var time = File.GetLastWriteTime(file);
                if (time > max)
                {
                    max = time;
                    maxFile = file;
                }
            }
            return maxFile;
        }

        public static void RemoveErrorLogFile(Guid Id)
        {
            var files = Directory.EnumerateFiles(ErrorDirectoryPath, Id.ToString() + LOG_FILE_EXTENSION);
            foreach (string file in files)
            {
                File.Delete(file);
            }
        }

        public static void RemoveExceptionFile(Guid Id)
        {
            var files = Directory.EnumerateFiles(ErrorDirectoryPath, Id.ToString() + EXCEPTION_FILE_EXTENSION);
            foreach (string file in files)
            {
                File.Delete(file);
            }
        }

        public static void RemoveAllFiles()
        {
            var files = Directory.EnumerateFiles(ErrorDirectoryPath);
            foreach (string file in files)
            {
                File.Delete(file);
            }
        }

    }
}
