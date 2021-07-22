using System.Xml.Serialization;

namespace JdaTools.Connection
{
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class MocaParameter
    {

        private string _name;
        private string _type;

        public MocaParameter(string name, object value)
        {
            Name = name;
            Value = value.ToString();
        }

        public MocaParameter()
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
        [System.Xml.Serialization.XmlAttributeAttribute("type")]
        public string Type
        {
            get => _type;
        }

        [XmlAttributeAttribute("oper")]
        public readonly string Oper = "EQ";

        private string _value;

        [XmlTextAttribute()]
        public string Value
        {
            get => _value;
            set => _value = value;
        }
    }
    
}