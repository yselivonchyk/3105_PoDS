package RicartAndAgrawala;

import uni_bonn.pds.Client;
import uni_bonn.pds.RandomOperation;

public class RAClient extends Client implements Runnable {

	public static Request request;
	public static State state;
	RandomOperation randomOperation;
	LCE logClock = RAServer.logClock;
	long startTime;

	public RAClient() {
		state = State.RELEASED;
		request = new Request();
		randomOperation = new RandomOperation();
	}

	@Override
	public void join(String memberIPandPort) {
		super.join(memberIPandPort);
		LCE.machineID = this.serverURLs.size();
	}

	public void enterSection() throws InterruptedException {
		System.out.println("Entering critical area!");

		RAServer.numberOfReplies = 0;
		state = State.WANTED;
		request.modify(logClock.getCurrentTimeStamp());
		this.executeForAll("Server.receiveRequest", request.getParams());

		// Waiting OKs from others
		while (RAServer.numberOfReplies < serverURLs.size()) {
			Thread.sleep(200);
		}
		state = State.HELD;

		System.err.println("Access to critical area obtained!");
		// System.err.println(RAServer.queue.toString());
		/** Do calculations on all machines */
		this.executeForAll("Server.doCalculation",
				randomOperation.nextOperationAndValue());

	}

	public void exitSection() {
		System.out.println("Exiting critical area!");
		state = State.RELEASED;
		RAServer.sendOK();
	}

	@Override
	public void run() {
		startTime = System.currentTimeMillis();
		while (SESSION_LENGTH > System.currentTimeMillis() - startTime) {

			try {
				Thread.sleep(randomOperation.getRandomWaitingTime());
				enterSection();
				exitSection();
			} catch (InterruptedException e) {
				System.err.println(e.getMessage());
			}
		}
		finalizeSession();
	}
}
