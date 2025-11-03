using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Services.Entity
{
    public class Note
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string CreatedAt { get; set; }

        public int? CSLId { get; set; }

        public int? CustomerId { get; set; }

        public int? AppointmentId { get; set; }
        
        public string CompanyId { get; set; }
        public string UserId { get; set; }
        public int? TagId { get; set; }
        public string UserName { get; set; }

    }
}