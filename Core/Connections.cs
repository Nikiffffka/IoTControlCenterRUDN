using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Net;
using System.Security.RightsManagement;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace IoTControl.Core
{
    public static class Connections
    {
        public static EventHandler<Command> MonCommand;
		public static EventHandler<Command> LogCommand;
        public static List<IoT> Things = new List<IoT>();
        public static List<Command> CommandList = new List<Command>();

		public static void Preload()
        {

        }
		public static void Start()
		{
			foreach (IoT i in Things)
			{
				i.SetupUPD();
				i.Start(new Thread(async () =>
				{
					bool work = true;
					await i.UDP.SendCommandAsync("r");
					while (work) // mb CheckIinThing(i)
					{
						try
						{
							if (CheckIinThings(i)){
								Console.WriteLine("Things in thread " + i.name);
								Command cmd = new Command();
								cmd = i.UDP.ReceiveCommandAsync(i).Result;
								if (cmd != null) { AddCommandToMon(i, cmd); AddCommandToLog(i, cmd); }
							}
						}
						catch (Exception e)
						{
							Debug.WriteLine(e.Message + " connections catch\n" + e.StackTrace);
						}
						Thread.Sleep(1000); //FIXME Нужно что-то с этим делать, беспощадно лагать может      а может и нет ╰(*°▽°*)╯
					}
				}));
			}
		}
		public static async void Close()
        {
            foreach (IoT i in Things)
            {
				Console.WriteLine(i.name.ToString());
				await i.UDP.SendCommandAsync("s");
				Console.WriteLine("SENDED!!!");
                i.thread.Abort(); // изменил местами возможно он не мог отправить s и сдыхал PS Вернул назад тк не помогло
				Console.WriteLine("ABORTED!!!");
				i.UDP.Close();
				Console.WriteLine("UDPCLOSED!!!");
			}
		}
        internal static void AddCommandToMon(IoT ioT, Command cmd)
        {
			CommandList.Add(cmd);
			MonCommand.Invoke(ioT, cmd);
        }
		internal static void AddCommandToLog(IoT ioT, Command cmd)
		{
			CommandList.Add(cmd);
			LogCommand.Invoke(ioT, cmd);
		}
		public static async void SendForAllThings(string letter)
		{
            foreach (IoT i in Things)
            {
                await i.UDP.SendCommandAsync(letter);
            }
		}
		public static bool CheckIinThings(IoT i)
		{
			if (Things.Contains(i))
				return true;
			else
				return false;
		}
	}
}

