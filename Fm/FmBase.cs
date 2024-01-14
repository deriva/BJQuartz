using System.Reflection;

namespace BJ.Quartz.Fm
{
    public class FmBase : Form
    {
        /// <summary>
        /// 获取窗体 如果不存在则创建
        /// </summary>
        /// <param name="fmName"></param>
        /// <param name="fm2"></param>
        /// <returns></returns>
        public Form GetForm(string fmName)
        {
            FormCollection formCollection = Application.OpenForms;//获取存在的窗体集合
            foreach (Form fm in formCollection)//循环遍历，判断
            {
                if (fm.Name == fmName)//判断是否存在该窗体
                {
                    return fm;
                }
            } 
            return CreateForm(fmName);
        }

        public Form CreateForm(string fmName)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Form form = assembly.CreateInstance("BJ.Quartz.Fm." + fmName) as Form;   //命名空间+窗体名称
            return form;

        }
    }
}
