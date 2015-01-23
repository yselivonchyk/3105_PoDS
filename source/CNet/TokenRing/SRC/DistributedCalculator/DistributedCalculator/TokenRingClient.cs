using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CookComputing.XmlRpc;
using System.Collections;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using System.IO;

namespace DistributedCalculator
{
    class TokenRingClient: Client
    {
        private static String operation; //store the generated operation
        private static int param; //store the generated parameter
  
        IServiceContract proxy = XmlRpcProxyGen.Create<IServiceContract>();

        public void start()
        {
            DateTime executeTime;
            int count = 0;
            do
            {
                count++;  //the number of operations that this host generated
                operation = randomOperation(ref param); //generate a random operation with a random parameter
                if (startFlag && count == 1)
                {  //control the output, for the host who starts the algorithm
                }  //executing for the first time, don't wait, start directly
                else
                    TokenRing.waitForToken();
                execute(operation, param);
                TokenRing.sendToken();
                Random rnd = new Random();
                int sleepTime = rnd.Next(2000, 4001); //generate a random waiting time between 2s and 4s
                System.Threading.Thread.Sleep(sleepTime);
                executeTime = DateTime.Now; //the time when a random operation starts
                if (TimeSpan.Compare(executeTime - startTime, Program.maxDuration) >= 0)
                {
                    Console.WriteLine("Time is up in host {0}.", Hosts.IndexOf(thisHostInfo));
                    TokenRing.stopTokenRingAlgorithm();
                    Console.WriteLine("It ended at time {0}.", executeTime);
                    stopFlag = true; //set the stop falg to true
                    break;
                }
            } while (!stopFlag);

            if (startFlag)
                startFlag = false;
            stopFlag = false; //set the default value back
            //so that a new session runs correctly
        }

        public void execute(String operation, int param)
        {
            foreach (HostInfo h in Hosts)
            {
                proxy.Url = h.getFullUrl();
                proxy.doCalculation(operation, param);
            }

        }

        public static String randomOperation(ref int param)
        {
            Random rnd = new Random();
            param = rnd.Next(1, 11); //generate a random parameter between 1 to 10 as another operand
            int operation = rnd.Next(0, 3); //generate an integer between 0 to 3
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
                    Console.WriteLine("generate random operation wrong. Exit!");
                    Environment.Exit(0);
                    return null;
            }
        }

    }
}
