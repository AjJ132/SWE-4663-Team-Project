using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeamProject.Data
{
    public class Risk
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int ProjectID { get; set; }

        [Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RiskID { get; set; }

        [Required, MaxLength(50)]
        public string RiskName { get; set; }

        [Required, MaxLength(500)]
        public string RiskDescription { get; set; }

        [Required]
        public int RiskSeverity { get; set; }

        [Required]
        public bool RiskStatus { get; set; }

        [Required, MaxLength(250)]
        public string RiskMitigation { get; set; }

        //Default Constructor
        public Risk()
        {

        }

        //DEV CONSTRUCTOR
        public Risk(int na)
        {
            ProjectID = 1;
            RiskName = "Test Risk";
            RiskDescription = "This is a test risk.";
            RiskSeverity = 1;
            RiskStatus = false;
            RiskMitigation = "Test Mitigation Strategy";
        }
    }
}