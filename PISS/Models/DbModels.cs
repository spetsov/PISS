using System;
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
        public DbSet<DefenceCommision> DefenceCommisions { get; set; }
        public DbSet<StudentTeaher> StudentsTeahers { get; set; }
        public DbSet<StudentConsultant> StudentsConsultants { get; set; }
        public DbSet<File> Files { get; set; }
    }

    [Table("UserProfile")]
    public class UserProfile
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        public string Email { get; set; }
    }

    [Table("Diploma")]
    public class Diploma
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? АssignmentFileId { get; set; }
        [ForeignKey("АssignmentFileId")]
        public File АssignmentFile { get; set; }
        public PISS.Models.ApprovedStatus Approved { get; set; }
        public DateTime? DefenceDate { get; set; }
        public int StudentId { get; set; }
        [ForeignKey("StudentId")]
        public UserProfile Student { get; set; }
        public int? Grade { get; set; }
        public int? ReviewFileId { get; set; }
        [ForeignKey("ReviewFileId")]
        public File ReviewFile { get; set; }
        public int? ThesisId { get; set; }
        [ForeignKey("ThesisId")]
        public Thesis Thesis { get; set; }
        public ICollection<DefenceCommision> DefenceCommisions { get; set; } 
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


    [Table("DefenceCommision")]
    public class DefenceCommision
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int TeacherId { get; set; }
        [ForeignKey("TeacherId")]
        public UserProfile Teacher { get; set; }
        public int? DiplomaId { get; set; }
        [ForeignKey("DiplomaId")]
        public Diploma Diploma { get; set; }
    }

    [Table("StudentTeacher")]
    public class StudentTeaher
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int StudentId { get; set; }
        [ForeignKey("StudentId")]
        public UserProfile Student { get; set; }
        public int? LeadTeacherId { get; set; }
        [ForeignKey("LeadTeacherId")]
        public UserProfile LeadTeacher { get; set; }
    }

    [Table("StudentConsultant")]
    public class StudentConsultant
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int StudentId { get; set; }
        [ForeignKey("StudentId")]
        public UserProfile Student { get; set; }
        public int? ConsultantId { get; set; }
        [ForeignKey("ConsultantId")]
        public UserProfile Consultant { get; set; }
    }

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
