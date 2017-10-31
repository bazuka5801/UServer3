using System;
using System.Collections.Generic;
using SapphireEngine;

namespace RakNet.Network
{
    public abstract class Server : NetworkPeer
    {
        public string ip = "";

        public int port = 5678;

        public bool compressionEnabled;

        public bool logging;

        public Action<string, Connection> onDisconnected;

        
        public bool debug;

        internal uint lastValueGiven = 1024;

        public List<Connection> connections = new List<Connection>();

        private Dictionary<ulong, Connection> connectionByGUID = new Dictionary<ulong, Connection>();

        protected Server()
        {
        }

        public virtual void Cycle()
        {
        }

        public void SetEncryptionLevel(uint level)
        {
            for (int i = 0; i < connections.Count; i++)
            {
                connections[i].encryptionLevel = level;
                connections[i].encryptOutgoing = true;
            }
        }

        protected Connection FindConnection(ulong guid)
        {
            Connection connection;
            if (this.connectionByGUID.TryGetValue(guid, out connection))
            {
                return connection;
            }
            return null;
        }


        public abstract bool IsConnected();

        public abstract void Kick(Connection cn, string message);

        protected void OnDisconnected(string strReason, Connection cn)
        {
            if (cn == null)
            {
                return;
            }
            cn.connected = false;
            cn.active = false;
            if (this.onDisconnected != null)
            {
                this.onDisconnected(strReason, cn);
            }
            this.RemoveConnection(cn);
        }

        protected abstract void OnNewConnection();

        protected void OnNewConnection(Connection connection)
        {
            connection.connectionTime = DateTime.Now;
            this.connections.Add(connection);
            this.connectionByGUID.Add(connection.guid, connection);
        }

        protected void RemoveConnection(Connection connection)
        {
            this.connectionByGUID.Remove(connection.guid);
            this.connections.Remove(connection);
            connection.OnDisconnected();
        }

        public void Reset()
        {
            this.ResetUIDs();
        }

        internal void ResetUIDs()
        {
            this.lastValueGiven = 1024;
        }

        public void ReturnUID(uint uid)
        {
        }

        public abstract void SendUnconnected(uint netAddr, ushort netPort, byte[] steamResponseBuffer, int packetSize);

        public virtual bool Start()
        {
            return true;
        }

        public virtual void Stop(string shutdownMsg)
        {
        }

        public uint TakeUID()
        {
            if (this.lastValueGiven > 4294967263u)
            {
                ConsoleSystem.LogError(string.Concat("TakeUID - hitting ceiling limit!", this.lastValueGiven));
            }
            this.lastValueGiven++;
            return this.lastValueGiven;
        }
    }
}