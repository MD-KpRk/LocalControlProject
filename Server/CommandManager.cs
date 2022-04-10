using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Server
{
    public static class CommandManager
    {

        public static void FindCommand(string command)
        {
            if(!command.Contains(";"))
            {
                throw new Exception("Неправильный формат команды");
            }
            
            string[] args = command.Split(';');

            int intcom = Convert.ToInt32(args[0]);
            string param = args[1].Trim();

            switch(intcom)
            {
                case 1:
                    ShowMessage(param);
                    break;




                default: 
                    throw new ArgumentException("Запрос несуществующей команды");
            }
        }





        public static void ShowMessage(string str)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                MessageWindow window = new MessageWindow(str);
                window.Show();
            });
        }

    }
}
