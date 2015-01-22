using System;
using System.Collections;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using System.Threading.Tasks;
using CookComputing.XmlRpc;

namespace CalculationNode.RicartAgrawala
{
	/// <summary>
	/// Implements all algorithms required by RicardAgrawala routin
	/// </summary>
	public class RicartAgrawalaClient : ClientBase
	{
		private static readonly Object outgoingRequestCS = new object();

		public RicartAgrawalaClient(Uri baseServerUri)
			: base(baseServerUri)
		{
			var localServerUri = BaseServerUri;
			LocalServerAddress = localServerUri.ToString();

			var props = new Hashtable();
			props["name"] = "Server";
			props["port"] = baseServerUri.Port;
			var channel = new HttpChannel(props, null,
			  new XmlRpcServerFormatterSinkProvider()
		   );
			ChannelServices.RegisterChannel(channel, false);
			RemotingConfiguration.RegisterWellKnownServiceType(
				typeof(RicartAgrawalaServer),
				"xmlrpc",
				WellKnownObjectMode.SingleCall);
		}

		public override void Sum(int param)
		{
			PerformOperation("sum", param);
		}

		public override void Subtract(int param)
		{
			PerformOperation("sub", param);
		}

		public override void Divide(int param)
		{
			PerformOperation("div", param);
		}

		public override void Multiply(int param)
		{
			PerformOperation("mul", param);
		}

		private static void PerformOperation(string op, int param)
		{
			// One request at a time
			lock (outgoingRequestCS)
			{
				RicardAgrawalaData.IsInterested = true;

				var peers = PeersData.GetChannels();
				Parallel.ForEach(peers,
					proxy => proxy.RecieveAccess(RicardAgrawalaData.RequestTime, PeersData.LocalID));
				Parallel.ForEach(peers,
					proxy =>
					{
						if (!proxy.DoCalculation(op, param))
							ConsoleExtentions.Warning("Calculation was not performed" + proxy.Url);
					});

				RicardAgrawalaData.IsInterested = false;
			}
		}
	}
}
