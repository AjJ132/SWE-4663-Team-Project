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

        [NotMapped]
        public Dictionary<DateTime, object> ProjectDates { get; set; } = new Dictionary<DateTime, object>();


        //Default Constructor
        public ProjectTeamMember()
        {

        }

        //DEV CONSTRICTOR
        public ProjectTeamMember(int projectid, int teammemberid, string teammembername, int permissionlevel, string permissionlevelname)
        {
            ProjectID = projectid;
            TeamMemberID = teammemberid;
            TeamMemberName = teammembername;
            PermissionLevel = permissionlevel;
            PermissionLevelName = permissionlevelname;
        }
    }
}