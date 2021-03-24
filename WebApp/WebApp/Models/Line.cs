using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models
{
    public class Line
    {
        [Key]
        public string Id { get; set; }

        public LineType LineType { get; set; }

        public string Description { get; set; }

        public string Color { get; set; }

        public bool IsActive { get; set; }                 // za logicko brisanje

        [InverseProperty("Lines")]
        public ICollection<Station> Stations { get; set; } 

        public Line() { }
    }
}