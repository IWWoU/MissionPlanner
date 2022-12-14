namespace MissionPlanner.GCSViews.ConfigurationView
{
    partial class ConfigHWESP8266
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigHWESP8266));
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.pictureBox5 = new System.Windows.Forms.PictureBox();
            this.BUT_espsettings = new MissionPlanner.Controls.MyButton();
            this.cmb_baud = new System.Windows.Forms.ComboBox();
            this.txt_ssid = new System.Windows.Forms.TextBox();
            this.txt_password = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cmb_channel = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).BeginInit();
            this.SuspendLayout();
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // groupBox5
            // 
            resources.ApplyResources(this.groupBox5, "groupBox5");
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.TabStop = false;
            // 
            // pictureBox5
            // 
            this.pictureBox5.BackColor = System.Drawing.Color.White;
            this.pictureBox5.BackgroundImage = global::MissionPlanner.Properties.Resources.MinimOSD;
            resources.ApplyResources(this.pictureBox5, "pictureBox5");
            this.pictureBox5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox5.Image = global::MissionPlanner.Properties.Resources.BT_hc06;
            this.pictureBox5.Name = "pictureBox5";
            this.pictureBox5.TabStop = false;
            // 
            // BUT_espsettings
            // 
            resources.ApplyResources(this.BUT_espsettings, "BUT_espsettings");
            this.BUT_espsettings.Name = "BUT_espsettings";
            this.BUT_espsettings.UseVisualStyleBackColor = true;
            this.BUT_espsettings.Click += new System.EventHandler(this.BUT_ESPsettings_Click);
            // 
            // cmb_baud
            // 
            this.cmb_baud.FormattingEnabled = true;
            this.cmb_baud.Items.AddRange(new object[] {
            resources.GetString("cmb_baud.Items"),
            resources.GetString("cmb_baud.Items1"),
            resources.GetString("cmb_baud.Items2"),
            resources.GetString("cmb_baud.Items3"),
            resources.GetString("cmb_baud.Items4"),
            resources.GetString("cmb_baud.Items5"),
            resources.GetString("cmb_baud.Items6"),
            resources.GetString("cmb_baud.Items7"),
            resources.GetString("cmb_baud.Items8")});
            resources.ApplyResources(this.cmb_baud, "cmb_baud");
            this.cmb_baud.Name = "cmb_baud";
            // 
            // txt_ssid
            // 
            resources.ApplyResources(this.txt_ssid, "txt_ssid");
            this.txt_ssid.Name = "txt_ssid";
            // 
            // txt_password
            // 
            resources.ApplyResources(this.txt_password, "txt_password");
            this.txt_password.Name = "txt_password";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // cmb_channel
            // 
            this.cmb_channel.FormattingEnabled = true;
            this.cmb_channel.Items.AddRange(new object[] {
            resources.GetString("cmb_channel.Items"),
            resources.GetString("cmb_channel.Items1"),
            resources.GetString("cmb_channel.Items2"),
            resources.GetString("cmb_channel.Items3"),
            resources.GetString("cmb_channel.Items4"),
            resources.GetString("cmb_channel.Items5"),
            resources.GetString("cmb_channel.Items6"),
            resources.GetString("cmb_channel.Items7"),
            resources.GetString("cmb_channel.Items8"),
            resources.GetString("cmb_channel.Items9"),
            resources.GetString("cmb_channel.Items10")});
            resources.ApplyResources(this.cmb_channel, "cmb_channel");
            this.cmb_channel.Name = "cmb_channel";
            // 
            // ConfigHWESP8266
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cmb_channel);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txt_password);
            this.Controls.Add(this.txt_ssid);
            this.Controls.Add(this.cmb_baud);
            this.Controls.Add(this.BUT_espsettings);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.pictureBox5);
            this.Name = "ConfigHWESP8266";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.PictureBox pictureBox5;
        private Controls.MyButton BUT_espsettings;
        private System.Windows.Forms.ComboBox cmb_baud;
        private System.Windows.Forms.TextBox txt_ssid;
        private System.Windows.Forms.TextBox txt_password;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmb_channel;
    }
}
