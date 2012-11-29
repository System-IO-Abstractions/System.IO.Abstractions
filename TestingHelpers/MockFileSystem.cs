﻿using System.Collections.Generic;
using System.Linq;

namespace System.IO.Abstractions.TestingHelpers
{
    public class MockFileSystem : IFileSystem, IMockFileDataAccessor
    {
        readonly IDictionary<string, MockFileData> files;
        readonly FileBase file;
        readonly DirectoryBase directory;
        readonly IFileInfoFactory fileInfoFactory;
        readonly PathBase path;
        readonly IDirectoryInfoFactory directoryInfoFactory;

        public MockFileSystem(IDictionary<string, MockFileData> files)
        {
            this.files = new Dictionary<string, MockFileData>(
                files,
                StringComparer.InvariantCultureIgnoreCase);

            file = new MockFile(this);
            directory = new MockDirectory(this, file);
            fileInfoFactory = new MockFileInfoFactory(this);
            path = new MockPath();
            directoryInfoFactory = new MockDirectoryInfoFactory(this);
        }

        public FileBase File
        {
            get { return file; }
        }

        public DirectoryBase Directory
        {
            get { return directory; }
        }

        public IFileInfoFactory FileInfo
        {
            get { return fileInfoFactory; }
        }

        public PathBase Path
        {
            get { return path; }
        }

        public IDirectoryInfoFactory DirectoryInfo
        {
            get { return directoryInfoFactory; }
        }

        public MockFileData GetFile(string path)
        {
            return FileExists(path) ? files[path] : null;
        }

        public void AddFile(string path, MockFileData mockFile)
        {
            files.Add(path, mockFile);
        }

        public void RemoveFile(string path)
        {
            files.Remove(path);
        }

        public bool FileExists(string path)
        {
            return files.ContainsKey(path);
        }

        public IEnumerable<string> AllPaths
        {
            get { return files.Keys; }
        }

        public IEnumerable<string> AllFiles {
            get { return files.Where(f => !f.Value.IsDirectory).Select(f => f.Key); }
        }

        public IEnumerable<string> AllDirectories {
            get { return files.Where(f => f.Value.IsDirectory).Select(f => f.Key); }
        }
    }
}