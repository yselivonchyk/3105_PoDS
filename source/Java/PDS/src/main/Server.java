package main;

import java.io.IOException;
import java.net.InetAddress;

import org.apache.xmlrpc.XmlRpcException;
import org.apache.xmlrpc.server.PropertyHandlerMapping;
import org.apache.xmlrpc.server.XmlRpcServer;
import org.apache.xmlrpc.server.XmlRpcServerConfigImpl;
import org.apache.xmlrpc.webserver.*;

public class Server {

	public static final int PORT = 88;

	public void connect() throws XmlRpcException, IOException {

		System.out.println("Attempting to start XML-RPC Server...");
		WebServer webServer = new WebServer(PORT, InetAddress.getByName("0.0.0.0"));
                
		System.out.println("Creating XmlRpcServer...");
		XmlRpcServer xmlRpcServer = webServer.getXmlRpcServer();
	
                
		System.out.println("Creating PropertyHandlerMapping...");
		PropertyHandlerMapping phm = new PropertyHandlerMapping();
                
                
		System.out.println("Adding handler...");
		phm.addHandler("Server", Server.class);
		xmlRpcServer.setHandlerMapping(phm);

		XmlRpcServerConfigImpl serverConfig = (XmlRpcServerConfigImpl) xmlRpcServer.getConfig();
		serverConfig.setEnabledForExtensions(true); 
		serverConfig.setContentLengthOptional(false);
      		webServer.start();

	}

	public void launch() {
		try {
			connect();
			System.out.println("Started successfully.");
			System.out.println("Accepting requests. (Halt program to stop.)");
		} catch (Exception e) {
			System.err.println("Error: " + e.getMessage());
		}
	}

	public Double sum(double x, double y) {
	    return new Double(x+y);
	 }
        public Double sub(double x, double y) {
	    return new Double(x-y);
	 }
        public Double mul(double x, double y) {
	    return new Double(x*y);
	 }
        public Double div(double x, double y) {
	    return new Double(x/y);
	 }

}
