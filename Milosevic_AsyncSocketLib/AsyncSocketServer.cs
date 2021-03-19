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

            Console.WriteLine("Server in ascolto su IP: {0} - Porta: {1}", mIP.ToString(), mPort.ToString());

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
                string recvText = new string(buff);
                Console.WriteLine("N° byte: {0}. Nickname: {1}", nBytes, recvText);
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
                    Debug.WriteLine("In attesa di un messaggio");
                    //ricezione messaggio asincrono
                    nBytes = await reader.ReadAsync(buff, 0, buff.Length);

                    if (nBytes == 0)
                    {
                        RimuoviClient(client);
                        Debug.WriteLine("Client Disconnesso");
                        break;
                    }
                    string recvText = new string(buff);
                    Debug.WriteLine("N° byte: {0}. Messaggio: {1}", nBytes, recvText);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Errore: " + ex.Message);
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
                foreach (TcpClient client in mClients)
                {
                    byte[] buff = Encoding.ASCII.GetBytes(messaggio);
                    client.GetStream().WriteAsync(buff, 0, buff.Length);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Errore: " + ex.Message);
            }
        }

        public void Disconnetti()
        {
            try
            {
                foreach (TcpClient client in mClients)
                {
                    client.Close();
                    RimuoviClient(client);
                }
                mServer.Stop();
                mServer = null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Errore: " + ex.Message);
            }
        }
    }
}