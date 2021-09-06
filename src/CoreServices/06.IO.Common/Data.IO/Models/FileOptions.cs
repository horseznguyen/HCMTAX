using System;

namespace Data.IO
{
    public class FileOptions
    {
        public string[] PermittedExtensions { get; set; }
        public long FileSizeLimit { get; set; }
        public string Path { get; set; }
        public DateTimeKind Kind { get; set; }
    }
}