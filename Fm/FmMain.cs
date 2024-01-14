using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BJ.Quartz.Fm
{
    public partial class FmMain : FmBase
    {
        public FmMain()
        {
            InitializeComponent();
        }

        private void http服务ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddTabFmShow(GetForm("FmHost"), "Http服务", true);

        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void FmMain_Load(object sender, EventArgs e)
        {
            tabControl1.TabPages.Clear();
      
            AddTabFmShow(GetForm("FmHost"), "Http服务", true);
            AddTabFmShow(GetForm("FmMsg"), "消息中心");
        }

        private void AddTabFmShow(Form frm, string title, bool isSelect = false)
        {
           
            if (tabControl1.TabPages.Count > 20)
            {
                MessageBox.Show("不允许超过20个标签,请先关闭其他标签");
                return;
            }
            var index = tabControl1.TabPages.IndexOfKey(title);
            if (index == -1)
            {
                frm.TopLevel = false;
                frm.Dock = DockStyle.Fill;
                TabPage myTabPage = new TabPage(title);
                frm.Parent = myTabPage;
                frm.Show();
                frm.MaximizeBox = true;
                myTabPage.Dock = DockStyle.Fill;
                myTabPage.Controls.Add(frm);
                tabControl1.TabPages.Add(myTabPage);
                if (isSelect) tabControl1.SelectedTab = myTabPage;
            }
            else
            {
                if (isSelect) tabControl1.SelectedIndex = index;
            }
        }
    }
}
