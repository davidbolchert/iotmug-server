
namespace IoTMug.Core.IoTMessages
{
    public class Event : IoTMessage
    {
        public override string Type { get; } = IoTMessageType.EVENT;
        public string Value { get; set; }
    }
}
