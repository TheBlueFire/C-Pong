using System;
using System.Threading;
using System.Net.Sockets;
using System.Text;
using System.Runtime.InteropServices;
using Blue_Debugger;

namespace ConsoleApplication1
{
    class Server
    {
        private static TcpListener serverSocket { get; set; }
        private static TcpClient clientSocket { get; set; }
        private static Debug Debugger { get; set; }
        static void Main(string[] args)
        {
            //Init-stuff
            Debugger = new Debug("server_log.txt");

            serverSocket = new TcpListener(8888);
            clientSocket = default(TcpClient);

            int counter = 0;
            serverSocket.Start();
            Debugger.StatusMessage("OK", "Server Started");
            Debugger.StatusMessage("INFO", "Waiting for connections...");

            counter = 0;
            while (true)
            {
                counter += 1;
                clientSocket = serverSocket.AcceptTcpClient();
                Debugger.StatusMessage("OK", "New Client. Number: " + counter);

                Client client = new Client(Debugger);
                client.startClient(clientSocket, counter);
                //handleClinet client = new handleClinet(Debugger); //Transfer Debugger to handleClient
                //client.startClient(clientSocket, Convert.ToString(counter));
            }
        }
    }
    //Class to handle each client request separatly
    public class Client
    {
        private Debug Debugger { get; set; }

        private TcpClient ClientSocket { get; set; }
        private int ClientNumber { get; set; }
        private Thread ClientThread { get; set; }
        public Client(Debug DebuggerMain) //Constructor
        {
            this.Debugger = DebuggerMain;
            this.ClientSocket = null;
            this.ClientNumber = -1;
        }
        public void startClient(TcpClient CSocket, int CNumber)
        {
            this.ClientSocket = CSocket;
            this.ClientNumber = CNumber;
            ClientThread = new Thread(Connect);
            ClientThread.Start();
            Debugger.StatusMessage("OK", "Client " + ClientNumber + " created");
        }
        public void Connect()
        {
            Debugger.StatusMessage("OK", "Client " + ClientNumber + " connected");
            int requestCount = 0;
            byte[] FromClient = new byte[10024];
            string ClientData = null;
            byte[] FromServer = null;
            string ServerData = null;
            while(true)
            {
                try
                {
                    requestCount++;

                    NetworkStream networkStream = ClientSocket.GetStream();
                    networkStream.Read(FromClient, 0, (int)ClientSocket.ReceiveBufferSize);
                    ClientData = System.Text.Encoding.ASCII.GetString(FromClient);
                    ClientData = ClientData.Substring(0, ClientData.IndexOf("$"));
                    Debugger.StatusMessage("OK", "Client " + ClientNumber + " Received: " + ClientData);

                    ServerData = "Server to clinet(" + ClientNumber + ") " + requestCount;
                    FromServer = Encoding.ASCII.GetBytes(ServerData);
                    networkStream.Write(FromServer, 0, FromServer.Length);
                    networkStream.Flush();
                    Debugger.StatusMessage("OK", "Server response:" + ServerData);
                }
                catch
                {
                    ClientSocket.Close();
                    ClientThread.Abort();
                    Debugger.StatusMessage("OK", "Client " + ClientNumber + ": Thread closed");
                }
            }
        }
    }
    public class handleClinet
    {
        private static Debug Debugger { get; set; }

        TcpClient clientSocket;
        string clNo;
        public handleClinet(Debug DebuggerMain)
        {
            Debugger = DebuggerMain;
        }
        public void startClient(TcpClient inClientSocket, string clineNo)
        {
            this.clientSocket = inClientSocket;
            this.clNo = clineNo;
            Thread ctThread = new Thread(doChat);
            ctThread.Start();
        }
        private void doChat()
        {
            int requestCount = 0;
            byte[] bytesFrom = new byte[10025];
            string dataFromClient = null;
            Byte[] sendBytes = null;
            string serverResponse = null;
            string rCount = null;
            requestCount = 0;

            while (true)
            {
                try
                {
                    requestCount = requestCount + 1;
                    NetworkStream networkStream = clientSocket.GetStream();
                    networkStream.Read(bytesFrom, 0, (int)clientSocket.ReceiveBufferSize);
                    dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom);
                    dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("$"));
                    Console.WriteLine(" >> " + "From client-" + clNo + dataFromClient);

                    rCount = Convert.ToString(requestCount);
                    serverResponse = "Server to clinet(" + clNo + ") " + rCount;
                    sendBytes = Encoding.ASCII.GetBytes(serverResponse);
                    networkStream.Write(sendBytes, 0, sendBytes.Length);
                    networkStream.Flush();
                    Console.WriteLine(" >> " + serverResponse);
                }
                catch
                {
                    clientSocket.Close();
                    Debugger.StatusMessage("OK", "Client " + clNo + " connection closed");
                    Thread.CurrentThread.Abort();
                }
            }
        }
    }
}