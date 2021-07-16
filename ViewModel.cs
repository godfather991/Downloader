using Microsoft.WindowsAPICodePack.Dialogs;
using Prism.Commands;
using Prism.Mvvm;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;

namespace WpfApp1
{
    //下载器ViewModel(后台逻辑
     public class ViewModel : BindableBase //应用Prism框架
    {
        public ViewModel()
        {
        }
        private DelegateCommand _downloadstart;
        public DelegateCommand DownloadStart
        {
            get
            {
                if (_downloadstart == null) _downloadstart = new DelegateCommand(() => DownloadFileFromServer());
                return _downloadstart;
            }
        }
        private DelegateCommand _downloadcancel;
        public DelegateCommand Downloadcancel
        {
            get
            {
                if (_downloadcancel == null) _downloadcancel = new DelegateCommand(() => DownloadCancellation());
                return _downloadcancel;
            }
        }
        private CancellationTokenSource tokenSource = null; //用于取消操作
        private bool _enable = true; //按钮是否可用
        public bool Enable
        {
            get { return _enable; }
            set { if (_enable != value) { _enable = value; RaisePropertyChanged(); } }
        }
        private long progress;
        public long Progress
        {
            get { return progress; }
            set { if (progress != value) { progress = value;RaisePropertyChanged(); } }
        }
        private long maxlength = 10;
        public long Maxlength
        {
            get { return maxlength; }
            set { if(maxlength!= value) { maxlength = value;RaisePropertyChanged(); } }
        }
        public void DownloadFileFromServer()
        {
            //指定下载URL
            string serverFilePath = "https://cloud.tsinghua.edu.cn/f/1353408044844e45bd03/?dl=1";
            //打开文件夹选择器
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            CommonFileDialogResult result = dialog.ShowDialog();
            if (result == CommonFileDialogResult.Cancel) { return; }
            //获取下载文件保存路径(路径下新建LocalDownloader文件夹并存储)
            string dirPath = Path.Combine(dialog.FileName, "LocalDownloader");
            if (!Directory.Exists(dirPath))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(dirPath);
                directoryInfo.Create();
            }
            string targetPath = Path.Combine(dirPath);
            Progress = 0;
            Maxlength = 10;
            Enable = false;
            //实例化取消模块
            tokenSource = new CancellationTokenSource();
            //异步操作以保证UI线程不死锁
            Thread thread = new Thread(() =>
              {
                  DownloadFile(serverFilePath, targetPath);
              });
            thread.IsBackground = true;
            thread.Start();
        }
        public void DownloadFile(string serverFilePath, string targetPath)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(serverFilePath);
            WebResponse respone = request.GetResponse();
            Stream netStream = respone.GetResponseStream();
            //获取文件名
            string filename = respone.ResponseUri.Segments.Last();
            //获取下载条最长长度
            Maxlength = respone.ContentLength;
            using (Stream fileStream = new FileStream(Path.Combine(targetPath, filename), FileMode.Create))
            {
                byte[] read = new byte[1024];
                int realReadLen = netStream.Read(read, 0, read.Length);
                //下载文件循环
                while (realReadLen > 0)
                {
                    //取消操作
                    if (tokenSource.IsCancellationRequested)
                    {
                        Progress = 0;
                        Maxlength = 10;
                        fileStream.Dispose();
                        File.Delete(Path.Combine(targetPath, filename));
                        tokenSource = null;
                        Enable = true;
                        return;
                    }
                    //进度条进度更新
                    Progress += realReadLen;
                    fileStream.Write(read, 0, realReadLen);
                    realReadLen = netStream.Read(read, 0, read.Length);
                }
                netStream.Close();
                fileStream.Close();
            }
            Enable = true;
            MessageBox.Show("Download Complete");
        }
        public void DownloadCancellation()
        {
            if (tokenSource != null) tokenSource.Cancel();
        }
    }
}