﻿namespace System.IO.Abstractions
{
    public interface IFileSystem
    {
        FileBase File { get; }
        DirectoryBase Directory { get; }
        IFileInfoFactory FileInfo { get; }
        IDirectoryInfoFactory DirectoryInfo { get; }
    }
}