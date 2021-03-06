using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using Milosevic_AsyncSocketLib.Model;

namespace Milosevic_AsyncSocketLib
{
    public class AsyncSocketServer
    {
        IPAddress mIP;
        int mPort;
        TcpListener mServer;
        List<ClientChat> mClients;

        public AsyncSocketServer()
        {
            mClients = new List<ClientChat>();
        }

        //Server inizia as ascoltare
        public async void InAscolto(IPAddress ipaddr = null, int port = 23000)
        {
            //controlli generali
            if (ipaddr == null)
            {
                ipaddr = IPAddress.Any;
            }

            if (port < 0 || port > 65535)
            {
                port = 23000;
            }

            mIP = ipaddr;
            mPort = port;

            mServer = new TcpListener(mIP, mPort);
            mServer.Start();
            Console.WriteLine("Server avviato.");

            while (true)
            {
                TcpClient client = await mServer.AcceptTcpClientAsync();
                RegistraClient(client);
                //Console.WriteLine("Client connessi: {0}. Client connesso: {1}", mClients.Count, client.Client.RemoteEndPoint);
                RiceviMessaggio(client);
            }
        }

        public async void RegistraClient(TcpClient client)
        {
            NetworkStream stream = null;
            StreamReader reader = null;

            try
            {
                stream = client.GetStream();
                reader = new StreamReader(stream);
                char[] buff = new char[512];
                int nBytes = 0;

                Console.WriteLine("In attesa di Nickname...");
                //ricezione del nickname
                nBytes = await reader.ReadAsync(buff, 0, buff.Length);
                string recvText = new string(buff, 0, nBytes);
                Console.WriteLine("Nickname: {0}", recvText);
                ClientChat newclient = new ClientChat();
                newclient.Nickname = recvText;
                newclient.Client = client;

                mClients.Add(newclient);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Errore: " + ex.Message);
            }
        }

        public async void RiceviMessaggio(TcpClient client)
        {
            NetworkStream stream = null;
            StreamReader reader = null;

            try
            {
                stream = client.GetStream();
                reader = new StreamReader(stream);
                char[] buff = new char[512];
                int nBytes = 0;

                while (true)
                {
                    Console.WriteLine("In attesa di un messaggio");
                    //ricezione messaggio asincrono
                    nBytes = await reader.ReadAsync(buff, 0, buff.Length);

                    if (nBytes == 0)
                    {
                        RimuoviClient(client);
                        Console.WriteLine("Client Disconnesso");
                        break;
                    }
                    string recvText = new string(buff, 0, nBytes);

                    ClientChat nickClient = mClients.Where( e => e.Client == client).FirstOrDefault();
                    string risp = $"{nickClient.Nickname}: {recvText}";

                    InviaTutti(risp);
                }
            }
            catch (Exception ex)
            {
                RimuoviClient(client);
                Console.WriteLine("Errore: " + ex.Message);
            }
        }

        private void RimuoviClient(TcpClient client)
        {
            ClientChat nm = mClients.Where(riga => riga.Client == client).FirstOrDefault();

            if (nm != null)
            {
                mClients.Remove(nm);
            }
        }

        public void InviaTutti(string messaggio)
        {
            try
            {
                foreach (ClientChat client in mClients)
                {
                    byte[] buff = Encoding.ASCII.GetBytes(messaggio);
                    client.Client.GetStream().WriteAsync(buff, 0, buff.Length);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Errore: " + ex.Message);
            }
        }

        public void Disconnetti()
        {
            try
            {
                foreach (ClientChat client in mClients)
                {
                    client.Client.Close();
                    RimuoviClient(client.Client);
                }
                mServer.Stop();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Errore: " + ex.Message);
            }
        }
    }
}