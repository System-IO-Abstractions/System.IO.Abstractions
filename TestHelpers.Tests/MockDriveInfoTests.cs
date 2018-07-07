﻿using NUnit.Framework;

namespace System.IO.Abstractions.TestingHelpers.Tests
{
    using XFS = MockUnixSupport;

    [TestFixture]
    public class MockDriveInfoTests
    {
        [TestCase(@"c:")]
        [TestCase(@"c:\")]
        public void MockDriveInfo_Constructor_ShouldInitializeLocalWindowsDrives(string driveName)
        {
            if (XFS.IsUnixPlatform())
            {
                Assert.Inconclusive("Unix does not have the concept of drives.");
            }

            // Arrange
            var fileSystem = new MockFileSystem();
            fileSystem.AddDirectory(XFS.Path(@"c:\Test"));
            var path = XFS.Path(driveName);

            // Act
            var driveInfo = new MockDriveInfo(fileSystem, path);

            // Assert
            Assert.AreEqual(@"C:\", driveInfo.Name);
        }

        [Test]
        public void MockDriveInfo_Constructor_ShouldInitializeLocalWindowsDrives_SpecialForWindows()
        {
            if (XFS.IsUnixPlatform())
            {
                Assert.Inconclusive("Using XFS.Path transform c into c:.");
            }

            // Arrange
            var fileSystem = new MockFileSystem();
            fileSystem.AddDirectory(XFS.Path(@"c:\Test"));

            // Act
            var driveInfo = new MockDriveInfo(fileSystem, "c");

            // Assert
            Assert.AreEqual(@"C:\", driveInfo.Name);
        }

        [TestCase(@"\\unc\share")]
        [TestCase(@"\\unctoo")]
        public void MockDriveInfo_Constructor_ShouldThrowExceptionIfUncPath(string driveName)
        {
            if (XFS.IsUnixPlatform())
            {
                Assert.Inconclusive("Unix does not have the concept of drives.");
            }

            // Arrange
            var fileSystem = new MockFileSystem();

            // Act
            TestDelegate action = () => new MockDriveInfo(fileSystem, XFS.Path(driveName));

            // Assert
            Assert.Throws<ArgumentException>(action);
        }

        [Test]
        public void MockDriveInfo_RootDirectory_ShouldReturnTheDirectoryBase()
        {
            if (XFS.IsUnixPlatform())
            {
                Assert.Inconclusive("Unix does not have the concept of drives.");
            }
            
            // Arrange
            var fileSystem = new MockFileSystem();
            fileSystem.AddDirectory(XFS.Path(@"c:\Test"));
            var driveInfo = new MockDriveInfo(fileSystem, "c:");
            var expectedDirectory = XFS.Path(@"C:\");

            // Act
            var actualDirectory = driveInfo.RootDirectory;

            // Assert
            Assert.AreEqual(expectedDirectory, actualDirectory.FullName);
        }
    }
}
