package uni_bonn.pds;

import java.io.IOException;
import java.net.MalformedURLException;
import java.net.ServerSocket;
import java.net.URL;
import java.util.HashSet;

import org.apache.xmlrpc.XmlRpcException;
import org.apache.xmlrpc.server.PropertyHandlerMapping;
import org.apache.xmlrpc.server.XmlRpcServer;
import org.apache.xmlrpc.server.XmlRpcServerConfigImpl;
import org.apache.xmlrpc.webserver.WebServer;

import RicartAndAgrawala.RAClient;
import RicartAndAgrawala.RAServer;
import TokenRing.TokenRingClient;
import TokenRing.TokenRingServer;

public class Server {

	public volatile static long processingValue = 0;
	public static final int PORT = findFreePort();

	// Holds IPs of network nodes
	public static HashSet<String> machinesIPs = new HashSet<String>();

	/* Creates WebServer and starts it */
	public void launch() {
		/* Putting current machine on the list */
		try {
			machinesIPs.add(Client.currentMachineInfo);
			Client.serverURLs.add(Client
					.addressToUrl(Client.currentMachineInfo));

		} catch (MalformedURLException e1) {
			System.err.println(e1.getMessage());
		}

		// System.out.println("Attempting to start XML-RPC Server...");
		WebServer webServer = new WebServer(PORT);

		// System.out.println("Creating XmlRpcServer...");
		XmlRpcServer xmlRpcServer = webServer.getXmlRpcServer();

		// System.out.println("Creating PropertyHandlerMapping...");
		PropertyHandlerMapping phm = new PropertyHandlerMapping();

		// Adding handlers according chosen algorithm
		try {
			if (Main.algorithm == 0)
				phm.addHandler("Node", TokenRingServer.class);
			else
				phm.addHandler("Node", RAServer.class);

		} catch (XmlRpcException e) {
			System.err.println(e.getMessage());
		}
		xmlRpcServer.setHandlerMapping(phm);

		XmlRpcServerConfigImpl serverConfig = (XmlRpcServerConfigImpl) xmlRpcServer
				.getConfig();
		serverConfig.setEnabledForExtensions(true);
		serverConfig.setContentLengthOptional(false);

		try {
			webServer.start();
		} catch (IOException e) {
			System.err.println(e.getMessage());
		}
		System.out.println("Server started successfully.");
	}

	/******************************************** Server functions ***************************************************************/

	/* Joins to network via network member Ip and Port */
	public Object[] join(String newMemberIPandPort) {
		try {
			if (machinesIPs.add(newMemberIPandPort))
			// Checking if node is new and adding to set
			{
				// Adding node to url list
				Client.serverURLs.add(Client.addressToUrl(newMemberIPandPort));
				System.out.println("Client is connected!");
				System.out.println("IP:Port=" + newMemberIPandPort);
			}
		} catch (MalformedURLException e) {
			System.err.println("Wrong new member IP and port!");
			System.err.println(e.getMessage());
		}
		return machinesIPs.toArray(); // sending array of known nodes
	}

	/* Receive initial value and start algorithm */
	public boolean start(int initValue) {
		processingValue = initValue;
		finishedSessions = 0;
		System.out.println("Start calculations! InitialValue = "
				+ processingValue);
		if (Main.algorithm == 0) {
			System.out.println("Starting TokenRing algorithm...");
			new Thread(new TokenRingClient()).start();
		} else {
			System.out.println("Starting Ricard & Agrawala algorithm...");
			new Thread(new RAClient()).start();
		}
		return true;
	}

	/* Leave the network! */
	public boolean signOff(String leavingMachine) {

		if (machinesIPs.remove(leavingMachine)) {
			System.out.println("Machine " + leavingMachine + " left network!");

			for (int i = 0; i < Client.serverURLs.size(); i++) {
				URL url = Client.serverURLs.get(i);
				if (url.toString().compareTo(leavingMachine) == 0)
					Client.serverURLs.remove(i);
			}

			// System.out.println("MachinesIPS.size " + machinesIPs.size()
			// + " ServerURLs.size " + Client.serverURLs.size());
			return true;
		}

		return false;
	}

	public boolean doCalculation(String operation, int value) {
		// int intValue = Integer.parseInt(value);
		switch (operation) {
		case "sum":
			processingValue += value;
			break;
		case "div":
			processingValue /= value;
			break;
		case "sub":
			processingValue -= value;
			break;
		case "mul":
			processingValue *= value;
			break;
		default:
			System.err.println("Unknown operation in doCalculation!");
			return false;
		}
		Log.logger.info("< " + operation + " >" + " performed with value:"
				+ value + "  PROCESSING_VALUE: " + processingValue + "\n");
		return true;
	}

	public static int finishedSessions;

	public boolean finalizeSession() {
		finishedSessions++;
		System.out.println("Finished received! Machines: " + finishedSessions);
		if (finishedSessions >= machinesIPs.size()) {
			System.out.println("SESSION ENDED! FINAL RESULT: "
					+ processingValue);
		}
		return true;
	}

	private static int findFreePort() {
		ServerSocket socket = null;
		try {
			socket = new ServerSocket(0);
			socket.setReuseAddress(true);
			int port = socket.getLocalPort();
			try {
				socket.close();
			} catch (IOException e) {
				// Ignore IOException on close()
			}
			return port;
		} catch (IOException e) {
		} finally {
			if (socket != null) {
				try {
					socket.close();
				} catch (IOException e) {
				}
			}
		}
		throw new IllegalStateException(
				"Could not find a free TCP/IP port to start Server on");
	}

}
