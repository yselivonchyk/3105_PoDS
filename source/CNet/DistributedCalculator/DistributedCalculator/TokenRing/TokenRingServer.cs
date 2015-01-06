using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CookComputing.XmlRpc;
using DistributedCalculator;

namespace DistributedCalculator.TokenRing
{

    class TokenRingServer : Server
    {
        public bool receiveToken()
        {
            return true;
        }

    }
}
