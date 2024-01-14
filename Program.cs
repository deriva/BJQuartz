using BJ.Quartz.Fm;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;

namespace BJ.Quartz
{
    internal static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]
        private static void Main()
        {
            var curProp = Process.GetCurrentProcess();
            curProp.PriorityClass = ProcessPriorityClass.AboveNormal;
            #region 运行基础线程数设置
            var ioType = Environment.ProcessorCount * 2 - 1;//IO写入型
            ThreadPool.SetMinThreads(ioType, ioType); 
            #endregion
            try
            {
                var process = RuningInstance();
                if (process is null)
                {
                    BindExceptionHandler();//绑定程序中的异常处理
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new FmMain());
                }
                else
                {
                    HandleRunningInstance(process);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(GetExceptionMsg(ex, string.Empty),
                                @"Handle Process Corrupted State Exceptions",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }

        }


 
 
        #region 系统运行异常事件
        /// <summary>
        /// 绑定程序中的异常处理
        /// </summary>
        private static void BindExceptionHandler()
        {
            //设置应用程序处理异常方式：ThreadException处理
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            //处理UI线程异常
            Application.ThreadException += Application_ThreadException;
            //处理未捕获的异常
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }
        /// <summary>
        /// 处理UI线程异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            MessageBox.Show(GetExceptionMsg(e.Exception, e.ToString()),
                            "Unhandled Thread Exception",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
        }
        /// <summary>
        /// 处理未捕获的异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show(GetExceptionMsg(e.ExceptionObject as Exception, e.ToString()),
                            "Unhandled UI Exception",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
        }
        /// <summary>
        /// 生成自定义异常消息
        /// </summary>
        /// <param name="ex">异常对象</param>
        /// <param name="backStr">备用异常消息：当ex为null时有效</param>
        /// <returns>异常字符串文本</returns>
        private static string GetExceptionMsg(Exception ex, string backStr)
        {
            var sb = new StringBuilder();
            sb.AppendLine("************************EXCEPTION INFO*************************");
            sb.AppendLine($"[TIME]:{DateTime.Now}{Environment.NewLine}");
            if (ex != null)
            {
                sb.AppendLine($"[TYPE]:{ex.GetType().Name}{Environment.NewLine}");
                sb.AppendLine($"[INFO]:{ex.Message}{Environment.NewLine}");
                sb.AppendLine($"[STACK]:{ex.StackTrace}{Environment.NewLine}");
            }
            else
            {
                sb.AppendLine($"[UNOPT]:{backStr}{Environment.NewLine}");
            }
            sb.AppendLine("************************EXCEPTION INFO*************************");
            return sb.ToString();
        }
        #endregion
        #region 唯一EXE运行
        /// <summary>
        /// 该函数设置由不同线程产生的窗口的显示状态
        /// </summary>
        /// <param name="hWnd">窗口句柄</param>
        /// <param name="cmdShow">指定窗口如何显示。查看允许值列表，请查阅ShowWlndow函数的说明部分</param>
        /// <returns>如果函数原来可见，返回值为非零；如果函数原来被隐藏，返回值为零</returns>
        [DllImport("User32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int cmdShow);
        /// <summary>
        /// 该函数将创建指定窗口的线程设置到前台，并且激活该窗口。键盘输入转向该窗口，并为用户改各种可视的记号。
        /// 系统给创建前台窗口的线程分配的权限稍高于其他线程。
        /// </summary>
        /// <param name="hWnd">将被激活并被调入前台的窗口句柄</param>
        /// <returns>如果窗口设入了前台，返回值为非零；如果窗口未被设入前台，返回值为零</returns>
        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        ///// <summary>
        ///// 后台执行
        ///// </summary>
        //private const int SW_SHOWBACKGROUP = 0;
        ///// <summary>
        ///// 正常启动
        ///// </summary>
        //private const int SW_SHOWNOMAL = 1;
        ///// <summary>
        ///// 最小化到任务栏
        ///// </summary>
        //private const int SW_SHOWMINISIZE = 2;
        /// <summary>
        /// 最大化
        /// </summary>
        private const int SW_SHOWMAXSIZE = 3;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        private static void HandleRunningInstance(Process instance)
        {
            ShowWindowAsync(instance.MainWindowHandle, SW_SHOWMAXSIZE);//显示
            SetForegroundWindow(instance.MainWindowHandle);//当到最前端
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static Process RuningInstance()
        {
            var curP = Process.GetCurrentProcess();
            var p = Process.GetProcessesByName(curP.ProcessName);
            return p.Where(pro => pro.Id != curP.Id)
                    .FirstOrDefault(c => Assembly.GetExecutingAssembly().Location.Replace("/", "\\") == curP.MainModule?.FileName);
        }
        #endregion
    }
}