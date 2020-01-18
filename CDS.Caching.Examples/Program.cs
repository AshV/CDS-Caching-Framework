using Newtonsoft.Json;
using System;

namespace CDS.Caching.Examples
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var contactCache = new EntityCache<Contact>("contacts", 5, (a, b) => { return ""; });

            contactCache.ForEach(cache =>
            {
                Console.WriteLine(cache.Key + " - " + cache.Value.ToString());
            });
        }
    }

    public class Contact : IRecord
    {
        [JsonProperty("contactid")]
        public Guid Id { get; set; }

        [JsonProperty("@odata.etag")]
        public string ETag { get; set; }
    }
}