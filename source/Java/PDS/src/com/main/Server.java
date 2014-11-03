package com.main;

import java.io.IOException;
import java.net.InetAddress;
import java.util.ArrayList;
import java.util.List;

import org.apache.xmlrpc.XmlRpcException;
import org.apache.xmlrpc.server.PropertyHandlerMapping;
import org.apache.xmlrpc.server.XmlRpcServer;
import org.apache.xmlrpc.server.XmlRpcServerConfigImpl;
import org.apache.xmlrpc.webserver.WebServer;

public class Server {

	// Holds IP and corresponding of system members
	public static ArrayList<String> otherMachines = new ArrayList<String>();

	public static final int PORT = 800;

	/* Creates WebServer and starts it */
	public void start() throws XmlRpcException, IOException {

		System.out.println("Attempting to start XML-RPC Server...");
		WebServer webServer = new WebServer(PORT,
				InetAddress.getByName("0.0.0.0"));

		System.out.println("Creating XmlRpcServer...");
		XmlRpcServer xmlRpcServer = webServer.getXmlRpcServer();

		System.out.println("Creating PropertyHandlerMapping...");
		PropertyHandlerMapping phm = new PropertyHandlerMapping();

		System.out.println("Adding handler...");
		phm.addHandler("Server", Server.class);
		xmlRpcServer.setHandlerMapping(phm);

		XmlRpcServerConfigImpl serverConfig = (XmlRpcServerConfigImpl) xmlRpcServer
				.getConfig();
		serverConfig.setEnabledForExtensions(true);
		serverConfig.setContentLengthOptional(false);

		webServer.start();

	}

	public void launch() {
		try {
			start();
			System.out.println("Started successfully.");
			System.out.println("Accepting requests. (Halt program to stop.)");

		} catch (Exception e) {
			System.err.println("Error: " + e.getMessage());
		}
	}

	/* Server functions */
	public List<String> join(String newMemberIPandPort) {
		System.out.println("Client is connecting...");

		List<String> tmp = (List) otherMachines.clone();
		tmp.add(Client.currentMachineIP + ":" + Server.PORT);

		otherMachines.add(newMemberIPandPort);
		System.out.println("IP:Port=" + newMemberIPandPort);

		return tmp;

	}

	public boolean signOff(String leavingMachine) {
		
		for (String str:otherMachines)
		{
			
			if(str.equals(leavingMachine))
			{
				otherMachines.remove(str);
				System.out.println(leavingMachine+" has left!");
				return true;
			}
		}
		return false;

	}

	public Double sum(double x, double y) {
		return new Double(x + y);
	}

	public Double substract(double x, double y) {
		return new Double(x - y);
	}

	public Double multiply(double x, double y) {
		return new Double(x + y);
	}

	public Double divide(double x, double y) {
		if (y == 0)
			return null;
		return new Double(x / y);
	}

}
