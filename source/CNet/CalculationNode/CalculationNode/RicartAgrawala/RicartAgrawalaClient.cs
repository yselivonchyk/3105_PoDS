using System;
using System.Collections;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using System.Threading;
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

		private void PerformOperation(string op, int param)
		{
			var guid = Guid.NewGuid().ToString().Substring(0, 20);
			// One request at a time
			lock (outgoingRequestCS)
			{
				RicardAgrawalaData.IsInterested = true;

				var peers = PeersData.GetAll();
				Parallel.ForEach(peers,
					peer =>
					{
						var siblingProxy = PeersData.GetChannel(peer);
						var siblingTime = siblingProxy.RecieveAccess( RicardAgrawalaData.RequestTime, PeersData.ID);
						//RicardAgrawalaData.RATimestamp = siblingTime;
//						Console.WriteLine("-> 1 {2}  {0} is Ok at {1} (r:{3} c:{4})", 
//							"_", siblingTime, guid, 
//							RicardAgrawalaData.RequestTime, RicardAgrawalaData.ExectTime);
					});

				Parallel.ForEach(peers,
					peer =>
					{
//						Console.WriteLine("-> 2 {2}  {0} performes {1} (r:{3} c:{4})", "", op + "(" + param + ")", guid,
//							RicardAgrawalaData.RequestTime, RicardAgrawalaData.ExectTime);
						var siblingProxy = PeersData.GetChannel(peer);
						//siblingProxy.DoCalculation(op, param);
						//if (!siblingProxy.DoCalculation(op, param, current))
						if (!siblingProxy.DoCalculation(op, param))
						{
							Console.WriteLine("This guy messed up: " + peer);
							Thread.Sleep(60000);
						}
//						Console.WriteLine("-> 3 {2}  {0} performed {1} (r:{3} c:{4})", "", op + "(" + param + ")", guid,
//							RicardAgrawalaData.RequestTime, RicardAgrawalaData.ExectTime);
					});

				RicardAgrawalaData.IsInterested = false;
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
