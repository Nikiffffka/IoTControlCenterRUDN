using IoTControl.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace IoTControl
{
	/// <summary>
	/// Логика взаимодействия для VisualizationRemoteTerminalWindow.xaml
	/// </summary>
	public partial class VisualizationRemoteTerminalWindow : Window
	{
		IoT robot;
		public VisualizationRemoteTerminalWindow(IoT i)
		{
			InitializeComponent();
			robot = i;
			alwaysReadDataStart(i);
			Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
			this.Closing += Dispatcher_ShutdownStarted;
		}
		private void Dispatcher_ShutdownStarted(object sender, EventArgs e)
		{
			robot.threadVisualization.Abort();
			robot.visualizationIsOpen = false;
		}
		private void alwaysReadDataStart(IoT i)
		{
			i.StartVisualization(new Thread(async () =>
			{
				bool work = true;
				while (work)
				{
					try
					{
						Dispatcher.Invoke(() =>
						{
							tb_Text1.Text = i.RemoteTerminalText["D0"];
							tb_Text2.Text = i.RemoteTerminalText["D1"];
							tb_Text3.Text = i.RemoteTerminalText["D2"];
							tb_Text4.Text = i.RemoteTerminalText["D3"];

							if (i.ThingControl["L1"] == "1") lamp_L1.Fill = Brushes.Red; else lamp_L1.Fill = null;
							if (i.ThingControl["L2"] == "1") lamp_L2.Fill = Brushes.Blue; else lamp_L2.Fill = null;
							if (i.ThingControl["L3"] == "1") lamp_L3.Fill = Brushes.Green; else lamp_L3.Fill = null;
							if (i.ThingControl["L4"] == "1") lamp_L4.Fill = Brushes.Orange; else lamp_L4.Fill = null;

							if (i.ThingMonitoring["p"] == "1") lamp_DeadmanSwitch.Fill = Brushes.Red; else lamp_DeadmanSwitch.Fill = null;

							tb_button1.Text = i.ThingMonitoring["b2"];
							tb_button2.Text = i.ThingMonitoring["b3"];
							tb_button3.Text = i.ThingMonitoring["b1"];


							Console.WriteLine("robot " + robot.name);
						});
						Thread.Sleep(50);
					}
					catch { }
				}
			}));
		}
	}
}
