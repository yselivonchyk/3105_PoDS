package RicartAndAgrawala;

import java.net.URL;
import java.util.Vector;
import java.util.concurrent.LinkedBlockingQueue;

import uni_bonn.pds.Client;
import uni_bonn.pds.Client.State;
import uni_bonn.pds.Server;

public class RAServer extends Server {
	static Client client = new Client();
	public static volatile LCE logClock = new LCE();
	private static final LinkedBlockingQueue<String> queue = new LinkedBlockingQueue<String>();
	public volatile static int numberOfReplies = 0;

	static Vector<String> emptyParams = new Vector<>();

	public RAServer() { // Why constructor is called so much?
	}

	@Override
	public boolean start() {
		logClock.reset();
		return super.start();
	}

	synchronized public boolean receiveRequest(String IPandPort, int TimeStamp,
			int ID) {

		System.out.println("Request received!");
		// System.err.println("request " + queue.toString());

		logClock.adjustClocks(TimeStamp);
		if ((RAClient.state == State.HELD)
				|| ((RAClient.state == State.WANTED) && (RAClient.request
						.getTimestampAndID() < (TimeStamp * 10 + ID)))) {
			synchronized (queue) {
				queue.add(IPandPort);
			}
			System.err.println("Adding request to a queue!");
			System.out.println("Received: " + (TimeStamp + ID) + " Current: "
					+ RAClient.request.getTimestampAndID());

			// System.err.println("request " + queue.toString());
		} else {
			System.out.println("Received: " + TimeStamp + ID + " Current: "
					+ RAClient.request.getTimestampAndID());
			sendOK(IPandPort);
			// System.err.println(queue.toString());
		}
		// System.err.println("request " + queue.toString());

		return false;
	}

	synchronized public boolean receiveOK() {
		numberOfReplies += 1;
		System.out.println("Ok received! " + numberOfReplies + " out of "
				+ Server.machinesIPs.size());
		// System.err.println("receiveOK " + queue.toString());
		if (numberOfReplies >= machinesIPs.size())
			RAClient.state = State.HELD;

		return true;
	}

	@Override
	public boolean doCalculation(String operation, int value) {
		RAServer.logClock.increase();
		return super.doCalculation(operation, value);
	}

	synchronized public static void sendOKToAll() {

		logClock.increase();
		try {
			for (String str : queue) {
				Client.config
						.setServerURL(new URL("http://" + str + "/xmlrpc"));
				Client.xmlRpcClient.setConfig(Client.config);
				Client.xmlRpcClient.execute("Node.receiveOK", emptyParams);
			}
			queue.clear();

		} catch (Exception e) {
			System.err.println("Error in sendOK static!");
			System.err.println(e.getMessage());
			e.printStackTrace();
		}

	}

	synchronized public void sendOK(String IPandPort) {
		logClock.increase();
		// System.err.println("sendOK " + queue.toString());
		try {
			System.out.println("Sending OK!");
			Client.config.setServerURL(new URL("http://" + IPandPort));
			Client.xmlRpcClient.setConfig(Client.config);
			Client.xmlRpcClient.execute("Node.receiveOK", emptyParams);
		} catch (Exception e) {
			System.err.println(e.getMessage());
			e.printStackTrace();
		}
	}

}
