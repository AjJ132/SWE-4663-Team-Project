

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace TeamProject.Data
{
    public class ProjectRequirement
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RequirementId { get; set; }

        [Required, MaxLength(50)]
        public string Title { get; set; }

        [Required, MaxLength(500)]
        public string Description { get; set; }

        [Required]
        public RequirementType Type { get; set; }

        [Required]
        public RequirementStatus Status { get; set; }

        [Required]
        public string Priority { get; set; }

        [Required, MaxLength(50)]
        public string EffortEstimation { get; set; }

        [MaxLength(500)]
        public string Notes { get; set; }
    }

    public enum RequirementType
    {
        Functional,
        NonFunctional
    }

    public enum RequirementStatus
    {
        Proposed,
        Accepted,
        InProgress,
        Implemented,
        Deferred,
        Rejected
    }

}