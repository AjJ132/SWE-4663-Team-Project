

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeamProject.Data
{
    public class Project
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required, MaxLength(50)]
        public string Name { get; set; }
        [MaxLength(500)]
        public string Description { get; set; }
        [Required]
        public int ProjectOwnerID { get; set; }
        [Required]
        public string ProjectOwnerName { get; set; }

        [Required]
        public int ProjectPhase { get; set; }

        [Required]
        public DateTime ProjectStartDate { get; set; }


        //Ignored relationship objects
        [NotMapped]
        public List<ProjectRequirement> Requirements { get; set; }

        [NotMapped]
        public List<Risk> Risks { get; set; }

        [NotMapped]
        public List<ProjectTeamMember> TeamMembers { get; set; }

        [NotMapped]
        public List<LoggedManHours> LoggedManHours { get; set; }

        [NotMapped]
        public double ProjectCompletion { get; set; }
        [NotMapped]
        public int TotalRequirements { get; set; }
        [NotMapped]
        public int CompletedRequirements { get; set; }
        [NotMapped]
        public int HighPriorityTasks { get; set; }

        // [NotMapped] //used for the ghannt chart on home page
        // public List<CustomGhanntChartColumn> GhanntChartColumns { get; set; }
        //default constructor

        [NotMapped]
        public List<DateTime> ProjectDates { get; set; } = new List<DateTime>();
        [NotMapped]
        public List<TaskModel> Tasks { get; set; } = new List<TaskModel>();
        public Project()
        {

        }

        //DEVELOPMENT CONSTRUCTOR
        public Project(string name, string description, int projectOwnerID, string projectOwnerName, int projectPhase, DateTime projectStartDate)
        {
            Name = name;
            Description = description;
            ProjectOwnerID = projectOwnerID;
            ProjectOwnerName = projectOwnerName;
            ProjectPhase = projectPhase;
            ProjectStartDate = projectStartDate;
            ProjectDates = new List<DateTime>();    
        }

        public void SetProjectStats()
        {
            //First Calculate the totla number of requirements
            this.TotalRequirements = Requirements.Count;
            Console.WriteLine("Total Requirements: " + this.TotalRequirements);

            //if there are no requirements, set the project completion to 0
            if (this.TotalRequirements == 0)
            {
                ProjectCompletion = 0;
                CompletedRequirements = 0;
                this.TotalRequirements = 0;
            }
            else
            {
                //if there are requirements, calculate the number of completed requirements
                foreach (var requirement in this.Requirements)
                {
                    if (requirement.Status == RequirementStatus.Implemented)
                    {
                        this.CompletedRequirements++;
                    }

                    if(requirement.Priority.Equals("High"))
                    {
                        this.HighPriorityTasks++;
                    }
                }

                //calculate the project completion percentage
                this.ProjectCompletion = Math.Round((double)this.CompletedRequirements / this.TotalRequirements * 100, 2);


            }
        }

    }

    public enum ProjectPhase
    {
        Requirement_Analysis,
        Design,
        Development,
        Testing,
        Implementation
    }
}