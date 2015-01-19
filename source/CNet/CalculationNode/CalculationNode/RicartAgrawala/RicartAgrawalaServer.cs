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
			ConsoleExtentions.Regresh();
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

		/// <summary>
		/// Split of start into two methods due to Java limitations
		/// </summary>
		/// <param name="seed"></param>
		/// <returns></returns>
		public bool Init(int seed)
		{
			//Console.Clear();
			ConsoleExtentions.Log("Server init recieved: " + seed);
			PeersData.Calculations = 0;
			CurrentValue = seed;
			return true;
		}

		public bool Start()
		{
			ConsoleExtentions.Log("Server start recieved");
			var t = new Thread(x => PeersData.LocalClient.StartSelf(CurrentValue));
			t.Start();
			return true;
		}

		public bool DoCalculation(string operation, int value)
		{
			PeersData.Calculations++;
			var original = CurrentValue;
			Console.WriteLine("Lock reached");
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

		public bool DoCalculation(string operation, int value, int final)
		{
			var initial = RicardAgrawalaData.CurrentValue;
			DoCalculation(operation, value);
			if (final == initial)
				return true;
			else
			{
				Console.WriteLine("I have done something wrong :(  my: {0} - {1} : expected", initial, final);
				Thread.Sleep(1000);
				return false;
			}
		}

		/// <summary>
		/// Controll access to critical section giving one positive responce at a time
		/// </summary>
		/// <param name="address"></param>
		/// <param name="time"></param>
		/// <returns>current timestemp</returns>
		public bool RecieveAccess(int time, int id)
		{
			RicardAgrawalaData.RATimestamp = time;
			// create request object
			var request = new CalculationRequest
			              {
				              Address = id.ToString(),
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
					if (!ClientBase.Running)
					{
						Console.WriteLine("NOT STARTED YET");
						Thread.Sleep(1000);
						continue;
					}
					if (PeersData.ID == id 
						|| !RicardAgrawalaData.IsInterested 
						|| HasPriority(time, id))
					{
						RicardAgrawalaData.PopRequest(request);
						Console.WriteLine("I am good with ID:{1} at his time: {0}", time, id);
						//return RicardAgrawalaData.ExectTime;
						return true;
					}
				}
				Thread.Sleep(5);
			}
		}

		public void EspaciallyForElchyn()
		{
			Console.WriteLine("It is, so called, required finalization!");
		}

		private bool HasPriority(int remoteTime, int remoteID)
		{
//			Console.WriteLine("l: {0} \tr: {1} \tlid: {2} \trid: {3}",
//				RicardAgrawalaData.RequestTime,
//				remoteTime,
//				PeersData.ID,
//				remoteID);
			if (RicardAgrawalaData.RequestTime > remoteTime)
				return true;
			if (RicardAgrawalaData.RequestTime < remoteTime)
				return false;
			return remoteID < PeersData.ID;
		}
	}
}
