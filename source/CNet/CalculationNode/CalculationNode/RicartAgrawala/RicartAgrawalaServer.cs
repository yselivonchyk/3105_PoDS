using System;
using System.Collections.Generic;
using System.Linq;

namespace CalculationNode
{
	public class RicartAgrawalaServer : MarshalByRefObject, IRicartAgrawalaServer
    {
		public List<String> Fellows = new List<String>();
        public int CurrentValue { get; protected set; }

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
