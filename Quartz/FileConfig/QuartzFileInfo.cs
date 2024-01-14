using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BJ.Quartz.Quartz
{
    public class QuartzFileInfo
    {
        /// <summary>
        /// 所有任务相关存放的文件夹默认生成在当前项目类库同级(子文件夹包括：日志,作业配置)
        /// </summary>
        public static string QuartzSettingsFolder = "QuartzSettings";
        //所有作业配置存储文件
        public static string JobConfigFileName = "job_options.json";

        /// <summary>
        /// 日志文件夹名称 
        /// </summary>
        public static string Logs = "logs";
    }

    // Grab the Scheduler instance from the Factory
    //StdSchedulerFactory factory = new StdSchedulerFactory();
    //IScheduler scheduler = await factory.GetScheduler();

    //// and start it off
    //await scheduler.Start();

    //// some sleep to show what's happening
    //await Task.Delay(TimeSpan.FromSeconds(10));

    //        // and last shut down the scheduler when you are ready to close your program
    //        await scheduler.Shutdown(); 
}
