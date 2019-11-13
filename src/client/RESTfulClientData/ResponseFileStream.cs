using System.IO;

namespace savaged.mvvm.Data
{
    public class ResponseFileStream : IResponseFileStream
    {
        public ResponseFileStream(string fileName, Stream stream)
        {
            FileName = fileName;
            Stream = stream;
        }

        public string FileName { get; private set; }

        public Stream Stream { get; private set; }

        public static IResponseFileStream Null =>
            new NullResponseFileStream();
    }

    public sealed class NullResponseFileStream : ResponseFileStream
    {
        internal NullResponseFileStream() 
            : base(string.Empty, Stream.Null) { }
    }
}
