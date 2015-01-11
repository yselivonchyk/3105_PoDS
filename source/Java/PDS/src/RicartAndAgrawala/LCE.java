package RicartAndAgrawala;

public class LCE { // Extended Lamport Clocks

	public static int machineID = 1;
	int currentTimeStamp;

	public LCE() {
		currentTimeStamp = 0 ;
	}

	public int getCurrentTimeStamp() {
		return currentTimeStamp;
	}

	public void increase() {
		currentTimeStamp++;
	}

	public void reset() {
		currentTimeStamp = 0;
	}

	public void adjustClocks(int otherMachineTime) {
		if (otherMachineTime > currentTimeStamp)
			currentTimeStamp = otherMachineTime + 1;
	}

}
