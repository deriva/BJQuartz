using BJ.Quartz.Helper;
using BJ.Quartz.Model;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Triggers;

namespace BJ.Quartz.Quartz
{
    [DisallowConcurrentExecution]
    //[TaskCustom("测试Job", "测试", "0/10 * * * * ?")] //, "0/10 **** ?"
    public class TestJob : IJob
    {
        readonly IHttpClientFactory httpClientFactory;
        /// <summary>
        /// 2020.05.31增加构造方法
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="httpClientFactory"></param>
        public TestJob(IServiceProvider serviceProvider, IHttpClientFactory httpClientFactory)
        {

            this.httpClientFactory = httpClientFactory;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            //定时器：todo
            DateTime dateTime = DateTime.Now;
            QuartzTask quartzTask = context.GetQuartzTask();
            string httpMessage = "";
            AbstractTrigger trigger = (context as JobExecutionContextImpl).Trigger as AbstractTrigger;
            if (quartzTask == null)
            {
                FileHelper.WriteFile(FileQuartz.LogPath + trigger.Group, $"{trigger.Name}.txt", "未到找作业或可能被移除", true);
                return;
            }

            var result = "OK";

            ControlHelper.AddMsg(quartzTask.TaskName + "执行:OK " + httpMessage);
         // ControlHelper.AddMsg()
            return;
        }
    }
}
