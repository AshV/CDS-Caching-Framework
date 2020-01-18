using CSharpTest.Net.Collections;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;

namespace CDS.Caching
{
    public class EntityCache<TRecord> : IEnumerable<KeyValuePair<Guid, TRecord>> where TRecord : IRecord
    {
        public List<string> CachedAttributeNames { get; }

        public int Count { get { return CachedRecords.Count; } }

        private LurchTable<Guid, TRecord> CachedRecords;

        private string _selectAttributes;

        private string _entityPluralSchemaName;

        private Func<string, string, string> _recordValidator;

        public EntityCache(string entityPluralSchemaName, int capacity, Func<string, string, string> recordValidator, params string[] attributesToCache)
        {
            _entityPluralSchemaName = entityPluralSchemaName;
            _recordValidator = recordValidator;
            CachedRecords = new LurchTable<Guid, TRecord>(capacity);
            CachedAttributeNames = new List<string>(attributesToCache);
            _selectAttributes = string.Join(",", attributesToCache);
        }

        public TRecord GetQuickest(Guid recordId)
        {
            var record = GetCached(recordId);
            if (record != null)
                return record;
            return GetLatest(recordId);
        }

        public TRecord GetCached(Guid recordId)
        {
            TRecord record;
            CachedRecords.TryGetValue(recordId, out record);
            return record;
        }

        public TRecord GetLatest(Guid recordId)
        {
            var cached = GetCached(recordId);
            var latest = ValidateCache(PrepareAction(recordId), cached?.ETag);

            if (string.IsNullOrEmpty(latest))
                return cached;
            else
            {
                var latestRecord = Deserialize(latest);
                SyncCacheStores(recordId, cached?.ETag, latestRecord);
                return latestRecord;
            }
        }

        private TRecord Deserialize(string json)
        {
            return JsonConvert.DeserializeObject<TRecord>(json);
        }

        private string PrepareAction(Guid recordId)
        {
            var action = $"{_entityPluralSchemaName}({recordId})";
            if (CachedAttributeNames.Count > 0)
                action += $"?select={_selectAttributes}";
            return action;
        }

        private string ValidateCache(string action, string eTag)
        {
            return _recordValidator(action, eTag);
        }

        private void SyncCacheStores(Guid recordId, string eTag, TRecord record)
        {
            if (string.IsNullOrEmpty(eTag))
                AddToModelCacheStore(recordId, record);
            else
                UpdateModelCacheStore(recordId, record);
        }

        private void AddToModelCacheStore(Guid recordId, TRecord record)
        {
            CachedRecords.TryAdd(recordId, record);
        }

        private void UpdateModelCacheStore(Guid recordId, TRecord record)
        {
            CachedRecords[recordId] = record;
        }

        // Wrapper over LurchTable Methods
        public IEnumerator<KeyValuePair<Guid, TRecord>> GetEnumerator()
        {
            return CachedRecords.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return CachedRecords.GetEnumerator();
        }
    }
}