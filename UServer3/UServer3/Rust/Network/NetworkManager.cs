using System;
using RakNet.Network;
using SapphireEngine;
using UServer3.CSharp.Reflection;
using UServer3.Rust;
using UServer3.Rust.Data;
using UServer3.Rust.Struct;

namespace UServer3.Rust.Network
{
    public class NetworkManager : SapphireType
    {
        public static NetworkManager Instance;
        
        public override void OnAwake() => Instance = this;

        public bool IN_NetworkMessage(Message message)
        {
            switch (message.type)
            {
                case Message.Type.RPCMessage:
                    return OnRPCMessage(ERPCNetworkType.IN, message);
                    break;
            }
            return false;
        }

        public bool Out_NetworkMessage(Message message)
        {
            switch (message.type)
            {
                case Message.Type.Tick:
                    if (BasePlayer.IsHaveLocalPlayer)
                        return BasePlayer.LocalPlayer.OnTick(message);
                    else return false;
                    break;
                case Message.Type.RPCMessage:
                    return OnRPCMessage(ERPCNetworkType.OUT, message);
                    break;
            }
            return false;
        }

        private static bool OnRPCMessage(ERPCNetworkType type, Message message)
        {
            UInt32 UID = message.read.EntityID();
            UInt32 rpcId = message.read.UInt32();
            if (type == ERPCNetworkType.IN)
                message.read.UInt64();
            return RPCManager.RunRPCMethod(UID, (ERPCMethodUID) rpcId, type, message);
        }

        public void OnDisconnected()
        {
            BaseNetworkable.DestroyAll();
        }
    }
}