package RicardAndAgrawala;

public class LogicalClock {

	int currentTimeStamp;

	public LogicalClock() {
		currentTimeStamp = 0;
	}
	

	public long getCurrentTimeStamp() {
		return currentTimeStamp;
	}

	public void adjustClocks(int otherMachineTime) {
		currentTimeStamp = otherMachineTime + 1;
	}

}

