using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using System.Windows.Forms;
using Assignment1;

namespace UDPClientServer
{
    public class UDPController
    {
        private const int m_bufSize = 8 * 1024;
        private readonly Socket m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        private readonly byte[] m_buffer = new byte[m_bufSize];
        private readonly Queue<string> m_messages = new Queue<string>();
        private EndPoint m_epFrom = new IPEndPoint(IPAddress.Any, 0);
        private bool m_isServer = false;
        private Queue<string> m_sendQueue = new Queue<string>();
        private int m_messageCount { get; set; }
        private bool m_canSend { get; set; }
        private int m_sendRetry = Environment.TickCount;

        public void Server(string address, int port)
        {
            m_socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.ReuseAddress, true);
            m_socket.Bind(new IPEndPoint(IPAddress.Parse(address), port));
            m_isServer = true;
            m_messageCount = -1;
            Receive();
        }

        public void Client(string address, int port)
        {
            m_socket.Connect(IPAddress.Parse(address), port);
            m_messageCount = -1;
            m_canSend = true;
            Receive();
        }

        public void ProcessSendQueue()
        {
            if ((m_canSend) &&
                (m_sendQueue.Count > 0))
            {
                m_messageCount++;
                m_canSend = false;
                string s = m_sendQueue.Peek();
                byte[] data = Encoding.ASCII.GetBytes(m_messageCount.ToString() + ":" + s);
                m_socket.Send(data, data.Length, SocketFlags.None);
            }

            if (Environment.TickCount - m_sendRetry > 50)
            {
                m_sendRetry = Environment.TickCount;
                m_canSend = true;
            }
        }

        public void Send(string text)
        {
            Console.WriteLine($"Attempting to send message: {text}");
            m_sendQueue.Enqueue(text);
            ProcessSendQueue();

        }

      

        private void Receive()
        {
            m_socket.BeginReceiveFrom(m_buffer, 0, m_bufSize, SocketFlags.None, ref m_epFrom, new AsyncCallback(RecvCallback), null);
        }

        private void RecvCallback(IAsyncResult ar)
        {
            int bytes = m_socket.EndReceiveFrom(ar, ref m_epFrom);
            string message = Encoding.ASCII.GetString(m_buffer, 0, bytes);
            lock (m_messages)
            {
                m_messages.Enqueue(message);
            }
            string[] m = message.Split(':');
            if (m_isServer)
            {
                int index = int.Parse(m[0]);
                if (m_messageCount < index)
                {
                    string retMessage = m[0] + ":ACK";
                    m_socket.SendTo(Encoding.ASCII.GetBytes(retMessage), m_epFrom);
                    m_messageCount = index;
                }
            }
            else
            {
                if (m[0] == m_messageCount.ToString())
                {
                    m_sendQueue.Dequeue();
                    m_canSend = true;
                    m_sendRetry = 0;
                }
            }
            Receive();
        }

        public string GetNextMessage()
        {
            lock (m_messages)
            {
                if (m_messages.Count > 0)
                {
                    var message = m_messages.Dequeue();
                    Console.WriteLine($"Message dequeued: {message}");
                    return message;
                }
            }
            Console.WriteLine("No message in queue.");
            return string.Empty;
        }
    }
}