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
    public class TokenRing
    {
        private static TokenRing instance = new TokenRing(); //just for synchronization locking
        private static bool tokenExistance = false;
        private static bool haveToken = false;
        private static bool wantToken = false;

        internal static void initializeTokenRing()
        {
            tokenExistance = true;
            haveToken = false;
            wantToken = false;
        }

        internal static void startTokenRingAlgorithm()
        {
            if(!tokenExistance)
            {
                Console.WriteLine("Token was not initialized. Can't start!");
                Console.ReadKey();
                Environment.Exit(0);
            }
            Console.WriteLine("### Token Ring running ###");
            haveToken = true;
        }

        internal static void stopTokenRingAlgorithm()
        {
            if (!tokenExistance)
            {
                Console.WriteLine("The algorithm didn't start. Wrong!");
                Console.ReadKey();
                Environment.Exit(0);
            }
            Console.WriteLine("### Token Ring stopped ###");
            tokenExistance = false;
            haveToken = false;
            wantToken = false;
        }

        internal static void waitForToken()
        {
            Console.WriteLine("### Waiting for receiving token ###");
            if (!tokenExistance)
            {
                Console.WriteLine("The algorithm didn't start. Wrong!");
                Console.ReadKey();
                Environment.Exit(0);
            }
            wantToken = true;
            bool flag = false;
            while (!flag)
                lock (instance)
                {
                    flag = haveToken;
                }

        }

        internal static void sendToken()
        {
            if (!tokenExistance)
            {
                Console.WriteLine("The algorithm has been stopped. Stop sending token.");
                return;
            }
            haveToken = false;
            wantToken = false;
            TokenSender newClient = new TokenSender(Client.nextHostOnRing());
            Thread oThread1 = new Thread(new ThreadStart(newClient.run));
            oThread1.Start();
        }

        public static int takeTheToken(int ack)
        {
            if (!tokenExistance)
            {
                Console.WriteLine("The algorithm has been stop. Don't take the token.");
                return 0;
            }
            if (wantToken)
            {
                lock (instance)
                {
                    haveToken = true;
                }

                Console.WriteLine("### Receive Token ###");
            }
            else
            {
                sendToken();
            }
            return ack + 1;
        }
    }

    class TokenSender
    {
        private HostInfo nextHostOnRing;

        public TokenSender(HostInfo nextHostOnRing)
        {
            this.nextHostOnRing = nextHostOnRing;
        }
        public void run()
        {
            try
            {
                int ack = 5;
                IServiceContract proxy = XmlRpcProxyGen.Create<IServiceContract>();
                proxy.Url = nextHostOnRing.getFullUrl();
                int respond = proxy.takeTheToken(ack);
                if (respond != ack+1 && respond != 0)
                    Console.WriteLine("Token Ring algorithm has failed.");
                return;
            }
            catch (Exception e)
            {
                Console.WriteLine("Token Ring algorithm has failed.");
                Console.WriteLine(e.Message);
            }
        }
    }
}
