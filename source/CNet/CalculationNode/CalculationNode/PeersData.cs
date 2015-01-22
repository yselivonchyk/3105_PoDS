using System;
using System.Collections.Generic;
using System.Linq;
using CalculationNode.Extentions;
using CookComputing.XmlRpc;

namespace CalculationNode
{
	/// <summary>
	/// Class caries data about peers that should be shared between all threads.
	/// Also contains method for managing list of peers
	/// </summary>
	public static class PeersData
	{
		public static int LocalID;
		public static ClientBase LocalClient { get; set; }

		internal static Dictionary<String, IRicardAgrawalaProxy> Fellows =
			new Dictionary<String, IRicardAgrawalaProxy>();

		public static void Add(string address)
		{
			// put address into collection and create channelFactory for this address
			if (Fellows.ContainsKey(address)) return;

			var properUrl = NetworkExtentions.TryBuildServerUri(address);
			var proxy = XmlRpcProxyGen.Create<IRicardAgrawalaProxy>();
			proxy.Url = properUrl.ToString();
			Fellows[address] = proxy;
		}

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
			if (!Fellows.ContainsKey(address))
			{
				Console.WriteLine("Warning: {0}", address);
				foreach (var fellow in Fellows)
					Console.WriteLine(fellow);
			}
			return Fellows[address];
		}

		public static string[] GetAll()
		{
			return Fellows.Keys.ToArray();
		}

		public static IRicardAgrawalaProxy[] GetChannels(Func<String, bool> func = null)
		{
			if (func == null)
				return Fellows.Values.ToArray();

			var proper = Fellows.Keys.Where(func);
			return Fellows.Join(proper, x => x.Key, y => y, (x, y) => x.Value).ToArray();
		}

		public static bool HasAny()
		{
			return Fellows.Any();
		}

		public static void ListPeers()
		{
			Console.WriteLine("\n\rPeers data:");
			foreach (var peer in GetAll().OrderBy(x => x))
			{
				Console.WriteLine(peer);
			}
		}
	}
}
