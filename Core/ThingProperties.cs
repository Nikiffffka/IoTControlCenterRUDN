using IoTControl.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Documents;

namespace IoTControl.Core
{
	public class NestedValues{
		public Dictionary<string, string> control {	get; set;}
		public Dictionary<string, string> monitoring { get; set;	}
		public string firstLetter { get; set; }

		public string source { get; set; }

		public NestedValues(IEnumerable<string> c,IEnumerable<string> m, IEnumerable<string> a,IEnumerable<string> b,string fl,string sourse)
		{
			this.firstLetter = fl;
			this.source = sourse;
			IEnumerable<string> temp = m.Select((value) => value);
			IEnumerable<string> stemp = c.Select((value) => value);
			var templist = new List<string>(temp);
			var stemplist = new List<string>(stemp);
			if (a != null) foreach (var item in a) templist.Add(item);
			if (b != null) foreach (var item in b) stemplist.Add(item);
			Console.WriteLine(templist);
			Console.WriteLine(stemplist);
			this.control = stemplist.ToDictionary(x => x, x => "0");
			this.monitoring = templist.ToDictionary(x => x, x => "0");
		}
	}
	public static class robotProperties { 
		public static Dictionary<string, NestedValues> MyDictionary { get; set;}

		public static void InitProperty() {
			MyDictionary = new Dictionary<string, NestedValues>();
			var temp = Enumerable.Range(1, 6).Select(i => $"m{i}").Concat(Enumerable.Range(1, 6).Select(i => $"l{i}")).Concat(Enumerable.Range(1, 6).Select(i => $"t{i}"));

			MyDictionary.Add("P",
				new NestedValues(new string[] { "X", "Y", "Z", "V" },
				temp,
				new string[] { "s", "c", "n" },
				null,
				"p",
				"PalletazerImageThing"));
			MyDictionary.Add("M",
				new NestedValues(new string[] { "X", "Y", "T", "Z", "V" },
				temp,
				new string[] { "s", "c", "n" },
				null,
				"g",
				"ManipulatorImageThing"));
			MyDictionary.Add("R2",
				new NestedValues(Enumerable.Range(1, 4).Select(i => $"L{i}"),
				Enumerable.Range(1, 3).Select(i => $"b{i}"),
				new string[] { "p" },
				new string[] { "D1", "DT" },
				"r",
				"RemoteTerminalImageThing"));
			MyDictionary.Add("C",
				new NestedValues(new string[] { "G" },
				Enumerable.Range(1, 6).Select(i => $"l{i}"),
				null,
				null,
				"c",
				"SmartCameraImageThing"));
			MyDictionary.Add("T",
				new NestedValues(new string[] { "L1", "L2", "L3", "L4" },
				new string[] { },
				null,
				null,
				"l",
				"TrafficLightImageThing"));
			MyDictionary.Add("B",
				new NestedValues(new string[] { },
				new string[] { "c" },
				null,
				null,
				"the barcode does not exist in reality",
				"BarcodeReaderImageThing"));
			MyDictionary.Add("ANOTHER",
				new NestedValues(new string[] { },
				new string[] { },
				null,
				null,
				"zxc What Are Doing Man? This Thing Is Not Exist zxc",
				"AnotherImageThing"));

		}

	}



}
