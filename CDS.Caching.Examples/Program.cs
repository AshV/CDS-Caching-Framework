using Newtonsoft.Json;
using System;

namespace CDS.Caching.Examples
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }

    public class Contact : IRecord
    {
        [JsonProperty("contactid")]
        public Guid Id { get; set; }

        [JsonProperty("@odata.etag")]
        public string ETag { get; set; }
    }

    public class ContactCache : EntityCache<Contact>
    {
        public ContactCache() : base("contact", "contactid", new HttpConfig())
        {

        }
    }
}
