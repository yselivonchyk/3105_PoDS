﻿using System;
using System.Diagnostics;
using System.Threading;

namespace CalculationNode
{
	public static class EventGenerator
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="client">Client interface to handle events</param>
		/// <param name="sessionLength">length of session in milliseconds</param>
		/// <param name="avgDelay">average delay between events in milliseconds</param>
		public static void Start(ClientBase client, int sessionLength, int avgDelay)
		{
			var generator = new Random(AppDomain.CurrentDomain.GetHashCode());
			var watch = Stopwatch.StartNew();
			Console.WriteLine("genarator: " + generator.GetHashCode());
			while (true)
			{
				var operation = generator.Next(3);
				var param = generator.Next(2, 10);
				var wait = Convert.ToInt32(avgDelay + 0.9 * generator.Next((-1)*avgDelay, avgDelay));
				Thread.Sleep(wait);

				// termination condition
				if (watch.ElapsedMilliseconds > sessionLength)
					return;
				ConsoleExtentions.Log("Local event: " + param + " " + (Operations)operation);
				
				switch ((Operations)operation)
				{
					case Operations.Divide:
						client.Divide(param);
						break;
					case Operations.Multuply:
						client.Multiply(param);
						break;
					case Operations.Subtract:
						client.Subtract(param);
						break;
					case Operations.Sum:
						client.Sum(param);
						break;
				}
			}
		}

		enum Operations
		{
			Sum = 0,
			Subtract = 1,
			Multuply = 2,
			Divide = 3
		}
	}
}
