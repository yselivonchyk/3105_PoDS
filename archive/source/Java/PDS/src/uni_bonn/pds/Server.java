package uni_bonn.pds;

import java.io.IOException;
import java.net.MalformedURLException;
import java.net.URL;
import java.util.*;

import org.apache.xmlrpc.XmlRpcException;
import org.apache.xmlrpc.webserver.WebServer;
import org.apache.xmlrpc.server.*;

import RicardAndAgrawala.RAClient;
import RicardAndAgrawala.RAServer;
import TokenRing.TokenRingClient;
import TokenRing.TokenRingServer;

public class Server {

	public static long processingValue = 0;
	public static int PORT = 9999; // Use only ports higher than 1024
	public static HashSet<String> machinesIPs = new HashSet<String>();

	/* Creates WebServer and starts it */
	public void launch() {
		/* Putting current machine on the list */
		try {
			machinesIPs.add(Client.currentMachineInfo);
			Client.serverURLs
					.add(new URL("http://" + Client.currentMachineInfo));

		} catch (MalformedURLException e1) {
			System.err.println(e1.getMessage());
		}

		// System.out.println("Attempting to start XML-RPC Server...");
		WebServer webServer = new WebServer(PORT);

		// System.out.println("Creating XmlRpcServer...");
		XmlRpcServer xmlRpcServer = webServer.getXmlRpcServer();

		// System.out.println("Creating PropertyHandlerMapping...");
		PropertyHandlerMapping phm = new PropertyHandlerMapping();

		// System.out.println("Adding handlers...");
		try {
			if (Main.algorithmType == 0)
				phm.addHandler("Server", TokenRingServer.class);
			else
				phm.addHandler("Server", RAServer.class);

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
	public ArrayList<String> join(String newMemberIPandPort) {
		try {
			if (machinesIPs.add(newMemberIPandPort)) {
				Client.serverURLs.add(new URL("http://" + newMemberIPandPort));
				System.out.println("Client is connected!");
				System.out.println("IP:Port=" + newMemberIPandPort);
			}
		} catch (MalformedURLException e) {
			System.err.println("Wrong new member IP and port!");
			System.err.println(e.getMessage());
		}
		ArrayList<String> ips = new ArrayList<String>();
		for (String str : machinesIPs) {
			ips.add(str);
		}

		return ips;
	}

	// /* Joins to network via network member Ip and Port */
	// public Object[] join(String newMemberIPandPort) {
	// try {
	// if (machinesIPs.add(newMemberIPandPort)) {
	// Client.serverURLs.add(new URL("http://" + newMemberIPandPort));
	// System.out.println("Client is connected!");
	// System.out.println("IP:Port=" + newMemberIPandPort);
	// }
	// } catch (MalformedURLException e) {
	// System.err.println("Wrong new member IP and port!");
	// System.err.println(e.getMessage());
	// }
	// return machinesIPs.toArray();
	// }

	/* Receive initial value and start algorithm */
	public boolean start(int initValue) {
		System.out.println("Start calculations!");
		processingValue = initValue;
		if (Main.algorithmType == 0) {
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
			return true;
		}
		return false;
	}

	public boolean doCalculation(String operation, String value) {
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
}
