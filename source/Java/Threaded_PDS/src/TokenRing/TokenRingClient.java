package TokenRing;

import java.net.MalformedURLException;
import java.net.URL;
import java.util.Collections;
import java.util.Comparator;
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
	RandomOperation randomizer = new RandomOperation();
	long startTime;

	public static ReentrantLock lock = new ReentrantLock();
	public static Condition condition = lock.newCondition();

	static URL findNextHost() {
		URL currentMachineURL;
		try {
			currentMachineURL = Client.addressToUrl(currentMachineInfo);
			@SuppressWarnings("unchecked")
			Vector<URL> temp = (Vector<URL>) TokenRingClient.serverURLs.clone();
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
			try {
				Thread.sleep(randomizer.getRandomWaitingTime());
			} catch (InterruptedException e1) {
				e1.printStackTrace();
			}
			lock.lock();
			try {
				enterSection();
				while (state != State.HELD) {
					condition.await();
				}
				executeForAll("Node.doCalculation",
						randomizer.nextOperationAndValue());
				exitSection();
			} catch (InterruptedException e) {
				e.printStackTrace();
			} finally {
				lock.unlock();
			}
		}
		finalizeSession();
	}

	// Sends token to the next node on the ring
	public static void sendToken() {
		try {
			state = State.RELEASED;
			System.err.println("Sending token to " + nextHost);
			config.setServerURL(nextHost);
			xmlRpcClient.execute("Node.receiveToken", emptyParams);
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

class URLComparator implements Comparator<URL> {
	public int compare(URL url1, URL url2) {
		return url1.getAuthority().compareTo(url2.getAuthority());

	}

}
