using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace JdaTools.ConfigurationManager
{
    [XmlRoot("configurations")]
    public class Configurations
    {
        [XmlElement("connection")]
        public List<ConnectionInfo> Connections { get; set; } = new List<ConnectionInfo>();
    }
}
