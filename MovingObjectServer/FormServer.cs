using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace MovingObjectServer
{
    public partial class FormServer : Form
    {
        private List<Socket> clientSockets = new List<Socket>();
        private Socket serverSocket;
        private Thread listenThread;

        private int x = 50, y = 50;
        private System.Windows.Forms.Timer timer;

        public FormServer()
        {
            InitializeComponent();
            StartServer();

            timer = new System.Windows.Forms.Timer();
            timer.Interval = 100; // 100 ms
            timer.Tick += MoveObject;
            timer.Start();
        }

        private void StartServer()
        {
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(new IPEndPoint(IPAddress.Any, 9000));
            serverSocket.Listen(10);

            listenThread = new Thread(AcceptClients);
            listenThread.IsBackground = true;
            listenThread.Start();
        }

        private void AcceptClients()
        {
            while (true)
            {
                try
                {
                    Socket client = serverSocket.Accept();
                    clientSockets.Add(client);
                }
                catch { break; }
            }
        }

        private void MoveObject(object sender, EventArgs e)
        {
            // update posisi kotak
            x += 5;
            if (x > this.ClientSize.Width - 50) x = 0;

            // gambar ulang di form server
            this.Invalidate();

            // broadcast posisi ke semua client
            string data = $"{x},{y}";
            byte[] buffer = Encoding.ASCII.GetBytes(data);

            List<Socket> disconnected = new List<Socket>();

            foreach (var client in clientSockets)
            {
                try { client.Send(buffer); }
                catch { disconnected.Add(client); }
            }

            // buang client yg disconnect
            foreach (var dc in disconnected)
                clientSockets.Remove(dc);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.FillRectangle(Brushes.Blue, x, y, 50, 50);
        }
    }
}
