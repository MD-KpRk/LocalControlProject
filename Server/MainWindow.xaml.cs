using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
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
                            foreach (Process prs in process)
                            {
                                sb.Append(prs.ProcessName + "         (" + prs.PrivateMemorySize64.ToString() + ")\n");
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

}
