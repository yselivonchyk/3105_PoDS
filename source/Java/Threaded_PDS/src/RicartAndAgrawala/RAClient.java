package RicartAndAgrawala;

import java.net.URL;
import java.util.LinkedList;
import java.util.concurrent.Callable;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;
import java.util.concurrent.Future;

import org.apache.xmlrpc.XmlRpcException;
import org.apache.xmlrpc.client.XmlRpcClient;
import org.apache.xmlrpc.client.XmlRpcClientConfigImpl;

import uni_bonn.pds.Client;
import uni_bonn.pds.RandomOperation;

public class RAClient extends Client implements Runnable {

	public volatile static Request request;
	public volatile static State state;
	RandomOperation randomOperation;
	LCE logClock = RAServer.logClock;
	long startTime;

	ExecutorService pool;
	LinkedList<Future<Boolean>> oKs = new LinkedList<Future<Boolean>>();

	public RAClient() {
		pool = Executors.newCachedThreadPool();
		state = State.RELEASED;
		request = new Request();
		randomOperation = new RandomOperation();
	}

	@Override
	public void join(String memberIPandPort) {
		super.join(memberIPandPort);
		LCE.machineID = Client.serverURLs.size();
	}

	public synchronized void enterSection() {
		System.out.println("Entering critical area!");

		state = State.WANTED;

		request.modify(logClock.getCurrentTimeStamp());

		for (URL url : serverURLs)
			oKs.add(pool.submit(new RequestSender(url, request)));
		boolean notListen = false;
		while (!notListen) {
			notListen = true;
			for (Future<Boolean> result : oKs) {
				notListen = notListen && result.isDone();
			}
		}
		oKs.clear();

		state = State.HELD;
	}

	synchronized public void exitSection() {
		System.out.println("Exiting critical area!");
		state = State.RELEASED;
	}

	@Override
	public void run() {
		startTime = System.currentTimeMillis();
		while (SESSION_LENGTH > System.currentTimeMillis() - startTime) {

			enterSection();
			System.err.println("Access to critical area obtained!");
			this.executeForAll("Node.doCalculation",
					randomOperation.nextOperationAndValue());
			exitSection();
		}
		finalizeSession();
	}
}

class RequestSender implements Callable<Boolean> {
	XmlRpcClientConfigImpl config = new XmlRpcClientConfigImpl();
	XmlRpcClient xmlRpcClient = new XmlRpcClient();
	URL url;
	Request req;

	public RequestSender(URL url, Request req) {
		this.url = url;
		this.req = req;
	}

	public Boolean call() {
		config.setServerURL(url);
		xmlRpcClient.setConfig(config);
		try {
			return (Boolean) xmlRpcClient.execute("Node.receiveRequest",
					req.getParams());
		} catch (XmlRpcException e) {
			System.err.println(e.getMessage());
		}
		return false;
	}
}
