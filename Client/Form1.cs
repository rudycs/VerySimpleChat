using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace Client
{
    public partial class Form1 : Form
    {
        private TcpClient TcpClient { get; set; }

        public Form1()
        {
            InitializeComponent();

            txtChat.KeyDown += txtChat_KeyDown;
            FormClosing += Form1_FormClosing;

            TcpClient = null;
        }

        private void txtChat_KeyDown(object sender, KeyEventArgs e)
        {
            // on enter key pressed, send text message to server
            if (e.KeyCode == Keys.Enter)
            {
                var writer = new StreamWriter(TcpClient.GetStream());
                writer.WriteLine(txtChat.Text);
                writer.Flush(); // ensure the buffer is empty

                txtChat.Text = string.Empty;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (TcpClient != null)
            {
                TcpClient.Close();
            }
            Environment.Exit(0);
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            btnConnect.Enabled = txtServerAddress.Enabled = txtServerPort.Enabled = false;
            try
            {
                int port = int.Parse(txtServerPort.Text);
                TcpClient = new TcpClient();
                TcpClient.Connect(txtServerAddress.Text, port);
                var chatThread = new Thread(new ThreadStart(ConnectToServer));
                chatThread.Start();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnConnect.Enabled = txtServerAddress.Enabled = txtServerPort.Enabled = true;
            }
        }

        private void ConnectToServer()
        {
            var reader = new StreamReader(TcpClient.GetStream());
            while (true)
            {
                string text = reader.ReadLine();
                AppendText(text);
            }
        }

        private void AppendText(string text)
        {
            if (ricMessage.InvokeRequired) // call from non UI thread
            {
                Invoke(new MethodInvoker(delegate() { AppendText(text); }));
            }
            else
            {
                ricMessage.AppendText(text + "\n");
                //place the cursor at the end of the text in the textbox for typing our messages
                ricMessage.SelectionStart = ricMessage.Text.Length;
            }
        }
    }
}
