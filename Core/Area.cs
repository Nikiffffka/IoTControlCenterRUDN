﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTControl.Core
{
    public class Area
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public List<IoT> IoTs { get; set; }
		public string Appkey { get; set; }
		public string ServerIP { get; set; }

		public Area() {
            IoTs = new List<IoT>();
        }
    }
}
