﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Windows.Markup;
using System.Net;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;

namespace IoTControl.Core
{
	public class UDP
    {
		UdpClient udpClient;
		IPEndPoint groupEP;
		public static bool active;

		public UDP(string hostname, int port)
        {
			udpClient = new UdpClient(hostname, port);
			groupEP = new IPEndPoint(IPAddress.Parse(hostname), 8888);
		}
		public void Reconect(string hostname, int port)
        {
            udpClient.Close();
            udpClient.Connect(hostname, port);
        }
        public void Close() => udpClient.Close();
		public async Task<Command> ReceiveCommandAsync(IoT iInThread)
		{
			try
			{
				if (!active)
				{
					active = true;
					UdpClient listener = new UdpClient(iInThread.port); // SocketException тут   КРЧ есть вариант сделать переменную которая будет хранить состояние(подключено ли сейчас или же занят ли порт сейчас) и делать if на это      PS. не помогает))))))
					var response = await listener.ReceiveAsync();
					listener.Close();
					Debug.WriteLine(response.RemoteEndPoint.Address.ToString());
					active = false;
					foreach (IoT i in Connections.Things)
					{
						Console.WriteLine("\nЧЗХ Я НЕ ПОНИМАЮ1  " + i.hostname + "   " + response.RemoteEndPoint.Address.ToString() + "\n");
						if (i.hostname == response.RemoteEndPoint.Address.ToString())
						{
							Console.WriteLine("\nЧЗХ Я НЕ ПОНИМАЮ2  "+ i.hostname+"   " + response.RemoteEndPoint.Address.ToString()+ "\n");
							Debug.WriteLine(response.Buffer);
							var temp = Thingworx.Connect(response, i);
							return (new Command(response.Buffer, temp.Result, i));
						};
					};  //Может быть поменять на что-то подобное     Главное чтобы это было оптимизировано 
					/*if (response.RemoteEndPoint.Address.ToString() == iInThread.hostname)
					{
						Debug.WriteLine(response.Buffer);
						var temp = Thingworx.Connect(response, iInThread);
						return (new Command(response.Buffer, groupEP, temp.Result, iInThread));
					}*/
					return null;
				}
				else { 
					return null;
				}
			}
			catch(SocketException e) {
				active = false;
				Console.WriteLine(e.Message +"\n"+ e.StackTrace);
				return null;
			}

		}
        public async Task<bool> SendCommandAsync(string command)
        {
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(command);
                await udpClient.SendAsync(data, data.Length); //clientIP , groupEP
				return true;
            }
            catch (Exception e)
            {
				Console.WriteLine(e.Message + "\n" + e.StackTrace);
                return false;
            }
        }
    }
}
