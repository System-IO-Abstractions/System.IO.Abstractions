﻿using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace System.IO.Abstractions.TestingHelpers.Tests
{
    using XFS = MockUnixSupport;

    public class MockDirectoryInfoTests
    {
        [Fact]
        public void MockDirectoryInfo_GetExtension_ShouldReturnEmptyString()
        {
            // Arrange
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>());
            var directoryInfo = new MockDirectoryInfo(fileSystem, XFS.Path(@"c:\temp"));

            // Act
            var result = directoryInfo.Extension;

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void MockDirectoryInfo_GetExtensionWithTrailingSlash_ShouldReturnEmptyString()
        {
            // Arrange
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>());
            var directoryInfo = new MockDirectoryInfo(fileSystem, XFS.Path(@"c:\temp\"));

            // Act
            var result = directoryInfo.Extension;

            // Assert
            Assert.Equal(string.Empty, result);
        }

        public static IEnumerable<object[]> MockDirectoryInfo_Exists_Cases
        {
            get
            {
                yield return new object[]{ XFS.Path(@"c:\temp\folder"), true };
                yield return new object[]{ XFS.Path(@"c:\temp\folder\notExistant"), false };
            }
        }

        [TestCaseSource("MockDirectoryInfo_Exists_Cases")]
        public void MockDirectoryInfo_Exists(string path, bool expected) 
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> 
            {
                {XFS.Path(@"c:\temp\folder\file.txt"), new MockFileData("Hello World")}
            });
            var directoryInfo = new MockDirectoryInfo(fileSystem, path);

            var result = directoryInfo.Exists;

            Assert.Equal(expected, result);
        }
  
        [Fact]
        public void MockDirectoryInfo_FullName_ShouldReturnFullNameWithoutIncludingTrailingPathDelimiter() 
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {
                    XFS.Path(@"c:\temp\folder\file.txt"),
                        new MockFileData("Hello World")
                }
            });
            var directoryInfo = new MockDirectoryInfo(fileSystem, XFS.Path(@"c:\temp\folder"));

            var result = directoryInfo.FullName;

            Assert.Equal(XFS.Path(@"c:\temp\folder"), result);
        }

        [Fact]
        public void MockDirectoryInfo_GetFileSystemInfos_ShouldReturnBothDirectoriesAndFiles()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { XFS.Path(@"c:\temp\folder\file.txt"), new MockFileData("Hello World") },
                { XFS.Path(@"c:\temp\folder\folder"), new MockDirectoryData() }
            });

            var directoryInfo = new MockDirectoryInfo(fileSystem, XFS.Path(@"c:\temp\folder"));
            var result = directoryInfo.GetFileSystemInfos();

            Assert.Equal(2, result.Length);
        }

        [Fact]
        public void MockDirectoryInfo_EnumerateFileSystemInfos_ShouldReturnBothDirectoriesAndFiles()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { XFS.Path(@"c:\temp\folder\file.txt"), new MockFileData("Hello World") },
                { XFS.Path(@"c:\temp\folder\folder"), new MockDirectoryData() }
            });

            var directoryInfo = new MockDirectoryInfo(fileSystem, XFS.Path(@"c:\temp\folder"));
            var result = directoryInfo.EnumerateFileSystemInfos().ToArray();

            Assert.Equal(2, result.Length);
        }

        [Fact]
        public void MockDirectoryInfo_GetFileSystemInfos_ShouldReturnDirectoriesAndNamesWithSearchPattern()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { XFS.Path(@"c:\temp\folder\file.txt"), new MockFileData("Hello World") },
                { XFS.Path(@"c:\temp\folder\folder"), new MockDirectoryData() },
                { XFS.Path(@"c:\temp\folder\older"), new MockDirectoryData() }
            });

            var directoryInfo = new MockDirectoryInfo(fileSystem, XFS.Path(@"c:\temp\folder"));
            var result = directoryInfo.GetFileSystemInfos("f*");

            Assert.Equal(2, result.Length);
        }

        [Fact]
        public void MockDirectoryInfo_EnumerateFileSystemInfos_ShouldReturnDirectoriesAndNamesWithSearchPattern()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { XFS.Path(@"c:\temp\folder\file.txt"), new MockFileData("Hello World") },
                { XFS.Path(@"c:\temp\folder\folder"), new MockDirectoryData() },
                { XFS.Path(@"c:\temp\folder\older"), new MockDirectoryData() }
            });

            var directoryInfo = new MockDirectoryInfo(fileSystem, XFS.Path(@"c:\temp\folder"));
            var result = directoryInfo.EnumerateFileSystemInfos("f*", SearchOption.AllDirectories).ToArray();

            Assert.Equal(2, result.Length);
        }

        [Fact]
        public void MockDirectoryInfo_GetParent_ShouldReturnDirectoriesAndNamesWithSearchPattern()
        {
            // Arrange
            var fileSystem = new MockFileSystem();
            fileSystem.AddDirectory(XFS.Path(@"c:\a\b\c"));
            var directoryInfo = new MockDirectoryInfo(fileSystem, XFS.Path(@"c:\a\b\c"));

            // Act
            var result = directoryInfo.Parent;

            // Assert
            Assert.Equal(XFS.Path(@"c:\a\b"), result.FullName);
        }

        [Fact]
        public void MockDirectoryInfo_EnumerateFiles_ShouldReturnAllFiles()
        {
          // Arrange
          var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                //Files "above" in folder we're querying
                { XFS.Path(@"c:\temp\a.txt"), "" },

                //Files in the folder we're querying
                { XFS.Path(@"c:\temp\folder\b.txt"), "" },
                { XFS.Path(@"c:\temp\folder\c.txt"), "" },

                //Files "below" the folder we're querying
                { XFS.Path(@"c:\temp\folder\deeper\d.txt"), "" }
            });

          // Act
          var directoryInfo = new MockDirectoryInfo(fileSystem, XFS.Path(@"c:\temp\folder"));

          // Assert
          Assert.Equal(new[]{"b.txt", "c.txt"}, directoryInfo.EnumerateFiles().ToList().Select(x => x.Name).ToArray());
        }

        [Fact]
        public void MockDirectoryInfo_EnumerateDirectories_ShouldReturnAllDirectories()
        {
            // Arrange
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                //A file we want to ignore entirely
                { XFS.Path(@"c:\temp\folder\a.txt"), "" },

                //Some files in sub folders (which we also want to ignore entirely)
                { XFS.Path(@"c:\temp\folder\b\file.txt"), "" },
                { XFS.Path(@"c:\temp\folder\c\other.txt"), "" },
            });
            var directoryInfo = new MockDirectoryInfo(fileSystem, XFS.Path(@"c:\temp\folder"));

            // Act
            var directories = directoryInfo.EnumerateDirectories().Select(a => a.Name).ToArray();

            // Assert
            Assert.Equal(new[] { "b", "c" }, directories);
        }
    }
}