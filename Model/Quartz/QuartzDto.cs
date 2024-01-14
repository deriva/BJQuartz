namespace BJ.Quartz.Model.Quartz
{
    public  class QuartzDto: PageParm
    { /// <summary>
      /// 任务名称 
      /// </summary>
        public string TaskName { get; set; }
        /// <summary>
        /// 任务组 
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// 租户ID
        /// </summary>
        public Guid? TenantId { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool? IsDeleted { get; set; }

        /// <summary>
        /// 任务类型0:http,1:业务代码 2:sql
        /// </summary>
        public int? TaskType { get; set; }
    }
}
