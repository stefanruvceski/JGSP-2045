using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebApp.Models
{
    /// <summary>
    /// povezna tabela izmedju PriceList i TicketType (cenovnik i stavka),  
    /// sa stranim kljucem na AgeGroup, da bi mogli da racunamo CENA * KOEFICIJENT
    /// </summary>
    public class PriceList_TicketType
    {
        public int Id { get; set; }

        [ForeignKey("PriceList")]
        public int PriceListId { get; set; }
        public PriceList PriceList { get; set; }

        [ForeignKey("TicketType")]
        public int TicketTypeId { get; set; }
        public TicketType TicketType { get; set; }

        [ForeignKey("AgeGroup")]
        public int AgeGroupId { get; set; }
        public AgeGroup AgeGroup { get; set; }

        public double Price { get; set; }

        public PriceList_TicketType() { }
    }
}