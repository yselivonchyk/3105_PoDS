package RicartAndAgrawala;

import uni_bonn.pds.Client.State;
import uni_bonn.pds.Server;

public class RAServer extends Server {
	public static volatile LCE logClock = new LCE();

	public boolean receiveRequest(int TimeStamp, int ID) {
		//System.out.println("Request received!");
		logClock.adjustClocks(TimeStamp);
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
	//	System.out.println("Sending OK!");
		return true;
	}
}
