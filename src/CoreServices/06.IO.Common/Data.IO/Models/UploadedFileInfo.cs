namespace Data.IO.Models
{
    public class UploadedFileInfo
    {
        public string FileType { get; set; }
        public string TrustedFileNameForDisplay { get; set; }
        public string OriginalName { get; set; }
        public string Path { get; set; }
        public long Size { get; set; }
        public string Extension { get; set; }
    }
}