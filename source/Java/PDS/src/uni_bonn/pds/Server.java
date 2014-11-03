package uni_bonn.pds;

import java.io.IOException;
import java.net.MalformedURLException;
import java.net.URL;
import java.util.ArrayList;
import java.util.List;

import org.apache.xmlrpc.XmlRpcException;
import org.apache.xmlrpc.server.PropertyHandlerMapping;
import org.apache.xmlrpc.server.XmlRpcServer;
import org.apache.xmlrpc.server.XmlRpcServerConfigImpl;
import org.apache.xmlrpc.webserver.WebServer;

import RicardAndAgrawala.RA;
import TokenRing.TokenRing;

public class Server {

	// Holds IP and corresponding of system members
	public static ArrayList<String> machinesIPs = new ArrayList<String>();
	public static final int PORT = 1111; // Use only ports higher than 1000

	/* Creates WebServer and starts it */
	public void launch() {

		// Putting this machine on the list
		try {
			machinesIPs.add(Client.currentMachineInfo);
			Client.serverURLs.add(new URL("http://" + Client.currentMachineInfo));
			
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
			phm.addHandler("Server", Server.class);
			phm.addHandler("Calculator", Calculator.class);
			phm.addHandler("TokenRing", TokenRing.class);
			phm.addHandler("RicardAndAgrawala", RA.class);
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
		// System.out.println("Accepting requests. (Halt program to stop.)");
	}

	/* Server functions */

	/* Joins to network via network member Ip and Port */
	public List<String> join(String newMemberIPandPort) throws MalformedURLException {
		System.out.println("Client is connecting...");

		// Making copy of IPlist
		List<String> tmp = (List) machinesIPs.clone();

		machinesIPs.add(newMemberIPandPort); // adding new member to the list
		Client.serverURLs.add(new URL("http://" + newMemberIPandPort));

		System.out.println("IP:Port=" + newMemberIPandPort);
		return tmp;

	}

	/* Adds newly connected machines */
	public boolean addNewMember(String newMemberInfo) {
		try {

			machinesIPs.add(newMemberInfo);
			Client.serverURLs.add(new URL("http://" + newMemberInfo));
			System.out.println("Client connected! " + newMemberInfo);
		} catch (MalformedURLException e) {
			System.out.println("Error during adding new member!");
		}
		return true;

	}

	/* Receive initial value and start algorithm */
	public void start(int initValue, int algorithmType) {
		Calculator.processingValue = initValue;

		if (algorithmType == 0) {
			System.out.println("Starting TokenRing algorithm...");
			// new Thread(new TokenRing()).run();
		} else {
			System.out.println("Starting Ricard & Agrawala algorithm...");
			// new Thread(new RA()).run();
		}
	}

	/* Leave the network! */
	public boolean signOff(String leavingMachine) {

		for (String str : machinesIPs) {

			if (str.equals(leavingMachine)) {
				machinesIPs.remove(str);
				System.out.println(leavingMachine + " has left!");
				return true;
			}
		}
		return false;

	}

}
