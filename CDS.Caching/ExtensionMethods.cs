using System;
using System.Collections.Generic;

namespace CDS.Caching
{
    public static class ExtensionMethods
    {
        public static void ForEach<TRecord>(this EntityCache<TRecord> entityCache, Action<KeyValuePair<Guid, TRecord>> action) where TRecord : IRecord
        {
            foreach (var entity in entityCache)
                action(entity);
        }
    }
}