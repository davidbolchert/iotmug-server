namespace IoTMug.Core.IoTMessages
{
    public class Measure : IoTMessage
    {
        public override string Type { get; } = IoTMessageType.MEASURE;
        public decimal Value { get; set; }
        public string Unit { get; set; }
    }
}
