package com.main;

import java.net.URL;
import java.util.Vector;

import org.apache.xmlrpc.client.XmlRpcClient;
import org.apache.xmlrpc.client.XmlRpcClientConfigImpl;

public class Client {
	
	Vector<Object> params; //parameters to be sent to others
	XmlRpcClient client;
	
	public void start() {
		try {
			System.out.println("Setting URL...");
			XmlRpcClientConfigImpl config = new XmlRpcClientConfigImpl();
			config.setServerURL(new URL("http://localhost"+ ":"
					+ Server.PORT));

			System.out.println("Creating XmlRpcClient...");
			client = new XmlRpcClient();

			System.out.println("Setting configuration...");
			client.setConfig(config);

			System.out.println("Creating parameters...");
		
			  params = new Vector<>();
			     params.addElement(new Double(32));
			     params.addElement(new Double(13));
			
			System.out.println("Executing...");
			Object result = client.execute("Server.sum", params);

			double sum = ((Double) result).intValue();
			System.out.println("The sum is: " + sum);

		} catch (Exception exception) {
			System.err.println("Error: " + exception.getMessage());
		}
	}

public boolean join(String memberIP, int memberPort)
{
	return false;
	
	
}

}