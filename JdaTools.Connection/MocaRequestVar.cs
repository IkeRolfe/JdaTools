namespace JdaTools.Connection
{
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class MocaRequestVar
    {

        private string _name;
        private string _value;

        public MocaRequestVar(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public MocaRequestVar()
        {
            
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("name")]
        public string Name
        {
            get => _name;
            set => _name = value;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("value")]
        public string Value
        {
            get => _value;
            set => _value = value;
        }
    }


}