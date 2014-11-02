using System;
using System.Linq;
using System.ServiceModel;
using Microsoft.Samples.XmlRpc;

namespace CalculationNode.RicartAgrawala
{
	/// <summary>
	/// Implements all algorithms required by RicardAgrawala routin
	/// </summary>
	public class RicartAgrawalaClient : ClientBase
	{
		public RicartAgrawalaClient(Uri baseServerUri) : base(baseServerUri)
		{
			var localServerUri = BaseServerUri;
			LocalServerAddress = localServerUri.ToString();
			HostObject = new ServiceHost(typeof(RicartAgrawalaServer));
			var epXmlRpc = HostObject.AddServiceEndpoint(typeof(IRicartAgrawalaServer),
				new WebHttpBinding(WebHttpSecurityMode.None), localServerUri);
			epXmlRpc.Behaviors.Add(new XmlRpcEndpointBehavior());
			HostObject.Open();
			Console.WriteLine("Ricard-Agrawala node listning at {0}", epXmlRpc.ListenUri);
		}

		public override void Join(Uri fellowAddress)
		{
			var fellowAddressString = fellowAddress.ToString();
			// Set up client channel factory
			PeersData.Add(fellowAddressString);

			var calculator = PeersData.GetChannel(fellowAddressString);
			var fellows = calculator.Join(LocalServerAddress).ToList().Where(x => !x.Equals(fellowAddressString));

			foreach (var fellow in fellows)
			{
				PeersData.Add(fellow);
				var fellowChannel = PeersData.GetChannel(fellow);
				var response = fellowChannel.Join(LocalServerAddress);
				ConsoleExtentions.Log(String.Format("Got response with {0} items", response.Length));
				//response.ToList().ForEach(x => Console.WriteLine("{0} - {1}", fellow, x));
			}
		}

		public override void Start(int seed)
		{
			throw new NotImplementedException();
		}

		public override void Sum(int param)
		{
			throw new NotImplementedException();
		}

		public override void Substract(int param)
		{
			throw new NotImplementedException();
		}

		public override void Divide(int param)
		{
			throw new NotImplementedException();
		}

		public override void Multiply(int param)
		{
			throw new NotImplementedException();
		}
	}
}
