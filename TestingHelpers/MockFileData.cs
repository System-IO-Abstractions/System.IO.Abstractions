﻿using System.Linq;
using System.Text;

namespace System.IO.Abstractions.TestingHelpers
{
    /// <summary>
    /// The class represents the associated data of a file.
    /// </summary>
    [Serializable]
    public class MockFileData
    {
        /// <summary>
        /// The default encoding.
        /// </summary>
        public static readonly Encoding DefaultEncoding = new UTF8Encoding(false, true);

        /// <summary>
        /// The null object.
        /// </summary>
        public static readonly MockFileData NullObject = new MockFileData(string.Empty) {
          LastWriteTime = new DateTime(1601, 01, 01, 00, 00, 00, DateTimeKind.Utc),
          LastAccessTime = new DateTime(1601, 01, 01, 00, 00, 00, DateTimeKind.Utc),
          CreationTime = new DateTime(1601, 01, 01, 00, 00, 00, DateTimeKind.Utc),
        };

        /// <summary>
        /// The actual contents of the file.
        /// </summary>
        private byte[] contents;
        private readonly Func<byte[]> lazyLoadContent;

        /// <summary>
        /// The date and time the <see cref="MockFileData"/> was created.
        /// </summary>
        private DateTimeOffset creationTime = new DateTimeOffset(2010, 01, 02, 00, 00, 00, TimeSpan.FromHours(4));

        /// <summary>
        /// The date and time of the <see cref="MockFileData"/> was last accessed to.
        /// </summary>
        private DateTimeOffset lastAccessTime = new DateTimeOffset(2010, 02, 04, 00, 00, 00, TimeSpan.FromHours(4));

        /// <summary>
        /// The date and time of the <see cref="MockFileData"/> was last written to.
        /// </summary>
        private DateTimeOffset lastWriteTime = new DateTimeOffset(2010, 01, 04, 00, 00, 00, TimeSpan.FromHours(4));

        /// <summary>
        /// The attributes of the <see cref="MockFileData"/>.
        /// </summary>
        private FileAttributes attributes = FileAttributes.Normal;

        /// <summary>
        /// Gets a value indicating whether the <see cref="MockFileData"/> is a directory or not.
        /// </summary>
        public virtual bool IsDirectory { get { return false; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="MockFileData"/> class with an empty content.
        /// </summary>
        private MockFileData()
        {
            // empty
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MockFileData"/> class with the content of <paramref name="textContents"/> using the encoding of <see cref="DefaultEncoding"/>.
        /// </summary>
        /// <param name="textContents">The textual content encoded into bytes with <see cref="DefaultEncoding"/>.</param>
        public MockFileData(string textContents)
            : this(DefaultEncoding.GetBytes(textContents))
        {}

        /// <summary>
        /// Initializes a new instance of the <see cref="MockFileData"/> class with the content of <paramref name="textContents"/> using the encoding of <paramref name="encoding"/>.
        /// </summary>
        /// <param name="textContents">The textual content.</param>
        /// <param name="encoding">The specific encoding used the encode the text.</param>
        /// <remarks>The constructor respect the BOM of <paramref name="encoding"/>.</remarks>
        public MockFileData(string textContents, Encoding encoding)
            : this()
        {
            contents = encoding.GetPreamble().Concat(encoding.GetBytes(textContents)).ToArray();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MockFileData"/> class with the content of <paramref name="contents"/>.
        /// </summary>
        /// <param name="contents">The actual content.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="contents"/> is <see langword="null" />.</exception>
        public MockFileData(byte[] contents)
        {
            if (contents == null)
            {
                throw new ArgumentNullException("contents");
            }

            this.contents = contents;
        }

        /// <summary>
        /// Allows insertion of a file where the content is lazy loaded through a Func.
        /// </summary>
        /// <param name="lazyLoadContent">Func to return the file's content the first time it is requested.</param>
        public MockFileData(Func<byte[]> lazyLoadContent)
        {
            if (lazyLoadContent == null)
            {
                throw new ArgumentNullException("lazyLoadContent");
            }
            this.lazyLoadContent = lazyLoadContent;
        }

        /// <summary>
        /// Allows insertion of a FileSystemInfoBase object into a new file system. Attributes and time stamps
        /// are copied, and the content will be lazy loaded the first time it is touched.
        /// </summary>
        /// <param name="fileSystem">The source file system where the file info originates</param>
        /// <param name="fileInfo">The source file to be copied</param>
        public MockFileData(IFileSystem fileSystem, FileSystemInfoBase fileInfo)
        {
            if (fileSystem == null)
            {
                throw new ArgumentNullException("fileSystem");
            }
            if (fileInfo == null)
            {
                throw new ArgumentNullException("fileInfo");
            }
            creationTime = fileInfo.CreationTime;
            lastAccessTime = fileInfo.LastAccessTime;
            lastWriteTime = fileInfo.LastWriteTime;
            attributes = fileInfo.Attributes;
            lazyLoadContent = () => fileSystem.File.ReadAllBytes(fileInfo.FullName);
        }

        /// <summary>
        /// Gets or sets the byte contents of the <see cref="MockFileData"/>.
        /// </summary> 
        public byte[] Contents
        {
            get { return contents ?? (contents = lazyLoadContent()); }
            set { contents = value; }
        }

        /// <summary>
        /// Gets or sets the string contents of the <see cref="MockFileData"/>.
        /// </summary>
        /// <remarks>
        /// The setter uses the <see cref="DefaultEncoding"/> using this can scramble the actual contents.
        /// </remarks>
        public string TextContents
        {
            get { return MockFile.ReadAllBytes(contents, DefaultEncoding); }
            set { contents = DefaultEncoding.GetBytes(value); }
        }

        /// <summary>
        /// Gets or sets the date and time the <see cref="MockFileData"/> was created.
        /// </summary>
        public DateTimeOffset CreationTime
        {
            get { return creationTime; }
            set { creationTime = value; }
        }

        /// <summary>
        /// Gets or sets the date and time of the <see cref="MockFileData"/> was last accessed to.
        /// </summary>
        public DateTimeOffset LastAccessTime
        {
            get { return lastAccessTime; }
            set { lastAccessTime = value; }
        }

        /// <summary>
        /// Gets or sets the date and time of the <see cref="MockFileData"/> was last written to.
        /// </summary>
        public DateTimeOffset LastWriteTime
        {
            get { return lastWriteTime; }
            set { lastWriteTime = value; }
        }

        /// <summary>
        /// Casts a string into <see cref="MockFileData"/>.
        /// </summary>
        /// <param name="s">The path of the <see cref="MockFileData"/> to be created.</param>
        public static implicit operator MockFileData(string s)
        {
            return new MockFileData(s);
        }

        /// <summary>
        /// Gets or sets the specified <see cref="FileAttributes"/> of the <see cref="MockFileData"/>.
        /// </summary>
        public FileAttributes Attributes
        {
            get { return attributes; }
            set { attributes = value; }
        }
    }
}