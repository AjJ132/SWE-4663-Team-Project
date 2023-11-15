using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeamProject.Data
{
    public class ProjectTeamMember
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EntryID { get; set; }
        [Required]
        public int ProjectID { get; set; }
        [Required]
        public int TeamMemberID { get; set; }
        [Required]
        public string TeamMemberName { get; set; }
        [Required]
        public int PermissionLevel { get; set; }
        [Required]
        public string PermissionLevelName { get; set; }


        //Default Constructor
        public ProjectTeamMember()
        {

        }

        //DEV CONSTRICTOR
        public ProjectTeamMember(int na)
        {
            ProjectID = 1;
            TeamMemberID = 1;
            TeamMemberName = "Test Name";
            PermissionLevel = 1;
            PermissionLevelName = "Member";
        }
    }
}