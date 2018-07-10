using System.Security.AccessControl;
using Microsoft.Win32.SafeHandles;

namespace System.IO.Abstractions.TestingHelpers
{
    [Serializable]
    public class MockFileStreamFactory : IFileStreamFactory
    {
        private readonly IMockFileDataAccessor mockFileSystem;

        public MockFileStreamFactory(IMockFileDataAccessor mockFileSystem)
        {
            if (mockFileSystem == null)
            {
                throw new ArgumentNullException("mockFileSystem");
            }

            this.mockFileSystem = mockFileSystem;
        }

        public Stream Create(string path, FileMode mode)
        {
            return new MockFileStream(mockFileSystem, path, GetStreamType(mode));
        }

        public Stream Create(string path, FileMode mode, FileAccess access)
        {
            return new MockFileStream(mockFileSystem, path, GetStreamType(mode, access));
        }

        public Stream Create(string path, FileMode mode, FileAccess access, FileShare share)
        {
            return new MockFileStream(mockFileSystem, path, GetStreamType(mode, access));
        }

        public Stream Create(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize)
        {
            return new MockFileStream(mockFileSystem, path, GetStreamType(mode, access));
        }

        public Stream Create(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize, FileOptions options)
        {
            return new MockFileStream(mockFileSystem, path, GetStreamType(mode, access));
        }

        public Stream Create(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize, bool useAsync)
        {
            return new MockFileStream(mockFileSystem, path, GetStreamType(mode, access));
        }

        public Stream Create(string path, FileMode mode, FileSystemRights rights, FileShare share, int bufferSize, FileOptions options, FileSecurity fileSecurity)
        {
            return new MockFileStream(mockFileSystem, path, GetStreamType(mode));
        }

        public Stream Create(string path, FileMode mode, FileSystemRights rights, FileShare share, int bufferSize, FileOptions options)
        {
            return new MockFileStream(mockFileSystem, path, GetStreamType(mode));
        }

        [Obsolete("This method has been deprecated. Please use new Create(SafeFileHandle handle, FileAccess access) instead. http://go.microsoft.com/fwlink/?linkid=14202")]
        public Stream Create(IntPtr handle, FileAccess access)
        {
            return new MockFileStream(mockFileSystem, handle.ToString(), GetStreamType(FileMode.Append, access));
        }

        [Obsolete("This method has been deprecated. Please use new Create(SafeFileHandle handle, FileAccess access) instead, and optionally make a new SafeFileHandle with ownsHandle=false if needed. http://go.microsoft.com/fwlink/?linkid=14202")]
        public Stream Create(IntPtr handle, FileAccess access, bool ownsHandle)
        {
            return new MockFileStream(mockFileSystem, handle.ToString(), GetStreamType(FileMode.Append, access));
        }

        [Obsolete("This method has been deprecated. Please use new Create(SafeFileHandle handle, FileAccess access, int bufferSize) instead, and optionally make a new SafeFileHandle with ownsHandle=false if needed. http://go.microsoft.com/fwlink/?linkid=14202")]
        public Stream Create(IntPtr handle, FileAccess access, bool ownsHandle, int bufferSize)
        {
            return new MockFileStream(mockFileSystem, handle.ToString(), GetStreamType(FileMode.Append, access));
        }

        [Obsolete("This method has been deprecated. Please use new Create(SafeFileHandle handle, FileAccess access, int bufferSize, bool isAsync) instead, and optionally make a new SafeFileHandle with ownsHandle=false if needed. http://go.microsoft.com/fwlink/?linkid=14202")]
        public Stream Create(IntPtr handle, FileAccess access, bool ownsHandle, int bufferSize, bool isAsync)
        {
            return new MockFileStream(mockFileSystem, handle.ToString(), GetStreamType(FileMode.Append, access));
        }

        public Stream Create(SafeFileHandle handle, FileAccess access)
        {
            return new MockFileStream(mockFileSystem, handle.ToString(), GetStreamType(FileMode.Append, access));
        }

        public Stream Create(SafeFileHandle handle, FileAccess access, int bufferSize)
        {
            return new MockFileStream(mockFileSystem, handle.ToString(), GetStreamType(FileMode.Append, access));
        }

        public Stream Create(SafeFileHandle handle, FileAccess access, int bufferSize, bool isAsync)
        {
            return new MockFileStream(mockFileSystem, handle.ToString(), GetStreamType(FileMode.Append, access));
        }

        private static MockFileStream.StreamType GetStreamType(FileMode mode, FileAccess access = FileAccess.ReadWrite)
        {
            if (access == FileAccess.Read)
            {
                return MockFileStream.StreamType.READ;
            }
            else if (mode == FileMode.Append) 
            {
                return MockFileStream.StreamType.APPEND;
            }
            else
            {
                return MockFileStream.StreamType.WRITE;
            }
        }
    }
}