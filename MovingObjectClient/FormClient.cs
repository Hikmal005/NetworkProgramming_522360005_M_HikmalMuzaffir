using System;
using System.Drawing;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace MovingObjectClient
{
    public partial class FormClient : Form
    {
        private Socket clientSocket;
        private Thread receiveThread;
        private int x = 50, y = 50;

        public FormClient()
        {
            InitializeComponent();
            ConnectToServer();
        }

        private void ConnectToServer()
        {
            try
            {
                clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                clientSocket.Connect("127.0.0.1", 9000); // connect ke server

                receiveThread = new Thread(ReceiveData);
                receiveThread.IsBackground = true;
                receiveThread.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal konek ke server: " + ex.Message);
            }
        }

        private void ReceiveData()
        {
            while (true)
            {
                try
                {
                    byte[] buffer = new byte[1024];
                    int received = clientSocket.Receive(buffer);
                    string data = Encoding.ASCII.GetString(buffer, 0, received);

                    string[] parts = data.Split(',');
                    if (parts.Length == 2)
                    {
                        x = int.Parse(parts[0]);
                        y = int.Parse(parts[1]);
                        this.Invalidate(); // refresh tampilan
                    }
                }
                catch
                {
                    break;
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.FillRectangle(Brushes.Blue, x, y, 50, 50);
        }
    }
}
