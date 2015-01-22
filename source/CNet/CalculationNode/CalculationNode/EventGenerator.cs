using System;
using System.Diagnostics;
using System.Threading;
using CalculationNode.RicartAgrawala;

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
			var generator = new Random(AppDomain.CurrentDomain.GetHashCode() + PeersData.LocalID);
			var watch = Stopwatch.StartNew();
			Console.WriteLine("Genarator seed: " + (generator.GetHashCode() + PeersData.LocalID));
			var localEvents = 0;
			while (true)
			{
				var operation = generator.Next(3);
				var param = generator.Next(2, 10);
				var wait = Convert.ToInt32(avgDelay + 0.9 * generator.Next((-1)*avgDelay, avgDelay));
				Thread.Sleep(wait);

				// termination condition
				if (watch.ElapsedMilliseconds > sessionLength)
				{
					Console.WriteLine("\r\nGenerated {0} events localy", localEvents);
					return;
				}
				//ConsoleExtentions.Log("Local event: " + param + " " + (Operations)operation + " t:" + RicardAgrawalaData.ExactTime);
				localEvents++;
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
