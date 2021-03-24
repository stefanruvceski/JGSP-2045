using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApp.Models
{
    public enum UserType
    {
        Passenger = 0,
        BusController,
        Admin
    }

    public enum DayInWeek
    {
        Weekday = 0,
        Saturday,
        Sunday
    }

    public enum LineType
    {
        Urban = 0,
        Suburban
    }

    public enum VerificationStatus
    {
        Pending = 0,
        Successful,
        Unsuccessful
    }
}