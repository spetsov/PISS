using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PISS.Models
{
    public class DiplomaViewModel
    {
        public string[] SelectedConsultantsUserIds { get; set; }
        public string[] SelectedDefenceCommisionMembersUserIds { get; set; }
        public Diploma Diploma { get; set; }
    }
}