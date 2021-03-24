using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebApp.Models
{
    public class TimeTable
    {
        public int Id { get; set; }
        public DayInWeek Day { get; set; }
        public string Schedule { get; set; }

        [ForeignKey("Line")]
        public string LineId { get; set; }
        public Line Line { get; set; }

        public TimeTable() { }
    }
}