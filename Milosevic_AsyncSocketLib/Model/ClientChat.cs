using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Milosevic_AsyncSocketLib.Model
{
    public class ClientChat
    {
        public string Nickname { get; set; }

        public TcpClient Client { get; set; }
    }
}
