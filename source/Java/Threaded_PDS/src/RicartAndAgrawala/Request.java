package RicartAndAgrawala;

import java.util.Vector;

public class Request {
	/** currentMachineInfo,TimeStamp,machineID,operation, Value */
	private Vector<Object> params = new Vector<Object>(3);

	int nodeID;
	int currentTimeStamp;

	public Request() {
		this.nodeID = LCE.machineID;
		this.params.add(0);
		this.params.add(nodeID);
	}

	public Vector<Object> getParams() {
		this.params.set(0, currentTimeStamp);
		return this.params;
	}

	public int getTimestamp() {
		return this.currentTimeStamp;
	}

	public int getTimestampAndID() {
		return this.currentTimeStamp * 10 + nodeID;
	}

	public void modify(int currentTimeStamp) {
		this.currentTimeStamp = currentTimeStamp;
	}

}
