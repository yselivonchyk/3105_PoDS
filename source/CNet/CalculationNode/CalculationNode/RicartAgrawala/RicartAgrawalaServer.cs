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

        public int SingOff(string address)
        {
			ConsoleExtentions.Log(String.Format("Sign off request from {0}", address));
            PeersData.Remove(address);
	        return 0;
        }

        public void Start(int seed)
        {
            CurrentValue = seed;
        }

        public virtual void Sum(int param)
        {
            CurrentValue += param;
        }

        public virtual void Substract(int param)
        {
            CurrentValue -= param;
        }

        public virtual void Divide(int param)
        {
            CurrentValue /= param;
        }

        public virtual void Multiply(int param)
        {
            CurrentValue *= param;
        }

		public virtual string Echo(string param)
		{
			return param;
		}
    }
}
