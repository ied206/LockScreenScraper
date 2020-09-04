using CommandLine;
using LockScreenScraper;
using System;
using System.Collections.Generic;
using System.IO;

namespace LockScreenScraper.Cli
{
    #region CommandLine
    public class ScrapeOptions
    {
        [Option('s', "src", Required = false, HelpText = "Source directory to scrape images.")]
        public string SourceDir { get; set; }
        [Option('d', "dest", Required = true, HelpText = "Dest directory to store scraped images.")]
        public string DestDir { get; set; }
    }
    #endregion

    public class Program
    {
        public static void Main(string[] args)
        {
            string baseDir = AppContext.BaseDirectory;
            NativeLibrary.Init(baseDir);

            Parser.Default.ParseArguments<ScrapeOptions>(args)
                .WithParsed(RunOptions)
                .WithNotParsed(HandleParseError);
        }

        public static void RunOptions(ScrapeOptions opts)
        {
            string srcDir = opts.SourceDir ?? ImageScraper.DefaultSourceDir;
            ImageScraper scraper = new ImageScraper(NativeLibrary.MagicFile, srcDir);

            int count = ScanTargetDir(scraper, opts.DestDir);
            Console.WriteLine($"Scraped {count} images from [{srcDir}].");
        }

        public static void HandleParseError(IEnumerable<Error> errs)
        {
            foreach (Error err in errs)
            {
                Console.WriteLine($"Error: {err}");
            }
        }

        private static int ScanTargetDir(ImageScraper scraper, string destDir)
        {
            Directory.CreateDirectory(destDir);

            int count = 0;
            foreach (ImageEntry entry in scraper.Scan(SearchOption.TopDirectoryOnly))
            {
                string ext = string.Empty;
                switch (entry.Kind)
                {
                    case ImageKind.None:
                        continue;
                    case ImageKind.Jpg:
                        ext = ".jpg";
                        break;
                    case ImageKind.Png:
                        ext = ".png";
                        break;
                }

                string destName = entry.FileName + ext;
                string destPath = Path.Combine(destDir, destName);
                Console.WriteLine($"Copying [{entry.FileName}] into [{destPath}].");
                File.Copy(entry.FullPath, destPath, true);

                count += 1;
            }
            return count;
        }
    }
}
