﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CalculationNode.RicartAgrawala;

namespace CalculationNode
{
	public class RicartAgrawalaServer : MarshalByRefObject, IRicartAgrawalaServer
	{
		public List<String> Fellows = new List<String>();

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
			return true;
		}

		public bool Start(int seed)
		{
			ConsoleExtentions.Log("Server start recieved: " + seed);
			SafetyCheck();
			CurrentValue = seed;
			var t = new Thread(x => PeersData.LocalClient.StartSelf(seed));
			t.Start();
			return true;
		}

		private static void SafetyCheck()
		{
			if (RicardAgrawalaData.Calculations != 0)
			{
				Console.BackgroundColor = ConsoleColor.Red;
				Console.WriteLine("WARNING! some operations were performed before start");
				Console.BackgroundColor = ConsoleColor.Black;
			}
		}

		public bool DoCalculation(string operation, int value)
		{
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

		public bool DoCalculation(string operation, int value, int final)
		{
			var initial = RicardAgrawalaData.CurrentValue;
			DoCalculation(operation, value);
			if (final == initial)
				return true;
			else
			{
				Console.WriteLine("I have done something wrong :(  my: {0} - {1} : expected", initial.ToString("D20"), final.ToString("D20"));
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
			RicardAgrawalaData.RegisterOperation(id);
			// create request object
			var request = new CalculationRequest
			              {
				              Address = id.ToString(),
				              Time = time,
				              Guid = Guid.NewGuid()
			              };
			// place into queue
			RicardAgrawalaData.AddRequest(request);
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
						Console.WriteLine("\tSend OK (ID:{1} T:{0}) MY(T:{2} RT:{3} ID:{4})", time, id, 
							RicardAgrawalaData.ExectTime,
							RicardAgrawalaData.IsInterested ? RicardAgrawalaData.RequestTime.ToString() : "-",
							PeersData.ID);
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
