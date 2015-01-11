package uni_bonn.pds;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;

import org.apache.xmlrpc.XmlRpcException;

public class UserInputReader implements Runnable {
	Client client;
	BufferedReader in = new BufferedReader(new InputStreamReader(System.in));

	UserInputReader(Client client) {
		this.client = client;
	}

	private void printOptions() {
		System.out.println("Available commands:"
				+ "\n ->join    - Join network"
				+ "\n ->signoff - Sign off network"
				+ "\n ->start   - Give command to start calculation session"
				+ "\n ->list    - Print out list of network members"
				+ "\n ->info    - Information about this node"
				+ "\n ->exit    - Close program");
	}

	public void run() {
		System.out.println("\nUserInputReader daemon started! ");
		printOptions();
		while (true) {
			try {
				userChoiceSwitcher(in.readLine().trim());
			} catch (IOException | XmlRpcException e) {
				System.err.println(e.getMessage());
			}
		}

	}

	private void userChoiceSwitcher(String userDecision) throws IOException,
			XmlRpcException {
		switch (userDecision) {
		case "join":
			System.out.print("Enter member \"IP:Port\" \n-> ");
			String memberInfo = in.readLine().trim();
			client.join(memberInfo);
			break;
		case "signoff":
			client.signoff();
			break;
		case "start":
			System.out.print("Enter initial value -> ");
			try {
				client.start(Integer.parseInt(in.readLine()));
			} catch (NumberFormatException e) {

				System.err.println("Error! " + e.getMessage());
			}
			break;
		case "list":
			client.printListOfNodes();
			break;
		case "info":
			System.out.println("Node Info: " + Client.currentMachineInfo);
			break;
		case "help":
			printOptions();
			break;
		case "exit":
			System.out.println("Exiting...");
			client.signoff();
			System.exit(0);
			break;

		default:
			System.err
					.println("Unknown command! Type \"help\" for commands list");
			return;
		}
	}

}