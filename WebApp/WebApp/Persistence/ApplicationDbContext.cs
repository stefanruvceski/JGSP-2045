using System;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using WebApp.Models;

namespace WebApp.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<AgeGroup> AgeGroups { get; set; }
        public DbSet<Bus> Buses { get; set; }
        public DbSet<Line> Lines { get; set; }
        public DbSet<PriceList> PriceLists { get; set; }
        public DbSet<PriceList_TicketType> PriceList_TicketTypes { get; set; }  
        public DbSet<Station> Stations { get; set; }
        public DbSet<StationLine> StationLines { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<TicketType> TicketTypes { get; set; }     
        public DbSet<TimeTable> TimeTables { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }

        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }
        
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}