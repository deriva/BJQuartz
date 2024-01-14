using BJ.Quartz.Helper;
using BJ.Quartz.Quartz;
using BJ.Quartz.Utilities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using System.Diagnostics;
using System.Runtime.InteropServices;
namespace BJ.Quartz
{



    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
          //  GlobalContext.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
            });
           // services.AddSignalR();
             services.AddHttpClient();
            services.AddHttpContextAccessor();
            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(TaskAuthorizeFilter));
            });
            services.AddSession().AddMemoryCache();
            services.AddSingleton<IPathProvider, PathProvider>();

            //var assemblyName = " BJ.Manage.Bussiness";
            //services.AddScoped(Assembly.Load(assemblyName), Assembly.Load(assemblyName));

            //sqlsugar注入
          //  services.AddRepositories();
            ConfigureJobServices(services);
           // GlobalContext.Services = services;
            //RedisServer.Initalize();

            #region 配置Json格式
            services.AddMvc().AddNewtonsoftJson(options =>
            {
                // 忽略循环引用
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                // 不使用驼峰
                options.SerializerSettings.ContractResolver = new CustomContractResolver();
                // 设置时间格式
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                // 如字段为null值，该字段不会返回到前端
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            }).AddJsonOptions(option =>
            {

                //返回json小写
          //   option.JsonSerializerOptions.PropertyNamingPolicy = new LowercasePolicy();


            });
            #endregion

            //注入全局异常过滤
            services.AddControllers(options =>
            {
                //全局异常过滤
              //  options.Filters.Add<GlobalExceptions>();
                //全局日志
              //  options.Filters.Add<GlobalActionMonitor>();

            });
        }

        public async void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
         
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }


            // GlobalContext.ServiceProvider = app.ApplicationServices;
            app.UseHttpsRedirection();

            DefaultFilesOptions defaultFilesOptions = new DefaultFilesOptions();
            defaultFilesOptions.DefaultFileNames.Clear();
            defaultFilesOptions.DefaultFileNames.Add("index.html");
            app.UseDefaultFiles(defaultFilesOptions);

            app.UseStaticFiles();
            app.UseRouting();
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            (await app.UseQuartz(env)).UseStaticHttpContext();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
               // endpoints.MapHub<SignalRHub>("/hub");



            });


        }

        /// <summary>
        /// Job定时器实例
        /// </summary>
        /// <param name="services"></param>
        private void ConfigureJobServices(IServiceCollection services)
        {

            var aType = typeof(IJob);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                         .SelectMany(a => a.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(IJob))))
                         .ToArray();
            foreach (var v in types)
            { //实例化定时器
              //services.AddTransient<HttpResultfulJob>();
              //services.AddTransient<TestJob>();
                services.AddTransient(v);
            }
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
            services.AddSingleton<IJobFactory, IOCJobFactory>();
        }

        public bool CheckIsIIS()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return false;
            }

            string processName = Path.GetFileNameWithoutExtension(Process.GetCurrentProcess().ProcessName);
            return (processName.Contains("w3wp", StringComparison.OrdinalIgnoreCase) ||
                processName.Contains("iisexpress", StringComparison.OrdinalIgnoreCase));
        }
    }
}