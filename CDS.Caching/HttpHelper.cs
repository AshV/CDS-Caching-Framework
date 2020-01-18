using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace CDS.Caching
{
    public class HttpHelper
    {
        public string GetRecord(string action, string eTag)
        {
            var client = new HttpClient();
            var msg = new HttpRequestMessage(HttpMethod.Get, action);
            msg.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "");
            msg.Headers.Add("If-None-Match", eTag);
            msg.Headers.Add("OData-MaxVersion", "4.0");
            msg.Headers.Add("OData-Version", "4.0");
            var response = client.SendAsync(msg).Result;
            response.EnsureSuccessStatusCode();
            if (response.StatusCode == HttpStatusCode.NotModified)
                return string.Empty;
            else
                return response.Content.ReadAsStringAsync().Result;
        }
    }
}