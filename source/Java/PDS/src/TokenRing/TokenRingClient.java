package TokenRing;

import java.net.MalformedURLException;
import java.net.URL;
import java.util.ArrayList;
import java.util.Collections;
import java.util.Vector;

import org.apache.xmlrpc.XmlRpcException;

import uni_bonn.pds.Client;
import uni_bonn.pds.RandomOperation;

public class TokenRingClient extends Client implements Runnable {
	public static URL nextHost;
	static Vector<Object> emptyParams = new Vector<>();
	public static State state;
	RandomOperation randomOperation;
	long startTime;

	static URL findNextHost() {
		URL currentMachineURL;
		try {
			currentMachineURL = new URL("http://"
					+ TokenRingClient.currentMachineInfo);
			@SuppressWarnings("unchecked")
			ArrayList<URL> temp = (ArrayList<URL>) TokenRingClient.serverURLs
					.clone();

			URLComparator urlComparator = new URLComparator();
			Collections.sort(temp, urlComparator);
			for (URL next : temp) {
				if (urlComparator.compare(currentMachineURL, next) < 0) {
					return next;
				}
			}
			return temp.get(0);
		} catch (MalformedURLException e) {
			System.err.println(e.getMessage());
			return null;
		}
	}

	public TokenRingClient() {
		System.err.println("TokenRingClient constructor");
		state = State.HELD;
		/**********************************************************************************/
		// config.setServerURL(nextHost);
		// xmlRpcClient.setConfig(config);
		randomOperation = new RandomOperation();
	}

	public void enterSection() {
		System.out.println("Entering critical area!");
		if (state != State.HELD)
			state = State.WANTED;
	}

	public void exitSection() {
		System.out.println("Exiting critical area!");
		state = State.RELEASED;
		sendToken();
	}

	@Override
	public void run() {
		startTime = System.currentTimeMillis();
		while (SESSION_LENGTH > System.currentTimeMillis() - startTime) {
			enterSection();
			while (state != State.HELD) {
				try {
					Thread.sleep(200);
				} catch (InterruptedException e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
				}
			}
			executeForAll("Server.doCalculation",
					randomOperation.nextOperationAndValue());
			exitSection();
		}
		finalizeSession();
	}

	public static void sendToken() {
		try {
			state = State.RELEASED;
			System.err.println("Sending token to " + nextHost);
			config.setServerURL(nextHost);
			xmlRpcClient.execute("Server.receiveToken", emptyParams);
			// System.err.println("Token is sent to: "+config.getServerURL());
		} catch (XmlRpcException e) {
			System.err.println("Error while sending token!");
			e.printStackTrace();
		}

	}

	
	@Override
	public void finalizeSession() {
		TokenRingServer.finished=true;
		super.finalizeSession();
	}
}
