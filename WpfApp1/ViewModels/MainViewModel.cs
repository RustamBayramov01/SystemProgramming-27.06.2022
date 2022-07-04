using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp1.ViewModels
{
    public class MainViewModel : ViewModelBase
    {

        public Thread thread;

        private string _fromtxt;
        
        private string _totxt;
        
        private int _filesize;
        
        private int _cursize;


        public string FromTxt
        {
            get => _fromtxt; set { _fromtxt = value; RaisePropertyChanged(); }
        }
    
        public string ToTxt
        {
            get => _totxt; set { _totxt = value; RaisePropertyChanged(); }
        }


        public int FileSize
        {
            get => _filesize; set { _filesize = value; RaisePropertyChanged(); }
        }

        public int CurSize
        {
            get => _cursize; set { _cursize = value; RaisePropertyChanged(); }
        }



        public RelayCommand FromCommand
        {
            get => new RelayCommand
            (() =>
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                if (openFileDialog.ShowDialog() == true)
                    FromTxt = openFileDialog.FileName;
            });
        }

        public RelayCommand ToCommand
        {
            get => new RelayCommand
            (() =>
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                if (openFileDialog.ShowDialog() == true)
                    ToTxt = openFileDialog.FileName;
            });
        }



        public RelayCommand CopyCommand
        {
            get => new RelayCommand(
                () =>
                {
                    try
                    {
                        thread = new Thread(Copy);
                        thread.Start();
                    }
                    catch (Exception ex) { MessageBox.Show(ex.Message); }
                }
                );
        }

        [Obsolete]
        public RelayCommand SuspendCommand
        {
            get => new RelayCommand(
            () =>
            {
                try { thread.Suspend(); }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }
            );
        }


        [Obsolete]
        public RelayCommand ResumeCommand
        {
            get => new RelayCommand(
            () =>
            {
                try { thread.Resume(); }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }
            );
        }

        [Obsolete]
        public RelayCommand AbortCommand
        {
            get => new RelayCommand(
            () =>
            {
                try
                {
                    FileSize = 0;
                    CurSize = 0;
                    thread.Abort();
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }
            );
        }


        public void Copy()
        {
            if (!File.Exists(FromTxt)) { return; }

            using (FileStream Read = new FileStream(FromTxt, FileMode.Open, FileAccess.Read))
            {
                Console.WriteLine($"Size {Read.Length} bytes");
                using (FileStream fsWrite = new FileStream(ToTxt, FileMode.Create, FileAccess.Write))
                {
                    var Len = 10;
                    byte[] Bf = new byte[Len];
                    var FileSize = Read.Length;
                    this.FileSize = (int)FileSize;

                    while(Len != 0)
                    {
                        Len = Read.Read(Bf, 0, Bf.Length);
                        fsWrite.Write(Bf, 0, Len);
                        FileSize -= Len;
                        CurSize = this.FileSize - (int)FileSize;
                    }

                }
            }
        }

    }
}
