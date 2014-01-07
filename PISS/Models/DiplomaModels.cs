using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PISS.Models
{
    public class DiplomaTeacherViewModel
    {
        public Diploma Diploma { get; set; }

        [Display(Name = "Add notes (Optional)")]
        public string Notes { get; set; }
    }

    public enum ApprovedStatus
    {
        Unapproved = 0,
        ConditionallyApproved = 1,
        Approved = 2
    }
}