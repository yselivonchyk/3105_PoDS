package uni_bonn.pds;

import java.util.Scanner;
import org.apache.xmlrpc.XmlRpcException;
import RicartAndAgrawala.RAClient;
import RicartAndAgrawala.RAServer;
import TokenRing.TokenRingClient;
import TokenRing.TokenRingServer;

public class Main {

	public static int algorithm = 1; // 1-R&A 0-TokenRing
	public static Client client;

	public static void main(String[] args) throws InterruptedException,
			XmlRpcException {
		Scanner reader = new Scanner(System.in);

		// ==========================FOR_FIDDLER==================================

		// System.setProperty("http.proxyHost", "127.0.0.1");
		// System.setProperty("https.proxyHost", "127.0.0.1");
		// System.setProperty("http.proxyPort", "8888");
		// System.setProperty("https.proxyPort", "8888");

		// ========================================================================

		System.out
				.println("******** Principles of Distributed Systems  WS 14/15 ********\n");
		System.out
				.print("Choose algorithm: 0-Token Ring  1-Ricart and Agrawala \n-> ");
		algorithm = reader.nextInt();

		switch (algorithm) {
		case 0:
			new TokenRingServer().launch();
			break;
		case 1:
			new RAServer().launch();
			break;
		default:
			System.err.println("Wrong character! Exiting...");
			System.exit(0);
		}
		reader = null;
		System.out.println("Node Info:" + Client.currentMachineInfo);

		client = new Client();
		Thread inputThread = new Thread(new UserInputReader(client));
		inputThread.setDaemon(true);
		inputThread.start();

	}
}
