namespace IoTMug.Core.IoTMessages
{
    public class Measure : IoTMessage
    {
        public override string Type { get; } = IoTMessageType.MEASURE;
        public double Value { get; set; }
        public string Unit { get; set; }
    }
}
