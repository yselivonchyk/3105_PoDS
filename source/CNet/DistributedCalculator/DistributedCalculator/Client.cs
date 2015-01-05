﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
       
        [XmlRpcMethod("Node.join")] //return the nodes in the network
        Object[] join(String thisHostInfo);

        [XmlRpcMethod("Node.signOff")]
        bool signOff(String leavingMachine);

        [XmlRpcMethod("Node.start")] //start a round of calculation
        bool start(int initValue);

    }

    public class Client
    {
        IServiceContract proxy;
        public static HashSet<String> Nodes = new HashSet<String>();  //store a list of nodes
        
     //   private static int port;    //port of this host

      
        public void run()
        {
            Console.WriteLine("This host is " + Server.thisHostInfo);
            Nodes.Add(Server.thisHostInfo);
            while (true)
            {
                Console.WriteLine("------------------------");
                Console.WriteLine("Your choice:\n 1 - join a network \n 2 - list all nodes in the network \n" +
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
                        listNodes();
                        break;
                    case 3:
                        Console.WriteLine("Input an initial value (integer) for the calculation:");
                        int initValue = int.Parse(Console.ReadLine());
                        start(initValue);
                        break;
                    case 4:
                        signOff();
                        break;
                    case 0:
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Wrong input!");
                        break;
                }
            }

        }

        private void join()
        {
            Console.WriteLine("Input an IP address and port number (split by ':')");
            Console.WriteLine("in the network you want to join:");
            String IPnPort = Console.ReadLine();

            proxy = XmlRpcProxyGen.Create<IServiceContract>();
            proxy.Url = "http://" + IPnPort + "/xmlrpc";

            Object[] listOfNodes = proxy.join(Server.thisHostInfo); //get a list of the nodes currently in the network
            Console.WriteLine("List of nodes received!");
            for (int i = 0; i < listOfNodes.Length; i++)
            {
                String str = (String)listOfNodes[i];
                Nodes.Add(str);
                proxy.Url = "http://" + str + "/xmlrpc";
                proxy.join(Server.thisHostInfo);
            }
        }

        private void listNodes()
        {
            //display all the hosts info in the network 
            if (Nodes.Count < 2)
                Console.WriteLine("There is only one host in the ring " + Server.thisHostInfo);
            else
            {
                Console.WriteLine("There are {0} hosts in the ring.", Nodes.Count);
                foreach(String node in Nodes)
                    Console.WriteLine(node);
            }
        }

        private void start(int initValue)
        {

        }

        private void signOff()
        {
            String[] copy = Nodes.ToArray();
            foreach(String str in copy)
            {
                proxy.Url = "http://" + str + "/xmlrpc";
                proxy.signOff(Server.thisHostInfo);
            }
        }





    }}
