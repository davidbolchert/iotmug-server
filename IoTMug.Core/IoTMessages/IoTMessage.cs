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
        public Guid Id { get; set; }
        public virtual string Type { get; }
        public string Name { get; set; }
        public long Ttl { get; set; }
        public DateTimeOffset Timestamp { get; set; }
    }
}
