namespace TeamProject.Data
{
    public class TaskModel
    {
        public string TaskName { get; set; }
        public Dictionary<DateTime, bool> Dates { get; set; }
    }

}