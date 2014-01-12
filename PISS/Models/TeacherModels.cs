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
        public string[] SelectedLeadTeachersUserIds { get; set; }
        public Diploma Diploma { get; set; }       
    }

    public class Grade
    {
        public Grade(int? value, string display)
        {
            this.Value = value;
            this.Display = display;
        }
        public int? Value { get; set; }
        public string Display { get; set; }
    }

    public class GridDiplomaViewModel
    {
        public int Id { get; set; }
        public string StudenEmail { get; set; }
        public int? StudentId { get; set; }
        public ApprovedStatus Approved { get; set; }
        public int? Grade { get; set; }
        public DateTime? GraduationDate { get; set; }
        public string ReviewerEmail { get; set; }

    }
}