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
using System.Diagnostics;
using System.Windows.Threading;

namespace Client.Dialog_Windows
{
    public partial class ProcessWindow : Window
    {
        ProcessWindowViewModel viewModel = new ProcessWindowViewModel();
        MainWindow window;
        DispatcherTimer timer = new DispatcherTimer();

        public ProcessWindow(MainWindow window)
        {
            DataContext = viewModel;
            this.window = window;
            InitializeComponent();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            try
            {
                UpdateProcessList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            timer.Start();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            timer.Stop();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            try
            {
                UpdateProcessList();
            }
            catch (Exception ex)
            {
                timer.Stop();
                viewModel.Clear();
                MessageBox.Show(ex.Message);
            }
        }

        void UpdateProcessList()
        {
            string answer = window.SendMessageFromSocket(window.Port, "2;");
            string[] processes = answer.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            List<ProcessModel> list = new List<ProcessModel>();
            for (int i = 0; i < processes.Length; i++)
            {
                string[] process = processes[i].Split('|');
                int id;
                if(int.TryParse(process[1],out id))
                list.Add(new ProcessModel() { ProcessName = process[0], ProcessId = id });
            }
            viewModel.List = new ObservableCollection<ProcessModel>(list);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender == null) return;
            window.SendMessageFromSocket(window.Port, "3;" + ((Button)sender).Tag.ToString());
            UpdateProcessList();
        }


    }

    public partial class ProcessWindowViewModel : INotifyPropertyChanged
    {
        ObservableCollection<ProcessModel> list = new ObservableCollection<ProcessModel>();

        public void Clear()
        {
            List = new ObservableCollection<ProcessModel>();
        }

        public ObservableCollection<ProcessModel> List
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
