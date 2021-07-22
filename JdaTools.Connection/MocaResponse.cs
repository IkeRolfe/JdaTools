using System.Data;

namespace JdaTools.Connection
{

        // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
        /// <remarks/>
        [System.SerializableAttribute(), System.ComponentModel.DesignerCategory("code"),
         System.Xml.Serialization.XmlType(AnonymousType = true),
         System.Xml.Serialization.XmlRoot("moca-response", Namespace = "", IsNullable = false)]
        public partial class MocaResponse
        {

            private string _sessionId;

            private int _status;

            private MocaResult _mocaResults;

            /// <remarks/>
            [System.Xml.Serialization.XmlElement("session-id")]
            public string SessionId
            {
                get => _sessionId;
                set => _sessionId = value;
            }

            /// <remarks/>
            public int status
            {
                get => _status;
                set => _status = value;
            }

            public string message { get; set; }

            /// <remarks/>
            [System.Xml.Serialization.XmlElement("moca-results")]
            public MocaResult MocaResults
            {
                get => _mocaResults;
                set => _mocaResults = value;
            }
        }
}