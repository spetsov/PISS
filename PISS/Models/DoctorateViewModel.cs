using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PISS.Models
{
    public class DoctorateViewModel
    {
        public string[] SelectedConsultantsUserIds { get; set; }
        public string[] SelectedLeadTeachersUserIds { get; set; }
        public Doctorate Doctorate { get; set; }       
    }
}