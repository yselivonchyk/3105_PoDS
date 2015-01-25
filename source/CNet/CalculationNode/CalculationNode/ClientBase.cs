using System;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using CalculationNode.Extentions;
using CalculationNode.RicartAgrawala;

namespace CalculationNode
{
	public abstract class ClientBase : IClient, IDisposable
	{
		public const int SesstionLength = 20000;
		public const int EventDelayAvg = 900;

		protected Uri BaseServerUri;
		protected ServiceHost HostObject;
		internal string LocalServerAddress;


		protected ClientBase(Uri baseServerUri)
		{
			BaseServerUri = baseServerUri;
			PeersData.LocalClient = this;
		}

		public void Join(Uri knownNodeUri)
		{
			// Add self 
			if (!LocalServerAddress.Equals(knownNodeUri.ToString()))
				JoinSingle(new Uri(LocalServerAddress));
			// save information about known node
			var peers = JoinSingle(knownNodeUri).Select(NetworkExtentions.TryBuildServerUri);
			var undiscoveredNodesAddresses = peers
				.Where(x => knownNodeUri != x && LocalServerAddress != x.ToString());
			// Make parallel non-blocking call to all other nodes
			Parallel.ForEach(
				undiscoveredNodesAddresses,
				siblingAddress => JoinSingle(siblingAddress));
		}

		// Get a connetion and call remote Join operation on a single node
		private string[] JoinSingle(Uri nodeAddress)
		{
			PeersData.Add(nodeAddress.ToString());
			var siblingProxy = PeersData.GetChannel(nodeAddress.ToString());

			var joinResponse = siblingProxy.Join(LocalServerAddress);
			ConsoleExtentions.Log(
				String.Format("Got response with {0} items from {1}",
				joinResponse.Length,
				nodeAddress));
			return joinResponse.Select(x => x.ToString()).ToArray();
		}

		public void SingOff()
		{
			Parallel.ForEach(PeersData.GetChannels(p => p != LocalServerAddress),
				proxy => proxy.SignOff(LocalServerAddress));
			PeersData.Empty();
			// Join self back again
			JoinSingle(new Uri(LocalServerAddress));
		}

		public void Start(int seed)
		{
			if (RicardAgrawalaData.Running)
				return;

			Parallel.ForEach(PeersData.GetChannels(),
				proxy =>
				{
					proxy.Start(seed);
					Console.WriteLine("Finished start request with seed '{0}' for peer '{1}'", seed, proxy.Url);
				});
			Console.WriteLine("\r\n\r\n");
		}

		internal void StartSelf(int seed)
		{
			if (RicardAgrawalaData.Running)
				return;

			RicardAgrawalaData.Running = true;
			EventGenerator.Start(this, SesstionLength, EventDelayAvg);

			Parallel.ForEach(PeersData.GetChannels(), proxy => proxy.CalculationStopped());

			// Wait for the late requests from peers.
			while (RicardAgrawalaData.GetQueueCount() != 0 || RicardAgrawalaData.TimeFromRequest() < 2000)
			{
				Thread.Sleep(250);
			}

			RicardAgrawalaData.Running = false;

			RicardAgrawalaData.PrintStats();
			ConsoleExtentions.Log(String.Format("Final value after {1} calculations: {0}",
				RicardAgrawalaData.CurrentValue,
				RicardAgrawalaData.Calculations));
		}

		public abstract void Sum(int param);

		public abstract void Subtract(int param);

		public abstract void Divide(int param);

		public abstract void Multiply(int param);
		public void Dispose()
		{
			HostObject.Close();
		}
	}
}
