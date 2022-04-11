using Client.Dialog_Windows;
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
        public int Port = 8889;

        MainWindowViewModel viewModel = new MainWindowViewModel();
        Config config = new Config("cfg");
        public MainWindow()
        {
            viewModel.IP = config.GetIP();
            DataContext = viewModel;
            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            config.SetIP(viewModel.IP);
        }

        public string SendMessageFromSocket(int port, string message)
        {
            if (message == null) return "";
            if (String.IsNullOrEmpty(message)) return "";

            byte[] bytes = new byte[1024];
            string answer = "";
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(viewModel.IP), port);
            Socket sender = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                sender.Connect(ipEndPoint, TimeSpan.FromSeconds(1));
            }
            catch (Exception)
            {
                MessageBox.Show("Цель недоступна");
                return answer;
            }

            Debug.WriteLine("Сокет соединяется с " + sender.RemoteEndPoint.ToString());
            byte[] msg = Encoding.UTF8.GetBytes(message);

            sender.Send(msg);

            if (message[0] == '0')
            {
                int bytesRec = sender.Receive(bytes);
                answer = Encoding.UTF8.GetString(bytes, 0, bytesRec);
                Debug.WriteLine("\nОтвет от сервера: " + answer);
            }

            if (message[0] == '2')
            {
                int bytesRec = sender.Receive(bytes);
                answer = Encoding.UTF8.GetString(bytes, 0, bytesRec);
                Debug.WriteLine("\nОтвет от сервера: " + answer);
            }

            // Освобождаем сокет
            sender.Shutdown(SocketShutdown.Both);
            sender.Close();
            return answer;
        }

        private void Button_Click(object sender, RoutedEventArgs e) // Проверка связи
        {
            SendMessageFromSocket(Port, "0;test");
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            SendMessageFromSocket(Port, viewModel.Command);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Dialog_Windows.MessageWindow messageWindow = new Dialog_Windows.MessageWindow();
            messageWindow.ShowDialog();
            if (messageWindow.Succes == false) return;
            SendMessageFromSocket(Port, "1;" + messageWindow.viewModel.Message);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            ProcessWindow processWindow = new ProcessWindow(this);
            processWindow.Show();
        }
    }

    public static class SocketExtensions
    {
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
        string ip ="", command ="";
        public string IP
        {
            get { return ip; }
            set
            {
                ip = value;
                OnPropertyChanged("IP");
            }
        }

        public string Command
        {
            get { return command; }
            set 
            { 
                command = value; 
                OnPropertyChanged("Command"); 
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
