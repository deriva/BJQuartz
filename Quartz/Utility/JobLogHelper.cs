using BJ.Quartz.Model;
using BJ.Quartz.Service;
using Quartz.Util;

namespace BJ.Quartz.Quartz
{
    public class JobLogHelper
    {
       // private static JobLogManager _jobLogManager() => IocManager.Instance.GetService<JobLogManager>();

        /// <summary>
        /// 添加日志
        /// </summary>
        /// <param name="quartzTask"></param>
        /// <param name="content"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static async Task AddJobLog(QuartzTask quartzTask, string content, int result = 0)
        {
            await new QuartzTaskService().UpdateLastTime(quartzTask.Id);
            if (content.IsNullOrWhiteSpace()) return;
            if (content.Length > 512) content = content.Substring(0, 512);
            await new JobLogService().AddJobLog(quartzTask.Id, quartzTask.TaskName, content, result );
        }

        /// <summary>
        /// 添加日志
        /// </summary>
        /// <param name="quartzTask"></param>
        /// <param name="content"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static async Task AddJobLog(long jobid, string taskname, string content, int result = 0)
        {
            if (content.IsNullOrWhiteSpace()) return;
            if (content.Length > 512) content = content.Substring(0, 512);
            await new JobLogService().AddJobLog(jobid, taskname, content, result);

        }
    }
}
