package uni_bonn.pds;

import java.net.InetAddress;
import java.net.MalformedURLException;
import java.net.URL;
import java.net.UnknownHostException;
import java.util.ArrayList;
import java.util.Vector;

import org.apache.xmlrpc.XmlRpcException;
import org.apache.xmlrpc.client.XmlRpcClient;
import org.apache.xmlrpc.client.XmlRpcClientConfigImpl;

public class Client {
	public enum State {
		RELEASED, WANTED, HELD
	};

	private boolean connectedToNetwork;

	Vector<Object> params = new Vector<Object>();// parameters to be sent to
	public static String currentMachineInfo = machineIP() + ":" + Server.PORT;
	public XmlRpcClient xmlRpcClient;
	public XmlRpcClientConfigImpl config;

	// URLs of other machines
	public static ArrayList<URL> serverURLs = new ArrayList<URL>();

	public Client() {
		connectedToNetwork = false;
		config = new XmlRpcClientConfigImpl();
		System.out.println("Creating XmlRpcClient...");
		xmlRpcClient = new XmlRpcClient();
	}

	public static String machineIP() {
		try {
			return InetAddress.getLocalHost().toString().split("/")[1];
		} catch (UnknownHostException e) {
			System.err.println(e.getMessage());
			return null;
		}
	}

	public void join(String memberIPandPort) {

		System.out.println("Setting URL...");
		try {
			config.setServerURL(new URL("http://" + memberIPandPort));

			System.out.println("Setting configuration...");
			xmlRpcClient.setConfig(config);

			params.removeAllElements();
			params.add(currentMachineInfo);
			try {
				Object[] result = (Object[]) xmlRpcClient.execute(
						"Server.join", params);
				for (Object obj : result) {
					String temp = (String) obj;
					if (Server.machinesIPs.add(temp))
						serverURLs.add(new URL("http://" + temp));
				}
				System.out.println("Successfully connected!\nData received");

				/* Letting other nodes know */
				for (int i = 0; i < serverURLs.size(); i++) {
					config.setServerURL(serverURLs.get(i));
					xmlRpcClient.setConfig(config);
					xmlRpcClient.execute("Server.join", params);
				}
			} catch (XmlRpcException e) {
				System.err.println(e.getMessage());
			}
		} catch (MalformedURLException e1) {
			System.err.println("Wrong remote machine address!!!");
		}
		connectedToNetwork = true;
	}

	public void signoff() throws XmlRpcException {

		/* Telling all machines about leaving */
		if (serverURLs.size() > 1) {
			System.out.println("Signing off...");
			for (URL url : serverURLs) {
				config.setServerURL(url);
				xmlRpcClient.setConfig(config);
				if (!(boolean) xmlRpcClient.execute("Server.signOff", params)) {
					System.out.println("Failed to signOff from " + params);
				}
			}
			System.out.println("Signed off!");
		}

		else {
			System.out.println("You are not connected to network!");
		}
		connectedToNetwork = false;
	}

	public void start(int initValue) {
		Vector<Object> initialValue = new Vector<Object>();
		initialValue.add(initValue);
		executeForAll("Server.start", initialValue);
	}

	public void executeForAll(String methodName, Vector params) {

		for (URL url : serverURLs) {
			xmlRpcClient.setConfig(null);
			config.setServerURL(url);
			xmlRpcClient.setConfig(config);
			// System.out.println("Executing for: " + url.getAuthority());
			try {
				xmlRpcClient.execute(methodName, params);
			} catch (XmlRpcException e) {
				System.err.println(e.getMessage());
			}
		}
	}

	public boolean isConnected() {
		return connectedToNetwork;

	}
}