using System;
using System.Diagnostics;
using RakNet;
using RakNet.Network;
using Facepunch;
using SapphireEngine;

namespace RakNet
{
    public class Client : Network.Client
    {
        public static float MaxReceiveTime;

        public const string DemoHeader = "RUST DEMO FORMAT";

        private Peer peer;

        public static byte[] ReusableBytes;

        private Stopwatch cycleTimer = Stopwatch.StartNew();

        public Action<byte> OnRakNetPacket;
        
        
        static Client()
        {
            MaxReceiveTime = 20f;
            ReusableBytes = new byte[1048576];
        }

        public Client()
        {
        }
        
        public override void Send()
        {
            write.Send(new SendInfo(Connection));
        }


        public override bool Connect(string strURL, int port)
        {
            base.Connect(strURL, port);
            this.peer = Peer.CreateConnection(strURL, port, 12, 400, 0);
            if (this.peer == null)
            {
                return false;
            }
            this.write = new StreamWrite(this, this.peer);
            this.read = new StreamRead(this, this.peer);
            this.connectedAddress = strURL;
            this.connectedPort = port;
            this.ServerName = "";
            base.Connection = new Connection();
            return true;
        }

        public override void Cycle()
        {
            using (TimeWarning.New("Raknet.Client.Cycle", 0.1f))
            {
                if (this.IsConnected())
                {
                    this.cycleTimer.Reset();
                    this.cycleTimer.Start();
                    while (this.peer.Receive())
                    {
                        using (TimeWarning.New("HandleMessage", 0.1f))
                        {
                            this.HandleMessage();
                        }
                        if (this.cycleTimer.Elapsed.TotalMilliseconds > (double) Client.MaxReceiveTime)
                        {
                            break;
                        }
                        if (!this.IsConnected())
                        {
                            break;
                        }
                    }
                }
            }
        }

        public override void Disconnect(string reason, bool sendReasonToServer)
        {
            if (sendReasonToServer && this.write != null && this.write.Start())
            {
                this.write.PacketID(Message.Type.DisconnectReason);
                this.write.String(reason);
                this.write.Send(new SendInfo(base.Connection)
                {
                    method = SendMethod.ReliableUnordered,
                    priority = Priority.Immediate
                });
            }
            if (this.peer != null)
            {
                this.peer.Close();
                this.peer = null;
            }
            this.write = null;
            this.read = null;
            this.connectedAddress = "";
            this.connectedPort = 0;
            base.Connection = null;
            base.OnDisconnected(reason);
        }


        
        protected void HandleMessage()
        {
            this.read.Start(base.Connection);
            byte b = this.read.PacketID();
            if (this.HandleRaknetPacket(b))
            {
                return;
            }
            b -= 140;
            if (b > 22)
            {
                ConsoleSystem.LogWarning("Invalid Packet (higher than " + Message.Type.EAC + ")");
                this.Disconnect(string.Concat(new object[]
                {
                    "Invalid Packet (",
                    b,
                    ") ",
                    this.peer.incomingBytes,
                    "b"
                }), true);
                return;
            }
            Message message = base.StartMessage((Message.Type)b, base.Connection);
            if (this.onMessage != null)
            {
                try
                {
                    using (TimeWarning.New("onMessage", 0.1f))
                    {
                        this.onMessage(message);
                    }
                }
                catch (Exception ex)
                {
                    ConsoleSystem.LogError(ex.Message);
                }
            }
            message.Clear();
            Pool.Free<Message>(ref message);
        }


        internal bool HandleRaknetPacket(byte type)
        {
            if (type >= 140)
            {
                return false;
            }
            OnRakNetPacket?.Invoke(type);
            switch (type)
            {
                case 16:
                    base.ConnectionAccepted = true;
                    if (base.Connection.guid != 0uL)
                    {
                        Console.WriteLine("Multiple PacketType.CONNECTION_REQUEST_ACCEPTED");
                    }
                    base.Connection.guid = this.peer.incomingGUID;
                    return true;
                case 17:
                    this.Disconnect("Connection Attempt Failed", false);
                    return true;
                case 20:
                    this.Disconnect("Server is Full", false);
                    return true;
                case 21:
                    if (base.Connection != null && base.Connection.guid != this.peer.incomingGUID)
                    {
                        return true;
                    }
                    this.Disconnect(Client.disconnectReason, false);
                    return true;
                case 22:
                    if (base.Connection == null && base.Connection.guid != this.peer.incomingGUID)
                    {
                        return true;
                    }
                    this.Disconnect("Timed Out", false);
                    return true;
                case 23:
                    if (base.Connection == null && base.Connection.guid != this.peer.incomingGUID)
                    {
                        return true;
                    }
                    this.Disconnect("Connection Banned", false);
                    return true;
            }
            if (base.Connection != null && this.peer.incomingGUID != base.Connection.guid)
            {
                ConsoleSystem.LogWarning(string.Concat(new object[]
                {
                    "[CLIENT] Unhandled Raknet packet ",
                    type,
                    " from unknown source ",
                    this.peer.incomingAddress
                }));
                return true;
            }
            ConsoleSystem.LogWarning("Unhandled Raknet packet " + type);
            return true;
        }

        public override bool IsConnected()
        {
            return peer != null;
        }
    }}