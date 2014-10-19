package com.main;

import java.io.IOException;
import java.net.InetAddress;
import java.net.MalformedURLException;
import java.net.URL;
import java.util.LinkedHashMap;

import org.apache.xmlrpc.XmlRpcException;
import org.apache.xmlrpc.server.PropertyHandlerMapping;
import org.apache.xmlrpc.server.XmlRpcServer;
import org.apache.xmlrpc.server.XmlRpcServerConfigImpl;
import org.apache.xmlrpc.webserver.WebServer;

public class Server {

	// Holds IP and corresponding of system members
	LinkedHashMap<String, URL> otherMachinesIPs;

	public static final int PORT = 88;

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
			otherMachinesIPs = new LinkedHashMap<String, URL>();
		} catch (Exception e) {
			System.err.println("Error: " + e.getMessage());
		}
	}

	/* Server functions */
	public boolean join(String newMemberIP, int newMemberPort) {
		try {
			otherMachinesIPs.put(newMemberIP, new URL("http://" + newMemberIP
					+ ":" + newMemberPort));
			return true;
		} catch (MalformedURLException e) {
			System.out.print("\nRemote machine failde to join");
			System.out.print("\nError: " + e.getMessage());
			return false;
		}

	}

	public boolean signOff(String newMemberIP) {
		if (otherMachinesIPs.remove(newMemberIP) != null) {
			return true;
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
