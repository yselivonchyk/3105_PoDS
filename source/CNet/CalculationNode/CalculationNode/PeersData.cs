using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using Microsoft.Samples.XmlRpc;

namespace CalculationNode
{
	/// <summary>
	/// Class caries data about fellow nodes that should be shared between all threads.
	/// Also contains method for managing list of peers
	/// </summary>
	public static class PeersData
	{
		public static Dictionary<String, ChannelFactory<IRicartAgrawalaServer>> Fellows = 
			new Dictionary<String, ChannelFactory<IRicartAgrawalaServer>>();

		public static void Add(string address)
		{
			// put address into collection and create channelFactory for this address
			if (!Fellows.ContainsKey(address))
			{
				var channelFactory = new ChannelFactory<IRicartAgrawalaServer>(
					new WebHttpBinding(WebHttpSecurityMode.None), new EndpointAddress(address));
				channelFactory.Endpoint.Behaviors.Add(new XmlRpcEndpointBehavior());

				Fellows[address] = channelFactory;
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


		public static IRicartAgrawalaServer GetChannel(string address)
		{
			var fabric = Fellows[address];
			return fabric.CreateChannel();
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
