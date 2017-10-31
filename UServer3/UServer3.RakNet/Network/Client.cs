using System;

namespace RakNet.Network
{
    public abstract class Client : NetworkPeer
    {
        public static string disconnectReason;

        public string connectedAddress = "unset";

        public int connectedPort;

        public string ServerName;

        public bool IsOfficialServer;

        public Action<string> onDisconnected;


        public Connection Connection
        {
            get;
            protected set;
        }

        public bool ConnectionAccepted
        {
            get;
            protected set;
        }

        protected Client()
        {
        }

        public virtual bool Connect(string strURL, int port)
        {
            this.ConnectionAccepted = false;
            Client.disconnectReason = "Disconnected";
            return true;
        }

        public abstract void Cycle();

        public abstract void Disconnect(string reason, bool sendReasonToServer = true);


        public abstract bool IsConnected();

        protected void OnDisconnected(string str)
        {
            if (this.onDisconnected != null)
            {
                this.onDisconnected(str);
            }
        }
    }
}