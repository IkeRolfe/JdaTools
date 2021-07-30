using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using JdaTools.Connection.Attributes;

namespace JdaTools.Connection
{
    public class MocaClient
    {
        private HttpClient _httpClient;
        private MocaHttpHandler _httpHandler;
        private MocaRequestFactory _mocaRequestFactory;
        private string _endpoint;

        public MocaClient(string endpoint)
        {
            _endpoint = endpoint;
            //_httpHandler = new MocaHttpHandler();
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
        }

        public MocaClient()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
        }

        public string Endpoint
        {
            get => _endpoint;
            set { _endpoint = value; } //TODO check if logged in etc
        }


        public async Task<MocaResponse> ConnectAsync(MocaCredentials credentials)
        {
            var loginRequest = MocaRequestFactory.GetLoginQuery(credentials);
            var response = await PostAsync(loginRequest);
            var sessionKey = response.MocaResults.GetDataTable().Rows[0]["session_key"];
            _mocaRequestFactory = new MocaRequestFactory(sessionKey.ToString());

            return response;
        }
        public async Task<MocaResponse> Logout() 
        {
            return await ExecuteQuery("logout");
        }

        private async Task<MocaResponse> PostAsync(MocaRequest request)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(MocaRequest));
            var requestXml = request.ToXML();
            var content = new StringContent(requestXml, Encoding.UTF8, "application/moca-xml");
            var result = await _httpClient.PostAsync(new Uri(_endpoint), content);
            result.EnsureSuccessStatusCode(); //Throws exception for bad html status
            //var responseString = await result.Content.ReadAsStringAsync();

            var mySerializer = new XmlSerializer(typeof(MocaResponse));
            var stream = await result.Content.ReadAsStreamAsync();
            var response = (MocaResponse)mySerializer.Deserialize(stream);
            //var dt = response.MocaResults.GetDataTable();
            //var row = dt.Rows[0];
            return response;
        }

        public async Task<MocaResponse> ExecuteQuery(string query, object parameters = null)
        {
            var request = _mocaRequestFactory.Get(query, parameters);
            var response = await PostAsync(request);
            return response;
        }

        public async Task<IEnumerable<T>> ExecuteQuery<T>(string query) 
        {
            var response = await ExecuteQuery(query);
            var dt = response.MocaResults.GetDataTable();
            var type = typeof(T);

            var properties = type.GetProperties()
                .Where(p=>p.IsDefined(typeof(MocaColumnAttribute), false));

            var returnList = new List<T>();
            //Create new instance for each row
            foreach (DataRow dataRow in dt.Rows)
            {
                //Set attributes
                T retVal;
                //Handle immutuable string
                if (typeof(T) == typeof(string))
                {
                    //TODO: Throw error if more than 1 column
                    var firstCell = dataRow.ItemArray.FirstOrDefault().ToString();
                    retVal = (T)Convert.ChangeType(firstCell, typeof(T));
                }
                else
                {
                    retVal = (T)Activator.CreateInstance(typeof(T));
                    foreach (var propertyInfo in properties)
                    {
                        var columnAttribute = (MocaColumnAttribute)propertyInfo.GetCustomAttribute(typeof(MocaColumnAttribute), false);
                        var columnName = columnAttribute.ColumnName;

                        try
                        {
                            var value = dataRow[columnName];
                            if (propertyInfo.PropertyType == typeof(DateTime))
                            {
                                value = DateTime.Parse((string)value);
                            }
                            else if (propertyInfo.PropertyType == typeof(bool))
                            {
                                if (value == "0" || value == "")
                                {
                                    value = false;
                                }
                                else if (value == "1")
                                {
                                    value = true;
                                }
                                value = bool.Parse(value.ToString());
                            }
                            TrySetProperty(retVal, propertyInfo.Name, value);
                        }
                        catch (ArgumentException e)
                        {
                            Console.WriteLine(e);
                            throw new ArgumentException($"Unable match property {columnName} from {propertyInfo.Name}");
                        }
                    }
                }
                returnList.Add(retVal);
            }
            return returnList;
        }

        private bool TrySetProperty(object obj, string property, object value)
        {
            if (value.GetType() == typeof(DBNull))
            {
                value = null;
            }
            var prop = obj.GetType().GetProperty(property, BindingFlags.Public | BindingFlags.Instance);
            if (prop != null && prop.CanWrite)
            {
                prop.SetValue(obj, value, null);
                return true;
            }
            return false;
        }
    }
}
