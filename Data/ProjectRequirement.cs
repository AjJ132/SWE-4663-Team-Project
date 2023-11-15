

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace TeamProject.Data
{
    public class ProjectRequirement
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RequirementId { get; set; }

        [Required]
        public int ProjectId { get; set; }

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

        //Default Constructor
        public ProjectRequirement()
        {

        }

        //DEV CONSTRUCTOR
        public ProjectRequirement(int projectid, string title, string description, RequirementType type, RequirementStatus status, string priority, string effortEstimation, string notes)
        {
            ProjectId = projectid;
            Title = title;
            Description = description;
            Type = type;
            Status = status;
            Priority = priority;
            EffortEstimation = effortEstimation;
            Notes = notes;
        }
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