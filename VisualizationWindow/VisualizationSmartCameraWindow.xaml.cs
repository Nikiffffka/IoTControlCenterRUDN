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
	/// Логика взаимодействия для VisualizationSmartCameraWindow.xaml
	/// </summary>
	public partial class VisualizationSmartCameraWindow : Window
	{
		IoT robot;
		public VisualizationSmartCameraWindow(IoT i)
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
