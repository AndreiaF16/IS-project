using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPLeiriaSmartCampusFireBaseIO
{
    public class Sensor
    {
        public string Key { get; set; }

        public int SensorID { get; set; }

        public decimal Temperature { get; set; }

        public decimal Humidity { get; set; }

        public int Battery { get; set; }

        public DateTime Timestamp { get; set; }

        public bool Alert { get; set; }
    }
}
