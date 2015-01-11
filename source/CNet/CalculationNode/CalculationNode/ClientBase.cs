using System;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using CalculationNode.Extentions;

namespace CalculationNode
{
	public abstract class ClientBase : IClient, IDisposable
	{
		protected Uri BaseServerUri;
		protected ServiceHost HostObject;
		protected string LocalServerAddress;

		protected ClientBase(Uri baseServerUri)
		{
			BaseServerUri = baseServerUri;
		}

		public void Join(Uri knownNodeUri)
		{
			// Add self 
			if(!LocalServerAddress.Equals(knownNodeUri.ToString()))
				Join(LocalServerAddress);
			// save information about known node
			var knownNodeAddress = knownNodeUri.ToString();
			var peers = Join(knownNodeAddress);
			var undiscoveredNodesAddresses = peers
				.Where(x => !knownNodeAddress.Contains(x) && !LocalServerAddress.Contains(x));
			// Make parallel non-blocking call to all other nodes
			Parallel.ForEach(
				undiscoveredNodesAddresses,
				siblingAddress => Join(NetworkExtentions.TryBuildServerUri(siblingAddress).ToString()));
		}

		// Get a connetion and call remote Join operation on a single node
		private string[] Join(string nodeAddress)
		{
			PeersData.Add(nodeAddress);
			var siblingProxy = PeersData.GetChannel(nodeAddress);

			var joinResponse = siblingProxy.Join(LocalServerAddress);
			ConsoleExtentions.Log(String.Format("Got response with {0} items from {1}", 
				joinResponse.Length,
				nodeAddress));
			return joinResponse.Select(x => x.ToString()).ToArray();
		}

		public void SingOff()
		{
			ConsoleExtentions.Log("Empty request recieved.");

			foreach (var peer in PeersData.GetAll())
			{
				var siblingProxy = PeersData.GetChannel(peer);
				siblingProxy.SingOff(LocalServerAddress);
			}

			Parallel.ForEach(PeersData.GetAll(),
				peer =>
				{
					var siblingProxy = PeersData.GetChannel(peer);
					siblingProxy.SingOff(LocalServerAddress);
				});
			PeersData.Empty();
		}

		public abstract void Start(int seed);

		public abstract void Sum(int param);

		public abstract void Substract(int param);

		public abstract void Divide(int param);

		public abstract void Multiply(int param);
		public void Dispose()
		{
			HostObject.Close();
		}
	}
}
