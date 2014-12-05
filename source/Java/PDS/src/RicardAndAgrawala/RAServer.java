package RicardAndAgrawala;

import java.net.URL;
import java.util.TreeMap;
import java.util.Vector;

import uni_bonn.pds.Client;
import uni_bonn.pds.Client.State;
import uni_bonn.pds.Server;

public class RAServer extends Server {
	static Client client = new Client();
	public static final LCE logClock = new LCE();
	private static final TreeMap<String, String> queue = new TreeMap<String, String>();
	public static int numberOfReplies = 0;

	static Vector<String> emptyParams = new Vector<>();

	public RAServer() { // Why constructor is called so much?
	}

	public boolean receiveRequest(String IPandPort, String TimeStamp, String ID) {
		System.out.println("Request received!");
		// System.err.println("request " + queue.toString());
		logClock.adjustClocks(Integer.parseInt(TimeStamp));
		if ((RAClient.state == State.HELD)
				|| ((RAClient.state == State.WANTED) && RAClient.request
						.getTimestampAndID().compareTo(TimeStamp + ID) == -1)) {
			queue.put(TimeStamp + ID, IPandPort);
			System.err.println("Adding request to a queue!");
			// System.err.println("request " + queue.toString());
		} else {
			sendOK(IPandPort);
			// System.err.println(queue.toString());
		}
		// System.err.println("request " + queue.toString());
		return false;
	}

	public boolean receiveOK() {
		numberOfReplies += 1;
		System.out.println("Ok received! " + numberOfReplies + " out of "
				+ Server.machinesIPs.size());
		// System.err.println("receiveOK " + queue.toString());
		return true;
	}

	@Override
	public boolean doCalculation(String operation, String value) {
		RAServer.logClock.increase();
		return super.doCalculation(operation, value);
	}

	public static void sendOK() {
		logClock.increase();
		try {
			while (queue.size() > 0) {
				String key = queue.firstKey();
				Client.config.setServerURL(new URL("http://" + queue.get(key)));
				Client.xmlRpcClient.setConfig(Client.config);
				Client.xmlRpcClient.execute("Server.receiveOK", emptyParams);
				queue.remove(key);
			}
		} catch (Exception e) {
			System.err.println("Error in sendOK static!");
			System.err.println(e.getMessage());
			e.printStackTrace();
		}
	}

	public void sendOK(String IPandPort) {
		RAServer.logClock.increase();
		// System.err.println("sendOK " + queue.toString());
		try {
			System.out.println("Sending OK!");
			Client.config.setServerURL(new URL("http://" + IPandPort));
			Client.xmlRpcClient.setConfig(Client.config);
			Client.xmlRpcClient.execute("Server.receiveOK", emptyParams);
		} catch (Exception e) {
			System.err.println(e.getMessage());
			e.printStackTrace();
		}
	}

}
