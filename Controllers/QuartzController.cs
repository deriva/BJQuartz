using BJ.Quartz.Helpers;
using BJ.Quartz.Model;
using BJ.Quartz.Quartz;
using BJ.Quartz.Service;
using Microsoft.AspNetCore.Mvc;
using Quartz;
using Quartz.Spi;
using Quartz.Util;
using SqlSugar;

namespace BJ.Quartz.Controllers
{


    public class QuartzController : Controller
    {
        SqlSugarClient db = LocalSqliteManage.SqliteDB;
        JobLogService _joblogService = new JobLogService();
        QuartzTaskService _quartzTaskService = new QuartzTaskService();
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly IJobFactory _jobFactory;
        public QuartzController(ISchedulerFactory schedulerFactory, IJobFactory jobFactory)
        {
            this._jobFactory = jobFactory;
            this._schedulerFactory = schedulerFactory;
            //this._quartzTaskService = quartzTaskService;
            //this._joblogService = jobLogService;
        }
        /// <summary>
        /// 测试
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ApiResult Test()
        {
            return ResultHelper.ToSuccess("这是一个测试");
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Page(string id = "")
        {
            var viewname = id;
            //if (!string.IsNullOrWhiteSpace(module))
            //    viewname = module;
            //if (!string.IsNullOrWhiteSpace(module) && !string.IsNullOrWhiteSpace(id))
            //viewname = id + "/" + module;
            return View(viewname);
        }

        [HttpGet]
        public ApiResult InitTables()
        {
            var db = LocalSqliteManage.SqliteDB;
            //建库
            db.DbMaintenance.CreateDatabase();
            //建表 
            db.CodeFirst.InitTables<QuartzTask, JobLog>();//定时任务表 
            return ResultHelper.ToResponse(1, "初始化表成功");
        }
        /// <summary>
        /// 获取所有的作业
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiPageResult<QuartzTask>> GetJobs([FromQuery] QuartzTaskDto dto)
        => await _quartzTaskService.GetPageListAsync(dto);

        /// <summary>
        /// 获取作业运行日志
        /// </summary>
        /// <param name="taskName"></param>
        /// <param name="groupName"></param>
        /// <param name="page"></param>
        /// <returns></returns> 
        public async Task<ApiPageResult<JobLog>> GetRunLog([FromQuery] JobLogDto search) => await _joblogService.GetPageListAsync(search);

        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="QuartzTask"></param>
        /// <returns></returns> 
        [HttpPost]
        public async Task<ApiResult> Add([FromBody] QuartzTask info)
        {
            if (info.Id > 0) return await Update(info);
            if (info.JobClassName.IsNullOrWhiteSpace())
                info.JobClassName = GetDefaultJobClassName();
            info.Status = TriggerState.Normal.GetHashCode();
            info.Id = SnowFlakeSingle.instance.NextId();
            info.LastRunTime = Convert.ToDateTime("1990-01-01");
            info.FKId = 0;
            info.FKTable = "";
            await db.Insertable<QuartzTask>(info).ExecuteCommandAsync();
            await info.AddJob(_schedulerFactory, false, _jobFactory);
            return ResultHelper.ToSuccess("操作成功");
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ApiResult> Remove([FromBody] QuartzTask info)
        {
            UpdateStatus(info.Id, TriggerState.None);
            await _schedulerFactory.Remove(info); return ResultHelper.ToSuccess("操作成功");
        }
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ApiResult> Update([FromBody] QuartzTask info)
        {
            info.Status = TriggerState.Normal.GetHashCode();
            var sub = await db.Queryable<QuartzTask>().FirstAsync(x => x.Id == info.Id);
            sub.TaskName = info.TaskName;
            sub.GroupName = info.GroupName;
            sub.ApiUrl = info.ApiUrl;
            sub.Interval = info.Interval;
            sub.RequestType = info.RequestType;
            sub.Describe = info.Describe;
            sub.AuthKey = info.AuthKey;
            sub.AuthValue = info.AuthValue;

            await db.Updateable<QuartzTask>(sub).ExecuteCommandAsync();
            await _schedulerFactory.Update(sub); return ResultHelper.ToSuccess("操作成功");
        }
        /// <summary>
        /// 暂停
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ApiResult> Pause([FromBody] QuartzTask info)
        {
            if (info.JobClassName.IsNullOrWhiteSpace())
                info.JobClassName = GetDefaultJobClassName();
            UpdateStatus(info.Id, TriggerState.Paused);
            await _schedulerFactory.Pause(info); return ResultHelper.ToSuccess("操作成功");
        }
        private string GetDefaultJobClassName()
        {
            return "BJ.Quartz.Quartz.HttpResultfulJob";
        }
        /// <summary>
        /// 开启
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ApiResult> Start([FromBody] QuartzTask info)
        {
            UpdateStatus(info.Id, TriggerState.Normal);
            await _schedulerFactory.Start(info); return ResultHelper.ToSuccess("操作成功");
        }


        /// <summary>
        /// 立即运行
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ApiResult> Run([FromBody] QuartzTask info)
        {
            await _schedulerFactory.Run(info); return ResultHelper.ToSuccess("操作成功");
        }

        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        private async void UpdateStatus(long id, TriggerState status)
        {
            if (status == TriggerState.None)
            {
                await db.Deleteable<QuartzTask>(x => x.Id == id).ExecuteCommandAsync();
                await db.Deleteable<JobLog>(x => x.JobId == id).ExecuteCommandAsync();
                return;
            }
            var model = db.Queryable<QuartzTask>().First(x => x.Id == id);
            model.Status = status.GetHashCode();
            await db.Updateable<QuartzTask>(model).ExecuteCommandAsync();
        }
    }
}
