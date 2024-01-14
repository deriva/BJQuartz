namespace BJ.Quartz.Model
{
    public class PageParm
    {
        /// <summary>
        /// 当前页
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        /// 每页总条数
        /// </summary>
        public int PageSize { get; set; } = 20;

        /// <summary>
        /// 排序字段
        /// </summary>
        public string OrderBy { get; set; } = "CreatedTime desc";

        /// <summary>
        /// 排序方式
        /// </summary>
        public string Sort { get; set; }

    }
    public class JobLogDto : PageParm
    {
        /// <summary> 
        /// 任务名称 
        /// </summary> 
        public long? Id { get; set; }
        /// <summary> 
        /// 任务名称 
        /// </summary> 
        public string Name { get; set; }
        /// <summary> 
        /// 执行结果 1 :成功  2： 异常 
        /// </summary> 
        public int? ExecuteResult { get; set; }
        /// <summary> 
        /// 创建时间 
        /// </summary> 
        public DateTime? CreationTime { get; set; }
        public DateTime? CreationTime2 { get; set; }
        /// <summary> 
        /// 描述 
        /// </summary> 
        public string Description { get; set; }

        public long? JobId { get; set; }
    }

}
