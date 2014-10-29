using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Samples.XmlRpc;

namespace CalculationNode
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class TokenRingCalculator : ICalculator
    {
		public List<String> Fellows = new List<String>();
        public int CurrentValue { get; protected set; }

        public string[] Join(string address)
        {
			ConsoleExtentions.Log("Join request from " + address);
			Fellows.Add(address);
			return Fellows.ToArray();
        }

        public string SingOff(string address)
        {
            return address + DateTime.Now.ToShortTimeString();
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
