package TokenRing;

import java.net.MalformedURLException;
import java.net.URL;
import java.util.ArrayList;
import java.util.Collections;
import java.util.Vector;
import java.util.concurrent.locks.Condition;
import java.util.concurrent.locks.ReentrantLock;

import org.apache.xmlrpc.XmlRpcException;

import uni_bonn.pds.Client;
import uni_bonn.pds.RandomOperation;

public class TokenRingClient extends Client implements Runnable {
	public static URL nextHost;
	public static boolean started;
	static Vector<Object> emptyParams = new Vector<>();
	public static State state;
	RandomOperation randomOperation;
	long startTime;

	public static ReentrantLock lock = new ReentrantLock();
	public static Condition condition = lock.newCondition();

	static URL findNextHost() {
		URL currentMachineURL;
		try {
			currentMachineURL = Client.addressToUrl(currentMachineInfo);
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
		// System.err.println("TokenRingClient constructor");
		/**********************************************************************************/
		// config.setServerURL(nextHost);
		// xmlRpcClient.setConfig(config);
		randomOperation = new RandomOperation();
	}

	@Override
	public void start(int initValue) {
		started = true;
		super.start(initValue);
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
			lock.lock();
			try {
				enterSection();
				while (state != State.HELD) {
					condition.await();
				}

				executeForAll("Node.doCalculation",
						randomOperation.nextOperationAndValue());
				exitSection();

			} catch (InterruptedException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			} finally {
				lock.unlock();
			}
		}

		finalizeSession();
	}

	public static void sendToken() {
		try {
			state = State.RELEASED;
			System.err.println("Sending token to " + nextHost);
			config.setServerURL(nextHost);
			xmlRpcClient.execute("Node.receiveToken", emptyParams);
			// System.err.println("Token is sent to: "+config.getServerURL());
		} catch (XmlRpcException e) {
			System.err.println("Error while sending token!");
			e.printStackTrace();
		}

	}

	@Override
	public void finalizeSession() {
		started = false;
		TokenRingServer.finished = true;
		super.finalizeSession();
	}
}