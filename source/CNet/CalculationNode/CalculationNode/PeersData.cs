using System;
using System.Collections.Generic;
using System.Linq;
using CookComputing.XmlRpc;

namespace CalculationNode
{
	/// <summary>
	/// Class caries data about fellow nodes that should be shared between all threads.
	/// Also contains method for managing list of peers
	/// </summary>
	public static class PeersData
	{
		public static Dictionary<String, IRicardAgrawalaProxy> Fellows =
			new Dictionary<String, IRicardAgrawalaProxy>();

		public static int CurrentValue;

		private static ClientBase localClientBase;
		public static ClientBase LocalClient
		{
			get { return localClientBase; }
			set
			{
				if (localClientBase != null)
				{
					Console.WriteLine("we are fucked");
				}
				localClientBase = value;
			}
		}

		public static void Add(string address)
		{
			// put address into collection and create channelFactory for this address
			if (!Fellows.ContainsKey(address))
			{
				var proxy = XmlRpcProxyGen.Create<IRicardAgrawalaProxy>();
				proxy.Url = address;

				Fellows[address] = proxy;
			}
		}

		public static void Remove(string address)
		{
			if(Fellows.ContainsKey(address))
				Fellows.Remove(address);
			else
			{
				Console.WriteLine("Warning. Can not remove {0} because it is not in the list", address);
			}
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
