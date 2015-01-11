using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CalculationNode.RicartAgrawala;

namespace CalculationNode
{
	public class RicartAgrawalaServer : MarshalByRefObject, IRicartAgrawalaServer
	{
		public List<String> Fellows = new List<String>();

		public int CurrentValue
		{
			get { return RicardAgrawalaData.CurrentValue; }
			protected set
			{
				RicardAgrawalaData.CurrentValue = value;
			}
		}

		public Object[] Join(string address)
		{
			ConsoleExtentions.Log("Join: " + address);
			PeersData.Add(address);
			return PeersData.GetAll().Select(x => (Object)x).ToArray();
		}

		public bool SingOff(string address)
		{
			ConsoleExtentions.Log(String.Format("Sign off request from {0}", address));
			PeersData.Remove(address);
			return true;
		}

		public bool Start(int seed)
		{
			ConsoleExtentions.Log("Server start recieved: " + seed);
			CurrentValue = seed;
			PeersData.LocalClient.StartSelf(seed);
			return true;
		}

		public bool DoCalculation(string operation, int value)
		{
			var original = CurrentValue;
			switch (operation)
			{
				case "sum":
					CurrentValue += value;
					break;
				case "div":
					CurrentValue /= value;
					break;
				case "sub":
					CurrentValue -= value;
					break;
				case "mul":
					CurrentValue *= value;
					break;
				default:
					Console.WriteLine("Unknown operation: {0}", operation);
					return false;
			}

			ConsoleExtentions.Log(String.Format("Server calc recieved: {0}({1}) \t {2} -> {3}", operation, value, original, CurrentValue));
			return true;
		}

		/// <summary>
		/// Controll access to critical section giving one positive responce at a time
		/// </summary>
		/// <param name="address"></param>
		/// <param name="time"></param>
		/// <returns>current timestemp</returns>
		public int RecieveAccess(string address, int time)
		{
			var localTime = RicardAgrawalaData.ExectTime;
			RicardAgrawalaData.RATimestamp = time;
			// create request object
			var request = new CalculationRequest
			              {
				              Address = address,
				              Time = time,
				              Guid = Guid.NewGuid()
			              };
			// place into queue
			RicardAgrawalaData.AddRequest(request);
			// loop
			var client = (RicartAgrawalaClient) PeersData.LocalClient;
			while (true)
			{
				lock (RicardAgrawalaData.QueueLock)
				{
					if (client.LocalServerAddress == address || !client.IsInterested 
						|| HasPriority(address, time, client.LocalServerAddress, localTime))
					{
						RicardAgrawalaData.PopRequest(request);
						return RicardAgrawalaData.RATimestamp;
					}
				}
				Thread.Yield();
			}
		}

		private bool HasPriority(string remoteAddress, int remoteTime, string localAddress, int localTime)
		{
			if (localTime > remoteTime)
				return true;
			if(localTime < remoteTime)
				return false;
			return String.Compare(localAddress, remoteAddress, StringComparison.InvariantCulture) > 0;
		}
	}
}
