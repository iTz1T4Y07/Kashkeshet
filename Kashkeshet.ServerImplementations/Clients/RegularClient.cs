﻿using Kashkeshet.ServerCore.Abstracts;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kashkeshet.ServerImplementations.Clients
{
    public class RegularClient : ClientBase
    {
        public RegularClient(TcpClient client) : base(client)
        {
        }

        public override Task<bool> UpdateClient()
        {
            throw new NotImplementedException();
        }
    }
}
