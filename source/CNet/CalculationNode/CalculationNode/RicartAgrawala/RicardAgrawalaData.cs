using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CookComputing.XmlRpc;

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
				if (Queue.Count() > 1)
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


		#endregion Request Queue
	}
}
