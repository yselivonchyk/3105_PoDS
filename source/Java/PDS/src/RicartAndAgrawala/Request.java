package RicartAndAgrawala;

import java.util.Vector;

public class Request {

	/** currentMachineInfo,TimeStamp,machineID,operation, Value */
	private Vector<Object> params = new Vector<Object>(3);

	public Request() {
		params.add(0, RAClient.currentMachineInfo);
		params.add("");
		params.add(2, LCE.machineID);
	}

	public Vector<Object> getParams() {
		return params;
	}

	public int getTimestampAndID() {
		return (((int) params.get(1)) * 10 + (int) params.get(2));
	}

	public void modify(int currentTimeStamp) {
		params.set(1, currentTimeStamp);

	}

}
