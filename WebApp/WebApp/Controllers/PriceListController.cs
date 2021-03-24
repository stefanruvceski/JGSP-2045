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

namespace WebApp.Controllers
{
    public class PriceListController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private IPriceListRepository priceListRepo;
        private ITicketTypeRepository ticketTypeRepo;
        private IAgeGroupRepository ageGroupRepo;
        private IPriceList_TicketTypeRepository priceList_ticketTypeRepo;

        private static readonly Object lockObj = new Object();

        public PriceListController(IPriceListRepository priceListRepo, ITicketTypeRepository ticketTypeRepo, IAgeGroupRepository ageGroupRepo, IPriceList_TicketTypeRepository priceList_ticketTypeRepo)
        {
            this.priceListRepo = priceListRepo;
            this.ticketTypeRepo = ticketTypeRepo;
            this.ageGroupRepo = ageGroupRepo;
            this.priceList_ticketTypeRepo = priceList_ticketTypeRepo;
        }

        // GET: api/PriceList
        public IQueryable<PriceList_TicketType> GetPriceList_TicketTypes()
        {
            return db.PriceList_TicketTypes;
        }

        // GET: api/PriceList/5
        [ResponseType(typeof(List<PriceListBindingModel>))]
        public IHttpActionResult GetPriceList_TicketType(int ageGroupId)
        {
            AgeGroup ageGroup = ageGroupRepo.Get(ageGroupId);

            if (ageGroup == null)
            {
                return NotFound();
            }

            // najveci datum pocetka vazenja cenovnika, koji je manji od danasnjeg datuma
            var maxYear = priceListRepo.GetAll().Where(x => x.IssueDate < DateTime.Now).Max(x => x.IssueDate);

            // spajanje tabela, uz uslov da pripada odgovarajucoj grupi i da se radi o tekucem cenovniku
            var query = (from priceTicket in priceList_ticketTypeRepo.GetAll().ToList()
                         join p in priceListRepo.GetAll().ToList() on priceTicket.PriceListId equals p.Id
                         join ticketType in ticketTypeRepo.GetAll().ToList() on priceTicket.TicketTypeId equals ticketType.Id
                         where (priceTicket.AgeGroupId == ageGroup.Id && p.IssueDate.Date == maxYear.Date)
                         select new { ticketType.TicketName, Price = priceTicket.Price * ageGroup.Coefficient }).ToList();

            if (query.Count == 0)
            {
                return NotFound();
            }

            List<PriceListBindingModel> retVal = new List<PriceListBindingModel>();
            foreach (var elem in query)
            {
                retVal.Add(new PriceListBindingModel() { TicketName = elem.TicketName, Price = (int)(elem.Price) });
            }

            return Ok(retVal);
        }

        // POST: api/PriceList/AddPricelist
        [Authorize(Roles = "Admin")]
        public IHttpActionResult AddPricelist(PricelistBindingModel model)
        {
            lock (lockObj)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // svaka izmena cenovnika mora da traje najmanje 3 meseca
                if (DateTime.Parse(model.IssueDate).AddMonths(3) >= DateTime.Parse(model.ExpireDate))
                {
                    return BadRequest("Expire date must be greater then issue date...");
                }

                // pravljenje novog cenovnika i dodavanje u bazu
                PriceList priceList = new PriceList() { IssueDate = DateTime.Parse(model.IssueDate), ExpireDate = DateTime.Parse(model.ExpireDate) };
                db.PriceLists.Add(priceList);
                db.SaveChanges();

                // dobavljanje ID-a priceList-a iz baze
                int priceListId = priceListRepo.GetAll().Where(x => x.IssueDate == priceList.IssueDate && x.ExpireDate == priceList.ExpireDate).ToList().First().Id;

                // dobavljanje ID-jeva svih tipova karata
                // ovo mi ne treba ako ce mi slati 4 razlicite cene, mada bi lepse bilo sa dva for-a
                //List<int> ticketTypeIds = (List<int>)ticketTypeRepo.GetAll().Select(x => x.Id).ToList();

                // dobavljanje ID-jeva svih starosnih grupa
                List<int> ageGroupIds = ageGroupRepo.GetAll().Select(x => x.Id).ToList();

                // pravljenje nove torke u poveznoj tabeli (za svaku starosnu grupu i za svaki tip karte) i dodavanje u bazu
                // TIME TICKET
                foreach (int ageGroup in ageGroupIds)
                {
                    PriceList_TicketType pl_tt = new PriceList_TicketType()
                    {
                        PriceListId = priceListId,
                        TicketTypeId = 1,
                        AgeGroupId = ageGroup,
                        Price = model.TimeTicketPrice
                    };
                    db.PriceList_TicketTypes.Add(pl_tt);
                }

                // DAILY TICKET
                foreach (int ageGroup in ageGroupIds)
                {
                    PriceList_TicketType pl_tt = new PriceList_TicketType()
                    {
                        PriceListId = priceListId,
                        TicketTypeId = 2,
                        AgeGroupId = ageGroup,
                        Price = model.DailyTicketPrice
                    };
                    db.PriceList_TicketTypes.Add(pl_tt);
                }

                // MONTHLY TICKET
                foreach (int ageGroup in ageGroupIds)
                {
                    PriceList_TicketType pl_tt = new PriceList_TicketType()
                    {
                        PriceListId = priceListId,
                        TicketTypeId = 3,
                        AgeGroupId = ageGroup,
                        Price = model.MonthlyTicketPrice
                    };
                    db.PriceList_TicketTypes.Add(pl_tt);
                }

                // ANNUAL TICKET
                foreach (int ageGroup in ageGroupIds)
                {
                    PriceList_TicketType pl_tt = new PriceList_TicketType()
                    {
                        PriceListId = priceListId,
                        TicketTypeId = 4,
                        AgeGroupId = ageGroup,
                        Price = model.AnnualTicketPrice
                    };
                    db.PriceList_TicketTypes.Add(pl_tt);
                }

                db.SaveChanges();

                return Ok();
            }
        }

        

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool PriceList_TicketTypeExists(int id)
        {
            return db.PriceList_TicketTypes.Count(e => e.Id == id) > 0;
        }
    }
}