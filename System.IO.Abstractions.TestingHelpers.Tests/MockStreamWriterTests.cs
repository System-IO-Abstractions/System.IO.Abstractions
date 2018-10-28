﻿namespace System.IO.Abstractions.TestingHelpers.Tests
{
    using System.Collections.Generic;

    using NUnit.Framework;

    using XFS = MockUnixSupport;

    [TestFixture]
    public class MockStreamWriterTests
    {
        [Test]
        public void MockStreamWriter_WriteLine_String()
        {
            // Arrange
            var filepath = XFS.Path(@"c:\something\foo.txt");
            var filesystem = new MockFileSystem(new Dictionary<string, MockFileData>());

            using (var writer = filesystem.StreamWriter.FromPath(filepath))
            {
                writer.WriteLine("Test");
            }

            var file = filesystem.GetFile(filepath);

            // TextContents should contain new line and "Test"-string
            Assert.AreEqual(file.TextContents, string.Format("Test{0}", Environment.NewLine));
        }

        [Test]
        public void MockStreamWriter_WriteLine_String_MultipleLines()
        {
            // Arrange
            var filepath = XFS.Path(@"c:\something\foo.txt");
            var filesystem = new MockFileSystem(new Dictionary<string, MockFileData>());

            using (var writer = filesystem.StreamWriter.FromPath(filepath))
            {
                writer.WriteLine("Test");
                writer.WriteLine("SecondLine");
                writer.WriteLine("LastLine");
            }

            var file = filesystem.GetFile(filepath);

            // TextContents should contain all lines seperated by '\r\n'
            Assert.AreEqual(file.TextContents, string.Format("Test{0}SecondLine{0}LastLine{0}", Environment.NewLine));
        }

        [Test]
        public void MockStreamWriter_Write_String()
        {
            // Arrange
            var filepath = XFS.Path(@"c:\something\foo.txt");
            var filesystem = new MockFileSystem(new Dictionary<string, MockFileData>());

            using (var writer = new MockStreamWriter(filesystem, filepath))
            {
                writer.Write("Test");
            }

            var file = filesystem.GetFile(filepath);

            // TextContents should just contain "Test"-string
            Assert.AreEqual(file.TextContents, "Test");
        }

        [Test]
        public void MockStreamWriter_Write_Int()
        {
            // Arrange
            var filepath = XFS.Path(@"c:\something\foo.txt");
            var filesystem = new MockFileSystem(new Dictionary<string, MockFileData>());

            int testInt = 2518925;  

            using (var writer = new MockStreamWriter(filesystem, filepath))
            {
                writer.Write(testInt);
            }

            var file = filesystem.GetFile(filepath);

            // TextContents should just contain "Test"-string
            Assert.AreEqual(file.TextContents, testInt.ToString());
        }

        [Test]
        public void MockStreamWriter_OnlyMemoryStreams()
        {
            // Arrange
            var filepath = Path.Combine(TestContext.CurrentContext.TestDirectory, "test.txt");
            var filesystem = new MockFileSystem(new Dictionary<string, MockFileData>());
            Assert.Throws<InvalidOperationException>(
                () => new MockStreamWriter(filesystem, new System.IO.FileStream(filepath, FileMode.Create)), 
                "Using MockStreamWriter with no MemoryStream should throw an exception");
        }
    }
}
