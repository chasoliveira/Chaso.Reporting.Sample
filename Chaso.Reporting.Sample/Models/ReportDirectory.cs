using System.Collections.Generic;
using System.IO;

namespace Chaso.Reporting.Sample.Models
{
    public class ReportDirectory
    {
        public ReportDirectory(string directory)
        {
            DirectoryInfo di = new DirectoryInfo(directory);

            this.Name = di.Name;
            this.FullName = di.FullName;
            this.ReportFiles = new List<ReportFile>();
            this.SubDirectories = new List<ReportDirectory>();
            ProcessDirectory();
        }
        public string Name { get; set; }
        public string FullName { get; }
        public IList<ReportFile> ReportFiles { get; set; }
        public IList<ReportDirectory> SubDirectories { get; set; }


        private void ProcessDirectory()
        {
            var files = new List<KeyValuePair<string, string>>();
            // Process the list of files found in the directory.
            string[] fileEntries = Directory.GetFiles(FullName);
            foreach (string fileName in fileEntries)
            {
                var re = new FileInfo(fileName);
                ReportFiles.Add(new ReportFile(re));
            }

            // Recurse into subdirectories of this directory.
            string[] subdirectoryEntries = Directory.GetDirectories(FullName);
            foreach (string subdirectory in subdirectoryEntries)
            {
                var subReportDirectory = new ReportDirectory(subdirectory);
                SubDirectories.Add(subReportDirectory);
            }
        }
    }
}