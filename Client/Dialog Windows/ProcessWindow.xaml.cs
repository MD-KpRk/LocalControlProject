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

using System.Drawing;         //For Icon
using System.Reflection;      //For Assembly
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Client.Dialog_Windows
{
    /// <summary>
    /// Логика взаимодействия для ProcessWindow.xaml
    /// </summary>
    public partial class ProcessWindow : Window
    {
        public ProcessWindow()
        {
            //var icon = Process.GetProcessById(1234).GetIcon()
            InitializeComponent();
        }
    }



    
}
