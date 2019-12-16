using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IPLeiriaSmartCampusAPI.Models
{
    public class Sensor
    {
        public int Id { get; set; }
        public decimal Temperatura { get; set; }
        public decimal Humidade { get; set; }
        public int Bateria { get; set; }
        public DateTime Data { get; set; }
        public Alerta Alerta { get; set; }
    }
}
