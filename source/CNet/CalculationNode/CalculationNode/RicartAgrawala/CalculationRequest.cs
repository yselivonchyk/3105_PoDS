using System;

namespace CalculationNode.RicartAgrawala
{
	public class CalculationRequest
	{
		public int Time { get; set; }
		public string Address { get; set; }
		public Guid Guid { get; set; }
	}
}
