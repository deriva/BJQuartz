using BJ.Quartz.Helper;
using BJ.Quartz.Model;
using BJ.Quartz.Service;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Triggers;
namespace BJ.Quartz.Quartz
{

    [DisallowConcurrentExecution]
    public class HttpResultfulJob : IJob
    {
        readonly IHttpClientFactory httpClientFactory;
        /// <summary>
        /// 2020.05.31增加构造方法
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="httpClientFactory"></param>
        public HttpResultfulJob(IServiceProvider serviceProvider, IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
            //serviceProvider.GetService()
        }
        public async Task Execute(IJobExecutionContext context)
        {
           
            DateTime dateTime = DateTime.Now;
            QuartzTask quartzTask = context.GetQuartzTask();
            string httpMessage = string.Empty;
            AbstractTrigger trigger = (context as JobExecutionContextImpl).Trigger as AbstractTrigger;
            if (quartzTask == null)
            {
                ControlHelper.AddMsg("作业不存在");

                // FileHelper.WriteFile(FileQuartz.LogPath + trigger.Group, $"{trigger.Name}.txt", "未到找作业或可能被移除", true);
                return;
            }
            ControlHelper.AddMsg($"作业[{quartzTask.TaskName}]开始:{DateTime.Now.ToString("yyyy-MM-dd HH:mm:sss")}");
            if (string.IsNullOrEmpty(quartzTask.ApiUrl) || quartzTask.ApiUrl == "/")
            {
                ControlHelper.AddMsg("quartzTask.ApiUrl作业不存在");
                //   FileHelper.WriteFile(FileQuartz.LogPath + trigger.Group, $"{trigger.Name}.txt", $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:sss")}未配置url,", true);
                return;
            }
            var result = 0;
            try
            {
                Dictionary<string, string> header = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(quartzTask.AuthKey)
                    && !string.IsNullOrEmpty(quartzTask.AuthValue))
                {
                    header.Add(quartzTask.AuthKey.Trim(), quartzTask.AuthValue.Trim());
                }
                HttpMethod httpMethod = HttpMethod.Get;
                if (quartzTask.RequestType?.ToLower() == "get")
                    httpMethod = HttpMethod.Get;
                else if (quartzTask.RequestType?.ToLower() == "post")
                    httpMethod = HttpMethod.Post;
                else { return; }
                httpMessage = await httpClientFactory.HttpSendAsync(httpMethod, quartzTask.ApiUrl, header);
            }
            catch (Exception ex)
            {
                httpMessage = ex.Message;
                result = -1;
            }
            if (httpMessage.Length > 30)
                httpMessage = httpMessage.Substring(0,30);
            ControlHelper.AddMsg(quartzTask.TaskName + "执行:OK " + httpMessage.Substring(30));
          
            await JobLogHelper.AddJobLog(quartzTask, httpMessage, result);
            return;
        }
    }
}
