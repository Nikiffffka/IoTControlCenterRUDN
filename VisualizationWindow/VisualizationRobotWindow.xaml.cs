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
		bool cordSwap = true;
		string textcordswap = "Cry about it";
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
							var (x, y) = RobotCoordinate();
							if (cordSwap) { 
								textcordswap = "В реальном времени";
								tb_Xcoordinate.Text = "  Робот: " + i.name + " Координата по X: " + x;
								tb_Ycoordinate.Text = "  Робот: " + i.name + " Координата по Y: " + y + "\n  " + textcordswap;
							}

							else { 
								textcordswap = "По данным управления";
								tb_Xcoordinate.Text = "  Робот: " + i.name + " Координата по X: " + double.Parse(robot.ThingControl["X"]);
								tb_Ycoordinate.Text = "  Робот: " + i.name + " Координата по Y: " + double.Parse(robot.ThingControl["Y"]) + "\n  " + textcordswap;
							}
							
							LineToRobot.X2 = 325.0 - y;
							LineToRobot.Y2 = 292.0 - x;
							Canvas.SetLeft(EllipseToRobot, 325.0 - y - 6);
							Canvas.SetTop(EllipseToRobot, 292.0 - x - 6);
							Console.WriteLine("robot " + robot.name);
						});
						Thread.Sleep(50);
					}
					catch { }
				}
			}));
		}

		private (double, double) RobotCoordinate()
		{
			var czh = (360d / 4096d);
			if (robot.type.ToLower() == "p")
			{
					if (cordSwap)
					{
						var L1 = 15;
						var L2 = 20;
						var angle_arm = (double.Parse(robot.ThingMonitoring["m2"]) * czh);
						var angle_rotate = (double.Parse(robot.ThingMonitoring["m1"]) * czh);

						var vector_size = L2 + (L1 * Math.Cos(angle_arm * (Math.PI / 180)));

						var X = Math.Sin(angle_rotate * (Math.PI / 180)) * vector_size;
						var Y = Math.Cos(angle_rotate * (Math.PI / 180)) * vector_size;


						return (-X * 20, Y * 20);
					}
					return (double.Parse(robot.ThingControl["Y"]), -double.Parse(robot.ThingControl["X"]));
			}
			else
			{
					if (cordSwap)
					{
						var L1 = 20;
						var L2 = 20;

						var M1 = ((double.Parse(robot.ThingMonitoring["m2"]) * czh)) * (Math.PI / 180);
						var M2 = ((double.Parse(robot.ThingMonitoring["m3"]) * czh) - 90) * (Math.PI / 180);

						var angle_rotate = (double.Parse(robot.ThingMonitoring["m1"]) * czh);


						var L = (Math.Cos(M1) * L1) + Math.Abs(Math.Cos(M1 + M2) * L2); //* Math.Sin(M1 + M2 + M3) 

						var X = Math.Sin(angle_rotate * (Math.PI / 180)) * L;
						var Y = Math.Cos(angle_rotate * (Math.PI / 180)) * L;

						return (-X * 10, Y * 10);
					}
					return (-double.Parse(robot.ThingControl["Y"]), double.Parse(robot.ThingControl["X"]));

				}
			
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

		private void button_CoordinateSwap_Click(object sender, RoutedEventArgs e)
		{
			if (cordSwap == false)
			{
				cordSwap = true;
			}
			else
			{
				cordSwap = false;
			}
		}
	}
}
