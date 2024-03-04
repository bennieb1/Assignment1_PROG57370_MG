
using UDPClientServer;

namespace Assignment1
{
    public partial class Form1 : Form
    {
        public UDPController UDP { get; }

        public Form1()
        {
            UDP = new UDPController();
        }

        public virtual bool UpdateList() { return false; }
        public virtual void ProcessSendQueue() { }
    }
}
