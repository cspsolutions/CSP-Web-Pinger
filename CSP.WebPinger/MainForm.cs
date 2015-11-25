using CSP.WebPinger.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSP.WebPinger
{
    public partial class MainForm : Form
    {
        SettingsManager _settingsManager = null;

        public MainForm()
        {
            InitializeComponent();

            this.Initialize();
        }

        private void Initialize()
        {
            this._settingsManager = SettingsManager.Instance();

            this.Load += MainForm_Load;
        }

        void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                this.lb_Sites.DataSource = this._settingsManager.GetWebsites();
            }
            catch  
            { 
            }
        }

        private void btn_AddWebsite_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(this.tb_Url.Text))
                {
                    this._settingsManager.AddWebsite(this.tb_Url.Text);

                    this.lb_Sites.DataSource = this._settingsManager.GetWebsites();
                }
            }
            catch
            {

            }
        }



        private bool PingUrl(string url)
        {
            bool success = false;
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "GET";
            String test = String.Empty;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                test = reader.ReadToEnd();
                reader.Close();
                dataStream.Close();

                success = true;
            }

            return success;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            foreach (string url in this._settingsManager.GetWebsites())
            {
                try
                {
                    this.PingUrl(url);
                }
                catch 
                { 
                }
            }

        }


        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            this.progressBar1.Visible = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                this.backgroundWorker1.RunWorkerAsync();
            }
            catch 
            {
                
            }
        }

        private void btn_Start_Click(object sender, EventArgs e)
        {
            try
            {
                int minutes = 10;

                if (this.numericUpDown1.Value > 0)
                {
                    minutes = (int)this.numericUpDown1.Value;
                }

                this.timer1.Interval = 60000 * minutes;
                this.timer1.Start();

                this.backgroundWorker1.RunWorkerAsync();
            }
            catch
            {

            }
        }

        private void btn_Clear_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to clear all URls", "Confirmation", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                try
                {
                    this._settingsManager.Clear();
                    this.lb_Sites.DataSource = null;
                }
                catch
                {

                }
            }
        }


    }
}
