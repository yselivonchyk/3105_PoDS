using System;
using System.Linq;
using System.Threading;
using CalculationNode.RicartAgrawala;

namespace CalculationNode
{
	public class RicartAgrawalaServer : MarshalByRefObject, IRicartAgrawalaServer
	{
		public long CurrentValue
		{
			get { return RicardAgrawalaData.CurrentValue; }
			protected set
			{
				RicardAgrawalaData.CurrentValue = value;
			}
		}

		public Object[] Join(string address)
		{
			ConsoleExtentions.Regresh();
			ConsoleExtentions.Log("Join: " + address);

			PeersData.Add(address);
			return PeersData.GetAll().Select(x => (Object)x).ToArray();
		}

		public bool SignOff(string address)
		{
			ConsoleExtentions.Log(String.Format("Sign off request from {0}", address));
			PeersData.Remove(address);
			PeersData.ListPeers();
			return true;
		}

		public bool Start(int seed)
		{
			ConsoleExtentions.Log("Server start recieved: " + seed);
			CurrentValue = seed;
			(new Thread(x => PeersData.LocalClient.StartSelf(seed))).Start();
			return true;
		}

		public bool DoCalculation(string operation, int value)
		{
			if (!RicardAgrawalaData.Running)
				ConsoleExtentions.Error("WARNING! Calculation request before server even started!!!");

			RicardAgrawalaData.Calculations++;
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

			ConsoleExtentions.Log(String.Format("Server calc recieved: {0}({1}) \t {2} -> {3}", 
				operation, value, original, CurrentValue));
			return true;
		}

		/// <summary>
		/// Method requested by Java implementation. To be called after event generation loop has stopped
		/// </summary>
		public void CalculationStopped() { }

		/// <summary>
		/// Controll access to critical section giving one positive responce at a time
		/// </summary>
		/// <param name="address"></param>
		/// <param name="time"></param>
		/// <returns>current timestemp</returns>
		public bool RecieveAccess(int time, int id)
		{
			RicardAgrawalaData.UpdateClock(time);
			// create request object
			var request = new CalculationRequest
			              {
				              Time = time,
				              Guid = Guid.NewGuid(),
							  CallerID = id
			              };
			// place into queue
			RicardAgrawalaData.AddRequest(request);
			if (!RicardAgrawalaData.Running)
				ConsoleExtentions.Warning("Add request recieved before server started. Request put on hold");

			// Main await loop
			while (true)
			{
				lock (RicardAgrawalaData.QueueLock)
				{
					if (PeersData.LocalID == id 
						|| !RicardAgrawalaData.IsInterested 
						|| HasPriority(time, id))
					{
						LogSuccessAccessRequest(request);
						return true;
					}
				}
				Thread.Sleep(5);
			}
		}


		private static void LogSuccessAccessRequest(CalculationRequest request)
		{
			Console.WriteLine("\tSend OK (LocalID:{1} T:{0}) MY(T:{2} RT:{3} LocalID:{4})", request.Time, request.CallerID,
				RicardAgrawalaData.ExactTime,
				RicardAgrawalaData.IsInterested ? RicardAgrawalaData.RequestTime.ToString() : "-",
				PeersData.LocalID);

			RicardAgrawalaData.PopRequest(request);
		}

		private static bool HasPriority(int remoteTime, int remoteID)
		{
			if (RicardAgrawalaData.RequestTime > remoteTime)
				return true;
			if (RicardAgrawalaData.RequestTime < remoteTime)
				return false;
			return remoteID < PeersData.LocalID;
		}
	}
}
