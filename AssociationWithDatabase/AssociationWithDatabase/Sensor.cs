using System;

namespace AssociationWithDatabase
{
    internal class Sensor
    {
        public int SensorID { get; set; }
        public decimal Temperature { get; set; }
        public decimal Humidity { get; set; }
        public int Battery { get; set; }
        public DateTime Timestamp { get; set; }
        public bool ActiveAlert { get; set; }
        public string AlertDescription { get; set; }
    }
}