using System.Linq;

namespace JdaTools.Connection
{
    internal class MocaRequestFactory
    {
        public bool AutoCommitEnabled { get; set; } = true;
        private string _sessionKey;

        public MocaRequestFactory(string sessionKey)
        {
            _sessionKey = sessionKey;
        }

        public MocaRequest Get(string query, object parameters)
        {
            var request = new MocaRequest()
            {
                Environment = new MocaRequestVar[]
                {
                    new MocaRequestVar("SESSION_KEY", _sessionKey)
                },
                Query = query
            };
            if (parameters != null)
            {

                var type = parameters.GetType();
                var properties = type.GetProperties();
                request.Context = properties
                    .Select(p => new MocaParameter(p.Name, p.GetValue(parameters)))
                    .ToArray();
            }
            return request;
        }

        public static MocaRequest GetLoginQuery(MocaCredentials credentials)
        {
            return new MocaRequest()
            {
                Environment = new MocaRequestVar[]
                {
                    new MocaRequestVar("DEVCOD", credentials.DevCod),
                    new MocaRequestVar("LOCALE_ID", "US_ENGLISH"),
                },
                Context = new MocaParameter[]
                {
                    new MocaParameter("usr_id",credentials.UserName),
                    new MocaParameter("usr_pswd", credentials.Password)
                },
                Query = @"login user where usr_id = @usr_id and usr_pswd = @usr_pswd"
            };
        }
    }
}