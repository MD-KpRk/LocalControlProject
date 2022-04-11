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
            InitializeComponent();
            UpdateProcessList();
        }


        void UpdateProcessList()
        {
            try
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
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }

    public partial class ProcessWindowViewModel : INotifyPropertyChanged
    {
        ObservableCollection<ProcessModel> list = new ObservableCollection<ProcessModel>();

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
