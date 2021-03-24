using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using WebApp.Models;

namespace WebApp.Persistence.Repository
{
    public class AgeGroupRepository : Repository<AgeGroup, int>, IAgeGroupRepository
    {
        public AgeGroupRepository(DbContext context) : base(context)
        {
        }
    }
}