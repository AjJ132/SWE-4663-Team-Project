namespace TeamProject.Data
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    public class ProjectPhaseHours
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EntryID { get; set; }
        public int ProjectID { get; set; }
        public int ReqAnaHrs { get; set; }
        public int DesignHrs { get; set; }
        public int CodingHrs { get; set; }
        public int TestingHrs { get; set; }
        public int ProjManaHrs { get; set; }

        public ProjectPhaseHours()
        {

        }

        public ProjectPhaseHours(int projectid, int reqanahrs, int designhrs, int codinghrs, int testinghrs, int projmanahrs)
        {
            ProjectID = projectid;
            ReqAnaHrs = reqanahrs;
            DesignHrs = designhrs;
            CodingHrs = codinghrs;
            TestingHrs = testinghrs;
            ProjManaHrs = projmanahrs;
        }
    }
}