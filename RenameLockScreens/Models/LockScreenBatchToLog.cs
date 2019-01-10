using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO.Abstractions;

namespace RenameLockScreens.Models {
    public class LockScreenBatchToLog {

        private readonly IFileSystem _filesystem;
        public List<LockScreenImageToLog> myLockScreens;


        public LockScreenBatchToLog(string directoryPath) 
            : this(directoryPath, new FileSystem()) { }

        // constructor when building batch from files in directory
        public LockScreenBatchToLog(string directoryPath, IFileSystem fileSystem) {
            _filesystem = fileSystem;
            IEnumerable<string> files = GetFilesFromDownloadFolder(directoryPath);
            myLockScreens = new List<LockScreenImageToLog>();
            myLockScreens = CreateLockScreensFromFiles(files);
        }

        // constructor when building batch from a log file
        public LockScreenBatchToLog(FileInfoBase myFile) 
            : this(myFile, new FileSystem()) { }

        // constructor when building batch from a log file
        public LockScreenBatchToLog(FileInfoBase myFile, IFileSystem fileSystem) {
            _filesystem = fileSystem;
            string jsonObjects = _filesystem.File.ReadAllText(myFile.FullName);
            myLockScreens = JsonConvert.DeserializeObject<List<LockScreenImageToLog>>(jsonObjects);
        }

        public void RemoveLockScreensIfExistInLog(LockScreenBatchToLog lockScreensFromLogFile) {
            if (myLockScreens.Count > 0) {
                myLockScreens = myLockScreens.Except(lockScreensFromLogFile.myLockScreens).ToList();
                // myLockScreens = myLockScreens.Intersect(lockScreensFromLogFile.myLockScreens).ToList();
            }
        }

        private IEnumerable<string> GetFilesFromDownloadFolder(string directoryPath) {
            // The System.IO.Abstractions Directory.EnumerateFiles doesn't seem to work with a search pattern & search options
            IEnumerable<string> myFiles = _filesystem.Directory.EnumerateFiles(directoryPath, "*.", SearchOption.TopDirectoryOnly)
                .Where(s => new FileInfo(s).Length > 100000)
                .OrderBy(s => new FileInfo(s).CreationTime)
                //.ToList()
                ;
            return myFiles;
        }

        public void MoveRenameAndMoveLockScreensToAspectRatioFolder(string workingFolder) {
            if (myLockScreens.Count > 0) {
                foreach (LockScreenImageToLog ls in myLockScreens) {
                    ls.MoveToFolder(workingFolder);
                    ls.TrimFilenameDownToLast();
                    ls.AppendExtensionToFilename();
                    ls.MoveToAspectRatioFolder(workingFolder);
                }
            }
        }

        private List<LockScreenImageToLog> CreateLockScreensFromFiles(IEnumerable<string> myFiles) {
            List<LockScreenImageToLog> lockScreens = new List<LockScreenImageToLog>();
            foreach (string s in myFiles) {
                FileInfoBase fi = new FileInfo(s);
                LockScreenImageToLog ls = new LockScreenImageToLog(fi);
                lockScreens.Add(ls);
            }
            return lockScreens;
        }

        private string SerializeLockScreensToJson() {
            string output = "";
            if (myLockScreens.Count > 0) {
                output = JsonConvert.SerializeObject(myLockScreens, Formatting.Indented);
            }
            return output;
        }

        private List<string> GetLockScreensOriginalFilenames() {
            List<string> originalFilenames = new List<string>();
            if (myLockScreens.Count > 0) {
                foreach (LockScreenImageToLog ls in myLockScreens) {
                    originalFilenames.Add(ls.originalFileName);
                }
            }
            return originalFilenames;
        }

        public void LogLockScreensToFileAsFilenames(string outputPath) {
            List<string> originalFilenames = GetLockScreensOriginalFilenames();
            using (TextWriter tw = new StreamWriter(outputPath)) {
                foreach (string s in originalFilenames) {
                    tw.WriteLine(s);
                }
            }
        }

        public void LogLockScreensToFileAsJson(string outputPath) {
            FileInfoBase fi = new FileInfo(outputPath);
            string jsonOutput = SerializeLockScreensToJson();
            if (fi.Exists) {
                _filesystem.File.AppendAllText(outputPath, jsonOutput);
            }
            else {
                _filesystem.File.WriteAllText(outputPath, jsonOutput);
            }
        }
    }
}
