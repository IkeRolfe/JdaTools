namespace JdaTools.Studio.Models
{
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute("component-level", Namespace = "", IsNullable = false)]
    public class ComponentLevel
    {

        private uint _nameField;

        private string _descriptionField;

        private ushort _sortSequenceField;

        /// <remarks/>
        public uint Name
        {
            get
            {
                return this._nameField;
            }
            set
            {
                this._nameField = value;
            }
        }

        /// <remarks/>
        public string Description
        {
            get
            {
                return this._descriptionField;
            }
            set
            {
                this._descriptionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("sort-sequence")]
        public ushort SortSequence
        {
            get
            {
                return this._sortSequenceField;
            }
            set
            {
                this._sortSequenceField = value;
            }
        }
    }
}