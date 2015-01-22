package uni_bonn.pds;

import org.apache.xmlrpc.XmlRpcException;
import RicartAndAgrawala.RAClient;
import RicartAndAgrawala.RAServer;
import TokenRing.TokenRingClient;

public class Main {

	public static int algorithm = 1;// 1-R&A 0-TokenRing
	public static Client client;

	public static void main(String[] args) throws InterruptedException,
			XmlRpcException {

		// Scanner reader = new Scanner(System.in);
		System.out
				.println("******** Principles of Distributed Systems  WS 14/15 ********\n");
		// System.out
		// .print("Choose algorithm: 0-Token Ring  1-Ricart and Agrawala \n-> ");
		// algorithm = reader.nextInt();

		// Launching server for chosen algorithm
		// switch (algorithm) {
		// case 0:
		// new TokenRingServer().launch();
		// break;
		// case 1:
		new RAServer().launch();
		// break;
		// default: // exiting in case of wrong input
		// System.err.println("Wrong character! Exiting...");
		// System.exit(0);
		// }
		// reader = null; // We don't need scanner anymore!
		// Input reading thread will be used instead

		System.out.println("Node Info: " + Client.currentMachineInfo);

		// Creating client object to main client commands (join, signOff and
		// etc.)

		if (algorithm == 0)
			client = new TokenRingClient();
		else
			client = new RAClient();

		// Creating and startinf input reader thread
		Thread inputThread = new Thread(new UserInputReader(client));
		inputThread.setDaemon(true); // Thread will stop when there will be no
										// other threads alive
		inputThread.start();

	}
}
