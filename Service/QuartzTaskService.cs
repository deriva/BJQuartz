using BJ.Quartz.Helpers;
using BJ.Quartz.Model;
using Quartz.Util;
using SqlSugar;

namespace BJ.Quartz.Service
{
    public class QuartzTaskService
    {
        private SqlSugarClient db = LocalSqliteManage.SqliteDB;
        public async Task<ApiPageResult<QuartzTask>> GetPageListAsync(QuartzTaskDto parm)
        {
            var expr = Expressionable.Create<QuartzTask>();
            if (parm.Id.HasValue) expr = expr.And(x => x.Id == parm.Id);
            if (!parm.TaskName.IsNullOrWhiteSpace()) expr = expr.And(x => x.TaskName == parm.TaskName);
            if (!parm.GroupName.IsNullOrWhiteSpace()) expr = expr.And(x => x.GroupName == parm.GroupName);
            if (!parm.RequestType.IsNullOrWhiteSpace()) expr = expr.And(x => x.RequestType == parm.RequestType);

            var totalCount = 0; parm.OrderBy = "Id desc";

            var tuples = db.Queryable<QuartzTask>().Where(expr.ToExpression()).OrderBy(!string.IsNullOrEmpty(parm.OrderBy) ? parm.OrderBy : null).ToPageList(parm.Page, parm.PageSize, ref totalCount);
            return ResultHelper<QuartzTask>.ToPageResponse(tuples, totalCount, parm.Page, parm.PageSize);
        }

        public async Task UpdateLastTime(long id)
        {
            var ff = await db.Queryable<QuartzTask>().Where(x => x.Id == id).FirstAsync();
            ff.LastRunTime = DateTime.Now;
            await db.Updateable<QuartzTask>(ff).ExecuteCommandAsync();
        }

        public void Init()
        {
            using (var db = LocalSqliteManage.SqliteDB)
            {
                //建库
                db.DbMaintenance.CreateDatabase();
                //建表 
                db.CodeFirst.InitTables<QuartzTask, JobLog>();//定时任务表 

                db.Deleteable<JobLog>(x => x.CreationTime <= DateTime.Now.AddDays(-30)).ExecuteCommand();
            }
        }
    }
}
