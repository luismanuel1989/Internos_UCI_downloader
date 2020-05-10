using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Internos_douwnloaderv2._0Net.Class;
using System.Runtime.InteropServices;
using Microsoft.WindowsAPICodePack.Taskbar;
using System.Globalization;

namespace Internos_douwnloaderv2._0Net
{
    
    public partial class Form1 : Form
    {
        TaskbarManager taskbar = TaskbarManager.Instance;
        
        string url_base;
        Thread_Pool pool = new Thread_Pool();
        DateTime downloadStartTime;
        double total_evolution_progress = 0;
        bool Finished_message_throwed = false;
        bool asyncTask1Busy = false;
        int asyncTask1VideoIndex = -1;
        bool asyncTask2Busy = false;
        int asyncTask2VideoIndex = -1;


        public Form1()
        {           
            DateTime expire = new DateTime(2020,4,12);            
            if ((DateTime.Now - expire).TotalDays>30)
            {
                MessageBox.Show("Please contact luismanuel.8902@gmail.com for a free update", "Application out of range", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                if (System.Windows.Forms.Application.MessageLoop)
                {
                    System.Windows.Forms.Application.Exit();
                }
                else
                {
                    System.Environment.Exit(1);
                }
            }            
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            InitializeComponent();
            label1.ForeColor = Color.White;
            label2.ForeColor = Color.White;
            label3.ForeColor = Color.White;
            label4.ForeColor = Color.White;
            label5.ForeColor = Color.White;
            label6.ForeColor = Color.White;
            label7.ForeColor = Color.White;
            label8.ForeColor = Color.White;
            checkBox1.ForeColor = Color.White;

            panel2.Visible = false;
            checkBox1.Checked = true;
            pool.Threads_lim1 = 2;            
            //progressBar2.SetState(2);
        }


        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            if (textBox1.Text == "Internos URL")
            {
                textBox1.Text = "";
            }
        }

        private void textBox1_MouseHover(object sender, EventArgs e)
        {
            if (textBox1.Text == "Internos URL")
            {
                textBox1.Text = "";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string url = textBox1.Text;
            if (!IsCorrectInternosUrl(url))
            {
                MessageBox.Show("Wrong Url","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
            else
            {
                 
                Task.Run(() =>
                {
                    //DialogResult mbWait = MessageBox.Show("Please wait a second, we are gathering the information", "Gathering information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    AutoClosingMessageBox.Show("Please wait a second, we are gathering the information", "Gathering information", 4000);

                });
                try
                {

                    DateTime start = DateTime.Now;
                    downloadStartTime = start;
                    string html = Obtain_HTML(url);

                    define_url_base(html);
                    //MessageBox.Show(url_base);
                    List<string> videosList = ObtainVideosList(html);
                    videosList.Sort();
                    panel1.Visible = false;
                    panel2.Visible = true;
                    foreach (string item in videosList)
                    {
                        checkedListBox1.Items.Add(item);
                    }
                    label1.Text = "Information requested in: " + String.Format("{0:0.00}", (DateTime.Now - start).TotalSeconds) + " seconds";
                }
                catch (Exception excepction_object)
                {
                    Task.Run(()=> {
                        AutoClosingMessageBox.Show("There was a problem connecting to internos, and was this: \n"+ excepction_object.ToString(), "Connection error", 40000);
                    });
                }
            }
            
        }
        private List<string> ObtainVideosList(string html) {
            List<string> videosList = new List<string>();
            string[] videos= html.Split(new string[] { "<div class=\"media-actions\">" }, StringSplitOptions.None);
            videos = videos.Skip(1).ToArray();
            foreach (string item in videos)
            {
                string it = item.Split(new string[] { "/archivo_multimedia/video/descargar?almacen=" }, StringSplitOptions.None)[1].Split(new string[] { "&amp;" },StringSplitOptions.None)[1].Split(new string[] { "A80/" },StringSplitOptions.None)[1];
                videosList.Add(it);
            }
            return videosList;
        }
        private bool IsCorrectInternosUrl(string url) {
            if (!url.Contains("http") || url.Equals(null) || !url.Contains("internos"))
            {
                return false;
            }
            return true;
        }
        private string Obtain_HTML(string url) {
            
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage response = client.GetAsync(url).Result)
                {
                    using (HttpContent content = response.Content)
                    {                        
                        string result = content.ReadAsStringAsync().Result;                        
                        return result;
                    }
                }
            }
            
        }
        private void define_url_base(string html) {
            int minternos = (html.Split(new string[] { "minternos.uci.cu" }, StringSplitOptions.None)).Length;
            int sinternos = (html.Split(new string[] { "sinternos.uci.cu" }, StringSplitOptions.None)).Length;
            if (minternos>sinternos)
            {
                url_base = "http://minternos.uci.cu/";
            }
            else
            {
                url_base = "http://sinternos.uci.cu/";
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar==(char)Keys.Enter)
            {
                button1_Click(sender,e);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (check_if_is_at_least_one_checked())
            {
                uncheck_all_items();
            }
            else
            {
                check_all_items();
            }
        }
        private bool check_if_is_at_least_one_checked() {
            foreach (string item in checkedListBox1.CheckedItems)
            {
                return true;
            }
            return false;
        }
        private void check_all_items() {
            for (int i = 0; i < (checkedListBox1.Items).Count; i++)
            {
                checkedListBox1.SetItemChecked(i,true);
            }
        }
        private void uncheck_all_items() {
            for (int i = 0; i < (checkedListBox1.Items).Count; i++)
            {
                checkedListBox1.SetItemChecked(i, false);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (checkedListBox1.CheckedItems.Count != 0)
            {
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                DialogResult result = fbd.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    download_checked_videos(fbd.SelectedPath);
                }
                
            }
            else
                MessageBox.Show("You need to select at least one video.","Selection error",MessageBoxButtons.OK,MessageBoxIcon.Error);
            
        }
        private void download_checked_videos(string folder_path) {
            if (pool.Threads_lim1==1)            
            HideSecondThreadVisualComponents();
            panel2.Visible = false;
            panel3.Visible = true;
            pool.Folder_path = folder_path;
            DateTime start = DateTime.Now;            
            pool.Fill_videos_list(checkedListBox1.CheckedItems);            
            MyOwnFactoryMethod_Next_download();
            //            MessageBox.Show(pool.get_videos_Count().ToString());
            //foreach (string item in checkedListBox1.CheckedItems)
            //{
            //    //label2.Text = "Downloading "+item+" file";

            //    //client.DownloadProgressChanged += client_DownloadProgressChanged;
            //    ////client. += client_Disposed;
            //    //client.DownloadFileAsync(new Uri(url_base + item), folder_path + "/" + item);                                
            //}
            //while (client.IsBusy)
            //{

            //}
            //MessageBox.Show((DateTime.Now-start).ToString());
        }
        public void HideSecondThreadVisualComponents() {
            label7.Visible = false;
            label8.Visible = false;
            progressBar3.Visible = false;
        }
        private void MyOwnFactoryMethod_Next_download() {
            if (pool.Actual_amount_threads < pool.Threads_lim1&& pool.Index < pool.get_videos_Count())
            {
                if (!asyncTask1Busy)
                {
                    asyncTask1Busy = true;
                    asyncTask1VideoIndex = pool.Index;
                    WebClient client = new WebClient();
                    client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadDataCompleted1);
                    client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged1);
                    client.DownloadFileAsync(new Uri(url_base + pool.get_Next_Video_to_download()), pool.Folder_path + "/" + pool.get_Next_Video_to_download());
                    label2.Text = "Downloading " + pool.get_Next_Video_to_download() + " file.";

                    pool.Actual_amount_threads += 1;
                    pool.Index++;
                    MyOwnFactoryMethod_Next_download();
                }
                else if (!asyncTask2Busy)
                {
                    asyncTask2Busy = true;
                    asyncTask2VideoIndex = pool.Index;
                    WebClient client = new WebClient();
                    client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadDataCompleted2);
                    client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged2);
                    client.DownloadFileAsync(new Uri(url_base + pool.get_Next_Video_to_download()), pool.Folder_path + "/" + pool.get_Next_Video_to_download());
                    label7.Text = "Downloading " + pool.get_Next_Video_to_download() + " file.";

                    pool.Actual_amount_threads += 1;
                    pool.Index++;
                    MyOwnFactoryMethod_Next_download();
                }
                
            }
            
        }
        private void client_DownloadDataCompleted1(object sender, AsyncCompletedEventArgs e) {
            
            if (pool.AllVideoOneHundredPercentFinished())
            {
                MessageBox.Show("Download process finished in " + ((DateTime.Now - downloadStartTime).ToString()) + " seconds","Process finished",MessageBoxButtons.OK,MessageBoxIcon.Information);
            }

            pool.Actual_amount_threads -= 1;
            if (pool.Index == pool.get_videos_Count() && !Finished_message_throwed)
            {
                //MessageBox.Show("Download process finished ");
                Finished_message_throwed = true;
            }
            asyncTask1Busy = false;
            pool.UpdateDownloadPercentVideosList(asyncTask1VideoIndex,100);
            MyOwnFactoryMethod_Next_download();

        }
        private void client_DownloadDataCompleted2(object sender, AsyncCompletedEventArgs e)
        {

            if (pool.AllVideoOneHundredPercentFinished())
            {
                MessageBox.Show("Download process finished in " + ((DateTime.Now - downloadStartTime).ToString()) + " seconds", "Process finished", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            pool.Actual_amount_threads -= 1;
            if (pool.Index == pool.get_videos_Count() && !Finished_message_throwed)
            {
                //MessageBox.Show("Download process finished ");
                Finished_message_throwed = true;
            }
            asyncTask2Busy = false;
            pool.UpdateDownloadPercentVideosList(asyncTask2VideoIndex, 100);
            MyOwnFactoryMethod_Next_download();

        }
        private void client_DownloadProgressChanged1(object sender, DownloadProgressChangedEventArgs e)
        {
            
            pool.UpdateDownloadPercentVideosList(asyncTask1VideoIndex, e.ProgressPercentage);
            label3.Text = e.ProgressPercentage.ToString() + " %";
            //double total_ev = ((double)(pool.Index - 1) / (double)pool.get_videos_Count() * 100 + (double)e.ProgressPercentage / (double)(pool.get_videos_Count()));
            double total_ev = pool.getDownloadPercent();
            if (total_evolution_progress<total_ev)
            {
                total_evolution_progress = total_ev;
                label5.Text = (Math.Round(total_evolution_progress, 0)).ToString() + " %";

            }
            
            progressBar1.Value = Convert.ToInt32(e.ProgressPercentage);
            progressBar2.Value = Convert.ToInt32(total_evolution_progress);
            taskbar.SetProgressValue(Convert.ToInt32(total_evolution_progress), 100);

        }
        private void client_DownloadProgressChanged2(object sender, DownloadProgressChangedEventArgs e)
        {
            pool.UpdateDownloadPercentVideosList(asyncTask2VideoIndex, e.ProgressPercentage);
            label8.Text = e.ProgressPercentage.ToString() + " %";
            double total_ev = pool.getDownloadPercent();
            if (total_evolution_progress < total_ev)
            {
                total_evolution_progress = total_ev;
                label5.Text = (Math.Round(total_evolution_progress,0)).ToString() + " %";

            }

            progressBar3.Value = Convert.ToInt32(e.ProgressPercentage);
            progressBar2.Value = Convert.ToInt32(total_evolution_progress);
            taskbar.SetProgressValue(Convert.ToInt32(total_evolution_progress), 100);

        }

        private void client_Disposed(object senser) { }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
                pool.Threads_lim1 = 2;
            else
                pool.Threads_lim1 = 1;
        }
    }
    public static class ModifyProgressBarColor
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr w, IntPtr l);
        public static void SetState(this ProgressBar pBar, int state)
        {
            SendMessage(pBar.Handle, 1040, (IntPtr)state, IntPtr.Zero);
        }
    }
}
