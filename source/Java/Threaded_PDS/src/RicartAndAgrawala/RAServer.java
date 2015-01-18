package RicartAndAgrawala;

import java.util.concurrent.locks.ReentrantLock;

import uni_bonn.pds.Client.State;
import uni_bonn.pds.Server;

public class RAServer extends Server {
	public static volatile LCE logClock = new LCE();
	ReentrantLock clockLock = new ReentrantLock();

	@Override
	synchronized public boolean start() {
		logClock.reset();
		return super.start();
	}

	public boolean receiveRequest(int TimeStamp, int ID) {

		System.out.println("Request received!");
		clockLock.lock();
		logClock.adjustClocks(TimeStamp);
		clockLock.unlock();
		while (true) {
			RAClient.lock.lock();
			try {
				if ((RAClient.state == State.HELD)
						|| ((RAClient.state == State.WANTED) && (RAClient.request
								.getTimestampAndID() < (TimeStamp * 10 + ID))))
					RAClient.condition.await();
				else
					break;

			} catch (InterruptedException e) {
				System.err.println(e.getMessage());
			} finally {
				RAClient.lock.unlock();
			}
		}

		/**
		 * System.out .println("Received: "+ TimeStamp+ ID+ " Current: "+
		 * RAClient.request.getTimestampAndID() + " State: "+ RAClient.state +
		 * " Condition: " + ((RAClient.state == State.HELD) || ((RAClient.state
		 * == State.WANTED) && (RAClient.request .getTimestampAndID() <
		 * (TimeStamp * 10 + ID)))));
		 */
		System.out.println("Sending OK!");
		return true;
	}

	@Override
	public boolean doCalculation(String operation, int value) {
		clockLock.lock();
		RAServer.logClock.increase();
		clockLock.unlock();
		return super.doCalculation(operation, value);
	}
}
