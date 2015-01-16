package RicartAndAgrawala;

import uni_bonn.pds.Client.State;
import uni_bonn.pds.Server;

public class RAServer extends Server {
	public static volatile LCE logClock = new LCE();

	@Override
	synchronized public boolean start() {
		logClock.reset();
		return super.start();
	}

	synchronized public boolean receiveRequest(String IPandPort, int TimeStamp,
			int ID) throws InterruptedException {

		System.out.println("Request received!");

		logClock.adjustClocks(TimeStamp);
		while ((RAClient.state == State.HELD)
				|| ((RAClient.state == State.WANTED) && (RAClient.request
						.getTimestampAndID() < (TimeStamp * 10 + ID)))) {
			Thread.yield();

		}

		System.out
				.println("Received: "
						+ TimeStamp
						+ ID
						+ " Current: "
						+ RAClient.request.getTimestampAndID()
						+ " State: "
						+ RAClient.state
						+ " Condition: "
						+ ((RAClient.state == State.HELD) || ((RAClient.state == State.WANTED) && (RAClient.request
								.getTimestampAndID() < (TimeStamp * 10 + ID)))));
		System.out.println("Sending OK!");

		return true;
	}

	@Override
	synchronized public boolean doCalculation(String operation, int value) {
		RAServer.logClock.increase();
		return super.doCalculation(operation, value);
	}

}
