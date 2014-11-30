package RicardAndAgrawala;

import java.net.URL;
import java.util.TreeMap;
import java.util.Vector;

import uni_bonn.pds.Client;
import uni_bonn.pds.Client.State;
import uni_bonn.pds.Log;
import uni_bonn.pds.Server;

public class RAServer extends Server {
	static Client client = new Client();
	public static LCE logClock;
	public static TreeMap<String, String> queue;
	public static int numberOfReplies = 0;

	static Vector<String> emptyParams = new Vector<>();

	public RAServer() { // Why constructor is called so much?
		logClock = new LCE();
		queue = new TreeMap<String, String>();
	}

	@Override
	public Object[] join(String newMemberIPandPort) {
		Object[] temp = super.join(newMemberIPandPort);
		LCE.machineID = temp.length;
		return temp;
	}

	public boolean receiveRequest(String IPandPort, String TimeStamp, String ID) {
		System.out.println("Request received!");
		logClock.adjustClocks(Integer.parseInt(TimeStamp));
		if ((RAClient.state == State.HELD)
				|| ((RAClient.state == State.WANTED) && RAClient.request
						.getTimestampAndID().compareTo(TimeStamp + ID) == 1)) {
			queue.put(TimeStamp + ID, IPandPort);
		} else {
			sendOK(IPandPort);
		}
		return false;
	}

	public boolean receiveOK() {
		numberOfReplies += 1;
		;
		System.out.println("Ok received! " + numberOfReplies + " out of "
				+ this.machinesIPs.size());
		return true;
	}

	public boolean doCalculation(String operation, String value) {
		this.logClock.increase();
		int intValue = Integer.parseInt(value);
		switch (operation) {
		case "sum":
			processingValue += intValue;
			break;
		case "div":
			processingValue /= intValue;
			break;
		case "sub":
			processingValue -= intValue;
			break;
		case "mul":
			processingValue *= intValue;
			break;
		default:
			System.err.println("Unknown operation in doCalculation!");
			return false;
		}
		Log.logger.info("< " + operation + " >" + " performed with value:"
				+ value);
		return true;
	}

	public static void sendOK() {
		logClock.increase();
		try {
			if (queue.size() > 0) {
				String key = queue.firstKey();
				client.config.setServerURL(new URL(key));
				client.xmlRpcClient.setConfig(client.config);
				client.xmlRpcClient.execute("Server.receiveOK", emptyParams);
				queue.remove(key);
			}
		} catch (Exception e) {
			System.err.println("Error in sendOK static!");
			System.err.println(e.getMessage());
			e.printStackTrace();
		}
	}

	public void sendOK(String IPandPort) {
		this.logClock.increase();
		try {
			System.out.println("Sending OK!");
			client.config.setServerURL(new URL("http://" + IPandPort));
			client.xmlRpcClient.setConfig(client.config);
			client.xmlRpcClient.execute("Server.receiveOK", emptyParams);
		} catch (Exception e) {
			System.err.println(e.getMessage());
			e.printStackTrace();
		}
	}

}
