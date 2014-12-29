package TokenRing;

import uni_bonn.pds.Client.State;
import uni_bonn.pds.Server;

public class TokenRingServer extends Server {

	private boolean tokenRingExists = true;

	@Override
	public boolean start(int initValue) {
		TokenRingClient.nextHost = TokenRingClient.findNextHost();
		System.err.println("NextHost: " + TokenRingClient.nextHost);
		return super.start(initValue);
	}

	public boolean receiveToken() {
		System.out.println("Token received!");
		if ((TokenRingClient.state == State.RELEASED)&&(tokenRingExists)) {
			TokenRingClient.sendToken();
		} else
			TokenRingClient.state = State.HELD;
		return true;
	}

}
