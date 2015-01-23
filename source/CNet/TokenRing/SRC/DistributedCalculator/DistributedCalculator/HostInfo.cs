using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistributedCalculator
{
    // the IP address + port number as a identity of a host
    public class HostInfo
    {
        private String IP;
        private int port;

        //initial the values
        public HostInfo(String IP, int port)
        {
            this.IP = IP;
            this.port = port;
        }

        //the parameter is a string of the IP address and port number
        //split with ':', 127.0.0.1:800 for example
        public HostInfo(String IPnPort)
        {
            String[] obj = IPnPort.Split(':');
            this.IP = obj[0];
            this.port = Convert.ToInt32(obj[1]);
        }

        public String getIP()
        {
            return IP;
        }

        public void setIP(String IP)
        {
            this.IP = IP;
        }

        public int getPort()
        {
            return port;
        }

        public void setPort(int port)
        {
            this.port = port;
        }

        public String getIPnPort()
        {
            return this.IP + ":" + this.port;
        }

        public String getFullUrl()
        {
            return "http://" + this.IP + ":" + this.port + "/xmlrpc";
        }

        public String toString()
        {
            return "http://" + this.IP + ":" + this.port + "/";
        }


        // return the host url as an integer 
        public long getHostId()
        {
            String[] parts = this.IP.Split('.');
            String ID = "";
            for (int i = 0; i < parts.Length; i++)
                ID += parts[i];
            ID += port;
            return Convert.ToInt64(ID);
        }

        //compare the IDs of two hosts
        public int compare(HostInfo host2)
        {
            if (this.getHostId() < host2.getHostId())
                return -1;
            if (this.getHostId() == host2.getHostId())
                return 0;
            else
                return 1;
        }

    }
}
