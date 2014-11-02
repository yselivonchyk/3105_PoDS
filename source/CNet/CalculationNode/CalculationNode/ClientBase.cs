using System;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;

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
			// save information about known node
			var knownNodeAddress = knownNodeUri.ToString();
			PeersData.Add(knownNodeAddress);
			// send join request to known node
			var nodeProxy = PeersData.GetChannel(knownNodeAddress);
			var allNodesAddresses = nodeProxy.Join(LocalServerAddress);
			var undiscoveredNodesAddresses = allNodesAddresses.Where(x => !x.Equals(knownNodeAddress));
			// Make parallel non-blocking call to all other nodes
			Parallel.ForEach(
				undiscoveredNodesAddresses,
				siblingAddress =>
				{
					PeersData.Add(siblingAddress);
					var siblingProxy = PeersData.GetChannel(siblingAddress);
					var joinResponse = siblingProxy.Join(LocalServerAddress);
					ConsoleExtentions.Log(String.Format("Got response with {0} items", joinResponse.Length));
					//response.ToList().ForEach(x => Console.WriteLine("{0} - {1}", fellow, x));
				});
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
