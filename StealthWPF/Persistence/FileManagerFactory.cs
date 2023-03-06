using System.IO;

namespace StealthWPF.Persistence
{
    public class FileManagerFactory
    {
        public static IFileManager? CreateForPath(string path) => Path.GetExtension(path) switch
        {
            ".txt" => new TxtFileManager(path),
            ".pdf" => new PdfFileManager(path),
            _      => null
        };
    }
}
