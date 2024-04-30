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
	/// Логика взаимодействия для VisualizationTrafficLightWindow.xaml
	/// </summary>
	public partial class VisualizationTrafficLightWindow : Window
	{
		IoT robot;
		public VisualizationTrafficLightWindow(IoT i)
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

							if (i.ThingControl["L1"] == "1") RectangleL1.Fill = Brushes.Red; else RectangleL1.Fill = null;
							if (i.ThingControl["L4"] == "1") RectangleL2.Fill = Brushes.Yellow; else RectangleL2.Fill = null;
							if (i.ThingControl["L3"] == "1") RectangleL3.Fill = Brushes.Blue; else RectangleL3.Fill = null;
							if (i.ThingControl["L2"] == "1") RectangleL4.Fill = Brushes.Green; else RectangleL4.Fill = null; 

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
