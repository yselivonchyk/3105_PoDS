package uni_bonn.pds;

import java.net.InetAddress;
import java.net.UnknownHostException;
import java.util.Scanner;

import org.apache.xmlrpc.XmlRpcException;

import RicartAndAgrawala.LCE;
import RicartAndAgrawala.RAClient;
import RicartAndAgrawala.RAServer;
import TokenRing.TokenRingClient;
import TokenRing.TokenRingServer;

public class Main {

	public static final int sessionDuration = 20000;
	public static boolean standalone = false;
	public static int algorithmType = 1; // 1-R&A 0-TokenRing
	public static Client client;

	public static void main(String[] args) throws InterruptedException,
			XmlRpcException {
		// Scanner reader = new Scanner(System.in);
		// ==========================FOR_FIDDLER==================================

		System.setProperty("http.proxyHost", "127.0.0.1");
		System.setProperty("https.proxyHost", "127.0.0.1");
		System.setProperty("http.proxyPort", "8888");
		System.setProperty("https.proxyPort", "8888");

		// ========================================================================
		/***************************************************** For_Testing **************************************************************/

		String memberIPandPort = "localhost:28600";

		if (algorithmType != 0) {
			new RAServer().launch();
			client = new RAClient();
			if (!standalone) {
				client.join(memberIPandPort);
				Thread.sleep(2000);
				client.start(5);
				// RAC.signoff();
			}
		} else {
			new TokenRingServer().launch();
			client = new TokenRingClient();
			if (!standalone) {
				client.join(memberIPandPort);
				Thread.sleep(2000);
				client.start(5);
				// TRC.signoff();
			}
		}
		try {
			System.out.println("Your IP:" + InetAddress.getLocalHost()
					+ "  Port: " + Server.PORT + "  ID: " + LCE.machineID);
		} catch (UnknownHostException e) {
			System.out.println("Error: " + e.getMessage());
		}
		/**************************************************
		 * ***********************************************
		 */

		// int userDecision = -1;
		// System.out
		// .println("Choose algorithm: 0-Token Ring  1-Ricart and Agrawala");
		// algorithmType = reader.nextInt();
		// switch (algorithmType) {
		// case 0:
		// new TokenRingServer().launch();
		// break;
		// case 1:
		// new RAServer().launch();
		// break;
		// default:
		// System.err.println("Wrong character! Exiting...");
		// return;
		// }
		//
		// try {
		// System.out.println("Node IP:"
		// + InetAddress.getLocalHost().toString().split("/")[1]
		// + "  Port: " + Server.PORT);
		// } catch (UnknownHostException e) {
		// System.out.println("Error: " + e.getMessage());
		// }
		//
		// if (algorithmType == 0)
		// client = new TokenRingClient();
		// else
		// client = new RAClient();
		//
		// System.out.println("0-Join  1-Sign off  2-Start  3-Standalone mode");
		// userDecision = reader.nextInt();
		// switch (userDecision) {
		// case 0:
		// System.out.println("Enter member \"IP:Port\"");
		// reader.nextLine();
		// String memberInfo = reader.nextLine();
		// client.join(memberInfo);
		// break;
		// case 1:
		// client.signoff();
		// break;
		// case 2:
		// System.out.print("Enter initial value: ");
		// client.start(reader.nextInt());
		// break;
		//
		// case 3:
		// System.out.println("Waiting to other nodes...");
		// break;
		//
		// default:
		// System.err.println("Wrong character! Exiting...");
		// return;
		// }

	}
}
