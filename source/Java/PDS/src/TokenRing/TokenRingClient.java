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
	Vector<Object> emptyParams = new Vector<>();
	public static State state;
	RandomOperation randomOperation;

	private static URL findNextHost() {
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
		config.setServerURL(nextHost);
		xmlRpcClient.setConfig(config);
		randomOperation = new RandomOperation();
	}

	public void start(int initValue) {
		nextHost = findNextHost();
		super.start(initValue);
	}

	public void enterSection() {
		state = State.WANTED;
	}

	public void exitSection() {
		state = State.RELEASED;
		sendToken();
	}

	@Override
	public void run() {

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

	public void sendToken() {
		try {
			xmlRpcClient.execute("Server.receiveToken", emptyParams);
			state = State.RELEASED;
		} catch (XmlRpcException e) {
			System.err.println("Error while sending token!");
			e.printStackTrace();
		}

	}

}
