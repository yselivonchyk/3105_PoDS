import java.util.Date;
import java.util.Vector;


public class Host {

	private static int processValue = 0;

    public Boolean add(String IPnPort)
    {
    	System.out.println("New Host has joined.");
        HostInfo newObj = new HostInfo(IPnPort);
     	int k=0;
     	while( k<Client.Hosts.size() && Client.Hosts.get(k).compare(newObj)<0) k++;                    	
     	Client.Hosts.add(k,newObj); 
    	return true;
    }
    
    public Vector<String> getListOfHosts() 
    {
        System.out.println("Return the list of hosts.");
        Vector<String> result = new Vector<String>();
        for (int i = 0; i < Client.Hosts.size(); i++) 
        {
             result.add(Client.Hosts.get(i).getIPnPort());
        }
        System.out.println("List passed!");
        return result;
    }
    
    public Boolean init(int initValue, int hostPos)
    {
    	processValue = initValue;
    	TokenRing.initializeTokenRing();
    	if(Client.startFlag)
    	{
    		TokenRing.startTokenRingAlgorithm();
    		Client.startTime = System.currentTimeMillis();
    		System.out.println("The algorithm starts at time " + new Date());
    		System.out.println("I started it. The initial token is here. The initial value is " + processValue);
    	}
    	else
    	{
    		Client.startTime = System.currentTimeMillis();
    		System.out.println("The algorithm starts at time " + new Date());
    		System.out.println("Host " + hostPos + " started it. The initial value is " + processValue);
    	}
    	return true;
    }
    
    public Boolean start()
    {
    	new Thread(new TokenRingClient()).start();
    	return true;
    }
    
    public Boolean delete(String IPnPort)
    {
    	System.out.println("A host has signed off");
        int k=0;
     	while( k<Client.Hosts.size() && !Client.Hosts.get(k).getIPnPort().equals(IPnPort)) k++;
     	if(k<Client.Hosts.size())
     		Client.Hosts.remove(k);
     	else
     		System.out.println("You are not in any network!");
        return true;
    }
    
    public Boolean doCalculation(String operation, int param)
    {
    	 switch (operation)
         {
             case "Sum":
                 processValue = processValue + param;
                 break;
             case "Substract":
                 processValue = processValue - param;
                 break;
             case "Multiply":
                 processValue = processValue * param;
                 break;
             case "Divide":
                 processValue = processValue / param;
                 break;
             default:
                 System.out.println("Unknown operation! Pass Over!");
                 break;
         }
         System.out.println(operation + " " + param + " : " + processValue);
         return true;
    }
    
    public int takeTheToken(int ack)
    {
    	return TokenRing.takeTheToken(ack);
    }
    
}
