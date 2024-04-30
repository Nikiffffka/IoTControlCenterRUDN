using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace IoTControl.Core
{
    public class Command
    {
        public string Data;
		public string TypeThing;
		public Dictionary<string, string> Response;
		public IoT ThingSelf;

		public Command() { }
		public Command(byte[] result, Dictionary<string,string> Resp, IoT i) // System.Net.Sockets.UdpReceiveResult byte[] result
		{
			ThingSelf = i;
			TypeThing = i.type;
			Response = Resp;
			Data = i.hostname+ ":"+ i.port + " \n" + Encoding.UTF8.GetString(result); //Encoding.UTF8.GetString(result.Buffer); Encoding.ASCII.GetString(result, 0, result.Length);
		}
    }
}
