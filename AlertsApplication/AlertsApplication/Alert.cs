namespace ProjectXML
{
    internal class Alert
    {
        public int ID { get; set; }
        public string Type { get; set; }
        public string Operator { get; set; }
        public decimal Value { get; set; }
        public decimal Value2 { get; set; }
        public bool Enable { get; set; }
        public string Description { get; set; }
    }
}