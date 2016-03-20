using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using Blue_Debugger;
using ClientSocketDLL;

namespace ClientSocketNew
{
    public partial class Form1 : Form
    {
        private Debug Debugger;
        private ClientConnection client;
        System.Net.Sockets.TcpClient clientSocket = new System.Net.Sockets.TcpClient();
        NetworkStream serverStream;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Debugger = new Debug("client_log.txt");
            client = new ClientConnection("localhost", 8888);
            msg("Client Started");
            //clientSocket.Connect("127.0.0.1", 8888);
            label1.Text = "Client Socket Program - Server Connected ...";
        }
        private void Form1_Close(object sender, System.ComponentModel.CancelEventArgs e)
        {
            client.Disconnect();
            Environment.Exit(0);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                for(int i = 0; i < 100; i++)
                {
                    client.Send("Message from Client");
                    Debugger.StatusMessage("OK", "Sended data to Server",true);
                    msg("Sended data to Server");
                    string fromServer = client.Receive();
                    if (fromServer != null)
                    {
                        Debugger.StatusMessage("OK", "Server response: " + fromServer,true);
                        msg("Data from Server: " + fromServer);
                    }
                    else
                    {
                        Debugger.StatusMessage("FAIL", "No Server response",true);
                        msg("No Server response");
                    }
                }
                /*while(true)
                {
                    NetworkStream serverStream = clientSocket.GetStream();
                    byte[] outStream = System.Text.Encoding.ASCII.GetBytes("Message from Client$");
                    serverStream.Write(outStream, 0, outStream.Length);
                    serverStream.Flush();

                    byte[] inStream = new byte[10025];
                    serverStream.Read(inStream, 0, (int)clientSocket.ReceiveBufferSize);
                    string returndata = System.Text.Encoding.ASCII.GetString(inStream);
                    msg("Data from Server : " + returndata);
                }*/
            }
            catch
            {
                Debugger.StatusMessage("FAIL", "Couldn't send xor receive. Server offline?",true);
                msg("Error: Server offline?");
            }
        }

        public void msg(string mesg)
        {
            textBox1.Text = textBox1.Text + Environment.NewLine + " >> " + mesg;
        }
    }
}
