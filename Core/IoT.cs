using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace IoTControl.Core
{
    public class IoT
    {
        public string type;
		public string firstLetter; // первая буква в пакете управления
		public string name;
        public string hostname; 
        public int port;
		public string service;
		public string sourcetophoto;
		public Thread thread;
		public Thread threadVisualization;
        public bool visualizationIsOpen;
		public UDP UDP;
		public Dictionary<string, string> ThingMonitoring = new Dictionary<string, string>();
		public Dictionary<string, string> ThingControl = new Dictionary<string, string>();
		public Dictionary<string, string> RemoteTerminalText;
		
		public IoT(string type, string name, string hostname, int port)
        {
            this.type = type;
            this.name = name;
            this.hostname = hostname;
            this.port = port;
        }
        public IoT(string[] data)
        {
            this.type = data[0];
            this.name = data[4];
            this.hostname = data[2];
            this.port = int.Parse(data[3]);
            this.service = data[5];
			if (robotProperties.MyDictionary.ContainsKey(type))
			{
				if (type == "R2") 
				{
					this.RemoteTerminalText = new Dictionary<string, string>() { { "D0","" }, { "D1","" }, { "D2", "" }, { "D3", "" } }; //D = Display
				}
				this.ThingMonitoring = new Dictionary<string, string>(robotProperties.MyDictionary[type].monitoring); //new Dictionary<string, string>(); потому что без него начинает перезаписывать robotProperties и делает его как связующее звено (*/ω＼*) 
				this.ThingControl = new Dictionary<string, string>(robotProperties.MyDictionary[type].control);
				this.firstLetter = robotProperties.MyDictionary[type].firstLetter;
				this.sourcetophoto = robotProperties.MyDictionary[type].source;
			}
			else {
				this.ThingMonitoring = robotProperties.MyDictionary["ANOTHER"].monitoring;
				this.ThingControl = robotProperties.MyDictionary["ANOTHER"].control;
				this.firstLetter = robotProperties.MyDictionary["ANOTHER"].firstLetter;
				this.sourcetophoto = robotProperties.MyDictionary["ANOTHER"].source;
			}
		}
       
        public void Start(Thread t)
        {
            if(thread != null)
                thread.Abort();
            thread = t;
            thread.Start();
        }
		public void StartVisualization(Thread t)
		{
			if (threadVisualization != null) 
			{ 
				threadVisualization.Abort();
				visualizationIsOpen = false;
			}
			threadVisualization = t;
			visualizationIsOpen = true; //необходимо для оптимизации / прошлые варианты приводили либо к куче потоков бесконечных, либо же к нерабочим прошлым окнам \ а так кайф
			threadVisualization.Start();
		}
		public void SetupUPD() => UDP = new UDP(hostname,port);
		public Dictionary<string, string> GetRobotsData(string[] Subs)
		{
			var Data = new Dictionary<string, string>();


			Debug.WriteLine("SUBS ", Subs.ToString());
			Debug.WriteLine("SUBS[0] ", Subs[0]);
			Debug.WriteLine("SUBS[0].Length ", Subs[0].Length.ToString());
			
			for (int i = 0; i < Subs.Length - 1; i++)
			{
				var sub = Subs[i].Split(':');
				for (int j = 4; j <= 9; j++)
				{
					try
					{
						Data.Add(sub[0].ToLower() + (j - 3), sub[j]);

					}
					catch
					{
						Data.Add(sub[0].ToLower() + (j - 3), "0");

					}
				}
			}

			Data.Add("c", Subs[1].Split(':')[1]); // видимо, с последним патчем прошивки робота он перестал приходить(или никогда не приходил (_　_)。゜zｚＺ )
			Data.Add("s", Subs[1].Split(':')[2]);
			Data.Add("n", Subs[2].Split(':')[3]);

			this.ThingMonitoring = Data;
			return Data;
		}
		public Dictionary<string, string> GetTerminalData(string[] Subs)
		{
			var Data = new Dictionary<string, string>();
			Data.Add("p", Subs[0].Split(':')[2]);
			Data.Add("b1", Subs[0].Split(':')[3]);
			Data.Add("b2", Subs[0].Split(':')[4]);
			Data.Add("b3", Subs[0].Split(':')[5]);

			this.ThingMonitoring = Data;
			return Data;
		}
		public Dictionary<string, string> GetCameraData(string[] Subs)
		{
			var Data = new Dictionary<string, string>();

			string[] strokes = Subs[0].Split(':');
			for (int i = 1; i < strokes.Length; i++)
			{
				Data.Add(("l" + (i - 1)), strokes[i]);
			}
			
			this.ThingMonitoring = Data;
			return Data;
		}

	}
}
