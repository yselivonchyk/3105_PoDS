using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.IO;
using CookComputing.XmlRpc;



namespace DistributedCalculator
{
    public interface IServiceContract : IXmlRpcProxy
    {
        [XmlRpcMethod("Host.add")]  //join
        bool addHost(String IPnPort);

        [XmlRpcMethod("Host.delete")] //sign off
        bool deleteHost(String IPnPort);

        [XmlRpcMethod("Host.getListOfHosts")] //return the nodes in the network
        Object[] getListOfHosts();

        [XmlRpcMethod("Host.init")] //host hostPos start the algorithm 
        bool init(int initValue, int hostPos); //inform every host to initialize the algorithm

        [XmlRpcMethod("Host.start")] //inform every host to start running the algorithm
        bool start();

        [XmlRpcMethod("Host.doCalculation")] //do the operation
        bool doCalculation(String operation, int param);

        [XmlRpcMethod("Host.takeTheToken")] //receive the token from the previous host
        int takeTheToken(int ack);

    }

    public class Client
    {
        public static List<HostInfo> Hosts = new List<HostInfo>();
        //store a list of nodes
        protected static HostInfo thisHostInfo;
        private int port;    //port of this host
        public static DateTime startTime;
        public static bool startFlag = false;
        public static bool stopFlag = false;

        IServiceContract proxy = XmlRpcProxyGen.Create<IServiceContract>();

        public Client()
        {

        }

        public Client(int port)
        { 
            this.port = port;
        }

        public static HostInfo nextHostOnRing()
        {
            int i = Hosts.IndexOf(thisHostInfo);
            //make circularity in the ring
            if (i == -1)
                return null;
            else if (i == Hosts.Count - 1) //if the i refers to the last one, return the first one
                return Hosts[0];
            else
                return Hosts[i + 1];	//else return next one
        }
      
        public void run()
        {
            initialize();
            Console.WriteLine("This host is " + thisHostInfo.toString());
            Hosts.Add(thisHostInfo);
           while(true)
            {
                Console.WriteLine("------------------------");

                Console.WriteLine("Your choice:\n 1 - join a network \n 2 - list all hosts in the network \n" +
                        " 3 - start a calculation \n 4 - sign off \n 0 - exit ");
                Console.WriteLine("------------------------");
                Console.WriteLine("Input your choice:");
                
                int choice = int.Parse(Console.ReadLine());
                switch (choice)
                {
                    case 1:
                        join();
                        break;
                    case 2:
                        listHosts();
                        break;
                    case 3:
                        Console.WriteLine("Input an initial value (integer) for the calculation:");
                        int initValue = int.Parse(Console.ReadLine());
                        start(initValue);
                        do{
                            if (!startFlag)
                                break;
                        }while(true); //Control so that the host who started the algorithm
                                      //doesn't output the interface
                        break;
                    case 4:
                        signOff();
                        break;
                    case 0:
                        if (Hosts.Count > 1)
                        {
                            Console.WriteLine("You are in a network. You must sign off first!");
                        }
                        else
                        {
                            Environment.Exit(0);
                        }
                        break;
                    default:
                        Console.WriteLine("Wrong input!");
                        break;
                }
            }

        }

        private void initialize()
        {
            String ownIP = "127.0.0.1";
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            //get the local IP address
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    ownIP = ip.ToString();
                }
            }

            thisHostInfo = new HostInfo(ownIP, this.port);
        }

        private void join()
        {
            Console.WriteLine("Input an IP address and port number (split by :)");
            Console.WriteLine("in the network you want to join:");
            String IPnPort = Console.ReadLine();
            HostInfo joinedNode = new HostInfo(IPnPort); //create an object for the host this local host wants to join
            if (joinedNode.compare(thisHostInfo) == 0)
            {   //check if the local address is input
                Console.WriteLine("You can't join yourself!");
            }
            else
            {
                if (Hosts.Count > 1)
                {
                    Console.WriteLine("You are in a network. Can't join another one!");
                }
                else
                {
                    proxy.Url = joinedNode.getFullUrl();
                    Object[] listOfHosts = proxy.getListOfHosts();
                    //get a list of the nodes currently in the network
                    for (int i = 0; i < listOfHosts.Length; i++)
                    {
                        HostInfo newObj = new HostInfo(listOfHosts[i].ToString());
                        int k = 0;
                        while (k < Hosts.Count && Hosts[k].compare(newObj) < 0) k++;
                        //insert the hosts in the required list into the local list at the right position
                        //ascending order
                        Hosts.Insert(k, newObj);
                    }

                    foreach (HostInfo h in Hosts)
                    {
                        //send join information to each host in the current network now
                        if (!h.Equals(thisHostInfo))
                        {
                            proxy.Url = h.getFullUrl();
                            proxy.addHost(thisHostInfo.getIPnPort());
                        }
                    }
                }
            }
        }

        private void listHosts()
        {
            //display all the hosts info in the network 
            if (Hosts.Count < 2)
                Console.WriteLine("There is only one host in the ring " + thisHostInfo.toString());
            else
            {
                Console.WriteLine("There are {0} hosts in the ring.", Hosts.Count);
                for (int i = 0; i < Hosts.Count; i++)
                    Console.WriteLine("{0}. " + Hosts[i].toString(), i);
                Console.WriteLine("I am the {0}th host.", Hosts.IndexOf(thisHostInfo));
            }
        }

        private void start(int initValue)
        {
            if (Hosts.Count > 1)
            {
                int hostPos = Hosts.IndexOf(thisHostInfo);
                foreach (HostInfo h in Hosts)
                {  //set the initial value
                    if (h.Equals(thisHostInfo))
                    {
                        startFlag = true;  //the client who started the calculation
                    }
                    proxy.Url = h.getFullUrl();
                    proxy.init(initValue, hostPos);                       
                }

                foreach (HostInfo h in Hosts)
                {  //start running the algorithm
                    proxy.Url = h.getFullUrl();
                    proxy.start(); 
                }

            }
            else
            {
                Console.WriteLine("Only you are in the network. TokenRing doesn't start!");
            }
        }

        private void signOff()
        {
            if (Hosts.Count > 1)
            {
                foreach (HostInfo h in Hosts)
                {
                    if (!h.Equals(thisHostInfo))
                    {
                        proxy.Url = h.getFullUrl();
                        proxy.deleteHost(thisHostInfo.getIPnPort());
                    } 
                }
                Hosts.Clear();  //clear all the nodes in the list 
                Hosts.Add(thisHostInfo); //keep itself in the list as initial state
            }
            else Console.WriteLine("You are not in any network!");
            //if there is only itself in the list, cannot sign off
        }

    }
}
