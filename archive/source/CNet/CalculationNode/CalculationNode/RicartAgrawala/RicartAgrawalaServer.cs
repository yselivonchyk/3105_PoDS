using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace CalculationNode
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class RicartAgrawalaServer : IRicartAgrawalaServer
    {
		public List<String> Fellows = new List<String>();
        public int CurrentValue { get; protected set; }

        public string[] Join(string address)
        {
			ConsoleExtentions.Log("Join: " + address);
			PeersData.Add(address);
			return PeersData.GetAll();
        }

        public bool SingOff(string address)
        {
			ConsoleExtentions.Log(String.Format("Sign off request from {0}", address));
            PeersData.Remove(address);
			return true;
        }

		public bool Start(int seed)
        {
            CurrentValue = seed;
			return true;
        }

		public virtual bool Sum(int param)
        {
            CurrentValue += param;
			return true;
        }

		public virtual bool Substract(int param)
        {
            CurrentValue -= param;
			return true;
        }

		public virtual bool Divide(int param)
        {
            CurrentValue /= param;
			return true;
        }

		public virtual bool Multiply(int param)
        {
            CurrentValue *= param;
			return true;
        }
    }
}
