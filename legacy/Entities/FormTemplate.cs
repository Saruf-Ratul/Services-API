using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Services.Entity
{
    public class FormTemplate
    {
        public int Id { get; set; }
        public string CompanyID { get; set; }
        public string TemplateName { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }

   
        public bool IsAutoAssignEnabled { get; set; }
        public string AutoAssignServiceTypes { get; set; }

       
        public string FormStructure { get; set; } 
        public bool RequireSignature { get; set; }
        public bool RequireTip { get; set; }
        public bool IsActive { get; set; }

        
        public string CreatedBy { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDateTime { get; set; }
    }
}