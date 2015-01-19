package TokenRing;

import uni_bonn.pds.Client.State;
import uni_bonn.pds.Server;

public class TokenRingServer extends Server {

	public static boolean finished;

	@Override
	synchronized public boolean start(int initValue) {
		if (TokenRingClient.started)
			TokenRingClient.state = State.HELD;
		else
			TokenRingClient.state = State.RELEASED;
		finished = false;
		TokenRingClient.nextHost = TokenRingClient.findNextHost();
		System.err.println("NextHost: " + TokenRingClient.nextHost);
		return super.start(initValue);
	}

	public boolean receiveToken() {
		System.out.println("Token received!");
		TokenRingClient.lock.lock();

		if ((TokenRingClient.state == State.RELEASED) && (!finished)) {
			if (finishedSessions < machinesIPs.size() - 1)
				TokenRingClient.sendToken();
			else {
				TokenRingClient.state = State.HELD;
				TokenRingClient.condition.signal();
			}
		} else {
			TokenRingClient.state = State.HELD;
			TokenRingClient.condition.signal();
		}

		TokenRingClient.lock.unlock();

		return true;
	}

}
