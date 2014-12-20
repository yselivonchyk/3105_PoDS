package TokenRing;

import java.net.MalformedURLException;
import java.net.URL;
import java.util.ArrayList;
import java.util.Collections;
import java.util.Vector;
import uni_bonn.pds.Client;
import uni_bonn.pds.Main;
import uni_bonn.pds.RandomOperation;

public class TokenRingClient extends Client implements Runnable {
	public static URL nextHost;
	static Vector<Object> emptyParams = new Vector<>();

	public static State state = State.RELEASED;
	RandomOperation randomOperation;
	long startTime;

	static URL findNextHost() {
		URL currentMachineURL;
		try {
			currentMachineURL = new URL("http://"
					+ TokenRingClient.currentMachineInfo);
			@SuppressWarnings("unchecked")
			ArrayList<URL> temp = (ArrayList<URL>) TokenRingClient.serverURLs
					.clone();

			URLComparator urlComparator = new URLComparator();
			Collections.sort(temp, urlComparator);
			for (URL next : temp) {
				if (urlComparator.compare(currentMachineURL, next) < 0) {
					return next;
				}
			}
			return temp.get(0);
		} catch (MalformedURLException e) {
			System.err.println(e.getMessage());
			return null;
		}
	}

	public TokenRingClient() {
		System.err.println("TokenRingClient constructor");
		randomOperation = new RandomOperation();
	}

	@Override
	public void start(int initValue) {
		System.err.println("I started TokenRing! Token is mine!");
		state = State.HELD;
		super.start(initValue);
	}

	public void enterSection() {
		System.out.println("Entering critical area!");
		if (state != State.HELD)
			state = State.WANTED;
	}

	public void exitSection() {
		System.out.println("Exiting critical area!");
		state = State.RELEASED;
		TokenRingServer.sendToken();
	}

	public void finilize() {
		executeForAll("Server.finilize", emptyParams);
		System.out.println("FINISHED SESSION!");
		TokenRingServer.finished = true;
	}

	@Override
	public void run() {
		startTime = System.currentTimeMillis();
		while (Main.sessionDuration > System.currentTimeMillis() - startTime) {
			enterSection();
			while (state != State.HELD) {
				try {
					Thread.sleep(200);
				} catch (InterruptedException e) {
					e.printStackTrace();
				}
			}
			executeForAll("Server.doCalculation",
					randomOperation.nextOperationAndValue());
			exitSection();
		}
		finilize();
	}

}
