using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models
{
    public class Station
    {
        public int Id { get; set; }
        public string StationName { get; set; }
        public string Address { get; set; }
        public double XCooridinate { get; set; }
        public double YCoordinate { get; set; }
        public bool IsStation { get; set; }

        [InverseProperty("Stations")]
        public ICollection<Line> Lines { get; set; }

        public Station() { }
    }
}