﻿using System.Collections.Generic;
using System.Security.AccessControl;
using System.Text;
using System.IO;

namespace System.IO.Abstractions
{
#if NET40
    [Serializable]
#endif
    public class FileWrapper : FileBase
    {
        public override void AppendAllLines(string path, IEnumerable<string> contents)
        {
            File.AppendAllLines(path, contents);
        }

        public override void AppendAllLines(string path, IEnumerable<string> contents, Encoding encoding)
        {
            File.AppendAllLines(path, contents, encoding);
        }

        public override void AppendAllText(string path, string contents)
        {
            File.AppendAllText(path, contents);
        }

        public override void AppendAllText(string path, string contents, Encoding encoding)
        {
            File.AppendAllText(path, contents, encoding);
        }

        public override StreamWriter AppendText(string path)
        {
            return File.AppendText(path);
        }

        public override void Copy(string sourceFileName, string destFileName)
        {
            File.Copy(sourceFileName, destFileName);
        }

        public override void Copy(string sourceFileName, string destFileName, bool overwrite)
        {
            File.Copy(sourceFileName, destFileName, overwrite);
        }

        public override Stream Create(string path)
        {
            return File.Create(path);
        }

        public override Stream Create(string path, int bufferSize)
        {
            return File.Create(path, bufferSize);
        }

        public override Stream Create(string path, int bufferSize, FileOptions options)
        {
            return File.Create(path, bufferSize, options);
        }

#if NET40
        public override Stream Create(string path, int bufferSize, FileOptions options, FileSecurity fileSecurity)
        {
            return File.Create(path, bufferSize, options, fileSecurity);
        }
#endif

        public override StreamWriter CreateText(string path)
        {
            return File.CreateText(path);
        }

#if NET40
        public override void Decrypt(string path)
        {
            File.Decrypt(path);
        }
#endif

        public override void Delete(string path)
        {
            File.Delete(path);
        }

#if NET40
        public override void Encrypt(string path)
        {
            File.Encrypt(path);
        }
#endif

        public override bool Exists(string path)
        {
            return File.Exists(path);
        }

        public override FileSecurity GetAccessControl(string path)
        {
#if NET40
            return File.GetAccessControl(path);
#elif DOTNET5_4
            var info = new FileInfo(path);
            return info.GetAccessControl();
#endif
        }

        public override FileSecurity GetAccessControl(string path, AccessControlSections includeSections)
        {
#if NET40
            return File.GetAccessControl(path, includeSections);
#elif DOTNET5_4
            var info = new FileInfo(path);
            return info.GetAccessControl(includeSections);
#endif
        }

        /// <summary>
        /// Gets the <see cref="FileAttributes"/> of the file on the path.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <returns>The <see cref="FileAttributes"/> of the file on the path.</returns>
        /// <exception cref="ArgumentException"><paramref name="path"/> is empty, contains only white spaces, or contains invalid characters.</exception>
        /// <exception cref="PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.</exception>
        /// <exception cref="NotSupportedException"><paramref name="path"/> is in an invalid format.</exception>
        /// <exception cref="FileNotFoundException"><paramref name="path"/> represents a file and is invalid, such as being on an unmapped drive, or the file cannot be found.</exception>
        /// <exception cref="DirectoryNotFoundException"><paramref name="path"/> represents a directory and is invalid, such as being on an unmapped drive, or the directory cannot be found.</exception>
        /// <exception cref="IOException">This file is being used by another process.</exception>
        /// <exception cref="UnauthorizedAccessException">The caller does not have the required permission.</exception>
        public override FileAttributes GetAttributes(string path)
        {
            return File.GetAttributes(path);
        }

        public override DateTime GetCreationTime(string path)
        {
            return File.GetCreationTime(path);
        }

        public override DateTime GetCreationTimeUtc(string path)
        {
            return File.GetCreationTimeUtc(path);
        }

        public override DateTime GetLastAccessTime(string path)
        {
            return File.GetLastAccessTime(path);
        }

        public override DateTime GetLastAccessTimeUtc(string path)
        {
            return File.GetLastAccessTimeUtc(path);
        }

        public override DateTime GetLastWriteTime(string path)
        {
            return File.GetLastWriteTime(path);
        }

        public override DateTime GetLastWriteTimeUtc(string path)
        {
            return File.GetLastWriteTimeUtc(path);
        }

        public override void Move(string sourceFileName, string destFileName)
        {
            File.Move(sourceFileName, destFileName);
        }

        public override Stream Open(string path, FileMode mode)
        {
            return File.Open(path, mode);
        }

        public override Stream Open(string path, FileMode mode, FileAccess access)
        {
            return File.Open(path, mode, access);
        }

        public override Stream Open(string path, FileMode mode, FileAccess access, FileShare share)
        {
            return File.Open(path, mode, access, share);
        }

        public override Stream OpenRead(string path)
        {
            return File.OpenRead(path);
        }

        public override StreamReader OpenText(string path)
        {
            return File.OpenText(path);
        }

        public override Stream OpenWrite(string path)
        {
            return File.OpenWrite(path);
        }

        public override byte[] ReadAllBytes(string path)
        {
            return File.ReadAllBytes(path);
        }

        public override string[] ReadAllLines(string path)
        {
            return File.ReadAllLines(path);
        }

        public override string[] ReadAllLines(string path, Encoding encoding)
        {
            return File.ReadAllLines(path, encoding);
        }

        public override string ReadAllText(string path)
        {
            return File.ReadAllText(path);
        }

        public override string ReadAllText(string path, Encoding encoding)
        {
            return File.ReadAllText(path, encoding);
        }

        public override IEnumerable<string> ReadLines(string path)
        {
            return File.ReadLines(path);
        }

        public override IEnumerable<string> ReadLines(string path, Encoding encoding)
        {
            return File.ReadLines(path, encoding);
        }

        public override void Replace(string sourceFileName, string destinationFileName, string destinationBackupFileName)
        {
#if NET40
            File.Replace(sourceFileName, destinationFileName, destinationBackupFileName);
#elif DOTNET5_4
            File.Copy(destinationFileName, destinationBackupFileName);
            File.Move(sourceFileName, destinationFileName);
#endif
        }

        public override void Replace(string sourceFileName, string destinationFileName, string destinationBackupFileName, bool ignoreMetadataErrors)
        {
#if NET40
            File.Replace(sourceFileName, destinationFileName, destinationBackupFileName, ignoreMetadataErrors);
#elif DOTNET5_4
            File.Copy(destinationFileName, destinationBackupFileName);
            File.Move(sourceFileName, destinationFileName);
#endif
        }

        public override void SetAccessControl(string path, FileSecurity fileSecurity)
        {
#if NET40
            File.SetAccessControl(path, fileSecurity);
#elif DOTNET5_4
            var info = new FileInfo(path);
            info.SetAccessControl(fileSecurity);
#endif
        }

        public override void SetAttributes(string path, FileAttributes fileAttributes)
        {
            File.SetAttributes(path, fileAttributes);
        }

        public override void SetCreationTime(string path, DateTime creationTime)
        {
            File.SetCreationTime(path, creationTime);
        }

        public override void SetCreationTimeUtc(string path, DateTime creationTimeUtc)
        {
            File.SetCreationTimeUtc(path, creationTimeUtc);
        }

        public override void SetLastAccessTime(string path, DateTime lastAccessTime)
        {
            File.SetLastAccessTime(path, lastAccessTime);
        }

        public override void SetLastAccessTimeUtc(string path, DateTime lastAccessTimeUtc)
        {
            File.SetLastAccessTimeUtc(path, lastAccessTimeUtc);
        }

        public override void SetLastWriteTime(string path, DateTime lastWriteTime)
        {
            File.SetLastWriteTime(path, lastWriteTime);
        }

        public override void SetLastWriteTimeUtc(string path, DateTime lastWriteTimeUtc)
        {
            File.SetLastWriteTimeUtc(path, lastWriteTimeUtc);
        }

        public override void WriteAllBytes(string path, byte[] bytes)
        {
            File.WriteAllBytes(path, bytes);
        }

        public override void WriteAllLines(string path, IEnumerable<string> contents)
        {
            File.WriteAllLines(path, contents);
        }

        public override void WriteAllLines(string path, IEnumerable<string> contents, Encoding encoding)
        {
            File.WriteAllLines(path, contents, encoding);
        }

        public override void WriteAllLines(string path, string[] contents)
        {
            File.WriteAllLines(path, contents);
        }

        public override void WriteAllLines(string path, string[] contents, Encoding encoding)
        {
            File.WriteAllLines(path, contents, encoding);
        }

        public override void WriteAllText(string path, string contents)
        {
            File.WriteAllText(path, contents);
        }

        public override void WriteAllText(string path, string contents, Encoding encoding)
        {
            File.WriteAllText(path, contents, encoding);
        }
    }
}