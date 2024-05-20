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
    public static class TeamLoadManager
    {
        private static Team LoadFile(string path)
        {
            Team temp = new Team();
            temp.Name = path.Substring(path.LastIndexOf('/')).Replace("_","").Split('.')[0];
            temp.Path = path;
            using (var reader = File.OpenText(path))
            {
                string line = "";
				int currentLine = 1;
				while ((line = reader.ReadLine()) != null)
                {
                    if (currentLine >= 2)
                    {
						temp.IoTs.Add(new IoT(line.Split(';')));
					}
                    else //TODO убрать 
                    {
						//temp.ServerIP = line.Split(';')[0];
						//temp.Appkey = line.Split(';')[1];
					}
					currentLine++;
                }
                    
            }
            return temp;
        }
        public static List<Team> LoadTeams()
        {
            try
            {
                List<Team> teams = new List<Team>();
                foreach (string file in Directory.EnumerateFiles(Environment.CurrentDirectory + "/Teams/", "_*.txt", SearchOption.AllDirectories))
                {
                    teams.Add(LoadFile(file));
                    Console.WriteLine(file);
                }
                return teams;
            }
            catch
            {
                string messageBoxText = "Гайд по настройке IoT control center:\nВам нужно создать папку Teams в директории IoT control center, затем создать в нём файл _{название команды}.txt," +
                    " потом написать в нём:\n\n{IPAddress Thingworx};{Appkey Thingworx}\n\nДальше нужно написать информацию об устройствах(вещах) с таким шаблоном:\n\n{type};{номер узла};{IPAddress Thing};{Port Thing};{Имя вещи на Thingworx};{Имя сервиса вещи на Thingworx}";
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
