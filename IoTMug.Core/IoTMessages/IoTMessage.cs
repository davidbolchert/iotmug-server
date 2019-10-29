using System;

namespace IoTMug.Core.IoTMessages
{
    public struct IoTMessageType
    {
        public const string EVENT = "event";
        public const string MEASURE = "measure";
    }

    public abstract class IoTMessage
    {
        public Guid Id { get; } = Guid.NewGuid();
        public virtual string Type { get; }
        public string Name { get; set; }
        public long Ttl { get; } = 2592000000;
        public DateTimeOffset Timestamp { get; set; }
    }
}
