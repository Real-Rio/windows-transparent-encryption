﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.Threading;


namespace ModernDashboard.View
{
    /// <summary>
    /// Interaction logic for DownloadView.xaml
    /// </summary>
    public partial class DownloadView : UserControl
    {
        public DownloadView()
        {
            InitializeComponent();
        }

        [DllImport("PocUserDll.dll")]
        static extern int PocUserInitCommPort(ref IntPtr hPort);
        [DllImport("PocUserDll.dll")]
        static extern int PocUserGetMessage(IntPtr hPort, ref uint Command);
        [DllImport("PocUserDll.dll")]
        static extern int PocUserGetMessageEx(IntPtr hPort, ref uint Command, StringBuilder MessageBuffer);
        [DllImport("PocUserDll.dll")]
        static extern int PocUserSendMessage(IntPtr hPort, string Buffer, int Command);
        [DllImport("PocUserDll.dll")]
        static extern int PocUserAddProcessRules(IntPtr hPort, string ProcessName, uint Access);
        [DllImport("Kernel32.dll")]
        static extern bool CloseHandle(IntPtr hObject);

        const uint STATUS_SUCCESS = 0x00000001;

        const int POC_PR_ACCESS_READWRITE = 0x00000001;
        const int POC_PR_ACCESS_BACKUP = 0x00000002;

        const int POC_GET_PROCESS_RULES = 0x00000005;
        const int POC_ADD_PROCESS_RULES = 0x00000009;

        const int POC_ADD_SECURE_EXTENSION = 0x00000003;
        const int POC_GET_FILE_EXTENSION = 0x00000006;
        const int POC_REMOVE_FILE_EXTENSION = 0x00000007;

        const int POC_PROCESS_RULES_SIZE = 324;
        const int POC_FILE_EXTENSION_SIZE = 32;

        private static IntPtr hPort;

        private void GetMessageThread()
        {
            uint ReturnCommand = 0;
            PocUserGetMessage(hPort, ref ReturnCommand);

            if (POC_ADD_SECURE_EXTENSION == ReturnCommand)
            {
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    ListBox.Items.Add(Ext.Text);
                }));

                MessageBox.Show("Poc add file extension success.");
            }
            else if(POC_REMOVE_FILE_EXTENSION == ReturnCommand)
            {
                MessageBox.Show("Poc remove file extension success.");
            }
            else
            {
                string ErrorText = "Poc failed!->ReturnCommand = %d";
                ErrorText = ErrorText + ReturnCommand.ToString("X");
                MessageBox.Show(ErrorText);

                if (0 != hPort.ToInt32())
                {
                    //MessageBox.Show("hPort close.");
                    CloseHandle(hPort);
                    hPort = (IntPtr)0;
                }

                return;
            }

            if (0 != hPort.ToInt32())
            {
                //MessageBox.Show("hPort close.");
                CloseHandle(hPort);
                hPort = (IntPtr)0;
            }
        }


        private void AddListBox()
        {
            uint ReturnCommand = 0;

            StringBuilder ReplyBuffer = new StringBuilder(4096 * 10);

            int count = PocUserGetMessageEx(hPort, ref ReturnCommand, ReplyBuffer);

            if (POC_GET_FILE_EXTENSION == ReturnCommand)
            {
                //MessageBox.Show("Poc flush process rules success.");
            }
            else
            {
                string ErrorText = "Poc failed!->ReturnCommand = ";
                ErrorText = ErrorText + ReturnCommand.ToString("X");
                MessageBox.Show(ErrorText);

                if (0 != hPort.ToInt32())
                {
                    //MessageBox.Show("hPort close.");
                    CloseHandle(hPort);
                    hPort = (IntPtr)0;
                }

                return;
            }

            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                ListBox.Items.Clear();
            }));

            ReplyBuffer.ToString().Replace("  ", "\0\0");

            for (int i = 0; i < count; i++)
            {
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    ListBox.Items.Add(ReplyBuffer.ToString().Substring(i * POC_FILE_EXTENSION_SIZE, POC_FILE_EXTENSION_SIZE));
                }));
            }

            if (0 != hPort.ToInt32())
            {
                //MessageBox.Show("hPort close.");
                CloseHandle(hPort);
                hPort = (IntPtr)0;
            }
        }


        private void FileExt_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (0 == hPort.ToInt32())
            {
                //MessageBox.Show("New port init.");
                int ret = PocUserInitCommPort(ref hPort);
                if (0 != ret)
                {
                    return;
                }
            }

            Thread thread = new Thread(new ThreadStart(AddListBox));
            thread.Start();

            PocUserSendMessage(hPort, "Get Ext", POC_GET_FILE_EXTENSION);
        }

        private void RemoveExt_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (0 == hPort.ToInt32())
            {
                //MessageBox.Show("New port init.");
                int ret = PocUserInitCommPort(ref hPort);
                if (0 != ret)
                {
                    return;
                }
            }

            Thread thread = new Thread(new ThreadStart(GetMessageThread));
            thread.Start();

            String Extension = ListBox.SelectedItem.ToString();

            PocUserSendMessage(hPort, Extension.Replace("  ", "\0\0"), POC_REMOVE_FILE_EXTENSION);

            ListBox.Items.Remove(ListBox.SelectedItem);
        }

        private void AddExt_Click(object sender, System.Windows.RoutedEventArgs e)
        {

            if (0 == hPort.ToInt32())
            {
                //MessageBox.Show("New port init.");
                int ret = PocUserInitCommPort(ref hPort);
                if (0 != ret)
                {
                    return;
                }
            }

            Thread thread = new Thread(new ThreadStart(GetMessageThread));
            thread.Start();

            PocUserSendMessage(hPort, Ext.Text, POC_ADD_SECURE_EXTENSION);
        }

        private void Ext_MouseEnter(object sender, MouseEventArgs e)
        {
            if (string.Compare(Ext.Text, "txt(每次只能添加一个)") == 0)
                Ext.Text = "";
        }

    }
}
