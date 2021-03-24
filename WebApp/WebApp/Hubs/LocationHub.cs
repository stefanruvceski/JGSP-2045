using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Timers;
using WebApp.Models;
using WebApp.Persistence.Repository;
using WebApp.Persistence;

namespace WebApp.Hubs
{
    [HubName("notifications")]
    public class LocationHub : Hub
    {
        private static IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<LocationHub>();

        private string line;
        private int lineNum;
        private string lineDirection;
        int busId1;
        int busId2;

        private IStationRepository stationRepo;
        private IStationLineRepository stationLineRepo;
        private IBusRepository busRepo;

        private static Dictionary<string, Timer> timers = new Dictionary<string, Timer>();

        private List<Tuple<double, double>> coords = new List<Tuple<double, double>>();
        private List<Tuple<double, double>> coordsOrg = new List<Tuple<double, double>>();
        private List<Tuple<double, double>> coordsInv = new List<Tuple<double, double>>();

        private List<Tuple<double, double>> coords2 = new List<Tuple<double, double>>();
        private List<Tuple<double, double>> coordsOrg2 = new List<Tuple<double, double>>();
        private List<Tuple<double, double>> coordsInv2 = new List<Tuple<double, double>>();

        private Tuple<double, double> stationA = new Tuple<double, double>(0, 0);
        private Tuple<double, double> stationB = new Tuple<double, double>(0, 0);
        private Tuple<double, double> currentCoords = new Tuple<double, double>(0, 0);

        private Tuple<double, double> stationA2 = new Tuple<double, double>(0, 0);
        private Tuple<double, double> stationB2 = new Tuple<double, double>(0, 0);
        private Tuple<double, double> currentCoords2 = new Tuple<double, double>(0, 0);

        private int i = 0, j = 1, ii = 0, jj = 0;
        private string frontVal;
        private double deltaX = 0, deltaY = 0;
        private double deltaX2 = 0, deltaY2 = 0;
        private bool start = false;

        public LocationHub(IStationRepository stationRepo, IStationLineRepository stationLineRepo, IBusRepository busRepository)
        {
            this.stationRepo = stationRepo;
            this.stationLineRepo = stationLineRepo;
            this.busRepo = busRepository;
        }

        public void GetTime()
        {
            //Svim klijentima se salje setRealTime poruka
            Clients.All.SendCoordinates(frontVal);
        }

        public void TimeServerUpdates(string line)
        {
            this.line = line;

            // formiranje naziva inverzne linije, da se bus moze vratiti
            lineNum = Int32.Parse(line.Substring(0, line.Length-1));
            lineDirection = line.Last().ToString();

            string inverseLine = lineNum.ToString();
            if (lineDirection == "A")
            {
                inverseLine += "B";
            }
            else
            {
                inverseLine += "A";
            }

            //////////////////////////////////////
            ///
            var bus = (from sl in busRepo.GetAll().ToList()

                       where sl.LineId == line
                       select  sl).ToList();

            //////////////////////////////////////

            // dobavljanje koordinata svih stanica zadate linije
            var query = (from sl in stationLineRepo.GetAll().ToList()
                         join s in stationRepo.GetAll().ToList() on sl.StationId equals s.Id
                         where sl.LineId == line
                         select new { s.XCooridinate, s.YCoordinate }).ToList();

            // dobavljanje koordinata svih stanica inverzne linije
            var queryInv = (from sl in stationLineRepo.GetAll().ToList()
                            join s in stationRepo.GetAll().ToList() on sl.StationId equals s.Id
                            where sl.LineId == inverseLine
                            select new { s.XCooridinate, s.YCoordinate }).ToList();

            foreach (var s in query)
            {
                coords.Add(new Tuple<double, double>(s.XCooridinate, s.YCoordinate));
                coords2.Add(new Tuple<double, double>(s.XCooridinate, s.YCoordinate));
                coordsOrg.Add(new Tuple<double, double>(s.XCooridinate, s.YCoordinate));
                coordsOrg2.Add(new Tuple<double, double>(s.XCooridinate, s.YCoordinate));
            }

            foreach (var s in queryInv)
            {
                coordsInv.Add(new Tuple<double, double>(s.XCooridinate, s.YCoordinate));
                coordsInv2.Add(new Tuple<double, double>(s.XCooridinate, s.YCoordinate));
            }

            frontVal = line + "_";

            //// zastita da postoje bar dve stanice
            if (coords.Count >= 2)
            {
                // za prvi autobus
                stationA = new Tuple<double, double>(bus[0].XCooridinate, bus[0].YCoordinate);
                stationB = new Tuple<double, double>(bus[0].NextStationX, bus[0].NextStationY);
                currentCoords = stationA;
                busId1 = bus[0].Id;

                // za drugi autobus
                ii = (int)(coords.Count / 2);
                jj = (int)(coords.Count / 2) + 1;
                stationA2 = new Tuple<double, double>(bus[1].XCooridinate, bus[1].YCoordinate);
                stationB2 = new Tuple<double, double>(bus[1].NextStationX, bus[1].NextStationY);
                currentCoords2 = stationA2;
                busId2 = bus[1].Id;
            }

            Timer timer = new Timer();
            timer.Interval = 1000;
            timer.Start();
            timer.Elapsed += OnTimedEvent;
            if (timers.ContainsKey(line))
            {
                StopTimeServerUpdates(line);
            }
            timers[line] = timer;
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            frontVal = line;

            frontVal += "_" + currentCoords.Item1 + "_" + currentCoords.Item2;

            // pokusaj 1. -> autobus se krece od stanice do stanice, diskretno
            //stationA = coords[(++i) % coords.Count];
            //stationB = coords[(++j) % coords.Count];

            deltaX += (stationB.Item1 - stationA.Item1) / 3;
            deltaY += (stationB.Item2 - stationA.Item2) / 3;

            double newX = stationA.Item1 + deltaX;
            double newY = stationA.Item2 + deltaY;

            currentCoords = new Tuple<double, double>(newX, newY);

            // provera da li je dosao do stanice
            if (currentCoords.Item1 == stationB.Item1 && currentCoords.Item2 == stationB.Item2)
            {
                // provera da li je dosao do poslednje stanice
                if (currentCoords.Item1 == coords.Last().Item1 && currentCoords.Item2 == coords.Last().Item2)
                {
                    // provera da li postoji inverzna linija
                    if (coordsInv.Count != 0)
                    {
                        // ako postoji, treba krenuti od pocetka inverznom linijom
                        coords = coordsInv;
                        coordsInv = coordsOrg;
                        coordsOrg = coords;
                    }

                    // ako ne postoji, krece od pocetka
                    i = 0;
                    j = 1;
                    stationA = coords[i];
                    stationB = coords[j];
                }
                else
                {
                    // ako nije, granice se pomeraju i algoritam se nastavlja
                    stationA = coords[(++i) % coords.Count];
                    stationB = coords[(++j) % coords.Count];
                }

                //Task.Factory.StartNew(() => {
                    List<Bus> buses = busRepo.GetAll().Where(x => x.LineId.ToUpper() == line).ToList();
                    Bus b1 = buses[0];
                    Bus b2 = buses[1];

                    b1.XCooridinate = stationA.Item1;
                    b1.YCoordinate = stationA.Item2;
                    b1.NextStationX = stationB.Item1;
                    b1.NextStationY = stationB.Item2;

                    b2.XCooridinate = stationA2.Item1;
                    b2.YCoordinate = stationA2.Item2;
                    b2.NextStationX = stationB2.Item1;
                    b2.NextStationY = stationB2.Item2;

                    db.Entry(b1).State = System.Data.Entity.EntityState.Modified;
                    db.Entry(b2).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                //});

                deltaX = 0;
                deltaY = 0;
            }

            GetTime();

            frontVal = line + line.Last();

            frontVal += "_" + currentCoords2.Item1 + "_" + currentCoords2.Item2;

            // pokusaj 1. -> autobus se krece od stanice do stanice, diskretno
            //stationA = coords[(++i) % coords.Count];
            //stationB = coords[(++j) % coords.Count];

            deltaX2 += (stationB2.Item1 - stationA2.Item1) / 3;
            deltaY2 += (stationB2.Item2 - stationA2.Item2) / 3;

            double newX2 = stationA2.Item1 + deltaX2;
            double newY2 = stationA2.Item2 + deltaY2;

            currentCoords2 = new Tuple<double, double>(newX2, newY2);

            // provera da li je dosao do stanice
            if (currentCoords2.Item1 == stationB2.Item1 && currentCoords2.Item2 == stationB2.Item2)
            {
                // provera da li je dosao do poslednje stanice
                if (currentCoords2.Item1 == coords2.Last().Item1 && currentCoords2.Item2 == coords2.Last().Item2)
                {
                    if (coordsInv2.Count != 0)
                    {
                        // ako jeste, treba krenuti od pocetka inverznom linijom
                        coords2 = coordsInv2;
                        coordsInv2 = coordsOrg2;
                        coordsOrg2 = coords2;
                    }
                    ii = 0;
                    jj = 1;
                    stationA2 = coords2[ii];
                    stationB2 = coords2[jj];
                }
                else
                {
                    // ako nije, granice se pomeraju i algoritam se nastavlja
                    stationA2 = coords2[(++ii) % coords2.Count];
                    stationB2 = coords2[(++jj) % coords2.Count];
                }

                deltaX2 = 0;
                deltaY2 = 0;
            }

            GetTime();
        }
        private ApplicationDbContext db = new ApplicationDbContext();
        public void StopTimeServerUpdates(string line)
        {
            timers[line].Stop();
        }

        public void NotifyAdmins(int clickCount)
        {
            hubContext.Clients.Group("Admins").userClicked($"Clicks: {clickCount}");
        }

        public override Task OnConnected()
        {
            //if (Context.User.IsInRole("Admin"))
            //{
                Groups.Add(Context.ConnectionId, "Admins");
            //}

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            //if (Context.User.IsInRole("Admin"))
            //{
                Groups.Remove(Context.ConnectionId, "Admins");
            //}

            return base.OnDisconnected(stopCalled);
        }
    }
}