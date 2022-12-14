using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using MissionPlanner.Controls;
using MissionPlanner.Properties;
using MissionPlanner.Utilities;

namespace MissionPlanner.GCSViews
{
    public partial class Help : MyUserControl, IActivate
    {
        public Help()
        {
            InitializeComponent();
        }

        public void Activate()
        {
            try
            {
                CHK_showconsole.Checked = Settings.Instance.GetBoolean("showconsole");
            }
            catch
            {
            }
        }

        public void BUT_updatecheck_Click(object sender, EventArgs e)
        {
            try
            {
                Utilities.Update.CheckForUpdate(true);
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show(ex.ToString(), Strings.ERROR);
            }
        }

        private void CHK_showconsole_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Instance["showconsole"] = CHK_showconsole.Checked.ToString();
        }

        private void Help_Load(object sender, EventArgs e)
        {
            richTextBox1.Rtf = Resources.help_text;
            ThemeManager.ApplyThemeTo(richTextBox1);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://firmware.ardupilot.org/Tools/MissionPlanner/upgrade/ChangeLog.txt");
        }

        private void PIC_wizard_Click(object sender, EventArgs e)
        {
            var cfg = new Wizard.Wizard();

            cfg.ShowDialog(this);
        }

        private void BUT_betaupdate_Click(object sender, EventArgs e)
        {
            try
            {
                Utilities.Update.dobeta = true;
                Utilities.Update.DoUpdate();
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show(ex.ToString(), Strings.ERROR);
            }
        }
    }
}