using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using Microsoft.Samples.XmlRpc;

namespace CalculationNode
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("XML-RPC PoDS_3101");
            Uri baseAddress = new UriBuilder(Uri.UriSchemeHttp, Environment.MachineName, -1, "/PoDS/").Uri;
            ServiceHost serviceHost = new ServiceHost(typeof(TokenRingCalculator));
            var epXmlRpc = serviceHost.AddServiceEndpoint(typeof(ICalculator), new WebHttpBinding(WebHttpSecurityMode.None), new Uri(baseAddress, "./tokenring"));
            epXmlRpc.Behaviors.Add(new XmlRpcEndpointBehavior());
            serviceHost.Open();

            Console.WriteLine("Token ring listning at {0}", epXmlRpc.ListenUri);

            Uri blogAddress = new UriBuilder(Uri.UriSchemeHttp, Environment.MachineName, -1, "/PoDS/tokenring").Uri;

            ChannelFactory<ICalculator> bloggerAPIFactory = new ChannelFactory<ICalculator>(new WebHttpBinding(WebHttpSecurityMode.None), new EndpointAddress(blogAddress));
            bloggerAPIFactory.Endpoint.Behaviors.Add(new XmlRpcEndpointBehavior());

            ICalculator bloggerAPI = bloggerAPIFactory.CreateChannel();

            Console.WriteLine("Smoke test: " + bloggerAPI.SingOff("smoke"));


            Console.Write("Press ENTER to quit");
            Console.ReadLine();

            serviceHost.Close();
        }
    }
}
