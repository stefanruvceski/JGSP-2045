using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using WebApp.Models;

namespace WebApp.Persistence.Repository
{
    public class PriceList_TicketTypeRepository : Repository<PriceList_TicketType, int>, IPriceList_TicketTypeRepository
    {
        public PriceList_TicketTypeRepository(DbContext context) : base(context)
        {
        }
    }
}