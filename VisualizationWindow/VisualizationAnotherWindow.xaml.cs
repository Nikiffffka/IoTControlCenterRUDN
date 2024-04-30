using IoTControl.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
	/// Логика взаимодействия для VisualizationAnotherWindow.xaml
	/// </summary>
	public partial class VisualizationAnotherWindow : Window
	{
		IoT thing;
		public VisualizationAnotherWindow(IoT i)
		{
			InitializeComponent();
			thing = i;
			this.Closing += Another_ShutdownStarted;
		}
		private void Another_ShutdownStarted(object sender, EventArgs e)
		{
			thing.visualizationIsOpen = false;
		}
	}
}
