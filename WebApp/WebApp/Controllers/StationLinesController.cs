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
    [Authorize]
    [RoutePrefix("api/StationLine")]
    public class StationLinesController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ILineRepository lineRepo;
        private IStationRepository stationRepo;
        private IStationLineRepository stationLineRepo;

        private static readonly Object lockObj = new Object();

        public StationLinesController(ILineRepository lineRepo, IStationRepository stationRepo, IStationLineRepository stationLineRepo)
        {
            this.lineRepo = lineRepo;
            this.stationRepo = stationRepo;
            this.stationLineRepo = stationLineRepo;
        }

        // POST: api/StationLine/AddNewLine
        [Authorize(Roles = "Admin")]
        [Route("AddNewLine")]
        public IHttpActionResult AddNewLine(LineStBindingModel model)
        {
            lock (lockObj)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // provera da li vec postoji linija sa tim ID-jem
                if (lineRepo.GetAll().Where(x => x.Id.ToUpper() == model.LineId.ToUpper()).Count() != 0)
                {
                    return BadRequest("Line already exists...");
                }

                // kreiranje linije i dodavanje u bazu
                LineType type;
                Enum.TryParse(model.LineType, out type);
                Line line = new Line() { Id = model.LineId, LineType = type, Description = model.Description, Color = model.Color, IsActive = true };
                db.Lines.Add(line);
                db.SaveChanges();

                // kreiranje stanica i dodavanje u bazu (u dve tabele)
                foreach (StationBindingModel st in model.Stations)
                {
                    bool temp = false;
                    if (st.IsStation.ToUpper() == "YES")
                    {
                        temp = true;
                    }

                    Station station = new Station() { StationName = st.Name, Address = st.Address, XCooridinate = st.XCoordinate, YCoordinate = st.YCoordinate, IsStation = temp };
                    db.Stations.Add(station);           // dodavanje linije u tabelu Lines
                    db.SaveChanges();

                    int stationId = stationRepo.GetAll().Where(x => x.Address == station.Address).ToList().First().Id;      // nalazenje id-a dodate stanice
                    StationLine stationLine = new StationLine() { LineId = line.Id, StationId = stationId };
                    db.StationLines.Add(stationLine);
                    db.SaveChanges();
                }

                ///////////////////////////////////////////////////////////////
                //dodavanje dva busa u bazu buseva
                int secondBus = (int)(model.Stations.Count / 2);

                Bus b1 = new Bus() { LineId = model.LineId, XCooridinate = model.Stations[0].XCoordinate, YCoordinate= model.Stations[0].YCoordinate, NextStationX = model.Stations[1].XCoordinate, NextStationY = model.Stations[1].YCoordinate };
                Bus b2 = new Bus() { LineId = model.LineId, XCooridinate = model.Stations[secondBus].XCoordinate, YCoordinate = model.Stations[secondBus].YCoordinate, NextStationX = model.Stations[secondBus+1].XCoordinate, NextStationY = model.Stations[secondBus+1].YCoordinate };
                db.Buses.Add(b1);
                db.Buses.Add(b2);
                db.SaveChanges();
                ///////////////////////////////////////////////////////////////
                return Ok();
            } 
        }

        // GET: api/StationLine/GetLine
        [AllowAnonymous]
        [System.Web.Http.HttpGet]
        [Route("GetLine")]
        [ResponseType(typeof(List<LineStBindingModel>))]
        public IHttpActionResult GetLine(string lineId)
        {
            Line line = lineRepo.GetAll().Where(x => x.Id.ToUpper() == lineId.ToUpper()).ToList().First();
            if (line == null)
            {
                return NotFound();
            }

            LineStBindingModel retVal = new LineStBindingModel();
            retVal.LineId = line.Id;
            retVal.LineType = line.LineType.ToString();
            retVal.Description = line.Description;
            retVal.Color = line.Color;
            retVal.Stations = new List<StationBindingModel>();

            var query = (from stationLine in stationLineRepo.GetAll().ToList()
                         join station in stationRepo.GetAll().ToList() on stationLine.StationId equals station.Id
                         where stationLine.LineId.ToUpper() == line.Id.ToUpper()
                         select new { station.StationName, station.Address, station.XCooridinate, station.YCoordinate, station.IsStation }).ToList();

            foreach (var s in query)
            {
                string temp = "No";
                if (s.IsStation)
                {
                    temp = "Yes";
                }

                string address = String.Empty;
                if (s.StationName != s.Address)
                {
                    address = s.Address;
                }

                retVal.Stations.Add(new StationBindingModel() { Name = s.StationName, Address = address, XCoordinate = s.XCooridinate, YCoordinate = s.YCoordinate, IsStation = temp});
            }

            return Ok(retVal);
        }

        // GET: api/StationLine/GetLines
        [AllowAnonymous]
        [System.Web.Http.HttpGet]
        [Route("GetLines")]
        [ResponseType(typeof(List<string>))]
        public IHttpActionResult GetLines()
        {
            List<string> lineIds = lineRepo.GetAll().Where(x => x.IsActive == true).Select(x => x.Id).ToList();

            if (lineIds.Count == 0)
            {
                return BadRequest("There are no lines...");
            }

            return Ok(lineIds);
        }

        // GET: api/StationLine/DeleteLine
        [Authorize(Roles = "Admin")]
        [System.Web.Http.HttpGet]
        [Route("DeleteLine")]
        public IHttpActionResult DeleteLine(string lineId)
        {
            lock (lockObj)
            {
                var lines = lineRepo.GetAll().Where(x => x.Id.ToUpper() == lineId.ToUpper()).ToList();
                if (lines.Count == 0)
                {
                    return BadRequest("Line doesn't exist...");
                }

                // logicko brisanje preko flag-a
                Line line = lines.First();
                line.IsActive = false;

                // cuvanje izmene u bazi
                db.Entry(line).State = EntityState.Modified;
                db.SaveChanges();

                return Ok();
            }
        }

        // POST: api/StationLine/EditLine
        [Authorize(Roles = "Admin")]
        [Route("EditLine")]
        public IHttpActionResult EditLine(LineStBindingModel model)
        {
            lock (lockObj)
            {
                var lines = lineRepo.GetAll().Where(x => x.Id.ToUpper() == model.LineId.ToUpper()).ToList();
                if (lines.Count == 0)
                {
                    return BadRequest("Line doesn't exist...");
                }

                // setovanje novih podataka za liniju
                Line line = lines.First();

                LineType type;
                Enum.TryParse(model.LineType, out type);
                line.LineType = type;
                line.Description = model.Description;
                line.Color = model.Color;
                line.IsActive = true;

                // cuvanje izmene o liniji u bazi
                db.Entry(line).State = EntityState.Modified;
                db.SaveChanges();

                // brisanje starih vrednosti iz povezne tabele izmedju linije i stanica
                List<StationLine> oldLinePath = stationLineRepo.GetAll().Where(x => x.LineId.ToUpper() == line.Id.ToUpper()).ToList();
                /*if(oldLinePath.Count != 0)
                {
                    db.StationLines.RemoveRange(oldLinePath);
                }   */

                foreach (StationLine sl in oldLinePath)
                {
                    db.StationLines.Attach(sl);
                    db.StationLines.Remove(sl);
                }
                db.SaveChanges();

                foreach (StationBindingModel st in model.Stations)
                {
                    bool temp = false;
                    if (st.IsStation.ToUpper() == "YES")
                    {
                        temp = true;
                    }

                    // pravljenje nove stanice (korisnik crta novu putanju)
                    Station station = new Station() { StationName = st.Name, Address = st.Address, XCooridinate = st.XCoordinate, YCoordinate = st.YCoordinate, IsStation = temp };
                    db.Stations.Add(station);           // dodavanje linije u tabelu Lines
                    db.SaveChanges();

                    // povezivanje nove stanice i odgovarajuce linije
                    int stationId = stationRepo.GetAll().Where(x => x.XCooridinate == station.XCooridinate && x.YCoordinate == station.YCoordinate).ToList().First().Id;      // nalazenje id-a dodate stanice
                    StationLine stationLine = new StationLine() { LineId = line.Id, StationId = stationId };
                    db.StationLines.Add(stationLine);
                    db.SaveChanges();
                }

                return Ok();
            }
        }

        // GET: api/StationLines
        public IQueryable<StationLine> GetStationLines()
        {
            return db.StationLines;
        }

        // GET: api/StationLines/5
        [ResponseType(typeof(StationLine))]
        public IHttpActionResult GetStationLine(int id)
        {
            StationLine stationLine = db.StationLines.Find(id);
            if (stationLine == null)
            {
                return NotFound();
            }

            return Ok(stationLine);
        }

        // PUT: api/StationLines/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutStationLine(int id, StationLine stationLine)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != stationLine.Id)
            {
                return BadRequest();
            }

            db.Entry(stationLine).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StationLineExists(id))
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

        // POST: api/StationLines
        [ResponseType(typeof(StationLine))]
        public IHttpActionResult PostStationLine(StationLine stationLine)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.StationLines.Add(stationLine);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = stationLine.Id }, stationLine);
        }

        // DELETE: api/StationLines/5
        [ResponseType(typeof(StationLine))]
        public IHttpActionResult DeleteStationLine(int id)
        {
            StationLine stationLine = db.StationLines.Find(id);
            if (stationLine == null)
            {
                return NotFound();
            }

            db.StationLines.Remove(stationLine);
            db.SaveChanges();

            return Ok(stationLine);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool StationLineExists(int id)
        {
            return db.StationLines.Count(e => e.Id == id) > 0;
        }
    }
}