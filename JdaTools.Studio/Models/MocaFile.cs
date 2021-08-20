using JdaTools.Connection.Attributes;

namespace JdaTools.Studio.Models
{
    public class MocaFile
    {
        [MocaColumn("type")]
        public string Type { get; set; }
        [MocaColumn("pathname")]
        public string PathName { get; set; }
        [MocaColumn("filename")]
        public string FileName { get; set; }

        public override string ToString()
        {
            return FileName;
        }
    }

}