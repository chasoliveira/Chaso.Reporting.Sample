using System;
using System.IO;

namespace Chaso.Reporting.Sample.Models
{
    public class ReportFile
    {
        public ReportFile(FileInfo fi)
        {
            FileName = fi.Name;
            FullName = fi.FullName;
            CreationTime = fi.CreationTime;
            LastWriteTime = fi.LastWriteTime;
            Length = fi.Length;
        }

        public string FileName { get; }
        public string FullName { get; }
        public DateTime CreationTime { get; private set; }
        public DateTime LastWriteTime { get; private set; }
        public long Length { get; private set; }
    }

}