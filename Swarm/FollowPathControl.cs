using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ProjNet.CoordinateSystems.Transformations;
using ProjNet.CoordinateSystems;
using ProjNet.Converters;

namespace MissionPlanner.Swarm
{
    public partial class FollowPathControl : Form
    {
        FollowPath SwarmInterface = null;
        bool threadrun = false;

        public FollowPathControl()
        {
            InitializeComponent();

            SwarmInterface = new FollowPath();

            bindingSource1.DataSource = MainV2.Comports;

            CMB_mavs.DataSource = bindingSource1;

            MessageBox.Show("this is beta, use at own risk");

            MissionPlanner.Utilities.Tracking.AddPage(this.GetType().ToString(), this.Text);
        }

        private void CMB_mavs_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (var port in MainV2.Comports)
            {
                if (port.ToString() == CMB_mavs.Text)
                {
                    MainV2.comPort = port;
                }
            }
        }

        private void BUT_Start_Click(object sender, EventArgs e)
        {
            if (threadrun == true)
            {
                threadrun = false;
                BUT_Start.Text = Strings.Start;
                return;
            }

            if (SwarmInterface != null)
            {
                new System.Threading.Thread(mainloop) {IsBackground = true}.Start();
                BUT_Start.Text = Strings.Stop;
            }
        }

        void mainloop()
        {
            threadrun = true;

            while (threadrun)
            {
                // update leader pos
                SwarmInterface.Update();

                // update other mavs
                SwarmInterface.SendCommand();

                // 5 hz
                System.Threading.Thread.Sleep(200);
            }
        }

        private void BUT_Arm_Click(object sender, EventArgs e)
        {
            if (SwarmInterface != null)
            {
                SwarmInterface.Arm();
            }
        }

        private void BUT_Disarm_Click(object sender, EventArgs e)
        {
            if (SwarmInterface != null)
            {
                SwarmInterface.Disarm();
            }
        }

        private void BUT_Takeoff_Click(object sender, EventArgs e)
        {
            if (SwarmInterface != null)
            {
                SwarmInterface.Takeoff();
            }
        }

        private void BUT_Land_Click(object sender, EventArgs e)
        {
            if (SwarmInterface != null)
            {
                SwarmInterface.Land();
            }
        }

        private void BUT_leader_Click(object sender, EventArgs e)
        {
            if (SwarmInterface != null)
            {
                SwarmInterface.setLeader(MainV2.comPort);
                BUT_Start.Enabled = true;
            }
        }

        private void BUT_connect_Click(object sender, EventArgs e)
        {
            Comms.CommsSerialScan.Scan(false);

            DateTime deadline = DateTime.Now.AddSeconds(50);

            while (Comms.CommsSerialScan.foundport == false)
            {
                System.Threading.Thread.Sleep(100);

                if (DateTime.Now > deadline)
                {
                    CustomMessageBox.Show("Timeout waiting for autoscan/no mavlink device connected");
                    return;
                }
            }

            MAVLinkInterface com2 = new MAVLinkInterface();

            com2.BaseStream.PortName = Comms.CommsSerialScan.portinterface.PortName;
            com2.BaseStream.BaudRate = Comms.CommsSerialScan.portinterface.BaudRate;

            com2.Open(true);

            MainV2.Comports.Add(com2);

            bindingSource1.ResetBindings(false);
        }

        private void Control_FormClosing(object sender, FormClosingEventArgs e)
        {
            threadrun = false;
        }

        private void timer_status_Tick(object sender, EventArgs e)
        {
            // clean up old
            foreach (Control ctl in PNL_status.Controls)
            {
                if (!MainV2.Comports.Contains((MAVLinkInterface) ctl.Tag))
                {
                    ctl.Dispose();
                }
            }

            // setup new
            foreach (var port in MainV2.Comports)
            {
                bool exists = false;
                foreach (Control ctl in PNL_status.Controls)
                {
                    if (ctl is Status && ctl.Tag == port)
                    {
                        exists = true;
                        ((Status) ctl).GPS.Text = port.MAV.cs.gpsstatus >= 3 ? "OK" : "Bad";
                        ((Status) ctl).Armed.Text = port.MAV.cs.armed.ToString();
                        ((Status) ctl).Mode.Text = port.MAV.cs.mode;
                        ((Status) ctl).MAV.Text = port.ToString();
                        ((Status) ctl).Guided.Text = port.MAV.GuidedMode.x + "," + port.MAV.GuidedMode.y + "," +
                                                     port.MAV.GuidedMode.z;
                    }
                }

                if (!exists)
                {
                    Status newstatus = new Status();
                    newstatus.Tag = port;
                    PNL_status.Controls.Add(newstatus);
                }
            }
        }
    }
}