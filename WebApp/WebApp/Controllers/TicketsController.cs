using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using WebApp.Models;
using WebApp.Persistence;
using WebApp.Persistence.Repository;
using WebApp.Persistence.UnitOfWork;

namespace WebApp.Controllers
{
    [Authorize]
    [RoutePrefix("api/Ticket")]
    public class TicketsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ITicketRepository ticketRepo;
        private IUserRepository userRepo;
        private IPriceList_TicketTypeRepository priceList_ticketTypeRepo;
        private IPriceListRepository priceListRepo;
        private ITicketTypeRepository ticketTypeRepo;
        private IAgeGroupRepository ageGroupRepo;

        public TicketsController(ITicketRepository ticketRepo, IUserRepository userRepo, IPriceList_TicketTypeRepository priceList_ticketTypeRepo, IPriceListRepository priceListRepo, ITicketTypeRepository ticketTypeRepo, IAgeGroupRepository ageGroupRepo)
        {
            this.ticketRepo = ticketRepo;
            this.userRepo = userRepo;
            this.priceList_ticketTypeRepo = priceList_ticketTypeRepo;
            this.priceListRepo = priceListRepo;
            this.ticketTypeRepo = ticketTypeRepo;
            this.ageGroupRepo = ageGroupRepo;
        }

        // GET: api/Tickets
        public IQueryable<Ticket> GetTickets()
        {
            return db.Tickets;
        }

        // GET: api/Ticket/GetTicketStatus
        [Authorize(Roles = "Controller")]
        [ResponseType(typeof(TicketVerificationBindingModel))]
        [Route("GetTicketStatus")]
        public IHttpActionResult GetTicket(string id)
        {
            int ticketId = 0;
            if (!Int32.TryParse(id, out ticketId))
            {
                return BadRequest();
            }

            Ticket ticket = ticketRepo.Get(ticketId);
            TicketVerificationBindingModel retVal = new TicketVerificationBindingModel();

            if (ticket == null) {
                retVal.Status = "danger";
                retVal.Description = "Invalid";
                return Ok(retVal);
            }

            // provera validnosti karte -> ako je vreme pregleda karte vece od vremena vazenja => karta nije validna
            if (DateTime.Now > ticket.ExpirationDate) {
                retVal.Status = "danger";
                retVal.Description = "Expired";
                return Ok(retVal);
            }

            retVal.Status = "success";
            retVal.Description = "Valid";
            return Ok(retVal);
        }

        
        [Authorize(Roles = "AppUser")]
        [ResponseType(typeof(List<UserTicketBindingModel>))]
        [Route("GetUserTickets")]
        public IHttpActionResult GetUserTickets(string username)
        {
            Passenger user = (Passenger)userRepo.GetAll().Where(x => x.Username == username).ToList().First();
            if (user == null)
            {
                return NotFound();
            }

            List<int> ticketIds = (from ticket in ticketRepo.GetAll()
                                   where ticket.PassengerId == user.Id
                                   select ticket.Id).ToList();

            var query = (from ticket in ticketRepo.GetAll().ToList()
                         join ticketType in ticketTypeRepo.GetAll().ToList() on ticket.TicketTypeId equals ticketType.Id
                         where ticket.PassengerId == user.Id
                         select new { Id = ticket.Id.ToString(), ticketType.TicketName, IssuingTime = ticket.IssuingDate.ToString(), ExpirationTime = ticket.ExpirationDate.ToString() });

            List<UserTicketBindingModel> retVal = new List<UserTicketBindingModel>();
            foreach (var temp in query)
            {
                retVal.Add(new UserTicketBindingModel() { TicketId = temp.Id, TicketType = temp.TicketName, IssuingTime = temp.IssuingTime, ExpirationTime = temp.ExpirationTime });
            }

            return Ok(retVal);
        }

        // GET: api/Tickets/BuyTicket
        [Authorize(Roles = "AppUser")]
        [System.Web.Http.HttpGet]
        [ResponseType(typeof(BuyTicketBindingModel))]
        [Route("BuyTicket")]
        public IHttpActionResult BuyTicket(string username, string ticketName)
        {
            Passenger user = (Passenger)userRepo.GetAll().Where(x => x.Username == username).ToList().First();
            if (user == null)
            {
                return NotFound();
            }

            AgeGroup ageGroup;

            // ako je status korisnika verifikovan, moze da kupuje tip karte kojoj starosnoj grupi pripada (Regular/Student/Pensioner)
            if (user.VerificationStatus == VerificationStatus.Successful)
            {
                ageGroup = ageGroupRepo.Get(user.AgeGroupId);
            }
            // ako nije verifikovan, ili ako je odbijen, moze da kupi samo Regular kartu
            else
            {
                ageGroup = ageGroupRepo.Get(1);         // samo Regular...
            }

            // najveci datum pocetka vazenja cenovnika, koji je manji od danasnjeg datuma
            var maxYear = priceListRepo.GetAll().Where(x => x.IssueDate < DateTime.Now).Max(x => x.IssueDate);

            // spajanje tabela za racunanje cene karte (aktuelni cenovnik, za adekvatnu starosnu grupu i zeljeni tip karte)
            var query = (from priceTicket in priceList_ticketTypeRepo.GetAll().ToList()
                         join p in priceListRepo.GetAll().ToList() on priceTicket.PriceListId equals p.Id
                         join ticketType in ticketTypeRepo.GetAll().ToList() on priceTicket.TicketTypeId equals ticketType.Id
                         where (priceTicket.AgeGroupId == ageGroup.Id && p.IssueDate.Date == maxYear.Date && ticketType.TicketName == ticketName)
                         select new { ticketType.Id, ticketType.TicketName, Price = priceTicket.Price * ageGroup.Coefficient }).ToList().First();

            if (query == null)
            {
                return NotFound();
            }

            BuyTicketBindingModel retVal = new BuyTicketBindingModel();

            if (user.VerificationStatus == VerificationStatus.Successful)
            {
                retVal.Description = String.Format($"Your account verification status is: {user.VerificationStatus.ToString()}.");
            }
            else
            {
                retVal.Description = String.Format($"Your account verification status is: {user.VerificationStatus.ToString()}.\n You can buy only Regular tickets.");
            }

            DateTime issuingTime = DateTime.Now;
            DateTime expiringTime;

            switch (ticketName)
            {
                case "Time Ticket":
                    expiringTime = new DateTime(issuingTime.Year, issuingTime.Month, issuingTime.Day, issuingTime.Hour + 1, issuingTime.Minute, issuingTime.Second);
                    break;
                case "Daily Ticket":
                    expiringTime = new DateTime(issuingTime.Year, issuingTime.Month, issuingTime.Day, 23, 59, 59);
                    break;
                case "Monthly Ticket":
                    expiringTime = new DateTime(issuingTime.Year, issuingTime.Month, DateTime.DaysInMonth(issuingTime.Year, issuingTime.Month), 23, 59, 59);
                    break;
                case "Annual Ticket":
                    expiringTime = new DateTime(issuingTime.Year, 12, DateTime.DaysInMonth(issuingTime.Year, issuingTime.Month), 23, 59, 59);
                    break;
                default:
                    expiringTime = new DateTime(issuingTime.Year, issuingTime.Month, issuingTime.Day, issuingTime.Hour + 1, issuingTime.Minute, issuingTime.Second);
                    break;
            }

            retVal.AgeGroup = ageGroup.GroupName;
            retVal.Price = query.Price.ToString();
            retVal.IssuingTime = DateTime.Now.ToString();
            retVal.ExpirationTime = expiringTime.ToString();
            retVal.TicketTypeId = query.Id.ToString();

            return Ok(retVal);
        }

        [Authorize(Roles = "AppUser")]
        [Route("ConfirmTicket")]
        [ResponseType(typeof(int))]
        public IHttpActionResult ConfirmTicket(BuyTicketBindingModel ticketInfo)
        {
            // kroz descritpion se potvrdjuje/odbija karta -> potvrdjuje se tako sto se u description stavi username
            if (ticketInfo.Description == String.Empty)
            {
                return Ok();                // korisnik je odustao od kupovine karte
            }

            // kroz description se prosledjuje username
            Passenger user = (Passenger)userRepo.GetAll().Where(x => x.Username == ticketInfo.Description).ToList().First();
            if (user == null)
            {
                return NotFound();
            }

            // kreiranje karte i cuvanje u bazu
            Ticket ticket = new Ticket(Int32.Parse(ticketInfo.TicketTypeId), user.Id, DateTime.Parse(ticketInfo.IssuingTime), DateTime.Parse(ticketInfo.ExpirationTime));
            db.Tickets.Add(ticket);
            db.SaveChanges();

            return Ok();
        }

        // PUT: api/Tickets/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutTicket(int id, Ticket ticket)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != ticket.Id)
            {
                return BadRequest();
            }

            db.Entry(ticket).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TicketExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Tickets
        [ResponseType(typeof(Ticket))]
        public IHttpActionResult PostTicket(Ticket ticket)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Tickets.Add(ticket);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = ticket.Id }, ticket);
        }

        // DELETE: api/Tickets/5
        [ResponseType(typeof(Ticket))]
        public IHttpActionResult DeleteTicket(int id)
        {
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return NotFound();
            }

            db.Tickets.Remove(ticket);
            db.SaveChanges();

            return Ok(ticket);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TicketExists(int id)
        {
            return db.Tickets.Count(e => e.Id == id) > 0;
        }
    }
}