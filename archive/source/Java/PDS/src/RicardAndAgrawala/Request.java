package RicardAndAgrawala;

import java.util.Vector;

public class Request {

	/** currentMachineInfo,TimeStamp,machineID,operation, Value */
	private Vector<String> params = new Vector<String>(3);

	public Request() {
		params.add(0, RAClient.currentMachineInfo);
		params.add("");
		params.add(2, Integer.toString(LCE.machineID));
	}

	public Vector<String> getParams() {
		return params;
	}

	public String getTimestampAndID() {
		return (params.get(1) + params.get(2));
	}

	public void modify(int currentTimeStamp) {
		params.set(1, Integer.toString(currentTimeStamp));

	}

}
