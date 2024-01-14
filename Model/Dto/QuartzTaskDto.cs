namespace BJ.Quartz.Model
{
    public class QuartzTaskDto:PageParm
    {
        /// <summary>
        /// 定时任务 
        /// </summary>
        public long? Id { get; set; }
        /// <summary>
        /// 任务名称 
        /// </summary>
        public string TaskName { get; set; }
        /// <summary>
        /// 任务组 
        /// </summary>
        public string GroupName { get; set; }
        /// <summary>
        /// 定时策略 
        /// </summary>
        public string Interval { get; set; }
        /// <summary>
        /// 请求地址 
        /// </summary>
        public string ApiUrl { get; set; }
        /// <summary>
        /// 授权key 
        /// </summary>
        public string AuthKey { get; set; }
        /// <summary>
        /// 授权的值 
        /// </summary>
        public string AuthValue { get; set; }
        /// <summary>
        /// 描述 
        /// </summary>
        public string Describe { get; set; }
        /// <summary>
        /// 请求方式 
        /// </summary>
        public string RequestType { get; set; }
        /// <summary>
        /// 最后允许时间 
        /// </summary>
        public DateTime? LastRunTime { get; set; }
        /// <summary>
        /// 状态 
        /// </summary>
        public int? Status { get; set; }
        /// <summary>
        /// Job类型 
        /// </summary>
        public string JobClassName { get; set; }
        /// <summary>
        /// 任务类型 
        /// </summary>
        public int? TaskType { get; set; }


    }
}
