using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace JdaTools.ConfigurationManager
{
    public class ConnectionInfo
    {
        [XmlElement("name")]
        public string Name { get; set; }
        [XmlElement("url")]
        public string Url { get; set; }
        [XmlElement("default")]
        public bool Default { get; set; }
    }
}
