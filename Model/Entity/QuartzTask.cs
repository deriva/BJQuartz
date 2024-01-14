using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BJ.Quartz.Model
{
    [SugarTable("QuartzTask")]
    public class QuartzTask
    {
        /// <summary>
        /// 任务名称 
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, ColumnDescription = "任务名称")]
        public long Id { get; set; }

        /// <summary>
        /// 任务名称 
        /// </summary>
        [SugarColumn(ColumnDescription = "任务名称", SqlParameterDbType = "varchar", Length = 64)]
        public string TaskName { get; set; }
        /// <summary>
        /// 任务组 
        /// </summary>
        [SugarColumn(ColumnDescription = "任务组", SqlParameterDbType = "varchar", Length = 64)]
        public string GroupName { get; set; }
        /// <summary>
        /// 定时策略 
        /// </summary>
        [SugarColumn(ColumnDescription = "定时策略", SqlParameterDbType = "varchar", Length = 64)]
        public string Interval { get; set; }
        /// <summary>
        /// 请求地址 
        /// </summary>
        [SugarColumn(ColumnDescription = "请求地址", SqlParameterDbType = "varchar", IsNullable = true, Length = 512)]
        public string ApiUrl { get; set; }
        /// <summary>
        /// 授权key 
        /// </summary>
        [SugarColumn(ColumnDescription = "授权key", SqlParameterDbType = "varchar", IsNullable = true, Length = 512)]
        public string AuthKey { get; set; }
        /// <summary>
        /// 授权的值 
        /// </summary>
        [SugarColumn(ColumnDescription = "授权key", SqlParameterDbType = "varchar", IsNullable = true, Length = 2048)]
        public string AuthValue { get; set; }
        /// <summary>
        /// 描述 
        /// </summary>
        [SugarColumn(ColumnDescription = "描述", SqlParameterDbType = "varchar", IsNullable = true, Length = 512)]
        public string Describe { get; set; }
        /// <summary>
        /// 请求方式 
        /// </summary>
        [SugarColumn(ColumnDescription = "描述", SqlParameterDbType = "varchar", IsNullable = true, Length = 16)]
        public string RequestType { get; set; }
        /// <summary>
        /// 最后允许时间 
        /// </summary>
        public DateTime? LastRunTime { get; set; }
        /// <summary>
        /// 状态 
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// Job类型 
        /// </summary>
        [SugarColumn(ColumnDescription = "描述", SqlParameterDbType = "varchar",IsNullable =true, Length = 64)]
        public string JobClassName { get; set; }
        /// <summary>
        /// 任务类型0
        /// </summary>
        public int TaskType { get; set; }

        /// <summary>
        ///来自其他表的ID
        /// </summary>
        public long? FKId { get; set; }


        /// <summary>
        /// 来自其他表
        /// </summary>
        public string FKTable { get; set; }

        public void SetId(long id)
        {
            this.Id = id;

        }

    }
}
