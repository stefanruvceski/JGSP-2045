using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApp.Persistence.Repository;

namespace WebApp.Persistence.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        ITicketRepository Tickets { get; set; }

        IAgeGroupRepository AgeGroups { get; set; }

        IBusRepository Buses { get; set; }

        ILineRepository Lines { get; set; }

        IPriceListRepository PriceLists { get; set; }

        IStationRepository Stations { get; set; }

        ITicketTypeRepository TicketTypes { get; set; }

        ITimeTableRepository TimeTables { get; set; }

        IPriceList_TicketTypeRepository PriceList_TicketTypes { get; set; }

        IUserRepository Users { get; set; }

        IStationLineRepository StationLines { get; set; }

        int Complete();
    }
}
