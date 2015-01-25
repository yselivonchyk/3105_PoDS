using System;
using System.Collections.Generic;
using System.Linq;

namespace CalculationNode.RicartAgrawala
{
	public static class RicardAgrawalaData
	{
		private static long _currentValue;
		private static bool _isInterested;
		private static bool _running;
		private static DateTime _lastRequestTS;
		private static Dictionary<int, int> _operations = new Dictionary<int, int>();
		private static List<CalculationRequest> Queue { get; set; }
		
		public static int RequestTime;
		public static int Calculations { get; set; }
		public static int ExactTime { get; private set; }
		public static Object QueueLock = new object();

		public static bool Running
		{
			get { return _running; }
			set
			{
				if (value)
					ResetStats();
				_running = value;
			}
		}

		internal static long CurrentValue
		{
			get { return _currentValue; }
			set
			{
				_currentValue = value;
				_lastRequestTS = DateTime.Now;
			}
		}
		
		public static bool IsInterested
		{
			get { return _isInterested; }
			set
			{
				RequestTime = IncrementClock();
				_isInterested = value;
			}
		}

		
		static RicardAgrawalaData()
		{
			Queue = new List<CalculationRequest>();
		}


		#region Lamport Clock

		public static int IncrementClock()
		{
			return ++ExactTime;
		}

		public static int UpdateClock(int candidateValue)
		{
			if (candidateValue > ExactTime)
				ExactTime = candidateValue;
			return ExactTime;
		}

		#endregion Lamport Clock



		#region Request Queue

		public static void AddRequest(CalculationRequest request)
		{
			lock (QueueLock)
			{
				Queue.Add(request);
				Queue = Queue.OrderBy(x => x.Time).ThenBy(x => x.CallerID).ToList();
			}
		}

		public static void PopRequest(CalculationRequest request)
		{
			lock (QueueLock)
			{
				Queue.Remove(request);
			}
			RegisterOperation(request.CallerID);
		}

		public static int GetQueueCount()
		{
			return Queue.Count;
		}

		public static long TimeFromRequest()
		{
			return (DateTime.Now.Ticks - _lastRequestTS.Ticks) / TimeSpan.TicksPerMillisecond;
		}

		#endregion Request Queue



		#region Statistics

		public static void ResetStats()
		{
			Calculations = 0;
			_operations = new Dictionary<int, int>();
		}

		public static void RegisterOperation(int id)
		{
			if(!_operations.ContainsKey(id))
				_operations.Add(id, 0);
			_operations[id]++;
		}

		public static void PrintStats()
		{
			foreach (var pair in _operations)
			{
				Console.WriteLine("Recieved {0} REC_REQUEST calls from LocalID:{1}", pair.Value, pair.Key);
			}
		}

		#endregion Statistics
	}
}
