using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using Newtonsoft.Json;
using System.IO.Abstractions;

namespace RenameLockScreens.Models {
    public class LockScreenImageToLog : IEquatable<LockScreenImageToLog> {
        // probably should use FileInfo in here
        // and then use SystemWrapper for unit testing it
        // [JsonIgnore]
        // public string _path { get; set; }

        [JsonIgnore]
        private readonly IFileSystem _fileSystem;

        [JsonIgnore]
        // public FileInfo _fileInfo { get; set; }
        public FileInfoBase _fileInfo { get; set; }

        public string originalFileName { get; set; }
        public long fileSize { get; set; }

        public DateTime lastModifiedTime { get; set; }

        public LockScreenImageToLog() {

        }

        public LockScreenImageToLog(FileInfoBase fi) : this(fi, new FileSystem()) {
        }

        public LockScreenImageToLog(FileInfoBase fi, IFileSystem fileSystem) {
            // _path = filePath;
            _fileInfo = fi;
            originalFileName = _fileInfo.Name; // Path.GetFileName(filePath);
            fileSize = _fileInfo.Length;
            lastModifiedTime = _fileInfo.LastWriteTime;
            _fileSystem = fileSystem;
        }

        public void TrimFilenameDownToLast(int maxCharactersDesiredInFilename = 7) {
            if (_fileSystem.Path.GetFileNameWithoutExtension(_fileInfo.Name).Length > maxCharactersDesiredInFilename) {
                string newName = GetFilenameTrimmedDownToLast(maxCharactersDesiredInFilename);
                // string newPath = Path.Combine(targetFolder, newName);
                // string currentDir = _fileInfo.DirectoryName;
                string newPath = _fileSystem.Path.Combine(_fileInfo.DirectoryName, newName);
                _fileInfo.MoveTo(newPath);
            }
        }

        public string GetFilenameTrimmedDownToLast(int maxCharsDesiredInFilename = 7) {
            string newName = _fileSystem.Path.GetFileNameWithoutExtension(_fileInfo.Name);
            string ext = _fileSystem.Path.GetExtension(_fileInfo.Name);
            if (_fileInfo.Name.Length > maxCharsDesiredInFilename) {
                newName = newName.Substring(newName.Length - maxCharsDesiredInFilename) + ext;
            }
            return newName;
        }

        public void AppendExtensionToFilename(string newExtension = ".jpg") {
            string newPath = GetFilenameWithNewExtension(newExtension);
            _fileInfo.MoveTo(newPath);
        }

        public string GetFilenameWithNewExtension(string newExtension = ".jpg") {

            string newName = _fileInfo.Name;
            if (Path.GetExtension(newName) != newExtension) {
                newName += newExtension;
            }
            return newName;
        }

        public void MoveToFolder(string newFolder) {
            string newPath = _fileSystem.Path.Combine(newFolder, _fileInfo.Name);
            _fileInfo.MoveTo(newPath);
        }

        public string CopyToFolder(string newFolder) {
            string newPath = _fileSystem.Path.Combine(newFolder, _fileInfo.Name);
            _fileInfo.CopyTo(newPath);
            _fileInfo = new FileInfo(newPath);
            return newPath;
        }

        public void PointToNewFile(string newPath) {
            if (_fileSystem.File.Exists(newPath)) {
                _fileInfo = new FileInfo(newPath);
            }
        }

        public void MoveToAspectRatioFolder(string workingFolder) {
            if (IsPortrait()) {
                MoveToFolder(Path.Combine(workingFolder, "portrait"));
            }
            else {
                MoveToFolder(Path.Combine(workingFolder, "landscape"));
            }
        }

        public bool IsPortrait() {
            bool isWider = false;
            
            //Image myImage = Image.FromFile(_fileInfo.FullName);
            //isWider = (myImage.Height > myImage.Width);
            //myImage.Dispose();
            try {
                Image myImage = Image.FromFile(_fileInfo.FullName);
                isWider = (myImage.Height > myImage.Width);
                myImage.Dispose();
            }
            catch (Exception e) {
                Console.WriteLine($"The process failed: {e.ToString()}");
            }
            return isWider;
        }

        public override bool Equals(object obj) {
            return Equals(obj as LockScreenImageToLog);
        }

        public bool Equals(LockScreenImageToLog other) {
            return other != null &&
                   originalFileName == other.originalFileName &&
                   fileSize == other.fileSize;
        }

        public override int GetHashCode() {
            var hashCode = 1755535401;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(originalFileName);
            hashCode = hashCode * -1521134295 + fileSize.GetHashCode();
            return hashCode;
        }
    }
}
