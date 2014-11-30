package RicardAndAgrawala;

import uni_bonn.pds.Client;
import uni_bonn.pds.RandomOperation;

public class RAClient extends Client implements Runnable {
	

	public static Request request;
	public static State state;
	RandomOperation randomOperation;
	LCE logClock = RAServer.logClock;

	public RAClient() {
		state = State.RELEASED;
		request = new Request();
		randomOperation = new RandomOperation();
	}


	public void enterSection() throws InterruptedException {
	System.out.println("Entering critical area!");

		RAServer.numberOfReplies = 0;
		state = State.WANTED;
		request.modify(logClock.getCurrentTimeStamp());
		this.executeForAll("Server.receiveRequest", request.getParams());
	
		//Waiting OKs from others
		while (RAServer.numberOfReplies < (serverURLs.size() - 1)) {
			Thread.sleep(200);
		}
		state = State.HELD;
		/**   Do calculations on all machines  */
		this.executeForAll("Server.doCalculation", randomOperation.nextOperationAndValue());

	}

	public void exitSection() {
		System.out.println("Exiting critical area!");
		state = State.RELEASED;
		RAServer.sendOK();
	}

	@Override
	public void run() {
		try {
			System.out.println("Hello!");
			Thread.sleep(randomOperation.getRandomWaitingTime());
			enterSection();
			exitSection();
		} catch (InterruptedException e) {
			System.err.println(e.getMessage());
		}

	}
}
