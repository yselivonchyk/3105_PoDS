
import java.net.MalformedURLException;
import java.net.URL;
import org.apache.xmlrpc.XmlRpcException;
import org.apache.xmlrpc.client.XmlRpcClient;
import org.apache.xmlrpc.client.XmlRpcClientConfigImpl;

public final class TokenSender implements Runnable{
	
	private XmlRpcClientConfigImpl config;
	private XmlRpcClient host;
	private Thread clientThread;
	private HostInfo nextNodeOnRing;
	
	public  TokenSender(HostInfo nextNodeOnRing)
	{
		this.nextNodeOnRing = nextNodeOnRing;
		config = new XmlRpcClientConfigImpl(); 
		host = new XmlRpcClient();
		clientThread = new Thread(this);
		clientThread.start();
	}

	@Override
	public void run() {
		try 
		{
			config.setServerURL(new URL(nextNodeOnRing.getFullUrl()));
			host.setConfig(config);
			int ack = 5;
			Object[] params = new Object[]{ack};
	        int respond = (int) host.execute("Host.takeTheToken", params);
	        if(respond != ack+1 && respond != 0)
                 System.out.println("Token Ring algorithm has failed.");
	        return;
		} catch (MalformedURLException e) {
			e.printStackTrace();
		} catch (XmlRpcException e) {
			e.printStackTrace();
		}
	}
	
}
