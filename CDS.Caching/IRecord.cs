using System;

namespace CDS.Caching
{
    public interface IRecord
    {
        public Guid Id { get; set; }
        public string ETag { get; set; }
    }
}