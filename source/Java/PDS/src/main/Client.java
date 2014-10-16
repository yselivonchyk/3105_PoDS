package main;

import java.net.URL;
import java.util.Vector;

import org.apache.xmlrpc.client.XmlRpcClient;
import org.apache.xmlrpc.client.XmlRpcClientConfigImpl;

public class Client {
	public void start() {
		try {
			System.out.println("Setting URL...");
			XmlRpcClientConfigImpl config = new XmlRpcClientConfigImpl();
			config.setServerURL(new URL("http://localhost"+ ":"
					+ Server.PORT+"/"));

			System.out.println("Creating XmlRpcClient...");
			XmlRpcClient client = new XmlRpcClient();

			System.out.println("Setting configuration...");
			client.setConfig(config);
                        

			System.out.println("Creating parameters...");
		

			  Vector<Double> params = new Vector<Double>();
			     params.addElement(new Double(32));
			     params.addElement(new Double(13));
			
			System.out.println("Executing...");
			Object result = client.execute("Server.mul", params);

			int sum = ((Double) result).intValue();
			System.out.println("The mul is: " + sum);

		} catch (Exception exception) {
			System.err.println("Error: " + exception.getMessage());
		}
	}
}