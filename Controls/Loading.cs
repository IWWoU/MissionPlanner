using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IronPython.Runtime;
using MissionPlanner.Utilities;

namespace MissionPlanner.Controls
{
    public partial class Loading : Form
    {
        static Loading Instance;

        static object locker = new object();

        public Loading()
        {
            InitializeComponent();
        }

        public new string Text 
        {
            get { return label1.Text; }
            set
            {
                label1.Text = value;

                if (this.IsHandleCreated && !IsDisposed)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        Application.DoEvents();
                    });
                }
            }
        }

        public new static void Close()
        {
            if (Instance != null)
            {
                if (!Instance.IsDisposed)
                {
                    if (Instance.IsHandleCreated)
                    {
                        Instance.Invoke((MethodInvoker)delegate
                        {
                            ((Form) Instance).Close();
                        });
                        Instance = null;
                    }
                }
            }
        }

        /// <summary>
        /// Create a new dialog or use an existing one if its still valid
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static Loading ShowLoading(string Text, IWin32Window owner = null)
        {
            // ensure we only have one instance at a time
            lock (locker)
            {
                if (Instance != null && !Instance.IsDisposed)
                {
                    Instance.Text = Text;
                    return Instance;
                }

                Loading frm = new Loading();
                frm.Text = Text;
                frm.TopMost = true;
                frm.StartPosition = FormStartPosition.CenterParent;

                ThemeManager.ApplyThemeTo(frm);

                MainV2.instance.Invoke((MethodInvoker) delegate
                {
                    frm.Show(owner);
                    Application.DoEvents();
                });

                Instance = frm;

                return frm;
            }
        }
    }
}
