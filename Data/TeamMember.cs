using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeamProject.Data
{
    public class TeamMember
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MemberID { get; set; }
        [Required, MaxLength(50)]
        public string Name { get; set; }

        //Default Constructor
        public TeamMember()
        {

        }

        //DEV CONSTRUCTOR
        public TeamMember(int na)
        {
            Name = "Test Name";
        }
    }
}