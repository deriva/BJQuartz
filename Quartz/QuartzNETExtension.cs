using BJ.Quartz.Helper;
using BJ.Quartz.Model;
using BJ.Quartz.Quartz;
using BJ.Quartz.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using Quartz.Impl.Triggers;
using Quartz.Spi;
using System.Reflection;

namespace BJ.Quartz.Quartz
{
    public static class QuartzNETExtension
    {
        private static List<QuartzTask> _taskList = new List<QuartzTask>();
        private static IHttpContextAccessor _accessor;
        private static IServiceProvider services;
        /// <summary>
        /// 初始化作业
        /// </summary>
        /// <param name="applicationBuilder"></param>
        /// <param name="env"></param>
        /// <returns></returns>
        public static async Task<IApplicationBuilder> UseQuartz(this IApplicationBuilder applicationBuilder, IWebHostEnvironment env)
        {
            services = applicationBuilder.ApplicationServices;

            ISchedulerFactory _schedulerFactory = services.GetService<ISchedulerFactory>();
            _accessor = services.GetService<IHttpContextAccessor>();
            Utility.HttpContext.Configure(_accessor);


            var _repository = new QuartzTaskService();
           
            _accessor = services.GetService<IHttpContextAccessor>();

            int errorCount = 0;
            string errorMsg = "";
            QuartzTask options = null;
            try
            {
                //加载配置文件里的--走的是http请求的
                _taskList = await LocalSqliteManage.SqliteDB.Queryable<QuartzTask>().Where(x => x.Status == 0).ToListAsync();
                if (_taskList == null) _taskList = new List<QuartzTask>();

                #region 反射获取所有带有CustomAttribute特性的类并调用对应方法
                var classes = Assembly.GetExecutingAssembly().GetTypes()
                    .Where(type => type.GetCustomAttributes<TaskCustomAttribute>().Any());

                foreach (var clazz in classes)
                {
                    //获取标记CustomAttribute的实例
                    var attr = clazz.GetCustomAttributes<TaskCustomAttribute>().First();
                    if (!string.IsNullOrEmpty(attr.Interval))
                        _taskList.Add(new QuartzTask() { TaskName = attr.TaskName, GroupName = attr.GroupName, Interval = attr.Interval ?? "", JobClassName = clazz.FullName });
                }

                #endregion

                //加载非数据库的
                //_taskList.Add(new QuartzTask() { TaskName = "tt", GroupName = "ttt", Interval = "0/10 * * * * ?", JobClassName = "BJ.Quartz.Quartz.TestJob" });
               
                for(var i=0;i<_taskList.Count;i++)
                {
                    var x = _taskList[i];
              
                    options = x;
                    var result =await x.AddJob(_schedulerFactory, true, jobFactory: services.GetService<IJobFactory>());
                    ControlHelper.AddMsg($"注册任务:{x.TaskName}");
                }
            }
            catch (Exception ex)
            {
                errorCount = +1;
                errorMsg += $"作业:{options?.TaskName},异常：{ex.Message}";
            }
            string content = $"成功:{_taskList.Count - errorCount}个,失败{errorCount}个,异常：{errorMsg}\r\n";
            // FileQuartz.WriteStartLog(content);
            ControlHelper.AddMsg(content);

            return applicationBuilder;
        }

        /// <summary>
        /// 获取所有的作业
        /// </summary>
        /// <param name="schedulerFactory"></param>
        /// <returns></returns>
        public static async Task<List<QuartzTask>> GetJobs(this ISchedulerFactory schedulerFactory)
        {
            List<QuartzTask> list = new List<QuartzTask>();
            try
            {
                IScheduler _scheduler = await schedulerFactory.GetScheduler();
                var groups = await _scheduler.GetJobGroupNames();
                foreach (var groupName in groups)
                {
                    foreach (var jobKey in await _scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(groupName)))
                    {
                        QuartzTask quartzTask = _taskList.Where(x => x.GroupName == jobKey.Group
                        && x.TaskName == jobKey.Name).FirstOrDefault();
                        if (quartzTask == null)
                            continue;

                        var triggers = await _scheduler.GetTriggersOfJob(jobKey);
                        foreach (ITrigger trigger in triggers)
                        {
                            DateTimeOffset? dateTimeOffset = trigger.GetPreviousFireTimeUtc();
                            if (dateTimeOffset != null)
                            {
                                quartzTask.LastRunTime = Convert.ToDateTime(dateTimeOffset.ToString());
                            } 
                        }
                        list.Add(quartzTask);
                    }
                }
            }
            catch (Exception ex)
            {
                FileQuartz.WriteStartLog("获取作业异常：" + ex.Message + ex.StackTrace);

            }
            return list;
        }


        /// <summary>
        /// 添加作业
        /// </summary>
        /// <param name="QuartzTask"></param>
        /// <param name="schedulerFactory"></param>
        /// <param name="init">是否初始化,否=需要重新生成配置文件，是=不重新生成配置文件</param>
        /// <returns></returns>
        public static async Task<object> AddJob(this QuartzTask quartzTask, ISchedulerFactory schedulerFactory, bool init = false, IJobFactory jobFactory = null)
        {
            try
            {
                if (schedulerFactory == null) return new { status = false }; ;
                (bool, string) validExpression = quartzTask.Interval.IsValidExpression();
                if (!validExpression.Item1)
                    return new { status = false, msg = validExpression.Item2 };

                (bool, object) result = quartzTask.Exists(init);
                if (!result.Item1)
                    return result.Item2;
                if (!init)
                {
                    _taskList.Add(quartzTask);
                }

                IJobDetail job = JobBuilder.Create(Type.GetType(quartzTask.JobClassName))
               .WithIdentity(quartzTask.TaskName, quartzTask.GroupName)
              .Build();
                ITrigger trigger = TriggerBuilder.Create()
                   .WithIdentity(quartzTask.TaskName, quartzTask.GroupName)
                   .StartNow()
                   .WithDescription(quartzTask.Describe)
                   .WithCronSchedule(quartzTask.Interval)
                   .Build();

                IScheduler scheduler = await schedulerFactory.GetScheduler();

                if (jobFactory == null)
                {
                    try
                    {
                        jobFactory = Utility.HttpContext.Current.RequestServices.GetService<IJobFactory>();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"创建任务[{quartzTask.TaskName}]异常,{ex.Message}");
                    }
                }

                if (jobFactory != null)
                {
                    scheduler.JobFactory = jobFactory;
                }

                await scheduler.ScheduleJob(job, trigger);
                if (quartzTask.Status == (int)TriggerState.Normal)
                {
                    await scheduler.Start();
                }
                else
                {
                    await schedulerFactory.Pause(quartzTask);
                
                   ControlHelper.AddMsg($"作业:{quartzTask.TaskName},分组:{quartzTask.GroupName},新建时未启动原因,状态为:{quartzTask.Status}");
                }
              //  if (!init)
              //      FileQuartz.WriteJobAction(JobAction.新增, quartzTask.TaskName, quartzTask.GroupName);
            }
            catch (Exception ex)
            {
                return new { status = false, msg = ex.Message };
            }
            return new { status = true };
        }

        /// <summary>
        /// 移除作业
        /// </summary>
        /// <param name="schedulerFactory"></param>
        /// <param name="taskName"></param>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public static Task<object> Remove(this ISchedulerFactory schedulerFactory, QuartzTask QuartzTask)
        {
            return schedulerFactory.TriggerAction(QuartzTask.TaskName, QuartzTask.GroupName, JobAction.删除, QuartzTask);
        }

        /// <summary>
        /// 更新作业
        /// </summary>
        /// <param name="schedulerFactory"></param>
        /// <param name="QuartzTask"></param>
        /// <returns></returns>
        public static Task<object> Update(this ISchedulerFactory schedulerFactory, QuartzTask QuartzTask)
        {
            return schedulerFactory.TriggerAction(QuartzTask.TaskName, QuartzTask.GroupName, JobAction.修改, QuartzTask);
        }

        /// <summary>
        /// 暂停作业
        /// </summary>
        /// <param name="schedulerFactory"></param>
        /// <param name="QuartzTask"></param>
        /// <returns></returns>
        public static Task<object> Pause(this ISchedulerFactory schedulerFactory, QuartzTask QuartzTask)
        {
            return schedulerFactory.TriggerAction(QuartzTask.TaskName, QuartzTask.GroupName, JobAction.暂停, QuartzTask);
        }

        /// <summary>
        /// 启动作业
        /// </summary>
        /// <param name="schedulerFactory"></param>
        /// <param name="QuartzTask"></param>
        /// <returns></returns>
        public static Task<object> Start(this ISchedulerFactory schedulerFactory, QuartzTask QuartzTask)
        {
            return schedulerFactory.TriggerAction(QuartzTask.TaskName, QuartzTask.GroupName, JobAction.开启, QuartzTask);
        }

        /// <summary>
        /// 立即执行一次作业
        /// </summary>
        /// <param name="schedulerFactory"></param>
        /// <param name="QuartzTask"></param>
        /// <returns></returns>
        public static Task<object> Run(this ISchedulerFactory schedulerFactory, QuartzTask quartzTask)
        {
            return schedulerFactory.TriggerAction(quartzTask.TaskName, quartzTask.GroupName, JobAction.立即执行, quartzTask);
        }

        public static object ModifyTaskEntity(this QuartzTask quartzTask, ISchedulerFactory schedulerFactory, JobAction action)
        {
            QuartzTask options = null;
            object result = null;
            switch (action)
            {
                case JobAction.删除:
                    for (int i = 0; i < _taskList.Count; i++)
                    {
                        options = _taskList[i];
                        if (options.TaskName == quartzTask.TaskName && options.GroupName == quartzTask.GroupName)
                        {
                            _taskList.RemoveAt(i);
                        }
                    }
                    break;
                case JobAction.修改:
                    options = _taskList.Where(x => x.TaskName == quartzTask.TaskName && x.GroupName == quartzTask.GroupName).FirstOrDefault();
                    //移除以前的配置
                    if (options != null)
                    {
                        _taskList.Remove(options);
                    }

                    //生成任务并添加新配置
                    result = quartzTask.AddJob(schedulerFactory, false).GetAwaiter().GetResult();
                    break;
                case JobAction.暂停:
                case JobAction.开启:
                case JobAction.停止:
                case JobAction.立即执行:
                    options = _taskList.Where(x => x.TaskName == quartzTask.TaskName && x.GroupName == quartzTask.GroupName).FirstOrDefault();
                    if (action == JobAction.暂停)
                    {
                        options.Status = (int)TriggerState.Paused;
                    }
                    else if (action == JobAction.停止)
                    {
                        options.Status = (int)action;
                    }
                    else
                    {
                        options.Status = (int)TriggerState.Normal;
                    }
                    break;
            }
            //生成配置文件 
            //FileQuartz.WriteJobAction(action, QuartzTask.TaskName, QuartzTask.GroupName, "操作对象：" + JsonConvert.SerializeObject(QuartzTask));
            return result;
        }

        /// <summary>
        /// 触发新增、删除、修改、暂停、启用、立即执行事件
        /// </summary>
        /// <param name="schedulerFactory"></param>
        /// <param name="taskName"></param>
        /// <param name="groupName"></param>
        /// <param name="action"></param>
        /// <param name="QuartzTask"></param>
        /// <returns></returns>
        public static async Task<object> TriggerAction(this ISchedulerFactory schedulerFactory, string taskName, string groupName, JobAction action, QuartzTask QuartzTask = null)
        {
            string errorMsg = "";
            try
            {
                IScheduler scheduler = await schedulerFactory.GetScheduler();
                List<JobKey> jobKeys = scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(groupName)).Result.ToList();
                if (jobKeys == null || jobKeys.Count() == 0)
                {
                    errorMsg = $"未找到分组[{groupName}]";
                    return new { status = false, msg = errorMsg };
                }
                JobKey jobKey = jobKeys.Where(s => scheduler.GetTriggersOfJob(s).Result.Any(x => (x as CronTriggerImpl).Name == taskName)).FirstOrDefault();
                if (jobKey == null)
                {
                    errorMsg = $"未找到触发器[{taskName}]";
                    return new { status = false, msg = errorMsg };
                }
                var triggers = await scheduler.GetTriggersOfJob(jobKey);
                ITrigger trigger = triggers?.Where(x => (x as CronTriggerImpl).Name == taskName).FirstOrDefault();

                if (trigger == null)
                {
                    errorMsg = $"未找到触发器[{taskName}]";
                    return new { status = false, msg = errorMsg };
                }
                object result = null;
                switch (action)
                {
                    case JobAction.删除:
                    case JobAction.修改:
                        await scheduler.PauseTrigger(trigger.Key);
                        await scheduler.UnscheduleJob(trigger.Key);// 移除触发器
                        await scheduler.DeleteJob(trigger.JobKey);
                        result = QuartzTask.ModifyTaskEntity(schedulerFactory, action);
                        break;
                    case JobAction.暂停:
                    case JobAction.停止:
                    case JobAction.开启:
                        result = QuartzTask.ModifyTaskEntity(schedulerFactory, action);
                        if (action == JobAction.暂停)
                        {
                            await scheduler.PauseTrigger(trigger.Key);
                        }
                        else if (action == JobAction.开启)
                        {
                            await scheduler.ResumeTrigger(trigger.Key);
                            //   await scheduler.RescheduleJob(trigger.Key, trigger);
                        }
                        else
                        {
                            await scheduler.Shutdown();
                        }
                        break;
                    case JobAction.立即执行:
                        await scheduler.TriggerJob(jobKey);
                        break;
                }
                return result ?? new { status = true, msg = $"作业{action.ToString()}成功" };
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                return new { status = false, msg = ex.Message };
            }
            finally
            {
                FileQuartz.WriteJobAction(action, taskName, groupName, errorMsg);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>通过作业上下文获取作业对应的配置参数
        /// <returns></returns>
        public static QuartzTask GetQuartzTask(this IJobExecutionContext context)
        {
            AbstractTrigger trigger = (context as JobExecutionContextImpl).Trigger as AbstractTrigger;
            QuartzTask QuartzTask = _taskList.Where(x => x.TaskName == trigger.Name && x.GroupName == trigger.Group).FirstOrDefault();
            return QuartzTask ?? _taskList.Where(x => x.TaskName == trigger.JobName && x.GroupName == trigger.JobGroup).FirstOrDefault();
        }

        /// <summary>
        /// 作业是否存在
        /// </summary>
        /// <param name="QuartzTask"></param>
        /// <param name="init">初始化的不需要判断</param>
        /// <returns></returns>
        public static (bool, object) Exists(this QuartzTask QuartzTask, bool init)
        {
            if (!init && _taskList.Any(x => x.TaskName == QuartzTask.TaskName && x.GroupName == QuartzTask.GroupName))
            {
                return (false,
                    new
                    {
                        status = false,
                        msg = $"作业:{QuartzTask.TaskName},分组：{QuartzTask.GroupName}已经存在"
                    });
            }
            return (true, null);
        }

        public static (bool, string) IsValidExpression(this string cronExpression)
        {
            try
            {
                CronTriggerImpl trigger = new CronTriggerImpl();
                trigger.CronExpressionString = cronExpression;
                DateTimeOffset? date = trigger.ComputeFirstFireTimeUtc(null);
                return (date != null, date == null ? $"请确认表达式{cronExpression}是否正确!" : "");
            }
            catch (Exception e)
            {
                return (false, $"请确认表达式{cronExpression}是否正确!{e.Message}");
            }
        }
    }

}
