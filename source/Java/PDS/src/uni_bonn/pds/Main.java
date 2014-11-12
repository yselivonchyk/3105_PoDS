package uni_bonn.pds;

import java.net.InetAddress;
import java.net.UnknownHostException;

public class Main {

	public static void main(String[] args) {

		int algorithmType = 0; // 0-TokenRing 1-R&A
		String memberIPandPort = "localhost:1111";

		Server server = new Server();
		Client client = new Client();

		server.launch();
		client.launch(memberIPandPort);

		try {
			System.out.println("Your IP:" + InetAddress.getLocalHost()
					+ "  Port: " + Server.PORT);
		} catch (UnknownHostException e) {
			System.out.println("Error: " + e.getMessage());
		}

	}
}
