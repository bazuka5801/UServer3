using System;
using System.Diagnostics;
using RakNet.Network;
using Facepunch;
using SapphireEngine;

namespace RakNet
{
    public class Server : RakNet.Network.Server
    {
        public static float MaxReceiveTime;

        public static ulong MaxPacketsPerSecond;

        private Peer peer;

        public Action<Connection> OnNewConnectionCallback;
        
        public Action<byte> OnRakNetPacket;
        
        static Server()
        {
            Server.MaxReceiveTime = 20f;
            Server.MaxPacketsPerSecond = (ulong)1500;
        }

        public Server()
        {
        }

        internal void ConnectedPacket(Connection connection)
        {
            if (connection.GetPacketsPerSecond(0) >= Server.MaxPacketsPerSecond)
            {
                this.Kick(connection, "Kicked: Packet Flooding");
                ConsoleSystem.LogWarning(string.Concat(connection.ToString(), " was kicked for packet flooding"));
                return;
            }
            connection.AddPacketsPerSecond(0);
            this.read.Start(connection);
            byte num = this.read.PacketID();
            if (this.HandleRaknetPacket(num, connection))
            {
                return;
            }
            num = (byte)(num - 140);
            Message message = base.StartMessage((Message.Type)num, connection);
            if (this.onMessage != null)
            {
                this.onMessage(message);
            }
            message.Clear();
            Pool.Free(ref message);
        }

        public override void Cycle()
        {
            base.Cycle();
            if (!this.IsConnected())
            {
                return;
            }
            Stopwatch stopwatch = Pool.Get<Stopwatch>();
            stopwatch.Reset();
            stopwatch.Start();
            do
            {
                if (!this.peer.Receive())
                {
                    break;
                }
                Connection connection = base.FindConnection(this.peer.incomingGUID);
                if (connection != null)
                {
                    using (TimeWarning timeWarning = TimeWarning.New("ConnectedPacket", (long)20))
                    {
                        this.ConnectedPacket(connection);
                    }
                }
                else
                {
                    using (TimeWarning timeWarning = TimeWarning.New("UnconnectedPacket", (long)20))
                    {
                        this.UnconnectedPacket();
                    }
                }
            }
            while (stopwatch.Elapsed.TotalMilliseconds <= (double)Server.MaxReceiveTime);
            Pool.Free<Stopwatch>(ref stopwatch);
        }



        internal bool HandleRaknetPacket(byte type, Connection connection)
        {
            if (type >= 140)
            {
                return false;
            }
            OnRakNetPacket?.Invoke(type);
            switch (type)
            {
                case 19:
                {
                    using (TimeWarning timeWarning = TimeWarning.New("OnNewConnection", (long)20))
                    {
                        this.OnNewConnection();
                    }
                    return true;
                }
                case 20:
                {
                    return true;
                }
                case 21:
                {
                    if (connection != null)
                    {
                        using (TimeWarning timeWarning = TimeWarning.New("OnDisconnected", (long)20))
                        {
                            base.OnDisconnected("Disconnected", connection);
                        }
                    }
                    return true;
                }
                case 22:
                {
                    if (connection != null)
                    {
                        using (TimeWarning timeWarning = TimeWarning.New("OnDisconnected (timed out)", (long)20))
                        {
                            base.OnDisconnected("Timed Out", connection);
                        }
                    }
                    return true;
                }
                default:
                {
                    return true;
                }
            }
        }

        public override bool IsConnected()
        {
            return this.peer != null;
        }

        public override void Kick(Connection cn, string message)
        {
            if (this.peer == null)
            {
                return;
            }
            if (this.write.Start())
            {
                this.write.PacketID(Message.Type.DisconnectReason);
                this.write.String(message);
                Write write = this.write;
                SendInfo sendInfo = new SendInfo(cn)
                {
                    method = SendMethod.ReliableUnordered,
                    priority = Priority.Immediate
                };
                write.Send(sendInfo);
            }
            ConsoleSystem.Log(string.Concat(cn.ToString(), " kicked: ", message));
            this.peer.Kick(cn);
            base.OnDisconnected(string.Concat("Kicked: ", message), cn);
        }

        protected override void OnNewConnection()
        {
            Connection connection = new Connection()
            {
                guid = this.peer.incomingGUID,
                ipaddress = this.peer.incomingAddress,
                active = true
            };
            base.OnNewConnection(connection);
            OnNewConnectionCallback?.Invoke(connection);
        }


        public override unsafe void SendUnconnected(uint netAddr, ushort netPort, byte[] data, int size)
        {
            fixed (byte* ptr = data)
            {
                this.peer.SendUnconnectedMessage(ptr, size, netAddr, netPort);
            }
        }

        public override bool Start()
        {
            this.peer = Peer.CreateServer(this.ip, this.port, 1024);
            if (this.peer == null)
            {
                return false;
            }
            this.write = new StreamWrite(this, this.peer);
            this.read = new StreamRead(this, this.peer);
            return true;
        }

        public override void Stop(string shutdownMsg)
        {
            if (this.peer == null)
            {
                return;
            }
            ConsoleSystem.Log(string.Concat("[Raknet] Server Shutting Down (", shutdownMsg, ")"));
            (this.write as StreamWrite).Shutdown();
            (this.read as StreamRead).Shutdown();
            using (TimeWarning timeWarning = TimeWarning.New("ServerStop", 0.1f))
            {
                this.peer.Close();
                this.peer = null;
                base.Stop(shutdownMsg);
            }
        }

        internal void UnconnectedPacket()
        {
            byte num = this.peer.ReadByte();
            if (this.onUnconnectedMessage != null && this.onUnconnectedMessage(num, this.read, this.peer.incomingAddressInt, (int)this.peer.incomingPort))
            {
                return;
            }
            this.HandleRaknetPacket(num, null);
        }


        public override void Send()
        {
            write.Send(new SendInfo(connections));
        }
    }
}