using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    internal class Config
    {
        string name;
        string extension = ".txt";
        public Config(string Name)
        {
            this.name = Name;
        }

        public string GetIP()
        {
            try
            {
                string text;
                using (StreamReader reader = new StreamReader(name + extension))
                {
                    text = reader.ReadToEnd();
                }
                return text;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public void SetIP(string IP)
        {
            using (StreamWriter writer = new StreamWriter(name + extension,false))
            {
                writer.Write(IP);
            }
        }
    }
}
