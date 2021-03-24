using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using WebApp.Models;

namespace WebApp.Persistence.Repository
{
    public class StationLineRepository : Repository<StationLine, int>, IStationLineRepository
    {
        public StationLineRepository(DbContext context) : base(context)
        {
        }
    }
}