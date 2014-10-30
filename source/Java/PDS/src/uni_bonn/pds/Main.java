package uni_bonn.pds;

import java.net.InetAddress;
import java.net.UnknownHostException;
import java.util.Scanner;

public class Main {

	public static void main(String[] args) {
		Scanner input = new Scanner(System.in);
		System.out.println("Choose Version. 0-Client, 1-Server");
		int choice = input.nextInt();

		String memberIPandPort = "212.201.72.198:1111";

		if (choice != 0)
			new Server().launch();
		else
			new Client().start(memberIPandPort);

		try {
			System.out.println("Your IP:" + InetAddress.getLocalHost()
					+ "  Port: " + Server.PORT);
		} catch (UnknownHostException e) {
			System.out.println("Error: " + e.getMessage());
		}

	}

}
