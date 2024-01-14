using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BJ.Quartz.Model
{
    [SugarTable("JobLog")]
    public class JobLog
    { /// <summary> 
      /// 任务名称 
      /// </summary> 
        [SugarColumn(IsPrimaryKey = true, ColumnDescription = "Id")]
        public long Id { get; set; }
        /// <summary> 
        /// 任务名称 
        /// </summary> 
        [SugarColumn(ColumnDescription = "任务名称", SqlParameterDbType = "varchar", Length = 32)]
        public string Name { get; set; }
        /// <summary> 
        /// 执行结果 1 :成功  2： 异常 
        /// </summary> 
        [SugarColumn(ColumnDescription = "执行结果 1 :成功  2： 异常 ")]
        public int ExecuteResult { get; set; }
        /// <summary> 
        /// 创建时间 
        /// </summary> 
        [SugarColumn(ColumnDescription = "创建时间 ")]
        public DateTime CreationTime { get; set; }

        /// <summary> 
        /// 描述 
        /// </summary> 
        [SugarColumn(ColumnDescription = "描述 ",SqlParameterDbType ="varchar",Length =-1, IsNullable = true)]
        public string Description { get; set; }


        [SugarColumn(ColumnDescription = "JobId ", IsNullable = false)]
        public long JobId { get; set; }

        public JobLog() { }
        public void SetId(long id)
        {
            Id = id;
            CreationTime = DateTime.Now;
        }

        public JobLog(long id, int result, string name, string description)
        {
            Id = id;
            Name = name;
            ExecuteResult = result;
            Description = description;
            CreationTime = DateTime.Now;
        }
    }
}
