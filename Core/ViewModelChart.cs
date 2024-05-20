using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using System.Diagnostics;


namespace IoTControl.Core
{
	public partial class ViewModelChart : ObservableObject
	{
		private readonly Dictionary<int, List<DateTimePoint>> _values = new() { { 1, new List<DateTimePoint>()}, { 2, new List<DateTimePoint>() }, { 3, new List<DateTimePoint>() } };
		
		public static string selectedmotor = "Motor 1";
		private readonly Dictionary<int, string> motordata = new() { { 1, "m" }, { 2, "l" }, { 3, "t" } };

		private readonly DateTimeAxis _customAxis;

		public ViewModelChart()
		{
			Series = new ObservableCollection<ISeries>
		{
			new LineSeries<DateTimePoint>
			{
				Name = "Motor",
				Values = _values[1],
				Fill = null,
				GeometryFill = null,
				GeometryStroke = null
			},
			new LineSeries<DateTimePoint>
			{
				Name = "Load",
				Values = _values[2],
				Fill = null,
				GeometryFill = null,
				GeometryStroke = null
			},
			new LineSeries<DateTimePoint>
			{
				Name = "Temp",
				Values = _values[3],
				Fill = null,
				GeometryFill = null,
				GeometryStroke = null
			},
			
		};

			_customAxis = new DateTimeAxis(TimeSpan.FromSeconds(1), Formatter)
			{
				CustomSeparators = GetSeparators(),
				AnimationsSpeed = TimeSpan.FromMilliseconds(0),
				SeparatorsPaint = new SolidColorPaint(SKColors.Black.WithAlpha(100))
			};

			XAxes = new Axis[] { _customAxis };

			ReadData();
		}

		public ObservableCollection<ISeries> Series { get; set; }

		public Axis[] XAxes { get; set; }

		public object Sync { get; } = new object();

		public bool IsReading { get; set; } = true;

		public int igetit { get; set; } = 322;

		public async void ReadData()
		{
			while (IsReading)
			{
				await Task.Delay(100);
				igetit = int.Parse(App.Current.Properties["Thinglist"].ToString());
				lock (Sync)
				{
					try
					{
						var numberofmotor = selectedmotor.Split()[1];

                        for (int i = 1; i <= 3; i++)
                        {
							_values[i].Add(new DateTimePoint(DateTime.Now, int.Parse(Connections.Things[igetit].ThingMonitoring[$"{motordata[i]}{numberofmotor}"])));
							if (_values[i].Count > 250) _values[i].RemoveAt(0);
						}

						// we need to update the separators every time we add a new point 
						_customAxis.CustomSeparators = GetSeparators();
					}
					catch(Exception e) {

						for (int i = 1; i <= 3; i++)
						{
							_values[i].Add(new DateTimePoint(DateTime.Now, 0));
							if (_values[i].Count > 250) _values[i].RemoveAt(0);
						}

						// we need to update the separators every time we add a new point 
						_customAxis.CustomSeparators = GetSeparators();
						Debug.WriteLine("Oshibka ", e.Message, e.StackTrace);
					}
				}
			}
		}

		private double[] GetSeparators()
		{
			var now = DateTime.Now;

			return new double[]
			{
			now.AddSeconds(-50).Ticks,
			now.AddSeconds(-40).Ticks,
			now.AddSeconds(-30).Ticks,
			now.AddSeconds(-25).Ticks,
			now.AddSeconds(-20).Ticks,
			now.AddSeconds(-15).Ticks,
			now.AddSeconds(-10).Ticks,
			now.AddSeconds(-5).Ticks,
			now.Ticks
			};
		}

		private static string Formatter(DateTime date)
		{
			var secsAgo = (DateTime.Now - date).TotalSeconds;

			return secsAgo < 1
				? "now"
				: $"{secsAgo:N0}s ago";
		}
	}
}
