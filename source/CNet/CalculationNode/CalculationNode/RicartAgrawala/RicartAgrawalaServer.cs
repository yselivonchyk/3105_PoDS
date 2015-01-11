using System;
using System.Collections.Generic;
using System.Linq;

namespace CalculationNode
{
	public class RicartAgrawalaServer : MarshalByRefObject, IRicartAgrawalaServer
    {
		public List<String> Fellows = new List<String>();

		public int CurrentValue
		{
			get { return PeersData.CurrentValue; }
			protected set { PeersData.CurrentValue = value; }
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

			ConsoleExtentions.Log(String.Format("Server calc recieved: {0}({1}) -> {2}", operation, value, CurrentValue));

			return true;
		}
    }
}
