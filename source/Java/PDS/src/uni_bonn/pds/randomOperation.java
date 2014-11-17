package uni_bonn.pds;

import java.util.Random;
import java.util.Vector;

public class RandomOperation {

	Random random = new Random();
	int maximalNaturalNumber;
	Vector<Integer> value = new Vector<Integer>();
	String calculatorClass;
	int valueLimit = 10;

	public RandomOperation() {
		calculatorClass="Calculator";
		value.add(0);
	}

	public Vector<Integer> nextValue() {
		value.set(0, random.nextInt(valueLimit) + 1);
		return value;
	}

	public String nextOperation() {

		String operation = calculatorClass;

		switch (random.nextInt(4)) {
		case 0:
			operation += ".sum";
			break;
		case 1:
			operation += ".sub";
			break;
		case 2:
			operation += ".div";
			break;
		case 3:
			operation += ".mul";
			break;
		}
		return operation;
	}
}