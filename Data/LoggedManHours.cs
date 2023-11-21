
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;




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

        public string Phase { get; set; }

        //Default Constructor
        public LoggedManHours()
        {

        }

        //DEV CONSTRUCTOR
        public LoggedManHours(int projectid, int teammemberid, int hours, DateTime date, int requirementid, string phase)
        {
            ProjectID = projectid;
            TeamMemberID = teammemberid;
            Hours = hours;
            Date = date;
            RequirementID = requirementid;
            Phase = phase;
        }
    }
}