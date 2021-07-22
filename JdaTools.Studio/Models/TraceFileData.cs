using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using JdaTools.Connection.Attributes;

namespace JdaTools.Studio.Models
{
    public class TraceFileData
    {
        [MocaColumn("filename")]
        public string FileName { get; set; }
        [MocaColumn("data")]
        public string Data { get; set; }

        public string GetTrace()
        {
            byte[] compressed = Convert.FromBase64String(Data);
            return Encoding.UTF8.GetString(compressed);
        }
    }
}