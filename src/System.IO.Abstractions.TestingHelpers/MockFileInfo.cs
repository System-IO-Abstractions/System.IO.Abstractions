using System.Runtime.Versioning;
using System.Security.AccessControl;
using System.Text;

namespace System.IO.Abstractions.TestingHelpers
{
    [Serializable]
    public class MockFileInfo : FileInfoBase
    {
        private readonly IMockFileDataAccessor mockFileSystem;
        private string path;
        private string originalPath;

        public MockFileInfo(IMockFileDataAccessor mockFileSystem, string path) : base(mockFileSystem?.FileSystem)
        {
            this.mockFileSystem = mockFileSystem ?? throw new ArgumentNullException(nameof(mockFileSystem));
            this.originalPath = path ?? throw new ArgumentNullException(nameof(path));
            this.path = mockFileSystem.Path.GetFullPath(path);

        }

        MockFileData MockFileData
        {
            get { return mockFileSystem.GetFile(path); }
        }

        public override void Delete()
        {
            mockFileSystem.RemoveFile(path);
        }

        public override void Refresh()
        {
            // Nothing to do here. Mock file system is always up-to-date.
        }

        public override FileAttributes Attributes
        {
            get
            {
                if (MockFileData == null)
                {
                    throw CommonExceptions.FileNotFound(path);
                }
                return MockFileData.Attributes;
            }
            set
            {
                if (MockFileData == null)
                {
                    throw CommonExceptions.FileNotFound(path);
                }
                MockFileData.Attributes = value;
            }
        }

        public override DateTime CreationTime
        {
            get
            {
                if (MockFileData == null)
                {
                    throw CommonExceptions.FileNotFound(path);
                }
                return MockFileData.CreationTime.DateTime;
            }
            set
            {
                if (MockFileData == null)
                {
                    throw CommonExceptions.FileNotFound(path);
                }
                MockFileData.CreationTime = value;
            }
        }

        public override DateTime CreationTimeUtc
        {
            get
            {
                if (MockFileData == null) throw CommonExceptions.FileNotFound(path);
                return MockFileData.CreationTime.UtcDateTime;
            }
            set
            {
                if (MockFileData == null) throw CommonExceptions.FileNotFound(path);
                MockFileData.CreationTime = value.ToLocalTime();
            }
        }

        public override bool Exists
        {
            get { return MockFileData != null && !MockFileData.IsDirectory; }
        }

        public override string Extension
        {
            get
            {
                // System.IO.Path.GetExtension does only string manipulation,
                // so it's safe to delegate.
                return Path.GetExtension(path);
            }
        }

        public override string FullName
        {
            get { return path; }
        }

        public override DateTime LastAccessTime
        {
            get
            {
                if (MockFileData == null) throw CommonExceptions.FileNotFound(path);
                return MockFileData.LastAccessTime.DateTime;
            }
            set
            {
                if (MockFileData == null) throw CommonExceptions.FileNotFound(path);
                MockFileData.LastAccessTime = value;
            }
        }

        public override DateTime LastAccessTimeUtc
        {
            get
            {
                if (MockFileData == null) throw CommonExceptions.FileNotFound(path);
                return MockFileData.LastAccessTime.UtcDateTime;
            }
            set
            {
                if (MockFileData == null) throw CommonExceptions.FileNotFound(path);
                MockFileData.LastAccessTime = value;
            }
        }

        public override DateTime LastWriteTime
        {
            get
            {
                if (MockFileData == null) throw CommonExceptions.FileNotFound(path);
                return MockFileData.LastWriteTime.DateTime;
            }
            set
            {
                if (MockFileData == null) throw CommonExceptions.FileNotFound(path);
                MockFileData.LastWriteTime = value;
            }
        }

        public override DateTime LastWriteTimeUtc
        {
            get
            {
                if (MockFileData == null) throw CommonExceptions.FileNotFound(path);
                return MockFileData.LastWriteTime.UtcDateTime;
            }
            set
            {
                if (MockFileData == null) throw CommonExceptions.FileNotFound(path);
                MockFileData.LastWriteTime = value.ToLocalTime();
            }
        }

        public override string Name
        {
            get { return new MockPath(mockFileSystem).GetFileName(path); }
        }

        public override StreamWriter AppendText()
        {
            return new StreamWriter(new MockFileStream(mockFileSystem, FullName, FileMode.Append));
        }

        public override IFileInfo CopyTo(string destFileName)
        {
            return CopyTo(destFileName, false);
        }

        public override IFileInfo CopyTo(string destFileName, bool overwrite)
        {
            if (!Exists)
            {
                if (MockFileData == null) throw CommonExceptions.FileNotFound(FullName);
            }
            if (destFileName == FullName)
            {
                return this;
            }
            mockFileSystem.File.Copy(FullName, destFileName, overwrite);
            return mockFileSystem.FileInfo.FromFileName(destFileName);
        }

        public override Stream Create()
        {
            return new MockFile(mockFileSystem).Create(FullName);
        }

        public override StreamWriter CreateText()
        {
            return new MockFile(mockFileSystem).CreateText(FullName);
        }

        public override void Decrypt()
        {
            if (MockFileData == null) throw CommonExceptions.FileNotFound(path);

            MockFileData.Attributes &= ~FileAttributes.Encrypted;
        }

        public override void Encrypt()
        {
            if (MockFileData == null) throw CommonExceptions.FileNotFound(path);

            MockFileData.Attributes |= FileAttributes.Encrypted;
        }

        [SupportedOSPlatform("windows")]
        public override FileSecurity GetAccessControl()
        {
            return mockFileSystem.File.GetAccessControl(this.path);
        }

        [SupportedOSPlatform("windows")]
        public override FileSecurity GetAccessControl(AccessControlSections includeSections)
        {
            return mockFileSystem.File.GetAccessControl(this.path, includeSections);
        }

        public override void MoveTo(string destFileName)
        {
            if (destFileName != FullName && Path.GetDirectoryName(destFileName) == DirectoryName
                && destFileName.ToLowerInvariant() == FullName.ToLowerInvariant())
            {
                var tempPath = GetSafeTempName(destFileName);
                MoveTo(tempPath);
            }

            var movedFileInfo = CopyTo(destFileName);
            if (destFileName == FullName)
            {
                return;
            }
            Delete();
            path = movedFileInfo.FullName;
        }

        private string GetSafeTempName(string outputFilePath)
        {
            var stb = new StringBuilder(outputFilePath);
            stb.Append('_');
            while (FileSystem.File.Exists(stb.ToString()))
            {
                stb.Append('_');
            }
            return stb.ToString();
        }

#if FEATURE_FILE_MOVE_WITH_OVERWRITE
        public override void MoveTo(string destFileName, bool overwrite)
        {
            var movedFileInfo = CopyTo(destFileName, overwrite);
            if (destFileName == FullName)
            {
                return;
            }
            Delete();
            path = movedFileInfo.FullName;
        }
#endif

        public override Stream Open(FileMode mode)
        {
            return new MockFile(mockFileSystem).Open(FullName, mode);
        }

        public override Stream Open(FileMode mode, FileAccess access)
        {
            return new MockFile(mockFileSystem).Open(FullName, mode, access);
        }

        public override Stream Open(FileMode mode, FileAccess access, FileShare share)
        {
            return new MockFile(mockFileSystem).Open(FullName, mode, access, share);
        }

        public override Stream OpenRead() => mockFileSystem.File.OpenRead(path);

        public override StreamReader OpenText() => mockFileSystem.File.OpenText(path);

        public override Stream OpenWrite() => mockFileSystem.File.OpenWrite(path);

        public override IFileInfo Replace(string destinationFileName, string destinationBackupFileName)
        {
            return Replace(destinationFileName, destinationBackupFileName, false);
        }

        public override IFileInfo Replace(string destinationFileName, string destinationBackupFileName, bool ignoreMetadataErrors)
        {
            mockFileSystem.File.Replace(path, destinationFileName, destinationBackupFileName, ignoreMetadataErrors);
            return mockFileSystem.FileInfo.FromFileName(destinationFileName);
        }

        [SupportedOSPlatform("windows")]
        public override void SetAccessControl(FileSecurity fileSecurity)
        {
            mockFileSystem.File.SetAccessControl(this.path, fileSecurity);
        }

        public override IDirectoryInfo Directory
        {
            get
            {
                return mockFileSystem.DirectoryInfo.FromDirectoryName(DirectoryName);
            }
        }

        public override string DirectoryName
        {
            get
            {
                // System.IO.Path.GetDirectoryName does only string manipulation,
                // so it's safe to delegate.
                return Path.GetDirectoryName(path);
            }
        }

        public override bool IsReadOnly
        {
            get
            {
                if (MockFileData == null)
                {
                    throw CommonExceptions.FileNotFound(path);
                }
                return (MockFileData.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly;
            }
            set
            {
                if (MockFileData == null)
                {
                    throw CommonExceptions.FileNotFound(path);
                }
                if (value)
                {
                    MockFileData.Attributes |= FileAttributes.ReadOnly;
                }
                else
                {
                    MockFileData.Attributes &= ~FileAttributes.ReadOnly;
                }
            }
        }

        public override long Length
        {
            get
            {
                if (MockFileData == null || MockFileData.IsDirectory)
                {
                    throw CommonExceptions.FileNotFound(path);
                }
                return MockFileData.Contents.Length;
            }
        }

        public override string ToString()
        {
            return originalPath;
        }
    }
}
