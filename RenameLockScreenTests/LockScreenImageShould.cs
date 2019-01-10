using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RenameLockScreens;
using RenameLockScreens.Models;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Text;
using System.Collections.Generic;

namespace RenameLockScreenTests {
    [TestClass]
    public class LockScreenImageShould {
        
        [TestMethod]
        public void TrimToDefaultLength() {
            // Arrange
            var creationTime = DateTime.Now.AddHours(-4);
            string myFileName = "1c4b311d47205c46951385c440c80bf12008ed1459b5d0d2b2bd1ec18a102d4e";
            string myPath = Path.Combine("C:\\Users\\PGullas\\Pictures\\downloaded lock screens\\download-test", myFileName);
            byte[] fileContentsBase64 = Encoding.ASCII.GetBytes("iVBORw0KGgoAAAANSUhEUgAAAAQAAAAECAYAAACp8Z5+AAAAE0lEQVR42mP8z8BQz4AEGEkXAADyrgX9UgHC3gAAAABJRU5ErkJggg==");

            var fileData = new MockFileData(fileContentsBase64) { CreationTime = creationTime };
            var fs = new MockFileSystem(new Dictionary<string, MockFileData> 
            {
                {myPath, fileData }
                });

            // FileInfoWrapper fileInfoWrap = new FileInfoWrapper()

            FileInfoBase fi = fs.FileInfo.FromFileName(myPath);

            var myFileInfo = new MockFileInfo(fs, myPath);

            // SystemWrapper.IO.FileInfoWrap fileInfoWrap = new SystemWrapper.IO.FileInfoWrap(myPath);
            LockScreenImageToLog ls = new LockScreenImageToLog(fi, fs);
            string expectedName = "a102d4e";

            

            // act
            string nameToTest = ls.GetFilenameTrimmedDownToLast();
            // assert
            Assert.AreEqual(expectedName, nameToTest);
        }

        [TestMethod]
        public void TrimToGivenLength() {
            // arrange
            var creationTime = DateTime.Now.AddHours(-4);
            string myFileName = "1c4b311d47205c46951385c440c80bf12008ed1459b5d0d2b2bd1ec18a102d4e";
            string myPath = Path.Combine("C:\\Users\\PGullas\\Pictures\\downloaded lock screens\\download-test", myFileName);
            byte[] fileContentsBase64 = Encoding.ASCII.GetBytes("iVBORw0KGgoAAAANSUhEUgAAAAQAAAAECAYAAACp8Z5+AAAAE0lEQVR42mP8z8BQz4AEGEkXAADyrgX9UgHC3gAAAABJRU5ErkJggg==");
            string expectedName = "c18a102d4e";

            var fileData = new MockFileData(fileContentsBase64) { CreationTime = creationTime };
            var fs = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {myPath, fileData }
                });

            FileInfoBase fi = fs.FileInfo.FromFileName(myPath);

            LockScreenImageToLog ls = new LockScreenImageToLog(fi, fs);

            // act
            string nameToTest = ls.GetFilenameTrimmedDownToLast(10);
            // assert
            Assert.AreEqual(expectedName, nameToTest);
        }

        [TestMethod]
        public void CopyThenPointToNewFile() {
            // Arrange
            var fs = new MockFileSystem();
            // need to add trailing slash to a mock foldername, otherwise it gives us "Could not find a part of the path"
            string folder1 = @"c:\folder1";
            string folder2 = @"c:\folder2";
            fs.AddDirectory(folder1);
            fs.AddDirectory(folder2);
            var creationTime = DateTime.Now.AddHours(-4);
            byte[] fileContentsBase64 = Encoding.ASCII.GetBytes("iVBORw0KGgoAAAANSUhEUgAAAAQAAAAECAYAAACp8Z5+AAAAE0lEQVR42mP8z8BQz4AEGEkXAADyrgX9UgHC3gAAAABJRU5ErkJggg==");

            var fileToCopy = new MockFileData(fileContentsBase64) {
                CreationTime = creationTime};
            string filename1 = @"testfile01.jpg";
            string filePath1 = fs.Path.Combine(folder1, filename1);

            fs.AddFile(filePath1, fileToCopy);
            FileInfoBase fi = fs.FileInfo.FromFileName(filePath1);

            LockScreenImageToLog ls = new LockScreenImageToLog(fi, fs);
            // Act

            ls.CopyToFolder(folder2);
            ls.TrimFilenameDownToLast(2);
            // Assert
            Assert.IsTrue(fs.FileExists(filePath1));
            string filePath2 = fs.Path.Combine(folder2, filename1);
            Assert.IsTrue(fs.FileExists(filePath2));
            Assert.AreEqual("01.jpg", ls._fileInfo.Name);
        }

        [TestMethod]
        public void FileInfoBase_CopyTo_ShouldKeepLengthOnCopy() {
            // Arrange
            var fs = new MockFileSystem();
            // need to add trailing slash to a mock foldername, otherwise it gives us "Could not find a part of the path"
            string folder1 = @"c:\folder1";
            string folder2 = @"c:\folder2";
            fs.AddDirectory(folder1);
            fs.AddDirectory(folder2);
            var creationTime = DateTime.Now.AddHours(-4);
            byte[] fileContentsBase64 = Encoding.ASCII.GetBytes("iVBORw0KGgoAAAANSUhEUgAAAAQAAAAECAYAAACp8Z5+AAAAE0lEQVR42mP8z8BQz4AEGEkXAADyrgX9UgHC3gAAAABJRU5ErkJggg==");
            
            var fileToCopy = new MockFileData(fileContentsBase64) {
                CreationTime = creationTime
            };
            string filename1 = @"testfile01.jpg";
            string filePath1 = fs.Path.Combine(folder1, filename1);

            fs.AddFile(filePath1, fileToCopy);
            FileInfoBase fi = fs.FileInfo.FromFileName(filePath1);

            string filename2 = @"copyfile02.jpg";
            string filePath2 = fs.Path.Combine(folder2, filename2);
            // Act
            fi.CopyTo(filePath2);
            FileInfoBase fi2 = new FileInfo(filePath2);
            // Assert
            Assert.AreEqual(fi.Length, fi2.Length);
        }

        [TestMethod]
        public void NotTrimFilenameWhenArgumentIsLargerThanLength() {
            // arrange
            string myFileName = "1c4b311d47205c46951385c440c80bf12008ed1459b5d0d2b2bd1ec18a102d4e";
            string myPath = Path.Combine("C:\\Users\\PGullas\\Pictures\\downloaded lock screens\\download-test", myFileName);
            byte[] fileContentsBase64 = Encoding.ASCII.GetBytes("iVBORw0KGgoAAAANSUhEUgAAAAQAAAAECAYAAACp8Z5+AAAAE0lEQVR42mP8z8BQz4AEGEkXAADyrgX9UgHC3gAAAABJRU5ErkJggg==");
            string expectedName = "1c4b311d47205c46951385c440c80bf12008ed1459b5d0d2b2bd1ec18a102d4e";

            var creationTime = DateTime.Now.AddHours(-4);

            var fileData = new MockFileData(fileContentsBase64) { CreationTime = creationTime };
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {myPath, fileData }
                });

            FileInfoBase fib = fileSystem.FileInfo.FromFileName(myPath);

            LockScreenImageToLog ls = new LockScreenImageToLog(fib, fileSystem);

            // act
            string nameToTest = ls.GetFilenameTrimmedDownToLast(1000);
            // assert
            Assert.AreEqual(expectedName, nameToTest);
        }

        [TestMethod]
        public void AddNewExtensionCorrectly() {
            // Arrange
            string myFileName = "ec18a102d4e";
            string myPath = Path.Combine("C:\\Users\\PGullas\\Pictures\\downloaded lock screens\\download-test", myFileName);
            byte[] fileContentsBase64 = Encoding.ASCII.GetBytes("iVBORw0KGgoAAAANSUhEUgAAAAQAAAAECAYAAACp8Z5+AAAAE0lEQVR42mP8z8BQz4AEGEkXAADyrgX9UgHC3gAAAABJRU5ErkJggg==");
            string expectedName = "ec18a102d4e.jpg";

            var creationTime = DateTime.Now.AddHours(-4);
            var fileData = new MockFileData(fileContentsBase64) { CreationTime = creationTime };
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {myPath, fileData }
                });

            FileInfoBase fi = fileSystem.FileInfo.FromFileName(myPath);

            LockScreenImageToLog ls = new LockScreenImageToLog(fi, fileSystem);

            // Act
            string nameToTest = ls.GetFilenameWithNewExtension();
            // Assert
            Assert.AreEqual(expectedName, nameToTest);
        }

    }
}
