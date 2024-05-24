using IoTControl.Core;
using IoTControl.UI;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Packaging;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace IoTControl
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

		List<IoT> ListBarcode = new List<IoT>();

		List<Area> Teams = new List<Area>();
		List<InputControlWlegend> InputControl = new List<InputControlWlegend>();


		public MainWindow()
        {
			InitializeComponent();
			App.Current.Properties["Thinglist"] = ThingsList.SelectedIndex;

			robotProperties.InitProperty();

			Connections.MonCommand += NowNewCommand;
			Connections.LogCommand += NowNewCommandToLog;

			Teams = AreaLoadManager.LoadAreas();

			List<string> ListForTeams = new List<string>(); 
            foreach (Area team in Teams)
            {
				TeamsList.Items.Add(team.Name);
			}

            for (int i = 1; i <= 6; i++)
            {
				cb_paramMon.Items.Add($"Motor {i}");
			}

			Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
			this.Closing += Dispatcher_ShutdownStarted; // ради интереса написал (просто после закрытия окна программа не всегда заканчивает работу) 

		}


		private void Dispatcher_ShutdownStarted(object sender, EventArgs e)
        {
            Connections.Close();
            Connections.MonCommand -= NowNewCommand;
			Connections.LogCommand -= NowNewCommandToLog;

		}
		private void TeamsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (ThingsList.SelectedItem != null) AddNewCommandToLog($"Выбрана площадка: {TeamsList.SelectedItem}");

			if (Connections.Things != null)	Connections.Close();

			Connections.Things = Teams[TeamsList.SelectedIndex].IoTs;

			Connections.Start();

			ListBarcode.Clear(); ThingsList.Items.Clear(); 

			foreach (IoT t in Teams[TeamsList.SelectedIndex].IoTs)				   
			{																	   
				Console.WriteLine(t.name.ToString());							   
				if (t.type == "B")												   
				{
					ListBarcode.Add(t);
				}
				ThingsList.Items.Add(t.name);

			}
			ThingsList.SelectedIndex = 0; ThingsList.SelectedItem = 0;
			

		}
		public void NowNewCommand(object s,Command cmd)
        {
				Dispatcher.Invoke(() =>
					{
						//chartik.ReadData();
						tb_monitoring.Text = Connections.Things[ThingsList.SelectedIndex].name;

					});
		}

		public void NowNewCommandToLog(object s, Command cmd)
		{
			Dispatcher.Invoke(() =>
			{
				var valzxc = "";
				if (cmd.Data != null && cmd.Data.Length < 4) cmd.Data = null;

				 
				if (cmd.Response == null) { }
				else
				{
					foreach (var value in cmd.Response)
					{
						valzxc += value.ToString();
					}
					valzxc = "\n" + valzxc;
				}

				string textToLog = (cmd.ThingSelf.name + " " + cmd.Data + valzxc + "\n");
				Debug.WriteLine(tb_log.Text.Length);

				if (tb_log.Text.Length > 4000)
					tb_log.Text = tb_log.Text.Substring(tb_log.Text.Length-1000, 1000); 
			
				tb_log.Text += textToLog;
				
			});

		}
		public void AddNewCommandToLog(string text)
		{
			tb_log.Text += text + "\n";
		}
		public void SendDataToRobot(object sender, RoutedEventArgs e)
		{
			for (int i = 0; i < InputControl.Count; i++)
			{
				Connections.Things[ThingsList.SelectedIndex].ThingControl[InputControl[i].getLegend()] = InputControl[i].Value;
				Console.WriteLine(robotProperties.MyDictionary["P"].control);
			}
			SendData(InputControl.Count, Connections.Things[ThingsList.SelectedIndex]);
		}
		public void SendDataToRobot(int Param, IoT ThingSelf)
		{
			SendData(Param,ThingSelf);
		}
		private void SendData(int Param, IoT ThingSelf)
		{
			if (ThingSelf.type == "R2") ThingSelf.RemoteTerminalText["D" + InputControl[4].Value] = InputControl[5].Value;

			if (ThingSelf.type == "T") 
			{
				var temp = ThingSelf.ThingControl["L2"];
				ThingSelf.ThingControl["L2"] = ThingSelf.ThingControl["L4"];
				ThingSelf.ThingControl["L4"] = temp;
			}

			string Cmd_package = ThingSelf.firstLetter;
			for (int i = 0; i < Param; i++)
			{
				Cmd_package += ":" + ThingSelf.ThingControl.ElementAt(i).Value;
			}

			Cmd_package += "#";

			Debug.WriteLine(ThingsList.SelectedIndex);
			AddNewCommandToLog($"SEND: {Cmd_package} \nFOR: {ThingSelf.name}");
			_ = ThingSelf.UDP.SendCommandAsync(Cmd_package);
		}
		

		private void ThingsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			App.Current.Properties["Thinglist"] = ThingsList.SelectedIndex;
			
			if (ThingsList.SelectedItem != null) AddNewCommandToLog($"Выбрана вещь: {ThingsList.SelectedItem}");
			if (ThingsList.SelectedIndex != -1) { 
				ChangeKeyValue(Connections.Things[ThingsList.SelectedIndex]);
				if (Connections.Things[ThingsList.SelectedIndex].firstLetter == "g" || Connections.Things[ThingsList.SelectedIndex].firstLetter == "p")
				{
					tb_insteadChart.Text = "";
					chart_Monitoring.Visibility = Visibility.Visible;
				}
				else
				{
					tb_insteadChart.Text = "Нет мониторинговых данных\nИли\nМониторинг представлен в визуализации";
					chart_Monitoring.Visibility = Visibility.Hidden;
				}
			}
			else 
				ChangeKeyValue(Connections.Things[0]);
			
		}
		public void ChangeKeyValue(IoT things)
		{
			Container_parameters.Children.Clear(); InputControl.Clear();

			foreach (var th in Connections.Things)
			{
				(FindName(th.sourcetophoto) as Image).Visibility = Visibility.Hidden;
			}
			try { 
				var obj = FindName(Connections.Things[ThingsList.SelectedIndex].sourcetophoto);
				(obj as Image).Visibility = Visibility.Visible;
			} catch { }

			foreach (string s in things.ThingControl.Keys)
			{
				InputControlWlegend temp = new InputControlWlegend(s, things.ThingControl[s].ToString());
				InputControl.Add(temp);
				Container_parameters.Children.Add(temp);
			}


		}
		public string GetTextValue(TextBox txtboxName)
		{
			return txtboxName.Text;
		}

		private void SendForAll(object sender, RoutedEventArgs e)
		{
			AddNewCommandToLog($"SendForAll: {((Button)sender).Tag.ToString()}");
			Connections.SendForAllThings(((Button)sender).Tag.ToString());
		}


		private async void SendNumberForThing(object sender, RoutedEventArgs e)
		{
			AddNewCommandToLog($"SendNumberForSelectedThing: {((Button)sender).Tag.ToString()}");
			await Connections.Things[ThingsList.SelectedIndex].UDP.SendCommandAsync(((Button)sender).Tag.ToString());
		}


		private void Log_ScrollChanged(object sender, ScrollChangedEventArgs e)
		{
			if (cb_log.IsChecked ?? false)
				scroll_log.ScrollToBottom();
		}

		
		private void button_Visualization_Click(object sender, RoutedEventArgs e)
		{
			if (Connections.Things[ThingsList.SelectedIndex].visualizationIsOpen == false) 
			{
				Window visualizationWindow;
				if (Connections.Things[ThingsList.SelectedIndex].type == "M" || Connections.Things[ThingsList.SelectedIndex].type == "P")
				{
					visualizationWindow = new VisualizationRobotWindow(Connections.Things[ThingsList.SelectedIndex]);
				} //может надо было сделать чтобы объект вещи это окно хранил? 
				else if (Connections.Things[ThingsList.SelectedIndex].type == "T") 
				{ 
					visualizationWindow = new VisualizationTrafficLightWindow(Connections.Things[ThingsList.SelectedIndex]); 
				}
				else if (Connections.Things[ThingsList.SelectedIndex].type == "B")
				{
					visualizationWindow = new VisualizationBarcodeReaderWindow(Connections.Things[ThingsList.SelectedIndex]); 
				}
				else if (Connections.Things[ThingsList.SelectedIndex].type == "R2")
				{
					visualizationWindow = new VisualizationRemoteTerminalWindow(Connections.Things[ThingsList.SelectedIndex]);
				}
				else if (Connections.Things[ThingsList.SelectedIndex].type == "C")
				{
					visualizationWindow = new VisualizationSmartCameraWindow(Connections.Things[ThingsList.SelectedIndex]);
				}
				else { visualizationWindow = new VisualizationAnotherWindow(Connections.Things[ThingsList.SelectedIndex]); }  
				visualizationWindow.Title = ThingsList.SelectedItem.ToString();
				visualizationWindow.Owner = this; //  mainWindow.Closing += MainWindow_Closing;             or             mainWindow.Closed += (s, e) => Close();
				Connections.Things[ThingsList.SelectedIndex].visualizationIsOpen = true;
				visualizationWindow.Show();
			}
		}

		private void cb_paramMon_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{

			ViewModelChart.selectedmotor = cb_paramMon.SelectedValue.ToString();
		}
	}
}
