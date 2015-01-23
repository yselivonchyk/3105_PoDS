
public class HostInfo {
	private String IP;
	private int port;
	
	public String getIP() {
		return IP;
	}
	
	public void setIp(String IP) {
		this.IP = IP;
	}
	
	public int getPort() {
		return port;
	}
	
	public void setPort(int port) {
		this.port = port;
	}
	
	public HostInfo(String IP, int port) {
		super();
		this.IP = IP;
		this.port = port;
	}
	
	public HostInfo(String IPnPort) {
		super();
		String[] obj = IPnPort.split(":");
		this.IP = obj[0];
		this.port = Integer.parseInt(obj[1]);
	}
	
	public String getIPnPort()
	{
		return this.IP+":"+this.port;
	}
	
	public String getFullUrl()
	{
		return "http://"+this.IP+":"+this.port+"/xmlrpc";
	}
	
	@Override
	public String toString() {
		return "http://"+this.IP+":"+this.port+"/";
	}
	
	public long getHostId()
	{
		String[] parts = this.IP.split("\\.");
		String ID = "";
		for(int i=0;i<parts.length;i++)
			ID += parts[i];
		ID += port;
		return Long.parseLong(ID);
	}
	
	public int compare(HostInfo obj2)
	{
		if(this.getHostId()<obj2.getHostId())
			return -1;
		if(this.getHostId()==obj2.getHostId())
			return 0;
		else
			return 1;
	}
}
