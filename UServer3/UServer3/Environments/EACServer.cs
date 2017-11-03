using System;
using System.Collections.Generic;
using System.IO;
using EasyAntiCheat.Server;
using EasyAntiCheat.Server.Hydra;
using RakNet.Network;
using SapphireEngine;
using Client = EasyAntiCheat.Server.Hydra.Client;

namespace UServer3.Environments
{
    public class EACServer
    {
        private static Dictionary<Client, Connection> client2connection;

        private static Dictionary<Connection, Client> connection2client;

        private static Dictionary<Connection, ClientStatus> connection2status;

        private static EasyAntiCheatServer<Client> easyAntiCheat;


        static EACServer()
        {
            client2connection = new Dictionary<Client, Connection>();
            connection2client = new Dictionary<Connection, Client>();
            connection2status = new Dictionary<Connection, ClientStatus>();
            easyAntiCheat = null;
        }

        public static void Decrypt(Connection connection, MemoryStream src, int srcOffset, MemoryStream dst, int dstOffset)
        {
            easyAntiCheat.NetProtect.UnprotectMessage(GetClient(connection), src, (long)srcOffset, dst, (long)dstOffset);
        }

        public static void DoShutdown()
        {
            client2connection.Clear();
            connection2client.Clear();
            connection2status.Clear();
            
            if (easyAntiCheat != null)
            {
                ConsoleSystem.Log("EasyAntiCheat Server Shutting Down");
                easyAntiCheat.Dispose();
                easyAntiCheat = null;
            }
        }

        public static void DoStartup()
        {
            client2connection.Clear();
            connection2client.Clear();
            connection2status.Clear();
            StreamWriter streamWriter = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "/Logs/Log.EAC.txt", true)
            {
                AutoFlush = true
            };
            Log.SetOut(streamWriter);
            Log.Prefix = string.Empty;
            Log.Level = LogLevel.Info;
            easyAntiCheat = new EasyAntiCheatServer<Client>(HandleClientUpdate, 20, "My Server Name");
            UServer3.CSharp.ExtensionMethods.ConsoleEx.ClearCurrentConsoleLine();
        }

        public static void DoUpdate() => easyAntiCheat?.HandleClientUpdates();

        public static void Encrypt(Connection connection, MemoryStream src, int srcOffset, MemoryStream dst, int dstOffset)
        {
            easyAntiCheat.NetProtect.ProtectMessage(GetClient(connection), src, (long)srcOffset, dst, (long)dstOffset);
        }

        public static Client GetClient(Connection connection)
        {
            Client client;
            connection2client.TryGetValue(connection, out client);
            return client;
        }

        public static Connection GetConnection(Client client)
        {
            Connection connection;
            client2connection.TryGetValue(client, out connection);
            return connection;
        }

        private static void HandleClientUpdate(ClientStatusUpdate<Client> clientStatus)
        {

                Client clientObject = clientStatus.ClientObject;
                Connection connection = GetConnection(clientObject);
                if (connection == null)
                {
                    ConsoleSystem.LogError(string.Concat("EAC status update for invalid client: ", clientObject.ClientID));
                }
                else if (!ShouldIgnore(connection))
                {
                    if (clientStatus.RequiresKick)
                    {
                    }
                    else if (clientStatus.Status == ClientStatus.ClientAuthenticatedLocal)
                    {
                        OnAuthenticatedLocal(connection);
                        easyAntiCheat.SetClientNetworkState(clientObject, false);
                    }
                    else if (clientStatus.Status == ClientStatus.ClientAuthenticatedRemote)
                    {
                        OnAuthenticatedRemote(connection);
                    }
                    OnAuthenticatedLocal(connection);
                    easyAntiCheat.SetClientNetworkState(clientObject, false);
                }
            
        }

        public static bool IsAuthenticated(Connection connection)
        {
            ClientStatus clientStatu;
            connection2status.TryGetValue(connection, out clientStatu);
            return clientStatu == ClientStatus.ClientAuthenticatedRemote;
        }

        private static void OnAuthenticatedLocal(Connection connection)
        {
            if (connection.authStatus == string.Empty)
            {
                connection.authStatus = "ok";
            }
            connection2status[connection] = ClientStatus.ClientAuthenticatedLocal;
        }

        private static void OnAuthenticatedRemote(Connection connection)
        {
            connection2status[connection] = ClientStatus.ClientAuthenticatedRemote;
        }

        public static void OnFinishLoading(Connection connection)
        {
            if (easyAntiCheat != null)
            {
                Client client = GetClient(connection);
                easyAntiCheat.SetClientNetworkState(client, true);
            }
        }

        public static void OnJoinGame(Connection connection)
        {
            if (easyAntiCheat == null)
            {
                OnAuthenticatedLocal(connection);
                OnAuthenticatedRemote(connection);
            }
            else
            {
                Client client = easyAntiCheat.GenerateCompatibilityClient();
                easyAntiCheat.RegisterClient(client, connection.userid.ToString(), "127.0.0.1", Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), (connection.authLevel <= 0 ? PlayerRegisterFlags.PlayerRegisterFlagNone : PlayerRegisterFlags.PlayerRegisterFlagAdmin));
                client2connection.Add(client, connection);
                connection2client.Add(connection, client);
                connection2status.Add(connection, ClientStatus.ClientDisconnected);
                if (ShouldIgnore(connection))
                {
                    OnAuthenticatedLocal(connection);
                    OnAuthenticatedRemote(connection);
                }
            }
        }

        public static void OnLeaveGame(Connection connection)
        {
            if (easyAntiCheat != null)
            {
                Client client = GetClient(connection);
                easyAntiCheat.UnregisterClient(client);
                client2connection.Remove(client);
                connection2client.Remove(connection);
                connection2status.Remove(connection);
            }
        }

        public static void OnMessageReceived(Message message)
        {
            if (!connection2client.ContainsKey(message.connection))
            {
                ConsoleSystem.LogError(string.Concat("EAC network packet from invalid connection: ", message.connection.userid));
                return;
            }
            Client client = GetClient(message.connection);
            MemoryStream memoryStream = message.read.MemoryStreamWithSize();
            easyAntiCheat.PushNetworkMessage(client, memoryStream.GetBuffer(), (int)memoryStream.Length);
        }

        public static void OnStartLoading(Connection connection)
        {
            if (easyAntiCheat != null)
            {
                Client client = GetClient(connection);
                easyAntiCheat.SetClientNetworkState(client, false);
            }
        }

        private static void SendToClient(Client client, byte[] message, int messageLength)
        {
            Connection connection = GetConnection(client);
            if (connection == null)
            {
                ConsoleSystem.LogError(string.Concat("EAC network packet for invalid client: ", client.ClientID));
                return;
            }
        }

        public static bool ShouldIgnore(Connection connection) => false;

    }
}