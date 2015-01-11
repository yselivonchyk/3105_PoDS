using System;
using System.Collections;
using System.Linq;
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
		public bool IsInterested = false;
		private static Object outgoingRequestCS = new object();

		public RicartAgrawalaClient(Uri baseServerUri)
			: base(baseServerUri)
		{
			var localServerUri = BaseServerUri;
			LocalServerAddress = localServerUri.ToString();

			var props = new Hashtable();
			props["name"] = "Server";
			props["port"] = baseServerUri.Port;
			var channel = new HttpChannel(
			  props,
			  null,
			  new XmlRpcServerFormatterSinkProvider()
		   );
			ChannelServices.RegisterChannel(channel, false);
			RemotingConfiguration.RegisterWellKnownServiceType(
				typeof(RicartAgrawalaServer),
				"xmlrpc",
				WellKnownObjectMode.Singleton);
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

		private void PerformOperation(string op, int param)
		{
			// One request at a time
			lock (outgoingRequestCS)
			{
				IsInterested = true;

				var peers = PeersData.GetAll();
				var time = RicardAgrawalaData.RATimestamp;
				Parallel.ForEach(peers,
					peer =>
					{
						var siblingProxy = PeersData.GetChannel(peer);
						var siblingTime = siblingProxy.RecieveAccess(LocalServerAddress, time);
						RicardAgrawalaData.RATimestamp = siblingTime;
						Console.WriteLine("{0} is Ok at {1}", peer, siblingTime);
					});

				Parallel.ForEach(peers,
					peer =>
					{
						var siblingProxy = PeersData.GetChannel(peer);
						siblingProxy.DoCalculation(op, param);
					});

				IsInterested = false;
			}
		}

		public void ListPeers()
		{
			Console.WriteLine("\n\rPeers data:");
			foreach (var peer in PeersData.GetAll().OrderBy(x => x))
			{
				Console.WriteLine(peer);
			}
		}
	}
}
