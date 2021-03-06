﻿using System;

namespace CalculationNode
{
	public static class ConsoleExtentions
	{
		public static void Error(string s)
		{
			Console.BackgroundColor = ConsoleColor.Red;
			Log(s);
			Console.BackgroundColor = ConsoleColor.Black;
		}

		public static void Warning(string s)
		{
			Console.BackgroundColor = ConsoleColor.Yellow;
			Log(s);
			Console.BackgroundColor = ConsoleColor.Black;
		}

		public static void Log(string message)
		{
			var log = String.Format("{0}: {1}", DateTime.Now.ToString("HH:mm:ss.fff"), message);
			Console.WriteLine(log);
		}

		public static void Regresh()
		{
			Console.Clear();
			Console.WriteLine("\nPress key to indicate desired operation:");
			Console.WriteLine("Enter - start command");
			Console.WriteLine("1 - Join command");
			Console.WriteLine("2 - Sign off command");
			Console.WriteLine("3 - Start");
			Console.WriteLine("8 - Clean up condole");
			Console.WriteLine("9 - List peers");
			Console.WriteLine("Esc - exit");
		}
	}
}
