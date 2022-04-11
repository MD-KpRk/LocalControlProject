using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
using System.Windows.Shapes;

namespace Client.Dialog_Windows
{
    public partial class MessageWindow : Window
    {
        public MessageWindowViewModel viewModel = new MessageWindowViewModel();
        bool succes = false;
        public bool Succes { get => succes; }
        public MessageWindow()
        {
            DataContext = viewModel;
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e) // Отмена
        {
            succes = false;
            Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e) // Отправить
        {
            succes = true;
            Close();
        }
    }

    public partial class MessageWindowViewModel : INotifyPropertyChanged
    {
        string message = "";
        public string Message
        {
            get { return message; }
            set
            {
                message = value;
                OnPropertyChanged("Message");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
