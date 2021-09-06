using System;
using System.IO;

namespace Data.IO.Utils
{
    public static class DirUtils
    {
        public static void CreateDefaultFolder(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return;

            Array values = Enum.GetValues(typeof(EFolderType));

            foreach (EFolderType val in values)
            {
                string dirPath = Path.Combine(path, Enum.GetName(typeof(EFolderType), val));

                bool exist = Directory.Exists(dirPath);

                if (!exist)
                {
                    Directory.CreateDirectory(dirPath);
                }
            }
        }
    }
}