using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;

namespace CDS.Caching
{
    public abstract class EntityCache<TRecord> where TRecord : IRecord
    {
        public ConcurrentDictionary<Guid, TRecord> CachedRecords { get; set; }

        public ConcurrentDictionary<Guid, string> CachedJsons { get; set; }

        public ReadOnlyCollection<string> AttributesNames { get; set; }

        public CachingType CacheType { get; set; }

        private string selectAttributes;

        private string entitySchemaName;

        private string idFieldName;

        public EntityCache(string entitySchemaName, string idFieldName, params string[] attributesToCache)
        {
            this.idFieldName = idFieldName;
            this.entitySchemaName = entitySchemaName;

            CacheType = CachingType.Both;
            CachedRecords = new ConcurrentDictionary<Guid, TRecord>();
            CachedJsons = new ConcurrentDictionary<Guid, string>();
            AttributesNames = new ReadOnlyCollection<string>(attributesToCache);
            selectAttributes = string.Join(",", attributesToCache);
        }

        public TRecord GetCached(Guid recordId)
        {
            return default(TRecord);
        }

        public TRecord RetriveLatest(Guid recordId)
        {
            return default(TRecord);
        }

        public TRecord GetCachedJson(Guid recordId)
        {
            return default(TRecord);
        }

        public TRecord RetriveLatestJson(Guid recordId)
        {
            return default(TRecord);
        }
    }
}