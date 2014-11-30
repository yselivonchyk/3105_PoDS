package uni_bonn.pds;

import java.net.InetAddress;
import java.net.UnknownHostException;

import org.apache.xmlrpc.XmlRpcException;

import RicardAndAgrawala.RAClient;
import RicardAndAgrawala.RAServer;
import TokenRing.TokenRingClient;
import TokenRing.TokenRingServer;

public class Main {

	public static boolean startedCalculations = true;
	public static boolean standalone = false;
	public static int algorithmType; // 0-R&A 1-TokenRing
	public static RAClient RAC;

	public static void main(String[] args) throws InterruptedException,
			XmlRpcException {

		// ==========================FOR_FIDDLER==================================

		System.setProperty("http.proxyHost", "127.0.0.1");
		System.setProperty("https.proxyHost", "127.0.0.1");
		System.setProperty("http.proxyPort", "8888");
		System.setProperty("https.proxyPort", "8888");
		// ========================================================================

		String memberIPandPort = "localhost:9999";
		algorithmType = 1;

		if (algorithmType != 0) {
			new RAServer().launch();
			RAClient RAC = new RAClient();
			if (!standalone) {
				RAC.join(memberIPandPort);
				Thread.sleep(2000);
				RAC.start(5);
				// RAC.signoff();
			}
		} else {

			new TokenRingServer().launch();
			if (!standalone)
				new TokenRingClient().join(memberIPandPort);
		}

		try {
			System.out.println("Your IP:" + InetAddress.getLocalHost()
					+ "  Port: " + Server.PORT);
		} catch (UnknownHostException e) {
			System.out.println("Error: " + e.getMessage());
		}

	}
}
