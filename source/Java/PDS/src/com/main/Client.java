package com.main;

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

	static Vector<Object> params = new Vector<>(); // parameters to be sent to
	public static String currentMachineIP = machineIP();
	static XmlRpcClient client;
	XmlRpcClientConfigImpl config;
	static ArrayList<URL> serverURLs = new ArrayList<URL>();

	public void start(String memberIPandPort) {

		config = new XmlRpcClientConfigImpl();

		System.out.println("Creating XmlRpcClient...");
		client = new XmlRpcClient();

		try {
			join(memberIPandPort);
			
			Thread.sleep(2000);
			
			signoff();
			
		} catch (MalformedURLException e) {
			e.printStackTrace();
		} catch (InterruptedException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} catch (XmlRpcException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}

	}

	public void join(String memberIPandPort) throws MalformedURLException {

		System.out.println("Setting URL...");
		config.setServerURL(new URL("http://" + memberIPandPort));

		System.out.println("Setting configuration...");
		client.setConfig(config);

		params.removeAllElements();
		params.add(currentMachineIP + ":" + Server.PORT);
		try {
			Object[] result = (Object[]) client.execute("Server.join", params);
			for (Object obj : result) {
				String temp = (String) obj;
				Server.otherMachines.add(temp);
				serverURLs.add(new URL("http://" + temp));
				System.out.println("Successfully connected!\nData received");
				System.out.println(Server.otherMachines);

			}

		} catch (XmlRpcException e) {
			System.err.println(e.getMessage());

		}
	}

	public void signoff() throws XmlRpcException{
	
		if (!serverURLs.isEmpty())
		{	
			for (URL url: serverURLs)
			{
				config.setServerURL(url);
				client.setConfig(config);

				System.out.println("Signing off...");
				client.execute("Server.signOff", params);
				System.out.println("Signed off!");
			}
			
		}
		
		else {System.out.println("You are not connected to network!");}
		
	}
	
	public static String machineIP() {
		try {
			return InetAddress.getLocalHost().toString().split("/")[1];
		} catch (UnknownHostException e) {
			System.err.println(e.getMessage());
			return null;
		}

	}
}