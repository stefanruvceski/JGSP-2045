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
    public class TimeTablesController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ITimeTableRepository timeTableRepo;
        private ILineRepository lineRepo;

        private static readonly Object lockObj = new Object();

        public TimeTablesController(ITimeTableRepository timeTableRepo, ILineRepository lineRepo)
        {
            this.timeTableRepo = timeTableRepo;
            this.lineRepo = lineRepo;
        }

        // korisnik prvo bira tip linije (gradska / prigradska), i onda mu se u nekoj listi pojave sve linije koje pripadaju datom tipu
        // GET: api/Lines/5
        [Route("api/Lines")]
        [ResponseType(typeof(List<LineBindingModel>))]
        public IHttpActionResult GetLines(string lineType)
        {
            // string mora biti IDENTICAN kao u enumeraciji (CASE SENSITIVE)
            LineType lineTypeId;
            if (!Enum.TryParse(lineType, out lineTypeId))
            {
                return NotFound();
            }

            var query = (from line in lineRepo.GetAll().ToList()
                         where line.LineType == lineTypeId
                         select new { LineId = line.Id, line.Description });
            List<LineBindingModel> list = new List<LineBindingModel>();
            foreach (var item in query)
            {
                list.Add(new LineBindingModel(){ Lineid=item.LineId, Description=item.Description});
            }

            return Ok(list);
        }

        // vraca red voznje za izabranu liniju i izabran dan
        // GET: api/TimeTables/5
        [ResponseType(typeof(List<string>))]
        public IHttpActionResult GetTimeTable(string lineId, string day)
        {
            // string mora biti IDENTICAN kao u enumeraciji (CASE SENSITIVE)
            DayInWeek dayId;
            if (!Enum.TryParse(day, out dayId))
            {
                return NotFound();
            }

            // upit vraca red voznje za izabranu liniju i izabran dan
            var query = (from timetable in timeTableRepo.GetAll()
                         where timetable.LineId == lineId && timetable.Day == dayId
                         select timetable.Schedule).ToList();

            if (query.Count == 0)
            {
                return NotFound();
            }

            string schedule = query.First();
            if (schedule == null)
            {
                return Ok("");                // da ne bih splitovao null (ako nije popunjeno polje u bazi), bice exception
            }

            return Ok(schedule.Split('|'));     // povratna vrednost je lista splitovana po | (da bi se svaki sat predstavio u posebnom redu)
        }

        // GET: api/TimeTables/DeleteTimetable
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("api/TimeTables/DeleteTimetable")]
        public IHttpActionResult DeleteTimetable(string lineId, string day)
        {
            lock (lockObj)
            {
                DayInWeek dayInWeek;
                Enum.TryParse(day, out dayInWeek);
                List<TimeTable> timetables = timeTableRepo.GetAll().Where(x => x.LineId.ToUpper() == lineId.ToUpper() && x.Day == dayInWeek).ToList();

                // ako ne postoji, nemam sta da brisem
                if (timetables.Count != 0)
                {
                    TimeTable timetable = timetables.First();
                    timetable.Schedule = String.Empty;
                    // brisanje se svodi na postavljanje opisa na prazan string
                    // cuvanje izmene u bazi
                    db.Entry(timetable).State = EntityState.Modified;
                    db.SaveChanges();
                }

                return Ok();
            }
        }

        // metoda radi ADD ili UPDATE, zavisi od potrebe
        // POST: api/TimeTables/SetTimetable
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("api/TimeTables/SetTimetable")]
        public IHttpActionResult SetTimetable(SetTimetableBindingModel model)
        {
            lock (lockObj)
            {
                DayInWeek day;
                Enum.TryParse(model.Day, out day);

                List<TimeTable> timetables = timeTableRepo.GetAll().Where(x => x.LineId.ToUpper() == model.LineId.ToUpper() && x.Day == day).ToList();

                // ako ne postoji ovakav timetable, pravim novi i dodajem ga u bazu
                if (timetables.Count == 0)
                {
                    TimeTable timetable = new TimeTable() { Day = day, Schedule = model.Schedule, LineId = model.LineId };
                    db.TimeTables.Add(timetable);
                    db.SaveChanges();
                }
                // ako postoji, radi se izmena reda voznje za odredjeni dan
                else
                {
                    TimeTable timetable = timetables.First();
                    timetable.Schedule = model.Schedule;
                    db.Entry(timetable).State = EntityState.Modified;
                    db.SaveChanges();
                }

                return Ok();
            }
        }

        // GET: api/TimeTables
        public IQueryable<TimeTable> GetTimeTables()
        {
            return db.TimeTables;
        }

        // PUT: api/TimeTables/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutTimeTable(int id, TimeTable timeTable)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != timeTable.Id)
            {
                return BadRequest();
            }

            db.Entry(timeTable).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TimeTableExists(id))
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

        // POST: api/TimeTables
        [ResponseType(typeof(TimeTable))]
        public IHttpActionResult PostTimeTable(TimeTable timeTable)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.TimeTables.Add(timeTable);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = timeTable.Id }, timeTable);
        }

        // DELETE: api/TimeTables/5
        [ResponseType(typeof(TimeTable))]
        public IHttpActionResult DeleteTimeTable(int id)
        {
            TimeTable timeTable = db.TimeTables.Find(id);
            if (timeTable == null)
            {
                return NotFound();
            }

            db.TimeTables.Remove(timeTable);
            db.SaveChanges();

            return Ok(timeTable);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TimeTableExists(int id)
        {
            return db.TimeTables.Count(e => e.Id == id) > 0;
        }
    }
}