using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
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

namespace Client
{
    public partial class MainWindow : Window
    {
        MainWindowViewModel viewModel = new MainWindowViewModel();
        Config config = new Config("cfg");
        public MainWindow()
        {
            viewModel.IP = config.GetIP();
            DataContext = viewModel;
            InitializeComponent();
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SendMessageFromSocket(8889);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            config.SetIP(viewModel.IP);
        }

        void SendMessageFromSocket(int port)
        {
            byte[] bytes = new byte[1024];


            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(viewModel.IP), port);
            Socket sender = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Нужна проверка на доступность
            //sender.Connect(ipEndPoint);

            try
            {
                sender.Connect(ipEndPoint, TimeSpan.FromSeconds(1));
            }
            catch (SocketException e)
            {
                if (e.ErrorCode == 10061)
                    MessageBox.Show("Порт закрыт");
                else if (e.ErrorCode == 10060)
                    MessageBox.Show("Цель недоступна");
                else MessageBox.Show(e.Message);
                return;
            }

            Console.Write("Введите сообщение: ");
            string message = "0";

            Debug.WriteLine("Сокет соединяется с " + sender.RemoteEndPoint.ToString());
            byte[] msg = Encoding.UTF8.GetBytes(message);

            int bytesSent = sender.Send(msg);

            if (message == "0")
            {
                int bytesRec = sender.Receive(bytes);
                Debug.WriteLine("\nОтвет от сервера: " + Encoding.UTF8.GetString(bytes, 0, bytesRec));
            }

            // Освобождаем сокет
            sender.Shutdown(SocketShutdown.Both);
            sender.Close();
        }
    }


    // new
    public static class SocketExtensions
    {
        /// <summary>
        /// Connects the specified socket.
        /// </summary>
        /// <param name="socket">The socket.</param>
        /// <param name="endpoint">The IP endpoint.</param>
        /// <param name="timeout">The timeout.</param>
        public static void Connect(this Socket socket, EndPoint endpoint, TimeSpan timeout)
        {
            var result = socket.BeginConnect(endpoint, null, null);

            bool success = result.AsyncWaitHandle.WaitOne(timeout, true);
            if (success)
            {
                socket.EndConnect(result);
            }
            else
            {
                socket.Close();
                throw new SocketException(10060); // Connection timed out.
            }
        }
    }

    public partial class MainWindowViewModel : INotifyPropertyChanged
    {
        string ip ="";
        public string IP
        {
            get { return ip; }
            set
            {
                ip = value;
                OnPropertyChanged("IP");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }


}
