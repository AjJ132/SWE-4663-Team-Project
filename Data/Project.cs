

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


        //Ignored relationship objects
        [NotMapped]
        public List<ProjectRequirement> Requirements { get; set; }

        [NotMapped]
        public List<Risk> Risks { get; set; }

        [NotMapped]
        public List<ProjectTeamMember> TeamMembers { get; set; }

        [NotMapped]
        public List<LoggedManHours> LoggedManHours { get; set; }

        //default constructor
        public Project()
        {

        }

        //DEVELOPMENT CONSTRUCTOR
        public Project(int na)
        {
            this.Name = "Test Project";
            this.Description = "This is a test project.";
            this.ProjectOwnerID = 1;
            this.ProjectOwnerName = "Test Owner";
            this.ProjectPhase = 0;
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