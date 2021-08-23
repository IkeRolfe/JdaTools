using JdaTools.Connection.Attributes;

namespace JdaTools.Studio.Models
{
    public interface IMocaFile
    {
        string Type { get; set; }
        string PathName { get; set; }
        string FileName { get; set; }
        public string Description { get; set; }
    }

    public class MocaFile : IMocaFile
    {
        [MocaColumn("type")]
        public string Type { get; set; }
        [MocaColumn("pathname")]
        public string PathName { get; set; }
        [MocaColumn("filename")]
        public string FileName { get; set; }

        public string Description { get; set; }

        public override string ToString()
        {
            return FileName;
        }
    }
}