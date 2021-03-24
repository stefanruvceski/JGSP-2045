using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using WebApp.Models;

namespace WebApp.Persistence.Repository
{
    public class BusRepository : Repository<Bus, int>, IBusRepository
    {
        public BusRepository(DbContext context) : base(context)
        {
        }
    }
}