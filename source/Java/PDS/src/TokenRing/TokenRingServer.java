package TokenRing;

import java.util.Vector;

import org.apache.xmlrpc.XmlRpcException;

import uni_bonn.pds.Client;
import uni_bonn.pds.Client.State;
import uni_bonn.pds.Server;

public class TokenRingServer extends Server {

	public static boolean finished = false;
	int finishedSessions = 0;
	static Vector<Object> emptyParams = new Vector<>();

	@Override
	public boolean start(int initValue) {
		TokenRingClient.nextHost = TokenRingClient.findNextHost();
		System.err.println("NextHost: " + TokenRingClient.nextHost);
		return super.start(initValue);
	}

	public boolean receiveToken() {
		System.out.println("Token received!");
		if ((TokenRingClient.state == State.RELEASED) && (finished == false)) {
			sendToken();
			return true;
		} else
			TokenRingClient.state = State.HELD;
		return false;
	}

	public static void sendToken() {
		try {
			TokenRingClient.state = State.RELEASED;
			System.err.println("Sending token to " + TokenRingClient.nextHost);
			Client.config.setServerURL(TokenRingClient.nextHost);
			Client.xmlRpcClient.execute("Server.receiveToken", emptyParams);
			System.err.println("Token is sent to: "
					+ Client.config.getServerURL());
		} catch (XmlRpcException e) {
			System.err.println("Error while sending token!");
			e.printStackTrace();
		}

	}

	public boolean finilize() {
		finishedSessions++;
		if ((finishedSessions == TokenRingServer.machinesIPs.size() - 1)
				&& (finished == false)) {
			finished = true;
			return true;
		} else
			return false;
	}

}
