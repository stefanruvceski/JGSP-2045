using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApp.Models
{
    public class Bus
    {
        public int Id { get; set; }

        public string LineId { get; set; }
        Line Line { get; set; }

        public double XCooridinate { get; set; }

        public double YCoordinate { get; set; }

        public double NextStationX { get; set; }

        public double NextStationY { get; set; }

        public Bus() { }
    }
}