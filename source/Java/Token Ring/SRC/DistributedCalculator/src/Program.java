import java.io.IOException;
import java.net.ServerSocket;


public class Program {

	public static long maxDuration = 20000;
	public static void main(String[] args) {
		// TODO Auto-generated method stub
		int port = findFreePort();
		Server server = new Server(port);
		Client client = new Client(port);
		server.run();
		client.run();
	}
	
	private static int findFreePort() {
		ServerSocket socket = null;
		try {
			socket = new ServerSocket(0);
			socket.setReuseAddress(true);
			int port = socket.getLocalPort();
			try {
				socket.close();
			} catch (IOException e) {
				// Ignore IOException on close()
			}
			return port;
		} catch (IOException e) {
		} finally {
			if (socket != null) {
				try {
					socket.close();
				} catch (IOException e) {
				}
			}
		}
		throw new IllegalStateException(
				"Could not find a free TCP/IP port");
	}

}
