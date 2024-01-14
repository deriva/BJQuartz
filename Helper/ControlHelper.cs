namespace BJ.Quartz.Helper
{
    public class ControlHelper
    {
        static ToolStripStatusLabel _labControl;
        static ListBox plstbox;
        static Label _lab;
        public static void InitListBox(ListBox listbox) { plstbox = listbox; }
        public static void InitListBox(Label lab) { _lab = lab; }
        public static void InitToolStripStatusLabel(ToolStripStatusLabel labControl) => _labControl = labControl;

        public static void AddMsg(string msg)
        {

            if (plstbox != null)
            {
                if (plstbox.InvokeRequired)
                {
                    Action<string> myAction = (p) => { AddMsg(p); };
                    plstbox.Invoke(myAction, msg);
                }
                else
                {
                    if (plstbox.Items.Count > 100)
                    {
                        plstbox.Items.Clear();
                    }
                    plstbox.Items.Insert(0, string.Format("{0}->{1}", DateTime.Now.ToString("MM-dd HH:mm:ss"), msg));
                }
            }
        }
        public static void AddToolStripStatusLabel(string msg)
        {
            if (_labControl != null) _labControl.Text = msg;
        }

        public static void AddLabMsg(Label lab, string msg)
        {
            if (lab != null)
            {
                if (lab.InvokeRequired)
                {
                    Action<string> myAction = (p) => { lab.Text = msg; };
                    lab.Invoke(myAction, msg);
                }
                else
                {
                    lab.Text = msg;
                } 
            }

        }
    }
}
