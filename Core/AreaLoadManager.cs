using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;

namespace IoTControl.Core
{
    public static class AreaLoadManager
    {
        private static Area LoadFile(string path)
        {
            Area temp = new Area();
            temp.Name = path.Substring(path.LastIndexOf('/')).Replace("_","").Split('.')[0];
            temp.Path = path;
            using (var reader = File.OpenText(path))
            {
                string line = "";
				int currentLine = 1;
				while ((line = reader.ReadLine()) != null)
                {
					temp.IoTs.Add(new IoT(line.Split(';')));
					currentLine++;
                }
                    
            }
            return temp;
        }
        public static List<Area> LoadAreas()
        {
            try
            {
                List<Area> teams = new List<Area>();
                foreach (string file in Directory.EnumerateFiles(Environment.CurrentDirectory + "/Areas/", "_*.txt", SearchOption.AllDirectories))
                {
                    teams.Add(LoadFile(file));
                    Console.WriteLine(file);
                }
                return teams;
            }
            catch
            {
                string messageBoxText = "Гайд по настройке IoT control center:\nВам нужно создать папку Areas в директории IoT control center, затем создать в нём файл _{название команды}.txt," +
					" потом написать в нём:\nИнформацию об устройствах(вещах) с таким шаблоном:\n{type};{номер узла};{IPAddress Thing};{Port Thing};{Имя вещи}\nБазированные type - P, M, R2, C, T, B";
                string caption = "Отсутствует папка Teams/Нарушена структура файла команды";
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBoxImage icon = MessageBoxImage.Error;
                MessageBoxResult result;

                result = MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.OK);
                Environment.Exit(0);
                return null; //без него ошибка
            }
        }
    }
}
