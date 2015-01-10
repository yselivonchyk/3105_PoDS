﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculationNode
{
	public static class ConsoleExtentions
	{
		public static void Log(string message)
		{
			var log = String.Format("{0}: {1}", DateTime.Now.ToLongTimeString(), message);
			Console.WriteLine(log);
		}
	}
}