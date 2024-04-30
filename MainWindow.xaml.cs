using IoTControl.Core;
using IoTControl.UI;
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

		List<Team> Teams = new List<Team>();
		List<InputControlWlegend> InputControl = new List<InputControlWlegend>();
		public static DataForThingworx DFThx = new DataForThingworx();
		public MainWindow()
        {
			InitializeComponent();
			robotProperties.InitProperty();

			Connections.MonCommand += NowNewCommand;
			Connections.LogCommand += NowNewCommandToLog;

			Teams = TeamLoadManager.LoadTeams();

			Connections.DFThx = DFThx;

			List<string> ListForTeams = new List<string>(); 
            foreach (Team team in Teams)
            {
				TeamsList.Items.Add(team.Name);
			}

			tb_appKey.Text = DataForThingworx.AppKey;
			tb_serverIP.Text = DataForThingworx.ServerIP;

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
			if (ThingsList.SelectedItem != null) AddNewCommandToLog($"Выбрана команда: {TeamsList.SelectedItem}");

			if (Connections.Things != null)	Connections.Close();

			Connections.Things = Teams[TeamsList.SelectedIndex].IoTs;
			DataForThingworx.ServerIP = Teams[TeamsList.SelectedIndex].ServerIP;
			DataForThingworx.AppKey = Teams[TeamsList.SelectedIndex].Appkey;
			tb_serverIP.Text = DataForThingworx.ServerIP;
			tb_appKey.Text = DataForThingworx.AppKey;

			Connections.Start();

			ListBarcode.Clear(); BarcodeList.Items.Clear(); ThingsList.Items.Clear(); 

			foreach (IoT t in Teams[TeamsList.SelectedIndex].IoTs)				   
			{																	   
				Console.WriteLine(t.name.ToString());							   
				if (t.type == "B")												   
				{
					ListBarcode.Add(t);
					BarcodeList.Items.Add(t.name);
				}
				ThingsList.Items.Add(t.name);

			}
			ThingsList.SelectedIndex = 0; ThingsList.SelectedItem = 0;
			BarcodeList.SelectedIndex = 0; BarcodeList.SelectedItem = 0;
			

		}
		public void NowNewCommand(object s,Command cmd)
        {
            Dispatcher.Invoke(() =>
            {
				GetReceiveFromThingworx(cmd.Response, cmd.ThingSelf);
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

				string textToMonitor = (cmd.Data != null ? cmd.ThingSelf.name + " " + cmd.Data + "\n" : "");
				Debug.WriteLine(tb_log.Text.Length);

				if (tb_Monitoring.Text.Length > 1000)
					tb_Monitoring.Text = tb_Monitoring.Text.Substring(tb_Monitoring.Text.Length - 1000, 1000);

				tb_Monitoring.Text += textToMonitor;

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

				if (tb_log.Text.Length > 1000)
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
			if (ThingsList.SelectedItem != null) AddNewCommandToLog($"Выбрана вещь: {ThingsList.SelectedItem}");
			if (ThingsList.SelectedIndex != -1)
				ChangeKeyValue(Connections.Things[ThingsList.SelectedIndex]);
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
		private void GetReceiveFromThingworx(Dictionary<string,string> Responce, IoT ThingSelf) 
		{
			if (ToggleReceiveFromThingworx.IsChecked == true)
			{
				foreach (var item in ThingSelf.ThingControl.Keys.ToList())
				{
					if (Responce.ContainsKey(item))
					{
						ThingSelf.ThingControl[item] = Responce[item];
						if (Connections.Things[ThingsList.SelectedIndex] == ThingSelf)
							ChangeKeyValue(ThingSelf);
					}
				}
				SendDataToRobot(ThingSelf.ThingControl.Count, ThingSelf);

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

		private void SaveDataForConnectToThingworx(object sender, RoutedEventArgs e)
		{
			AddNewCommandToLog($"Изменены значения для подключения к thingworx: {(tb_appKey.Text, tb_serverIP.Text, Teams[TeamsList.SelectedIndex].Path)}");
			Teams[TeamsList.SelectedIndex].ServerIP = tb_serverIP.Text;
			Teams[TeamsList.SelectedIndex].Appkey = tb_appKey.Text;
			DataForThingworx.ServerIP = tb_serverIP.Text;
			DataForThingworx.AppKey = tb_appKey.Text;
			DataForThingworx.SaveThx(tb_appKey.Text, tb_serverIP.Text, Teams[TeamsList.SelectedIndex].Path);
		}

		private async void SendNumberForThing(object sender, RoutedEventArgs e)
		{
			AddNewCommandToLog($"SendNumberForSelectedThing: {((Button)sender).Tag.ToString()}");
			await Connections.Things[ThingsList.SelectedIndex].UDP.SendCommandAsync(((Button)sender).Tag.ToString());
		}
		private void SendCodeToThingworx(object sender, RoutedEventArgs e)
		{
			if (BarcodeList.SelectedIndex != -1)
			{
				_ = Thingworx.SendToThingworx(ListBarcode[(BarcodeList.SelectedIndex)], new Dictionary<string, string> { { "c", textBox_Barcode.Text } });
				ListBarcode[(BarcodeList.SelectedIndex)].ThingMonitoring["c"] = textBox_Barcode.Text;
				AddNewCommandToLog($"Код был отправлен: {textBox_Barcode.Text} на {ListBarcode[(BarcodeList.SelectedIndex)].name}");

			}

		}


		private void Monitoring_ScrollChanged(object sender, ScrollChangedEventArgs e)
		{
			if(cb_Monitoring.IsChecked ?? false)
				scroll_Monitoring.ScrollToBottom();
		}

		private void Log_ScrollChanged(object sender, ScrollChangedEventArgs e)
		{
			if (cb_log.IsChecked ?? false)
				scroll_log.ScrollToBottom();
		}

		private void ToggleReceiveToThingworx_Click(object sender, RoutedEventArgs e)
		{
			if (ToggleReceiveToThingworx.IsChecked ?? false)
				AddNewCommandToLog("Данные будут отправляться на Thingworx");
			else
				AddNewCommandToLog("Данные не будут отправляться на Thingworx");

			Debug.WriteLine(ToggleReceiveToThingworx.IsChecked);
			Thingworx.SendToThx = ToggleReceiveToThingworx.IsChecked ?? false;
		}

		private void ToggleReceiveFromThingworx_Click(object sender, RoutedEventArgs e)
		{
			if (ToggleReceiveFromThingworx.IsChecked ?? false)
				AddNewCommandToLog("Данные с Thingworx будут приходить");
			else
				AddNewCommandToLog("Данные с Thingworx не будут приходить");

			Debug.WriteLine(ToggleReceiveFromThingworx.IsChecked);
			Thingworx.ReceiveFromThx = ToggleReceiveFromThingworx.IsChecked ?? false;

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
    }
}
