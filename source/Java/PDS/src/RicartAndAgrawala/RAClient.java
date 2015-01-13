package RicartAndAgrawala;

import uni_bonn.pds.Client;
import uni_bonn.pds.RandomOperation;

public class RAClient extends Client implements Runnable {

	public volatile static Request request;
	public volatile static State state;
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
		LCE.machineID = Client.serverURLs.size();
	}

	 public void enterSection() throws InterruptedException {
		System.out.println("Entering critical area!");
		RAServer.numberOfReplies = 0;
		state = State.WANTED;
		request.modify(logClock.getCurrentTimeStamp());
		this.executeForAll("Node.receiveRequest", request.getParams());
	}

	 public void exitSection() {
		System.out.println("Exiting critical area!");
		state = State.RELEASED;
		RAServer.sendOKToAll();
	}

	@Override
	 public void run() {
		startTime = System.currentTimeMillis();
		while (SESSION_LENGTH > System.currentTimeMillis() - startTime) {

			try {
				enterSection();

				while (state != State.HELD) {
					Thread.sleep(200);
				}
				System.err.println("Access to critical area obtained!");
				// System.err.println(RAServer.queue.toString());
				/** Do calculations on all machines */
				this.executeForAll("Node.doCalculation",
						randomOperation.nextOperationAndValue());
				exitSection();
			} catch (InterruptedException e) {
				System.err.println(e.getMessage());
			}
		}
		finalizeSession();
	}
}
