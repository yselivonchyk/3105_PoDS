package RicartAndAgrawala;

import java.net.URL;
import java.util.LinkedList;
import java.util.concurrent.Callable;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;
import java.util.concurrent.Future;
import java.util.concurrent.locks.Condition;
import java.util.concurrent.locks.ReentrantLock;

import org.apache.xmlrpc.XmlRpcException;
import org.apache.xmlrpc.client.XmlRpcClient;
import org.apache.xmlrpc.client.XmlRpcClientConfigImpl;

import uni_bonn.pds.Client;
import uni_bonn.pds.RandomOperation;

public class RAClient extends Client implements Runnable {

	public volatile static Request request;
	public volatile static State state;
	RandomOperation randomizer;
	LCE logClock = RAServer.logClock;
	long startTime;

	public static ReentrantLock lock = new ReentrantLock(true);
	public static Condition condition = lock.newCondition();

	ExecutorService pool;
	LinkedList<Future<Boolean>> oKs = new LinkedList<Future<Boolean>>();

	public RAClient() {
		pool = Executors.newCachedThreadPool();
		state = State.RELEASED;
		request = new Request();
		randomizer = new RandomOperation();
	}

	public void enterSection() {
		// System.out.println("Entering critical area!");
		lock.lock();
		logClock.increase();
		request.modify(logClock.getCurrentTimeStamp()); // creating new request
		state = State.WANTED;
		lock.unlock();

		// Sending requests to all node
		// and waiting for OKs
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

		lock.lock();
		state = State.HELD;
		lock.unlock();
	}

	public void exitSection() {
		// System.out.println("Exiting critical area!");
		lock.lock();
		state = State.RELEASED;
		condition.signalAll(); // Sending OKs to all waiting nodes
		lock.unlock();
	}

	@Override
	public void run() {
		startTime = System.currentTimeMillis();
		while (SESSION_LENGTH > System.currentTimeMillis() - startTime) {
			try {
				Thread.sleep(randomizer.getRandomWaitingTime());
			} catch (InterruptedException e1) {
				e1.printStackTrace();
			}
			enterSection();
			// System.err.println("Access to critical area obtained!");
			// Executing calculation on all machines
			this.executeForAll("Node.doCalculation",
					randomizer.nextOperationAndValue());
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