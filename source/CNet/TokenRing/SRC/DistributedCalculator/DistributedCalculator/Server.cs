using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CookComputing.XmlRpc;
using System.Collections;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using System.IO;


namespace DistributedCalculator
{
    public interface Operations
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

    class Server : MarshalByRefObject, Operations
    {
        private static int port;
        private int processValue = 0;

        public Server(int _port)
        {
            port = _port;
        }

        public Server()
        {

        }

        public void run()
        {
            IDictionary props = new Hashtable();
            props["name"] = "MyHttpChannel";
            props["port"] = port;
            HttpChannel channel = new HttpChannel(
              props,
              null,
              new XmlRpcServerFormatterSinkProvider()
           );
            ChannelServices.RegisterChannel(channel, false);

            RemotingConfiguration.RegisterWellKnownServiceType(
              typeof(Server),
              "xmlrpc",
              WellKnownObjectMode.Singleton);
        }

        public bool init(int initValue, int hostPos)
        {
            processValue = initValue; //set the processValue in the Server to the initValue
            TokenRing.initializeTokenRing(); //tokenExistance true
            if (Client.startFlag)
            {
                TokenRing.startTokenRingAlgorithm(); //the start client has token
                Client.startTime = DateTime.Now; //the time when the calculation starts
                Console.WriteLine("The algorithm starts at time {0}", Client.startTime);
                Console.WriteLine("I started it. The initial token is here. The initial value is {0}", processValue);
            }
            else
            {
                Client.startTime = DateTime.Now; //the time when the calculation starts
                Console.WriteLine("The algorithm starts at time {0}", Client.startTime);
                Console.WriteLine("Host {1} started it. The initial value is {0}", processValue, hostPos);
            }
            return true;
        }

        public bool start()
        { //generate a thread for each client
            TokenRingClient tokenRingClient = new TokenRingClient();
            new Thread(new ThreadStart(tokenRingClient.start)).Start();
            return true;
        }

        public bool doCalculation(String operation, int param)
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
                    Console.WriteLine("Unknown operation! Pass Over!");
                    break;
            }
            Console.WriteLine(operation + " {0} : {1}", param, processValue);
            return true;
        }

        public bool addHost(String IPnPort)
        {
            Console.WriteLine("New host has joined the network.");
            HostInfo newObj = new HostInfo(IPnPort);
            int k = 0;
            while (k < Client.Hosts.Count && Client.Hosts[k].compare(newObj) < 0) k++;
            Client.Hosts.Insert(k, newObj);
            return true;
        }

        public bool deleteHost(String IPnPort)
        {
            Console.WriteLine("Host has signed off from the network.");
            int k = 0;
            while (k < Client.Hosts.Count && !Client.Hosts[k].getIPnPort().Equals(IPnPort)) k++;
            if (k < Client.Hosts.Count)
            {
                HostInfo obj = Client.Hosts[k];
                Client.Hosts.Remove(obj);
            }
            else
            {
                Console.WriteLine("You are not in any network!");
            }
            return true;
        }

        public Object[] getListOfHosts()
        {
            Console.WriteLine("Return a list of hosts in the existing network.");
            String[] result = new String[Client.Hosts.Count];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = Client.Hosts[i].getIPnPort();
            }
            Console.WriteLine("List of hosts Passed!");
            return result;
        }

        public int takeTheToken(int ack)
        {
            return TokenRing.takeTheToken(ack);
        }
    }
}
