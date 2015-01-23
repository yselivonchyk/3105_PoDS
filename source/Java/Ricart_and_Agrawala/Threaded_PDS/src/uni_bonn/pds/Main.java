package uni_bonn.pds;

import org.apache.xmlrpc.XmlRpcException;

import RicartAndAgrawala.RAClient;
import RicartAndAgrawala.RAServer;

public class Main {
	public static Client client;

	public static void main(String[] args) throws InterruptedException,
			XmlRpcException {

		System.out
				.println("******** Principles of Distributed Systems  WS 14/15 ********\n");

		// Launching server for chosen algorithm
		new RAServer().launch();
		System.out.println("Node Info: " + Client.currentMachineInfo);

		// Creating client object to main client commands (join, signOff and
		// etc.)
		client = new RAClient();

		// Creating and startinf input reader thread
		Thread inputThread = new Thread(new UserInputReader(client));
		inputThread.setDaemon(true); // Thread will stop when there will be no
										// other threads alive
		inputThread.start();

	}
}
