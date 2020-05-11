using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Internos_douwnloaderv2._0Net.Class
{
    public class Thread_Pool
    {
        int Threads_lim = 1;
        int actual_amount_threads = 0;
        int indexFinished = 0;
        int index = 0;
        string folder_path;        
        List<string> videos_list = new List<string>();
        List<int> videosDownloadPercent = new List<int>();
        public Thread_Pool()
        {
            
        }

        public void Fill_videos_list(System.Windows.Forms.CheckedListBox.CheckedItemCollection list) {
            foreach (string item in list)
            {
                videos_list.Add(item);
                videosDownloadPercent.Add(0);
            }

        }
        public List<string> GetVideosList() {
            return videos_list;
        }
        public int get_videos_Count() {
            return videos_list.Count;
        }
        public string get_Next_Video_to_download() {
            return videos_list[index];
        }
        public void UpdateDownloadPercentVideosList(int videoIndex,int percent) {
            videosDownloadPercent[videoIndex] = percent;
        }
        public bool AllVideoOneHundredPercentFinished() {
            foreach (int item in videosDownloadPercent)
            {
                if (item!=100)
                {
                    return false;
                }
            }
            return true;
        }
        public double getDownloadPercent() {
            int sum = 0;
            foreach (int item in videosDownloadPercent)
            {
                sum += item;
            }

            return (double)sum/(double)videosDownloadPercent.Count;
        }
        public void try_download_next_video() {
            if (actual_amount_threads<Threads_lim1 && Index<videos_list.Count)
            {
                WebClient client = new WebClient();
                client.DownloadProgressChanged += client_DownloadProgressChanged;

                Actual_amount_threads += 1;
                Index++;
            }
        }
        private void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {

           

        }


        public int Actual_amount_threads { get => actual_amount_threads; set => actual_amount_threads = value; }
        public int Threads_lim1 { get => Threads_lim; set => Threads_lim = value; }
        public int Index { get => index; set => index = value; }
        public string Folder_path { get => folder_path; set => folder_path = value; }
    }
}
