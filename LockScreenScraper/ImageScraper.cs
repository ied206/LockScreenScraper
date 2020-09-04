using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using Joveler.FileMagician;

namespace LockScreenScraper
{
    public class ImageScraper
    {
        private readonly string _magicFile;
        private readonly string _srcDir;
        public static string DefaultSourceDir => Environment.ExpandEnvironmentVariables(@"%UserProfile%\AppData\Local\Packages\Microsoft.Windows.ContentDeliveryManager_cw5n1h2txyewy\LocalState\Assets");

        public ImageScraper(string magicFile, string srcDir)
        {
            _magicFile = magicFile;
            _srcDir = srcDir;
        }

        public IEnumerable<ImageEntry> Scan(SearchOption searchOption)
        {
            List<ImageEntry> entries = new List<ImageEntry>();
            using (Magic magic = Magic.Open(_magicFile, MagicFlags.MimeType))
            {
                foreach (string filePath in Directory.EnumerateFiles(_srcDir, "*", searchOption))
                {
                    string fileName = Path.GetFileName(filePath);
                    string mimeType = magic.CheckFile(filePath);
                    if (mimeType.Equals("image/jpeg", StringComparison.Ordinal))
                        entries.Add(new ImageEntry(fileName, filePath, ImageKind.Jpg));
                    else if (mimeType.Equals("image/png", StringComparison.Ordinal))
                        entries.Add(new ImageEntry(fileName, filePath, ImageKind.Png));
                }
            }
            return entries;
        }
    }

    public enum ImageKind
    {
        None = 0,
        Jpg = 1,
        Png = 2,
    }

    public class ImageEntry
    {
        public string FileName { get; set; }
        public string FullPath { get; set; }
        public ImageKind Kind { get; set; }

        public ImageEntry(string fileName, string fullPath, ImageKind kind)
        {
            FileName = fileName;
            FullPath = fullPath;
            Kind = kind;
        }
    }
}
