package uni_bonn.pds;

import java.net.InetAddress;
import java.net.MalformedURLException;
import java.net.URL;
import java.net.UnknownHostException;
import java.util.Vector;

import org.apache.xmlrpc.XmlRpcException;
import org.apache.xmlrpc.client.XmlRpcClient;
import org.apache.xmlrpc.client.XmlRpcClientConfigImpl;

public class Client {
	public enum State {
		RELEASED, WANTED, HELD
	};

	public final long SESSION_LENGTH = 20000;
	Vector<Object> params = new Vector<Object>();// parameters to be sent to
	public static String currentMachineInfo = urlFormatter(machineIP() + ":"
			+ Server.PORT);
	public static XmlRpcClient xmlRpcClient;
	public static XmlRpcClientConfigImpl config;

	// URLs of other machines
	public static Vector<URL> serverURLs = new Vector<URL>();

	public Client() {
		config = new XmlRpcClientConfigImpl();
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
		try {
			config.setServerURL(addressToUrl(urlFormatter(memberIPandPort)));
			xmlRpcClient.setConfig(config);
			params.removeAllElements();
			params.add(currentMachineInfo);
			try {
				Object[] result = (Object[]) xmlRpcClient.execute("Node.join",
						params);
				for (Object obj : result) {
					String temp = (String) obj;
					if (Server.machinesIPs.add(temp))
						serverURLs.add(addressToUrl(temp));
				}
				System.out.println("Successfully connected!");

				/* Letting other nodes know */
				for (int i = 0; i < serverURLs.size(); i++) {
					config.setServerURL(serverURLs.get(i));
					xmlRpcClient.setConfig(config);
					xmlRpcClient.execute("Node.join", params);
				}
			} catch (XmlRpcException e) {
				System.err.println(e.getMessage());
			}
		} catch (MalformedURLException e1) {
			System.err.println("Wrong remote machine address!!!");
		}
	}

	public void signOff() throws XmlRpcException {
		Vector<Object> machineInfo = new Vector<Object>();
		machineInfo.add(currentMachineInfo);

		/* Telling all machines about leaving */
		if (serverURLs.size() > 1) {
			System.out.println("Signing off...");
			URL[] a = new URL[serverURLs.size()];
			a = serverURLs.toArray(a);
			for (URL url : a) {
				config.setServerURL(url);
				xmlRpcClient.setConfig(config);
				if (!(boolean) xmlRpcClient
						.execute("Node.signOff", machineInfo)) {
					System.out.println("Failed to signOff from "
							+ url.getAuthority());
				}
			}
			System.out.println("Signed off!");
		} else {
			System.out.println("You are not connected to network!");
		}
	}

	public void start(int initValue) {
		if (serverURLs.size() > 1) {
			Vector<Object> parameters = new Vector<Object>();
			parameters.add(initValue);
			executeForAll("Node.start", parameters);
		} else
			System.out.println("You are not connected to network!");
	}

	/* Function for multicasting */
	synchronized public void executeForAll(String methodName,
			Vector<Object> params) {

		for (URL url : serverURLs) {
			config.setServerURL(url);
			xmlRpcClient.setConfig(config);
			try {
				xmlRpcClient.execute(methodName, params);
			} catch (XmlRpcException e) {
				System.err.println(e.getMessage());
			}
		}
	}

	public void finalizeSession() {
		params.removeAllElements();
		executeForAll("Node.finalizeSession", params);
	}

	public void printListOfNodes() {
		System.out.println("There are " + serverURLs.size()
				+ " network members:");
		for (URL url : serverURLs) {
			System.out.println(url.getAuthority());
		}
	}

	public static URL addressToUrl(String address) throws MalformedURLException {
		if (!address.contains("http://"))
			address = "http://" + address;
		if (!address.contains("/xmlrpc"))
			address = address + "/xmlrpc";
		return new URL(address);
	}

	public static String urlFormatter(String ipAndPort) {
		return "http://".concat(ipAndPort).concat("/xmlrpc");
	}
}