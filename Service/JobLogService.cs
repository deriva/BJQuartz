using BJ.Quartz.Helpers;
using BJ.Quartz.Model;
using Quartz.Util;
using SqlSugar;

namespace BJ.Quartz.Service
{
    public class JobLogService  
    {
        private static SqlSugarClient db = LocalSqliteManage.SqliteDB;
        public async Task<ApiPageResult<JobLog>> GetPageListAsync(JobLogDto parm)
        {
            var expr = Expressionable.Create<JobLog>();
            if (parm.JobId.HasValue) expr = expr.And(x => x.JobId == parm.JobId);
            var totalCount = 0; parm.OrderBy = "Id desc";

            var tuples = db.Queryable<JobLog>().Where(expr.ToExpression()).OrderBy(!string.IsNullOrEmpty(parm.OrderBy) ? parm.OrderBy : null).ToPageList(parm.Page, parm.PageSize, ref totalCount);
            return ResultHelper<JobLog>.ToPageResponse(tuples, totalCount, parm.Page, parm.PageSize);
     
        }

        /// <summary>
        /// 添加日志
        /// </summary>
        /// <param name="quartzTask"></param>
        /// <param name="content"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public async Task AddJobLog(QuartzTask quartzTask, string content, int result = 0)
        {
            if (content.IsNullOrWhiteSpace()) return;
            if (content.Length > 512) content = content.Substring(0, 512);
            var info = (new JobLog()
            {
                JobId = quartzTask.Id,
                Name = quartzTask.TaskName,
                CreationTime = DateTime.Now,
                Description = content,
                Id = SnowFlakeSingle.Instance.NextId(),
                ExecuteResult = result
            }); 
            await db.Insertable<JobLog>(info).ExecuteCommandAsync();
        }

        /// <summary>
        /// 添加日志
        /// </summary>
        /// <param name="quartzTask"></param>
        /// <param name="content"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public async Task AddJobLog(long jobid, string taskname, string content, int result = 0)
        {
            if (content.IsNullOrWhiteSpace()) return;
            if (content.Length > 512) content = content.Substring(0, 512);
            var info = (new JobLog()
            {
                JobId = jobid,
                Name = taskname,
                CreationTime = DateTime.Now,
                Id = SnowFlakeSingle.Instance.NextId(),
                Description = content,
                ExecuteResult = result
            });
            await db.Insertable<JobLog>(info).ExecuteCommandAsync();
        }
    }
}
