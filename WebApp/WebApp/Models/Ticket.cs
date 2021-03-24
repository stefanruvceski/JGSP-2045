using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models
{
    public class Ticket
    {
        public int Id { get; set; }

        [ForeignKey("TicketType")]
        public int TicketTypeId { get; set; }
        public TicketType TicketType { get; set; }

        [ForeignKey("Passenger")]
        public int PassengerId { get; set; }
        public Passenger Passenger { get; set; }

        public DateTime IssuingDate { get; set; }
        public DateTime ExpirationDate { get; set; }

        public Ticket() { }

        public Ticket(int ticketTypeId, int passengerId, DateTime issuingDate, DateTime expirationDate)
        {
            this.TicketTypeId = ticketTypeId;
            this.PassengerId = passengerId;
            this.IssuingDate = issuingDate;
            this.ExpirationDate = expirationDate;
        }
    }
}