
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

        //Default Constructor
        public LoggedManHours()
        {

        }

        //DEV CONSTRUCTOR
        public LoggedManHours(int na, int requirementID)
        {
            ProjectID = 1;
            TeamMemberID = 1;
            //random number between 1 and 10
            Random rand = new Random();
            Hours = rand.Next(1, 10);

            Date = DateTime.Now;

            Date.AddDays(rand.Next(-20, -1));
            RequirementID = requirementID;
        }
    }
}