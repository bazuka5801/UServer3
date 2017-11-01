using System;
using System.IO;
using ProtoBuf;
using SapphireEngine;
using SapphireEngine.Functions;
using RakNet.Network;
using UServer3.Environments;
using UServer3.Environments.Cryptography;
using UServer3.Rust;
using UServer3.Struct;

namespace UServer3.Network
{
    public class VirtualServer : SapphireType
    {
        public static Server BaseServer;
        public static Client BaseClient;

        public static UserInformation ConnectionInformation { get; private set; }
        public static bool IsHaveConnection => ConnectionInformation != null;
        
        public static UInt32 LastEntityNUM { get; private set; } = 0;
        public static UInt32 TakeEntityNUM => ++LastEntityNUM;
        
        public override void OnAwake()
        {
            this.InitializationNetwork();
            this.InitializationEAC();
            ConsoleSystem.Log("[VirtualServer]: Все службы готовы к работе!");
        }

        public override void OnUpdate()
        {
            this.CycleNetwork();
        }

        #region [Method] [Example] CycleNetwork

        private void CycleNetwork()
        {
            BaseClient.Cycle();
            BaseServer.Cycle();
        }

        #endregion

        #region [Method] [Example] InitializationEAC

        private void InitializationEAC()
        {
            try
            {
                ConsoleSystem.Log("[VirtualServer]: Служба EAC запускается...");
                EACServer.DoStartup();
                Timer.SetInterval(EACServer.DoUpdate, 1f);
                ConsoleSystem.Log("[VirtualServer]: Служба EAC успешно запущена!");
            }
            catch (Exception ex)
            {
                ConsoleSystem.LogError("[VirtualServer]: Исключение в InitializationEAC(): " + ex.Message);
            }
        }

        #endregion

        #region [Method] [Example] InitializationNetwork

        private void InitializationNetwork()
        {
            try
            {
                ConsoleSystem.Log("[VirtualServer]: Служба Network запускается...");
                BaseServer = new RakNet.Server
                {
                    ip = "0.0.0.0",
                    port = 28001,
                    OnRakNetPacket = OUT_OnRakNetMessage,
                    onMessage = OUT_OnNetworkMessage,
                    OnNewConnectionCallback = OnNewConnection,
                    onDisconnected = OUT_OnDisconnected
                };

                BaseClient = new RakNet.Client()
                {
                    onMessage = IN_OnNetworkMessage,
                    OnRakNetPacket = IN_OnRakNetMessage,
                    onDisconnected = IN_OnDisconnected
                };
                BaseServer.cryptography = new NetworkCryptographyServer();
                BaseClient.cryptography = new NetworkCryptographyServer();

                BaseServer.Start();
                ConsoleSystem.Log("[VirtualServer]: Служба Network успешно запущена!");
            }
            catch (Exception ex)
            {
                ConsoleSystem.LogError("[VirtualServer]: Исключение в InitializationNetwork(): " + ex.Message);
            }
        }

        #endregion

        #region [Method] [Example] OnNewConnection

        public void OnNewConnection(Connection connection)
        {
            try
            {
                if (BaseServer.connections.Count <= 1)
                {
                    ConsoleSystem.Log($"[VirtualServer]: Есть новое подключение [{BaseServer.connections[0].ipaddress}]");
                    ConsoleSystem.Log($"[VirtualServer]: Подключаемся к игровому серверу [{Settings.TargetServer_IP}:{Settings.TargetServer_Port}]");
                    if (BaseClient.Connect(Settings.TargetServer_IP, Settings.TargetServer_Port))
                    {

                        BaseClient.Connection.ipaddress = "127.0.0.1";
                        BaseClient.Connection.userid = Settings.Connection1_SteamID;
                        BaseClient.Connection.ownerid = Settings.Connection1_SteamID;
                        BaseClient.Connection.username = Settings.Connection1_Username;
                        BaseClient.Connection.authLevel = 1;
                        EACServer.OnJoinGame(BaseClient.Connection);
                        EACServer.OnStartLoading(BaseClient.Connection);
                        EACServer.OnFinishLoading(BaseClient.Connection);

                        BaseServer.connections[0].ipaddress = "127.0.0.1";
                        BaseServer.connections[0].userid = Settings.Connection2_SteamID;
                        BaseServer.connections[0].ownerid = Settings.Connection2_SteamID;
                        BaseServer.connections[0].username = Settings.Connection2_Username;
                        BaseServer.connections[0].authLevel = 1;
                        EACServer.OnJoinGame(BaseServer.connections[0]);
                        EACServer.OnStartLoading(BaseServer.connections[0]);
                        EACServer.OnFinishLoading(BaseServer.connections[0]);
                        ConsoleSystem.Log("[VirtualServer]: Инициализация подключения успешно завершена!");
                    }
                    else
                        ConsoleSystem.LogError($"[VirtualServer]: В попытке подключения отказано!");
                }
                else
                    ConsoleSystem.LogError($"[VirtualServer]: Уже есть одно подключение, больше подключений не может быть!");
            }
            catch (Exception ex)
            {
                ConsoleSystem.LogError("[VirtualServer]: Исключение в OnNewConnection(): " + ex.Message);
            }
        }

        #endregion

        #region [Method] [Example] OUT_OnDisconnected

        public void OUT_OnDisconnected(string reason, Connection conn)
        {
            if (BaseClient?.IsConnected() == true)
            {
                ConsoleSystem.LogWarning("[VirtualServer]: Соеденение с игровым клиентом разорвано: " + reason);
                BaseClient?.Disconnect(reason, false);
                if (BaseClient != null & BaseClient.Connection != null)
                {
                    BaseClient.Connection.decryptIncoming = false;
                    BaseClient.Connection.encryptOutgoing = false;
                }
                NetworkManager.Instance.OnDisconnected();
                
                ConnectionInformation = null;
                LastEntityNUM = 0;
            }
            EACServer.OnLeaveGame(conn);
        }

        #endregion

        #region [Method] [Example] IN_OnDisconnected

        public void IN_OnDisconnected(string reason)
        {
            if (BaseServer != null && BaseServer.connections.Count > 0)
            {
                BaseServer?.Kick(BaseServer.connections[0], reason);
                ConsoleSystem.LogWarning("[VirtualServer]: Соеденение с игровым сервером разорвано: " + reason);
            }
        }

        #endregion

        #region [Method] [Example] OUT_OnNetworkMessage

        public void OUT_OnNetworkMessage(Message packet)
        {
            switch (packet.type)
            {
                case Message.Type.Ready:
                    packet.connection.decryptIncoming = true;
                    SendPacket(BaseClient, packet);
                    BaseClient.Connection.encryptOutgoing = true;
                    return;
                case Message.Type.GiveUserInformation:
                    ConnectionInformation = UserInformation.ParsePacket(packet);
                    SendPacket(BaseClient, packet);
                    break;
                case Message.Type.EAC:
                    EACServer.OnMessageReceived(packet);
                    SendPacket(BaseClient, packet);
                    break;
                default:
                    if (NetworkManager.Instance.Out_NetworkMessage(packet) == false)
                        SendPacket(BaseClient, packet);
                    break;
            }
        }

        #endregion

        #region [Method] [Example] IN_OnNetworkMessage

        public void IN_OnNetworkMessage(Message packet)
        {
            switch (packet.type)
            {
                case Message.Type.Approved:
                    OnApproved(packet);
                    break;
                case Message.Type.EAC:
                    EACServer.OnMessageReceived(packet);
                    SendPacket(BaseServer, packet);
                    break;
                case Message.Type.DisconnectReason:
                    SendPacket(BaseServer, packet);
                    if (BaseServer != null && BaseServer.connections.Count > 0)
                    {
                        packet.read.Position = 1L;
                        string reasone = packet.read.String();
                        BaseServer?.Kick(BaseServer.connections[0], reasone);
                        ConsoleSystem.LogWarning("[VirtualServer]: От игрового сервера получена причина дисконнекта: " + reasone);
                    }
                    break;
                case Message.Type.Entities:
                    packet.read.UInt32();
                    using (Entity entity = Entity.Deserialize(packet.read))
                    {
                        if (EntityManager.OnEntity(entity) == false)
                        {
                            if (BaseServer.write.Start())
                            {
                                BaseServer.write.PacketID(Message.Type.Entities);
                                BaseServer.write.UInt32(TakeEntityNUM);
                                entity.WriteToStream(BaseServer.write);
                                BaseServer.write.Send(new SendInfo(BaseServer.connections[0]));
                            }
                        }
                    }
                    break;
                case Message.Type.EntityDestroy:
                    EntityManager.OnEntityDestroy(packet);
                    SendPacket(BaseServer, packet);
                    break;
                case Message.Type.EntityPosition:
                    EntityManager.OnEntityPosition(packet);
                    SendPacket(BaseServer, packet);
                    break;
                default:
                    if (NetworkManager.Instance.IN_NetworkMessage(packet) == false)
                        SendPacket(BaseServer, packet);
                    break;
            }
        }

        #endregion

        #region [Method] [Example] OnApproved

        private void OnApproved(Message packet)
        {
            try
            {
                using (Approval approval = Approval.Deserialize(packet.read))
                {
                    ConsoleSystem.LogWarning($"[VirtualServer]: Вы подключились к: {(approval.official ? "[Oficial] " : "")}" + approval.hostname);

                    BaseClient.Connection.encryptionLevel = approval.encryption;
                    BaseClient.Connection.decryptIncoming = true;

                    approval.encryption = Settings.GameClient_EncryptionLevel;

                    if (BaseServer.write.Start())
                    {
                        BaseServer.write.PacketID(Message.Type.Approved);
                        Approval.Serialize(BaseServer.write, approval);
                        BaseServer.Send();
                    }
                    BaseServer.SetEncryptionLevel(approval.encryption);
                }
            }
            catch (Exception ex)
            {
                ConsoleSystem.LogError("[VirtualServer]: Исключение в OnApproved(): " + ex.Message);
            }
        }

        #endregion

        #region [Methods] [Example] RakNet Unconnected

        public void OUT_OnRakNetMessage(byte uid)
        {
            if (BaseClient == null || !BaseClient.IsConnected()) return;
            if (BaseClient.write.Start())
            {
                BaseClient.write.UInt8(uid);
                BaseClient.Send();
            }
        }

        public void IN_OnRakNetMessage(byte uid)
        {
            if (BaseServer.write.Start())
            {
                BaseServer.write.UInt8(uid);
                BaseServer.Send();
            }
        }

        #endregion

        #region [Method] [Example] SendPacket

        public byte[] GetPacketBytes(Message message)
        {
            byte[] buffer = null;
            long start_pos = message.read.Position;
            message.peer.read.Position = 0L;
            using (BinaryReader br = new BinaryReader(message.peer.read))
            {
                buffer = br.ReadBytes((int) message.peer.read.Length);
            }
            message.read.Position = start_pos;
            return buffer;
        }

        public void SendPacket(NetworkPeer peer, Message message)
        {
            message.peer.read.Position = 0L;
            using (BinaryReader br = new BinaryReader(message.peer.read))
            {
                peer.write.Start();
                peer.write.Write(br.ReadBytes((int) message.peer.read.Length), 0, (int) message.peer.read.Length);
                peer.write.Send(new SendInfo(peer is RakNet.Client ? BaseClient.Connection : BaseServer.connections[0]));
            }
        }

        public void SendPacket(NetworkPeer peer, byte[] message)
        {
            peer.write.Start();
            peer.write.Write(message, 0, (int) message.Length);
            peer.write.Send(new SendInfo(peer is RakNet.Client ? BaseClient.Connection : BaseServer.connections[0]));
        }

        #endregion
    }
}