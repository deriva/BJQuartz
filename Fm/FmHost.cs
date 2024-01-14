using Autofac.Extensions.DependencyInjection;
using BJ.Quartz.Helper;
using BJ.Quartz.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BJ.Quartz.Fm
{
    public partial class FmHost : Form
    {
        WebApplicationBuilder builder;
        WebApplication app;
        private readonly string url = "http://0.0.0.0:33022";
        public FmHost()
        {
            InitializeComponent();
        }
        private void btnStop_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(async () =>
            {
                await Stop();
            });
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(async () =>
            {
                await Start();
                //btnStart.Enabled = false;
                //btnStop.Enabled = true;
            });
        }
        private async Task Start()
        {

            ControlHelper.AddLabMsg(this.lab1, $"当前状态已启动:{url}");

            new QuartzTaskService().Init();
            //try
            //{
            //    var dd = LocalSqliteManage.PG;
            //    dd.DbMaintenance.BackupDataBase("SRM_Master", $"D:\\backupdb\\SRM_Master_{DateTime.Now.ToString("yyyy-MM-dd")}.backup");
            //}
            //catch(Exception ex)
            //{
            //    ControlHelper.AddMsg($"{ex.ToString()}");
            //}
            ControlHelper.AddMsg($"初始化表成功");

            var builder = WebApplication.CreateBuilder();

            builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());


            // Add services to the container.
            //  builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
            ///获取配置
            var configuration = new ConfigurationBuilder().SetBasePath(Environment.CurrentDirectory)
                                                      .AddJsonFile("appsettings.json")
                                                      .Build();
            // var url = configuration.GetSection("StartUrl").Value;
            var startup = new Startup(builder.Configuration);
            startup.ConfigureServices(builder.Services);
            app = builder.Build();

            startup.Configure(app, app.Environment);
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");


            await app.RunAsync(url);

        }

        private async Task Stop()
        {
            if (app != null)
            {
                await app.StopAsync();
                ControlHelper.AddLabMsg(this.lab1, $"当前状态已停止:{url}");
                btnStart.Enabled = true;
                btnStop.Enabled = false;
            }
        }

        private void FmHost_Load(object sender, EventArgs e)
        {
            Task.Factory.StartNew(async () =>
            {
                await Start();

            });
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            //  System.Diagnostics.Process.Start("http://127.0.0.1:15012/content/page/Quartz/Index.html");
        }

        private void lab1_Click(object sender, EventArgs e)
        {
           // System.Diagnostics.Process.Start("http://127.0.0.1:15012/content/page/Quartz/Index.html");

        }
    }
}
