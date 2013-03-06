using System.Collections.Generic;
using System.Text;

namespace System.IO.Abstractions.TestingHelpers
{
    public class MockStreamWriter : IStreamWriter
    {
        public MemoryStream WrittenData { get; set; }
        public void Close()
        {
        }

        public void Flush()
        {
            throw new NotImplementedException();
        }

        public void Write(char value)
        {
            throw new NotImplementedException();
        }

        public void Write(char[] buffer)
        {
            throw new NotImplementedException();
        }

        public void Write(char[] buffer, int index, int count)
        {
            throw new NotImplementedException();
        }

        public void Write(string value)
        {
            throw new NotImplementedException();
        }

        public void WriteLine(string value)
        {
            if (WrittenData == null)
                WrittenData = new MemoryStream();

            var bytes = Encoding.Default.GetBytes(value);
            WrittenData.Write(bytes, 0, bytes.Length);
        }

        public bool AutoFlush { get; set; }
        public Stream BaseStream { get; private set; }
        public Encoding Encoding { get; private set; }
    }
}