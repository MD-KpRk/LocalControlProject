using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
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

namespace Server
{
    public partial class MainWindow : Window
    {
        MyViewModel viewModel = new MyViewModel();
        public MainWindow()
        {
            DataContext = viewModel;
            InitializeComponent();
            
        }
    }

    public class MyViewModel : INotifyPropertyChanged
    {
        private int _value;
        public int Value
        {
            get => _value;
            set
            {
                if (value == _value) return;
                _value = value;
                OnPropertyChanged();
            }
        }

        private string _text;
        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                OnPropertyChanged();
            }
        }


        private Thread _thread;
        private CancellationTokenSource _tokenSource;

        public MyViewModel()
        {
            _tokenSource = new CancellationTokenSource();
            _thread = new Thread(Worker) { IsBackground = true };
            _thread.Start(_tokenSource.Token);
        }

        private void Worker(object state)
        {
            var token = (CancellationToken)state;

            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, 8889);
            Socket sListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                sListener.Bind(ipEndPoint);
                sListener.Listen(10);

                while (!token.IsCancellationRequested)
                {
                    Debug.WriteLine("Ожидаем соединение через порт "+ ipEndPoint);
                    Socket handler = sListener.Accept();
                    Value++; 
                    byte[] bytes = new byte[1024];
                    int bytesRec = handler.Receive(bytes);
                    string data = "" + Encoding.UTF8.GetString(bytes, 0, bytesRec);

                    try
                    {
                        Debug.Write("Полученный текст: " + data + "\n\n");

                        Text = data;

                        if (data[0] == '0')
                        {
                            Debug.WriteLine("Проверка связи");
                            string reply = "true";
                            byte[] msg = Encoding.UTF8.GetBytes(reply);
                            handler.Send(msg);
                        }
                        if(data[0] == '2')
                        {
                            StringBuilder sb = new StringBuilder();
                            Process[] process = Process.GetProcesses();

                            ProcessWindow[] applications = ProcessHelper.GetRunningApplications();
                            foreach (ProcessWindow pw in applications)
                            {
                                sb.Append(pw.WindowTitle + "|" + pw.Process.Id + "\n");
                            }

                            byte[] msg = Encoding.UTF8.GetBytes(sb.ToString());
                            handler.Send(msg);
                        }
                        else
                        {
                            CommandManager.FindCommand(data);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                    finally
                    {
                        handler.Shutdown(SocketShutdown.Both);
                        handler.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }




 
    public class ProcessWindow
    {
        public string WindowTitle { get; private set; }
        public Process Process { get; private set; }

        public ProcessWindow(string windowTitle, Process process)
        {
            WindowTitle = windowTitle;
            Process = process;
        }
    }
    public static class ProcessHelper
    {
        public static ProcessWindow[] GetRunningApplications()
        {
            var allProccesses = Process.GetProcesses();
            var myPid = Process.GetCurrentProcess().Id;
            var explorerPids = allProccesses.Where(p => "explorer".Equals(p.ProcessName, StringComparison.OrdinalIgnoreCase)).Select(p => p.Id).ToArray();
            var windows = new List<ProcessWindow>();
            EnumDelegate filter = delegate (IntPtr hWnd, int lParam)
            {
                var sbTitle = new StringBuilder(255);
                GetWindowText(hWnd, sbTitle, sbTitle.Capacity + 1);
                string windowTitle = sbTitle.ToString();

                if (!string.IsNullOrEmpty(windowTitle) && IsWindowVisible(hWnd))
                {
                    int pid;
                    GetWindowThreadProcessId(hWnd, out pid);
                    if (pid != myPid && !explorerPids.Contains(pid))
                    {
                        windows.Add(new ProcessWindow(windowTitle, allProccesses.FirstOrDefault(p => p.Id == pid)));
                    }
                }

                return true;
            };

            EnumDesktopWindows(IntPtr.Zero, filter, IntPtr.Zero);
            return windows.ToArray();
        }

        delegate bool EnumDelegate(IntPtr hWnd, int lParam);

        [DllImport("user32.dll", EntryPoint = "EnumDesktopWindows",
        ExactSpelling = false, CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool EnumDesktopWindows(IntPtr hDesktop, EnumDelegate lpEnumCallbackFunction, IntPtr lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        [DllImport("user32.dll", EntryPoint = "GetWindowText",
        ExactSpelling = false, CharSet = CharSet.Auto, SetLastError = true)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpWindowText, int nMaxCount);
    }

}
