package uni_bonn.pds;

import java.util.Random;
import java.util.Vector;

//Class for random operation, waiting time and operation generation
public class RandomOperation {

	int maximalWaitingTime = 1000;
	int minimalWaitingTime = 200;
	int valueLimit = 9;
	Vector<Object> operationAndValue = new Vector<Object>();

	Random random = new Random();
	String[] operations = { "sum", "sub", "div", "mul" };

	public int nextValue() {
		return (random.nextInt(valueLimit) + 1);
	}

	public String nextOperation() {
		return operations[random.nextInt(4)];
	}

	public long getRandomWaitingTime() {// Return waiting time between
										// minimalWaitingTime and
										// maximalWaitingTime
		return (random.nextInt(maximalWaitingTime - minimalWaitingTime) + minimalWaitingTime);
	}

	public Vector<Object> nextOperationAndValue() {
		operationAndValue.removeAllElements();
		operationAndValue.add(this.nextOperation());
		operationAndValue.add(this.nextValue());
		return operationAndValue;
	}

}