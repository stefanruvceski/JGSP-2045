using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebApp.Models
{
    public class Passenger : AppUser
    {
        public ICollection<Ticket> Tickets { get; set; }//ask question!
        public byte[] Document { get; set; }
        
        [ForeignKey("AgeGroup")]
        public int AgeGroupId { get; set; }
        public AgeGroup AgeGroup { get; set; }

        public VerificationStatus VerificationStatus { get; set; }

        public Passenger() { }

        public Passenger(string email, string password, string firstName, string lastName, string username, DateTime birthday, string address, byte[] document)
        {
            this.Email = email;
            this.Password = password;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Username = username;
            this.Birthday = birthday;
            this.Address = address;
            this.Type = UserType.Passenger;
            this.Document = document;
        }
    }
}