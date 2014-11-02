using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using Microsoft.Samples.XmlRpc;

namespace CalculationNode
{
	public static class PeersData
	{
		public static Dictionary<String, ChannelFactory<ICalculator>> Fellows = 
			new Dictionary<String, ChannelFactory<ICalculator>>();

		public static void Add(string address)
		{
			if (!Fellows.ContainsKey(address))
			{
				ChannelFactory<ICalculator> channelFactory = new ChannelFactory<ICalculator>(
					new WebHttpBinding(WebHttpSecurityMode.None), new EndpointAddress(address));
				channelFactory.Endpoint.Behaviors.Add(new XmlRpcEndpointBehavior());

				Fellows[address] = channelFactory;
			}
		}

		public static ICalculator GetChannel(string address)
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
