using System;
using System.Xml;
using System.Xml.Serialization;

namespace JdaTools.Studio.Models
{
    
    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false, ElementName = "command")]
    public class MocaCommandFile
    {
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElementAttribute("description")]
        public string Description { get; set; }
        [XmlElementAttribute("type")]
        public string Type { get; set; } 
        [XmlElementAttribute("local-syntax")]
        public string LocalSyntax { get; set; }
        /*[XmlIgnore]
        public string Content { get; set; }

        [XmlText]
        public XmlNode[] CDataContent
        {
            get
            {
                var dummy = new XmlDocument();
                return new XmlNode[] { dummy.CreateCDataSection(Content) };
            }
            set
            {
                if (value == null)
                {
                    Content = null;
                    return;
                }

                if (value.Length != 1)
                {
                    throw new InvalidOperationException(
                        $"Invalid array length {value.Length}");
                }

                Content = value[0].Value;
            }
        }*/
    }

}


