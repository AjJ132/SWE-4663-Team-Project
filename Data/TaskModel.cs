namespace TeamProject.Data
{
    public class TaskModel
    {
        public string TaskName { get; set; }
        public SortedList<DateTime, bool> Dates { get; set; }

        public int UserID { get; set; }
        public string Status { get; set; }
    }

}