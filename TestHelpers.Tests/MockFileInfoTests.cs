﻿using System.Collections.Generic;
using NUnit.Framework;

namespace System.IO.Abstractions.TestingHelpers.Tests
{
    [TestFixture]
    public class MockFileInfoTests
    {
        [Test]
        public void MockFileInfo_Exists_ShouldReturnTrueIfFileExistsInMemoryFileSystem()
        {
            // Arrange
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"c:\a.txt", new MockFileData("Demo text content") },
                { @"c:\a\b\c.txt", new MockFileData("Demo text content") },
            });
            var fileInfo = new MockFileInfo(fileSystem, @"c:\a.txt");

            // Act
            var result = fileInfo.Exists;

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void MockFileInfo_Exists_ShouldReturnFalseIfFileDoesNotExistInMemoryFileSystem()
        {
            // Arrange
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"c:\a.txt", new MockFileData("Demo text content") },
                { @"c:\a\b\c.txt", new MockFileData("Demo text content") },
            });
            var fileInfo = new MockFileInfo(fileSystem, @"c:\foo.txt");

            // Act
            var result = fileInfo.Exists;

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void MockFileInfo_Length_ShouldReturnLengthOfFileInMemoryFileSystem()
        {
            // Arrange
            const string fileContent = "Demo text content";
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"c:\a.txt", new MockFileData(fileContent) },
                { @"c:\a\b\c.txt", new MockFileData(fileContent) },
            });
            var fileInfo = new MockFileInfo(fileSystem, @"c:\a.txt");

            // Act
            var result = fileInfo.Length;

            // Assert
            Assert.AreEqual(fileContent.Length, result);
        }

        [Test]
        public void MockFileInfo_Length_ShouldThrowFileNotFoundExcpetionIfFileDoesNotExistInMemoryFileSystem()
        {
            // Arrange
            const string fileContent = "Demo text content";
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"c:\a.txt", new MockFileData(fileContent) },
                { @"c:\a\b\c.txt", new MockFileData(fileContent) },
            });
            var fileInfo = new MockFileInfo(fileSystem, @"c:\foo.txt");

            try
            {
                // Act
                fileInfo.Length.ToString();

                // Assert
                Assert.Fail("Expected exception was not thrown");
            }
            catch (FileNotFoundException ex)
            {
                // Assert
                Assert.AreEqual(@"c:\foo.txt", ex.FileName);
            }
        }

        [Test]
        public void MockFileInfo_CreationTimeUtc_ShouldReturnCreationTimeUtcOfFileInMemoryFileSystem()
        {
            // Arrange
            var creationTime = DateTime.Now.AddHours(-4);
            var fileData = new MockFileData("Demo text content") { CreationTime = creationTime };
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"c:\a.txt", fileData }
            });
            var fileInfo = new MockFileInfo(fileSystem, @"c:\a.txt");

            // Act
            var result = fileInfo.CreationTimeUtc;

            // Assert
            Assert.AreEqual(creationTime.ToUniversalTime(), result);
        }

        [Test]
        public void MockFileInfo_LastAccessTimeUtc_ShouldReturnLastAccessTimeUtcOfFileInMemoryFileSystem()
        {
            // Arrange
            var lastAccessTime = DateTime.Now.AddHours(-4);
            var fileData = new MockFileData("Demo text content") { LastAccessTime = lastAccessTime };
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"c:\a.txt", fileData }
            });
            var fileInfo = new MockFileInfo(fileSystem, @"c:\a.txt");

            // Act
            var result = fileInfo.LastAccessTimeUtc;

            // Assert
            Assert.AreEqual(lastAccessTime.ToUniversalTime(), result);
        }

        [Test]
        public void MockFileInfo_LastWriteTimeUtc_ShouldReturnLastWriteTimeUtcOfFileInMemoryFileSystem()
        {
            // Arrange
            var lastWriteTime = DateTime.Now.AddHours(-4);
            var fileData = new MockFileData("Demo text content") { LastWriteTime = lastWriteTime };
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"c:\a.txt", fileData }
            });
            var fileInfo = new MockFileInfo(fileSystem, @"c:\a.txt");

            // Act
            var result = fileInfo.LastWriteTimeUtc;

            // Assert
            Assert.AreEqual(lastWriteTime.ToUniversalTime(), result);
        }

        [Test]
        public void CreateText_DataSentIn_ShouldWriteToFile()
        {
            //Arrange
            var fileData = new MockFileData("Demo text content");
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"c:\a.txt", fileData }
            });

            var fileInfo = new MockFileInfo(fileSystem, @"c:\a.txt");

            //Act
            var writer = fileInfo.CreateText();
            writer.WriteLine("test");
            writer.Close();

            var reader = fileInfo.OpenText();
            var result = reader.ReadToEnd();

            //Assert
            Assert.AreEqual("test",result);
        }
    }
}