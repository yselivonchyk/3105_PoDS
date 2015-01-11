using System;
using System.Collections.Generic;
using System.Linq;
using CalculationNode.RicartAgrawala;
using CookComputing.XmlRpc;

namespace CalculationNode
{
	/// <summary>
	/// Class caries data about fellow nodes that should be shared between all threads.
	/// Also contains method for managing list of peers
	/// </summary>
	public static class PeersData
	{
		public static ClientBase LocalClient { get; set; }
		public static Object QueueLock = new object();

		internal static Dictionary<String, IRicardAgrawalaProxy> Fellows =
			new Dictionary<String, IRicardAgrawalaProxy>();

		private static int currentValue;
		internal static int CurrentValue
		{
			get
			{
				//Console.WriteLine("Get Current: {0} \t {1}", AppDomain.CurrentDomain.GetHashCode(), currentValue);
				return currentValue;
			}
			set
			{
				//Console.WriteLine("Set Current: {0} \t {1} <- {2}", AppDomain.CurrentDomain.GetHashCode(), currentValue, value);
				currentValue = value;	
			}
		}

		internal static bool AwaitCalculationFlag;

		private static List<CalculationRequest> Queue { get; set; }
		private static int currentTime;

		public static int CurrentTime
		{
			get
			{
				currentTime++;
				return currentTime++;
			}
			set
			{
				if (value > currentTime)
					currentTime = value;
			}
		}

		static PeersData()
		{
			Queue = new List<CalculationRequest>();
		}

		public static void Add(string address)
		{
			// put address into collection and create channelFactory for this address
			if (Fellows.ContainsKey(address)) return;

			var proxy = XmlRpcProxyGen.Create<IRicardAgrawalaProxy>();
			proxy.Url = address;
			Fellows[address] = proxy;
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

		public static CalculationRequest PopRequest()
		{
			lock (QueueLock)
			{
				if(!AwaitCalculationFlag)
					Console.WriteLine("THIS IS NOT HAPPENING");
				var request = Queue.First();
				Queue.Remove(request);
				//ConsoleExtentions.Log("Reqest removed from queue: " + request.Address);
				AwaitCalculationFlag = false;
				return request;
			}
		}

		public static CalculationRequest NexRequest()
		{
			lock (QueueLock)
			{
				return Queue.First();
			}
		}

		#endregion Request Queue


		public static void Remove(string address)
		{
			if(Fellows.ContainsKey(address))
				Fellows.Remove(address);
			else
				Console.WriteLine("Warning. Can not remove {0} because it is not in the list", address);
		}

		public static void Empty()
		{
			Fellows.Clear();
		}


		public static IRicardAgrawalaProxy GetChannel(string address)
		{
			var fabric = Fellows[address];
			return fabric;
		}

		public static string[] GetAll()
		{
			return Fellows.Keys.ToArray();
		}

		public static bool HasAny()
		{
			return Fellows.Any();
		}
	}
}
