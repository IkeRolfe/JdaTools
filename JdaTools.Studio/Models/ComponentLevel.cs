namespace JdaTools.Studio.Models
{
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute("component-level", Namespace = "", IsNullable = false)]
    public class ComponentLevel
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("name")]
        public string Name { get; set; }
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("description")]
        public string Description { get; set; }
        [System.Xml.Serialization.XmlElementAttribute("library")]
        public string Library { get; set; }
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("sort-sequence")]
        public int SortSequence { get; set; }
        [System.Xml.Serialization.XmlElementAttribute("editable")]
        public bool Editable { get; set; }
    }
}