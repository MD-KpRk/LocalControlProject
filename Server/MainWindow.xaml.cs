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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {

        public MainWindow()
        {
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

                    Debug.Write("Полученный текст: " + data + "\n\n");

                    if(data == "0")
                    {
                        Debug.WriteLine("Проверка связи");
                        string reply = "Спасибо за запрос в " + data.Length.ToString() + " символов";
                        byte[] msg = Encoding.UTF8.GetBytes(reply);
                        handler.Send(msg);
                    }

                    Text = data;

                    if (data.IndexOf("<TheEnd>") > -1)
                    {
                        Debug.WriteLine("Сервер завершил соединение с клиентом.");
                        break;
                    }

                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
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

    //class Server2
    //{
    //    private IPEndPoint ip;
    //    private Socket socket;
    //    private int max_conn = 2;
    //    private ManualResetEvent acceptEvent = new ManualResetEvent(false);
    //    MainWindow window;

    //    public Server2(MainWindow window)
    //    {
    //        this.window = window;
    //        this.ip = new IPEndPoint(IPAddress.Any, 8888);
    //        this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    //        this.socket.Bind(this.ip);
    //        this.socket.Listen(this.max_conn);
    //    }

    //    public void StartListening()
    //    {
    //        window.Dispatcher.BeginInvoke(new Action(() => { Debug.WriteLine("Server starting..."); }));
    //        while (true)
    //        {
    //            acceptEvent.Reset();
    //            this.socket.BeginAccept(new AsyncCallback(AcceptCallBack), this.socket);
    //            acceptEvent.WaitOne();
    //        }
    //    }

    //    private void AcceptCallBack(IAsyncResult ar)
    //    {
    //        Socket socket = (Socket)ar.AsyncState;
    //        Socket accept_socket = socket.EndAccept(ar);
    //        acceptEvent.Set();

    //        byte[] bytes = new byte[1024];
    //        int bytesRec = accept_socket.Receive(bytes);
    //        string data = "";
    //        data += Encoding.UTF8.GetString(bytes, 0, bytesRec);
    //        Debug.Write("Полученный текст: " + data + "\n\n");
    //    }
    //}

}
