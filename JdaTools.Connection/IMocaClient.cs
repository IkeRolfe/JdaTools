using System.Collections.Generic;
using System.Threading.Tasks;

namespace JdaTools.Connection
{
    public interface IMocaClient
    {
        string Endpoint { get; set; }

        Task<MocaResponse> ConnectAsync(MocaCredentials credentials);
        Task<MocaResponse> ExecuteQuery(string query, object parameters = null);
        Task<IEnumerable<T>> ExecuteQuery<T>(string query, object parameters = null);
        Task<MocaResponse> ExecuteQueryAsync(string query, object parameters = null);
        Task<IEnumerable<T>> ExecuteQueryAsync<T>(string query, object parameters = null);
        Task<T> FirstOrDefault<T>(string query, object parameters = null);
        Task Logout();
    }
}