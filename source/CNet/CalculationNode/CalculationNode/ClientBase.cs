using System;
using System.ServiceModel;

namespace CalculationNode
{
	public abstract class ClientBase : IClient, IDisposable
	{
		protected Uri BaseServerUri;
		protected ServiceHost HostObject;
		protected string LocalServerAddress;

		public ClientBase(Uri baseServerUri)
		{
			BaseServerUri = baseServerUri;
		}

		public abstract void Join(Uri knownNodeUri);

		public void SingOff(string address)
		{
			
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
