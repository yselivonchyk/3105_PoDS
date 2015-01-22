using System;

namespace CalculationNode.RicartAgrawala
{
	public class CalculationRequest
	{
		public int Time { get; set; }
		public Guid Guid { get; set; }
		public int CallerID { get; set; }
	}
}
