using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CDS.Caching
{
    public abstract class EntityCache<TRecord> where TRecord : IRecord
    {
        public Dictionary<Guid, TRecord> CachedRecords { get; set; }

        public ConcurrentDictionary<Guid, string> CachedJsons { get; set; }

        public ReadOnlyCollection<string> AttributesNames { get; set; }

        private string _selectAttributes;

        private string _entityPluralSchemaName;

        private string _idFieldName;

        private HttpConfig _httpConfig;

        public EntityCache(string entityPluralSchemaName, string idFieldName, HttpConfig httpConfig, params string[] attributesToCache)
        {
            _idFieldName = idFieldName;
            _entityPluralSchemaName = entityPluralSchemaName;

            CachedRecords = new Dictionary<Guid, TRecord>();
            CachedJsons = new ConcurrentDictionary<Guid, string>();
            AttributesNames = new ReadOnlyCollection<string>(attributesToCache);
            _selectAttributes = string.Join(",", attributesToCache);
        }

        public TRecord GetCached(Guid recordId)
        {
            TRecord record;
            CachedRecords.TryGetValue(recordId, out record);
            return record;
        }

        public TRecord RetrieveLatest(Guid recordId)
        {
            RetrieveLatestJson(recordId);
            return GetCached(recordId);
        }

        public string GetCachedJson(Guid recordId)
        {
            string json;
            CachedJsons.TryGetValue(recordId, out json);
            return json;
        }

        public string RetrieveLatestJson(Guid recordId)
        {
            var cachedRecord = GetCached(recordId);
            var cachedJson = GetCachedJson(recordId);
            var cacheState = ValidateCache(PrepareAction(recordId), cachedRecord.ETag);
            if (string.IsNullOrEmpty(cacheState))
                return cachedJson;
            else
            {
                SyncCacheStores(recordId, cachedRecord.ETag, cacheState);
                return cacheState;
            }
        }

        private string PrepareAction(Guid recordId)
        {
            var action = $"{_entityPluralSchemaName}({recordId})";
            if (AttributesNames.Count > 0)
                action += $"?select={_selectAttributes}";
            return action;
        }

        private string ValidateCache(string action, string eTag)
        {
            return _httpConfig.GetRecord(action, eTag);
        }

        private void SyncCacheStores(Guid recordId, string eTag, string cacheState)
        {
            if (string.IsNullOrEmpty(eTag))
            {
                AddToJsonCacheStore(recordId, cacheState);
                AddToModelCacheStore(recordId, cacheState);
            }
            else
            {
                UpdateJsonCacheStore(recordId, cacheState);
                UpdateModelCacheStore(recordId, cacheState);
            }
        }

        private void AddToJsonCacheStore(Guid recordId, string json)
        {
            CachedJsons.TryAdd(recordId, json);
        }

        private void AddToModelCacheStore(Guid recordId, string json)
        {
            CachedRecords.TryAdd(recordId, JsonConvert.DeserializeObject<TRecord>(json));
        }

        private void UpdateJsonCacheStore(Guid recordId, string json)
        {
            CachedJsons[recordId] = json;
        }

        private void UpdateModelCacheStore(Guid recordId, string json)
        {
            CachedRecords[recordId] = JsonConvert.DeserializeObject<TRecord>(json);
        }
    }
}