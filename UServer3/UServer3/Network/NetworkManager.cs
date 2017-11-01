using System;
using RakNet.Network;
using SapphireEngine;
using UServer2.Struct;
using UServer3.Reflection;

namespace UServer3.Network
{
    public class NetworkManager : SapphireType
    {
        public static NetworkManager Instance;

        public UserInformation ConnectionInfromation;
        
        public override void OnAwake() => Instance = this;

        public bool IN_NetworkMessage(Message message)
        {
            switch (message.type)
            {
                case Message.Type.RPCMessage:
                    return OnRPCMessage(OpCodes.ERPCNetworkType.IN, message);
                    break;
            }
            return false;
        }

        public bool Out_NetworkMessage(Message message)
        {
            switch (message.type)
            {
                case Message.Type.RPCMessage:
                    return OnRPCMessage(OpCodes.ERPCNetworkType.OUT, message);
                    break;
            }
            return false;
        }

        private static bool OnRPCMessage(OpCodes.ERPCNetworkType type, Message message)
        {
            UInt32 UID = message.read.EntityID();
            UInt32 rpcId = message.read.UInt32();
            //TODO: Optimize Enum.IsDefined
            if (!Enum.IsDefined(typeof(OpCodes.ERPCMethodUID), rpcId)) return false;
            return RPCManager.RunRPCMethod(UID, (OpCodes.ERPCMethodUID) rpcId, type, message);
        }

        public void OnDisconnected()
        {
            
        }
    }
}