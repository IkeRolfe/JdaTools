using System.Xml.Serialization;

namespace JdaTools.Connection
{

    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [System.Xml.Serialization.XmlType(AnonymousType = true)]
    [System.Xml.Serialization.XmlRoot("moca-request", Namespace = "", IsNullable = false)]
    public partial class MocaRequest
    {

        private MocaRequestVar[] _environment;

        #region Xml Attributes
        private string _query;
        private bool _autoCommit = true;
        private MocaParameter[] _context;

                /// <remarks/>
        [XmlAttribute()]
        public bool AutoCommit
        {
            get => _autoCommit;
            set => _autoCommit = value;
        } 
        #endregion

        /// <remarks/>
        [XmlArray("environment")]
        [XmlArrayItem("var", IsNullable = false)]
        public MocaRequestVar[] Environment
        {
            get => _environment;
            set => _environment = value;
        }

        /// <remarks/>
        [XmlArray("context")]
        [XmlArrayItem("field", IsNullable = true)]
        public MocaParameter[] Context
        {
            get => _context;
            set => _context = value;
        }

        /// <remarks/>
        [XmlElement("query")]
        public string Query
        {
            get => _query;
            set => _query = value;
        }



        public string ToXML()
        {
            using var sw = new System.IO.StringWriter();
            var serializer = new XmlSerializer(this.GetType());
            serializer.Serialize(sw, this);
            return sw.ToString();
        }
    }
}