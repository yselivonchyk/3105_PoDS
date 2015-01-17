﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace CalculationNode.RicartAgrawala
{
	public static class RicardAgrawalaData
	{
		internal static int CurrentValue { get; set; }

		public static Object QueueLock = new object();
		private static List<CalculationRequest> Queue { get; set; }

		public static int ExectTime { get; private set; }

		public static int RATimestamp
		{
			get
			{
				return ++ExectTime;
			}
			set
			{
				if (value > ExectTime)
					ExectTime = value;
				else
					ExectTime++;
			}
		}

		private static bool _isInterested;
		public static bool IsInterested
		{
			get { return _isInterested; }
			set
			{
				RequestTime = RATimestamp;
				_isInterested = value;
			}
		}

		public static int RequestTime;

		static RicardAgrawalaData()
		{
			Queue = new List<CalculationRequest>();
		}


		#region Request Queue

		public static void AddRequest(CalculationRequest request)
		{
			lock (QueueLock)
			{
				Queue.Add(request);
				Queue = Queue.OrderBy(x => x.Time).ThenBy(x => x.Address).ToList();

				//debug
				if (Queue.Count() <= 1) return;
				Console.WriteLine("\r\n Local time: {0}, Is interested: {1} at {2}", 
					ExectTime, IsInterested, RequestTime);
				foreach (var calculationRequest in Queue.ToList())
					Console.WriteLine(calculationRequest.Time + " " + calculationRequest.Address);
			}
		}

		public static CalculationRequest PopRequest(CalculationRequest request)
		{
			lock (QueueLock)
			{
				Queue.Remove(request);
				return request;
			}
		}

		public static int GetQueueCount()
		{
			return Queue.Count;
		}

		#endregion Request Queue
	}
}
