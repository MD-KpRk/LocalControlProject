using System;
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
using System.Windows.Shapes;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using Client.Models;

namespace Client.Dialog_Windows
{
    public partial class ProcessWindow : Window
    {
        ProcessWindowViewModel viewModel = new ProcessWindowViewModel();
        MainWindow window;
        public ProcessWindow(MainWindow window)
        {
            DataContext = viewModel;
            this.window = window;
            UpdateProcessList();
            InitializeComponent();
        }


        void UpdateProcessList()
        {
            string answer = window.SendMessageFromSocket(window.Port, "2;");
            MessageBox.Show(answer);
        }

    }

    public partial class ProcessWindowViewModel : INotifyPropertyChanged
    {
        ObservableCollection<ProcessModel> list = new ObservableCollection<ProcessModel>();

        ObservableCollection<ProcessModel> List
        {
            get { return list; }
            set { list = value; OnPropertyChanged("List"); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }

}
