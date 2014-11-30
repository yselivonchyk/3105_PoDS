package uni_bonn.pds;

import java.util.Random;
import java.util.Vector;

public class RandomOperation {

	int waitingTimeLimit = 4000;
	int minimalWaitingTime = 200;
	int valueLimit = 10;
	Vector<String> operationAndValue = new Vector<String>();

	Random random = new Random();
	String[] operations = { "sum", "sub", "div", "mul" };

	public int nextValue() {
		return (random.nextInt(valueLimit) + 1);
	}

	public String nextOperation() {
		return operations[random.nextInt(4)];
	}

	public long getRandomWaitingTime() {
		return (random.nextInt(waitingTimeLimit - minimalWaitingTime) + minimalWaitingTime);
	}

	public Vector<String> nextOperationAndValue() {
		operationAndValue.removeAllElements();
		operationAndValue.add(this.nextOperation());
		operationAndValue.add(Integer.toString(this.nextValue()));
		return operationAndValue;
	}

}