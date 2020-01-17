using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GatewayCloud.API.DataControl
{
    public class ConnectPrivateAPIController
    {
        private readonly UdpClient _udpClient = new UdpClient();
        private IPEndPoint _serverEp;
        private readonly int _port;
        private readonly ReceiveMessageDelegate _messageDelegate;

        public delegate void ReceiveMessageDelegate(IPEndPoint remoteEP, string message);

        //Checks and connect to the client port
        public ConnectPrivateAPIController(int port, ReceiveMessageDelegate messageDelegate)
        {
            _port = port;
            _serverEp = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
            _udpClient.Connect(_serverEp);
            _messageDelegate = messageDelegate;
        }

        //Send message to the client
        public void SendMessage(string message)
        {
            byte[] responseData = Encoding.UTF8.GetBytes(message);
            _udpClient.Send(responseData, responseData.Length);
        }

        public void StartListening()
        {
            byte[] data;
            while (true)
            {
                try
                {
                    data = _udpClient.Receive(ref _serverEp); 
                    string message = Encoding.UTF8.GetString(data);
                    _messageDelegate(_serverEp, message);
                }
                catch (SocketException)
                {
                    Console.WriteLine("Disconnected.  Will try to reconnect in a moment...");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
