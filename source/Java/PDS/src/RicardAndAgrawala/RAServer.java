package RicardAndAgrawala;

import java.util.TreeMap;

import uni_bonn.pds.Server;

public class RAServer extends Server {

	LogicalClock logClock;
	TreeMap<Integer, String> queue;

	public RAServer() {
		logClock = new LogicalClock();

	}

}
