package uni_bonn.pds;

public class Calculator {

	public static int processingValue = 0;
	
	public void sum(double x) {
		processingValue+=x;
	}

	public void substract(double x) {
		processingValue-=x;
	}

	public void multiply(double x) {
		processingValue*=x;
	}

	public void divide(double x) {
		processingValue/=x;
	}
}
