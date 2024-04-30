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
	/// Логика взаимодействия для VisualizationRobotWindow.xaml
	/// </summary>
	public partial class VisualizationRobotWindow : Window 
	{
		IoT robot;
		public VisualizationRobotWindow(IoT i)
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
							tb_Xcoordinate.Text = "  Робот: " + i.name + " Координата по X: " + i.ThingControl["X"]; // + "test m1:" + i.ThingMonitoring["m1"]
							tb_Ycoordinate.Text = "  Робот: " + i.name + " Координата по Y: " + i.ThingControl["Y"]; // желательно сделать обратную кинематику
							LineToRobot.X2 = 325.0 - Convert.ToDouble(i.ThingControl["Y"]);
							LineToRobot.Y2 = 292.0 - Convert.ToDouble(i.ThingControl["X"]);
							Canvas.SetLeft(EllipseToRobot, 325.0 - Convert.ToDouble(i.ThingControl["Y"])-6);
							Canvas.SetTop(EllipseToRobot, 292.0 - Convert.ToDouble(i.ThingControl["X"])-6);
							Console.WriteLine("robot "+robot.name);
						});
						Thread.Sleep(50);
					}
					catch { }
				}
			}));
		}

		private void button_RemoveLine_Click(object sender, RoutedEventArgs e)
		{
			if (L1.Visibility == Visibility.Visible)
			{
				L1.Visibility = Visibility.Hidden;
				L2.Visibility = Visibility.Hidden;
				L3.Visibility = Visibility.Hidden;
				L4.Visibility = Visibility.Hidden;
			}
			else
			{
				L1.Visibility = Visibility.Visible;
				L2.Visibility = Visibility.Visible;
				L3.Visibility = Visibility.Visible;
				L4.Visibility = Visibility.Visible;
			}
		}
	}
}
