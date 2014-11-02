using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using Microsoft.Samples.XmlRpc;

namespace CalculationNode
{
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

		public static IRicartAgrawalaServer GetChannel(string address)
		{
			var fabric = Fellows[address];
			return fabric.CreateChannel();
		}

		public static string[] GetAll()
		{
			return Fellows.Keys.ToArray();
		}
	}
}
