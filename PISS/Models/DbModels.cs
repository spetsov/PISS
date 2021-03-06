﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;

namespace PISS.Models
{
    public class SystemContext : DbContext
    {
        public SystemContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Diploma> Diplomas { get; set; }
        public DbSet<DefenceCommisionMember> DefenceCommisionsMembers { get; set; }
        public DbSet<Consultant> Consultants { get; set; }
        public DbSet<LeadTeacher> LeadTeachers { get; set; }
        public DbSet<Doctorate> Doctorates { get; set; }
        public DbSet<File> Files { get; set; }
        public DbSet<WorkExperience> WorkExperience { get; set; }
        
    }

    [Table("UserProfile")]
    public class UserProfile
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    #region Student related
    [Table("Diploma")]
    public class Diploma
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public PISS.Models.ApprovedStatus Approved { get; set; }

        public DateTime? DefenceDate { get; set; }

        public DateTime? GraduationDate { get; set; }

        [Display(Name = "Grade")]
        public int? Grade { get; set; }

        public int StudentId { get; set; }
        [ForeignKey("StudentId")]
        public UserProfile Student { get; set; }

        public int? АssignmentFileId { get; set; }
        [ForeignKey("АssignmentFileId")]
        public File АssignmentFile { get; set; }

        public int? ApproverId { get; set; }
        [ForeignKey("ApproverId")]
        public UserProfile Approver { get; set; }

        public int? ReviewFileId { get; set; }
        [ForeignKey("ReviewFileId")]
        public File ReviewFile { get; set; }

        public int? ReviewerId { get; set; }
        [ForeignKey("ReviewerId")]
        public UserProfile Reviewer { get; set; }        

        [Column(TypeName = "ntext")]
        [MaxLength]
        public string ReviewNotes { get; set; }

        public int? ThesisId { get; set; }
        [ForeignKey("ThesisId")]
        public Thesis Thesis { get; set; }

        public ICollection<DefenceCommisionMember> DefenceCommisionMembers { get; set; }

        public ICollection<Consultant> Consultants { get; set; }

        public ICollection<LeadTeacher> LeadTeachers { get; set; }
    }

    [Table("Thesis")]
    public class Thesis
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column(TypeName = "ntext")]
        [MaxLength]
        [Required]
        [Display(Name = "Text")]
        public string Text { get; set; }

        [Column(TypeName = "ntext")]
        [MaxLength]
        [Display(Name = "Summary in English")]
        public string Summary_EN { get; set; }

        [Column(TypeName = "ntext")]
        [MaxLength]
        [Display(Name = "Summary in Bulgarian")]
        public string Summary_BG { get; set; }

        public int? SourceCodeFileId { get; set; }

        [ForeignKey("SourceCodeFileId")]
        public File SourceCodeFile { get; set; }
    }


    [Table("DefenceCommisionMember")]
    public class DefenceCommisionMember
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int? MemberId { get; set; }
        [ForeignKey("MemberId")]
        public UserProfile Member { get; set; }
    }

    [Table("Consultants")]
    public class Consultant
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int TeacherId { get; set; }
        [ForeignKey("TeacherId")]
        public UserProfile Teacher { get; set; }
    }

    [Table("LeadTeacher")]
    public class LeadTeacher
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int? TeacherId { get; set; }
        [ForeignKey("TeacherId")]
        public UserProfile Teacher { get; set; }
    }
    #endregion

    #region Work Experience

    [Table("WorkExperience")]
    public class WorkExperience
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int StudentId { get; set; }
        [ForeignKey("StudentId")]
        public UserProfile Student { get; set; }

        public int? SuggestionFileId { get; set; }
        [ForeignKey("SuggestionFileId")]
        public File SuggestionFile { get; set; }

        public bool SuggestionApproved { get; set; }

        public bool GradeApproved { get; set; }

        public int? GradeFileId { get; set; }
        [ForeignKey("GradeFileId")]
        public File GradeFile { get; set; }

    }

    #endregion

    #region Doctorant related
    [Table("Doctorate")]
    public class Doctorate
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int? GeneralWorkPlanFileId { get; set; }
        [ForeignKey("GeneralWorkPlanFileId")]
        public File GeneralWorkPlanFile { get; set; }

        public int? PersonalWorkPlanFileId { get; set; }
        [ForeignKey("PersonalWorkPlanFileId")]
        public File PersonalWorkPlanFile { get; set; }

        public int? YearByYearPlanFileId { get; set; }
        [ForeignKey("YearByYearPlanFileId")]
        public File YearByYearPlanFile { get; set; }

        public int? AttestationFileId { get; set; }
        [ForeignKey("AttestationFileId")]
        public File AttestationFile { get; set; }

        public int? ReviewerId { get; set; }
        [ForeignKey("ReviewerId")]
        public UserProfile Reviewer { get; set; }  

        public int DoctorantId { get; set; }
        [ForeignKey("DoctorantId")]
        public UserProfile Doctorant { get; set; }

        public ICollection<Consultant> Consultants { get; set; }
        public ICollection<LeadTeacher> LeadTeachers { get; set; }
    }
    #endregion

    [Table("Files")]
    public class File
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public Byte[] Content { get; set; }
        public string MimeType { get; set; }
        public string FileName { get; set; }
    }
}
