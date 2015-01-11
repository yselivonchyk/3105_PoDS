package uni_bonn.pds;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.util.Scanner;

import org.apache.xmlrpc.XmlRpcException;

import RicartAndAgrawala.RAClient;
import RicartAndAgrawala.RAServer;
import TokenRing.TokenRingClient;
import TokenRing.TokenRingServer;

public class Main {

	public static int algorithmType = 1; // 1-R&A 0-TokenRing
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
		algorithmType = reader.nextInt();

		switch (algorithmType) {
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

		if (algorithmType == 0)
			client = new TokenRingClient();
		else
			client = new RAClient();
		Thread inputThread = new Thread(new UserInputReader(client));
		inputThread.setDaemon(true);
		inputThread.start();

	}
}


