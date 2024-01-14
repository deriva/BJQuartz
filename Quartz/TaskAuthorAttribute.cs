namespace BJ.Quartz.Quartz
{
    public class TaskAuthorAttribute : System.Attribute
    {
        public string Name { get; set; }
        public string Role { get; set; }
    }

    /// <summary>
    /// string taskName任务名称 , string groupName ="" 组名称, string interval="" 时间间隔
    /// </summary>
    public class TaskCustomAttribute : System.Attribute
    {
        /// <summary>
        /// 任务组配置
        /// </summary>
        /// <param name="taskName"></param>
        /// <param name="groupName"></param>
        /// <param name="interval"></param>
        public TaskCustomAttribute(string taskName, string groupName ="", string interval="")
        {
            TaskName = taskName;
            GroupName = groupName;
            Interval = interval;
            //if (string.IsNullOrEmpty(interval))
            //{
            //    Interval = AppSettings.Configuration["DayFrozenDeviceJob:Corn"];// ConfigHelper.GetValue<string>("DayFrozenDeviceJob:Corn");
            //}
        }
        public string TaskName { get; set; }
        public string GroupName { get; set; }
        public string Interval { get; set; }
    }
}
