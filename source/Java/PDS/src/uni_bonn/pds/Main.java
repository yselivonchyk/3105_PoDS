package uni_bonn.pds;

import java.net.InetAddress;
import java.net.UnknownHostException;

import RicardAndAgrawala.RAClient;
import RicardAndAgrawala.RAServer;
import TokenRing.TokenRingClient;
import TokenRing.TokenRingServer;

public class Main {

	public static boolean startedCalculations =true;
	public static boolean standalone = false;
	public static int algorithmType; // 0-TokenRing 1-R&A

	public static void main(String[] args) {

		String memberIPandPort = "localhost:2222";
		algorithmType = 0;
		
		
		if (algorithmType == 0)
		{        
			new RAServer().launch();
			if(!standalone) new RAClient().join(memberIPandPort);
		}
		else {
			
			new TokenRingServer().launch();
			if(!standalone) new TokenRingClient().join(memberIPandPort);
		}
		
		
		

		try {
			System.out.println("Your IP:" + InetAddress.getLocalHost()
					+ "  Port: " + Server.PORT);
		} catch (UnknownHostException e) {
			System.out.println("Error: " + e.getMessage());
		}

	}
}
