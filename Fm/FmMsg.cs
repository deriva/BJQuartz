using BJ.Quartz.Helper;
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
    public partial class FmMsg : Form
    {
        public FmMsg()
        {
            InitializeComponent();
        }

        private void FmMsg_Load(object sender, EventArgs e)
        {
            ControlHelper.InitListBox(this.lstMsgBox);
        }
    }
}
