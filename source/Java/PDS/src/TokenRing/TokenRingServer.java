package TokenRing;

import uni_bonn.pds.Client.State;
import uni_bonn.pds.Log;
import uni_bonn.pds.Server;

public class TokenRingServer extends Server {

	public boolean receiveToken() {
		TokenRingClient.state = State.HELD;
		return true;
	}

	public boolean doCalculation(String operation, String value) {
		int intValue = Integer.parseInt(value);
		switch (operation) {
		case "sum":
			processingValue += intValue;
			break;
		case "div":
			processingValue /= intValue;
			break;
		case "sub":
			processingValue -= intValue;
			break;
		case "mul":
			processingValue *= intValue;
			break;
		default:
			System.err.println("Unknown operation in doCalculation!");
			return false;
		}
		Log.logger.info("< " + operation + " >" + " performed with value:"
				+ value);
		return true;
	}

}
