import org.apache.xmlrpc.*;
import org.apache.xmlrpc.client.XmlRpcClient;
import org.apache.xmlrpc.client.XmlRpcClientConfigImpl;

import java.io.*;
import java.net.InetAddress;
import java.net.MalformedURLException;
import java.net.URL;
import java.net.UnknownHostException;
import java.util.ArrayList;
import java.util.Scanner;
import java.util.Vector;

public class Client{

    public static ArrayList<HostInfo> Hosts = new ArrayList<HostInfo>(); //a list of hosts in the ring
    public static HostInfo thisHostInfo; //the IP and port of this host
    private int port; //port of this host
    public static long startTime;  //denotes when this client starts
    public static Boolean startFlag = false; //it's true when this host started
    public static Boolean stopFlag = false; //it's true when this host stopped
	public Vector<Object> params = new Vector<Object>(); //parameter to be sent

    public static XmlRpcClient xmlRpcClient = new XmlRpcClient();
    public static XmlRpcClientConfigImpl config = new XmlRpcClientConfigImpl();
    
    public Client()
    {
    	
    }
    
    public Client(int port) {
        this.port = port;
    }
    
   
    public static HostInfo nextHostOnRing()
    {
    	int i = Hosts.indexOf(thisHostInfo);
    	//make circularity in the ring
    	if(i==-1)
    		return null;
    	else if (i == Hosts.size()-1) //if the i refers to the last one we must return the first one
    		return Hosts.get(0);
    	else
    		return Hosts.get(i+1);	//else we must return next one
    }
    
    public void run(){
    	initialize();
        System.out.println("This host is " + thisHostInfo.toString());
        Hosts.add(thisHostInfo);
        Scanner sc = new Scanner(System.in);
        while (true) {
           System.out.println("-----------------------");
           System.out.println("Your choice:\n 1 - join a network \n 2 - list all hosts in the network \n" +
                    " 3 - start a calculation \n 4 - sign off \n 0 - exit ");
           System.out.println("-----------------------");
           
           int choice = sc.nextInt();
           switch(choice) {
               case 1:
                   join();
                   break; 
               case 2:
                   listHosts();
                   break; 
               case 3:
                   System.out.println("Input an initial value (integer) for the calculation");
                   int initValue = sc.nextInt();
                   start(initValue);
                   //while(startFlag); //Control so that the host who started the algorithm
                                  //doesn't output the interface*/
                   break; 
               case 4:
                   signOff();
                   break;
               case 0:
                   if(Hosts.size()>1)
                   {
                	   System.out.println("You are in a network. You must sign off first!");
                   }
                   else
                   {
                	   System.exit(0);
                   }
                   break;
               default:
                   System.out.println("Wrong input!");
           }
       }
    }

    private void initialize() 
    {
        String ownIP = "127.0.0.1";
    	//get the local IP address
        try {
            ownIP = InetAddress.getLocalHost().toString().split("/")[1];
        } catch (UnknownHostException e) {
            e.printStackTrace();
        }
        thisHostInfo = new HostInfo(ownIP, this.port);
    }

    private void join() {
       System.out.println("Input an IP address and port number (split by :)");
       System.out.println("in the network you want to join:");
       BufferedReader br = new BufferedReader(new InputStreamReader(System.in));
       String IPnPort = null;
       try {
            IPnPort = br.readLine();
       } catch (IOException e) {
    	   System.out.println("IO error!");
    	   System.exit(1);
       }
       HostInfo joinedHost = new HostInfo(IPnPort);
       if(joinedHost.compare(thisHostInfo)==0)
       {
    	   System.out.println("You can't join yourself!");
       }  
       else if(Hosts.size()>1)
       {
    	   System.out.println("You are in a network. Can't join another one!");
       }
       else
       {
    	   try 
           {
           	config.setServerURL(new URL(joinedHost.getFullUrl()));
           } 
           catch (MalformedURLException e) 
           {
               e.printStackTrace();
           }
    	   xmlRpcClient.setConfig(config);
    	   params.removeAllElements();
           try {

               Object[] listOfHosts = (Object[]) xmlRpcClient.execute("Host.getListOfHosts", params);

               for (int i = 0; i < listOfHosts.length; i++) 
               {
               		HostInfo newObj = new HostInfo(listOfHosts[i].toString());
               		int k=0;
               		while( k<Hosts.size() && Hosts.get(k).compare(newObj)<0) k++;                    	
               	    Hosts.add(k,newObj); 
               }
           } catch (XmlRpcException e) {
               e.printStackTrace();
           }
           
           for(HostInfo h:Hosts)
           {
        	   if(!h.equals(thisHostInfo))
        		   joinOverRPC(h.getFullUrl());
           }
       }
    }
    
    private void listHosts()
    { 
    	if(Hosts.size()<2)
    	{
    		System.out.println("There is only one host in the ring " + thisHostInfo.toString());
    	}
    	else{
    		System.out.println("There are " + Hosts.size() + " hosts in the network.");
    		for (int i = 0; i < Hosts.size(); i++) 
    		{
    			System.out.println(Hosts.get(i).toString());//changed
    		}
    		System.out.println("I am the " + Hosts.indexOf(thisHostInfo) + "th host.");
    	}
    }

    private void start(int initValue)
    {
    	int hostPos = Hosts.indexOf(thisHostInfo); 
    	if(Hosts.size()>1)
    	{
    		for(HostInfo h:Hosts)
    		{
    			if(h.equals(thisHostInfo))
    			{
    				startFlag = true;
    			}
    			initOverRPC(initValue, hostPos, h.getFullUrl());
    		}
    		for(HostInfo h:Hosts)
    		{
    			startOverRPC(h.getFullUrl());
    		}
    	}
    	else
    	{
    		System.out.println("Only you are in the network. TokenRing doesn't start!");
    	}
    }   

    private void signOff() {
    	if(Hosts.size()>1)
    	{
            for (HostInfo h:Hosts) 
            {
            	if(!h.equals(thisHostInfo))
            		signOffOverRPC(h.getFullUrl());
            }
            Hosts.clear();
            Hosts.add(thisHostInfo);
        }  else {
            System.out.println("You are not in any network!");
        }
    }

    private void joinOverRPC(String fullURL) 
    {
        try 
        {
        	config.setServerURL(new URL(fullURL));
            xmlRpcClient.setConfig(config);
            params.removeAllElements();
            params.add(thisHostInfo.getIPnPort());
            xmlRpcClient.execute("Host.add", params);
        } catch (XmlRpcException e) 
        {
            System.out.println("XML RPC threw an exception.");
        } catch (MalformedURLException e) {
        	System.out.println("The url was wrong."); 
        }
    }

    private void initOverRPC(int initValue, int hostPos, String fullUrl)
    {
    	 try {
             config.setServerURL(new URL(fullUrl)); 
             xmlRpcClient.setConfig(config);
             params.removeAllElements();
             params.add(initValue);
             params.add(hostPos);
             xmlRpcClient.execute("Host.init", params);
         } catch (XmlRpcException e) {
             e.printStackTrace();
         } catch (MalformedURLException e) {
             e.printStackTrace();
         }
    }
    
    private void startOverRPC(String fullUrl) {
        try {
            config.setServerURL(new URL(fullUrl)); 
            xmlRpcClient.setConfig(config);
            params.removeAllElements();
            xmlRpcClient.execute("Host.start", params);
        } catch (XmlRpcException e) {
            e.printStackTrace();
        } catch (MalformedURLException e) {
            e.printStackTrace();
        }		
	}
    
    private void signOffOverRPC(String fullUrl) {
        try {
            config.setServerURL(new URL(fullUrl));
            xmlRpcClient.setConfig(config);
            params.removeAllElements();
            params.add(thisHostInfo.getIPnPort());
            xmlRpcClient.execute("Host.delete", params);
        } catch (XmlRpcException e) {
            e.printStackTrace();
        } catch (MalformedURLException e) {
            e.printStackTrace();
        }
    }

}
