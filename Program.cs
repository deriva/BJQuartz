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
        /// Ӧ�ó��������ڵ㡣
        /// </summary>
        [STAThread]
        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]
        private static void Main()
        {
            var curProp = Process.GetCurrentProcess();
            curProp.PriorityClass = ProcessPriorityClass.AboveNormal;
            #region ���л����߳�������
            var ioType = Environment.ProcessorCount * 2 - 1;//IOд����
            ThreadPool.SetMinThreads(ioType, ioType); 
            #endregion
            try
            {
                var process = RuningInstance();
                if (process is null)
                {
                    BindExceptionHandler();//�󶨳����е��쳣����
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


 
 
        #region ϵͳ�����쳣�¼�
        /// <summary>
        /// �󶨳����е��쳣����
        /// </summary>
        private static void BindExceptionHandler()
        {
            //����Ӧ�ó������쳣��ʽ��ThreadException����
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            //����UI�߳��쳣
            Application.ThreadException += Application_ThreadException;
            //����δ������쳣
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }
        /// <summary>
        /// ����UI�߳��쳣
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
        /// ����δ������쳣
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
        /// �����Զ����쳣��Ϣ
        /// </summary>
        /// <param name="ex">�쳣����</param>
        /// <param name="backStr">�����쳣��Ϣ����exΪnullʱ��Ч</param>
        /// <returns>�쳣�ַ����ı�</returns>
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
        #region ΨһEXE����
        /// <summary>
        /// �ú��������ɲ�ͬ�̲߳����Ĵ��ڵ���ʾ״̬
        /// </summary>
        /// <param name="hWnd">���ھ��</param>
        /// <param name="cmdShow">ָ�����������ʾ���鿴����ֵ�б������ShowWlndow������˵������</param>
        /// <returns>�������ԭ���ɼ�������ֵΪ���㣻�������ԭ�������أ�����ֵΪ��</returns>
        [DllImport("User32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int cmdShow);
        /// <summary>
        /// �ú���������ָ�����ڵ��߳����õ�ǰ̨�����Ҽ���ô��ڡ���������ת��ô��ڣ���Ϊ�û��ĸ��ֿ��ӵļǺš�
        /// ϵͳ������ǰ̨���ڵ��̷߳����Ȩ���Ը��������̡߳�
        /// </summary>
        /// <param name="hWnd">�������������ǰ̨�Ĵ��ھ��</param>
        /// <returns>�������������ǰ̨������ֵΪ���㣻�������δ������ǰ̨������ֵΪ��</returns>
        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        ///// <summary>
        ///// ��ִ̨��
        ///// </summary>
        //private const int SW_SHOWBACKGROUP = 0;
        ///// <summary>
        ///// ��������
        ///// </summary>
        //private const int SW_SHOWNOMAL = 1;
        ///// <summary>
        ///// ��С����������
        ///// </summary>
        //private const int SW_SHOWMINISIZE = 2;
        /// <summary>
        /// ���
        /// </summary>
        private const int SW_SHOWMAXSIZE = 3;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        private static void HandleRunningInstance(Process instance)
        {
            ShowWindowAsync(instance.MainWindowHandle, SW_SHOWMAXSIZE);//��ʾ
            SetForegroundWindow(instance.MainWindowHandle);//������ǰ��
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