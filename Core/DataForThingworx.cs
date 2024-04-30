using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace IoTControl.Core
{
	public class DataForThingworx
	{
		public static string AppKey { set; get; }
		public static string ServerIP { set; get; }
		public DataForThingworx() { }
		public static void LoadThx(string[] line)
		{
			try
			{
				DataForThingworx.ServerIP = line[0];
				DataForThingworx.AppKey = line[1];
			}
			catch (Exception e)
			{
				Debug.WriteLine("Exception: " + e.Message);
			}
			finally { };
		}
		public static void SaveThx(string key, string sip, string path)
		{
			try
			{
				Debug.WriteLine($"{path}");
				
				string[] lines = File.ReadAllLines(path);
				lines[0] = "";
				string line = "";
                foreach (var item in lines)
                {
					line += item + "\n"; 

				}
				line = ($"{sip};{key}") + line.Remove(line.Length - 1);
				File.WriteAllText(path, line);

			}
			catch (Exception e)
			{
				Debug.WriteLine("Exception: " + e.Message);
			}
			finally { };
		}
	}
}
