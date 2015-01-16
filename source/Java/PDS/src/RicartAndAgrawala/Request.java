package RicartAndAgrawala;

import java.util.Vector;

public class Request {
	/** currentMachineInfo,TimeStamp,machineID,operation, Value */
	private Vector<Object> params = new Vector<Object>(3);

	String nodeInfo;
	int nodeID;
	int currentTimeStamp;

	public Request() {
		this.nodeInfo = RAClient.currentMachineInfo;
		nodeID = LCE.machineID;

		params.add(nodeInfo);
		params.add(0);
		params.add(nodeID);
	}

	public Vector<Object> getParams() {
		params.set(1, currentTimeStamp);
		return params;
	}

	public int getTimestampAndID() {
		return (currentTimeStamp * 10 + nodeID);
	}

	public void modify(int currentTimeStamp) {
		this.currentTimeStamp = currentTimeStamp;

	}

}
