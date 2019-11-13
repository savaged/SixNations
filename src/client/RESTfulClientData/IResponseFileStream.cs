using System.IO;

namespace savaged.mvvm.Data
{
    public interface IResponseFileStream
    {
        string FileName { get; }
        Stream Stream { get; }
    }
}