using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using Blue_Debugger;

namespace ClientSocketDLL
{
    public class ClientConnection
    {
        private TcpClient ClientSocket { get; set; }
        private Debug Debugger { get; set; }

        public ClientConnection(string IP, int Port) //Consructor
        {
            Debugger = new Debug("client_connection_log.txt");
            ClientSocket = new TcpClient();
            try
            {
                ClientSocket.Connect(IP, Port);
                Debugger.StatusMessage("OK", "Connected to " + IP + ":" + Port,true);
            }
            catch (Exception e)
            {
                Debugger.StatusMessage("FAIL", "Couldn't connect to: " + IP + ":" + Port,true);
                Debugger.StatusMessage("FAIL", e.ToString());
            }
        }
        public void Disconnect()
        {
            ClientSocket.Close();
            Debugger.StatusMessage("OK", "Client connection closed");
        }
        public void Send(string toServer)
        {
            try
            {
                NetworkStream serverStream = ClientSocket.GetStream();
                byte[] toSend = Encoding.ASCII.GetBytes(toServer + "$"); //$ als EOF
                serverStream.Write(toSend, 0, toSend.Length);
                serverStream.Flush();
            }
            catch (Exception e)
            {
                Debugger.StatusMessage("FAIL", "Couldn't send " + toServer + " to server",true);
                Debugger.StatusMessage("FAIL", e.ToString(),true);
            }
        }
        public string Receive()
        {
            try
            {
                NetworkStream serverStream = ClientSocket.GetStream();
                byte[] fromServer = new byte[10024];
                serverStream.Read(fromServer, 0, ClientSocket.ReceiveBufferSize);
                serverStream.Flush();
                return Encoding.ASCII.GetString(fromServer);
            }
            catch (Exception e)
            {
                Debugger.StatusMessage("FAIL", "Couldn't receive",true);
                Debugger.StatusMessage("FAIL", e.ToString(),true);
                return null;
            }
        }
    }
}
