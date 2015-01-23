import java.net.MalformedURLException;
import java.net.URL;
import java.util.Date;
import java.util.Random;

import org.apache.xmlrpc.XmlRpcException;


public class TokenRingClient extends Client implements Runnable{

	private static String operation;
	private static int param;
	
	public void run()
	{
		long executeTime;
		int count = 0;
		do{
			count++;
			operation = randomOperation();
			Random rnd = new Random();
			param = rnd.nextInt(10)+1; //generate a random number between 1 and 10
			if(startFlag && count == 1)
			{}
			else
				TokenRing.waitForToken();
			execute(operation, param);
			TokenRing.sendToken();
			int sleepTime = rnd.nextInt(4000-2000)+2000;
			try {
				Thread.sleep(sleepTime);
			} catch (InterruptedException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
			executeTime = System.currentTimeMillis();
			if(executeTime-startTime >= Program.maxDuration)
			{
				System.out.println("Time is up in host " + Hosts.indexOf(thisHostInfo));
				TokenRing.stopTokenRingAlgorithm();
				System.out.println("It ended at time " + new Date());
				stopFlag = true;
				break;
			}
		}while(!stopFlag);
		
		if(startFlag)
			startFlag = false;
		stopFlag = false;
	}
	
	private static String randomOperation()
	{
		Random rnd = new Random();
		int operation = rnd.nextInt(4);
		switch (operation)
        {
            case 0:
                return "Sum";
            case 1:
                return "Substract";
            case 2:
                return "Multiply";
            case 3:
                return "Divide";
            default:
                System.out.println("Generate random operation wrong. Exit!");
                System.exit(0);
                return null;
        }
	}
	
	private void execute(String operation, int param)
	{
		params.removeAllElements();
		params.add(operation);
		params.add(param);
		for(HostInfo h:Hosts)
		{
			try 
	        {
	        	config.setServerURL(new URL(h.getFullUrl()));
	            xmlRpcClient.setConfig(config);
	            xmlRpcClient.execute("Host.doCalculation", params);
	        } catch (XmlRpcException e) 
	        {
	            System.out.println("XML RPC threw an exception.");
	        } catch (MalformedURLException e) {
	        	System.out.println("The url was wrong.");
	        }
		}
	}
}
