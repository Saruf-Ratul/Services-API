using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Services.Models
{
    public class CSLImageDTO
    {
        public int CustomerId { get; set; }
        public int AppointmentId { get; set; }
        public int CSLId { get; set; }
        public string CompanyId { get; set; }
        public string  TagName { get; set; }
        public List<Image>  ImageList { get; set; }
    }

    public class Image
    {
        public string ImageName { get; set; }
        public string ImageBase64 { get; set; }
        public string Description { get; set; }
        public string CreatedAt { get; set; }
    }
}