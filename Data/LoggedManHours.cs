
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeamProject.Data
{
    public class LoggedManHours
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EntryID { get; set; }
        [Required]
        public int ProjectID { get; set; }
        [Required]
        public int TeamMemberID { get; set; }

        [Required]
        public int Hours { get; set; }
        [Required]
        public DateTime Date { get; set; }

        public int RequirementID { get; set; }
    }
}