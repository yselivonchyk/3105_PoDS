package RicartAndAgrawala;

public class LCE { // Extended Lamport Clocks

	public static int machineID;
	int currentTimeStamp;

	public LCE() {
		currentTimeStamp = 0;
	}

	public int getCurrentTimeStamp() {
		return currentTimeStamp;
	}

	synchronized public void increase() {
		currentTimeStamp++;
	}

	public void reset() {
		currentTimeStamp = 0;
	}

	synchronized public void adjustClocks(int otherMachineTime) {
		if (otherMachineTime > currentTimeStamp)
			currentTimeStamp = otherMachineTime + 1;
	}

}
