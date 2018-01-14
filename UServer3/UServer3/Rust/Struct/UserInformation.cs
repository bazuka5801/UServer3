using System;
using System.IO;
using RakNet.Network;
using SapphireEngine;

namespace UServer3.Rust.Struct
{
    public class UserInformation
    {
        public Byte PacketProtocol;
        public UInt64 SteamID;
        public UInt32 ConnectionProtocol;
        public String OS;
        public String Username;
        public String Branch;
        public Byte[] SteamToken;
        public Byte[] PacketBuffer;

        public static UserInformation ParsePacket(Message message)
        {
            try
            {
                message.read.Position = 1;
                var userInformation = new UserInformation();
                userInformation.PacketProtocol = message.read.UInt8();
                userInformation.SteamID = message.read.UInt64();
                userInformation.ConnectionProtocol = message.read.UInt32();
                userInformation.OS = message.read.String();
                userInformation.Username = message.read.String();
                userInformation.Branch = message.read.String();
                userInformation.SteamToken = message.read.BytesWithSize();
                
                message.peer.read.Position = 0L;
                using (BinaryReader br = new BinaryReader(message.peer.read))
                    userInformation.PacketBuffer = br.ReadBytes((int) message.peer.read.Length);
                
                
                return userInformation;
            }
            catch (Exception ex)
            {
                ConsoleSystem.LogError("Error to Struct.UserInformation.ParsePacket(): " + ex.Message);
            }
            return default(UserInformation);
        }

        public void Write(NetworkPeer peer)
        {
            peer.write.UInt8(PacketProtocol);
            peer.write.UInt64(SteamID);
            peer.write.UInt32(ConnectionProtocol);
            peer.write.String(OS);
            peer.write.String(Username);
            peer.write.String(Branch);
            peer.write.BytesWithSize(SteamToken);
        }
    }
}