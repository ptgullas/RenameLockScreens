using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;


using RenameLockScreens.Properties;
using RenameLockScreens.Models;

namespace RenameLockScreens {
    public class Program {
        public static void Main(string[] args) {
            ProcessLogFiles(new LockScreenBatchToLog(Settings.Default.downloadFolder));
        }

        // Not sure if I want this to actually be used in Production, but it's the basic program flow:
        static void ProcessLogFiles(LockScreenBatchToLog batchFromDownload) {
            string workingFolder = Settings.Default.defaultFolder;
            string baseLogFileName = Settings.Default.logFileName; // $"lockscreenlog_{DateTime.Now.ToString("yyyyMMddHHmmss")}.json";
            string logFilePath = Path.Combine(workingFolder, "log", baseLogFileName);

            FileInfo myLogFile = new FileInfo(logFilePath);

            if (myLogFile.Exists) {
                batchFromDownload.RemoveLockScreensIfExistInLog(new LockScreenBatchToLog(myLogFile));
            }
            if (batchFromDownload.myLockScreens.Count > 0) {
                batchFromDownload.MoveRenameAndMoveLockScreensToAspectRatioFolder(workingFolder);
                batchFromDownload.LogLockScreensToFileAsJson(myLogFile.FullName);
            }
        }


        static void CreateInitialLog() {
            string logFileLocation = Settings.Default.defaultFolder + "\\log";
            string baseLogFileName = Settings.Default.logFileName; // $"lockscreenlog_{DateTime.Now.ToString("yyyyMMddHHmmss")}.json";
            string logFilePath = logFileLocation + "\\" + baseLogFileName;

            string downloadFolder = Settings.Default.downloadFolder;
            LockScreenBatchToLog lockScreenBatch = GetLockScreenBatchFromDownloadFolder(downloadFolder);
            // myBatch.LogLockScreensToFileAsFilenames(logFileLocation + "\\" + baseLogFileName);
            lockScreenBatch.LogLockScreensToFileAsJson(logFilePath);
        }

        public static LockScreenBatchToLog GetLockScreenBatchFromDownloadFolder(string downloadPath) {
            LockScreenBatchToLog myBatch = new LockScreenBatchToLog(downloadPath);
            return myBatch;
        }
    }
}
