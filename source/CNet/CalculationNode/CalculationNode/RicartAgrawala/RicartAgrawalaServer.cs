using System;
using System.Collections.Generic;
using System.Diagnostics;
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
			get { return PeersData.CurrentValue; }
			protected set
			{
				PeersData.CurrentValue = value;
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
			Console.WriteLine("Server: " + AppDomain.CurrentDomain.GetHashCode() + "\n\r");
			ConsoleExtentions.Log("Server start recieved: " + seed);
			CurrentValue = seed;
			PeersData.LocalClient.StartSelf(seed);
			return true;
		}

		public bool DoCalculation(string operation, int value)
		{
			if(!PeersData.AwaitCalculationFlag)
				Debug.Fail("Calculation was not expected");

			PeersData.PopRequest();
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
		/// <returns></returns>
		public bool RecieveAccess(string address, int time)
		{
			PeersData.CurrentTime = time;
			// create request object
			var request = new CalculationRequest
			              {
				              Address = address,
				              Time = time,
				              Guid = Guid.NewGuid()
			              };
			// place into queue
			PeersData.AddRequest(request);
			// loop
			var processed = false;
			while (!processed)
			{
				lock (PeersData.QueueLock)
				{
					if (!PeersData.AwaitCalculationFlag && PeersData.NexRequest().Guid == request.Guid)
					{
						PeersData.AwaitCalculationFlag = true;
						return true;
					}
				}
				Thread.Yield();
			}

			throw new Exception("Unreachable section");
		}
	}
}
