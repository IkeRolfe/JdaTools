using System;
using JdaTools.Connection.Attributes;

namespace JdaTools.Studio.Models
{
    public class TraceFileRecord
    {
        [MocaColumn("filnam")]
        public string FileName { get; set; }
        [MocaColumn("size")]
        public int Size { get; set; }
        [MocaColumn("credte")]
        public DateTime CreatedAt { get; set; }
        [MocaColumn("path")]
        public string Path { get; set; }
    }
}