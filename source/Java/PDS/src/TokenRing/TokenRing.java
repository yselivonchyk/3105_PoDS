package TokenRing;

import java.net.MalformedURLException;
import java.net.URL;
import java.sql.Time;
import java.util.ArrayList;
import java.util.Collections;
import java.util.Vector;

import org.apache.xmlrpc.XmlRpcException;
import org.apache.xmlrpc.client.XmlRpcClient;

import uni_bonn.pds.Client;
import uni_bonn.pds.RandomOperation;

public class TokenRing implements Runnable {
	
	static Vector vector = new Vector();
	RandomOperation randOp;
	
	static Client client = new Client();
	static XmlRpcClient xmlRpcClient = client.xmlRpcClient;

	long durationOfSeesion = 20000; //milliseconds
	

	private static boolean tokenRingExistance = false;
	private static boolean haveToken = false;
	private static boolean wantToken = false;
	public static URL nextHost = findNextHost();

	public TokenRing(boolean token) {
		if (!tokenRingExistance) {
			tokenRingExistance = true;
			haveToken = token;
			wantToken = false;
		randOp = new RandomOperation();
		}
	}

	private static URL findNextHost() {

		URL currentMachineURL;
		try {
			currentMachineURL = new URL("http://" + Client.currentMachineInfo);

			ArrayList<URL> temp = (ArrayList<URL>) Client.serverURLs.clone();

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

	protected static void startTokenRingAlgorithm() {
		System.out.println("### Token Ring running ###");
		tokenRingExistance = true;
		wantToken = false;
		sendToken();
	}

	protected static void stopTokenRingAlgorithm() {
		System.out.println("### Token Ring stopped ###");
		tokenRingExistance = false;
		wantToken = false;
	}

	/*
	 * protected static void waitForToken() {
	 * System.out.println("### Waiting for receiving token ###"); if
	 * (!tokenRingExistance) {
	 * System.out.println("### Token Ring is not running ###"); return; } else {
	 * wantToken = true; boolean flag = false; while (!flag) synchronized
	 * (instance) { flag = haveToken; } } }
	 */

	protected static void sendToken() {
		if (tokenRingExistance) {
			haveToken = false;
			wantToken = false;

			client.config.setServerURL(nextHost);
			xmlRpcClient.setConfig(client.config);
			try {
				xmlRpcClient.execute("TokenRing.recieveToken", vector);
				// if
				// (result==false){System.err.println("Error! Token wasn't received!");}
			} catch (XmlRpcException e) {
				System.err.println(e.getMessage());
			}
		}
	}

	public void receiveToken() {
		tokenRingExistance = true;
		if (!wantToken)
			sendToken();
		// return ack + 1;
	}

	@Override
	public void run() {
		long timeOfStart = System.currentTimeMillis();
		startTokenRingAlgorithm();
		if (!tokenRingExistance)
			System.err.println("TokenRing is not running!");
		while (tokenRingExistance) {
			if (haveToken) {
				if (wantToken) {
					client.executeForAll( randOp.nextOperation(), randOp.nextValue());
					wantToken = false;
					sendToken();
				} else
					sendToken();
			} else
				while (!haveToken) {
					try {
						Thread.sleep(10);
					} catch (InterruptedException e) {
						System.err.println(e.getMessage());
					}

				}
if ((System.currentTimeMillis()-timeOfStart)>durationOfSeesion)
	stopTokenRingAlgorithm();
		}

	}

}
